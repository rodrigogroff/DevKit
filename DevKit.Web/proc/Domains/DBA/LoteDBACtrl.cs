using LinqToDB;
using DataModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System.IO;
using System.Text;
using App.Web;
using System.Net.Http;

namespace DevKit.Web.Controllers
{
    public class LoteDBAController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/LoteDBA/exportar", Name = "ExportarLoteDBA")]
        public HttpResponseMessage Exp(int idLote)
        {
            StartDatabase();

            var t_lote = db.T_LoteCartao.FirstOrDefault(y => y.i_unique == idLote);

            t_lote.dt_envio_grafica = DateTime.Now;

            if (t_lote.tg_sitLote == 1)
                t_lote.tg_sitLote = 2;

            db.Update(t_lote);

            var abaString = "Exportar";

            var x = new ExportMultiSheetWrapper("lote"+ idLote + ".xlsx");

            x.NovaAba_Header(abaString, (new List<string>
            {
                "Nome",
                "Card1",
                "Card2",
                "Validade",
                "Card3",
                "Empresa",
                "Matrícula",
                "Nome2",
                "Tarja magnética",
            }).
            ToArray());

            var lstDetCartoes = db.T_LoteCartaoDetalhe.Where(y => y.fk_lote == idLote).ToList();

            var lstIdsEmps = lstDetCartoes.Select(y => y.fk_empresa).Distinct().ToList();
            var lstIdsCartoes = lstDetCartoes.Select(y => y.fk_cartao).Distinct().ToList();

            var lstEmp = db.T_Empresa.Where(y => lstIdsEmps.Contains((int)y.i_unique));
            var lstCartoes = db.T_Cartao.Where(y => lstIdsCartoes.Contains((int)y.i_unique));

            foreach (var item in lstDetCartoes.OrderBy(y => y.fk_empresa).ToList())
            {
                var emp = lstEmp.FirstOrDefault(y => y.i_unique == item.fk_empresa);

                var cart = lstCartoes.FirstOrDefault(y => y.i_unique == item.fk_cartao);
                var venctoCartao = cart.st_venctoCartao.PadLeft(4, '0');

                var nome = "";

                T_Proprietario prop = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cart.fk_dadosProprietario);

                if (cart.st_titularidade == "01")
                    nome = prop.st_nome;
                else
                    nome = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cart.fk_dadosProprietario &&
                                                               y.nu_titularidade == Convert.ToInt32(cart.st_titularidade)).st_nome;
                
