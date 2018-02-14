using DataModel;
using LinqToDB;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
                case 1:

                    switch (filter.modo)
                    {
                        case 1: return Ok(new Empresa().ListagemFechamento_CredSint(db, filter));
                    }

                    break;
            }

            return BadRequest("Tipo / modo inválidos");
        }
	}
}
