using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class CancelaVendaController : ApiControllerBase
    {
        public string terminal,
                      empresa,
                      matricula,
                      strMessage,
                      retorno;

        public long titularidadeFinal;
        
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var nsu = Request.GetQueryStringValue("nsu");

            terminal = userLoggedName.PadLeft(8, '0');

            var dtNow = DateTime.Now;

            var q = (from e in db.LOG_Transacoes

                     where e.fk_loja == db.currentUser.i_unique
                     where e.nu_nsu.ToString() == nsu.ToString()

                     where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada.ToString()

                     where e.dt_transacao.Value.Year == dtNow.Year
                     where e.dt_transacao.Value.Month == dtNow.Month
                     where e.dt_transacao.Value.Day == dtNow.Day

                     orderby e.dt_transacao descending

                     select e);

            var lTr = q.FirstOrDefault();

            if (lTr == null)
                return BadRequest("NSU Inválido");

            var cart = (from e in db.T_Cartao
                        where e.i_unique == lTr.fk_cartao
                        select e).
                        FirstOrDefault();

            var prop = (from e in db.T_Proprietario
                        where e.i_unique == cart.fk_dadosProprietario
                        select e).
                        FirstOrDefault();

            empresa = cart.st_empresa;
            matricula = cart.st_matricula;
            
            var sc = new SocketConvey();

            var sck = sc.connectSocket(cnet_server, cnet_port);

            strMessage = MontaCancelamento( cart.st_titularidade, nsu);

            if (!sc.socketEnvia(sck, strMessage))
            {
                sck.Close();
                return BadRequest("Falha de comunicação (0x2)");
            }

            retorno = sc.socketRecebe(sck);

            if (retorno.Length < 6)
            {
                sck.Close();
                return BadRequest("Falha de comunicação (0x3)");
            }

            var codResp = retorno.Substring(2, 4);

            sck.Close();

            if (codResp != "0000")
            {
                return BadRequest("Falha (0xE" + codResp + " - " + retorno.Substring(73, 20) + " )");
            }

            CleanCache(db, CacheTags.associado, (long) cart.i_unique);

            var cupom = new Cupom().
                Cancelamento ( db, 
                               cart, 
                               ObtemNsuRetorno(retorno),
                               terminal, 
                               nsu,
                               lTr, 
                               prop);
            
            return Ok(new
            {
                count = 1,
                results = cupom
            });
        }

        public string MontaCancelamento(string titularidade, string nsu)
        {
            var reg = "09";

            reg += "CECA";
            reg += terminal.PadRight(8, ' ');
            reg += "000000";
            reg += empresa.PadLeft(6, '0');
            reg += matricula.PadLeft(6, '0');
            reg += titularidade.ToString().PadLeft(2, '0');
            reg += "       ";
            reg += nsu.ToString().PadLeft(6,'0');
            
            return reg.PadRight (61, ' ');
        }

        public string ObtemNsuRetorno(string message)
        {
            return message.Substring(7, 6).TrimStart('0');
        }
    }
}
