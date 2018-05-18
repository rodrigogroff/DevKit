using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;
using DataModel;
using System.Collections;
using System.Diagnostics;

namespace DevKit.Web.Controllers
{
    public class VendaEmpresarial : BaseVenda
    {
        #region - variaveis - 

        public AutorizadorCNDB db;

        public POS_Entrada input_cont_pe = new POS_Entrada();
        public POS_Resposta output_cont_pr = new POS_Resposta();

        public T_Cartao cartPortador;
        public T_Cartao cartTitular;
        public T_Empresa emp;
        public T_Terminal term;
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
            var st = new Stopwatch();
            st.Start();

            db = _db;

            SetupFile();

            var_operacaoCartao = OperacaoCartao.VENDA_EMPRESARIAL;
            var_operacaoCartaoFail = OperacaoCartao.FALHA_VENDA_EMPRESARIAL;

            try
            {
                if (Authenticate())
                    Execute();

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

            var_codResp = "9999";
            var_nu_nsuAtual = Context.NONE;
            var_nu_nsuEntidade = Context.NONE;
            var_nu_nsuOrig = Context.NONE;
            var_nu_nsuEntOrig = Context.NONE;
            var_vr_total = input_cont_pe.vr_valor;
            var_nu_parcelas = input_cont_pe.nu_parcelas;

            var_codResp = "0606";

            {
                var q = db.T_Terminal.Where(y => y.nu_terminal == input_cont_pe.st_terminal);

                Registry("db.T_Terminal.Where(y => y.nu_terminal == " + input_cont_pe.st_terminal);

                term = q.FirstOrDefault();

                if (term == null)
                {
                    output_st_msg = "Terminal inexistente";
                    var_codResp = "0303";
                    return false;
                }
            }

            {
                var q = db.T_Empresa.Where(y => y.st_empresa == input_cont_pe.st_empresa);

                Registry("db.T_Empresa.Where(y => y.st_empresa == " + input_cont_pe.st_empresa);

                emp = q.FirstOrDefault();

                if (emp == null)
                {
                    output_st_msg = "Empresa inexistente";
                    var_codResp = "0303";                    
                    return false;
                }
            }

            Registry("emp.tg_bloq " + (emp.tg_bloq == null ? "NULO" : emp.tg_bloq.ToString()));

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

                Registry("db.LINK_LojaEmpresa.Where(y => y.fk_empresa == " + emp.i_unique.ToString() +
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

                Registry("db.T_Loja.Where(y => y.i_unique == " + term.fk_loja);

                loj = q.FirstOrDefault();

                if (loj == null)
                {
                    output_st_msg = "Erro aplicativo";
                    return false;
                }

                Registry("loj.tg_blocked " + (loj.tg_blocked == null ? "NULO" : loj.tg_blocked.ToString()));

                if (loj.tg_blocked != null)
                    if (loj.tg_blocked.ToString() == Context.TRUE)
                    {
                        output_st_msg = "Loja bloqueada";
                        var_codResp = "0303";
                        return false;
                    }

                Registry("loj.tg_cancel " + (loj.tg_cancel == null ? "NULO" : loj.tg_cancel.ToString()));

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

                Registry("db.T_Cartao.Where(y => y.st_empresa == " + input_cont_pe.st_empresa +
                    ").Where(y => y.st_matricula == " + input_cont_pe.st_matricula +
                    ").Where(y => y.st_titularidade == " + input_cont_pe.st_titularidade);

                cartPortador = q.FirstOrDefault();

                if (cartPortador == null)
                {
                    output_st_msg = "Cartão inexistente";
                    var_codResp = "0606";
                    return false;
                }

                Registry("cartPortador.tg_status " + (cartPortador.tg_status == null ? "NULO" : cartPortador.tg_status.ToString()));

                if (cartPortador.tg_status != null)
                    if (cartPortador.tg_status.ToString() == CartaoStatus.Bloqueado)
                    {
                        output_st_msg = "Cartão inválido";
                        var_codResp = "0505";
                        return false;
                    }

                Registry("cartPortador.tg_emitido " + (cartPortador.tg_emitido == null ? "NULO" : cartPortador.tg_emitido.ToString()));

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

            Registry("input_cont_pe.vr_valor " + (input_cont_pe.vr_valor == null ? "NULO" : input_cont_pe.vr_valor));

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

            Registry("input_cont_pe.nu_parcelas " + (input_cont_pe.nu_parcelas == null ? "NULO" : input_cont_pe.nu_parcelas));

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

                Registry("input_cont_pe.st_valores " + (input_cont_pe.st_valores == null ? "NULO" : input_cont_pe.st_valores));

                if (string.IsNullOrEmpty(input_cont_pe.st_valores))
                {
                    output_st_msg = "Valores parc. vazio";
                    var_codResp = "0510";
                    return false;
                }

                var lstCartoes = new List<string>();

                Registry("db.T_Cartao.Where(y => y.st_empresa == " + cartPortador.st_empresa + " && y.st_matricula == " + cartPortador.st_matricula);

                foreach (var item in db.T_Cartao.
                                        Where (y=> y.st_empresa == cartPortador.st_empresa && 
                                        y.st_matricula == cartPortador.st_matricula))
                {
                    var _id = item.i_unique.ToString();
                    Registry("T_Cartao => " + _id);
                    lstCartoes.Add(_id);
                }
                    
                for (int t = 1, index_pos = 0; t <= tmp_nu_parc; ++t)
                {
                    long valor_unit_parc = Convert.ToInt64(valoresParcelados.Substring(index_pos, 12));

                    index_pos += 12;

                    if (valor_unit_parc > cartPortador.vr_limiteMensal)
                    {
                        output_st_msg = "limite excedido";
                        var_codResp = "2722";
                        return false;
                    }

                    long dispMesParc = Convert.ToInt64(cartPortador.vr_limiteMensal);

                    T_Parcela parcTot = new T_Parcela();

                    foreach (var itemParcela in db.T_Parcelas.
                                            Where ( y=> lstCartoes.Contains(y.fk_cartao.ToString())).
                                            Where ( y=> y.nu_parcela == t).ToList() )
                    {
                        var ltr = db.LOG_Transacoes.
                                    FirstOrDefault(y => y.i_unique == itemParcela.fk_log_transacoes);

                        var tg_conf = ltr.tg_confirmada.ToString();

                        if (tg_conf == TipoConfirmacao.Confirmada || tg_conf == TipoConfirmacao.Pendente)
                            dispMesParc -= (long) parcTot.vr_valor;

                        if (valor_unit_parc > dispMesParc)
                        {
                            output_st_msg = "limite excedido";
                            var_codResp = "2723";
                            return false;
                        }
                    }
                }
            }
            else
            {
                Registry("if (vr_valor > vr_dispMes || vr_valor > vr_dispTot)");
                Registry("if (" + vr_valor  + " > " + vr_dispMes + " || " + vr_valor+ " > " + vr_dispTot + ")" );
                
                if (vr_valor > vr_dispMes || vr_valor > vr_dispTot)
                {
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

            prot = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cartTitular.fk_dadosProprietario);

            Registry("db.T_Proprietario.FirstOrDefault(y => y.i_unique == " + cartTitular.fk_dadosProprietario);

            if (prot == null)
            {
                output_st_msg = "Erro aplicativo (E1)";
                return false;
            }

            if (cartPortador.st_titularidade != "01")
            {
                var dep = db.T_Dependente.
                            FirstOrDefault(y => y.fk_proprietario == cartPortador.fk_dadosProprietario && 
                                                y.nu_titularidade == Convert.ToInt32(cartPortador.st_titularidade));

                Registry("db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == " + cartPortador.fk_dadosProprietario + 
                         " && y.nu_titularidade == " + Convert.ToInt32(cartPortador.st_titularidade));

                if (dep == null)
                {
                    output_st_msg = "Erro aplicativo (E2)";
                    return false;
                }

                var_nomeCliente = dep.st_nome;
            }
            else
                var_nomeCliente = prot.st_nome;

            Registry("Nome portador: " + var_nomeCliente);

            if (cartPortador.st_senha != input_cont_pe.st_senha)
            {
                Registry("Senha Errada!");

                long senhasErradas = (int)cartPortador.nu_senhaErrada + 1;

                Registry("senhasErradas: " + senhasErradas);

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
                cartPortador.nu_senhaErrada = 0;

                db.Update(cartPortador);
            }

            int tmp_nu_parc = Convert.ToInt32(input_cont_pe.nu_parcelas);
            int index_pos = 0;

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

            l_nsu.dt_log = DateTime.Now;
            l_nsu.i_unique = Convert.ToInt64(db.InsertWithIdentity(l_nsu));
            
            #endregion

            /*

            #region - Faz efetivamente a venda -

            int 	tmp_nu_parc  = Convert.ToInt32 ( input_cont_pe.get_nu_parcelas() );
            int 	index_pos    = 0;

            string  tmp_variavel = input_cont_pe.get_st_valores();

            if ( tmp_variavel.Length < tmp_nu_parc * 12 )
            {
                output_st_msg = "formato incorreto";
                return false;				
            }

            if ( cart.get_tg_tipoCartao() != TipoCartao.presente )
            {
                if ( tmp_nu_parc > emp.get_int_nu_parcelas() )
                {
                    output_st_msg = "excede max. parcelas";
                    var_codResp   = "1212";
                    return false;				
                }
            }

            #region - obtem nsu - 

            l_nsu.set_dt_log ( GetDataBaseTime() );

            if ( !l_nsu.create_LOG_NSU() )
            {
                output_st_msg = "Erro aplicativo";
                return false;
            }

            #endregion

            var_nu_nsuAtual    = l_nsu.get_identity();
            var_nu_nsuEntidade = var_nu_nsuAtual;

            var_dt_transacao = GetDataBaseTime();

            // ## Criar parcelas

            for (int t=1; t <= tmp_nu_parc; ++t )
            {
                T_Parcelas parc = new T_Parcelas (this);

                string valor_unit_parc = tmp_variavel.Substring ( index_pos, 12 );

                index_pos += 12;

                #region - atribui valores e links à parcela - 

                parc.set_nu_nsu				( l_nsu.get_identity()		);
                parc.set_fk_empresa			( emp.get_identity()		);
                parc.set_fk_cartao			( cart.get_identity()		);
                parc.set_dt_inclusao		( var_dt_transacao			);
                parc.set_nu_parcela			( t.ToString()				);
                parc.set_vr_valor			( valor_unit_parc      	 	);
                parc.set_nu_indice			( t.ToString()				); 
                parc.set_tg_pago			( TipoParcela.EM_ABERTO		);
                parc.set_fk_loja			( loj.get_identity()		);
                parc.set_nu_tot_parcelas	( tmp_nu_parc.ToString() 	);
                parc.set_fk_terminal		( term.get_identity()		);

                #endregion

                if ( !parc.create_T_Parcelas() )
                {
                    output_st_msg = "erro aplicativo";
                    return false;				
                }

                lstParcs.Add ( parc.get_identity() );
            }

            #endregion*/

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


            st.Stop();

            Registry("-------------------------");
            Registry("Finish DONE, tempo: " + st.ElapsedMilliseconds.ToString());
            Registry("-------------------------");

            return true;
        }
    }
}