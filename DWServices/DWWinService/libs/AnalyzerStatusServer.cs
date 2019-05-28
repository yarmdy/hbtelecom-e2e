using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DWWinService
{
    delegate string SendStrHandler();
    class AnalyzerStatusServer
    {
        AutoResetEvent are = new AutoResetEvent(true);
        public event SendStrHandler SendStr;
        bool _start = false;
        Socket skt = null;
        List<Socket> clientList = new List<Socket>();
        Socket udp = null;
        
        public void Start() {
            _start = true;
            skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            skt.Bind(new IPEndPoint(IPAddress.Any, 1213));
            skt.Listen(0);
            
            Thread th_listener = new Thread(new ThreadStart(listener));
            th_listener.Start();

            udp = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
            udp.Bind(new IPEndPoint(IPAddress.Any, 1213));

            Thread th_udpReceive = new Thread(new ThreadStart(udpReceive));
            th_udpReceive.Start();
        }
        public void Stop() {
            _start = false;
        }
        private void listener() {
            while (_start) {
                try {
                    var client=skt.Accept();
                    byte[] data=new byte[1024];
                    var len=client.Receive(data);
                    if (false)
                    {
                        client.Send(new byte[] { 0 });
                        continue;
                    }
                    are.WaitOne();
                    clientList.Add(client);
                    are.Set();
                    var sucstr = "";
                    if (SendStr != null) {
                        sucstr = SendStr();
                    }
                    var sucdata = Encoding.GetEncoding("gb2312").GetBytes(sucstr);
                    var senddata=new byte[sucdata.Length+1];
                    senddata[0] = 1;
                    sucdata.CopyTo(senddata,1);
                    client.Send(senddata);
                }
                catch { }
            }
        }
        private void udpReceive() {
            while (_start)
            {
                try
                {
                    byte[] data = new byte[1024];
                    EndPoint client=new IPEndPoint(IPAddress.Any,0);
                    var len = udp.ReceiveFrom(data,ref client);
                    if (false || SendStr==null)
                    {
                        udp.SendTo(new byte[]{0},client);
                        continue;
                    }
                    var act = Encoding.GetEncoding("gb2312").GetString(data,0,len);
                    byte[] senddata=null;
                    switch (act) { 
                        case "getstatus":
                            string datastr = SendStr();
                            senddata = Encoding.GetEncoding("gb2312").GetBytes(datastr);
                            break;
                        default:
                            if (senddata == null) {
                                senddata = new byte[] { 0 };
                            }
                            break;
                    }
                    
                    udp.SendTo(senddata,client);
                }
                catch { }
            }
        }
        public void send(byte[] data) {
            are.WaitOne();
            var list = clientList.Select(a => a).ToList();
            are.Set();
            foreach (var client in list) {
                try
                {
                    client.Send(data);
                }
                catch
                {
                    are.WaitOne();
                    clientList.Remove(client);
                    are.Set();
                }
            }
        }
    }
}