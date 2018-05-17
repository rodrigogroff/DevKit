using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;
using DataModel;
using System.Collections;

namespace DevKit.Web.Controllers
{
    public class VendaEmpresarial : BaseVenda
    {
        #region - variaveis - 

        public AutorizadorCNDB db;

        public POS_Entrada input_cont_pe = new POS_Entrada();
        public POS_Resposta output_cont_pr = new POS_Resposta();

        public T_Cartao cart;
        public T_Empresa emp;
        public T_Terminal term;
        public T_InfoAdicionai info;
        public LOG_NSU l_nsu;
        public T_Proprietario prot;
        public T_Loja loj;

        public string output_st_msg = "",
                      var_vr_total = "0",
                      var_nu_parcelas = "0",
                      var_operacaoCartao = "0",
                      var_operacaoCartaoFail = "0",
                      var_nu_nsuAtual = "0",
                      var_nu_nsuOrig = "0",
                      var_nu_nsuEntidade = "0",
                      var_nu_nsuEntOrig = "0",
                      var_codResp = "0",
                      var_nomeCliente = "",
                      var_dt_transacao = "",
                      st_doc = "";

        public long   vr_dispMes, 
                      vr_dispTot,
                      vr_valor;

        public bool   IsDigitado = false;
        
        public ArrayList lstParcs = new ArrayList();

        #endregion

        public void Run(AutorizadorCNDB _db)
        {
            db = _db;
            SetupFile();

            if (Authenticate())
            {

            }

            CloseFile();
        }

