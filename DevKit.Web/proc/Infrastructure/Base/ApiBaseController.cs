using DataModel;
using DevKit.Web.Services;
using System.Web.Http;
using System.Linq;
using System.Threading;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : MemCacheController
	{
        public DevKitDB db;
        
        public string apiError = "";

        public string userLoggedName
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.Name.ToUpper();
            }
        }

        // calculo codigo de acesso para cartoes convenio
        public string calculaCodigoAcesso(string empresa, string matricula, string titularidade, string via, string cpf)
        {
            string chave = matricula + empresa + titularidade.PadLeft(2, '0') + via + cpf.PadRight(14, ' ');
            int temp = 0;
            for (int n = 0; n < chave.Length; n++)
            {
                string s = chave.Substring(n, 1);
                char c = s[0]; // First character in s
                int i = c; // ascii code
                temp += i;
            }
            string calculado = temp.ToString("0000");
            temp += int.Parse(calculado[3].ToString()) * 1000;
            temp += int.Parse(calculado[2].ToString());
            if (temp > 9999) temp -= 2000;
            calculado = temp.ToString("0000");
            calculado = calculado.Substring(2, 1) +
                        calculado.Substring(0, 1) +
                        calculado.Substring(3, 1) +
                        calculado.Substring(1, 1);
            return calculado;
        }

        public HttpResponseMessage TransferirConteudo(string fileName)
        {
            byte[] fileData = File.ReadAllBytes(fileName);

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream2 = new MemoryStream(fileData);

            result.Content = new StreamContent(stream2);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };

            return result;
        }
        
        [NonAction]
        public bool StartDatabaseAndAuthorize()
        {
            db = new DevKitDB();

            var userCurrentName = userLoggedName;
            var tagName = CacheTags.User + userCurrentName;

            db.currentUser = RestoreCacheNoHit(tagName) as User;
            
            if (db.currentUser == null)
            {
                db.currentUser = (from ne in db.User
                                  where ne.stLogin.ToUpper() == userCurrentName
                                  select ne).
                                  FirstOrDefault();

                if (db.currentUser == null)
                    return false;

                db.currentUser.empresa = (from e in db.Empresa
                                          where e.id == db.currentUser.fkEmpresa
                                          select e).
                                          FirstOrDefault();
                
                BackupCacheNoHit(tagName, db.currentUser);
            }

            if (myApplication["start"] == null)
            {
                myApplication["start"] = true;

                StartCache();

                if (IsPrecacheEnabled)
                    System.Threading.Tasks.Task.Run(() => { new StartupPreCacheService().Run(myApplication, db.currentUser); });

                if (!IsSingleMachine)
                    System.Threading.Tasks.Task.Run(() => { new CacheControlService().Run(myApplication); });
            }

            return true;
        }
    }
}
