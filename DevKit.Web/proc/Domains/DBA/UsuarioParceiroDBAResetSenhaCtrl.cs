using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.Threading;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    [AllowAnonymous]
    public class UsuarioParceiroDBAResetSenhaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var email = Request.GetQueryStringValue("email");

            db = new AutorizadorCNDB();

            var user = (from e in db.UsuarioParceiro
                         where e.stEmail != null && email.ToLower() == e.stEmail.ToLower()
                         select e).
                         FirstOrDefault();

            if (user == null)
                return BadRequest();

            user.stSenha = GetRandomString(6);

            db.Update(user);

            if (SendEmail("ConveyNET - Segunda via de senha",
                        "<p>Prezado(a) PARCEIRO,</p>" +
                        "<p>Conforme solicitado no site da CONVEY, segue abaixo a 2º via de senha para acesso ao site.</p>" +
                        "<p>Nome: " + user.stNome + "<br>" +
                        "<p>Nova senha:" + user.stSenha + "<br><br></p>" +
                        "<p>Atenciosamente,</p>" +
                        "<p>CONVEY</p><p>&nbsp;</p>" +
                        "<p>Para enviar seus comentários, envie e-mail para atendimento@conveynet.com.br<br>" +
                        "Esta é uma mensagem gerada automaticamente, portanto, não deve ser respondida.</p>" +
                        "<p>CONVEY. Todos os direitos reservados.\n\n",
                        user.stEmail))
            {
                return Ok();
            }
            else
            {
                return BadRequest("Falha no envio");
            }
        }

        [NonAction]
        private string GetRandomString(int length)
        {
            var rand = new Random();
            var ret = "";

            for (int i = 0; i < length; i++)
            {
                Thread.Sleep(1);
                ret += rand.Next(0, 9);
            }

            return ret;
        }
    }
}
