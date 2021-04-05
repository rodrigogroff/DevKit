using DataModel;
using LinqToDB;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class EmissoraDespesaRecController : ApiControllerBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            var nome = Request.GetQueryStringValue("nome");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            var query = (from e in db.EmpresaDespesaRecorrente
                         where e.fkEmpresa == tEmp.i_unique                         
                         select e);

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(y => y.stDescricao.Contains(nome) || y.stCodigo.Contains(nome));

            query = query.OrderBy(y => y.stCodigo);

            var page = query.Skip(skip).Take(take).ToList();

            return Ok(new { count = query.Count(), results = page });
        }

        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            return Ok ( (from e in db.EmpresaDespesaRecorrente
                         where e.fkEmpresa == tEmp.i_unique
                         where e.id == id
                         select e).
                         FirstOrDefault() );           
        }

        [HttpPut]
        public IHttpActionResult Put(EmpresaDespesaRecorrente desp)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            var db_t = (from e in db.EmpresaDespesaRecorrente
                        where e.fkEmpresa == tEmp.i_unique
                        where e.id == desp.id
                        select e).
                        FirstOrDefault();

            db_t.stCodigo = desp.stCodigo.PadLeft(2,'0');
            db_t.stDescricao = desp.stDescricao;
            db_t.bAtivo = desp.bAtivo;

            db.Update(db_t);

            return Ok(desp);
        }

        [HttpPost]
        public IHttpActionResult Post(EmpresaDespesaRecorrente desp)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            desp.stCodigo = desp.stCodigo.PadLeft(2, '0');
                 
            if (db.EmpresaDespesa.Any(y => y.fkEmpresa == tEmp.i_unique && y.stCodigo == desp.stCodigo))
                return BadRequest("Código já utilizado");

            var db_t = new EmpresaDespesaRecorrente
            {
                fkEmpresa = (long)tEmp.i_unique,
                stCodigo = desp.stCodigo,
                stDescricao = desp.stDescricao,
                bAtivo = desp.bAtivo,
            };

            db.Insert(db_t);

            return Ok(desp);
        }
    }
}
