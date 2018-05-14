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

                var param_usuario = "conveynet@conveynet.com.br";

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

                using (var client = new SmtpClient
                {
                    Port = 587,
                    Host = "smtp.conveynet.com.br",
                    EnableSsl = false,
                    Timeout = 10000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(param_usuario, "c917800")
                })
                {
                    string  assunto = "ConveyNET Mobile Payment",

                            texto = "CARTÃO CONVEYNET BENEFICIOS" + 
                                    "\r\nVENDA AUTORIZADA - DATA / HORA:" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") +
                                    "\r\nVALOR TOTAL: " + new money().formatToMoney(dadosVenda.vr_total.ToString()) +
                                    "\r\nPARCELAS: " + dadosVenda.nu_parcelas +
                                    "\r\nNSU: " + dadosVenda.nu_nsu.ToString() +
                                    "\r\nEstabelecimento: " + estab.st_nome +
                                    "\r\nAssociado: " + dadosProp.st_nome +
                                    "\r\nAssociação: " + dadosCartao.st_empresa +
                                    "\r\nMatricula: " + dadosCartao.st_matricula;

                    var mm = new MailMessage(param_usuario, stEmail, assunto, texto)
                    {
                        BodyEncoding = UTF8Encoding.UTF8,
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                    };

                    client.Send(mm);
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
