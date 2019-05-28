using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Configuration;

namespace AlarmService
{
    class ftpHelper
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
            //ftp.UsePassive = false;
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
            for (int i = 0; i < filenames.Length; i++)
            {
                string fileName = Path.GetFileName(filenames[i]);
                string newFile = targetnames[i];
                string url = "ftp://" + fsv.ip + ":" + fsv.port + fsv.path + fileName;
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
    }
    class FileServer
    {
        public FileServer()
        {
            _stype = "ftp";
            _ip = ConfigurationManager.AppSettings["mrip"];
            _port = O2.O2I(ConfigurationManager.AppSettings["mrport"]);
            _uid = ConfigurationManager.AppSettings["mruid"];
            _pwd = ConfigurationManager.AppSettings["mrpwd"];
            _path = ConfigurationManager.AppSettings["mrpath"];
        }
        #region 私有
        string _stype;
        string _ip;
        int _port;
        string _uid;
        string _pwd;
        string _path;
        #endregion

        #region 属性
        public string stype { get { return _stype; } }
        public string ip { get { return _ip; } }
        public int port { get { return _port; } }
        public string uid { get { return _uid; } }
        public string pwd { get { return _pwd; } }
        public string path { get { return _path; } }
        #endregion
    }
}
