using LinqToDB;
using DataModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using App.Web;

namespace DevKit.Web.Controllers
{
    public class LoteDBAController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/LoteDBA/exportar", Name = "ExportarLoteDBA")]
        public IHttpActionResult Exportar(int idLote)
        {
            return Ok();
        }

        //[NonAction]
        //private IHttpActionResult Export(IQueryable<T_Loja> query)
        //{
        //    var myXLSWrapper = new ExportWrapper("Export_Lojas.xlsx",
        //                                           "Lojas",
        //                                           new string[] { "Código",
        //                                                          "Nome",
        //                                                          "Cidade",
        //                                                          "Estado",
        //                                                          "Endereço",
        //                                                          "Telefone",
        //                                                          "Empresas",
        //                                                          "Terminais",
        //                                                          "Bloqueio",
        //                                                          "Senha" });

        //    var lstEmpresas = db.T_Empresa.ToList();
        //    var lstLinks = db.LINK_LojaEmpresa.ToList();
        //    var lstTerms = db.T_Terminal.ToList();

        //    foreach (var item in query)
        //    {
        //        string senha = "Com Senha", empresas = "", terminais = "";

        //        if (item.tg_portalComSenha == 0)
        //            senha = "Sem Senha";

        //        foreach (var it in lstLinks.Where(y => y.fk_loja == item.i_unique).ToList())
        //            empresas += lstEmpresas.
        //                            FirstOrDefault(y => y.i_unique == it.fk_empresa).st_empresa.TrimStart('0') + ",";

        //        foreach (var it in lstTerms.Where(y => y.fk_loja == item.i_unique).ToList())
        //            terminais += it.nu_terminal.ToString().TrimStart('0') + ",";

        //        myXLSWrapper.AddContents(new string[]
        //        {
        //            item.st_loja,
        //            item.st_nome,
        //            item.st_cidade,
        //            item.st_estado,
        //            item.st_endereco,
        //            item.nu_telefone.ToString(),
        //            empresas,
        //            terminais,
        //            item.tg_blocked == '1' ? "Bloqueada" : "Ativa",
        //            senha,
        //        });
        //    };

        //    return ResponseMessage(myXLSWrapper.GetSingleSheetHttpResponse());
        //}

        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_LoteCartao select e);

            query = query.OrderByDescending(y => y.dt_abertura);

            var lst = new List<T_LoteCartao>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
                lst.Add(item);

            foreach (var item in lst)
            {
                switch (item.tg_sitLote)
                {
                    case 1: item.situacao = "Em Aberto"; break;
                    case 2: item.situacao = "Produção na gráfica"; break;
                    case 3: item.situacao = "Cartões ativados"; break;
                }

                item.data = Convert.ToDateTime(item.dt_abertura).ToString("dd/MM/yyyy HH:mm");

                if (item.dt_envio_grafica != null)
                    item.dataGrafica = Convert.ToDateTime(item.dt_envio_grafica).ToString("dd/MM/yyyy HH:mm");

                if (item.dt_ativacao != null)
                    item.dataAtiv = Convert.ToDateTime(item.dt_ativacao).ToString("dd/MM/yyyy HH:mm");

                item.cartoes = item.nu_cartoes.ToString();
                item.empresas = "";

                var lstDetalhes = db.T_LoteCartaoDetalhe.Where(y => y.fk_lote == item.i_unique).ToList();

                var lstEmps = lstDetalhes.Select(y => y.fk_empresa).Distinct();

                foreach (var it in lstEmps)
                {
                    var e = db.T_Empresa.FirstOrDefault(y => y.i_unique == it).st_empresa;

                    item.empresas += e + ", ";
                }

                item.empresas = item.empresas.Trim().TrimEnd(',');
            }

            return Ok(new
            {
                count = query.Count(),
                results = lst
            });
        }

        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.T_LoteCartao where e.i_unique == id select e).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(mdl);
        }

        [HttpPut]
        public IHttpActionResult Put(T_LoteCartao mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            db.Update(mdl);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post(T_LoteCartao mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            mdl.i_unique = Convert.ToInt64(db.InsertWithIdentity(mdl));

            return Ok(mdl);
        }
    }
}
