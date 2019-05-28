using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DWWinService
{
    class LocalDownloader : IDownloader
    {

        public FileServer fsv { get; set; }

        public string[] filelist
        {
            get
            {
                if (fsv == null)
                {
                    return null;
                }
                return Directory.GetFiles(fsv.path).Select(a => a.Substring(a.LastIndexOf("\\")+1)).ToArray();
            }
        }

        public string[] getFilelist(FileServer f)
        {
            fsv = f;
            return filelist;
        }

        public bool exist(string filename)
        {
            if (fsv == null) {
                return false;
            }
            return File.Exists(filename);
        }

        public bool hasExist(FileServer f, string filename)
        {
            fsv = f;
            return exist(filename);
        }


        public string[] download(string[] filenames, string[] targetnames)
        {
            if (fsv == null) {
                return null;
            }
            for(int i=0;i<filenames.Length;i++){
                File.Copy(Path.Combine(fsv.path,filenames[i]),targetnames[i],true);
            }
            return targetnames;
        }

        public string[] download(FileServer f, string[] filenames, string[] targetnames)
        {
            fsv = f;
            return download(filenames,targetnames);
        }

        public bool delete(string[] filenames)
        {
            if (fsv == null) {
                return false;
            }
            foreach (var file in filenames) {
                File.Delete(file);
            }
            return true;
        }

        public bool delete(FileServer f, string[] filenames)
        {
            fsv = f;
            return delete(filenames);
        }
    }
}
