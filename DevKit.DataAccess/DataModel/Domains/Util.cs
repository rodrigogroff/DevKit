using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataModel
{
    public class Util
    {
        private string __nomeFile;

        private StreamWriter __sw;

        public string SetupFile()
        {
            string dir = "c:\\saude_logs";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            DateTime dt = DateTime.Now;

            dir += "\\" + dt.Year;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            dir += "\\" + dt.Month;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            dir += "\\" + dt.Day;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            do
            {
                __nomeFile = dir + "\\" + GetRandomString(20) + ".txt";
            }
            while (File.Exists(__nomeFile));

            __sw = new StreamWriter(__nomeFile, false, Encoding.UTF8);

            Registry(" ---- File Created ---- ");

            return dir;
        }

        public void Registry(string text)
        {
            __sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffff") + " " + text);
            __sw.Flush();
        }

        public bool error = false;

        public void ErrorRegistry(string text)
        {
            error = true;
            __sw.WriteLine("-------------------------------------------------------");
            __sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffff") + " " + text);
            __sw.WriteLine("-------------------------------------------------------");
            __sw.Flush();
        }

        public void CloseFile()
        {
            __sw.Close();

            if (error)
                File.Move(__nomeFile, __nomeFile.Replace(".txt", "_falha.txt"));
        }

        private string GetRandomString(int length)
        {
            var rand = new Random();
            var ret = "";

            for (int i = 0; i < length; i++)
                ret += rand.Next(0, 9);

            return ret;
        }

        // calculo codigo de acesso para cartoes convenio
        public string calculaCodigoAcesso(string empresa, string matricula, string titularidade, string via, string cpf)
        {
            string chave = matricula + empresa + titularidade.PadLeft(2, '0') + via + cpf.PadRight(14, ' ');
            int temp = 0;
            for (int n = 0; n < chave.Length; n++)
            {
                string s = chave.Substring(n, 1);
                char c = s[0]; // First character in s
                int i = c; // ascii code
                temp += i;
            }
            string calculado = temp.ToString("0000");
            temp += int.Parse(calculado[3].ToString()) * 1000;
            temp += int.Parse(calculado[2].ToString());
            if (temp > 9999) temp -= 2000;
            calculado = temp.ToString("0000");
            calculado = calculado.Substring(2, 1) +
                        calculado.Substring(0, 1) +
                        calculado.Substring(3, 1) +
                        calculado.Substring(1, 1);
            return calculado;
        }
    }
}
