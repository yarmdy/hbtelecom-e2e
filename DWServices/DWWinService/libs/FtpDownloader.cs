using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace DWWinService
{
    class FtpDownloader : IDownloader
    {
        public FileServer fsv { get; set; }

        FtpWebRequest ftp;

        public string[] filelist
        {
            get
            {
                if (fsv == null)
                {
                    return null;
                }
                ConnectServer("ftp://" + fsv.ip + ":" + fsv.port + fsv.path);
                StringBuilder result = new StringBuilder();
                try
                {
                    ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                    WebResponse response = ftp.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");
                        line = reader.ReadLine();
                    }
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    reader.Close();
                    response.Close();
                    return result.ToString().Split('\n');
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        //得到ftp服务器连接
        private void ConnectServer(string url)
        {
            ftp = (FtpWebRequest)FtpWebRequest.Create(url);
            ftp.UseBinary = true;
            ftp.Credentials = new NetworkCredential(fsv.uid, fsv.pwd);
        }

        public string[] getFilelist(FileServer f)
        {
            fsv = f;
            return filelist;
        }

        public string[] download(string[] filenames, string[] targetnames)
        {
            Stream ftpStream = null;
            FileStream outputStream = null;
            FtpWebResponse response = null;
            for (int i=0;i< filenames.Length;i++)
            {
                string fileName = Path.GetFileName(filenames[i]);
                string newFile = targetnames[i];
                string url = "ftp://" + fsv.ip +":"+fsv.port+ fsv.path + fileName;
                ConnectServer(url);
                response = (FtpWebResponse)ftp.GetResponse();
                ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                outputStream = new FileStream(newFile, FileMode.Create);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
            }
            ftpStream.Close();
            outputStream.Close();
            response.Close();
            return targetnames;
        }

        public string[] download(FileServer f, string[] filenames, string[] targetnames)
        {
            fsv = f;
            return download(filenames, targetnames);
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
