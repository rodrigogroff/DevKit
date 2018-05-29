using System;
using System.Linq;
using LinqToDB;
using DataModel;
using System.Diagnostics;
using System.Text;

namespace DataModel
{
    public class VendaEmpresarialConfirmacao : BaseVenda
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

        public void Run(AutorizadorCNDB _db, string nsu)
        {
            var st = new Stopwatch();

            st.Start();

            db = _db;
            input_st_nsu = Convert.ToInt64(nsu);

            SetupFile();

            var_operacaoCartao = OperacaoCartao.VENDA_EMPRESARIAL;
            var_operacaoCartaoFail = OperacaoCartao.FALHA_VENDA_EMPRESARIAL;

            Registry("-------------------------");
            Registry("START VendaEmpresarialConfirmacao" );
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

                var q = db.LOG_Transacoes.Where(y => y.nu_nsu == input_st_nsu && 
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

            var_ltr.tg_confirmada = Convert.ToChar (TipoConfirmacao.Confirmada);

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

            Registry("(f1) db.T_Empresa.FirstOrDefault( y=> y.i_unique == " + var_ltr.fk_empresa);

            var emp = db.T_Empresa.FirstOrDefault( y=> y.i_unique == var_ltr.fk_empresa);

            if (emp == null)
            {
                output_st_msg = "Erro interno";
                var_codResp = "1301";
                return false;
            }

            Registry("(f2) db.T_Terminal.FirstOrDefault( y=> y.i_unique == " + var_ltr.fk_terminal);

            var term = db.T_Terminal.FirstOrDefault(y => y.i_unique == var_ltr.fk_terminal);

            if (term == null)
            {
                output_st_msg = "Erro interno";
                var_codResp = "1302";
                return false;
            }

            Registry("(f3) db.T_Loja.FirstOrDefault( y=> y.i_unique == " + var_ltr.fk_loja);

            var loj = db.T_Loja.FirstOrDefault(y => y.i_unique == var_ltr.fk_loja);

            if (loj == null)
            {
                output_st_msg = "Erro interno";
                var_codResp = "1303";
                return false;
            }

            Registry("(f4) db.T_Proprietario.FirstOrDefault( y=> y.i_unique == " + cartPortador.fk_dadosProprietario);

            var prot = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cartPortador.fk_dadosProprietario);

            output_cont_pr.st_codResp = var_codResp;
            output_cont_pr.st_nsuRcb = input_st_nsu.ToString().PadLeft(6, '0');

            output_cont_pr.st_nsuBanco = new StringBuilder().Append(DateTime.Now.Year.ToString())
                                                                       .Append(DateTime.Now.Month.ToString("00"))
                                                                       .Append(DateTime.Now.Day.ToString("00"))
                                                                       .Append(input_st_nsu.ToString().PadLeft(6, '0')).ToString();

            output_cont_pr.st_PAN = cartPortador.st_empresa + cartPortador.st_matricula;
            output_cont_pr.st_mesPri = Context.EMPTY;
            output_cont_pr.st_loja  =loj.st_loja;

            output_cont_pr.st_nomeCliente = prot.st_nome;

            Registry("(f5) db.T_Dependente.FirstOrDefault( y=> y.fk_proprietario == " + cartPortador.fk_dadosProprietario + " && y.nu_titularidade == " + cartPortador.st_titularidade);

            var dep_f = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cartPortador.fk_dadosProprietario && 
                                                            y.nu_titularidade == Convert.ToInt32(cartPortador.st_titularidade));

            if (dep_f != null)
                output_cont_pr.st_nomeCliente = dep_f.st_nome;

            st.Stop();

            #endregion

            Registry("-------------------------");
            Registry("Finish DONE, tempo: " + st.ElapsedMilliseconds.ToString());
            Registry("-------------------------");

            return true;
        }
    }
}
