using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace GetStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("------------------------");
            Console.WriteLine("Patch!");
            Console.WriteLine("------------------------");

            // nunca tirar FDP
            Console.ReadLine();

            /*
            using (var db = new AutorizadorCNDB())
            {
                var lst = db.T_Cartao.Where(y => y.st_empresa == "001401" || y.st_empresa == "001402" || y.st_empresa == "001403" || y.st_empresa == "001404").ToList();

                foreach (var item in lst)
                {
                    item.stCodigoFOPA = item.st_matricula;
                    db.Update(item);
                }
                
            }

                
                using (var db = new AutorizadorCNDB())
                {
                    using (var txt = new StreamReader("c:\\bkp\\1401-depara-cartao-fopag.txt"))
                    {
                        txt.ReadLine();

                        while (!txt.EndOfStream)
                        {
                            var line = txt.ReadLine().Split('\t');

                            var t_cart = db.T_Cartao.FirstOrDefault(y => y.st_empresa == line[3].PadLeft(6, '0') && y.st_matricula == line[1].PadLeft(6, '0'));

                            t_cart.stCodigoFOPA = line[0];

                            db.Update(t_cart);
                        }
                    }

                    using (var txt = new StreamReader("c:\\bkp\\1402-depara-cartao-fopag.txt"))
                    {
                        txt.ReadLine();

                        while (!txt.EndOfStream)
                        {
                            var line = txt.ReadLine().Split('\t');

                            var t_cart = db.T_Cartao.FirstOrDefault(y => y.st_empresa == line[3].PadLeft(6, '0') && y.st_matricula == line[1].PadLeft(6, '0'));

                            t_cart.stCodigoFOPA = line[0];

                            db.Update(t_cart);
                        }
                    }

                    using (var txt = new StreamReader("c:\\bkp\\1403-depara-cartao-fopag.txt"))
                    {
                        txt.ReadLine();

                        while (!txt.EndOfStream)
                        {
                            var line = txt.ReadLine().Split('\t');

                            var t_cart = db.T_Cartao.FirstOrDefault(y => y.st_empresa == line[3].PadLeft(6, '0') && y.st_matricula == line[1].PadLeft(6, '0'));

                            t_cart.stCodigoFOPA = line[0];

                            db.Update(t_cart);
                        }
                    }

                    using (var txt = new StreamReader("c:\\bkp\\1404-depara-cartao-fopag.txt"))
                    {
                        txt.ReadLine();

                        while (!txt.EndOfStream)
                        {
                            var line = txt.ReadLine().Split('\t');

                            var t_cart = db.T_Cartao.FirstOrDefault(y => y.st_empresa == line[3].PadLeft(6, '0') && y.st_matricula == line[1].PadLeft(6, '0'));

                            t_cart.stCodigoFOPA = line[0];

                            db.Update(t_cart);
                        }
                    }
                }*/

            Console.WriteLine("DONE!");

            //ReFecha("09", "2020", 38, 2020, 9, 15, false);
            //ForcaFech_9086("009086", new DateTime(2020, 9, 15, 0, 0, 0));

            //SetaValoresEmpresa("009622", 17200, 17250);

            //SetaValoresEmpresa("009622", 16000, 17200);
            //SetaValoresEmpresa("009622", 56000, 57250);


            //       MigraParcelas(new T_Cartao { st_empresa = "001201", st_matricula = "878870" }, new T_Cartao { st_empresa = "001201", st_matricula = "151806" });

            //SetaSenhaEmpresaPorMatricula("009971");

            //SetaValoresEmpresa("009971", 20000, 30000);

            //ImportaLimites();

            /* MigraParcelas ( new T_Cartao { st_empresa = "001201", st_matricula = "859575" }, new T_Cartao { st_empresa = "001201", st_matricula = "390531" }); */
            /* MigraParcelas ( new T_Cartao { st_empresa = "001201", st_matricula = "979040" }, new T_Cartao { st_empresa = "001201", st_matricula = "239089" }); */

            //CompilaDash();

            //9620,9621,9622,9623,9624,5041

            //ReFecha("03", "2020", 18, 2020, 3, 15);

            //ForcaFech("009620", new DateTime(2020, 3, 1, 0, 12, 0));
            //ForcaFech("009621", new DateTime(2020, 3, 1, 0, 12, 0));
            //ForcaFech("009622", new DateTime(2020, 3, 1, 0, 12, 0));
            //ForcaFech("009623", new DateTime(2020, 3, 1, 0, 12, 0));
            //ForcaFech("009624", new DateTime(2020, 3, 1, 0, 12, 0));
            // ForcaFech("001711", new DateTime(2020, 3, 13, 0, 09, 0));
        }

        static string DESCript(string dados, string chave = "12345678")
        {
            dados = dados.PadLeft(8, '*');

            byte[] key = System.Text.Encoding.ASCII.GetBytes(chave);//{1,2,3,4,5,6,7,8};
            byte[] data = System.Text.Encoding.ASCII.GetBytes(dados);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            des.Key = key;
            des.Mode = CipherMode.ECB;

            ICryptoTransform DESt = des.CreateEncryptor();
            DESt.TransformBlock(data, 0, 8, data, 0);

            string retorno = "";
            for (int n = 0; n < 8; n++)
            {
                retorno += String.Format("{0:X2}", data[n]);
            }

            return retorno;
        }

        static void SetaValoresEmpresa(string stEmpresa, int vlrantigo, int vlrnovo)
        {
            using (var db = new AutorizadorCNDB())
            {
                var t_empresa = db.T_Empresa.FirstOrDefault(y => y.st_empresa == stEmpresa);

                var lstCarts = db.T_Cartao.Where(y => y.st_empresa == stEmpresa).ToList();

                foreach (var item in lstCarts)
                {
                    if (item.vr_limiteMensal == vlrantigo)
                    {
                        item.vr_limiteMensal = vlrnovo;
                        item.vr_limiteTotal = vlrnovo;

                        db.Update(item);
                    }
                }
            }
        }

        static void SetaSenhaEmpresaPorMatricula(string stEmpresa)
        {
            using (var db = new AutorizadorCNDB())
            {
                var t_empresa = db.T_Empresa.FirstOrDefault(y => y.st_empresa == stEmpresa);

                var lstCarts = db.T_Cartao.Where(y => y.st_empresa == stEmpresa).ToList();

                foreach (var item in lstCarts)
                {
                    item.st_senha = DESCript(item.st_matricula.Substring (2,4));

                    db.Update(item);
                }
            }
        }

        static void ImportaLimites()
        {
            using (var sr = new StreamReader("C:\\bkp\\limites.csv"))
            {
                using (var db = new AutorizadorCNDB())
                {
                    var t_empresa = db.T_Empresa.FirstOrDefault(y => y.st_empresa == "009971");

                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        var dados = line.Split(';');

                        // mat,nome,limite

                        dados[0] = dados[0].PadLeft(6, '0');
                        dados[1] = dados[1].Trim().PadRight(30, ' ').Substring(0, 30).Trim();
                        dados[2] = dados[2].Replace(".", "").Replace(",", "").Replace("R$ ", "");

                        var cart = db.T_Cartao.FirstOrDefault(y => y.st_empresa == t_empresa.st_empresa && y.st_matricula == dados[0]);

                        if (cart == null)
                        {
                            Console.WriteLine(dados[0] + " " + dados[1] + " - matricula não encontrada");
                        }
                        else
                        { 
                            cart.vr_limiteMensal = Convert.ToInt32(dados[2]);
                            cart.vr_limiteTotal = Convert.ToInt32(dados[2]);

                            db.Update(cart);
                        }
                    }
                }
            }
        }

        static void ImportaCadastro()
        {
            using (var sr = new StreamReader("C:\\bkp\\cadastro.csv"))
            {
                using (var db = new AutorizadorCNDB())
                {
                    var t_empresa = db.T_Empresa.FirstOrDefault(y => y.st_empresa == "009971");

                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        var prop = new T_Proprietario();
                        var cart = new T_Cartao();

                        var dados = line.Split(';');

                        // mat,nome,cpf

                        dados[0] = dados[0].PadLeft(6, '0');
                        dados[1] = dados[1].Trim().PadRight(30, ' ').Substring(0, 30).Trim();
                        dados[2] = dados[2].Replace(".", "").Replace("-", "");

                        prop.dt_nasc = new DateTime(1970, 1, 1);
                        prop.st_bairro = "";
                        prop.st_cep = "";
                        prop.st_cidade = "SANTO ANGELO";
                        prop.st_complemento = "";
                        prop.st_cpf = dados[2];
                        prop.st_ddd = "";
                        prop.st_email = "";
                        prop.st_endereco = "";
                        prop.st_nome = dados[1];
                        prop.st_numero = "";
                        prop.st_senhaEdu = "";
                        prop.st_telefone = "";
                        prop.st_UF = "RS";
                        prop.vr_renda = 1000;

                        var id_prop = Convert.ToInt32(db.InsertWithIdentity(prop));

                        cart.fk_dadosProprietario = id_prop;

                        cart.st_empresa = t_empresa.st_empresa;
                        cart.st_matricula = dados[0];
                        cart.dt_inclusao = DateTime.Now;
                                                
                        cart.nu_webSenhaErrada = 0;
                        cart.st_agencia = "";
                        cart.st_aluno = "";
                        cart.st_banco = "";
                        cart.st_celCartao = "";
                        cart.st_conta = "";
                        cart.st_titularidade = "01";
                        cart.st_venctoCartao = "1227";

                        cart.tg_convenioComSaldo = false;

                        cart.vr_extraCota = 0;
                        cart.vr_limiteMensal = 100;
                        cart.vr_limiteRotativo = 100;
                        cart.vr_limiteTotal = 100;

                        cart.nu_viaCartao = 1;

                        cart.tg_status = Convert.ToChar(CartaoStatus.Habilitado);
                        cart.tg_emitido = Convert.ToInt32(StatusExpedicao.NaoExpedido);
                        cart.tg_tipoCartao = Convert.ToChar(TipoCartao.empresarial);
                        cart.nu_senhaErrada = Convert.ToInt32(Context.NONE);

                        cart.i_unique = Convert.ToDecimal(db.InsertWithIdentity(cart));

                        // ----------------------------------
                        // log de auditoria
                        // ----------------------------------

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = 1,
                            st_empresa = t_empresa.st_empresa,
                            st_oper = "Novo Cartão (carga)",
                            st_log = "Mat: " + cart.st_matricula + " Nome:" + prop.st_nome
                        });
                    }
                }
            }
        }

        static void CompilaDash()
        {
            using (var db = new AutorizadorCNDB())
            {
                var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                dt = dt.AddDays(-1);
                
                var dt_final = dt.AddHours(23).AddMinutes(59);

                while (true)
                {
                    var lst = db.LOG_Transacoes.
                                    Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                    Where(y => y.dt_transacao > dt && y.dt_transacao < dt_final).
                                    OrderByDescending(y => y.dt_transacao).
                                    ToList();

                    while ( dt_final > dt)
                    {
                        var dt_temp = new DateTime(dt_final.Year, dt_final.Month, dt_final.Day);
                        var _tmp_list = lst.Where(y => y.dt_transacao > dt_temp && y.dt_transacao < dt_final);

                        Console.WriteLine(dt_temp + " até " + dt_final);

                        db.Insert( new DashboardGrafico
                        { 
                            nuAno = dt_temp.Year,
                            nuMes = dt_temp.Month,
                            nuDia = dt_temp.Day,
                            totalTransacoes = _tmp_list.Count(),
                            totalCartoes = _tmp_list.Select ( y=> y.fk_cartao).Distinct().Count(),
                            totalFinanc = _tmp_list.Sum ( y=> (int) y.vr_total),
                            totalLojas = _tmp_list.Select(y => y.fk_loja).Distinct().Count(),
                            dtDia = dt_temp
                        });

                        if (dt_final.Day == 1)
                        {
                            dt_final = dt_final.AddDays(-1);
                            break;
                        }
                        else
                            dt_final = dt_final.AddDays(-1);
                    }

                    dt = dt.AddMonths(-1);                    
                }
            }
        }

        static void MigraParcelas( T_Cartao cartOriginal, T_Cartao cartDestino)
        {
            using (var db = new AutorizadorCNDB())
            {
                var t_cartOriginal = (  from e in db.T_Cartao
                                        where e.st_empresa == cartOriginal.st_empresa
                                        where e.st_matricula == cartOriginal.st_matricula
                                        select e).
                                        FirstOrDefault();

                var t_cartDestino = (from e in db.T_Cartao
                                      where e.st_empresa == cartDestino.st_empresa
                                      where e.st_matricula == cartDestino.st_matricula
                                      select e).
                                        FirstOrDefault();

                var lstParcelasOriginais = (from e in db.T_Parcelas
                                   where e.fk_cartao == t_cartOriginal.i_unique
                                   where e.nu_parcela == 1
                                   select e).
                                        ToList();

                foreach (var parcelaOriginal in lstParcelasOriginais)
                {
                    if (parcelaOriginal.nu_indice > 1)
                    {
                        // ---------------------------------------
                        // antiga, com várias parcelas
                        // ---------------------------------------

                        // busco transacao original
                        var tbLogTransOriginal = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parcelaOriginal.fk_log_transacoes);

                        var totalParcsOriginal = tbLogTransOriginal.nu_parcelas;

                        //                        2   =    3             - 1        
                        var atualizaLogTransOrigParcs = parcelaOriginal.nu_indice - 1;

                        tbLogTransOriginal.nu_parcelas = atualizaLogTransOrigParcs;

                        // somar valores
                        {
                            tbLogTransOriginal.vr_total = 0;

                            // atualizar parcelas desta transacao original
                            var lst_parcs_antigas = db.T_Parcelas.
                                                        Where(y => y.fk_log_transacoes == parcelaOriginal.fk_log_transacoes).
                                                        Where(y => y.nu_indice <= atualizaLogTransOrigParcs).
                                                        OrderBy(y => y.nu_indice).
                                                        ToList();

                            foreach (var itemParc in lst_parcs_antigas)
                                tbLogTransOriginal.vr_total += itemParc.vr_valor;
                        }

                        // transação original com parcelas reduzidas e valor reduzido atualizada
                        db.Update(tbLogTransOriginal);

                        //gera nsu
                        var novoNSU = new LOG_NSU();
                        novoNSU.i_unique = Convert.ToInt32(db.InsertWithIdentity(novoNSU));

                        // criar nova transação para as restantes
                        var novaTransacao = new LOG_Transaco
                        {
                            vr_total = 0, // será calculada pelas parcelas restantes
                            dt_transacao = DateTime.Now, // data de hoje
                            en_operacao = tbLogTransOriginal.en_operacao,
                            fk_cartao = (int)t_cartDestino.i_unique,
                            fk_empresa = tbLogTransOriginal.fk_empresa,
                            fk_loja = tbLogTransOriginal.fk_loja,
                            fk_terminal = tbLogTransOriginal.fk_terminal,
                            nu_cod_erro = tbLogTransOriginal.nu_cod_erro,
                            nu_nsu = (int)novoNSU.i_unique,
                            nu_nsuOrig = (int)novoNSU.i_unique,

                            //       8  =  10                - 2           
                            nu_parcelas = totalParcsOriginal - atualizaLogTransOrigParcs,

                            st_doc = "",
                            st_msg_transacao = tbLogTransOriginal.st_msg_transacao,
                            tg_confirmada = tbLogTransOriginal.tg_confirmada,
                            tg_contabil = tbLogTransOriginal.tg_contabil,
                            vr_saldo_disp = tbLogTransOriginal.vr_saldo_disp,
                            vr_saldo_disp_tot = tbLogTransOriginal.vr_saldo_disp_tot,
                        };

                        // acumular valores das outras parcelas
                        {                            
                            var lst_parcs_antigas = db.T_Parcelas.
                                                        Where(y => y.fk_log_transacoes == parcelaOriginal.fk_log_transacoes).
                                                        Where(y => y.nu_indice > atualizaLogTransOrigParcs).
                                                        OrderBy(y => y.nu_indice).
                                                        ToList();

                            foreach (var itemParc in lst_parcs_antigas)
                                novaTransacao.vr_total += itemParc.vr_valor;
                        }

                        novaTransacao.i_unique = Convert.ToInt32(db.InsertWithIdentity(novaTransacao));

                        {
                            // atualizar parcelas da nova transacao
                            var lst_parcs_antigas = db.T_Parcelas.
                                                        Where(y => y.fk_log_transacoes == parcelaOriginal.fk_log_transacoes).
                                                        Where(y => y.nu_indice > atualizaLogTransOrigParcs).
                                                        OrderBy ( y=> y.nu_indice).
                                                        ToList();

                            int index = 1;

                            foreach (var itemParc in lst_parcs_antigas)
                            {
                                itemParc.fk_cartao = (int)t_cartDestino.i_unique;
                                itemParc.fk_log_transacoes = (int) novaTransacao.i_unique;
                                itemParc.dt_inclusao = novaTransacao.dt_transacao;
                                itemParc.nu_parcela = index;
                                itemParc.nu_indice = index;
                                itemParc.nu_nsu = novaTransacao.nu_nsu;                                
                                itemParc.nu_tot_parcelas = novaTransacao.nu_parcelas;

                                db.Update(itemParc);

                                index++;
                            }                                
                        }
                    }
                    else
                    {
                        // ----------------------------------------
                        // deste mes, apenas trocar as chaves
                        // ----------------------------------------

                        // busco transacao original
                        var tbLogTransOriginal = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parcelaOriginal.fk_log_transacoes);

                        tbLogTransOriginal.fk_cartao = (int)t_cartDestino.i_unique;

                        // transação original com chave nova do cartão novo
                        db.Update(tbLogTransOriginal);

                        // atualizar parcelas desta transacao original
                        var lst_parcs_update = db.T_Parcelas.Where(y => y.fk_log_transacoes == parcelaOriginal.fk_log_transacoes).ToList();

                        foreach (var pUp in lst_parcs_update)
                        {
                            pUp.fk_cartao = (int)t_cartDestino.i_unique;
                            db.Update(pUp);
                        }
                    }
                }
            }
        }

        static void LimpaScheduler()
        {
            using (var db = new AutorizadorCNDB())
            {
                var lstSchedul = (from e in db.I_Scheduler
                                   where e.st_job.StartsWith("schedule_fech_mensal;empresa;")
                                   select e).
                                   ToList();

                foreach (var item in lstSchedul)
                {
                    db.Delete(item);
                }
            }
        }

        static void CopiaDadosDoScheduler()
        {
            using (var db = new AutorizadorCNDB())
            {
                foreach (var item in db.T_Empresa.ToList())
                {
                    var t_scheduler = (from e in db.I_Scheduler
                                       where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + item.st_empresa)
                                       select e).
                                       FirstOrDefault();

                    if (t_scheduler == null)
                        continue;

                    var dbEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == item.i_unique);

                    dbEmp.st_horaFech = t_scheduler.st_monthly_hhmm;
                    dbEmp.nu_diaFech = t_scheduler.nu_monthly_day;

                    db.Update(dbEmp);
                }
            }
        }

        static void AjustaParcela(int idParcela)
        {
            #region - code - 
            Console.WriteLine(idParcela);

            using (var db = new AutorizadorCNDB())
            {
                var fech = db.LOG_Fechamento.FirstOrDefault(y => y.fk_parcela == idParcela);

                var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == idParcela);
                parcUpd.nu_parcela++;
                db.Update(parcUpd);

                db.Delete(fech);
            }
            #endregion
        }

        static void AjustaParcelasErradas(string arquivo, int fkEmpresa, string mes, string ano)
        {
            #region - code - 

            using (var db = new AutorizadorCNDB())
            {
                var dbEmpresa = db.T_Empresa.FirstOrDefault(y => y.i_unique == fkEmpresa);

                var lstFech = db.LOG_Fechamento.Where(y => y.fk_empresa == fkEmpresa && y.st_mes == mes && y.st_ano == ano).ToList();

                using (var sr = new StreamReader(arquivo))
                {
                    var loja = new T_Loja();

                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        if (line.StartsWith("="))
                        {
                            loja = db.T_Loja.FirstOrDefault(y => y.st_loja == line.Replace("=", ""));

                            if (loja == null)
                                throw new Exception("ERRO: " + line + " não existe loja!");
                            else
                            {
                                Console.WriteLine(">> " + loja.st_nome);
                            }
                        }
                        else if (line.StartsWith("*"))
                        {
                            Console.WriteLine(line);

                            var dados = line.Split(';');

                            var nsu = Convert.ToInt32(dados[1]);

                            var cartao_mat = dados[3].Split('.')[0].PadLeft(6, '0');
                            var cartao_tit = dados[3].Split('.')[1];

                            var valor = Convert.ToInt64(dados[5].Replace(",", "").Replace(".", ""));
                            var parcela = dados[6].Split('/')[0].Trim();

                            var dbCart = db.T_Cartao.FirstOrDefault(y => y.st_empresa == dbEmpresa.st_empresa && y.st_matricula == cartao_mat && y.st_titularidade == cartao_tit);

                            if (dbCart != null)
                            {
                                var log_fech = lstFech.Where(y => y.fk_loja == loja.i_unique &&
                                                                      y.nu_parcela.ToString() == parcela &&
                                                                      y.fk_cartao == dbCart.i_unique).ToList();

                                bool found = false;

                                foreach (var itemF in log_fech)
                                {
                                    var t_parc = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemF.fk_parcela);

                                    if (t_parc != null)
                                    {
                                        if (t_parc.nu_nsu == nsu)
                                        {
                                            // ajusta a parcela
                                            var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemF.fk_parcela);
                                            parcUpd.nu_parcela++;
                                            db.Update(parcUpd);

                                            // deleta o fechamento
                                            db.Delete(log_fech);
                                            Console.WriteLine("Ajustado!");

                                            found = true;
                                        }
                                    }
                                }

                                if (!found)
                                    Console.WriteLine("Não achou fechamento!");
                            }
                            else
                            {
                                Console.WriteLine("Não achou cartão!");
                            }
                        }
                    }
                }
            }

            #endregion
        }

        static void ForcaFech(string mat, DateTime dt)
        {
            #region -  code - 

            using (var db = new AutorizadorCNDB())
            {
                string currentEmpresa = "";

                try
                {
                    var diaFechamento = dt.Day;
                    var horaAtual = dt.ToString("HHmm");
                    var ano = dt.ToString("yyyy");
                    var mes = dt.ToString("MM").PadLeft(2, '0');

                    var lstEmpresas = db.T_Empresa.Where(y => y.st_empresa == mat).ToList();

                    foreach (var empresa in lstEmpresas)
                    {
                        // ------------------------------
                        // só fecha uma vez no mes
                        // ------------------------------

                        if (db.LOG_Fechamento.Any(y => y.st_ano == ano &&
                                                        y.st_mes == mes &&
                                                        y.fk_empresa == empresa.i_unique))
                            continue;

                        currentEmpresa = empresa.st_empresa;

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = null,
                            st_oper = "Fechamento [INICIO]",
                            st_empresa = currentEmpresa,
                            st_log = "Ano " + ano + " Mes " + mes
                        });

                        var g_job = new T_JobFechamento
                        {
                            dt_inicio = DateTime.Now,
                            dt_fim = null,
                            fk_empresa = (int)empresa.i_unique,
                            st_ano = ano,
                            st_mes = mes
                        };

                        // ----------------------------
                        // registra job
                        // ----------------------------

                        g_job.i_unique = Convert.ToInt32(db.InsertWithIdentity(g_job));

                        // ----------------------------
                        // busca parcelas
                        // ----------------------------

                        long totValor = 0, ind_parc = 1, tot_parcs_sel;

                        var lst = db.T_Parcelas.Where(y => y.fk_empresa == empresa.i_unique && y.nu_parcela > 0).ToList();

                        tot_parcs_sel = lst.Count();

                        foreach (var parc in lst)
                        {
                            // ----------------------------
                            // somente confirmadas
                            // ----------------------------

                            var logTrans = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                            if (logTrans == null)
                                continue;
                            else
                            {
                                if (logTrans.tg_confirmada == null)
                                    continue;

                                if (logTrans.tg_confirmada.ToString() != TipoConfirmacao.Confirmada)
                                    continue;

                                if (logTrans.dt_transacao > dt)
                                    continue;
                            }

                            Console.WriteLine(empresa.st_empresa + " > " + empresa.i_unique + " --> " + ind_parc++ + " / " + tot_parcs_sel);

                            // ----------------------------
                            // decrementa parcela
                            // ----------------------------

                            var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == parc.i_unique);
                            parcUpd.nu_parcela--;
                            db.Update(parcUpd);

                            // -------------------------------------------
                            // insere fechamento quando parcela zerar 
                            // -------------------------------------------

                            if (parcUpd.nu_parcela == 0)
                            {
                                totValor += (int)parc.vr_valor;

                                db.Insert(new LOG_Fechamento
                                {
                                    dt_compra = logTrans.dt_transacao,
                                    dt_fechamento = DateTime.Now,
                                    fk_cartao = parc.fk_cartao,
                                    fk_empresa = parc.fk_empresa,
                                    fk_loja = parc.fk_loja,
                                    fk_parcela = (int)parc.i_unique,
                                    nu_parcela = parc.nu_parcela,
                                    st_afiliada = "",
                                    st_ano = ano,
                                    st_mes = mes,
                                    vr_valor = parc.vr_valor
                                });
                            }
                        }

                        // ----------------------------
                        // registra job / finalizado!
                        // ----------------------------

                        g_job.dt_fim = DateTime.Now;

                        db.Update(g_job);

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = null,
                            st_oper = "Fechamento [OK]",
                            st_empresa = currentEmpresa,
                            st_log = "Ano " + ano + " Mes " + mes + " Valor => " + totValor
                        });
                    }
                }
                catch (SystemException ex)
                {
                    db.Insert(new LOG_Audit
                    {
                        dt_operacao = DateTime.Now,
                        fk_usuario = null,
                        st_oper = "Fechamento [ERRO]",
                        st_empresa = currentEmpresa,
                        st_log = ex.ToString()
                    });
                }
            }

            #endregion
        }

        static void ForcaFech_9086(string mat, DateTime dt)
        {
            #region -  code - 

            using (var db = new AutorizadorCNDB())
            {
                string currentEmpresa = "";

                try
                {
                    var diaFechamento = dt.Day;
                    var horaAtual = dt.ToString("HHmm");
                    var ano = dt.ToString("yyyy");
                    var mes = dt.ToString("MM").PadLeft(2, '0');

                    var lstEmpresas = db.T_Empresa.Where(y => y.st_empresa == mat).ToList();

                    foreach (var empresa in lstEmpresas)
                    {
                        // ------------------------------
                        // só fecha uma vez no mes
                        // ------------------------------

                        if (db.LOG_Fechamento.Any(y => y.st_ano == ano &&
                                                        y.st_mes == mes &&
                                                        y.fk_empresa == empresa.i_unique))
                            continue;

                        currentEmpresa = empresa.st_empresa;

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = null,
                            st_oper = "Fechamento [INICIO]",
                            st_empresa = currentEmpresa,
                            st_log = "Ano " + ano + " Mes " + mes
                        });

                        var g_job = new T_JobFechamento
                        {
                            dt_inicio = DateTime.Now,
                            dt_fim = null,
                            fk_empresa = (int)empresa.i_unique,
                            st_ano = ano,
                            st_mes = mes
                        };

                        // ----------------------------
                        // registra job
                        // ----------------------------

                        g_job.i_unique = Convert.ToInt32(db.InsertWithIdentity(g_job));

                        var lst = new List<T_Parcela>();

                        // ---------------------------------------------------------------------
                        // ajustar parcelas futuras de agosto que ferraram o nu_parcela

                        // no dia 10 de agosto, fiz 5 parcelas de 10 pila
                        // a 1 fica 0, 2 fica 1, etc

                        {
                            var lst_fech_anterior = db.LOG_Fechamento.Where(y => y.fk_empresa == empresa.i_unique && 
                                                                                 y.st_mes == "08" && 
                                                                                 y.st_ano == "2020").
                                                                                 ToList();

                            int index = 1 , tot = lst_fech_anterior.Count();

                            foreach (var fech in lst_fech_anterior)
                            {
                                Console.WriteLine(empresa.st_empresa + " > " + empresa.i_unique + " --> A " + index++ + " / " + tot);

                                using (var db2 = new AutorizadorCNDB())
                                {
                                    var t_parcela_anterior = db2.T_Parcelas.FirstOrDefault(y => y.i_unique == fech.fk_parcela);

                                    var lst_parcs_to_fix = db2.T_Parcelas.Where(y => y.fk_log_transacoes == t_parcela_anterior.fk_log_transacoes &&
                                                                                    y.nu_indice > t_parcela_anterior.nu_indice).
                                                                                    OrderBy(y => y.nu_indice).
                                                                                    ToList();

                                    if (lst_parcs_to_fix.Any())
                                    {
                                        int nu_parcela = (int)t_parcela_anterior.nu_parcela + 1;

                                        bool insert = false;

                                        foreach (var item_fix in lst_parcs_to_fix)
                                        {
                                            item_fix.nu_parcela = nu_parcela;

                                            db2.Update(item_fix);

                                            if (!insert)
                                            {
                                                insert = true;
                                                lst.Add(item_fix);
                                            }

                                            nu_parcela++;
                                        }
                                    }
                                }
                            }
                        }
                        
                        // ----------------------------
                        // busca transações do periodo
                        // ----------------------------

                        var dt_ini = dt.AddMonths(-1);

                        var lst_antigas_transacoes = db.LOG_Transacoes.Where(y => y.fk_empresa == empresa.i_unique && 
                                                                                   y.dt_transacao > dt_ini && y.dt_transacao < dt && 
                                                                                   y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                                                   ToList();

                        // ----------------------------------
                        // ajusta nu_parcela das parcelas
                        // ----------------------------------

                        {
                            int index = 1, tot = lst_antigas_transacoes.Count();

                            foreach (var item in lst_antigas_transacoes)
                            {
                                Console.WriteLine(empresa.st_empresa + " > " + empresa.i_unique + " --> B " + index++ + " / " + tot);

                                using (var db2 = new AutorizadorCNDB())
                                {
                                    var lst_parcs_to_fix = db2.T_Parcelas.Where(y => y.fk_log_transacoes == item.i_unique).
                                                                                OrderBy(y => y.nu_indice).
                                                                                ToList();

                                    if (lst_parcs_to_fix.Any())
                                    {
                                        int nu_parcela = 1;

                                        foreach (var item_fix in lst_parcs_to_fix)
                                        {
                                            if (nu_parcela == 1)
                                                lst.Add(item_fix);

                                            item_fix.nu_parcela = nu_parcela;

                                            db2.Update(item_fix);

                                            nu_parcela++;
                                        }
                                    }
                                }
                            }
                        }

                        // ----------------------------
                        // busca parcelas
                        // ----------------------------

                        int tot_parcs_sel = lst.Count(), ind_parc = 1;
                        long totValor = 0;

                        foreach (var parc in lst)
                        {
                            using (var db2 = new AutorizadorCNDB())
                            {
                                // ----------------------------
                                // somente confirmadas
                                // ----------------------------

                                var logTrans = db2.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                                if (logTrans == null)
                                    continue;
                                else
                                {
                                    if (logTrans.tg_confirmada == null)
                                        continue;

                                    if (logTrans.tg_confirmada.ToString() != TipoConfirmacao.Confirmada)
                                        continue;

                                    if (logTrans.dt_transacao > dt)
                                        continue;
                                }

                                Console.WriteLine(empresa.st_empresa + " > " + empresa.i_unique + " --> C " + ind_parc++ + " / " + tot_parcs_sel);

                                // ----------------------------
                                // decrementa parcela
                                // ----------------------------

                                var parcUpd = db2.T_Parcelas.FirstOrDefault(y => y.i_unique == parc.i_unique);
                                parcUpd.nu_parcela--;
                                db2.Update(parcUpd);

                                // -------------------------------------------
                                // insere fechamento quando parcela zerar 
                                // -------------------------------------------

                                if (parcUpd.nu_parcela == 0)
                                {
                                    totValor += (int)parc.vr_valor;

                                    db2.Insert(new LOG_Fechamento
                                    {
                                        dt_compra = logTrans.dt_transacao,
                                        dt_fechamento = DateTime.Now,
                                        fk_cartao = parc.fk_cartao,
                                        fk_empresa = parc.fk_empresa,
                                        fk_loja = parc.fk_loja,
                                        fk_parcela = (int)parc.i_unique,
                                        nu_parcela = parc.nu_parcela,
                                        st_afiliada = "",
                                        st_ano = ano,
                                        st_mes = mes,
                                        vr_valor = parc.vr_valor
                                    });
                                }
                            }
                        }

                        // ----------------------------
                        // registra job / finalizado!
                        // ----------------------------

                        g_job.dt_fim = DateTime.Now;

                        db.Update(g_job);

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = null,
                            st_oper = "Fechamento [OK]",
                            st_empresa = currentEmpresa,
                            st_log = "Ano " + ano + " Mes " + mes + " Valor => " + totValor
                        });
                    }
                }
                catch (SystemException ex)
                {
                    db.Insert(new LOG_Audit
                    {
                        dt_operacao = DateTime.Now,
                        fk_usuario = null,
                        st_oper = "Fechamento [ERRO]",
                        st_empresa = currentEmpresa,
                        st_log = ex.ToString()
                    });
                }
            }

            #endregion
        }

        static void FixDeVelho(string mesAnt, string anoAnt, int fkEmpresa, string mesAtual, string anoAtual, int anoF, int mesF, int diaF)
        {
            #region - code -
            using (var db = new AutorizadorCNDB())
            {
                //busca trans antigas

                var targetDtFech = new DateTime(anoF, mesF, diaF, 0, 5, 0);

                var lstOld = db.LOG_Fechamento.Where(y => y.fk_empresa == fkEmpresa && y.st_mes == mesAnt && y.st_ano == anoAnt).ToList();
                var lstCurrentFech = db.LOG_Fechamento.Where(y => y.fk_empresa == fkEmpresa && y.st_mes == mesAtual && y.st_ano == anoAtual).ToList();

                foreach (var itemFech in lstOld)
                {
                    var parcOldFech = db.T_Parcelas.FirstOrDefault(y => itemFech.fk_parcela == y.i_unique);

                    if (parcOldFech == null)
                        continue;

                    var logTrans = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parcOldFech.fk_log_transacoes);

                    if (logTrans == null)
                        continue;

                    if (parcOldFech != null)
                    {
                        if (parcOldFech.nu_indice < parcOldFech.nu_tot_parcelas)
                        {
                            // tem continuação

                            var lstTotParc = db.T_Parcelas.Where(y => y.fk_log_transacoes == parcOldFech.fk_log_transacoes).OrderBy(y => y.i_unique).ToList();

                            var nuParc = 0;

                            foreach (var currentParcela in lstTotParc)
                            {
                                if (currentParcela.nu_indice > parcOldFech.nu_indice)
                                {
                                    currentParcela.nu_parcela = nuParc;
                                    db.Update(currentParcela);

                                    if (nuParc == 0)
                                    {
                                        var fe = lstCurrentFech.FirstOrDefault(y => y.fk_parcela == currentParcela.i_unique);
                                                                               
                                        if (fe == null)
                                        {
                                            db.Insert(new LOG_Fechamento
                                            {
                                                dt_compra = logTrans.dt_transacao,
                                                dt_fechamento = targetDtFech,
                                                fk_cartao = parcOldFech.fk_cartao,
                                                fk_empresa = parcOldFech.fk_empresa,
                                                fk_loja = parcOldFech.fk_loja,
                                                fk_parcela = (int)currentParcela.i_unique,
                                                nu_parcela = currentParcela.nu_indice,
                                                st_afiliada = "",
                                                st_ano = anoAtual,
                                                st_mes = mesAtual,
                                                vr_valor = currentParcela.vr_valor
                                            });
                                        }
                                        else
                                        {
                                            fe.st_mes = mesAtual;
                                            fe.st_ano = anoAtual;
                                            fe.dt_fechamento = targetDtFech;
                                            fe.nu_parcela = currentParcela.nu_indice;

                                            db.Update(fe);
                                        }
                                    }

                                    nuParc++;
                                }
                            }

                        }
                    }
                }

            }
            #endregion
        }


        static void ReFecha(string mes, string ano, int fkEmpresa, int anoF, int mesF, int diaF, bool reconstroi)
        {
            #region - code -
            using (var db = new AutorizadorCNDB())
            {
                // ------------------------------
                // desfaz fechamento
                // ------------------------------

                var lstDelFech = new List<long>();

                Console.WriteLine("--------- ajustando parcelas antigas da empresa " + fkEmpresa);

                var lstOld = db.LOG_Fechamento.Where(y => y.fk_empresa == fkEmpresa && y.st_mes == mes && y.st_ano == ano).ToList();

                int counterOld = 0;

                foreach (var itemFech in lstOld)
                {
                    ++counterOld;

                    Console.WriteLine("--------- ajustando parcelas antigas " + counterOld + " de " + lstOld.Count());

                    lstDelFech.Add((long)itemFech.i_unique);

                    var parc = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemFech.fk_parcela);
                    var logTrans = parc.fk_log_transacoes.ToString();
                    var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == itemFech.fk_cartao);

                    var lstParcs = db.T_Parcelas.Where(y => y.fk_log_transacoes.ToString() == logTrans).OrderBy(y => y.nu_indice).ToList();

                    foreach (var itemParc in lstParcs)
                    {
                        if (itemParc.i_unique >= itemFech.fk_parcela)
                        {
                            var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemParc.i_unique);
                            parcUpd.nu_parcela++;
                            db.Update(parcUpd);
                        }
                    }
                }

                Console.WriteLine("--------- Limpando antigo fechamento => " + lstDelFech.Count());

                foreach (var item in lstDelFech)
                {
                    var itemF = db.LOG_Fechamento.FirstOrDefault(y => y.i_unique == item);
                    db.Delete(itemF);
                }

                if (reconstroi)
                {
                    // reconstroi o fechamento
                    Console.WriteLine("--------- Fechamento vai ser reconstruido");

                    var lst = db.T_Parcelas.Where(y => y.fk_empresa == fkEmpresa && y.nu_parcela > 0).ToList();

                    int index = 0;

                    foreach (var parc in lst)
                    {
                        ++index;

                        Console.WriteLine("--------- Fechamento vai ser reconstruido " + index + " de " + lst.Count());

                        var logTrans = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                        if (logTrans != null)
                        {
                            if (logTrans.dt_transacao > new DateTime(anoF, mesF, diaF))
                                continue;

                            if (logTrans.tg_confirmada.ToString() != TipoConfirmacao.Confirmada)
                                continue;
                        }
                        else
                            continue;

                        var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == parc.i_unique);
                        parcUpd.nu_parcela--;
                        db.Update(parcUpd);

                        if (parcUpd.nu_parcela == 0)
                            db.Insert(new LOG_Fechamento
                            {
                                dt_compra = logTrans.dt_transacao,
                                dt_fechamento = DateTime.Now,
                                fk_cartao = parc.fk_cartao,
                                fk_empresa = parc.fk_empresa,
                                fk_loja = parc.fk_loja,
                                fk_parcela = (int)parc.i_unique,
                                nu_parcela = parc.nu_parcela,
                                st_afiliada = "",
                                st_ano = ano,
                                st_mes = mes,
                                vr_valor = parc.vr_valor
                            });
                    }

                    Console.WriteLine("##Fim!");
                }

            }
            #endregion
        }
    }
}
