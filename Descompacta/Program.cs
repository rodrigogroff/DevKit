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
            while (true)
            {
                Console.WriteLine(">> " + DateTime.Now.ToString());

                {
                    string zipVersaoAtualPath = @"c:\inetpub\devkit\versao.zip";
                    string extractVersaoAtualPath = @"c:\inetpub\devkit";

                    if (File.Exists(zipVersaoAtualPath))
                    {
                        #region - code zipVersaoAtualPath - 

                        Console.WriteLine(">> versão nova!");

                        try
                        {
                            using (var ZipArchive = ZipFile.OpenRead(zipVersaoAtualPath))
                            {
                                foreach (ZipArchiveEntry entry in ZipArchive.Entries)
                                {
                                    var entryFullname = Path.Combine(extractVersaoAtualPath, entry.FullName);
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

                            File.Delete(zipVersaoAtualPath);

                            Console.WriteLine(">> pronto!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(">> erro! " + ex.ToString());
                        }

                        #endregion
                    }
                }

                {
                    string zipVersaoAtualFrontPath = @"c:\inetpub\devkit\versao_front.zip";
                    string extractVersaoAtualFrontPath = @"c:\inetpub\portal2\front";

                    if (File.Exists(zipVersaoAtualFrontPath))
                    {
                        #region - code zipVersaoAtualFrontPath - 

                        Console.WriteLine(">> versão nova!");

                        try
                        {
                            using (var ZipArchive = ZipFile.OpenRead(zipVersaoAtualFrontPath))
                            {
                                foreach (ZipArchiveEntry entry in ZipArchive.Entries)
                                {
                                    var entryFullname = Path.Combine(extractVersaoAtualFrontPath, entry.FullName);
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

                            Console.WriteLine(">> versão nova pronta (front)!");

                            Thread.Sleep(500);

                            File.Delete(zipVersaoAtualFrontPath);

                            Console.WriteLine(">> pronto!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(">> erro! " + ex.ToString());                            
                        }

                        #endregion
                    }
                }

                {
                    string zipVersaoAtualMasterPath = @"c:\inetpub\devkit\versao_master.zip";
                    string extractVersaoAtualMasterPath = @"c:\inetpub\portal2\master";

                    if (File.Exists(zipVersaoAtualMasterPath))
                    {
                        #region - code zipVersaoAtualFrontPath - 

                        Console.WriteLine(">> versão nova!");

                        try
                        {
                            using (var ZipArchive = ZipFile.OpenRead(zipVersaoAtualMasterPath))
                            {
                                foreach (ZipArchiveEntry entry in ZipArchive.Entries)
                                {
                                    var entryFullname = Path.Combine(extractVersaoAtualMasterPath, entry.FullName);
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

                            Console.WriteLine(">> versão nova pronta (master)!");

                            Thread.Sleep(500);

                            File.Delete(zipVersaoAtualMasterPath);

                            Console.WriteLine(">> pronto!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(">> erro! " + ex.ToString());
                        }

                        #endregion
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}
