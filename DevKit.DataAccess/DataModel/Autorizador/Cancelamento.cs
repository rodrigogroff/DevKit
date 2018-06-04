using System;
using System.Linq;
using LinqToDB;
using DataModel;
using System.Diagnostics;
using System.Text;

namespace DataModel
{
    public class VendaEmpresarialCancelamento : BaseVenda
    {
        #region - variaveis - 

        public AutorizadorCNDB db;

        public POS_Entrada input_cont_pe = new POS_Entrada();
        public POS_Resposta output_cont_pr = new POS_Resposta();

        /// USER [ var_decl ]

        LOG_Transaco old_l_tr = new LOG_Transaco();
        T_Cartao cart = new T_Cartao();

        public string   input_st_nsu_cancel = "",
                        input_dt_hoje = "",
                        input_id_user = "",
                        output_st_msg = "",
                        var_nu_nsuAtual = "0",
                        var_codResp = "0",
                        var_operacaoCartao = "0",
                        var_operacaoCartaoFail = "0",
                        valor = "0",
                        dt_orig = "";

        public DateTime start, end;
        
        #endregion

        public void Run(AutorizadorCNDB _db, string nsu)
        {
            var st = new Stopwatch();

            st.Start();

            db = _db;

            SetupFile();

            var_operacaoCartao = OperacaoCartao.VENDA_EMPRESARIAL_CANCELA;
            var_operacaoCartaoFail = OperacaoCartao.FALHA_VENDA_EMPRESARIAL_CANCELA;

            Registry("-------------------------");
            Registry("START Cancelamento");
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

            var_codResp = "0606";

            if (input_dt_hoje != "")
            {
                start = ObtemData(input_dt_hoje);
                end = start.AddDays(1);

                // ## Buscar transação de NSU com data especifica

                old_l_tr = db.LOG_Transacoes.FirstOrDefault(y => y.nu_nsu.ToString() == input_st_nsu_cancel &&
                                                                y.en_operacao == OperacaoCartao.VENDA_EMPRESARIAL &&
                                                                y.dt_transacao > start && y.dt_transacao < end);

                Registry("db.LOG_Transacoes.FirstOrDefault(y => y.nu_nsu.ToString() == " + input_st_nsu_cancel + " && y.en_operacao == " + OperacaoCartao.VENDA_EMPRESARIAL + " && y.dt_transacao > " + start + " && y.dt_transacao < " + end);

                if (old_l_tr == null)
                {
                    output_st_msg = "NSU inválido (" + input_st_nsu_cancel.TrimStart('0') + ")";
                    var_codResp = "1212";
                    return false;
                }
            }
            else
            {
                start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                end = start.AddDays(1);

                old_l_tr = db.LOG_Transacoes.FirstOrDefault(y => y.nu_nsu.ToString() == input_st_nsu_cancel &&
                                                                 y.en_operacao == OperacaoCartao.VENDA_EMPRESARIAL &&
                                                                 y.dt_transacao > start && y.dt_transacao < end);

                Registry("db.LOG_Transacoes.FirstOrDefault(y => y.nu_nsu.ToString() == " + input_st_nsu_cancel + " && y.en_operacao == " + OperacaoCartao.VENDA_EMPRESARIAL + " && y.dt_transacao > " + start + " && y.dt_transacao < " + end);

                if (old_l_tr == null)
                {
                    output_st_msg = "NSU inválido (" + input_st_nsu_cancel.TrimStart('0') + ")";
                    var_codResp = "1212";
                    return false;
                }
            }
            
            valor = old_l_tr.vr_total.ToString();
            
            var term = db.T_Terminal.FirstOrDefault( y=> y.i_unique == old_l_tr.fk_terminal);

            Registry("db.T_Terminal.FirstOrDefault( y=> y.i_unique == " + old_l_tr.fk_terminal);

            if (term == null)
            {
                output_st_msg = "Erro aplicativo";
                var_codResp = "0101";
                return false;
            }

            cart = db.T_Cartao.FirstOrDefault(y => y.i_unique.ToString() == old_l_tr.fk_cartao.ToString());

            Registry("db.T_Cartao.FirstOrDefault( y=> y.i_unique == " + old_l_tr.fk_cartao);

            if (cart == null)
            {
                output_st_msg = "Erro aplicativo";
                var_codResp = "0102";
                return false;
            }

            if (old_l_tr.tg_confirmada.ToString() == TipoConfirmacao.Cancelada)
            {
                output_st_msg = "prev. cancel";
                var_codResp = "N3N3";
                return false;
            }

            Registry("Confirmando!");

            old_l_tr.tg_confirmada = Convert.ToChar(TipoConfirmacao.Cancelada);

            db.Update(old_l_tr);

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

            var st = new Stopwatch(); st.Start();

            #region - code - 

            var parcs = db.T_Parcelas.Where(y => y.nu_nsu.ToString() == input_st_nsu_cancel && 
                                                 y.dt_inclusao > start && y.dt_inclusao < end).
                                            ToList();

            Registry("db.T_Parcelas.Where(y => y.nu_nsu == " + input_st_nsu_cancel +
                     " && y.dt_inclusao > " + start + " && y.dt_inclusao < " + end);

            if (!parcs.Any())
            {
                output_st_msg = "erro aplicativo";
                var_codResp = "0103";                
                return false;
            }

            output_st_msg = "NSU: " + input_st_nsu_cancel.TrimStart('0');

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

            var l_nsu = new LOG_NSU
            {
                dt_log = DateTime.Now
            };

            l_nsu.i_unique = Convert.ToInt64(db.InsertWithIdentity(l_nsu));

            var_nu_nsuAtual = l_nsu.i_unique.ToString();

            if (var_codResp != "0000")
                var_operacaoCartao = var_operacaoCartaoFail;

            Registry("db.T_Empresa.FirstOrDefault( y=> y.i_unique.ToString() == " + old_l_tr.fk_empresa);

            var emp = db.T_Empresa.FirstOrDefault( y=> y.i_unique.ToString() == old_l_tr.fk_empresa.ToString());

            Registry("db.T_Terminal.FirstOrDefault( y=> y.i_unique.ToString() == " + old_l_tr.fk_terminal);

            var term = db.T_Terminal.FirstOrDefault(y => y.i_unique.ToString() == old_l_tr.fk_terminal.ToString());

            Registry("db.T_Loja.FirstOrDefault( y=> y.i_unique.ToString() == " + old_l_tr.fk_loja);

            var loj = db.T_Loja.FirstOrDefault(y => y.i_unique.ToString() == old_l_tr.fk_loja.ToString());

            output_cont_pr.st_codResp = var_codResp;
            output_cont_pr.st_nsuRcb = var_nu_nsuAtual.PadLeft(6, '0');
            output_cont_pr.st_nsuBanco = new StringBuilder().Append(DateTime.Now.Year.ToString())
                                                                       .Append(DateTime.Now.Month.ToString("00"))
                                                                       .Append(DateTime.Now.Day.ToString("00"))
                                                                       .Append(var_nu_nsuAtual.PadLeft(6, '0')).
                                                                       ToString();

            output_cont_pr.st_PAN = cart.st_empresa + cart.st_matricula;
            output_cont_pr.st_mesPri = Context.EMPTY;
            output_cont_pr.st_loja = loj.st_loja;            

            if (cart.st_titularidade != "01")
            {
                Registry("db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == " + cart.fk_dadosProprietario + " && y.nu_titularidade == " + cart.st_titularidade);

                var dep_f = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cart.fk_dadosProprietario &&
                                                                y.nu_titularidade.ToString() == cart.st_titularidade );
                 
                if (dep_f != null)
                    output_cont_pr.st_nomeCliente = dep_f.st_nome;                
            }            
            else
            {
                Registry("db.T_Proprietario.FirstOrDefault(y => y.i_unique.ToString() == " + cart.fk_dadosProprietario);
                    
                var prot = db.T_Proprietario.FirstOrDefault(y => y.i_unique.ToString() == cart.fk_dadosProprietario.ToString());

                output_cont_pr.st_nomeCliente = prot != null ? prot.st_nome : "";
            }

