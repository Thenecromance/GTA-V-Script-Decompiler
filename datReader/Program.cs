using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace datReader
{
    class Program
    {
        public static void GenerateSTAT()
        {
            MP_Stat_Collector mP_Stat = new MP_Stat_Collector();
            mP_Stat.GetStat(@"D:\GTA DATA\ytyp\mpstat\mpstatssetup.xml");
            HashSet<string> h = mP_Stat.GetHashSet();
            Console.WriteLine("//MP STAT Count:" + h.Count);

            foreach (string str in h)
            {

                Console.WriteLine((int)MyHash.Hash(str) + ":" + str.ToLowerInvariant());
            }
        }

        public static bool Deflate(Stream stream)
        {
            DeflateStream Deflate = new DeflateStream(stream, CompressionMode.Compress);
            return false;
        }

        public static void Collect()
        {
            HashSet<string> MyHashset = new HashSet<string>();
            foreach (FileInfo info in FileCollection.GetTargetFileInfo(@"D:\GTA DATA\ytyp\temp"))
            {
                using (StreamReader reader = new StreamReader(info.FullName))
                {
                    while (!reader.EndOfStream)
                    {
                        string str = reader.ReadLine();
                        if (str.Contains("<name>") && str.Contains("</name>"))
                        {
                            str = str.Remove(str.IndexOf("</name>"));
                            str = str.Substring(str.IndexOf("<name>") + 6);
                            if (!MyHashset.Contains(str))
                            {
                                MyHashset.Add(str);
                            }
                        }
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter(@"D:\temp.txt2"))
            {
                foreach (string str in MyHashset)
                {
                    string output = ((int)MyHash.Hash(str)).ToString() + ":" + str;
                    writer.WriteLine(output);
                }
            }
        }
        public static int Main()
        {
           FileStream fs= File.OpenRead(@"D:\Entities.dat.txt");
            FileInfo originInfo = new FileInfo(@"D:\Entities.dat.txt");
            using (FileStream compressedFileStream = File.Create(@"D:\Entities.dat"))
            {
                using (DeflateStream compressionStream = new DeflateStream(compressedFileStream, CompressionMode.Compress))
                {
                    fs.CopyTo(compressionStream);
                }
                FileInfo info = new FileInfo(@"D:\Entities.dat");
                Console.WriteLine("Compressed {0} from {1} to {2} bytes.", originInfo.Name, originInfo.Length, info.Length);
            }
             
            Console.ReadKey();
            return 0;
        }
    }
}
