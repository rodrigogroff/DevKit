using DataModel;
using System.Net;
using System.Web.Http;
using System.Linq;
using LinqToDB;
using System;
using DevKit.DataAccess;

namespace DevKit.Web.Controllers
{
    public class AutorizaProcController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var util = new Util();

            var emp = Request.GetQueryStringValue("emp", 0);
            var mat = Request.GetQueryStringValue("mat", 0);
            var ca = Request.GetQueryStringValue("ca");
            var senha = Request.GetQueryStringValue("senha");
            var tuss = Request.GetQueryStringValue("tuss", 0);
            var titVia = Request.GetQueryStringValue("titVia")?.PadLeft(4, '0');

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

            var caCalc = util.calculaCodigoAcesso ( emp.ToString().PadLeft(6, '0'),
                                                    mat.ToString().PadLeft(6, '0'),
                                                    associado.nuTitularidade.ToString(),
                                                    associado.nuViaCartao.ToString(),
                                                    associado.stCPF );

            if (ca != caCalc)
                return BadRequest("Dados do cartão inválidos!");

            if (senha != associado.stSenha)
                return BadRequest("Senha inválida! " + associado.stSenha);

            if (!db.MedicoEmpresa.Any(y => y.fkMedico == db.currentMedico.id && 
                                           y.fkEmpresa == empTb.id))            
                return BadRequest("Médico não conveniado à empresa " + emp);
        
            var proc = db.Procedimento.
                            Where(y => y.nuTUSS == tuss).
                            FirstOrDefault();

            if (proc == null)
                return BadRequest("Procedimento " + tuss + " inválido!");

            //var dtHoje = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //var dtHojeFim = dtHoje.AddDays(1);

            //if (db.Autorizacao.Any (y=> y.fkMedico == db.currentMedico.id && 
            //                            y.fkAssociado == associado.id && 
            //                            y.dtSolicitacao > dtHoje && y.dtSolicitacao < dtHojeFim))
            //{
            //return BadRequest("Procedimento " + tuss + " em duplicidade!");
            //}
            //else

            DateTime dt = DateTime.Now;

            if (dt.Day < empTb.nuDiaFech)
                dt = dt.AddMonths(-1);
            
            db.Insert(new Autorizacao
            {
                dtSolicitacao = DateTime.Now,
                fkAssociado = associado.id,
                fkMedico = db.currentMedico.id,
                fkEmpresa = associado.fkEmpresa,
                fkProcedimento = proc.id,
                nuAno = dt.Year,
                nuMes = dt.Month,
                tgSituacao = TipoSitAutorizacao.Autorizado,
            });
            
            return Ok();
        }
    }
}
