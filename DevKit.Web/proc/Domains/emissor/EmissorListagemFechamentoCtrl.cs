using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class EmissorListagemFechamentoController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new ListagemFechamentoFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                mes = Request.GetQueryStringValue("mes", 0),
                ano = Request.GetQueryStringValue("ano", 15),
                tipo = Request.GetQueryStringValue("tipo", 0),
                modo = Request.GetQueryStringValue("modo", 0),
            };

            switch(filter.tipo)
            {
                // ==================
                // credenciado
                // ==================

                case 1: 

                    switch (filter.modo)
                    {
                        case 1: return Ok(new Empresa().ListagemFechamento_CredSint(db, filter));
                        case 2: return Ok(new Empresa().ListagemFechamento_CredAnalitico(db, filter));
                    }

                    break;

                // ==================
                // Associado
                // ==================

                case 2: // Associado

                    switch (filter.modo)
                    {
                        case 1: return Ok(new Empresa().ListagemFechamento_AssocSint(db, filter));
                        case 2: return Ok(new Empresa().ListagemFechamento_AssocAnalitico(db, filter));
                    }

                    break;
            }

            return BadRequest("Critérios de fechamento inválidos");
        }
	}
}
