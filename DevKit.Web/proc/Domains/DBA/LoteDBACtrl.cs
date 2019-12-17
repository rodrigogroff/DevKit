using LinqToDB;
using DataModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System.IO;
using System.Text;

namespace DevKit.Web.Controllers
{
    public class LoteDBAController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/LoteDBA/exportar", Name = "ExportarLoteDBA")]
        public IHttpActionResult Exportar(int idLote)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            string dir = "c:\\fechamento_dbf", file = "lote" + idLote, ext = "txt";

            var t_lote = db.T_LoteCartao.FirstOrDefault(y => y.i_unique == idLote);

            t_lote.dt_envio_grafica = DateTime.Now;

            if (t_lote.tg_sitLote == 1)
                t_lote.tg_sitLote = 2;

            db.Update(t_lote);

            string FileName = dir + "\\" + file + "." + ext;

            if (File.Exists(FileName))
                File.Delete(FileName);

            using (var ts = new StreamWriter(dir + "\\" + file + "." + ext, false, Encoding.Default))
            {
                var lstDetCartoes = db.T_LoteCartaoDetalhe.Where(y => y.fk_lote == idLote).ToList();

                var lstIdsEmps = lstDetCartoes.Select(y => y.fk_empresa).Distinct().ToList();
                var lstIdsCartoes = lstDetCartoes.Select(y => y.fk_cartao).Distinct().ToList();
                
                var lstEmp = db.T_Empresa.Where(y => lstIdsEmps.Contains((int)y.i_unique));
                var lstCartoes = db.T_Cartao.Where(y => lstIdsCartoes.Contains((int)y.i_unique));

                var oldEmp = "";

                foreach (var item in lstDetCartoes.OrderBy ( y=> y.fk_empresa).ToList())
                {
                    var line = "";

                    var emp = lstEmp.FirstOrDefault(y => y.i_unique == item.fk_empresa);

                    if (oldEmp == "")
                    {
                        oldEmp = emp.st_empresa;
                    }                        
                    else if (oldEmp != emp.st_empresa)
                    {
                        ts.WriteLine("");
                        ts.WriteLine("");
                    }

                    var cart = lstCartoes.FirstOrDefault(y => y.i_unique == item.fk_cartao);
                    var venctoCartao = cart.st_venctoCartao.PadLeft(4, '0');

                    var nome = "";

                    if (cart.st_titularidade == "01")
                    {
                        nome = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cart.fk_dadosProprietario).st_nome;
                    }
                    else
                    {
                        nome = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cart.fk_dadosProprietario &&
                                                                   y.nu_titularidade == Convert.ToInt32(cart.st_titularidade)).st_nome;
                    }

                    line += nome.PadRight(30, ' ').Substring(0, 30).TrimEnd(' ') + ",";
                    line += emp.st_empresa + ",";
                    line += item.nu_matricula.ToString().PadLeft(6, '0') + ",";

                    line += venctoCartao.Substring(0, 2) + "/" +
                            venctoCartao.Substring(2, 2) + ",";

                    line += calculaCodigoAcesso (   cart.st_empresa,
                                                    cart.st_matricula,
                                                    cart.st_titularidade,
                                                    item.nu_via_original.ToString(),
                                                    item.nu_cpf ) + ",";

                    line += nome + ",|";

                    line += "826766" + cart.st_empresa +
                                            cart.st_matricula +
                                            cart.st_titularidade +
                                            item.nu_via_original.ToString() +
                                 "65" + cart.st_venctoCartao;

                    line += "|";

                    if (cart.tg_emitido == Convert.ToInt32(StatusExpedicao.NaoExpedido))
                    {
                        cart.tg_emitido = Convert.ToInt32(StatusExpedicao.EmExpedicao);

                        db.Update(cart);
                    }                    

                    ts.WriteLine(line);
                }                
            }

            return ResponseMessage(TransferirConteudo(dir, file, ext));
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
