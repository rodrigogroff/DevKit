using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

            sw = new StreamWriter(nomeFile, false, Encoding.UTF8);

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
            var rand = new Random();
            var ret = "";

            for (int i = 0; i < length; i++)
                ret += rand.Next(0, 9);

            return ret;
        }

        public string DESCript(string dados, string chave)
        {
            dados = dados.PadLeft(8, '*');

            byte[] key = System.Text.Encoding.ASCII.GetBytes(chave);//{1,2,3,4,5,6,7,8};
            byte[] data = System.Text.Encoding.ASCII.GetBytes(dados);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            des.Key = key;
            des.Mode = CipherMode.ECB;

            ICryptoTransform DESt = des.CreateEncryptor();
            DESt.TransformBlock(data, 0, 8, data, 0);

            string retorno = "";
            for (int n = 0; n < 8; n++)
            {
                retorno += String.Format("{0:X2}", data[n]);
            }

            return retorno;
        }

        public static string DESdeCript(string dados, string chave)
        {
            byte[] key = System.Text.Encoding.ASCII.GetBytes(chave);//{1,2,3,4,5,6,7,8};
            byte[] data = new byte[8];

            for (int n = 0; n < dados.Length / 2; n++)
            {
                data[n] = (byte)Convert.ToInt32(dados.Substring(n * 2, 2), 16);
            }

            DES des = new DESCryptoServiceProvider();
            des.Key = key;
            des.Mode = CipherMode.ECB;
            ICryptoTransform crypto = des.CreateDecryptor();
            MemoryStream cipherStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(cipherStream, crypto, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            crypto.TransformBlock(data, 0, 8, data, 0);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            string retorno = enc.GetString(data);

            return retorno;
        }
    }
}