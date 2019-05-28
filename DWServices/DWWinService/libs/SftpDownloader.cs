using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.Collections;
using System.IO;

namespace DWWinService
{
    class SftpDownloader : IDownloader
    {
        public FileServer fsv { get; set; }
        private SftpClient sftp;

        private void Connect()
        {
            sftp = new SftpClient(fsv.ip, fsv.port, fsv.uid, fsv.pwd);
            sftp.Connect();
        }

        public string[] filelist
        {
            get
            {
                Connect();
                var files = sftp.ListDirectory(fsv.path);
                sftp.Disconnect();
                var list = files.Where(a => !a.IsDirectory).Select(a => a.Name).ToArray();
                return list;
            }
        }

        public string[] getFilelist(FileServer f)
        {
            fsv = f;
            return filelist;
        }

        public string[] download(string[] filenames, string[] targetnames)
        {
            Connect();
            for (int i=0;i<filenames.Length;i++)
            {
                var byt = sftp.ReadAllBytes(filenames[i]);
                File.WriteAllBytes(targetnames[i], byt);
            }
            sftp.Disconnect();
            return targetnames;
        }

        public string[] download(FileServer f, string[] filenames, string[] targetnames)
        {
            fsv = f;
            return download(filenames,targetnames);
        }

        public bool delete(string[] filenames)
        {
            throw new NotImplementedException();
        }

        public bool delete(FileServer f, string[] filenames)
        {
            throw new NotImplementedException();
        }

        public bool exist(string filename)
        {
            throw new NotImplementedException();
        }

        public bool hasExist(FileServer f, string filename)
        {
            throw new NotImplementedException();
        }
    }
}