            var l_tr = new LOG_Transaco
            {
                fk_terminal = old_l_tr.fk_terminal,
                fk_empresa = old_l_tr.fk_empresa,
                fk_cartao = old_l_tr.fk_cartao,
                vr_total = old_l_tr.vr_total,
                nu_parcelas = old_l_tr.nu_parcelas,
                nu_nsu = (int)l_nsu.i_unique,
                dt_transacao = DateTime.Now,
                nu_cod_erro = Convert.ToInt32(output_cont_pr.st_codResp),
                nu_nsuOrig = 0,
                en_operacao = var_operacaoCartao
            };

            if (var_codResp != "0000")
                l_tr.tg_confirmada = Convert.ToChar(TipoConfirmacao.Erro);
            else
                l_tr.tg_confirmada = Convert.ToChar(TipoConfirmacao.Cancelada);

            if (input_cont_pe.st_terminalSITEF != "")
                l_tr.st_msg_transacao = input_cont_pe.st_terminalSITEF;
            else
                l_tr.st_msg_transacao = output_st_msg;

            l_tr.fk_loja = term.fk_loja;

            db.Insert(l_tr);

            #endregion

            Registry("-------------------------");
            Registry("Finish DONE, tempo: " + st.ElapsedMilliseconds.ToString());
            Registry("-------------------------");

            return true;
        }
    }
}
