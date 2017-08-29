using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class AssociadoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var empresa = Request.GetQueryStringValue("empresa");
            var matricula = Request.GetQueryStringValue("matricula");
            var vencimento = Request.GetQueryStringValue("vencimento");

            if (empresa.Length < 6) empresa = empresa.PadLeft(6, '0');
            if (matricula.Length < 6) matricula = matricula.PadLeft(6, '0');

            var acesso = Request.GetQueryStringValue("acesso");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            // busca associado

            var associado = RestoreTimerCache("associadoEMV", empresa + matricula + vencimento, 5) as T_Cartao;

            if (associado == null)
            {
                associado = (from e in db.T_Cartao
                             where e.st_empresa == empresa
                             where e.st_matricula == matricula
                             where e.st_venctoCartao == vencimento
                             select e).
                             FirstOrDefault();

                if (associado == null)
                    return BadRequest();

                BackupCache(associado);
            }

            var tEmpresa = RestoreTimerCache("empresa", associado.st_empresa, 5) as T_Empresa;

            if (tEmpresa == null)
            {
                tEmpresa = (from e in db.T_Empresa
                           where e.st_empresa == associado.st_empresa
                           select e).
                           FirstOrDefault();

                if (tEmpresa == null)
                    return BadRequest();

                BackupCache(tEmpresa);
            }

            // busca dados proprietario

            var dadosProprietario = (from e in db.T_Proprietario
                                     where e.i_unique == associado.fk_dadosProprietario
                                     select e).
                                     FirstOrDefault();

            var codAcessoCalc = new CodigoAcesso().Obter(empresa,
                                                           matricula,
                                                           associado.st_titularidade,
                                                           associado.nu_viaCartao.ToString(),
                                                           dadosProprietario.st_cpf);

            // verificação de código de acesso

            if (acesso != codAcessoCalc)
            {
                // pode ser dependente

                var lstCartoesDependentes = (from e in db.T_Cartao
                                             where e.st_empresa == empresa
                                             where e.st_matricula == matricula
                                             where e.i_unique != associado.i_unique
                                             select e).
                                             ToList();

                var found = false;

                foreach (var cartDep in lstCartoesDependentes)
                {
                    codAcessoCalc = new CodigoAcesso().Obter(empresa,
                                                           matricula,
                                                           cartDep.st_titularidade,
                                                           associado.nu_viaCartao.ToString(),
                                                           dadosProprietario.st_cpf);

                    if (acesso == codAcessoCalc)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return BadRequest();
            }

            long dispMensal = 0,
                 dispTotal = 0;

            new SaldoDisponivel().
                Obter(db, associado, ref dispMensal, ref dispTotal);

            var mon = new money();

            return Ok(new
            {
                count = 0,
                results = new List<Associado>
                {
                    new Associado
                    {
                        id = associado.i_unique.ToString(),
                        nome = dadosProprietario.st_nome,
                        dispMensal = mon.setMoneyFormat ((long)associado.vr_limiteMensal - dispMensal),
                        dispTotal = mon.setMoneyFormat ((long)associado.vr_limiteTotal - dispTotal),
                        dispExtra = mon.setMoneyFormat ((long)associado.vr_extraCota),
                        maxParcelasEmpresa = tEmpresa.nu_parcelas.ToString(),
                        bloqueado = associado.tg_status == '1' ? true : false,
                    }
                }
            });
        }

        /*
		public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.Client, id) is Client obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetClient(id);

			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);
		}

		public IHttpActionResult Post(Client mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

			if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            CleanCache(db, CacheTags.Client, null);
            StoreCache(CacheTags.Client, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, Client mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
			if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Client, null);
            StoreCache(CacheTags.Client, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetClient(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiError))
				return BadRequest(apiError);

			mdl.Delete(db);

            CleanCache(db, CacheTags.Client, null);

            return Ok();
		}
        */
    }
}
