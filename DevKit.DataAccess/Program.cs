using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using ClosedXML.Excel;
using System.Collections.Generic;

namespace GetStarted
{
    class Program
    {
        #region - funcs -

        static long GetNumberFromReal(string f)
        {
            var vr_proc = f.ToUpper().Trim();

            vr_proc = vr_proc.Replace("R$", "").Trim();

            if (!vr_proc.Contains(","))
                vr_proc += "00";
            else
                vr_proc = vr_proc.Replace(",", "");

            return Convert.ToInt64(vr_proc);
        }

        #endregion

        static string LimpaCampo (string origem)
        {
            var resp = "";

            foreach (var c in origem)
            {
                if (!Char.IsControl(c))
                {
                    resp += c;
                }
            }

            return resp;
        }

        static void Main(string[] args)
        {
            using (var db = new DevKitDB())
            {
                int opt = 0;

                var setup = new Setup();

                switch (opt)
                {
                    case 0:
                        {
                            var excel = new XLWorkbook("C:\\carga\\ValoresPacote.xlsx");
                            var sheetMateriais = excel.Worksheets.Skip(2).Take(1).FirstOrDefault();
                            var sheetMedicamentos = excel.Worksheets.Skip(3).Take(1).FirstOrDefault();

                            // ---------------------
                            // materiais
                            // ---------------------
                            {
                                int currentRow = 5;

                                var lstFinal = new List<string>();

                                while (true)
                                {
                                    if (currentRow > 6750)
                                        break;

                                    var id = sheetMateriais.Cell(currentRow, 1).Value.ToString();
                                    var desc = LimpaCampo ( sheetMateriais.Cell(currentRow, 2).Value.ToString() );
                                    var descCom = LimpaCampo(sheetMateriais.Cell(currentRow, 3).Value.ToString() );
                                    var fabric = sheetMateriais.Cell(currentRow, 4).Value.ToString();
                                    var facao = sheetMateriais.Cell(currentRow, 5).Value.ToString();
                                    var facriocinar = sheetMateriais.Cell(currentRow, 6).Value.ToString();
                                    var unidade = sheetMateriais.Cell(currentRow, 7).Value.ToString();
                                    var valor = sheetMateriais.Cell(currentRow, 8).Value.ToString();

                                    var id_next = sheetMateriais.Cell(currentRow + 1, 1).Value.ToString();
                                    var desc_next = LimpaCampo(sheetMateriais.Cell(currentRow + 1, 2).Value.ToString());
                                    var descCom_next = LimpaCampo(sheetMateriais.Cell(currentRow + 1, 3).Value.ToString());
                                    var fabric_next = sheetMateriais.Cell(currentRow + 1, 4).Value.ToString();
                                    var facao_next = sheetMateriais.Cell(currentRow + 1, 5).Value.ToString();
                                    var facriocinar_next = sheetMateriais.Cell(currentRow + 1, 6).Value.ToString();
                                    var unidade_next = sheetMateriais.Cell(currentRow + 1, 7).Value.ToString();
                                    var valor_next = sheetMateriais.Cell(currentRow + 1, 8).Value.ToString();

                                    if (id == "" && desc == "" && descCom == "" && fabric == "" && facao == "" && facriocinar == "" && unidade == "" && valor == "")
                                    {
                                        ++currentRow;
                                    }
                                    else if (desc == "" && desc_next == "")
                                    {
                                        ++currentRow;
                                    }
                                    else
                                    {
                                        if (id != "" && id_next != "")
                                        {
                                            lstFinal.Add(id + "¨" + desc + "¨" + descCom + "¨" + fabric + "¨" + facao + "¨" + facriocinar + "¨" + unidade + "¨" + valor);

                                            ++currentRow;
                                        }
                                        else
                                        {
                                            if (id == "") id = id_next;

                                            desc += " " + desc_next;
                                            descCom += " " + descCom_next;
                                            fabric += fabric_next;
                                            facao += facao_next;
                                            facriocinar += facriocinar_next;
                                            unidade += unidade_next;
                                            valor += valor_next;

                                            lstFinal.Add(id + "¨" + desc + "¨" + descCom + "¨" + fabric + "¨" + facao + "¨" + facriocinar + "¨" + unidade + "¨" + valor);

                                            currentRow += 2;
                                        }
                                    }
                                }

                                var strFilename = "c:\\carga\\listaMateriais.txt";

                                if (File.Exists(strFilename))
                                    File.Delete(strFilename);

                                using (var sw = new StreamWriter(strFilename, false, Encoding.Default))
                                {
                                    foreach (var item in lstFinal)
                                    {
                                        sw.WriteLine(item);
                                    }
                                }
                            }

                            // ---------------------
                            // medicamentos
                            // ---------------------
                            {
                                int currentRow = 5;

                                var lstFinal = new List<string>();

                                while (true)
                                {
                                    if (currentRow > 14340)
                                        break;

                                    var id = sheetMedicamentos.Cell(currentRow, 1).Value.ToString();
                                    var desc = sheetMedicamentos.Cell(currentRow, 2).Value.ToString();
                                    var descCom = sheetMedicamentos.Cell(currentRow, 3).Value.ToString();
                                    var fabric = sheetMedicamentos.Cell(currentRow, 4).Value.ToString();
                                    var facao = sheetMedicamentos.Cell(currentRow, 5).Value.ToString();
                                    var facriocinar = sheetMedicamentos.Cell(currentRow, 6).Value.ToString();
                                    var unidade = sheetMedicamentos.Cell(currentRow, 7).Value.ToString();
                                    var valor = sheetMedicamentos.Cell(currentRow, 8).Value.ToString();

                                    var id_next = sheetMedicamentos.Cell(currentRow + 1, 1).Value.ToString();
                                    var desc_next = sheetMedicamentos.Cell(currentRow + 1, 2).Value.ToString();
                                    var descCom_next = sheetMedicamentos.Cell(currentRow + 1, 3).Value.ToString();
                                    var fabric_next = sheetMedicamentos.Cell(currentRow + 1, 4).Value.ToString();
                                    var facao_next = sheetMedicamentos.Cell(currentRow + 1, 5).Value.ToString();
                                    var facriocinar_next = sheetMedicamentos.Cell(currentRow + 1, 6).Value.ToString();
                                    var unidade_next = sheetMedicamentos.Cell(currentRow + 1, 7).Value.ToString();
                                    var valor_next = sheetMedicamentos.Cell(currentRow + 1, 8).Value.ToString();

                                    if (id == "" && desc == "" && descCom == "" && fabric == "" && facao == "" && facriocinar == "" && unidade == "" && valor == "")
                                    {
                                        ++currentRow;
                                    }
                                    else if (desc == "" && desc_next == "")
                                    {
                                        ++currentRow;
                                    }
                                    else
                                    {
                                        if (id != "" && id_next != "")
                                        {
                                            lstFinal.Add(id + "¨" + desc + "¨" + descCom + "¨" + fabric + "¨" + facao + "¨" + facriocinar + "¨" + unidade + "¨" + valor);

                                            ++currentRow;
                                        }
                                        else
                                        {
                                            if (id == "") id = id_next;

                                            desc += " " + desc_next;
                                            descCom += " " + descCom_next;
                                            fabric += fabric_next;
                                            facao += facao_next;
                                            facriocinar += facriocinar_next;
                                            unidade += unidade_next;
                                            valor += valor_next;

                                            lstFinal.Add(id + "¨" + desc + "¨" + descCom + "¨" + fabric + "¨" + facao + "¨" + facriocinar + "¨" + unidade + "¨" + valor);

                                            currentRow += 2;
                                        }
                                    }
                                }

                                var strFilename = "c:\\carga\\listaMedicamentos.txt";

                                if (File.Exists(strFilename))
                                    File.Delete(strFilename);

                                using (var sw = new StreamWriter(strFilename, false, Encoding.Default))
                                {
                                    foreach (var item in lstFinal)
                                    {
                                        sw.WriteLine(item);
                                    }
                                }
                            }

                            break;
                        }

                    case 1:
                        {
                            Console.WriteLine("CARGA DE BASE");
                            Console.WriteLine("-----------------------------------------");

                            if (!db.Estado.Any())
                            {
                                #region - carga de estado e cidade - 

                                var hashEstado = new Hashtable();

                                using (var sr = new StreamReader("c:\\carga\\log_faixa_uf.txt", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga de base [ESTADO].");

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line != "")
                                        {
                                            var ar = line.Replace("\"", "").Split(';');

                                            var est = new Estado
                                            {
                                                stNome = ar[1],
                                                stSigla = ar[0]
                                            };

                                            est.id = Convert.ToInt64(db.InsertWithIdentity(est));

                                            hashEstado[est.stSigla] = est.id;
                                        }
                                    }
                                }

                                #endregion

                                #region - carga de cidades -

                                using (var sr = new StreamReader("c:\\carga\\log_localidade.txt", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga de base [CIDADE].");

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line != "")
                                        {
                                            var ar = line.Replace("\"", "").Split(';');

                                            var cid = new Cidade
                                            {
                                                fkEstado = hashEstado[ar[4]] as long?,
                                                stNome = ar[1]
                                            };

                                            cid.id = Convert.ToInt64(db.InsertWithIdentity(cid));
                                        }
                                    }
                                }

                                #endregion
                            }

                            if (!db.Associado.Where(y => y.fkEmpresa == 1).Any())
                            {
                                #region - carga 1801 - 

                                using (var sr = new StreamReader("c:\\carga\\cartoes_1801.csv", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga de base [CARTÕES 1801].");

                                    db.Insert(new EmpresaSecao
                                    {
                                        fkEmpresa = 1,
                                        nuEmpresa = 1801,
                                        stDesc = "Fumam Ativos"
                                    });

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line == ";;;")
                                            continue;

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 1,
                                                fkSecao = 1,
                                                fkUserAdd = 1,
                                                nuMatricula = Convert.ToInt64(ar[0]),
                                                nuViaCartao = 1,
                                                nuTitularidade = 1,
                                                stName = ar[1],
                                                stCPF = ar[2],
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));
                                        }
                                    }
                                }

