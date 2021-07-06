using DataModel;
using LinqToDB;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class BaixaManualHistDto
    {
        public long id { get; set; }
        public string valor { get; set; }
        public string data { get; set; }
        public int registros { get; set; }
    }
    
    public class BaixaManualDto
    {
        public long idCartao { get; set; }
        public string valor { get; set; }
        public long ano { get; set; }
        public long mes { get; set; }
    }

    public class EmissoraBaixaCCController : ApiControllerBase
    {
        [HttpPost]
        public IHttpActionResult Post(BaixaManualDto desp)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            var t_lanc_tipo = db.EmpresaDespesa.FirstOrDefault(y => y.stCodigo == "10" && y.fkEmpresa == (long)tEmp.i_unique);

            if (t_lanc_tipo == null)
            {
                db.Insert (new EmpresaDespesa
                {
                    fkEmpresa = (int)tEmp.i_unique,
                    stCodigo = "10",
                    stDescricao = "BAIXA MANUAL"
                });

                t_lanc_tipo = db.EmpresaDespesa.FirstOrDefault(y => y.fkEmpresa == tEmp.i_unique && y.stCodigo == "10");
            }

            var vlrBaixa = ObtemValor(desp.valor);
            var vlrLancsCartao = db.LancamentosCC.Where(y => y.nuAno == desp.ano && y.nuMes == desp.mes && y.fkCartao == desp.idCartao).Sum(y => (long)y.vrValor);
            var vrFinal = vlrLancsCartao - vlrBaixa;

            db.Insert(new LancamentosCC
            {
                bRecorrente = false,
                dtLanc = System.DateTime.Now,
                fkInicial = null,
                fkBaixa = null,
                fkCartao = desp.idCartao,
                fkEmpresa = (long) tEmp.i_unique,
                fkTipo = t_lanc_tipo.id,
                nuAno = desp.ano,
                nuMes = desp.mes,
                nuParcela = 1,
                nuTotParcelas = 1,
                vrValor = vrFinal,
            });
            
            return Ok(desp);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("baixacc")]
        public HttpResponseMessage BaixaAutomatica()
        {
            var httpRequest = HttpContext.Current.Request;

            int ano = Convert.ToInt32 (HttpContext.Current.Request.Form["ano"]),
                mes = Convert.ToInt32(HttpContext.Current.Request.Form["mes"]),
                empresa = Convert.ToInt32(HttpContext.Current.Request.Form["empresa"]);

            if (HttpContext.Current.Request.Files.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                IsPrecacheEnabled = false;

                StartDatabaseAndAuthorize();

                // empresa
                var t_emp = db.T_Empresa.FirstOrDefault(y => y.i_unique == empresa);

                // tipo despesa
                var t_lanc_tipo = db.EmpresaDespesa.FirstOrDefault(y => y.fkEmpresa == t_emp.i_unique && y.stCodigo == "11");

                if (t_lanc_tipo == null)
                {
                    db.Insert(new EmpresaDespesa
                    {
                        fkEmpresa = (int)t_emp.i_unique,
                        stCodigo = "11",
                        stDescricao = "BAIXA AUTOMÁTICA"
                    });

                    t_lanc_tipo = db.EmpresaDespesa.FirstOrDefault(y => y.fkEmpresa == t_emp.i_unique && y.stCodigo == "11");
                }

                var lstFechamento = db.LOG_Fechamento.Where(y => y.fk_empresa == t_emp.i_unique && y.st_ano == ano.ToString() && y.st_mes == mes.ToString().PadLeft(2,'0')).ToList();
                var lstDespesas = db.LancamentosCC.Where(y => y.nuAno == ano && y.nuMes == mes ).ToList();
                var lstCartoes = db.T_Cartao.Where(y => y.st_empresa == t_emp.st_empresa).ToList();

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var binReader = new BinaryReader(postedFile.InputStream);
                    byte[] byteArray = binReader.ReadBytes(postedFile.ContentLength);

                    string result = System.Text.Encoding.UTF8.GetString(byteArray);                   

                    var fkBaixa = Convert.ToInt64(db.InsertWithIdentity(new LancamentosCCBaixa
                    {
                        dtLog = DateTime.Now,
                        fkEmpresa = empresa,
                        fkUser = null,
                        nuMonth = mes,
                        nuYear = ano
                    }));

                    var dtAnoMes = new DateTime(ano, mes, 1);
                    var dtAnoMesProx = new DateTime(ano, mes, 1).AddMonths(1);

                    int nuRecords = 0;

                    var report = "";

                    foreach (var line in result.Split ('\n'))
                    {
                        var stLine = line.Replace("\t", " ");

                        if (stLine == "")
                            continue;

                        var mat = stLine.Substring(0, 6);

                        if (mat[0] == 65279)
                            mat = mat.Substring(1);

                        report += "--------------------------------------\n";
                        report += "Recebido fopa: " + mat + "\n";

                        mat = mat.TrimStart('0');

                        var t_cart = lstCartoes.FirstOrDefault(y => y.stCodigoFOPA != null && Convert.ToInt32(y.stCodigoFOPA) == Convert.ToInt32(mat));

                        if (t_cart == null)
                        {
                            t_cart = lstCartoes.FirstOrDefault(y => y.st_matricula == mat.PadLeft(6, '0'));

                            if (t_cart == null)
                            {
                                report += "Cartão não existe!\n";
                                continue;
                            }
                        }

                        report += "Cartão existe: " + t_cart.i_unique + "\n"; ;

                        nuRecords++;

                        var vlrBaixa = Convert.ToInt64(stLine.Substring(6).Trim().TrimStart('0'));

                        var vlrLancsCartao = lstDespesas.Where(y => y.fkCartao == t_cart.i_unique).Sum(y => (long) y.vrValor) +
                                             lstFechamento.Where ( y=> y.fk_cartao == t_cart.i_unique).Sum(y => (long)y.vr_valor);

                        var vrFinal = vlrLancsCartao - vlrBaixa;

                        report += "vlrBaixa: " + vlrBaixa + " lancs e fech: " + vlrLancsCartao + " Final: " + vrFinal + "\n";

                        if (vrFinal >= 0)
                        {
                            report += "Registro baixado!\n";

                            db.Insert(new LancamentosCC
                            {
                                bRecorrente = false,
                                dtLanc = DateTime.Now,
                                fkBaixa = fkBaixa,
                                fkCartao = (long)t_cart.i_unique,
                                fkEmpresa = (long)t_emp.i_unique,
                                fkInicial = null,
                                fkTipo = t_lanc_tipo.id,
                                nuAno = dtAnoMes.Year,
                                nuMes = dtAnoMes.Month,
                                nuParcela = 1,
                                nuTotParcelas = 1,
                                vrValor = vlrBaixa,
                            });

                            db.Insert(new LancamentosCC
                            {
                                bRecorrente = false,
                                dtLanc = DateTime.Now,
                                fkBaixa = fkBaixa,
                                fkCartao = (long)t_cart.i_unique,
                                fkEmpresa = (long)t_emp.i_unique,
                                fkInicial = null,
                                fkTipo = t_lanc_tipo.id,
                                nuAno = dtAnoMesProx.Year,
                                nuMes = dtAnoMesProx.Month,
                                nuParcela = 1,
                                nuTotParcelas = 1,
                                vrValor = vrFinal,
                            });
                        }
                        else
                        {
                            report += "Registro não baixado, por valor negativo ou zerado \n";
                        }
                    }

                    var tBaixa = db.LancamentosCCBaixa.FirstOrDefault(y => y.id == fkBaixa);

                    tBaixa.nuRecords = nuRecords;

                    db.Update(tBaixa);
                }

                return Request.CreateResponse(HttpStatusCode.Created);
            }
        }
    }
}
