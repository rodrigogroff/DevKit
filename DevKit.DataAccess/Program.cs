using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;

namespace GetStarted
{
	class Program
	{
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

                            if (!db.Person.Where (y=> y.fkEmpresa == 1).Any())
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

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Person()
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
                                                        
                            if (!db.Person.Where(y => y.fkEmpresa == 2).Any())
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

                                            var new_id = Convert.ToInt64(db.InsertWithIdentity(new Person()
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

                            if (!db.Medico.Any())
                            {
                                #region - carga de médicos - 

                                using (var sr = new StreamReader("c:\\carga\\medicos_1801.txt", Encoding.UTF8, false))
                                {
                                    Console.WriteLine("Carga de médicos");

                                    // pula a primeira linha
                                    sr.ReadLine();

                                    while (!sr.EndOfStream)
                                    {
                                        var line = sr.ReadLine().ToUpper();

                                        if (line == ";;;")
                                            continue;

                                        if (line != "")
                                        {
                                            var ar = line.Split(';');

                                            var nome = ar[0].Trim();
                                            var espec = ar[1].Trim();
                                            var homem = ar[2].Trim();
                                            var end = ar[3].Trim();
                                            var tels = ar[4].Replace("-", "").Replace(" ", "").Split(',');

                                            //CELSO JOSÉ BERTOLETI RACIC;CARDIOLOGISTA;H;Av. Emancipação, 1441, Centro;3158-1095

                                            if (!db.Especialidade.Any (y=> y.stNome == espec))
                                            {
                                                db.Insert(new Especialidade
                                                {
                                                    stNome = espec
                                                });
                                            }

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
                                                fkEspecialidade = db.Especialidade.FirstOrDefault(y=>y.stNome == espec).id,
                                                stNome = nome,
                                                tgMasculino = homem == "H" ? true : false,
                                                nuCodigo = codDisp,
                                            }));

                                            db.Insert(new MedicoAddress
                                            {
                                                stRua = end,
                                                fkMedico = new_id
                                            });

                                            foreach (var tel in tels)
                                            {
                                                db.Insert(new MedicoEmail
                                                {
                                                    stEmail = tel,
                                                    fkMedico = new_id
                                                });
                                            }

                                            db.Insert(new MedicoEmpresa
                                            {
                                                fkMedico = new_id,
                                                fkEmpresa = 1
                                            });

                                            db.Insert(new MedicoEmpresa
                                            {
                                                fkMedico = new_id,
                                                fkEmpresa = 2
                                            });
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

                            break;
                        }
                }
            }
		}
	}
}
