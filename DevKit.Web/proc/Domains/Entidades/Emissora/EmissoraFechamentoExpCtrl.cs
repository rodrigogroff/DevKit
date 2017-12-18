using LinqToDB;
using System.Web.Http;
using System.IO;
using System.Linq;
using SocialExplorer.IO.FastDBF;

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

                var tEmpresa = (from e in db.T_Empresa
                                where e.st_fantasia == emp
                                select e).
                                FirstOrDefault();

                string dir = "C:\\fechamento_dbf",
                        file = "F" + tEmpresa.i_unique + mes + ano.Substring(2),
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

                // -----------
                // finaliza
                // -----------

                odbf.WriteHeader();

                odbf.Close();

                //dbf.save();

                return ResponseMessage(TransferirConteudo(dir, file, ext));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
