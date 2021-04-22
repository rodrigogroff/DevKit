using LinqToDB;
using System.Web.Http;
using System.IO;
using System.Linq;
using SocialExplorer.IO.FastDBF;
using DataModel;
using System;

namespace DevKit.Web.Controllers
{
    public class EmissoraFechamentoExpController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/EmissoraFechamentoExp/exportar", Name = "EmissoraFechamentoExp")]
        public IHttpActionResult Exportar()
        {
            try
            {
                var emp = Request.GetQueryStringValue("emp");
                var mes = Request.GetQueryStringValue("mes").PadLeft(2, '0');
                var ano = Request.GetQueryStringValue("ano");

                if (!StartDatabaseAndAuthorize())
                    return BadRequest();

                var tEmpresa = new T_Empresa();

                try
                {
                    tEmpresa = (from e in db.T_Empresa
                                where e.i_unique == Convert.ToInt32(emp)
                                select e).
                                FirstOrDefault();
                }
                catch (System.Exception ex)
                {
                    ex.ToString();

                    tEmpresa = (from e in db.T_Empresa
                                where e.st_fantasia == emp
                                select e).
                                FirstOrDefault();
                }
                
                string dir = "C:\\fechamento_dbf",
                        file = "F" + tEmpresa.st_empresa + "-" + mes + ano.Substring(2),
                        ext = "DBF";

                var odbf = new DbfFile(System.Text.Encoding.Default);
                odbf.Open(Path.Combine(dir, file + "." + ext), FileMode.Create);

                odbf.Header.AddColumn(new DbfColumn("data", DbfColumn.DbfColumnType.Date));
                odbf.Header.AddColumn(new DbfColumn("nsu", DbfColumn.DbfColumnType.Character, 6, 0));
                odbf.Header.AddColumn(new DbfColumn("matricula", DbfColumn.DbfColumnType.Character, 6, 0));

                odbf.Header.AddColumn(new DbfColumn("valor", DbfColumn.DbfColumnType.Float, 10, 5));
                odbf.Header.AddColumn(new DbfColumn("parcela", DbfColumn.DbfColumnType.Character, 6, 0));
                odbf.Header.AddColumn(new DbfColumn("cnpj", DbfColumn.DbfColumnType.Character, 14, 0));

                // ------------------------------------------------------
                // acrescenta o movimento do mes/ano desejado
                // ------------------------------------------------------

                var lstFechamento = (from e in db.LOG_Fechamento
                                     join cart in db.T_Cartao on e.fk_cartao equals (int) cart.i_unique
                                     where e.fk_empresa == tEmpresa.i_unique
                                     where e.st_mes == mes
                                     where e.st_ano == ano
                                     orderby cart.st_matricula
                                     select e).
                                     ToList();

                var lstIdsLojistas = lstFechamento.Select(y => y.fk_loja).
                                     Distinct().
                                     ToList();

                var lstLojistas = (from e in db.T_Loja
                                   where lstIdsLojistas.Contains((int)e.i_unique)
                                   select e).
                                   ToList();

                var lstIdsParcelas = lstFechamento.Select(y => y.fk_parcela).
                                     Distinct().
                                     ToList();

                var lstParcelas = (from e in db.T_Parcelas
                                   where lstIdsParcelas.Contains((int)e.i_unique)
                                   select e).
                                   ToList();

                var lstIdsCartoes = lstFechamento.Select(y => y.fk_cartao).
                                    Distinct().
                                    ToList();

                var lstCartoes = (from e in db.T_Cartao
                                  where lstIdsCartoes.Contains((int)e.i_unique)
                                  select e).
                                   ToList();

                foreach (var item in lstFechamento)
                {
                    var lojista = lstLojistas.Where(y => y.i_unique == item.fk_loja).FirstOrDefault();
                    var parc = lstParcelas.Where(y => y.i_unique == item.fk_parcela).FirstOrDefault();
                    var cart = lstCartoes.Where(y => y.i_unique == item.fk_cartao).FirstOrDefault();

                    string vr_val = parc.vr_valor.ToString().PadLeft(3, '0');

                    vr_val = vr_val.Insert(vr_val.Length - 2, ".");

                    //add some records...
                    var orec = new DbfRecord(odbf.Header);

                    orec[0] = ObtemData(parc.dt_inclusao).Substring(0, 10);
                    orec[1] = parc.nu_nsu.ToString().PadLeft(6, '0');
                    orec[2] = cart.st_matricula;
                    orec[3] = vr_val;
                    orec[4] = parc.nu_indice + "/"+ parc.nu_tot_parcelas;
                    orec[5] = lojista.nu_CNPJ;

                    odbf.Write(orec, true);
                }

                {
                    var query = (from e in db.LancamentosCC
                                 where e.fkEmpresa == tEmpresa.i_unique
                                 where e.nuAno == Convert.ToInt32(ano)
                                 where e.nuMes == Convert.ToInt32(mes)                                 
                                 select e);

                    foreach (var item in query.ToList())
                    {
                        var t_cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == item.fkCartao);

                        var orec = new DbfRecord(odbf.Header);

                        orec[0] = ObtemData(item.dtLanc).Substring(0, 10);
                        orec[1] = " ";
                        orec[2] = t_cart.stCodigoFOPA;
                        orec[3] = item.vrValor.ToString();
                        orec[4] = item.nuParcela + "/" + item.nuTotParcelas;
                        orec[5] = " ";

                        odbf.Write(orec, true);
                    }
                }

                // -----------
                // finaliza
                // -----------

                odbf.WriteHeader();
                odbf.Close();               

                return ResponseMessage(TransferirConteudo(dir, file, ext));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
