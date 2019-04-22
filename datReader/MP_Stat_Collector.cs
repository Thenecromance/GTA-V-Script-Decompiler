using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datReader
{
    public class MP_Stat_Collector : FileCollection
    {
        HashSet<string> _MpStat = new HashSet<string>();

        /// <summary>
        /// Im too lazy to use Xml Parse... just do it by string
        /// </summary>
        /// <param name="Path"></param>
        public  void GetStat(string Path)
        {
            using (StreamReader reader = new StreamReader(Path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains("Name="))
                    {
                        line = line.Remove(line.IndexOf("Type="));
                        line = line.Substring(line.IndexOf("\"")+1);
                        line = line.Remove(line.LastIndexOf("\""));
                        if (!_MpStat.Contains(line))
                            _MpStat.Add(line);
                    }
                }
            }
        }

        public HashSet<string> GetHashSet()
        {
            return _MpStat;
        }
    }
}
