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
