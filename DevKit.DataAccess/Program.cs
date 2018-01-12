using DataModel;
using LinqToDB;
using System;
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
                int opt = 0;

                // carga de tabelas txt
                switch (opt)
                {
                    case 1:
                        {
                            Console.WriteLine("Carga de base.");

                            // carga de estado e cidade
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
                                    }
                                }
                            }

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

                            Console.WriteLine("Carga de base executada com sucesso.");

                            break;
                        }
                }
            }
		}
	}
}
