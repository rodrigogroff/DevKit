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
using DevKit.DataAccess;

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

        static long LimpaValor(string origem)
        {
            var resp = origem.Replace("R$","").Replace(".","").Replace (",","").Trim();

            if (resp == "")
                return 0;

            return Convert.ToInt64(resp);
        }

        static long LimpaValor4C(string origem)
        {
            var resp = origem.Replace("R$", "").Replace(".", "").Trim();

            if (resp == "")
                return 0;

            var antes = resp.Split(',')[0];
            var depois = resp.Split(',')[1].PadRight(4, '0');

            return Convert.ToInt64(antes + depois);
        }

        #endregion

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("Patch?");
                Console.WriteLine("--------------------");

                var opt = Console.ReadLine();

                using (var db = new DevKitDB())
                {
                    var setup = new Setup();

                    switch (opt)
                    {
                        case "fixAutorizacao":
                            {
                                var usr = db.User.FirstOrDefault();

                                foreach (var item in db.Autorizacao.ToList())
                                {
                                    var i = db.Autorizacao.FirstOrDefault(y=> y.id == item.id);

                                    if (i != null)
                                    {
                                        i.nuTipoAutorizacao = 1;
                                        i.fkPrecificacao = i.fkProcedimento;

                                        db.Update(i);

                                        Console.WriteLine("Aut -> " + item.id);
                                    }
                                }

                                break;
                            }

                        case "fixCPF":
                            {
                                
                                foreach (var item in db.Associado.ToList())
                                {
                                    if (string.IsNullOrEmpty(item.stCPF))
                                    {
                                        item.stCPF = "11111111111";

                                        Console.WriteLine("TIT -> " + item.stName);

                                        db.Update(item);
                                    }
                                }

                                foreach (var item in db.AssociadoDependente.ToList())
                                {
                                    if (string.IsNullOrEmpty(item.stCPF))
                                    {
                                        item.stCPF = "11111111111";

                                        Console.WriteLine("DEP -> " + item.stNome);

                                        db.Update(item);
                                    }
                                }

                                break;
                            }

                        case "1":
                            {
                                #region - carga 1801 - 

                                using (var sr = new StreamReader("c:\\carga\\1801_TIT.txt", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga titulares extras [1801].");

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 1,
                                                fkSecao = 1,
                                                fkUserAdd = 1,
                                                nuMatricula  = Convert.ToInt64(ar[0]),
                                                nuMatSaude = Convert.ToInt64(ar[1]),
                                                nuViaCartao = 1,
                                                nuTitularidade = 1,
                                                stName = ar[2],
                                                stCPF = "111111111111",
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));
                                        }
                                    }
                                }

                                #endregion

                                break;
                            }

                        case "2":
                            {
                                #region - carga 1801_DEP_F01 - 

                                using (var sr = new StreamReader("c:\\carga\\1801_DEP_F01.txt", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga titulares extras [1801_DEP_F01]");

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var tit = db.Associado.FirstOrDefault(y => y.nuMatricula == Convert.ToInt64(ar[0]) && 
                                                                                       y.fkEmpresa == 1 && 
                                                                                       y.fkSecao == 1);

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 1,
                                                fkSecao = 1,
                                                fkUserAdd = 1,
                                                nuMatricula = Convert.ToInt64(ar[0]),
                                                nuMatSaude = tit.nuMatSaude,
                                                nuViaCartao = 1,
                                                nuTitularidade = db.AssociadoDependente.Where(y => y.fkAssociado == tit.id).Count() + 2,
                                                stName = ar[2],
                                                stCPF = "111111111111",
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));

                                            db.Insert ( new AssociadoDependente()
                                            {                                                                                                                    
                                                dtNasc = new DateTime(Convert.ToInt32(ar[3].Substring(6, 4)), Convert.ToInt32(ar[3].Substring(3, 2)), Convert.ToInt32(ar[3].Substring(0, 2))),
                                                fkAssociado = tit.id,
                                                fkCartao = new_id,
                                                fkEmpresa = 1,
                                                fkTipoCoberturaDependente = 2
                                            });
                                        }
                                    }
                                }

                                #endregion

                                break;
                            }

                        case "3":
                            {
                                #region - carga 1801_DEPC - 

                                using (var sr = new StreamReader("c:\\carga\\1801_DEPC.txt", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga titulares extras [1801_DEPC]");

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var tit = db.Associado.FirstOrDefault(y => y.nuMatricula == Convert.ToInt64(ar[0]) &&
                                                                                       y.fkEmpresa == 1 &&
                                                                                       y.fkSecao == 1);

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 1,
                                                fkSecao = 1,
                                                fkUserAdd = 1,
                                                nuMatricula = Convert.ToInt64(ar[0]),
                                                nuMatSaude = tit.nuMatSaude,
                                                nuViaCartao = 1,
                                                nuTitularidade = db.AssociadoDependente.Where(y => y.fkAssociado == tit.id).Count() + 2,
                                                stName = ar[1],
                                                stCPF = "111111111111",
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));

                                            db.Insert(new AssociadoDependente()
                                            {
                                                dtNasc = new DateTime(1980,1,1),
                                                fkAssociado = tit.id,
                                                fkCartao = new_id,
                                                fkEmpresa = 1,
                                                fkTipoCoberturaDependente = 1
                                            });
                                        }
                                    }
                                }

                                #endregion

                                break;
                            }

                        default:
                        case "ajusta_cart_deps_dtNasc":
                            {
                                Console.WriteLine("ajusta_cart_deps_dtNasc");

                                int t = 0;

                                foreach (var item in db.AssociadoDependente.
                                                OrderBy(y => y.fkAssociado).
                                                ThenBy(y => y.id).
                                                ToList())
                                {
                                    var assoc = db.Associado.FirstOrDefault(y => y.id == item.fkCartao);

                                    if (assoc != null)
                                        if (item.dtNasc != null)
                                        {
                                            assoc.nuDayAniversary = Convert.ToDateTime(item.dtNasc).Day;
                                            assoc.nuMonthAniversary = Convert.ToDateTime(item.dtNasc).Month;
                                            assoc.nuYearBirth = Convert.ToDateTime(item.dtNasc).Year;

                                            db.Update(assoc);
                                            t++;
                                        }
                                }

                                Console.WriteLine("updates " + t);

                                break;
                            }
                                                            
                        case "ajusta_cart_deps":
                            {
                                Console.WriteLine("ajusta_cart_deps");

                                var lst = db.AssociadoDependente.
                                                OrderBy(y=> y.fkAssociado).
                                                ThenBy(y=> y.id).
                                                ToList();

                                int currentTit = 0, currentMat = 0, updates = 0;

                                foreach (var item in lst)
                                {
                                    var assoc = db.Associado.FirstOrDefault(y => y.id == item.fkAssociado);

                                    if (currentMat == 0 || currentMat != assoc.nuMatricula)
                                    {
                                        currentMat = (int)assoc.nuMatricula;
                                        currentTit = 2;
                                    }

                                    var cartDep = db.Associado.FirstOrDefault(y => y.nuMatricula == currentMat &&
                                                                                    y.nuTitularidade == currentTit &&
                                                                                    y.fkEmpresa == assoc.fkEmpresa &&
                                                                                    y.fkSecao == assoc.fkSecao);

                                    if (cartDep != null)
                                    {
                                        item.fkCartao = cartDep.id;
                                        db.Update(item);
                                        currentTit++;
                                        updates++;
                                    }
                                }

                                Console.WriteLine("updates " + updates);

                                break;
                            }

                        case "pacote":
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
                                        var desc = LimpaCampo(sheetMateriais.Cell(currentRow, 2).Value.ToString());
                                        var descCom = LimpaCampo(sheetMateriais.Cell(currentRow, 3).Value.ToString());
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

                        case "geral":
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
                                Console.WriteLine("");
                                Console.WriteLine("Deseja carregar a base de valores de pacote?");

                                var resp = Console.ReadLine();

                                if (resp.ToUpper() == "S")
                                {
                                    Console.WriteLine("Código da empresa?");
                                    var stEmp = Console.ReadLine();

                                    var empresa = db.Empresa.FirstOrDefault(y => y.nuEmpresa.ToString() == stEmp);

                                    if (empresa == null)
                                    {
                                        Console.WriteLine("Empresa inválida!");
                                        return;
                                    }

                                    var excel = new XLWorkbook("C:\\carga\\ValoresPacote.xlsx");

                                    var sheetVigencia = excel.Worksheets.Skip(0).Take(1).FirstOrDefault();
                                    var sheetDiarias = excel.Worksheets.Skip(1).Take(1).FirstOrDefault();
                                    var sheetMateriais = excel.Worksheets.Skip(2).Take(1).FirstOrDefault();
                                    var sheetMedicamentos = excel.Worksheets.Skip(3).Take(1).FirstOrDefault();
                                    var sheetNaoMeds = excel.Worksheets.Skip(4).Take(1).FirstOrDefault();
                                    var sheetOPME = excel.Worksheets.Skip(5).Take(1).FirstOrDefault();
                                    var sheetPacotes = excel.Worksheets.Skip(6).Take(1).FirstOrDefault();
                                    var sheetProcs = excel.Worksheets.Skip(7).Take(1).FirstOrDefault();

                                    var nuAnoAtual = sheetVigencia.Cell(2, 3).Value.ToString();

                                    // -------------------
                                    // diarias
                                    // -------------------

                                    Console.WriteLine("Diarias...");

                                    #region - code -
                                    {
                                        int currentRow = 2;

                                        foreach (var item in db.SaudeValorDiaria.
                                                    Where(y => y.fkEmpresa == empresa.id &&
                                                                y.nuAnoVigencia.ToString() == nuAnoAtual).
                                                    ToList())
                                        {
                                            db.Delete(item);
                                        }

                                        while (currentRow < 500)
                                        {
                                            var codigo = sheetDiarias.Cell(currentRow, 1).Value.ToString();
                                            var desc = LimpaCampo(sheetDiarias.Cell(currentRow, 2).Value.ToString());

                                            var nvl5 = LimpaValor(sheetDiarias.Cell(currentRow, 3).Value.ToString());
                                            var nvl4 = LimpaValor(sheetDiarias.Cell(currentRow, 4).Value.ToString());
                                            var nvl3 = LimpaValor(sheetDiarias.Cell(currentRow, 5).Value.ToString());
                                            var nvl2 = LimpaValor(sheetDiarias.Cell(currentRow, 6).Value.ToString());
                                            var nvl1 = LimpaValor(sheetDiarias.Cell(currentRow, 7).Value.ToString());

                                            currentRow++;

                                            if (codigo != "")
                                            {
                                                db.Insert(new SaudeValorDiaria
                                                {
                                                    fkEmpresa = empresa.id,
                                                    nuAnoVigencia = Convert.ToInt32(nuAnoAtual),
                                                    nuCodInterno = Convert.ToInt64(codigo),
                                                    stDesc = desc,
                                                    vrNivel1 = nvl1,
                                                    vrNivel2 = nvl2,
                                                    vrNivel3 = nvl3,
                                                    vrNivel4 = nvl4,
                                                    vrNivel5 = nvl5,
                                                });
                                            }
                                        }
                                    }
                                    #endregion

                                    // -------------------
                                    // materiais
                                    // -------------------

                                    Console.WriteLine("materiais...");

                                    #region - code - 
                                    {
                                        int currentRow = 2;

                                        foreach (var item in db.SaudeValorMaterial.
                                                    Where(y => y.fkEmpresa == empresa.id &&
                                                                y.nuAnoVigencia.ToString() == nuAnoAtual).
                                                    ToList())
                                        {
                                            db.Delete(item);
                                        }

                                        while (currentRow < 3500)
                                        {
                                            var codigo = sheetMateriais.Cell(currentRow, 1).Value.ToString();
                                            var desc = LimpaCampo(sheetMateriais.Cell(currentRow, 2).Value.ToString());
                                            var nomeCom = LimpaCampo(sheetMateriais.Cell(currentRow, 3).Value.ToString());
                                            var fab = LimpaCampo(sheetMateriais.Cell(currentRow, 4).Value.ToString());
                                            var fracao = LimpaValor(sheetMateriais.Cell(currentRow, 5).Value.ToString());
                                            var fracionar = LimpaCampo(sheetMateriais.Cell(currentRow, 5).Value.ToString());
                                            var unidade = LimpaCampo(sheetMateriais.Cell(currentRow, 5).Value.ToString());
                                            var vlr = LimpaValor(sheetMateriais.Cell(currentRow, 5).Value.ToString());

                                            if (!db.SaudeFabricanteMaterialEmpresa.Any(y => y.stNome == fab && y.fkEmpresa == empresa.id))
                                            {
                                                db.Insert(new SaudeFabricanteMaterialEmpresa
                                                {
                                                    fkEmpresa = empresa.id,
                                                    stNome = fab
                                                });
                                            }

                                            if (!db.SaudeUnidadeEmpresa.Any(y => y.stNome == unidade && y.fkEmpresa == empresa.id))
                                            {
                                                db.Insert(new SaudeUnidadeEmpresa
                                                {
                                                    fkEmpresa = empresa.id,
                                                    stNome = unidade
                                                });
                                            }

                                            var tbFab = db.SaudeFabricanteMaterialEmpresa.FirstOrDefault(y => y.stNome == fab && y.fkEmpresa == empresa.id);
                                            var tbUnidade = db.SaudeUnidadeEmpresa.FirstOrDefault(y => y.stNome == unidade && y.fkEmpresa == empresa.id);

                                            currentRow++;

                                            if (codigo != "")
                                            {
                                                db.Insert(new SaudeValorMaterial
                                                {
                                                    fkEmpresa = empresa.id,
                                                    nuAnoVigencia = Convert.ToInt32(nuAnoAtual),
                                                    nuCodInterno = Convert.ToInt64(codigo),
                                                    stDesc = desc,
                                                    stComercial = nomeCom,
                                                    bFracionar = fracionar == "SIM",
                                                    fkFabricanteMaterial = tbFab.id,
                                                    fkUnidade = tbUnidade.id,
                                                    vrFracao = fracao,
                                                    vrValor = vlr
                                                });
                                            }
                                        }
                                    }
                                    #endregion

                                    // -------------------
                                    // medicamentos
                                    // -------------------

                                    Console.WriteLine("medicamentos...");

                                    #region - code - 
                                    {
                                        int currentRow = 2;

                                        foreach (var item in db.SaudeValorMedicamento.
                                                    Where(y => y.fkEmpresa == empresa.id &&
                                                                y.nuAnoVigencia.ToString() == nuAnoAtual).
                                                    ToList())
                                        {
                                            db.Delete(item);
                                        }

                                        while (currentRow < 6500)
                                        {
                                            var codigo = sheetMedicamentos.Cell(currentRow, 1).Value.ToString();
                                            var desc = LimpaCampo(sheetMedicamentos.Cell(currentRow, 2).Value.ToString());
                                            var nomeCom = LimpaCampo(sheetMedicamentos.Cell(currentRow, 3).Value.ToString());
                                            var fab = LimpaCampo(sheetMedicamentos.Cell(currentRow, 4).Value.ToString());
                                            var fracao = LimpaValor(sheetMedicamentos.Cell(currentRow, 5).Value.ToString());
                                            var fracionar = LimpaCampo(sheetMedicamentos.Cell(currentRow, 5).Value.ToString());
                                            var unidade = LimpaCampo(sheetMedicamentos.Cell(currentRow, 5).Value.ToString());
                                            var vlr = LimpaValor(sheetMedicamentos.Cell(currentRow, 5).Value.ToString());

                                            if (!db.SaudeFabricanteMedicamentoEmpresa.Any(y => y.stNome == fab && y.fkEmpresa == empresa.id))
                                            {
                                                db.Insert(new SaudeFabricanteMedicamentoEmpresa
                                                {
                                                    fkEmpresa = empresa.id,
                                                    stNome = fab
                                                });
                                            }

                                            if (!db.SaudeUnidadeEmpresa.Any(y => y.stNome == unidade && y.fkEmpresa == empresa.id))
                                            {
                                                db.Insert(new SaudeUnidadeEmpresa
                                                {
                                                    fkEmpresa = empresa.id,
                                                    stNome = unidade
                                                });
                                            }

                                            var tbFab = db.SaudeFabricanteMedicamentoEmpresa.FirstOrDefault(y => y.stNome == fab && y.fkEmpresa == empresa.id);
                                            var tbUnidade = db.SaudeUnidadeEmpresa.FirstOrDefault(y => y.stNome == unidade && y.fkEmpresa == empresa.id);

                                            currentRow++;

                                            if (codigo != "")
                                            {
                                                db.Insert(new SaudeValorMedicamento
                                                {
                                                    fkEmpresa = empresa.id,
                                                    nuAnoVigencia = Convert.ToInt32(nuAnoAtual),
                                                    nuCodInterno = Convert.ToInt64(codigo),
                                                    stDesc = desc,
                                                    stComercial = nomeCom,
                                                    bFracionar = fracionar == "SIM",
                                                    fkFabricanteMedicamento = tbFab.id,
                                                    fkUnidade = tbUnidade.id,
                                                    vrFracao = fracao,
                                                    vrValor = vlr
                                                });
                                            }
                                        }
                                    }
                                    #endregion

                                    // -------------------
                                    // não medico
                                    // -------------------

                                    Console.WriteLine("não medico...");

                                    #region - code - 
                                    {
                                        int currentRow = 2;

                                        foreach (var item in db.SaudeValorNaoMedico.
                                                    Where(y => y.fkEmpresa == empresa.id &&
                                                                y.nuAnoVigencia.ToString() == nuAnoAtual).
                                                    ToList())
                                        {
                                            db.Delete(item);
                                        }

                                        while (currentRow < 100)
                                        {
                                            var codigo = sheetNaoMeds.Cell(currentRow, 1).Value.ToString();
                                            var desc = LimpaCampo(sheetNaoMeds.Cell(currentRow, 2).Value.ToString());
                                            var vlr = LimpaValor(sheetNaoMeds.Cell(currentRow, 3).Value.ToString());

                                            currentRow++;

                                            if (codigo != "")
                                            {
                                                db.Insert(new SaudeValorNaoMedico
                                                {
                                                    fkEmpresa = empresa.id,
                                                    nuAnoVigencia = Convert.ToInt32(nuAnoAtual),
                                                    nuCodInterno = Convert.ToInt64(codigo),
                                                    stDesc = desc,
                                                    vrValor = vlr
                                                });
                                            }
                                        }
                                    }
                                    #endregion

                                    // -------------------
                                    // opme
                                    // -------------------

                                    Console.WriteLine("opme ...");

                                    #region - code - 
                                    {
                                        int currentRow = 2;

                                        foreach (var item in db.SaudeValorOPME.
                                                    Where(y => y.fkEmpresa == empresa.id &&
                                                                y.nuAnoVigencia.ToString() == nuAnoAtual).
                                                    ToList())
                                        {
                                            db.Delete(item);
                                        }

                                        while (currentRow < 1000)
                                        {
                                            var codigo = sheetOPME.Cell(currentRow, 1).Value.ToString();
                                            var desc = LimpaCampo(sheetOPME.Cell(currentRow, 2).Value.ToString());
                                            var classif = LimpaCampo(sheetOPME.Cell(currentRow, 3).Value.ToString());
                                            var especialidade = LimpaCampo(sheetOPME.Cell(currentRow, 4).Value.ToString());
                                            var vlr = LimpaValor(sheetOPME.Cell(currentRow, 5).Value.ToString());

                                            if (!db.SaudeOPMEClassificacaoEmpresa.Any(y => y.stNome == classif && y.fkEmpresa == empresa.id))
                                            {
                                                db.Insert(new SaudeOPMEClassificacaoEmpresa
                                                {
                                                    fkEmpresa = empresa.id,
                                                    stNome = classif
                                                });
                                            }

                                            if (!db.SaudeOPMEEspecialidadeEmpresa.Any(y => y.stNome == especialidade && y.fkEmpresa == empresa.id))
                                            {
                                                db.Insert(new SaudeOPMEEspecialidadeEmpresa
                                                {
                                                    fkEmpresa = empresa.id,
                                                    stNome = especialidade
                                                });
                                            }

                                            var tbClassif = db.SaudeOPMEClassificacaoEmpresa.FirstOrDefault(y => y.stNome == classif && y.fkEmpresa == empresa.id);
                                            var tbEspec = db.SaudeOPMEEspecialidadeEmpresa.FirstOrDefault(y => y.stNome == especialidade && y.fkEmpresa == empresa.id);

                                            currentRow++;

                                            if (codigo != "")
                                            {
                                                db.Insert(new SaudeValorOPME
                                                {
                                                    fkEmpresa = empresa.id,
                                                    nuAnoVigencia = Convert.ToInt32(nuAnoAtual),
                                                    nuCodInterno = Convert.ToInt64(codigo),
                                                    stDesc = desc,
                                                    vrValor = vlr,
                                                    fkClassificacao = tbClassif.id,
                                                    fkEspecialidade = tbEspec.id,
                                                    stTecnica = ""
                                                });
                                            }
                                        }
                                    }
                                    #endregion

                                    // -------------------
                                    // pacote
                                    // -------------------

                                    Console.WriteLine("pacote ...");

                                    #region - code - 
                                    {
                                        int currentRow = 2;

                                        foreach (var item in db.SaudeValorPacote.
                                                    Where(y => y.fkEmpresa == empresa.id &&
                                                                y.nuAnoVigencia.ToString() == nuAnoAtual).
                                                    ToList())
                                        {
                                            db.Delete(item);
                                        }

                                        while (currentRow < 250)
                                        {
                                            var codigo = sheetPacotes.Cell(currentRow, 1).Value.ToString();
                                            var desc = LimpaCampo(sheetPacotes.Cell(currentRow, 2).Value.ToString());
                                            var vlr = LimpaValor(sheetPacotes.Cell(currentRow, 3).Value.ToString());

                                            currentRow++;

                                            if (codigo != "")
                                            {
                                                db.Insert(new SaudeValorPacote
                                                {
                                                    fkEmpresa = empresa.id,
                                                    nuAnoVigencia = Convert.ToInt32(nuAnoAtual),
                                                    nuCodInterno = Convert.ToInt64(codigo),
                                                    stDesc = desc,
                                                    vrValor = vlr,
                                                });
                                            }
                                        }
                                    }
                                    #endregion

                                    // -------------------
                                    // procedimentos
                                    // -------------------

                                    Console.WriteLine("procedimentos ...");

                                    #region - code - 
                                    {
                                        int currentRow = 2;

                                        foreach (var item in db.SaudeValorProcedimento.
                                                    Where(y => y.fkEmpresa == empresa.id &&
                                                                y.nuAnoVigencia.ToString() == nuAnoAtual).
                                                    ToList())
                                        {
                                            db.Delete(item);
                                        }

                                        while (currentRow < 4000)
                                        {
                                            var codigo = sheetProcs.Cell(currentRow, 1).Value.ToString();
                                            var desc = LimpaCampo(sheetProcs.Cell(currentRow, 2).Value.ToString());
                                            var vlrtotal = LimpaValor(sheetProcs.Cell(currentRow, 3).Value.ToString());
                                            var porte = LimpaCampo(sheetProcs.Cell(currentRow, 4).Value.ToString());
                                            var vlrhm = LimpaValor(sheetProcs.Cell(currentRow, 5).Value.ToString());
                                            var vlrco = LimpaValor(sheetProcs.Cell(currentRow, 6).Value.ToString());
                                            var naux = LimpaValor(sheetProcs.Cell(currentRow, 7).Value.ToString());
                                            var nporte = LimpaValor(sheetProcs.Cell(currentRow, 8).Value.ToString());
                                            var vlrporte = LimpaCampo(sheetProcs.Cell(currentRow, 9).Value.ToString());
                                            var filme = LimpaValor4C(sheetProcs.Cell(currentRow, 10).Value.ToString());
                                            var vrfilme = LimpaValor(sheetProcs.Cell(currentRow, 11).Value.ToString());

                                            long nvlrporte = 0;

                                            if (vlrporte.ToUpper() != "ANESTESIA LOCAL")
                                                nvlrporte = LimpaValor(sheetProcs.Cell(currentRow, 9).Value.ToString());

                                            if (!db.SaudePorteProcedimentoEmpresa.Any(y => y.stNome == porte && y.fkEmpresa == empresa.id))
                                            {
                                                db.Insert(new SaudePorteProcedimentoEmpresa
                                                {
                                                    fkEmpresa = empresa.id,
                                                    stNome = porte
                                                });
                                            }

                                            var tbPorte = db.SaudePorteProcedimentoEmpresa.FirstOrDefault(y => y.stNome == porte && y.fkEmpresa == empresa.id);

                                            currentRow++;

                                            if (codigo != "")
                                            {
                                                db.Insert(new SaudeValorProcedimento
                                                {
                                                    fkEmpresa = empresa.id,
                                                    nuAnoVigencia = Convert.ToInt32(nuAnoAtual),
                                                    nuCodInterno = Convert.ToInt64(codigo),
                                                    stDesc = desc,
                                                    vrTotalHMCO = vlrtotal,
                                                    fkSaudePorteProcedimento = tbPorte.id,
                                                    vrValorHM = vlrhm,
                                                    vrValorCO = vlrco,
                                                    nuAux = naux,
                                                    nuAnestesistas = nporte,
                                                    vrPorteAnestesista = nvlrporte,
                                                    nuFilme4C = filme,
                                                    vrFilme = vrfilme
                                                });
                                            }
                                        }
                                    }
                                    #endregion
                                }

                                Console.WriteLine("FIM!");

                                break;
                            }

                        case "geradeps":
                            {
                                var strName = "export_tits_deps.csv";

                                if (File.Exists(strName))
                                    File.Delete(strName);

                                using (var sw = new StreamWriter(strName, false, Encoding.Default))
                                {
                                    var lst = db.Associado.OrderBy(y => y.nuMatricula).
                                                            ThenBy(y => y.nuTitularidade).
                                                            ToList();

                                    var lstEmps = db.Empresa.ToList();
                                    var lstEmpSecao = db.EmpresaSecao.ToList();

                                    sw.WriteLine("empresa;secao;nome;matricula;mat saude;titularidade;dt nasc");

                                    foreach (var item in lst)
                                    {
                                        var emp = lstEmps.FirstOrDefault(y => y.id == item.fkEmpresa);
                                        var secao = lstEmpSecao.FirstOrDefault(y => y.id == item.fkSecao);

                                        var line = emp.nuEmpresa + ";" + secao.nuEmpresa + ";";

                                        line += item.stName + ";" + item.nuMatricula + ";" + item.nuMatSaude + ";" +
                                            item.nuTitularidade + ";";

                                        if (item.nuDayAniversary != null)
                                            line += item.nuDayAniversary.ToString().PadLeft(2, '0') + "." + 
                                                    item.nuMonthAniversary.ToString().PadLeft(2, '0') + "." + 
                                                    item.nuYearBirth;

                                        sw.WriteLine(line);
                                    }
                                }

                                break;
                            }

                        case "deps":
                            {
                                #region - carga de dependentes e matricula - 

                                Console.WriteLine("");
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine("Carga de dependentes e matricula");
                                Console.WriteLine("------------------------------------");

                                var excel = new XLWorkbook("C:\\carga\\deps.xlsx");
                                var sheet = excel.Worksheets.FirstOrDefault();

                                int currentRow = 2;

                                for (; ; currentRow++)
                                {
                                    if (currentRow > 14000)
                                        break;

                                    var matCartao = sheet.Cell(currentRow, 4).Value.ToString();

                                    if (matCartao == "")
                                        continue;

                                    var cartTit = db.Associado.FirstOrDefault(y => y.nuMatricula == Convert.ToInt64(matCartao));

                                    if (cartTit == null)
                                    {
                                        Console.WriteLine("NE -> " + matCartao);
                                    }
                                    else
                                    {
                                        Console.WriteLine("FE -> " + matCartao);

                                        cartTit.nuMatSaude = Convert.ToInt32(sheet.Cell(currentRow, 3).Value.ToString());

                                        db.Update(cartTit);

                                        var deps = sheet.Cell(currentRow, 2).Value.ToString().Split(',');

                                        foreach (var dep in deps)
                                        {
                                            var depStr = dep.Trim().ToUpper();

                                            if (depStr == "") continue;

                                            Console.WriteLine("FE [" + cartTit.nuMatSaude + "] DEPS -> " + depStr);

                                            var depDetails = depStr.Split(';');

                                            string depNome = depDetails[0],
                                                    depDtNasc = "",
                                                    deficiente = "N";

                                            if (depNome.Length > 25) depNome = depNome.Substring(0, 25);
                                            if (depDetails.Length >= 2) depDtNasc = depDetails[1].Trim();
                                            if (depDetails.Length >= 3) deficiente = "S";

                                            AssociadoDependente ent = new AssociadoDependente
                                            {
                                                fkEmpresa = cartTit.fkEmpresa,
                                                fkAssociado = cartTit.id,
                                                stNome = depNome
                                            };

                                            if (deficiente == "S")
                                                ent.fkTipoCoberturaDependente = 4;
                                            else
                                                ent.fkTipoCoberturaDependente = 1;

                                            if (depDtNasc != "")
                                                ent.dtNasc = new DateTime(Convert.ToInt32(depDtNasc.Substring(6, 4)),
                                                                          Convert.ToInt32(depDtNasc.Substring(3, 2)),
                                                                          Convert.ToInt32(depDtNasc.Substring(0, 2)));

                                            if (!(from ne in db.AssociadoDependente
                                                  where cartTit.fkEmpresa == ne.fkEmpresa &&
                                                      ne.fkAssociado == cartTit.id &&
                                                      ne.stNome == ent.stNome
                                                  select ne).
                                                Any())
                                            {
                                                Console.WriteLine("Inserindo ... " + ent.stNome + " > " + depDtNasc);

                                                db.Insert(new Associado
                                                {
                                                    fkEmpresa = cartTit.fkEmpresa,
                                                    fkSecao = cartTit.fkSecao,
                                                    fkUserAdd = cartTit.fkUserAdd,
                                                    nuMatricula = cartTit.nuMatricula,
                                                    nuMatSaude = cartTit.nuMatSaude,
                                                    tgStatus = TipoSituacaoCartao.Habilitado,
                                                    tgExpedicao = TipoExpedicaoCartao.Requerido,
                                                    stSenha = cartTit.stSenha,
                                                    stName = ent.stNome,
                                                    stCPF = "",
                                                    nuTitularidade = db.AssociadoDependente.Where(y => y.fkAssociado == cartTit.id).Count() + 2,
                                                    nuViaCartao = 1,
                                                    dtStart = DateTime.Now,
                                                });

                                                db.Insert(ent);
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }

                            break;
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("------------------------------------");
                Console.WriteLine(">> Patch encerrado!");
                Console.WriteLine("------------------------------------");

                Console.ReadLine();
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
