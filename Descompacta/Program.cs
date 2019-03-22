using System;
using System.IO;
using System.IO.Compression;
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
                                entry.ExtractToFile(entryFullname, true);
                        }
                    }

                    File.Delete(zipPath);

                    Console.WriteLine(">> versão nova pronta!");
                }

                Thread.Sleep(5000);
            }
        }
    }
}
