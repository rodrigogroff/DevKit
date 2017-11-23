using DataModel;
using DevKit.Web.Services;
using System.Web.Http;
using System.Linq;
using System.Threading;
using System.Web;
using System;
using LinqToDB;
using System.Security.Claims;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : MemCacheController
	{
        public AutorizadorCNDB db;

        public string   cnet_server = "54.233.109.221",
                        apiError = "";

        public int cnet_port = 2000;

        public string userLoggedName
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.Name.ToUpper();
            }
        }

        public string userLoggedType
        {
            get
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                
                return identity.Claims.
                    Where(c => c.Type == "tipo").
                    Select(c => c.Value).
                    SingleOrDefault();
            }
        }

        public string userLoggedEmpresa
        {
            get
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                return identity.Claims.
                    Where(c => c.Type == "empresa").
                    Select(c => c.Value).
                    SingleOrDefault();
            }
        }

        [NonAction]
        public bool StartDatabaseAndAuthorize()
        {
            db = new AutorizadorCNDB();

            var userCurrentName = userLoggedName;
            
            switch(userLoggedType)
            {
                case "1":
                    
                    #region - terminal lojista - 

                    if (userCurrentName != "DBA")
                    {
                        db.currentLojista = RestoreTimerCache(CacheTags.T_Terminal, userCurrentName, 2) as T_Loja;

                        if (db.currentLojista == null)
                        {
                            var term = (from e in db.T_Terminal
                                        where e.nu_terminal == userCurrentName.PadLeft(8, '0')
                                        select e).
                                        FirstOrDefault();

                            db.currentLojista = (from ne in db.T_Loja
                                                 where ne.i_unique == term.fk_loja
                                                 select ne).
                                              FirstOrDefault();

                            if (db.currentLojista == null)
                                return false;

                            BackupCache(db.currentLojista);
                        }

                    }
                    #endregion                    

                    break;

                case "2":

                    #region - associado -
                    {
                        db.currentAssociado = RestoreTimerCache(CacheTags.T_Cartao, userCurrentName, 2) as T_Cartao;

                        if (db.currentAssociado == null)
                        {
                            var lstName = userCurrentName.Split('.');

                            db.currentAssociado = (from e in db.T_Cartao
                                                   where e.st_empresa == lstName[0].PadLeft(6, '0')
                                                   where e.st_matricula == lstName[1].PadLeft(6, '0')
                                                   where e.st_titularidade == "01"
                                                   select e).
                                                    FirstOrDefault();

                            if (db.currentAssociado == null)
                                return false;

                            BackupCache(db.currentAssociado);
                        }

                        db.currentAssociadoEmpresa = RestoreTimerCache(CacheTags.T_Empresa, db.currentAssociado.st_empresa, 2) as T_Empresa;

                        if (db.currentAssociadoEmpresa == null)
                        {
                            db.currentAssociadoEmpresa = (from e in db.T_Empresa
                                                          where e.st_empresa == db.currentAssociado.st_empresa
                                                          select e).
                                                          FirstOrDefault();

                            if (db.currentAssociadoEmpresa == null)
                                return false;

                            BackupCache(db.currentAssociadoEmpresa);
                        }
                    }
                    #endregion

                    break;
                    
                case "3":

                    #region - gestão lojista - 
                    {
                        db.currentLojista = RestoreTimerCache(CacheTags.T_Loja, userCurrentName, 2) as T_Loja;

                        if (db.currentLojista == null)
                        {
                            db.currentLojista = (from e in db.T_Loja
                                                 where e.st_loja == userCurrentName
                                                 select e).
                                                 FirstOrDefault();

                            if (db.currentLojista == null)
                                return false;

                            BackupCache(db.currentLojista);
                        }
                    }
                    #endregion

                    break;

                case "4":

                    #region - gestão emissora - 
                    {
                        var st_empresa = userLoggedEmpresa.PadLeft(6,'0');

                        db.currentEmpresa = RestoreTimerCache(CacheTags.T_Empresa, st_empresa, 2) as T_Empresa;

                        if (db.currentEmpresa == null)
                        {
                            db.currentEmpresa = (from e in db.T_Empresa
                                                 where e.st_empresa == st_empresa
                                                 select e).
                                                 FirstOrDefault();

                            if (db.currentEmpresa == null)
                                return false;

                            BackupCache(db.currentEmpresa);
                        }

                    }
                    #endregion

                    break;
            }
            
            if (myApplication == null)
                myApplication = HttpContext.Current.Application;

            if (myApplication["start"] == null)
            {
                myApplication["start"] = true;

                StartCache();

                if (IsPrecacheEnabled)
                    System.Threading.Tasks.Task.Run(() => { new StartupPreCacheService().Run(myApplication, db.currentLojista); });

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
