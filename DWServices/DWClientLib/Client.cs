using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;

namespace DWClientLib
{
    public delegate void OnGotInfoHandler(string info);
    public delegate void OnErrorHandler(int count);
    public class Client
    {
        public static event OnGotInfoHandler OnGotInfo;
        public static int MaxErrorCount { get; set; }
        public static event OnErrorHandler OnError;
        static bool start = false;
        public static string GetAnalyzersStatus() {
            var res = "";
            try {
                IPAddress IP; int port;
                getIPPort(out IP,out port);

                EndPoint svr = new IPEndPoint(IP, port);
                Socket client = new Socket(SocketType.Dgram, ProtocolType.Udp);
                client.SendTo(Encoding.GetEncoding("gb2312").GetBytes("getstatus"), svr);
                byte[] data = new byte[1024 * 100];
                client.ReceiveTimeout = 10000;
                var len = client.ReceiveFrom(data, ref svr);
                if (len <= 1) {
                    return res;
                }
                res = Encoding.GetEncoding("gb2312").GetString(data, 0, len);
            }
            catch {
                res = "";
            }
            return res;
        }

        static NetworkStream stream = null;
        static TcpClient client = null;
        static int localCount=0;
        public static bool Start(out string sucinfo) {
            sucinfo = "";
            if (start) return true;
            start = true;
            localCount = 0;
            try {
                IPAddress IP; int port;
                getIPPort(out IP, out port);
                client = new TcpClient();
                client.Connect(IP, port);
                stream = client.GetStream();
                stream.Write(new byte[] { 1 }, 0, 1);
                byte[] data=new byte[1024*100];
                int len = stream.Read(data, 0, 1024 * 100);
                if (len <= 0 || data[0] == 0) {
                    throw new Exception("错误");
                }
                sucinfo = Encoding.GetEncoding("gb2312").GetString(data,1,len-1);
                Thread th = new Thread(new ThreadStart(getinfo));
                th.Start();
            }
            catch {
                try {
                    client.Close();
                    stream.Dispose();
                }
                catch { }
                start = false;
                client = null;
                stream = null;
                OnGotInfo = null;
            }
            return start;
        }
        public static void Stop(){
            try
            {
                client.Close();
                stream.Dispose();
            }
            catch { }
            start = false;
            client = null;
            stream = null;
            OnGotInfo = null;
        }
        private static void getinfo() {
            while (start)
            {
                try
                {
                    if (stream != null)
                    {
                        byte[] data = new byte[255];
                        int len = stream.Read(data, 0, 255);
                        if (OnGotInfo != null) {
                            var gotstr = Encoding.GetEncoding("gb2312").GetString(data, 0, len);
                            var gotlist = gotstr.Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries);
                            foreach (var str in gotlist) {
                                try
                                {
                                    OnGotInfo(str);
                                }
                                catch { }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                    localCount = 0;
                }
                catch
                {
                    localCount++;
                    if (MaxErrorCount > 0 && localCount == MaxErrorCount && OnError != null) {
                        try {
                            OnError(localCount);
                        }
                        catch { }
                        localCount = 0;
                    }
                    break;
                }
            }
        }

        private static void getIPPort(out IPAddress IP, out int port) {
            var IPc = System.Configuration.ConfigurationManager.AppSettings["IP"];
            var portc = ConfigurationManager.AppSettings["port"];
            if (!IPAddress.TryParse(IPc, out IP))
            {
                IP = IPAddress.Parse("127.0.0.1");
            }
            if (!int.TryParse(portc, out port))
            {
                port = 1213;
            }
        }
    }
}
