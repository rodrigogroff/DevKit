using DataModel;
using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

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

            var associado = (from e in db.T_Cartao
                             where e.st_empresa == empresa 
                             where e.st_matricula == matricula
                             where e.st_venctoCartao == vencimento
                             //where e.vr_limiteMensal
                             select e).
                             FirstOrDefault();

            if (associado == null)
                return BadRequest();

            return Ok(new
            {
                count = 0,
                results = new List<T_Cartao>
                {
                    associado
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