        private bool Authenticate()
        {
            Registry("authenticate exec_pos_vendaEmpresarial ");

            // Default é erro genérico
            var_codResp = "9999";

            // Normal
            var_nu_nsuAtual = Context.NONE;
            var_nu_nsuEntidade = Context.NONE;

            // Cancelamento
            var_nu_nsuOrig = Context.NONE;
            var_nu_nsuEntOrig = Context.NONE;

            // Valores básicos de comércio
            var_vr_total = input_cont_pe.vr_valor;
            var_nu_parcelas = input_cont_pe.nu_parcelas;

            var_codResp = "0606";

            #region - valida terminal - 

            // --------------------------------------------
            // ## Busca terminal pelo seu código
            // --------------------------------------------

            //00000212

            {
                var q = db.T_Terminal.Where(y => y.nu_terminal == input_cont_pe.st_terminal.PadLeft(8, '0'));

                Registry(q.ToString());

                term = q.FirstOrDefault();

                if (term == null)
                {
                    output_st_msg = "Terminal inexistente";
                    var_codResp = "0303";
                    return false;
                }
            }

            /*
            if (!term.select_rows_terminal(input_cont_pe.get_st_terminal()))
            {
                output_st_msg = "Terminal inexistente";
                var_codResp = "0303";
                return false;
            }

            if (!term.fetch())
            {
                output_st_msg = "Erro aplicativo";
                return false;
            }
            */

            #endregion
            /*
            #region - valida empresa - 

            // ## Busca empresa informada

            if (!emp.select_rows_empresa(input_cont_pe.get_st_empresa()))
            {
                output_st_msg = "Empresa inexistente";
                var_codResp = "0303";
                return false;
            }

            if (!emp.fetch())
            {
                output_st_msg = "Erro de aplicativo";
                return false;
            }

            // ## Caso empresa bloqueada, sair

            if (emp.get_tg_bloq() == Context.TRUE)
            {
                output_st_msg = "Empresa bloqueada";
                var_codResp = "0303";
                return false;
            }

            #endregion

            #region - valida relação da Loja do Terminal com Empresa (Convênio)

            LINK_LojaEmpresa loj_emp = new LINK_LojaEmpresa(this);

            if (!loj_emp.select_fk_empresa_loja(emp.get_identity(), term.get_fk_loja()))
            {
                output_st_msg = "Terminal não conveniado";
                var_codResp = "0303";
                return false;
            }

            if (!loj.selectIdentity(term.get_fk_loja()))
            {
                output_st_msg = "Erro aplicativo";
                return false;
            }

            if (loj.get_tg_blocked() == Context.TRUE)
            {
                output_st_msg = "Loja bloqueada";
                var_codResp = "0303";
                return false;
            }

            if (loj.get_tg_cancel() == Context.TRUE)
            {
                output_st_msg = "Loja cancelada";
                var_codResp = "0303";
                return false;
            }

            #endregion

            #region - valida cartão - 

            if (!cart.select_rows_tudo(input_cont_pe.get_st_empresa(),
                                           input_cont_pe.get_st_matricula(),
                                             input_cont_pe.get_st_titularidade()))
            {
                output_st_msg = "Cartão inexistente";
                var_codResp = "0606";
                return false;
            }

            if (!cart.fetch())
            {
                output_st_msg = "Erro aplicativo";
                return false;
            }

            // ## Verifica bloqueio 

            if (cart.get_tg_status() == CartaoStatus.Bloqueado)
            {
                output_st_msg = "Cartão inválido";
                var_codResp = "0505";
                return false;
            }

            if (cart.get_tg_emitido() != StatusExpedicao.Expedido)
            {
                output_st_msg = "Cartão inválido";
                var_codResp = "0505";
                return false;
            }

            if (cart.get_tg_tipoCartao() == TipoCartao.educacional)
            {
                // ## No caso educacional, permitir somente venda 
                // ## em uma parcela

                if (input_cont_pe.get_nu_parcelas().TrimStart('0') != "1")
                {
                    output_st_msg = "Somente uma parcela";
                    var_codResp = "0606";
                    return false;
                }
            }

            #endregion

            var_vr_total = input_cont_pe.get_vr_valor();
            var_nu_parcelas = input_cont_pe.get_nu_parcelas();

            SQL_LOGGING_ENABLE = false;

            #region - Verifica disponivel mensal nas parcelas - 

            T_Parcelas parc = new T_Parcelas(this);

            string myId = cart.get_identity();

            if (cart.get_st_titularidade() != "01")
            {
                cart.select_rows_tudo(cart.get_st_empresa(),
                                           cart.get_st_matricula(),
                                           "01");
                cart.fetch();
            }

            vr_dispMes = cart.get_int_vr_limiteMensal() + cart.get_int_vr_extraCota();
            vr_dispTot = cart.get_int_vr_limiteTotal() + cart.get_int_vr_extraCota();

            vr_valor = Convert.ToInt64(input_cont_pe.get_vr_valor());

            if (cart.get_tg_tipoCartao() != TipoCartao.presente)
            {
                new ApplicationUtil().GetSaldoDisponivel(ref cart, ref vr_dispMes, ref vr_dispTot);

                int tmp_nu_parc = Convert.ToInt32(input_cont_pe.get_nu_parcelas());

                if (tmp_nu_parc > 1)
                {
                    if (vr_valor > vr_dispTot)
                    {
                        output_st_msg = "limite excedido";
                        var_codResp = "2721";

                        SQL_LOGGING_ENABLE = true;

                        return false;
                    }

                    LOG_Transacoes ltr = new LOG_Transacoes(this);
                    T_Parcelas parcTot = new T_Parcelas(this);

                    string tmp = input_cont_pe.get_st_valores();

                    ArrayList lstCartoes = new ArrayList();

                    T_Cartao c_t = new T_Cartao(this);

                    c_t.select_rows_empresa_matricula(cart.get_st_empresa(),
                                                        cart.get_st_matricula());

                    while (c_t.fetch())
                        lstCartoes.Add(c_t.get_identity());

                    for (int t = 1, index_pos = 0; t <= tmp_nu_parc; ++t)
                    {
                        long valor_unit_parc = Convert.ToInt64(tmp.Substring(index_pos, 12));

                        index_pos += 12;

                        if (valor_unit_parc > cart.get_int_vr_limiteMensal())
                        {
                            output_st_msg = "limite excedido";
                            var_codResp = "2722";

                            SQL_LOGGING_ENABLE = true;

                            return false;
                        }

                        long dispMesParc = cart.get_int_vr_limiteMensal();

                        // Verifica disponivel mensal nas parcelas
                        if (parcTot.select_rows_cartao_mensal(ref lstCartoes, t.ToString())) // este mês
                        {
                            while (parcTot.fetch())
                            {
                                if (ltr.selectIdentity(parcTot.get_fk_log_transacoes())) // busca transação
                                {
                                    if (ltr.get_tg_confirmada() == TipoConfirmacao.Confirmada ||
                                           ltr.get_tg_confirmada() == TipoConfirmacao.Pendente)
                                    {
                                        dispMesParc -= parcTot.get_int_vr_valor();
                                    }
                                }
                            }
                        }

                        if (valor_unit_parc > dispMesParc)
                        {
                            output_st_msg = "limite excedido";
                            var_codResp = "2723";

                            SQL_LOGGING_ENABLE = true;

                            return false;
                        }
                    }
                }
                else
                {
                    if (vr_valor > vr_dispMes || vr_valor > vr_dispTot)
                    {
                        output_st_msg = "limite excedido";
                        var_codResp = "2724";

                        SQL_LOGGING_ENABLE = true;

                        return false;
                    }
                }
            }
            else
            {
                if (vr_valor > cart.get_int_vr_limiteTotal())
                {
                    output_st_msg = "limite excedido";
                    var_codResp = "2725";

                    SQL_LOGGING_ENABLE = true;

                    return false;
                }
            }

            if (myId != cart.get_identity())
            {
                // restaurar cartão dep
                cart.selectIdentity(myId);
            }

            #endregion

            SQL_LOGGING_ENABLE = true;

            /// USER [ authenticate ] END

            Registry("authenticate done exec_pos_vendaEmpresarial ");
            */

            return true;
        }
    }
}