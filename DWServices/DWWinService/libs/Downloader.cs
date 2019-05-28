using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    /// <summary>
    /// 下载器
    /// </summary>
    class Downloader
    {
        IDownloader downloader = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="f">服务器配置</param>
        public Downloader(FileServer f) {
            switch (f.stype) { 
                case "local":
                    downloader = new LocalDownloader();
                    downloader.fsv = f;
                    break;
                case "ftp":
                    downloader = new FtpDownloader();
                    downloader.fsv = f;
                    break;
                case "sftp":
                    downloader = new SftpDownloader();
                    downloader.fsv = f;
                    break;
            }
            if (downloader == null) {
                throw new Exception("下载器不存在");
            }
        }
        public string[] filelist { get { return downloader.filelist; } }
        public bool exist(string filename) {
            return downloader.exist(filename);
        }
        public string[] download(string[] filenames, string[] targetnames) {
            return downloader.download(filenames,targetnames);
        }
        public bool delete(string[] filenames) {
            return downloader.delete(filenames);
        }
    }
}
