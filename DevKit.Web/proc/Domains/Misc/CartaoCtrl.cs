using DataModel;
using System.Net;
using System.Web.Http;
using System.Linq;
using LinqToDB;
using System;

namespace DevKit.Web.Controllers
{
	public class CartaoController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var util = new Util();

            var emp = Request.GetQueryStringValue("emp",0);
            var mat = Request.GetQueryStringValue("mat",0);
            var ca = Request.GetQueryStringValue("ca");
            var titVia = Request.GetQueryStringValue("titVia")?.PadLeft(4,'0');

            var nuTit = Convert.ToInt32(titVia.Substring(0, 2));
            var nuVia = Convert.ToInt32(titVia.Substring(2, 2));

            var empTb = (from e in db.Empresa
                       where e.nuEmpresa == emp
                       select e).
                       FirstOrDefault();

            if (empTb == null)
                return BadRequest("Empresa inválida");

            var associado = (from e in db.Associado
                             where e.fkEmpresa == empTb.id
                             where e.nuMatricula == mat
                             where e.nuTitularidade == nuTit
                             where e.nuViaCartao == nuVia
                             select e).
                             FirstOrDefault();
            
            if (associado == null)
                return BadRequest("Matrícula inválida");

            var caCalc = util.calculaCodigoAcesso(emp.ToString().PadLeft(6, '0'),
                                               mat.ToString().PadLeft(6, '0'),
                                               associado.nuTitularidade.ToString(),
                                               associado.nuViaCartao.ToString(),
                                               associado.stCPF);

            if (ca != caCalc)
                return BadRequest("Cartão inválido >> " + caCalc);

            if (db.currentCredenciado != null)
            {
                if (!db.CredenciadoEmpresa.Any(y => y.fkCredenciado == db.currentCredenciado.id &&
                                           y.fkEmpresa == empTb.id))
                {
                    return BadRequest("Credenciado não conveniado à empresa " + emp);
                }
            }
            
            return Ok( associado );
        }
	}
}
