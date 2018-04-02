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
            using (var db = new AutorizadorCNDB())
            {
                Console.WriteLine("Patch?");

                var strPatch = Console.ReadLine().ToUpper();

                switch (strPatch)
                {
                    case "FIXABRIL9620":
                    case "":
                    {
                        var dtIni = new DateTime(2018, 3, 2);
                        var dtFim = new DateTime(2018, 3, 28).AddDays(1);

                        var lstEmpresas = "009620,009621,009622,009623,009624".Split (',');

                        var lst = ( from e in db.LOG_Fechamento
                                    join empDb in db.T_Empresa on e.fk_empresa equals (int) empDb.i_unique
                                    join parc in db.T_Parcelas on e.fk_parcela equals (int) parc.i_unique
                                    join ltr in db.LOG_Transacoes on parc.fk_log_transacoes equals (int) ltr.i_unique
                                    where ltr.dt_transacao > dtIni && ltr.dt_transacao < dtFim
                                    where lstEmpresas.Contains(empDb.st_empresa)
                                    where e.st_mes == "03" && e.st_ano == "2018"
                                    select e).                                        
                                    ToList();

                        foreach (var item in lst)
                        {
                            item.st_mes = "04";

                            db.Update(item);
                        }

                        break;
                    }
                }
            }
        }
    }
}
