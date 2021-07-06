using System;

namespace Calc
{
    class Program
    {
        static void Main(string[] args)
        {
            var vlrMaq = 250;
            var vlrCusto = 9900;

            Console.WriteLine("Investimento inicial? ");
            var vlrIni = Convert.ToInt32(Console.ReadLine());

            var vlrSaldo = 0;

            Console.WriteLine("Investimento mensal? ");

            var vlrInvest = Convert.ToInt32(Console.ReadLine());
            var rentalQtd = vlrIni > 0 ? (int) vlrIni / vlrCusto : 1;

            if (rentalQtd == 0)
                rentalQtd = 1;

            Console.WriteLine("Anos simulados? ");

            var vlrAnos = Convert.ToInt32(Console.ReadLine());

            for (int ano = 2022; ano < 2022 + vlrAnos; ano++)
                for (int mes = 1; mes <= 12; mes++)
                {
                    Console.WriteLine("Ano " + ano + " Mês " + mes );
                    Console.WriteLine("-------------------------------------");

                    Console.WriteLine("A> Saldo: " + vlrSaldo + " Maquinas " + rentalQtd + " Aluguel: " + vlrMaq * rentalQtd + " Invest:" + vlrInvest);

                    vlrSaldo += vlrMaq * rentalQtd + vlrInvest;

                    Console.WriteLine("B> Saldo: " + vlrSaldo );

                    if (vlrSaldo >= vlrCusto)
                    {
                        Console.WriteLine("C> Maquina nova! ");
                        rentalQtd++;
                        vlrSaldo -= vlrCusto;
                    }
                }

            Console.WriteLine("");
            Console.WriteLine("FINAL!");            
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Investimento inicial: " + vlrIni + " > " + vlrIni / vlrCusto + " maquinas iniciais");
            Console.WriteLine("Investimento mensal: " + vlrInvest);
            Console.WriteLine("Anos: " + vlrAnos);
            Console.WriteLine("");
            Console.WriteLine("Maquinas final: " + rentalQtd);
            Console.WriteLine("Aluguel: " + vlrMaq * rentalQtd);
            Console.WriteLine("Share: " + (vlrMaq * rentalQtd) / 2);
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}
