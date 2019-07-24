using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace Descompacta
{
    class Program
    {
        static void Main(string[] args)
        {
            string zipPath = @"c:\inetpub\devkit\versao.zip";
            string extractPath = @"c:\inetpub\devkit";

            while (true)
            {
                Console.WriteLine(">> " + DateTime.Now.ToString());

                if (File.Exists(zipPath))
                {
                    Console.WriteLine(">> versão nova!");

                    try
                    {
                        using (var ZipArchive = ZipFile.OpenRead(zipPath))
                        {
                            foreach (ZipArchiveEntry entry in ZipArchive.Entries)
                            {
                                var entryFullname = Path.Combine(extractPath, entry.FullName);
                                var entryPath = Path.GetDirectoryName(entryFullname);

                                if (!Directory.Exists(entryPath))
                                    Directory.CreateDirectory(entryPath);

                                var entryFn = Path.GetFileName(entryFullname);

                                if (!string.IsNullOrEmpty(entryFn))

                                {
                                    if (File.Exists(entryFullname))
                                        File.Delete(entryFullname);

                                    entry.ExtractToFile(entryFullname, true);
                                }                                    
                            }
                        }

                        Console.WriteLine(">> versão nova pronta!");

                        Thread.Sleep(500);

                        File.Delete(zipPath);
                        
                        Console.WriteLine(">> pronto!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(">> erro! " + ex.ToString());

                     //   using (var sw = new StreamWriter( Directory.GetCurrentDirectory() + "\\erro" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "log.txt", false, Encoding.Default))
                       // {
                         //   sw.WriteLine(ex.ToString());
                        //}
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}
