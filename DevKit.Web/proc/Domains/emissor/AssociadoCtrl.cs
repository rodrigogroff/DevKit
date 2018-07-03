using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class AssociadoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new AssociadoFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                fkSecao = Request.GetQueryStringValue("fkSecao"),
                matSaude = Request.GetQueryStringValue("matSaude"),
                matricula = Request.GetQueryStringValue("matricula"),
                titularidade = Request.GetQueryStringValue("titularidade"),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                email = Request.GetQueryStringValue("email")?.ToUpper(),
                cpf = Request.GetQueryStringValue("cpf"),
                phone = Request.GetQueryStringValue("phone")?.ToUpper(),
                tgSituacao = Request.GetQueryStringValue("tgSituacao")?.ToUpper(),
                tgExpedicao = Request.GetQueryStringValue("tgExpedicao")?.ToUpper(),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.AssociadoReport);
            if (hshReport[parameters] is AssociadoReport report)
                return Ok(report);

            var ret = new Associado().ComposedFilters(db, filter);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (RestoreCache(CacheTags.Associado, id) is Associado obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetAssociado(id);

            if (mdl.fkEmpresa != db.currentUser.fkEmpresa)
                return BadRequest("Associado inválido");

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);
        }

        public IHttpActionResult Post(Associado mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Associado, null);
            StoreCache(CacheTags.Associado, mdl.id, mdl);

            return Ok(mdl);
        }

        public IHttpActionResult Put(long id, Associado mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Associado, null);
            StoreCache(CacheTags.Associado, mdl.id, mdl);

            return Ok();
        }

        public IHttpActionResult Delete(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetAssociado(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            if (!mdl.CanDelete(db, ref apiError))
                return BadRequest(apiError);

            mdl.Delete(db);

            CleanCache(db, CacheTags.Associado, null);

            return Ok();
        }
    }
}






