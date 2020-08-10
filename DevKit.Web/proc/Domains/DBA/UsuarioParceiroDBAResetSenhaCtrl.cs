using DataModel;
using LinqToDB;
using System;
using System.Linq;
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
                         where email.ToLower() == e.stEmail.ToLower()
                         select e).
                         FirstOrDefault();

            if (user == null)
                return BadRequest();

            user.stSenha = GetRandomString(6);

            db.Update(user);

            SendEmail("ConveyNET - Segunda via de senha",
                      "Prezado(a) PARCEIRO,\n\n" +
                      "Conforme solicitado no site da CONVEY, segue abaixo a 2º via de senha para acesso ao site. \n" + 
                      "Nome: " + user.stNome + "\n" + 
                      "Nova senha:" + user.stSenha + "\n\n" +
                      "Atenciosamente,\n" +
                      "CONVEY\n\n" +
                      "Para enviar seus comentários, envie e-mail para atendimento@conveynet.com.br\n" + 
                      "Esta é uma mensagem gerada automaticamente, portanto, não deve ser respondida.\n" + 
                      "CONVEY. Todos os direitos reservados.\n\n",
                      user.stEmail);

            return Ok();
        }

        [NonAction]
        private string GetRandomString(int length)
        {
            var rand = new Random();
            var ret = "";

            for (int i = 0; i < length; i++)
                ret += rand.Next(0, 9);

            return ret;
        }
    }
}
