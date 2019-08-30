using System;
using System.IO;
using System.Threading;

namespace ServerIsoV2
{
    public partial class IsoCommand
    {
        Random random = new Random();

        int RandomNumber(int min, int max)
        {
            //Thread.Sleep(1);
            return random.Next(min, max);
        }

        public string GetRandomString(int size)
        {
            Thread.Sleep(1);

            var ret = "";

            for (int t = 0; t < size; ++t)
                ret += RandomNumber(0, 9).ToString();

            return ret;
        }

        bool firstLog = true;
        StreamWriter sw = null, swFalha = null;
        string idLogFile, strLogFile, dir;

        public void Log(ISO8583 isoRegistro)
        {
            // somente no disco!

            var dados = " ISO8583-DETALHES DO REGISTRO \r\n         ======================================================== \r\n         Registro Iso : codigo       =" + isoRegistro.codigo + "\r\n         Bits preenchidos :          =" + isoRegistro.relacaoBits + "\r\n         bit( 3  ) - Codigo Proc.    =" + isoRegistro.codProcessamento + "\r\n         bit( 4  ) - valor           =" + isoRegistro.valor + "\r\n         bit( 7  ) - datahora        =" + isoRegistro.datetime + "\r\n         bit( 11 ) - NSU Origem      =" + isoRegistro.nsuOrigem + "\r\n         bit( 13 ) - data            =" + isoRegistro.Date + "\r\n         bit( 22 ) - modo captura    =" + isoRegistro.bit22 + "\r\n         bit( 35 ) - trilha          =" + isoRegistro.trilha2 + "\r\n         bit( 37 ) - nsu alternativo =" + isoRegistro.nsu + "\r\n         bit( 39 ) - codResposta     =" + isoRegistro.codResposta + "\r\n         bit( 41 ) - terminal        =" + isoRegistro.terminal + "\r\n         bit( 42 ) - codigoLoja      =" + isoRegistro.codLoja + "\r\n         bit( 49 ) - codigo moeda    =" + isoRegistro.bit49 + "\r\n         bit( 52 ) - Senha           =" + isoRegistro.senha + "\r\n         bit( 62 ) - Dados transacao =" + isoRegistro.bit62 + "\r\n         bit( 63 ) - Dados transacao =" + isoRegistro.bit63 + "\r\n         bit( 64 ) - Dados transacao =" + isoRegistro.bit64 + "\r\n         bit( 90 ) - dados original  =" + isoRegistro.bit90 + "\r\n         bit( 125 )- NSU original    =" + isoRegistro.bit125 + "\r\n         bit( 127 )- NSU             =" + isoRegistro.bit127 + "\r\n         ======================================================== \r\n";
            var st = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffff") + " {" + dados + "}";

            sw.WriteLine(st);
        }

        public void Log(string dados)
        {
            var st = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffff") + " {" + dados + "}";
            
            if (firstLog)
            {
                firstLog = false;

                string mes = DateTime.Now.Month.ToString().PadLeft(2, '0');
                string ano = DateTime.Now.Year.ToString().PadLeft(2, '0');
                string dia = DateTime.Now.Day.ToString().PadLeft(2, '0');

                dir = "logs";

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                dir += "\\" + ano;

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                dir += "\\" + mes;

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                dir += "\\" + dia;

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                idLogFile = "logFile_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Guid.NewGuid().ToString() + ".txt";

                strLogFile = dir + "\\" + idLogFile;

                sw = new StreamWriter(strLogFile, false)
                {
                    AutoFlush = true
                };

                Log("==========================================");
                Log("CNET ISO vr 2.00");
                Log("==========================================");
            }

            sw.WriteLine(st);
            Console.WriteLine(st);
        }

        int numFalha = 1;

        public void LogFalha(string dados)
        {
            numFalha++;
            firstLog = true;

            Log(dados);

            var st = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffff") + " {" + dados + "}";

            string mes = DateTime.Now.Month.ToString().PadLeft(2, '0');
            string ano = DateTime.Now.Year.ToString().PadLeft(2, '0');
            string dia = DateTime.Now.Day.ToString().PadLeft(2, '0');

            string dir = "logs";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            dir += "\\" + ano;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            dir += "\\" + mes;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            dir += "\\" + dia;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string idLogFile = "logFile_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Guid.NewGuid().ToString() + ".txt";

            swFalha = new StreamWriter(dir + "\\FALHA" + numFalha + idLogFile, false)
            {
                AutoFlush = true
            };

            swFalha.WriteLine(st);
            Console.WriteLine(st);
        }
    }
}
