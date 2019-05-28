using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;

namespace DWMapService
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LOG.WriteLog("application start");
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GetDataClass.StartGetData();
        }
    }
    public class LOG
    {
        private static List<string> logs = new List<string>();
        private static bool doing = false;
        private static string logName
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (path.LastIndexOf("\\") < path.Length - 1)
                {
                    path += "\\";
                }
                path += "log\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DateTime dt = DateTime.Now;
                string fileName = "log" + dt.ToString("yyyyMMdd") + ".log";
                return path + fileName;
            }
        }
        public static void WriteLog(string info)
        {
            DateTime dt = DateTime.Now;
            lock (logs)
            {
                logs.Add("[" + dt.ToString("HH:mm:ss") + "] " + info.Replace("\r\n", "\r\n           "));
            }
            if (!doing)
            {
                doing = true;
                writeLog();
            }
        }
        private static void writeLog()
        {

            bool had = logs.Count > 0;
            if (had)
            {
                try
                {
                    FileStream fs = null;
                    if (File.Exists(logName))
                    {
                        fs = new FileStream(logName, FileMode.Append);
                    }
                    else
                    {
                        fs = new FileStream(logName, FileMode.Create);
                    }
                    StreamWriter sw = new StreamWriter(fs);
                    while (had)
                    {
                        sw.WriteLine(logs[0]);
                        logs.RemoveAt(0);
                        had = logs.Count > 0;
                    }
                    sw.Close();
                    sw.Dispose();
                    fs.Close();
                    fs.Dispose();
                }
                catch
                {
                }
            }
            doing = false;
        }
    }
}