                x.AdicionarConteudo(abaString, (new List<string>
                {
                    nome.PadRight(30, ' ').Substring(0, 30).TrimEnd(' '),
                    cart.st_empresa,
                    cart.st_matricula,
                    cart.st_venctoCartao.Substring(0, 2) + "/" + cart.st_venctoCartao.Substring(2, 2),
                    calculaCodigoAcesso(cart.st_empresa,
                                                    cart.st_matricula,
                                                    cart.st_titularidade,
                                                    cart.nu_viaCartao.ToString(),
                                                    prop.st_cpf),
                    cart.st_empresa,
                    cart.st_matricula,
                    nome.PadRight(30, ' ').Substring(0, 30).TrimEnd(' '),
                    "826766" + cart.st_empresa +
                                                cart.st_matricula +
                                                cart.st_titularidade +
                                                cart.nu_viaCartao.ToString() +
                                        "65" + cart.st_venctoCartao
                }).
                ToArray());
            }

            return x.GeraXLS();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/LoteDBA/exportarEmail", Name = "ExportarLoteDBAEmail")]
        public HttpResponseMessage exportarEmail(int idLote)
        {
            StartDatabase();

            var novoLote = db.T_LoteCartao.FirstOrDefault(y => y.i_unique == idLote);
            var tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == novoLote.fk_empresa);
            List<string> lstAttach = new List<string>();
            List<string> lstArquivos = new List<string>();

            var cartList = (from e in db.T_Cartao
                           join d in db.T_Proprietario on e.fk_dadosProprietario equals (int)d.i_unique
                           where e.st_empresa == "000002"
                           select new
                           {
                               id = e.i_unique.ToString(),
                               empresa = e.st_empresa,
                               matricula = e.st_matricula,
                               associado = d.st_nome,
                               cpf = d.st_cpf,
                               titularidade = e.st_titularidade,
                               via = e.nu_viaCartao,
                               venc = e.st_venctoCartao,
                               fkProp = e.fk_dadosProprietario
                           }).
                           ToList();

            // ----------------------------------
            // daqui para baixo
            // ----------------------------------

            var abaString = "Exportar";

            var tit = tEmp.st_empresa + "_" + novoLote.i_unique + "_PEDIDO_PRODUCAO.xlsx";
            var tituloArq = System.Web.Hosting.HostingEnvironment.MapPath("/") + "img\\" + tit;

            var x = new ExportMultiSheetWrapper(tituloArq);

            x.NovaAba_Header(abaString, (new List<string>
                                        {
                                            "Nome",
                                            "Card1",
                                            "Card2",
                                            "Validade",
                                            "Card3",
                                            "Empresa",
                                            "Matrícula",
                                            "Nome2",
                                            "Tarja magnética",
                                        }).
            ToArray());

            lstAttach.Add(tituloArq);

            lstArquivos.Add("https://meuconvey.conveynet.com.br/img/" + tit);

            if (File.Exists(tituloArq))
                File.Delete(tituloArq);

            try
            {
                var nome = "";

                foreach (var cart in cartList)
                {
                    var prop = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cart.fkProp);

                    if (cart.titularidade == "01")
                        nome = prop.st_nome;
                    else
                        nome = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cart.fkProp &&
                                                                    y.nu_titularidade == Convert.ToInt32(cart.titularidade)).st_nome;


                    var c_update = db.T_Cartao.FirstOrDefault(a => a.i_unique.ToString() == cart.id);

                    if (c_update.tg_emitido == Convert.ToInt32(StatusExpedicao.NaoExpedido))
                    {
                        c_update.tg_emitido = Convert.ToInt32(StatusExpedicao.EmExpedicao);
                        db.Update(c_update);
                    }

                    x.AdicionarConteudo(abaString, (new List<string>
                                                {
                                                    nome.PadRight(30, ' ').Substring(0, 30).TrimEnd(' '),
                                                    cart.empresa,
                                                    cart.matricula,
                                                    cart.venc.Substring(0, 2) + "/" + cart.venc.Substring(2, 2),
                                                    calculaCodigoAcesso(cart.empresa,
                                                                                    cart.matricula,
                                                                                    cart.titularidade,
                                                                                    cart.via.ToString(),
                                                                                    prop.st_cpf),
                                                    cart.empresa,
                                                    cart.matricula,
                                                    nome.PadRight(30, ' ').Substring(0, 30).TrimEnd(' '),
                                                    "826766" + cart.empresa +
                                                                                cart.matricula +
                                                                                cart.titularidade +
                                                                                cart.via.ToString() +
                                                                        "65" + cart.venc
                                                }).
                    ToArray());
                }
                
                x.Save();

                return x.GeraXLS();
            }
            catch (Exception ex1)
            {


                return x.GeraXLS();
            }            
        }

        // calculo codigo de acesso para cartoes convenio
        public string calculaCodigoAcesso(string empresa, string matricula, string titularidade, string via, string cpf)
        {
            string chave = matricula + empresa + titularidade.PadLeft(2, '0') + via + cpf.PadRight(14, ' ');
            int temp = 0;
            for (int n = 0; n < chave.Length; n++)
            {
                string s = chave.Substring(n, 1);
                char c = s[0]; // First character in s
                int i = c; // ascii code
                temp += i;
            }
            string calculado = temp.ToString("0000");
            temp += int.Parse(calculado[3].ToString()) * 1000;
            temp += int.Parse(calculado[2].ToString());
            if (temp > 9999) temp -= 2000;
            calculado = temp.ToString("0000");
            calculado = calculado.Substring(2, 1) +
                        calculado.Substring(0, 1) +
                        calculado.Substring(3, 1) +
                        calculado.Substring(1, 1);
            return calculado;
        }

        public IHttpActionResult Get()
        {
            if (userLoggedParceiroId != "1")
                return BadRequest("Não autorizado!");

            var busca = Request.GetQueryStringValue("busca");            
            var todos = Request.GetQueryStringValue<bool>("todos", false);
            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa");
            var matricula = Request.GetQueryStringValue<int?>("matricula");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = from e in db.T_LoteCartao select e;

            if (!todos)
                query = query.Where(y => y.tg_sitLote != 3);

            if (idEmpresa > 0 || matricula > 0)
            {
                var lstLotes = (from e in db.T_LoteCartaoDetalhe
                               where idEmpresa == null || e.fk_empresa == idEmpresa
                                where matricula == null || e.nu_matricula == matricula
                               select (long)e.fk_lote).ToList();

                query = from e in query                        
                        where lstLotes.Contains((long)e.i_unique)
                        select e;                     
            }
            
            query = query.OrderByDescending(y => y.dt_abertura);

            var lst = new List<T_LoteCartao>();

            var tu = query.ToList();

            var lstFkLotes = tu.Select(y => (long)y.i_unique).ToList();
            var lstCartDetalhe = db.T_LoteCartaoDetalhe.Where(y => lstFkLotes.Contains((long)y.fk_lote)).ToList();
            var lstCarts = lstCartDetalhe.Select(y => (long)y.fk_cartao).ToList();
            var lstTotCarts = db.T_Cartao.Where(y => lstCarts.Contains((long)y.i_unique)).ToList();

            foreach (var item in tu)
            {
                if (!todos)
                {
                    var found = false;

                    var lstCartDetalhe_tmp = lstCartDetalhe.Where(y => y.fk_lote == (long)item.i_unique).ToList();
                    var lstCarts_tmp = lstTotCarts.Where(y => lstCartDetalhe_tmp.Select(a => (long)a.fk_cartao).ToList().Contains((long)y.i_unique)).ToList();

                    foreach (var itemC in lstCarts_tmp)
                    {
                        var cart = lstTotCarts.FirstOrDefault(y => y.i_unique == itemC.i_unique);

                        if (cart != null)
                            if (cart.tg_emitido != 2) // ativo
                            found = true;
                    }

                    if (found)
                        lst.Add(item);
                }
                else
                    lst.Add(item);
            }                

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

                var lstDetalhes = lstCartDetalhe.Where(y => y.fk_lote == item.i_unique).ToList();

                var lstEmps = lstDetalhes.Select(y => y.fk_empresa).Distinct();

                foreach (var it in lstEmps)
                {
                    var e = db.T_Empresa.FirstOrDefault(y => y.i_unique == it);

                    if (e != null)
                        item.empresas += e.st_empresa + ", ";
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
