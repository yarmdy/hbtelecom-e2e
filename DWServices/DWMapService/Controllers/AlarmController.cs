using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Sockets;
namespace DWMapService.Controllers
{
    public class AlarmController : Controller
    {
        //
        // GET: /Alarm/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
        {
            try
            {
                if (REQUEST == "GetCapabilities")
                {
                    var resxml = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"GetCapabilities\GetCapabilities.txt"));
                    var bres = System.Text.Encoding.GetEncoding("utf-8").GetBytes(resxml.ToString());
                    Response.AddHeader("Access-Control-Allow-Origin", "*");
                    return File(bres, "text/xml");
                }
                else if (REQUEST == "GetMap")
                {
                    int crs = CRS == "EPSG:900913" ? 1 : 2;
                    string[] bboxs = BBOX.Split(',');
                    byte[] byteArr = GetData.get(1, WIDTH.Value, HEIGHT.Value, double.Parse(bboxs[0]), double.Parse(bboxs[1]), double.Parse(bboxs[2]), double.Parse(bboxs[3]), crs);
                    //System.IO.File.WriteAllBytes(@"C:\Users\guoco\Desktop\1.png", byteArr);
                    return File(byteArr, "application/x-png");
                }
                return null;
            }
            catch (Exception ex)
            {
                LOG.WriteLog(ex.Message + ":" + ex.StackTrace);
                return null;
            }
        }

        public ActionResult GetInfo(double minLat, double minLon, double maxLat, double maxLon)
        {
            byte[] arr = GetData.get(2, 0, 0, minLat, minLon, maxLat, maxLon, 2);
            return Content(System.Text.Encoding.GetEncoding("utf-8").GetString(arr));
        }

        Dictionary<int, string> cityNames = new Dictionary<int, string>() { { 0, "石家庄" }, { 1, "廊坊" }, { 2, "保定" }, { 3, "邯郸" }, { 4, "沧州" }, { 5, "衡水" }, { 6, "邢台" }, { 7, "唐山" }, { 8, "秦皇岛" }, { 9, "张家口" }, { 10, "承德" }, { 11, "雄安" },{ 12, "全省"} };

        public ActionResult GetCityData(int no)
        {
            byte[] arr = GetData.get(3, 0, 0, 0, 0, 0, 0, no);
            return File(arr, "application/octet-stream", cityNames[no]+".csv");
        }

        public ActionResult Query(int enbid, int cellid)
        {
            byte[] arr = GetData.get(4, enbid, cellid, 0, 0, 0, 0, 0);
            return Content(System.Text.Encoding.GetEncoding("utf-8").GetString(arr));
        }
    }
    
    public static class GetData
    {
        static int bufferlen = 1024 * 100;
        static void Receive(Socket skt,byte[] data){
            var len = data.Length;
            int offset = 0;
            while (offset < len) {
                var jielen = Math.Min(len - offset, bufferlen);
                var slen = skt.Receive(data, offset, jielen, SocketFlags.None);
                offset += slen;
            }
        }
        public static byte[] get(int act, int width, int height, double bboxx, double bboxy, double bboxx2, double bboxy2, int crs)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint ipep = new IPEndPoint(IPAddress.Parse(System.Configuration.ConfigurationManager.AppSettings["socketip"]), 3390);
            byte[] bytes = new byte[1024];
            MemoryStream stream = new MemoryStream(bytes);
            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(act);
            bw.Write(width);
            bw.Write(height);
            bw.Write(bboxx);
            bw.Write(bboxy);
            bw.Write(bboxx2);
            bw.Write(bboxy2);
            bw.Write(crs);
            bw.Flush();
            bytes = stream.ToArray();
            bw.Close();
            stream.Close();
            socket.Connect(ipep);
            socket.Send(bytes);
            byte[] bytes2 = new byte[1024 * 1024];
            socket.ReceiveTimeout = 5000;
            if(act == 3)
            {
                socket.ReceiveTimeout = 10000;
            }
            int i = 0;
            int len = 0;
            try
            {
                //byte[] length = new byte[4];
                //socket.Receive(length);
                //socket.Send(new byte[] { 0});
                //len = BitConverter.ToInt32(length, 0);
                //byte[] arr = new byte[len];
                
                //while(i<=len)
                //{
                //    byte[] ar = new byte[Math.Min(len-i,1024*10)];
                //    int relen = socket.Receive(ar, ar.Length, SocketFlags.None);
                //    int shengyu = ar.Length - relen;
                //    if (relen != ar.Length) {
                //        int o = 0;
                //    }
                //    if (len - i > 1024 * 10)
                //    {
                //        socket.Send(new byte[] { 0 });
                //    }
                //    ar.CopyTo(arr, i);
                //    i += 1024*10;
                //}
                //bytes2 = arr;
                //int b = 1;

                byte[] length = new byte[4];
                Receive(socket,length);
                len = BitConverter.ToInt32(length, 0);
                if (len <= 0) {
                    socket.Close();
                    return null;
                }
                byte[] arr=new byte[len];
                Receive(socket,arr);
                bytes2 = arr;

            }catch(Exception e)
            {
                int a = 0;
            }
            socket.Close();
            return bytes2;
        }
    }


    public class req
    {
        public int ACT;
        public int WIDTH; public int HEIGHT; public double BBOXx; public double BBOXy; public double BBOXx2; public double BBOXy2; public int CRS;
        public req()
        {

        }
        public req(int act, int width, int height, double bboxx, double bboxy, double bboxx2, double bboxy2, int crs)
        {
            ACT = act;
            WIDTH = width;
            HEIGHT = height;
            BBOXx = bboxx;
            BBOXy = bboxy;
            BBOXx2 = bboxx2;
            BBOXy2 = bboxy2;
            CRS = crs;
        }

    }


}
