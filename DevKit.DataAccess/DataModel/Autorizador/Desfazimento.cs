using System;
using System.Linq;
using LinqToDB;
using DataModel;
using System.Diagnostics;
using System.Text;

namespace DataModel
{
    public class VendaEmpresarialDesfazimento : BaseVenda
    {
        #region - variaveis - 

        public AutorizadorCNDB db;

        public POS_Entrada input_cont_pe = new POS_Entrada();
        public POS_Resposta output_cont_pr = new POS_Resposta();

        public LOG_Transaco var_ltr = new LOG_Transaco();
        public T_Cartao cartPortador = new T_Cartao();

        public string output_st_msg = "",
                      var_vr_total = "0",
                      var_operacaoCartao = "0",
                      var_operacaoCartaoFail = "0",
                      var_codResp = "0";

        public long input_st_nsu;

        #endregion

        public void Run(AutorizadorCNDB _db, string nsuOrigem)
        {
            var st = new Stopwatch();

            st.Start();

            db = _db;
            input_st_nsu = Convert.ToInt64(nsuOrigem);

            SetupFile();

            var_operacaoCartao = OperacaoCartao.VENDA_EMPRESARIAL_DESFAZ;
            var_operacaoCartaoFail = OperacaoCartao.VENDA_EMPRESARIAL_DESFAZ;

            Registry("-------------------------");
            Registry("START VendaEmpresarialDesfazimento");
            Registry("-------------------------");

            try
            {
                var_codResp = "9999";

                if (Authenticate())
                {
                    var_codResp = "0000";

                    Execute();
                }                   

                Finish();
            }
            catch (SystemException ex)
            {
                Registry("-------------------------");
                Registry("*ERROR! " + ex.ToString());
                Registry("-------------------------");
            }

            st.Stop();

            Registry("-------------------------");
            Registry("Finalizado! tempo: " + st.ElapsedMilliseconds.ToString());
            Registry("-------------------------");

            Registry("Resultado: " + output_st_msg);
            Registry("Código de resposta: " + var_codResp);
            Registry("-------------------------");

            CloseFile();
        }

        private bool Authenticate()
        {
            Registry("-------------------------");
            Registry("Authenticate");
            Registry("-------------------------");

            var st = new Stopwatch(); st.Start();

            #region - code -

            {
                var dtHoje = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var dtHojeFim = dtHoje.AddDays(1).AddSeconds(-1);

                var q = db.LOG_Transacoes.Where(y => y.nu_nsuOrig == input_st_nsu && 
                                                     y.en_operacao == OperacaoCartao.VENDA_EMPRESARIAL && 
                                                     y.dt_transacao > dtHoje &&
                                                     y.dt_transacao < dtHojeFim );

                Registry("(a1) db.LOG_Transacoes.Where(y => y.nu_nsu == input_st_nsu && y.en_operacao == OperacaoCartao.VENDA_EMPRESARIAL && y.dt_transacao > dtHoje && y.dt_transacao < dtHojeFim" );
                Registry("(a2) db.LOG_Transacoes.Where(y => y.nu_nsu == " + input_st_nsu + 
                                                      " && y.en_operacao == " + OperacaoCartao.VENDA_EMPRESARIAL +
                                                      " && y.dt_transacao > " + dtHoje + 
                                                      " && y.dt_transacao <  "+ dtHojeFim);

                var_ltr = q.FirstOrDefault();

                if (var_ltr == null)
                {
                    output_st_msg = "Nsu (" + input_st_nsu + ") inválido";
                    var_codResp = "1212";
                    return false;
                }
            }

            {
                var q = db.T_Cartao.Where(y => y.i_unique == var_ltr.fk_cartao );

                Registry("(a3) db.T_Cartao.Where(y => y.i_unique == var_ltr.fk_cartao )");
                Registry("(a4) db.T_Cartao.Where(y => y.i_unique == " + var_ltr.fk_cartao );

                cartPortador = q.FirstOrDefault();

                if (cartPortador == null)
                {
                    output_st_msg = "Cartão inválido";
                    var_codResp = "1213";
                    return false;
                }
            }

            #endregion

            st.Stop();

            Registry("-------------------------");
            Registry("Authenticate DONE, tempo: " + st.ElapsedMilliseconds.ToString());
            Registry("-------------------------");

            return true;
        }

        private bool Execute()
        {
            Registry("-------------------------");
            Registry("Execute");
            Registry("-------------------------");

            #region - code -

            var st = new Stopwatch(); st.Start();

            Registry("(x1) Atualizando var_ltr.tg_confirmada...");

            var_ltr.tg_confirmada = Convert.ToChar (TipoConfirmacao.Cancelada);

            db.Update(var_ltr);

            Registry("(x2) OK!");

            #endregion

            st.Stop();

            Registry("-------------------------");
            Registry("Execute DONE, tempo: " + st.ElapsedMilliseconds.ToString());
            Registry("-------------------------");

            return true;
        }

        private bool Finish()
        {
            Registry("-------------------------");
            Registry("Finish");
            Registry("-------------------------");

            var st = new Stopwatch(); st.Start();

            #region - code - 

            #endregion

            Registry("-------------------------");
            Registry("Finish DONE, tempo: " + st.ElapsedMilliseconds.ToString());
            Registry("-------------------------");

            return true;
        }
    }
}
