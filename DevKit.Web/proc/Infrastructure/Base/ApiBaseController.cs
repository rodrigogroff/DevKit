using DataModel;
using DevKit.Web.Services;
using System.Web.Http;
using System.Linq;
using System.Threading;
using System.Web;
using System;
using LinqToDB;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : MemCacheController
	{
        public AutorizadorCNDB db;

        public string   cnet_server = "138.118.164.191",
                        apiError = "";

        public int cnet_port = 2000;

        public string userLoggedName
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.Name.ToUpper();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
            base.Dispose(disposing);
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

        public string userLoggedTypeDBA
        {
            get
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                return identity.Claims.
                    Where(c => c.Type == "tipoDBA").
                    Select(c => c.Value).
                    SingleOrDefault();
            }
        }

        public string userLoggedParceiroId
        {
            get
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                return identity.Claims.
                    Where(c => c.Type == "parceiro").
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

        public string userLoggedEmpresaIdUsuario
        {
            get
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                return identity.Claims.
                    Where(c => c.Type == "IdUsuario").
                    Select(c => c.Value).
                    SingleOrDefault();
            }
        }

        public int? userIdLoggedUsuario
        {
            get
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var id = identity.Claims.
                         Where(c => c.Type == "IdUsuario").
                         Select(c => c.Value).
                         SingleOrDefault();

                if (id != null)
                    return Convert.ToInt32(id);
                else
                    return null;
            }
        }

        public int? parceiroIdLoggedUsuario
        {
            get
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var id = identity.Claims.
                         Where(c => c.Type == "parceiro").
                         Select(c => c.Value).
                         SingleOrDefault();

                if (id != null)
                    return Convert.ToInt32(id);
                else
                    return null;
            }
        }

        [NonAction]
        public bool StartDatabaseAndAuthorize()
        {
            db = new AutorizadorCNDB();

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

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
                                        where Convert.ToInt64(e.nu_terminal) == Convert.ToInt64(userCurrentName)
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
        public List<long> ObtemDBAListaEmpresasParceiroId()
        {
            try
            {
                var idParceiro = Convert.ToInt64(userLoggedParceiroId);

                if (userLoggedParceiroId != "1")
                    return db.T_Empresa.Where(y => y.fkParceiro == idParceiro).Select(y => (long)y.i_unique).ToList();
                else
                    return db.T_Empresa.Select(y => (long)y.i_unique).ToList();
            }
            catch (SystemException ex)
            {
                return null;
            }
        }

        [NonAction]
        public List<string> ObtemDBAListaEmpresasParceiroStr()
        {
            try
            {
                var idParceiro = Convert.ToInt64(userLoggedParceiroId);

                if (userLoggedParceiroId != "1")
                    return db.T_Empresa.Where(y => y.fkParceiro == idParceiro).Select(y => y.st_empresa).ToList();
                else
                    return db.T_Empresa.Select(y => y.st_empresa).ToList();
            }
            catch (SystemException ex)
            {
                return null;
            }
        }

        [NonAction]
        public string ObtemDataSegundos(DateTime? valor)
        {
            try
            {
                if (valor == null)
                    return "";

                return Convert.ToDateTime(valor).ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (SystemException ex)
            {
                return null;
            }
        }

        [NonAction]
        public string ObtemData(DateTime? valor)
        {
            try
            {
                if (valor == null)
                    return "";

                return Convert.ToDateTime(valor).ToString("dd/MM/yyyy HH:mm");
            }
            catch (SystemException ex)
            {
                return null;
            }
        }

        [NonAction]
        public DateTime? ObtemData(string valor)
        {
            try
            {
                if (valor == null)
                    return null;

                if (valor.Length < 8)
                    return null;

                if (valor.Length == 8)
                    valor = valor.Substring(0, 2) + "/" + 
                            valor.Substring(2, 2) + "/" + 
                            valor.Substring(4, 4);

                return new DateTime(Convert.ToInt32(valor.Substring(6, 4)),
                                    Convert.ToInt32(valor.Substring(3, 2)),
                                    Convert.ToInt32(valor.Substring(0, 2)), 0, 0, 0);
            }
            catch (SystemException ex)
            {
                return null;
            }
        }

        [NonAction]
        public long ObtemValor(string valor)
        {
            try
            {
                if (valor == null)
                    return 0;

                if (valor == "")
                    valor = "0";

                var iValor = 0;

                if (!valor.Contains(","))
                    valor += ",00";

                valor = valor.Replace(",", "").Replace(".", "");
                iValor = Convert.ToInt32(valor);

                return iValor;
            }
            catch (SystemException ex)
            {
                return 0;
            }
        }

        [NonAction]
        public int? LimpaValor(string valor)
        {
            try
            {
                if (valor == null)
                return 0;

                if (valor == "")
                    return 0;

                int? iValor = Convert.ToInt32(valor.Replace (",","").Replace(".",""));

                return iValor;
            }
            catch (SystemException ex)
            {
                return null;
            }
        }

        [NonAction]
        public string DESCript(string dados, string chave = "12345678")
        {
            dados = dados.PadLeft(8, '*');

            byte[] key = System.Text.Encoding.ASCII.GetBytes(chave);//{1,2,3,4,5,6,7,8};
            byte[] data = System.Text.Encoding.ASCII.GetBytes(dados);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            des.Key = key;
            des.Mode = CipherMode.ECB;

            ICryptoTransform DESt = des.CreateEncryptor();
            DESt.TransformBlock(data, 0, 8, data, 0);

            string retorno = "";
            for (int n = 0; n < 8; n++)
            {
                retorno += String.Format("{0:X2}", data[n]);
            }

            return retorno;
        }

        public HttpResponseMessage TransferirConteudo(string dir, string fileName, string extenT)
        {
            byte[] fileData = File.ReadAllBytes(dir + "\\" + fileName + "." + extenT);

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream2 = new MemoryStream(fileData);

            result.Content = new StreamContent(stream2);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName + "." + extenT
            };

            return result;
        }

        [NonAction]
        public void SendEmail(string assunto, string texto, string email)
        {
            var param_usuario = "conveynet@conveynet.com.br";

            using (var client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.conveynet.com.br",
                EnableSsl = false,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(param_usuario, "c917800")
            })
            {
                var mm = new MailMessage(param_usuario, email, assunto, texto)
                {
                    BodyEncoding = UTF8Encoding.UTF8,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };

                client.Send(mm);
            }
        }
    }
}
