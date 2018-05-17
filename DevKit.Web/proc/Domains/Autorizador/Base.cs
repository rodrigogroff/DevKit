using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace DevKit.Web.Controllers
{
    public class BaseVenda
    {
        public string nomeFile;
        public StreamWriter sw;

        public string SetupFile()
        {
            string dir = "c:\\cnet_logs";

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
                nomeFile = dir + "\\" + GetRandomString(20) + ".txt";
            }
            while (File.Exists(nomeFile));

            sw = new StreamWriter(nomeFile, false, Encoding.ASCII);

            Registry(" ---- File Created ---- ");

            return dir;
        }        

        public void Registry (string text)
        {
            sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffff") + " " + text);
            sw.Flush();
        }

        public void CloseFile()
        {
            sw.Close();
        }

        private string GetRandomString(int length)
        {
            Random rand = new Random();

            var ret = "";

            for (int i = 0; i < length; i++)
                ret += rand.Next(0, 9);

            return ret;
        }
    }
}