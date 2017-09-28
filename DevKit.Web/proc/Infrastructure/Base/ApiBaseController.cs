using DataModel;
using DevKit.Web.Services;
using System.Web.Http;
using System.Linq;
using System.Threading;
using System.Web;
using System;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : MemCacheController
	{
        public string cnet_server = "54.233.109.221";
        public int cnet_port = 2000;

        public AutorizadorCNDB db;
        
        public string apiError = "";

        public string userLoggedName
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.Name.ToUpper();
            }
        }
        
        [NonAction]
        public bool StartDatabaseAndAuthorize()
        {
            db = new AutorizadorCNDB();

            var userCurrentName = userLoggedName;

            if (userCurrentName != "DBA")
            {
                db.currentUser = RestoreTimerCache(CacheTags.T_Terminal, userCurrentName, 2) as T_Loja;

                if (db.currentUser == null)
                {
                    var term = (from e in db.T_Terminal
                                where e.nu_terminal == userCurrentName.PadLeft(8, '0')
                                select e).
                                FirstOrDefault();

                    db.currentUser = (from ne in db.T_Loja
                                      where ne.i_unique == term.fk_loja
                                      select ne).
                                      FirstOrDefault();

                    if (db.currentUser == null)
                        return false;

                    BackupCache(db.currentUser);
                }
            }
            else
            {
                if (myApplication == null)
                    myApplication = HttpContext.Current.Application;
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

        [NonAction]
        public long ObtemValor(string valor)
        {
            if (valor == null)
                return 0;

            var iValor = 0;

            if (!valor.Contains(","))
                valor += ",00";

            valor = valor.Replace(",", "").Replace(".", "");
            iValor = Convert.ToInt32(valor);

            return iValor;
        }
    }
}