                                #endregion

                                #region - carga 1802 - 

                                using (var sr = new StreamReader("c:\\carga\\cartoes_1802.csv", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga de base [CARTÕES 1802].");

                                    db.Insert(new EmpresaSecao
                                    {
                                        fkEmpresa = 1,
                                        nuEmpresa = 1802,
                                        stDesc = "Fumam Inativos"
                                    });

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line == ";;;")
                                            continue;

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 1,
                                                fkSecao = 2,
                                                fkUserAdd = 1,
                                                nuMatricula = Convert.ToInt64(ar[0]),
                                                nuViaCartao = 1,
                                                nuTitularidade = 1,
                                                stName = ar[1],
                                                stCPF = ar[2],
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));
                                        }
                                    }
                                }

                                #endregion

                                #region - carga 1803 - 

                                using (var sr = new StreamReader("c:\\carga\\cartoes_1803.csv", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga de base [CARTÕES 1803].");

                                    db.Insert(new EmpresaSecao
                                    {
                                        fkEmpresa = 1,
                                        nuEmpresa = 1803,
                                        stDesc = "Fumam Vereadores"
                                    });

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line == ";;;")
                                            continue;

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 1,
                                                fkSecao = 3,
                                                fkUserAdd = 1,
                                                nuMatricula = Convert.ToInt64(ar[0]),
                                                nuViaCartao = 1,
                                                nuTitularidade = 1,
                                                stName = ar[1],
                                                stCPF = ar[2],
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));
                                        }
                                    }
                                }

                                #endregion
                            }

                            if (!db.TUSS.Any())
                            {
                                #region - carga tuss - 

                                Console.WriteLine("Carga de tuss");

                                var excel = new XLWorkbook("C:\\carga\\tb_tuss.xlsx");
                                var sheet = excel.Worksheets.FirstOrDefault();

                                int currentRow = 2;

                                for (; ; currentRow++)
                                {
                                    if (sheet.Cell(currentRow, 1).Value.ToString() == "")
                                        break;

                                    var codTuss = sheet.Cell(currentRow, 2).Value.ToString();
                                    var descgp = sheet.Cell(currentRow, 3).Value.ToString();
                                    var descsbp = sheet.Cell(currentRow, 4).Value.ToString();
                                    var proc = sheet.Cell(currentRow, 5).Value.ToString();

                                    var tableTuss = db.TUSS.Where(y => y.nuCodTUSS.ToString() == codTuss).FirstOrDefault();

                                    if (tableTuss == null)
                                    {
                                        tableTuss = new TUSS();

                                        tableTuss.nuCodTUSS = Convert.ToInt64(codTuss);
                                        tableTuss.stDescricaoGP = descgp;
                                        tableTuss.stDescricaoSubGP = descsbp;
                                        tableTuss.stProcedimento = proc;

                                        db.Insert(tableTuss);
                                    }
                                    else
                                    {
                                        tableTuss.stDescricaoGP = descgp;
                                        tableTuss.stDescricaoSubGP = descsbp;
                                        tableTuss.stProcedimento = proc;

                                        db.Update(tableTuss);
                                    }
                                }

                                #endregion
                            }

                            if (!db.Credenciado.Any())
                            {
                                #region - carga especialidade x médico - 

                                Console.WriteLine("Atualizando especialidade x médico");

                                var excel = new XLWorkbook("C:\\carga\\medico_especialidade.xlsx");
                                var sheet = excel.Worksheets.FirstOrDefault();

                                int currentRow = 2;

                                Credenciado curCred = null;

                                for (; ; currentRow++)
                                {
                                    var nomeCredenciado = sheet.Cell(currentRow, 1).Value.ToString().ToUpper().Trim();

                                    if (nomeCredenciado == "FIM")
                                        break;
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(nomeCredenciado))
                                        {
                                            var stCnpj = sheet.Cell(currentRow, 4).Value.ToString().Trim();
                                            var especialidade = sheet.Cell(currentRow, 2).Value.ToString().ToUpper().Trim();

                                            var especTb = db.Especialidade.
                                                            Where(y => y.stNome == especialidade).
                                                            FirstOrDefault();

                                            if (especTb == null)
                                            {
                                                especTb = new Especialidade
                                                {
                                                    stNome = especialidade
                                                };

                                                especTb.id = Convert.ToInt64(db.InsertWithIdentity(especTb));
                                            }

                                            curCred = db.Credenciado.Where(y => y.stCnpj == stCnpj).FirstOrDefault();

                                            if (curCred == null)
                                            {
                                                var codDisp = setup.RandomInt(4);

                                                while (db.Credenciado.Any(y => y.nuCodigo == codDisp))
                                                {
                                                    Thread.Sleep(1);
                                                    codDisp = setup.RandomInt(4);
                                                }

                                                var new_id = Convert.ToInt64(db.InsertWithIdentity(new Credenciado
                                                {
                                                    dtStart = DateTime.Now,
                                                    fkUserAdd = 1,
                                                    fkEspecialidade = especTb.id,
                                                    stNome = nomeCredenciado,
                                                    stCnpj = stCnpj,
                                                    nuCodigo = codDisp,
                                                    nuTipo = stCnpj.Length > 11 ? 2 : 1,
                                                }));

                                                db.Insert(new CredenciadoEmpresa
                                                {
                                                    fkCredenciado = new_id,
                                                    fkEmpresa = 1
                                                });

                                                curCred = db.Credenciado.Where(y => y.id == new_id).FirstOrDefault();
                                            }
                                        }
                                        else if (curCred != null)
                                        {
                                            // guarda procedimento!

                                            var cod_tuss = sheet.Cell(currentRow, 2).Value.ToString();
                                            var st_tuss = sheet.Cell(currentRow, 3).Value.ToString();

                                            var vr_proc = GetNumberFromReal(sheet.Cell(currentRow, 4).Value.ToString());
                                            var vr_cop = GetNumberFromReal(sheet.Cell(currentRow, 6).Value.ToString());
                                            var nu_q_mes = sheet.Cell(currentRow, 7).Value.ToString().ToUpper();
                                            var nu_q_ano = sheet.Cell(currentRow, 8).Value.ToString().ToUpper();
                                            var nu_parc = sheet.Cell(currentRow, 9).Value.ToString().ToUpper();

                                            var curTuss = db.TUSS.
                                                            Where(y => y.nuCodTUSS.ToString() == cod_tuss).
                                                            FirstOrDefault();

                                            if (curTuss == null)
                                            {
                                                db.Insert(new TUSS
                                                {
                                                    nuCodTUSS = Convert.ToInt64(cod_tuss),
                                                    stProcedimento = st_tuss
                                                });
                                            }

                                            var lstCurProc = db.CredenciadoEmpresaTuss.
                                                                Where(y => y.fkCredenciado == curCred.id &&
                                                                            y.nuTUSS == Convert.ToInt64(cod_tuss)).
                                                                ToList();

                                            if (lstCurProc.Any())
                                            {
                                                foreach (var item in lstCurProc)
                                                {
                                                    var itemDb = db.CredenciadoEmpresaTuss.Where(y => y.id == item.id).FirstOrDefault();

                                                    itemDb.nuMaxAno = Convert.ToInt64(nu_q_ano);
                                                    itemDb.nuMaxMes = Convert.ToInt64(nu_q_ano);
                                                    itemDb.nuParcelas = Convert.ToInt64(nu_parc);
                                                    itemDb.vrCoPart = Convert.ToInt64(vr_cop);
                                                    itemDb.vrProcedimento = Convert.ToInt64(vr_proc);

                                                    db.Update(itemDb);
                                                }
                                            }
                                            else
                                            {
                                                db.Insert(new CredenciadoEmpresaTuss
                                                {
                                                    fkEmpresa = 1,
                                                    fkCredenciado = curCred.id,
                                                    nuMaxAno = Convert.ToInt64(nu_q_ano),
                                                    nuMaxMes = Convert.ToInt64(nu_q_ano),
                                                    nuParcelas = Convert.ToInt64(nu_parc),
                                                    nuTUSS = Convert.ToInt64(cod_tuss),
                                                    vrCoPart = Convert.ToInt64(vr_cop),
                                                    vrProcedimento = Convert.ToInt64(vr_proc),
                                                });
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }

                            if (!db.TipoCoberturaDependente.Any())
                            {
                                #region - carga -

                                Console.WriteLine("Carga de TipoCoberturaDependente");

                                db.Insert(new TipoCoberturaDependente { stDesc = "Esposa / conjuge" });
                                db.Insert(new TipoCoberturaDependente { stDesc = "Filho menor 21 anos" });
                                db.Insert(new TipoCoberturaDependente { stDesc = "Filho maior 21 anos / estudante" });
                                db.Insert(new TipoCoberturaDependente { stDesc = "Filho maior 21 anos / doença pré-existente" });

                                #endregion
                            }

                            Console.WriteLine("Carga de base finalizada.");
                            Console.WriteLine("-----------------------------------------");
                            Console.ReadLine();

                            break;
                        }
                }
            }
        }
    }
}
