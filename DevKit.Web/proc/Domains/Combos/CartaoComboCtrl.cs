using DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class CartaoComboReport
    {
        public int count;
        public List<DtoCartaoCombo> results;
    }

    public class DtoCartaoCombo
    {
        public long id { get; set; }
        public string stName { get; set; }
        public string stCodigoFOPA { get; set; }
        public string matricula { get; set; }
    }

    public class CartaoComboController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            string busca = Request.GetQueryStringValue("busca", "").ToUpper();

            if (busca == "")
                return Ok( new CartaoComboReport
                {
                    count = 0,
                    results = new List<DtoCartaoCombo>()
                });

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Cartao
                         join prop in db.T_Proprietario on (int)e.fk_dadosProprietario equals prop.i_unique
                         where e.st_empresa == db.currentEmpresa.st_empresa && e.st_titularidade == "01"
                         where busca != "" && (prop.st_nome.Contains(busca) || e.st_matricula.Contains (busca))
                         select e);

            var lst = new List<DtoCartaoCombo>();

            foreach (var item in query.ToList())
            {
                lst.Add(new DtoCartaoCombo
                {
                    id = (int) item.i_unique,
                    stName = item.st_matricula + " - " + db.T_Proprietario.FirstOrDefault ( y=> y.i_unique == item.fk_dadosProprietario )?.st_nome
                });
            }

            var ret = new CartaoComboReport
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

            var query = (from e in db.T_Cartao where e.i_unique == id select e);

            var mdl = query.FirstOrDefault();

			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(new DtoCartaoCombo
            {
                id = id,
                stName = mdl.st_matricula + " - " + db.T_Proprietario.FirstOrDefault(y => y.i_unique == mdl.fk_dadosProprietario).st_nome,
                stCodigoFOPA = mdl.stCodigoFOPA,
                matricula = mdl.st_matricula
            });
		}
	}
}
