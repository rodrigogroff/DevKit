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
                int patch = 1;

                switch (patch)
                {
                    case 1:

                        Console.WriteLine("Ajustando cartões");

                        var lst = db.T_Cartao.Where(y => y.st_empresa == "001201" || 
                                                         y.st_empresa == "001202").ToList();

                        foreach (var item in lst)
                        {
                            Console.WriteLine("Ajustando cartão " + item.st_empresa + " / " + item.st_matricula);
                            Console.WriteLine(".. De " + item.vr_limiteMensal );

                            var vlrlimM = item.vr_limiteMensal;
                            var vlrlimT = item.vr_limiteTotal;

                            vlrlimM += item.vr_limiteMensal * 207 / 10000;
                            vlrlimT += item.vr_limiteTotal * 207 / 10000;

                            item.vr_limiteMensal = vlrlimM;
                            item.vr_limiteTotal = vlrlimT;

                            Console.WriteLine(".. Para " + item.vr_limiteMensal);


                            db.Update(item);
                        }

                        Console.WriteLine("Ajustando cartões FIM");

                        break;
                }
            }
        }
    }
}
