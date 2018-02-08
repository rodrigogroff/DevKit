using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using ClosedXML.Excel;

namespace GetStarted
{
	class Program
	{
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

		static void Main(string[] args)
		{
			using (var db = new DevKitDB())
			{
                int opt = 1;
                var setup = new Setup();

                // carga de tabelas txt
                switch (opt)
                {
                    case 1:
                        {
                            Console.WriteLine("-----------------------------------------");
                            Console.WriteLine("CARGA DE BASE");
                            Console.WriteLine("-----------------------------------------");
                            Console.WriteLine("");

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

                            if (!db.Associado.Where (y=> y.fkEmpresa == 1).Any())
                            {
                                #region - carga 1801 - 

                                using (var sr = new StreamReader("c:\\carga\\cartoes_1801.csv", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga de base [CARTÕES 1801].");

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line == ";;;")
                                            continue;

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado()
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 1,
                                                fkUserAdd = 1,
                                                nuMatricula = Convert.ToInt64(ar[0]),
                                                nuViaCartao = 1,
                                                nuTitularidade = 1,
                                                stName = ar[1],
                                                stCPF = ar[2],
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));

                                            new AuditLog
                                            {
                                                fkUser = 1,
                                                fkActionLog = EnumAuditAction.PersonCreate,
                                                nuType = EnumAuditType.Person,
                                                fkTarget = new_id
                                            }.
                                            Create(db, "Importação", "");
                                        }
                                    }
                                }

                                #endregion
                            }
                                                        
                            if (!db.Associado.Where(y => y.fkEmpresa == 2).Any())
                            {
                                #region - carga 1802 - 

                                using (var sr = new StreamReader("c:\\carga\\cartoes_1802.csv", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga de base [CARTÕES 1802].");

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line == ";;;")
                                            continue;

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado()
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 2,
                                                fkUserAdd = 1,
                                                nuMatricula = Convert.ToInt64(ar[0]),
                                                nuViaCartao = 1,
                                                nuTitularidade = 1,
                                                stName = ar[1],
                                                stCPF = ar[2],
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));

                                            new AuditLog
                                            {
                                                fkUser = 1,
                                                fkActionLog = EnumAuditAction.PersonCreate,
                                                nuType = EnumAuditType.Person,
                                                fkTarget = new_id
                                            }.
                                            Create(db, "Importação", "");
                                        }
                                    }
                                }

                                #endregion
                            }

                            if (!db.Associado.Where(y => y.fkEmpresa == 3).Any())
                            {
                                #region - carga 1803 - 

                                using (var sr = new StreamReader("c:\\carga\\cartoes_1803.csv", Encoding.Default, false))
                                {
                                    Console.WriteLine("Carga de base [CARTÕES 1803].");

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine();

                                        if (line == ";;;")
                                            continue;

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Associado()
                                            {
                                                dtStart = DateTime.Now,
                                                fkEmpresa = 3,
                                                fkUserAdd = 1,
                                                nuMatricula = Convert.ToInt64(ar[0]),
                                                nuViaCartao = 1,
                                                nuTitularidade = 1,
                                                stName = ar[1],
                                                stCPF = ar[2],
                                                tgExpedicao = 0,
                                                tgStatus = 0,
                                            }));

                                            new AuditLog
                                            {
                                                fkUser = 1,
                                                fkActionLog = EnumAuditAction.PersonCreate,
                                                nuType = EnumAuditType.Person,
                                                fkTarget = new_id
                                            }.
                                            Create(db, "Importação", "");
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

                                // pula header
                                int currentRow = 2;

                                for(; ;currentRow++ )
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

                            {
                                #region - carga especialidade x médico - 

                                Console.WriteLine("Atualizando especialidade x médico");

                                var excel = new XLWorkbook("C:\\carga\\medico_especialidade.xlsx");
                                var sheet = excel.Worksheets.FirstOrDefault();

                                // pula header
                                int currentRow = 2;

                                Medico curMedico = null;

                                for (; ; currentRow++)
                                {
                                    var nomeMedico = sheet.Cell(currentRow, 1).Value.ToString().ToUpper().Trim();
                                    
                                    if (nomeMedico == "FIM")
                                        break;
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(nomeMedico))
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

                                            curMedico = db.Medico.Where(y => y.stCnpj == stCnpj).FirstOrDefault();

                                            if (curMedico == null)
                                            {                                                
                                                var codDisp = setup.RandomInt(4);

                                                while (db.Medico.Any (y=>y.nuCodigo == codDisp))
                                                {
                                                    Thread.Sleep(1);
                                                    codDisp = setup.RandomInt(4);
                                                }

                                                var new_id = Convert.ToInt64(db.InsertWithIdentity(new Medico()
                                                {
                                                    dtStart = DateTime.Now,
                                                    fkUserAdd = 1,
                                                    fkEspecialidade = especTb.id,
                                                    stNome = nomeMedico,
                                                    stCnpj = stCnpj,
                                                    nuCodigo = codDisp,
                                                    nuTipo = stCnpj.Length > 11 ? 2 : 1,
                                                }));

                                                for (int t=1; t <=3; ++t)
                                                    db.Insert(new MedicoEmpresa
                                                    {
                                                        fkMedico = new_id,
                                                        fkEmpresa = t
                                                    });

                                                curMedico = db.Medico.Where(y => y.id == new_id).FirstOrDefault();
                                            }
                                        }
                                        else if (curMedico != null)
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
                                                db.Insert( new TUSS
                                                {
                                                    nuCodTUSS = Convert.ToInt64(cod_tuss),
                                                    stProcedimento = st_tuss
                                                });
                                            }

                                            var lstCurProc = db.MedicoEmpresaTuss.
                                                                Where(y => y.fkMedico == curMedico.id && 
                                                                           y.nuTUSS == Convert.ToInt64(cod_tuss)).
                                                                ToList();

                                            if (lstCurProc.Any())
                                            {
                                                // ataaliza
                                                foreach (var item in lstCurProc)
                                                {
                                                    var itemDb = db.MedicoEmpresaTuss.Where(y => y.id == item.id).FirstOrDefault();

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
                                                // cria
                                                for (int t = 1; t <= 3; t++)
                                                    db.Insert(new MedicoEmpresaTuss
                                                    {
                                                        fkEmpresa = t,
                                                        fkMedico = curMedico.id,
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

                            Console.WriteLine("");
                            Console.WriteLine("-----------------------------------------");
                            Console.WriteLine("Carga de base finalizada.");
                            Console.WriteLine("-----------------------------------------");
                            Console.WriteLine("");
                            Console.ReadLine();

                            break;
                        }
                }
            }
		}
	}
}
