using DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class DespesaReport
    {
        public int count;
        public List<DtoDespesa> results;
    }

    public class DtoDespesa
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class DespesaController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            string busca = Request.GetQueryStringValue("busca", "").ToUpper();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
            var query = (from e in db.EmpresaDespesa where e.fkEmpresa == db.currentEmpresa.i_unique select e);

			if (busca != "")
				query = from e in query where e.stDescricao.ToUpper().Contains(busca) || e.stCodigo.ToUpper().Contains(busca)  select e;

            var lst = new List<DtoDespesa>();

            foreach (var item in query.ToList())
            {
                lst.Add(new DtoDespesa
                {
                    id = item.id,
                    stName = item.stCodigo + " - " + item.stDescricao
                });
            }

            var ret = new DespesaReport
            {
                count = query.Count(),
                results = lst
            };

            return Ok(ret);
		}

		public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.EmpresaDespesa where e.fkEmpresa == db.currentEmpresa.i_unique && e.id == id select e);

            var mdl = query.FirstOrDefault();

			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(new DtoDespesa
            {
                id = mdl.id,
                stName = mdl.stCodigo + " - " + mdl.stDescricao
            });
		}
	}
}
