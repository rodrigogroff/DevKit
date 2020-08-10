using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using System.Net.Mail;
using System.Text;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class EnvioEmailController : ApiControllerBase
    {
        public string nsu;

        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            try
            {
                var idCartao = Request.GetQueryStringValue("cartao");
                var stEmail = Request.GetQueryStringValue("email");

                if (string.IsNullOrEmpty(idCartao)) return BadRequest();
                if (!stEmail.Contains("@")) return BadRequest();

                

                var dadosCartao = db.T_Cartao.FirstOrDefault(y => y.i_unique.ToString() == idCartao);
                var dadosProp = db.T_Proprietario.FirstOrDefault(y => y.i_unique == dadosCartao.fk_dadosProprietario);
                var dadosVenda = db.LOG_Transacoes.Where(y => y.fk_cartao.ToString() == idCartao).OrderByDescending(y => y.i_unique).FirstOrDefault();
                var estab = db.T_Loja.FirstOrDefault(y=> y.i_unique == dadosVenda.fk_loja);

                // ---------------------------
                // atualiza novo email
                // ---------------------------

                if (dadosProp.st_email != stEmail)
                {
                    dadosProp.st_email = stEmail;

                    db.Update(dadosProp);
                }

                // ---------------------------
                // envia email
                // ---------------------------

                string assunto = "ConveyNET Mobile Payment",

                texto = "CARTÃO CONVEYNET BENEFICIOS" +
                        "\r\nVENDA AUTORIZADA - DATA / HORA:" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") +
                        "\r\nVALOR TOTAL: " + new money().formatToMoney(dadosVenda.vr_total.ToString()) +
                        "\r\nPARCELAS: " + dadosVenda.nu_parcelas +
                        "\r\nNSU: " + dadosVenda.nu_nsu.ToString() +
                        "\r\nEstabelecimento: " + estab.st_nome +
                        "\r\nAssociado: " + dadosProp.st_nome +
                        "\r\nAssociação: " + dadosCartao.st_empresa +
                        "\r\nMatricula: " + dadosCartao.st_matricula;

                {
                    MailMessage message = new System.Net.Mail.MailMessage("conveynet@zohomail.com", stEmail, assunto, texto);

                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.zoho.com",
                        Port = 587,
                        Credentials = new System.Net.NetworkCredential("conveynet@zohomail.com", "Gustavo@2012"),
                        EnableSsl = true
                    };

                    message.IsBodyHtml = true;

                    smtp.Send(message);
                }
            }
            catch (SystemException ex)
            {
                ex.ToString();
                return BadRequest();
            }

            return Ok();
        }
    }
}
