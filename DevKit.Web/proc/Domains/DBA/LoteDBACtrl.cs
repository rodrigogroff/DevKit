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

                foreach (var item in lstDetCartoes)
                {
                    var line = "";

                    var emp = lstEmp.FirstOrDefault(y => y.i_unique == item.fk_empresa);
                    var cart = lstCartoes.FirstOrDefault(y => y.i_unique == item.fk_cartao);
                    var venctoCartao = cart.st_venctoCartao.PadLeft(4, '0');

                    line += item.st_nome_cartao.PadRight(30, ' ').Substring(0, 30).TrimEnd(' ') + ",";
                    line += emp.st_empresa + ",";
                    line += item.nu_matricula.ToString().PadLeft(6, '0') + ",";

                    line += venctoCartao.Substring(0, 2) + "/" +
                            venctoCartao.Substring(2, 2) + ",";

                    line += calculaCodigoAcesso (   cart.st_empresa,
                                                    cart.st_matricula,
                                                    cart.st_titularidade,
                                                    item.nu_via_original.ToString(),
                                                    item.nu_cpf ) + ",";

                    line += item.st_nome_cartao + ",|";

                    line += "826766" + cart.st_empresa +
                                            cart.st_matricula +
                                            cart.st_titularidade +
                                            item.nu_via_original.ToString() +
                                 "65" + cart.st_venctoCartao;

                    line += "|";

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
