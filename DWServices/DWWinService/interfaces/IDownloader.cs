using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    interface IDownloader
    {
        FileServer fsv { get; set; }
        string[] filelist { get; }
        string[] getFilelist(FileServer f);
        bool exist(string filename);
        bool hasExist(FileServer f,string filename);
        string[] download(string[] filenames,string[] targetnames);
        string[] download(FileServer f,string[] filenames,string[] targetnames);
        bool delete(string[] filenames);
        bool delete(FileServer f,string[] filenames);
    }
}
