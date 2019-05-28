using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    class ZIP
    {
        public static void unzip(string filename, string path)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("ZIPFileName");
            }
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("zipFileName");
            }
            if (Path.GetExtension(filename).ToUpper() != ".ZIP")
            {
                throw new ArgumentException("ZipFileName is not Zip ");
            }
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(filename, path, "");
        }
    }
}
