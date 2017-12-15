using LinqToDB;
using System.Web.Http;
using System.IO;
using System.Linq;
using System.Collections;

namespace DevKit.Web.Controllers
{
    public class EmissoraFechamentoExpController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/EmissoraFechamentoExp/exportar", Name = "EmissoraFechamentoExp")]
        public IHttpActionResult Exportar()
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

            string dir = Directory.GetCurrentDirectory(),
                    file = "f" + tEmpresa.i_unique + mes + ano.Substring(2),
                    ext = "dbf";

            var dbf = new DBfileGen ( dir, file, ext);

            dbf.addCampo("data", "date");
            dbf.addCampo("nsu", "varchar(6)");
            dbf.addCampo("matricula", "varchar(6)");
            dbf.addCampo("valor", "currency");
            dbf.addCampo("parcela", "varchar(6)");
            dbf.addCampo("cnpj", "varchar(14)");

            // ------------------------------------------------------
            // acrescenta o movimento do mes/ano desejado
            // ------------------------------------------------------

            var lstFechamento = (from e in db.LOG_Fechamento
                                 where e.fk_empresa == tEmpresa.i_unique
                                 where e.st_mes == mes
                                 where e.st_ano == ano
                                 select e).
                                 ToList();

            var lstIdsLojistas = lstFechamento.Select(y => y.fk_loja).Distinct().ToList();

            var lstLojistas = (from e in db.T_Loja
                               where lstIdsLojistas.Contains((int)e.i_unique)
                               select e).
                               ToList();

            var lstIdsParcelas = lstFechamento.Select(y => y.fk_parcela).Distinct().ToList();

            var lstParcelas = (from e in db.T_Parcelas
                               where lstIdsParcelas.Contains((int)e.i_unique)
                               select e).
                               ToList();

            var lstIdsCartoes = lstFechamento.Select(y => y.fk_cartao).Distinct().ToList();

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

                dbf.addReg(new ArrayList
                {
                    ObtemData(item.dt_compra).Substring(0, 10),
                    parc.nu_nsu.ToString().PadLeft(6, '0'),
                    cart.st_matricula,
                    vr_val,
                    item.nu_parcela,
                    lojista.nu_CNPJ
                });
            }
            
            // -----------
            // finaliza
            // -----------

            dbf.save();

            return ResponseMessage ( TransferirConteudo(dir, file, ext) );
        }
    }
}
