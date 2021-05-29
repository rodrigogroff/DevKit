using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class DtoExtratoUsuario_LancFech
    {
        public string cod { get; set; }
        public string estab { get; set; }
        public string nsu { get; set; }
        public string vlrTot { get; set; }
        public string vlrParc { get; set; }
        public string parc { get; set; }
    }

    public class DtoExtratoUsuario_LancCC
    {
        public string cod { get; set; }
        public string desc { get; set; }
        public string vlrParc { get; set; }
        public string parc { get; set; }
    }

    public class DtoEmissoraLancCCExtratoUsuario
    {
        public string nome { get; set; }
        public string mat { get; set; }
        public string fopa { get; set; }
        public string vlrCartao { get; set; }
        public string vlrDespCC { get; set; }
        public string vlrTotCC { get; set; }

        public List<DtoExtratoUsuario_LancFech> listConvenio;
        public List<DtoExtratoUsuario_LancCC> listDespCC;
    }

    public class EmissoraLancCCExtratoUsuarioController : ApiControllerBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));
            var mat = Request.GetQueryStringValue("mat");

            if (string.IsNullOrEmpty(mat))
                return BadRequest("Informe uma matricula");

            mat = mat.PadLeft(6, '0');
            
            var mon = new money();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            var t_cart = db.T_Cartao.FirstOrDefault(y => y.st_matricula == mat && y.st_empresa == tEmp.st_empresa);

            if (t_cart == null)
                return BadRequest("Informe uma matricula válida");

            var prop = db.T_Proprietario.FirstOrDefault(y => y.i_unique == t_cart.fk_dadosProprietario);

            // ---------------------------------------------------------------

            var queryLancCC = ( from e in db.LancamentosCC
                                where e.fkEmpresa == tEmp.i_unique
                                where e.fkCartao == t_cart.i_unique
                                where e.nuAno == ano
                                where e.nuMes == mes
                                select e).
                                ToList();

            var lstCC = new List<DtoExtratoUsuario_LancCC>();

            var tmp_lstCCsEmp = db.EmpresaDespesa.Where(y => y.fkEmpresa == tEmp.i_unique).ToList();
            var tmp_lstCCsEmpRec = db.EmpresaDespesaRecorrente.Where(y => y.fkEmpresa == tEmp.i_unique).ToList();

            foreach (var item in queryLancCC)
            {
                if (item.bRecorrente == true)
                {
                    var d = tmp_lstCCsEmpRec.FirstOrDefault(y => y.id == item.fkTipo);

                    lstCC.Add(new DtoExtratoUsuario_LancCC
                    {
                        cod = d.stCodigo,
                        desc = d.stDescricao,
                        parc = item.nuParcela.ToString(),
                        vlrParc = mon.setMoneyFormat((long)item.vrValor),
                    });
                }
                else
                {
                    var d = tmp_lstCCsEmp.FirstOrDefault(y => y.id == item.fkTipo);

                    lstCC.Add(new DtoExtratoUsuario_LancCC
                    {
                        cod = d.stCodigo,
                        desc = d.stDescricao,
                        parc = "Recorrente",
                        vlrParc = mon.setMoneyFormat((long)item.vrValor),
                    });
                }
            }

            // ---------------------------------------------------------------

            var queryLancFech = (from e in db.LOG_Fechamento
                               where e.fk_empresa == tEmp.i_unique
                               where e.fk_cartao == t_cart.i_unique
                               where e.st_ano == ano.ToString()
                               where e.st_mes == mes.ToString().PadLeft(2, '0')
                               select e).
                                ToList();

            var tmp_lstIDsLojistas = queryLancFech.Select(y => (long)y.fk_loja).Distinct().ToList();
            var tmp_lstLojista = db.T_Loja.Where(a => tmp_lstIDsLojistas.Contains((long)a.i_unique)).ToList();

            var tmp_lstIDsParcelas = queryLancFech.Select(y => (long)y.fk_parcela).Distinct().ToList();
            var tmp_parcela = db.T_Parcelas.Where(a => tmp_lstIDsParcelas.Contains((long)a.i_unique)).ToList();
            var tmp_lstIDsLogTrans = tmp_parcela.Select(y => (long)y.fk_log_transacoes).Distinct().ToList();
            var tmp_logtrans = db.LOG_Transacoes.Where(a => tmp_lstIDsLogTrans.Contains((long)a.i_unique)).ToList();

            var lstFech = new List<DtoExtratoUsuario_LancFech>();

            foreach (var item in queryLancFech)
            {
                var loj = tmp_lstLojista.FirstOrDefault(y => y.i_unique == item.fk_loja);
                var parc = tmp_parcela.FirstOrDefault(y => y.i_unique == item.fk_parcela);
                var trans = tmp_logtrans.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                lstFech.Add(new DtoExtratoUsuario_LancFech
                {
                    cod = loj.st_loja,
                    estab = loj.st_nome,
                    nsu = parc.nu_nsu.ToString(),
                    parc = parc.nu_indice.ToString(),
                    vlrParc = mon.setMoneyFormat((long)parc.vr_valor),
                    vlrTot = mon.setMoneyFormat((long)trans.vr_total),
                });
            }

            // ---------------------------------------------------------------

            var totCC = queryLancCC.Sum(y => (long)y.vrValor);
            var totFech = queryLancFech.Sum(y => (long)y.vr_valor);

            return Ok(new DtoEmissoraLancCCExtratoUsuario
            {
                nome = prop.st_nome,
                mat = t_cart.st_matricula,
                fopa = t_cart.stCodigoFOPA,

                vlrCartao = "R$ " + mon.setMoneyFormat(totFech),
                vlrDespCC = "R$ " + mon.setMoneyFormat(totCC),
                vlrTotCC = "R$ " + mon.setMoneyFormat(totCC + totFech),

                listConvenio = lstFech,
                listDespCC = lstCC,
            }); 
        }        
    }
}
