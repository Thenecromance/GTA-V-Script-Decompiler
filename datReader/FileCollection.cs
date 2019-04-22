using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datReader
{
    public class FileCollection
    {
        static public FileInfo[] GetTargetFileInfo(string Path)
        {
            DirectoryInfo info = new DirectoryInfo(Path);
            return info.GetFiles();
        }
    }
}
