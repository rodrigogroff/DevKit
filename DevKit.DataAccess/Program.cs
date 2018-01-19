using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;

namespace GetStarted
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var db = new DevKitDB())
			{
                int opt = 1;

                // carga de tabelas txt
                switch (opt)
                {
                    case 1:
                        {
                            Console.WriteLine("Carga de base.");

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
                            }
                                                        
                            if (!db.Person.Where(y => y.fkEmpresa == 2).Any())
                            {
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
                            }

                            Console.WriteLine("Carga de base executada com sucesso.");

                            break;
                        }
                }
            }
		}
	}
}
