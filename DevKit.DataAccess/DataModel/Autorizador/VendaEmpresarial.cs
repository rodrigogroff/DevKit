using LinqToDB;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace DataModel
{
    public class VendaEmpresarial : BaseVenda
    {
        #region - variaveis - 

        public AutorizadorCNDB db;

        public POS_Entrada input_cont_pe = new POS_Entrada();
        public POS_Resposta output_cont_pr = new POS_Resposta();

        public T_Cartao cartPortador = null;
        public T_Cartao cartTitular = null;
        public T_Empresa emp = null;
        public T_Terminal term = null;
        public LOG_NSU l_nsu = null;
        public T_Proprietario dadosProprietario = null;
        public T_Loja loj = null;

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
                      st_doc = "";

        public DateTime var_dt_transacao;

        public long   vr_dispMes, 
                      vr_dispTot,
                      vr_valor;

        public bool   IsDigitado = false, IsSitef = false;
        
        public ArrayList lstParcs = new ArrayList();

        #endregion

        public void Run(AutorizadorCNDB _db)
        {
            var st = new Stopwatch();

            st.Start();

            db = _db;

            SetupFile();

            var_operacaoCartao = OperacaoCartao.VENDA_EMPRESARIAL;
            var_operacaoCartaoFail = OperacaoCartao.FALHA_VENDA_EMPRESARIAL;

            Registry("-------------------------");
            Registry("START VendaEmpresarial");
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

            if (var_codResp != "0000")
                File.Move(nomeFile, nomeFile.Replace(".txt", "_falha.txt"));
        }

        private bool Authenticate()
        {
            Registry("-------------------------");
            Registry("Authenticate");
            Registry("-------------------------");

            var st = new Stopwatch(); st.Start();

            #region - code - 

            var_nu_nsuAtual = Context.NONE;
            var_nu_nsuEntidade = Context.NONE;
            var_nu_nsuOrig = Context.NONE;
            var_nu_nsuEntOrig = Context.NONE;
            var_vr_total = input_cont_pe.vr_valor;
            var_nu_parcelas = input_cont_pe.nu_parcelas;

            if (IsSitef == false) // venda online
            {
                var q = db.T_Terminal.Where(y => y.nu_terminal == input_cont_pe.st_terminal);

                Registry("(a1) db.T_Terminal.Where(y => y.nu_terminal == " + input_cont_pe.st_terminal);

                term = q.FirstOrDefault();

                if (term == null)
                {
                    output_st_msg = "Terminal inexistente";
                    var_codResp = "0303";
                    return false;
                }
            }
            else // sitef
            {
                var termSt = input_cont_pe.st_codLoja.TrimStart('0');

                // passo 1
                Registry("(Sitef 1.1) db.T_Loja.FirstOrDefault(y => y.st_loja == " + termSt);
                
                var _loj = db.T_Loja.FirstOrDefault(y => y.st_loja == termSt);

                if (_loj == null)
                {
                    // no rows selected, tenta terminal com os zeros

                    var q = db.T_Terminal.Where(y => y.nu_terminal == input_cont_pe.st_terminal);

                    Registry("(a1.x1) db.T_Terminal.Where(y => y.nu_terminal == " + input_cont_pe.st_terminal);

                    term = q.FirstOrDefault();

                    if (term == null)
                    {
                        output_st_msg = "Terminal inexistente";
                        var_codResp = "0303";
                        return false;
                    }
                }
                else
                {
                    // pega primeito terminal associado

                    Registry("(a1.x2) db.T_Terminal.FirstOrDefault(y => y.fk_loja == " + _loj.i_unique);

                    term = db.T_Terminal.FirstOrDefault(y => y.fk_loja == _loj.i_unique);

                    if (term == null)
                    {
                        output_st_msg = "Terminal inexistente";
                        var_codResp = "0303";
                        return false;
                    }
                }
            }

            {
                var q = db.T_Empresa.Where(y => y.st_empresa == input_cont_pe.st_empresa);

                Registry("(a2) db.T_Empresa.Where(y => y.st_empresa == " + input_cont_pe.st_empresa);

                emp = q.FirstOrDefault();

                if (emp == null)
                {
                    output_st_msg = "Empresa inexistente";
                    var_codResp = "0303";                    
                    return false;
                }
            }

            Registry("(a3) emp.tg_bloq " + (emp.tg_bloq == null ? "NULO" : emp.tg_bloq.ToString()));

            if (emp.tg_bloq != null)
                if (emp.tg_bloq.ToString() == Context.TRUE)
                {
                    output_st_msg = "Empresa bloqueada";
                    var_codResp = "0303";
                    return false;
                }

            var loj_emp = new LINK_LojaEmpresa();

            {
                var q = db.LINK_LojaEmpresa.
                    Where(y => y.fk_empresa == emp.i_unique).
                    Where(y => y.fk_loja == term.fk_loja);

                Registry("(a4) db.LINK_LojaEmpresa.Where(y => y.fk_empresa == " + emp.i_unique.ToString() +
                    ").Where(y => y.fk_loja == " + term.fk_loja);

                loj_emp = q.FirstOrDefault();

                if (loj_emp == null)
                {
                    output_st_msg = "Terminal não conveniado";
                    var_codResp = "0303";
                    return false;
                }
            }

            {
                var q = db.T_Loja.Where(y => y.i_unique == term.fk_loja);

                Registry("(a5) db.T_Loja.Where(y => y.i_unique == " + term.fk_loja);

                loj = q.FirstOrDefault();

                if (loj == null)
                {
                    output_st_msg = "Erro aplicativo";
                    return false;
                }

                Registry("(a6) loj.tg_blocked " + (loj.tg_blocked == null ? "NULO" : loj.tg_blocked.ToString()));

                if (loj.tg_blocked != null)
                    if (loj.tg_blocked.ToString() == Context.TRUE)
                    {
                        output_st_msg = "Loja bloqueada";
                        var_codResp = "0303";
                        return false;
                    }

                Registry("(a7) loj.tg_cancel " + (loj.tg_cancel == null ? "NULO" : loj.tg_cancel.ToString()));

                if (loj.tg_cancel != null)
                    if (loj.tg_cancel.ToString() == Context.TRUE)
                    {
                        output_st_msg = "Loja cancelada";
                        var_codResp = "0303";
                        return false;
                    }
            }

            {
                var q = db.T_Cartao.
                        Where(y => y.st_empresa == input_cont_pe.st_empresa).
                        Where(y => y.st_matricula == input_cont_pe.st_matricula).
                        Where(y => y.st_titularidade == input_cont_pe.st_titularidade);

                Registry("(a8) db.T_Cartao.Where(y => y.st_empresa == " + input_cont_pe.st_empresa +
                    ").Where(y => y.st_matricula == " + input_cont_pe.st_matricula +
                    ").Where(y => y.st_titularidade == " + input_cont_pe.st_titularidade);

                cartPortador = q.FirstOrDefault();

                if (cartPortador == null)
                {
                    output_st_msg = "Cartão inexistente";
                    var_codResp = "0606";
                    return false;
                }

                Registry("(a9) cartPortador.tg_status " + (cartPortador.tg_status == null ? "NULO" : cartPortador.tg_status.ToString()));

                if (cartPortador.tg_status != null)
                    if (cartPortador.tg_status.ToString() == CartaoStatus.Bloqueado)
                    {
                        output_st_msg = "Cartão inválido";
                        var_codResp = "0505";
                        return false;
                    }

                Registry("(a10) cartPortador.tg_emitido " + (cartPortador.tg_emitido == null ? "NULO" : cartPortador.tg_emitido.ToString()));

                if (cartPortador.tg_emitido == null)
                    if (cartPortador.tg_emitido.ToString() == StatusExpedicao.Expedido)
                    {
                        output_st_msg = "Cartão inválido";
                        var_codResp = "0505";
                        return false;
                    }
            }

            var_vr_total = input_cont_pe.vr_valor;
            var_nu_parcelas = input_cont_pe.nu_parcelas;

            Registry("(a11) input_cont_pe.vr_valor " + (input_cont_pe.vr_valor == null ? "NULO" : input_cont_pe.vr_valor));

            if (string.IsNullOrEmpty(input_cont_pe.vr_valor))
            {
                output_st_msg = "Valor total nulo";
                var_codResp = "0507";
                return false;
            }

            vr_valor = Convert.ToInt64(input_cont_pe.vr_valor);

            if (vr_valor == 0)
            {
                output_st_msg = "Valor total zerado";
                var_codResp = "0508";
                return false;
            }

            if (cartPortador.st_titularidade != "01")
            {
                cartTitular = db.T_Cartao.FirstOrDefault ( y => y.st_empresa == cartPortador.st_empresa &&
                                                           y.st_matricula == cartPortador.st_matricula &&
                                                           y.st_titularidade == "01");
            }
            else
                cartTitular = cartPortador;                

            new SaldoDisponivel().Obter(db, cartTitular, ref vr_dispMes, ref vr_dispTot);

            Registry("(a12) input_cont_pe.nu_parcelas " + (input_cont_pe.nu_parcelas == null ? "NULO" : input_cont_pe.nu_parcelas));

            if (string.IsNullOrEmpty(input_cont_pe.nu_parcelas))
            {
                output_st_msg = "Parcelas inválidas";
                var_codResp = "0509";
                return false;
            }

            int tmp_nu_parc = Convert.ToInt32(input_cont_pe.nu_parcelas);

            if (tmp_nu_parc > 1)
            {
                if (vr_valor > vr_dispTot)
                {
                    output_st_msg = "limite excedido";
                    var_codResp = "2721";
                    return false;
                }

                string valoresParcelados = input_cont_pe.st_valores;

                Registry("(a13) input_cont_pe.st_valores " + (input_cont_pe.st_valores == null ? "NULO" : input_cont_pe.st_valores));

                if (string.IsNullOrEmpty(input_cont_pe.st_valores))
                {
                    output_st_msg = "Valores parc. vazio";
                    var_codResp = "0510";
                    return false;
                }

                var lstCartoes = new List<string>();

                Registry("(a14) db.T_Cartao.Where(y => y.st_empresa == " + cartPortador.st_empresa + " && y.st_matricula == " + cartPortador.st_matricula);

                foreach (var item in db.T_Cartao.
                                        Where (y=> y.st_empresa == cartPortador.st_empresa && 
                                        y.st_matricula == cartPortador.st_matricula))
                {
                    var _id = item.i_unique.ToString();
                    Registry("(15) T_Cartao => " + _id);
                    lstCartoes.Add(_id);
                }
                    
                for (int t = 1, index_pos = 0; t <= tmp_nu_parc; ++t)
                {
                    long valor_unit_parc = Convert.ToInt64(valoresParcelados.Substring(index_pos, 12));

                    Registry("(a16) valor_unit_parc => " + valor_unit_parc);

                    index_pos += 12;

                    Registry("(a17) valor_unit_parc > cartPortador.vr_limiteMensal " );
                    Registry("(a18) " + valor_unit_parc  + " > " + cartPortador.vr_limiteMensal );

                    if (valor_unit_parc > cartPortador.vr_limiteMensal)
                    {
                        output_st_msg = "limite excedido";
                        var_codResp = "2722";
                        return false;
                    }

                    long dispMesParc = Convert.ToInt64(cartPortador.vr_limiteMensal);

                    Registry("(a19) dispMesParc : " + dispMesParc);

                    var lstParcNoBanco = db.T_Parcelas.
                                            Where(y => lstCartoes.Contains(y.fk_cartao.ToString())).
                                            Where(y => y.nu_parcela == t).ToList();

                    Registry("(a19.1) lstParcNoBanco : " + lstParcNoBanco.Count());

                    var lstIdsTrans = lstParcNoBanco.Select(y => y.fk_log_transacoes.ToString()).ToList();

                    Registry("(a19.2) lstParcNoBanco : " + lstIdsTrans.Count());

                    var lstLTRNoBanco = db.LOG_Transacoes.Where(a => lstIdsTrans.Contains(a.i_unique.ToString())).ToList();

                    Registry("(a19.3) lstLTRNoBanco : " + lstLTRNoBanco.Count());

                    foreach (var itemParcela in db.T_Parcelas.
                                            Where ( y=> lstCartoes.Contains(y.fk_cartao.ToString())).
                                            Where ( y=> y.nu_parcela == t).ToList() )
                    {
                        var ltr = lstLTRNoBanco.FirstOrDefault(y => y.i_unique == itemParcela.fk_log_transacoes);

                        if (ltr == null)
                            continue;

                        if (ltr.tg_confirmada != null)
                        {
                            var tg_conf = ltr.tg_confirmada.ToString();

                            if (tg_conf == TipoConfirmacao.Confirmada || tg_conf == TipoConfirmacao.Pendente)
                                dispMesParc -= (long)itemParcela.vr_valor;

                            if (valor_unit_parc > dispMesParc)
                            {
                                Registry("(a20) if (valor_unit_parc > dispMesParc) ");

                                output_st_msg = "limite excedido";
                                var_codResp = "2723";
                                return false;
                            }
                        }                        
                    }
                }
            }
            else
            {
                Registry("(a21) if (vr_valor > vr_dispMes || vr_valor > vr_dispTot)");
                Registry("(a22) if (" + vr_valor  + " > " + vr_dispMes + " || " + vr_valor+ " > " + vr_dispTot + ")" );
                
                if (vr_valor > vr_dispMes || vr_valor > vr_dispTot)
                {
                    Registry("(a22.1) if (vr_valor > vr_dispMes || vr_valor > vr_dispTot)");

                    output_st_msg = "limite excedido";
                    var_codResp = "2724";
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

            var st = new Stopwatch(); st.Start();

            #region - code -

            dadosProprietario = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cartTitular.fk_dadosProprietario);

            Registry("(x1) db.T_Proprietario.FirstOrDefault(y => y.i_unique == " + cartTitular.fk_dadosProprietario);

            if (dadosProprietario == null)
            {
                output_st_msg = "Erro aplicativo (E1)";
                return false;
            }

            if (cartPortador.st_titularidade != "01")
            {
                var dep = db.T_Dependente.
                            FirstOrDefault(y => y.fk_proprietario == cartPortador.fk_dadosProprietario && 
                                                y.nu_titularidade == Convert.ToInt32(cartPortador.st_titularidade));

                Registry("(x2) db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == " + cartPortador.fk_dadosProprietario + 
                         " && y.nu_titularidade == " + Convert.ToInt32(cartPortador.st_titularidade));

                if (dep == null)
                {
                    output_st_msg = "Erro aplicativo (E2)";
                    return false;
                }

                var_nomeCliente = dep.st_nome;
            }
            else
                var_nomeCliente = dadosProprietario.st_nome;

            Registry("(x3) Nome portador: " + var_nomeCliente);

            if (loj.tg_portalComSenha == 1)
            {
                Registry("(x3.0) loj.tg_portalComSenha == 1 ");
                Registry("(x3.1) input_cont_pe.st_senha: " + (input_cont_pe.st_senha == null ? "NULO" : input_cont_pe.st_senha));

                if (cartTitular.st_senha != input_cont_pe.st_senha)
                {
                    Registry("(x4) Senha Errada!");

                    long senhasErradas = (int)cartPortador.nu_senhaErrada + 1;

                    Registry("(x5) senhasErradas: " + senhasErradas);

                    if (senhasErradas > 4)
                    {
                        cartPortador.tg_status = Convert.ToChar(CartaoStatus.Bloqueado);
                        cartPortador.tg_motivoBloqueio = Convert.ToChar(MotivoBloqueio.SENHA_ERRADA);
                        cartPortador.dt_bloqueio = DateTime.Now;
                    }

                    db.Update(cartPortador);

                    output_st_msg = "Senha inválida";
                    var_codResp = "4343";

                    return false;
                }
                else
                {
                    Registry("(x6) Senha correta... Zerando senhasErradas!");

                    cartPortador.nu_senhaErrada = 0;

                    db.Update(cartPortador);

                    Registry("(x6.1) Cartão atualizado.");
                }
            }

            int tmp_nu_parc = Convert.ToInt32(input_cont_pe.nu_parcelas),
                index_pos = 0;

            string tmp_variavel = input_cont_pe.st_valores;

            if (tmp_variavel.Length < tmp_nu_parc * 12)
            {
                output_st_msg = "formato incorreto";
                return false;
            }

            if (tmp_nu_parc > emp.nu_parcelas)
            {
                output_st_msg = "excede max. parcelas";
                var_codResp = "1212";
                return false;
            }

            Registry("(x7) criando nsu...");

            l_nsu = new LOG_NSU
            {
                dt_log = DateTime.Now
            };

            l_nsu.i_unique = Convert.ToInt32(db.InsertWithIdentity(l_nsu));

            Registry("(x8) criando nsu... " + l_nsu.i_unique);
            
            var_nu_nsuAtual = l_nsu.i_unique.ToString();
            var_nu_nsuEntidade = var_nu_nsuAtual;
            var_dt_transacao = DateTime.Now;

            // ## Criar parcelas

            for (int t = 1; t <= tmp_nu_parc; ++t)
            {
                var valor_unit_parc = Convert.ToInt32(tmp_variavel.Substring(index_pos, 12));

                Registry("(x9) criando parcela de ... " + valor_unit_parc);

                index_pos += 12;

                var parc = new T_Parcela
                {
                    nu_nsu = (int)l_nsu.i_unique,
                    fk_empresa = (int)emp.i_unique,
                    fk_cartao = (int)cartPortador.i_unique,
                    dt_inclusao = var_dt_transacao,
                    nu_parcela = t,
                    vr_valor = valor_unit_parc,
                    nu_indice = t,
                    tg_pago = '0',
                    fk_loja = (int)loj.i_unique,
                    nu_tot_parcelas = tmp_nu_parc,
                    fk_terminal = (int)term.i_unique
                };

                parc.i_unique = Convert.ToInt32(db.InsertWithIdentity(parc));

                lstParcs.Add(parc.i_unique.ToString());
            }

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

            if (var_codResp != "0000")
            {
                Registry("(f1) Nsu não foi criado!");

                l_nsu = new LOG_NSU
                {
                    dt_log = DateTime.Now
                };

                l_nsu.i_unique = Convert.ToInt32(db.InsertWithIdentity(l_nsu));

                var_nu_nsuAtual = l_nsu.i_unique.ToString();
                var_operacaoCartao = var_operacaoCartaoFail;

                Registry("(f1.1) Nsu criado:" + var_nu_nsuAtual);
            }

            Registry("(f2) var_codResp " + var_codResp);

            Registry("(f2.1)");
            output_cont_pr.st_codResp = var_codResp;
            Registry("(f2.2)");
            output_cont_pr.st_nsuRcb = var_nu_nsuAtual.PadLeft(6, '0');
            Registry("(f2.3)");
            output_cont_pr.st_nsuBanco = new StringBuilder().Append(DateTime.Now.Year.ToString())
                                                                       .Append(DateTime.Now.Month.ToString("00"))
                                                                       .Append(DateTime.Now.Day.ToString("00"))
                                                                       .Append(var_nu_nsuAtual.PadLeft(6, '0')).ToString();
            Registry("(f2.4)");
            if (cartPortador != null)
                output_cont_pr.st_PAN = cartPortador.st_empresa + cartPortador.st_matricula;
            Registry("(f2.5)");
            output_cont_pr.st_mesPri = Context.EMPTY;
            Registry("(f2.6)");
            if (loj != null)
                output_cont_pr.st_loja = loj.st_loja;
            Registry("(f2.7)");
            output_cont_pr.st_nomeCliente = var_nomeCliente;
            Registry("(f2.8)");
            if (cartPortador != null)
                output_cont_pr.st_via = cartPortador.nu_viaCartao.ToString();

            Registry("(f3) registra a transacao");

            if (var_codResp != "0000")
            {
                if (input_cont_pe.tipoWeb.ToLower() == "web")
                    output_st_msg = "Web (" + output_st_msg + ")";
                else if (input_cont_pe.tipoWeb.ToLower() == "mobile")
                    output_st_msg = "Mobile (" + output_st_msg + ")";
                else
                    output_st_msg = "SITEF (" + output_st_msg + ")";                
            }
            else
            {
                if (input_cont_pe.tipoWeb.ToLower() == "web")
                    output_st_msg = "Web Payment";
                else if (input_cont_pe.tipoWeb.ToLower() == "mobile")
                    output_st_msg = "Mobile Payment";
                else
                {
                    if (term != null)
                        output_st_msg = "SITEF (" + term.nu_terminal + ") " + term.st_localizacao;
                    else
                        output_st_msg = "SITEF";
                }
            }

            var l_tr = new LOG_Transaco
            {
                fk_terminal = term != null ? (int)term.i_unique : (int?)null,
                fk_empresa = emp != null ? (int)emp.i_unique : (int?)null,
                fk_cartao = cartPortador != null ? (int)cartPortador.i_unique : (int?)null, 
                vr_total = Convert.ToInt32(var_vr_total),
                nu_parcelas = Convert.ToInt32(var_nu_parcelas),
                nu_nsu = Convert.ToInt32(var_nu_nsuAtual),
                dt_transacao = DateTime.Now,
                nu_cod_erro = Convert.ToInt32(output_cont_pr.st_codResp),
                nu_nsuOrig = Convert.ToInt32(var_nu_nsuOrig),
                en_operacao = var_operacaoCartao,
                st_msg_transacao = output_st_msg,
                fk_loja = term != null ? (int)term.fk_loja : (int?)null,                
                st_doc = st_doc
            };

            if (var_codResp != "0000")
            {
                l_tr.tg_confirmada = Convert.ToChar(TipoConfirmacao.Erro);
                l_tr.tg_contabil = Convert.ToChar(Context.FALSE);
            }
            else
            {
                l_tr.tg_confirmada = Convert.ToChar(TipoConfirmacao.Pendente);
                l_tr.tg_contabil = Convert.ToChar(Context.TRUE);
            }

            l_tr.i_unique = Convert.ToInt32(db.InsertWithIdentity(l_tr));

            Registry("(f4) Transacao ID: " + l_tr.i_unique);
            Registry("(f5) Atualizando parcelas");

            for (int t = 0; t < lstParcs.Count; ++t)
            {
                Registry("(f5) Atualizando parcela " + lstParcs[t].ToString() + " com ID log_trans");

                var parc_upd = db.T_Parcelas.FirstOrDefault(y => y.i_unique.ToString() == lstParcs[t].ToString());

                parc_upd.fk_log_transacoes = (int) l_tr.i_unique;

                db.Update(parc_upd);                
            }
            
            #endregion

            st.Stop();

            Registry("-------------------------");
            Registry("Finish DONE, tempo: " + st.ElapsedMilliseconds.ToString());
            Registry("-------------------------");

            return true;
        }
    }
}
