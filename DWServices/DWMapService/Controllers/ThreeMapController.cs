using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;

namespace DWMapService.Controllers
{
    public class ThreeMapController : Controller
    {
        //
        // GET: /ThreeMap/

        public ActionResult ThreeMap(string token)
        {
#if !CESHI
            var now = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    DESCryptoServiceProvider dec = new DESCryptoServiceProvider();
                    var keyb = System.Text.Encoding.GetEncoding("utf-8").GetBytes("xyhsygck");
                    var token64 = System.Uri.UnescapeDataString(token);
                    var infob = Convert.FromBase64String(token64);
                    var stream = new System.IO.MemoryStream();
                    var cstream = new CryptoStream(stream, dec.CreateDecryptor(keyb, keyb), CryptoStreamMode.Write);
                    cstream.Write(infob, 0, infob.Length);
                    cstream.FlushFinalBlock();
                    var mima = System.Text.Encoding.GetEncoding("utf-8").GetString(stream.ToArray());
                    var uid = mima.Substring(0, 8);
                    var tick = mima.Substring(8);
                    var ticktime = new DateTime(long.Parse(tick));
                    var span = now - ticktime;
                    if (span.TotalSeconds < 0 || span.TotalSeconds > 10)
                    {
                        return Redirect(System.Configuration.ConfigurationManager.AppSettings["dwserviceurl"]);
                    }
                    else
                    {
                        Session["uid"] = uid;
                        var permiss = DB.Query("select permissions from data_user where ID=" + uid + "");
                        if (permiss != null && permiss.Tables[0].Rows.Count > 0)
                        {
                            var per = permiss.Tables[0].Rows[0][0].ToString();
                            Session["per"] = per;
                        }
                        return Redirect("threemap");
                    }
                }
                catch
                {
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings["dwserviceurl"]);
                }
            }
            else if (Session["uid"] == null || Session["uid"].ToString().Length != 8)
            {
                return Redirect(System.Configuration.ConfigurationManager.AppSettings["dwserviceurl"]);
            }
#endif
            return View();
        }

        #region 旧
//        public ActionResult Get1(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
//        {
//            try
//            {
//                if (REQUEST == "GetCapabilities")
//                {
//                    var resxml = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"GetCapabilities\GetCapabilities.txt"));
//                    var bres = System.Text.Encoding.GetEncoding("utf-8").GetBytes(resxml.ToString());
//                    Response.AddHeader("Access-Control-Allow-Origin", "*");
//                    return File(bres, "text/xml");
//                }
//                else if (REQUEST == "GetMap")
//                {
//                    ActionResult ar = null;
//                    if (!qx(WIDTH.Value, HEIGHT.Value, out ar))
//                    {
//                        return ar;
//                    }
//                    //**************************************************************************************//
//                    var rrect = BBOX.Split(',');
//#if CESHI
//                    var lon1 = double.Parse(rrect[0]);
//                    var lat1 = double.Parse(rrect[1]);
//                    var lon2 = double.Parse(rrect[2]);
//                    var lat2 = double.Parse(rrect[3]);
//#else
//                    var lon1 = double.Parse(rrect[1]);
//                    var lat1 = double.Parse(rrect[0]);
//                    var lon2 = double.Parse(rrect[3]);
//                    var lat2 = double.Parse(rrect[2]);
//#endif
//                    if (CRS == "EPSG:900913")
//                    {
//                        lon1 = lon1 / 20037508.34 * 180;
//                        lat1 = lat1 / 20037508.34 * 180;
//                        lat1 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat1 * Math.PI / 180)) - Math.PI / 2);

//                        lon2 = lon2 / 20037508.34 * 180;
//                        lat2 = lat2 / 20037508.34 * 180;
//                        lat2 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat2 * Math.PI / 180)) - Math.PI / 2);

//                        BBOX = lon1 + "," + lat1 + "," + lon2 + "," + lat2;
//                    }
//                    var w = lon2 - lon1;//经度跨度
//                    var h = lat2 - lat1;//纬度跨度
//                    var wb = WIDTH.Value / w;//像素经度
//                    var hb = HEIGHT.Value / h;//像素纬度
//                    Rectangle rect = new Rectangle(0, 0, WIDTH.Value, HEIGHT.Value);//矩形位置和大小
//                    Bitmap image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
//                    Graphics ig = Graphics.FromImage(image);
//                    var wb2 = w / WIDTH.Value;
//                    var hb2 = h / HEIGHT.Value;
//                    Dictionary<int, double> dic = new Dictionary<int, double>();
//                    Dictionary<int, int> dic2 = new Dictionary<int, int>();

//                    Bitmap output = null;
//                    {
//                        var tablei = 0;
//                        if (w > 11)
//                        {
//                            tablei = 1;
//                        }
//                        else if (w > 7)
//                        {
//                            tablei = 2;
//                        }
//                        else if (w > 3)
//                        {
//                            tablei = 3;
//                        }
//                        else if (w > 1)
//                        {
//                            tablei = 4;
//                        }
//                        else if (w > 0.9)
//                        {
//                            tablei = 5;
//                        }
//                        else if (w > 0.4)
//                        {
//                            tablei = 6;
//                        }

//                        #region 索引寻址
//                        var lastLat = lat1;

//                        var minInd = 0;

//                        var nbdt = GetDataClass.GetTableNBRSRPDX(tablei);
//                        var maxInd = nbdt.Rows.Count - 1;

//                        var tLen = maxInd;
//                        int numi = 0;
//                        int numj = 0;
//                        int numk = 0;
//                        double numg = 0;
//                        double numo = 0;
//                        double numy = 0;
//                        DateTime date1 = DateTime.Now;
//                        if (lat1 <= (double)(decimal)nbdt.Rows[maxInd]["sc_lat"] && lat2 >= (double)(decimal)nbdt.Rows[minInd]["sc_lat"])
//                        {

//                            while (lastLat < lat2)
//                            {
//                                numi++;
//                                var minR = nbdt.Rows[minInd];
//                                var maxR = nbdt.Rows[maxInd];
//                                if (!(minInd == 0 && lastLat <= (double)(decimal)minR[1] || minInd > 0 && lastLat < (double)(decimal)minR[1] && lastLat >= (double)(decimal)nbdt.Rows[minInd - 1][1]))
//                                {
//                                    DateTime date2 = DateTime.Now;
//                                    numk++;
//                                    //System.Diagnostics.Debug.WriteLine("aaaa");
//                                    var midInd = (maxInd - minInd) / 2 + minInd;
//                                    var midR = nbdt.Rows[midInd];
//                                    if (lastLat < (double)(decimal)midR[1])
//                                    {
//                                        maxInd = midInd;
//                                        if (maxInd - minInd == 1)
//                                        {
//                                            minInd++;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        minInd = midInd;
//                                        if (maxInd == minInd)
//                                            break;
//                                        if (maxInd - minInd == 1)
//                                        {
//                                            minInd++;
//                                        }

//                                    }
//                                    numg += (DateTime.Now - date2).TotalMilliseconds;
//                                }
//                                else
//                                {
//                                    var finish = false;
//                                    DateTime date3 = DateTime.Now;
//                                    for (int i = minInd; i < tLen + 1; i++)
//                                    {
//                                        numj++;
//                                        var dr2 = nbdt.Rows[i];
//                                        double lon = (double)((decimal)dr2[0]);
//                                        double lat = (double)((decimal)dr2[1]);
//                                        double rsrp = (double)((decimal)dr2[2]);
//                                        lastLat = lat;
//                                        if (lat < lat1 || lat > lat2)
//                                        {
//                                            finish = true;
//                                            //numo += (DateTime.Now - date3).TotalMilliseconds;
//                                            break;
//                                        }
//                                        if (lon < lon1)
//                                        {
//                                            //numo += (DateTime.Now - date3).TotalMilliseconds;
//                                            continue;
//                                        }
//                                        if (lon > lon2)
//                                        {
//                                            minInd = i;
//                                            maxInd = tLen;
//                                            //lastLat = (double)(decimal)nbdt.Rows[i - 1]["sc_lat"];
//                                            //numo += (DateTime.Now - date3).TotalMilliseconds;
//                                            break;
//                                        }
//                                        var xc = ((int)(float)((lon - lon1) * wb));
//                                        var yc = ((int)(HEIGHT.Value - (float)((lat - lat1) * hb)));
//                                        DateTime date4 = DateTime.Now;
//                                        if (dic.ContainsKey(xc * 1000000 + yc))
//                                        {
//                                            //dic[xc * 1000000 + yc]+= (double)((decimal)dr2[type]);
//                                            if (dr2[2] == DBNull.Value)
//                                            {
//                                                //dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]) < dic[xc * 1000000 + yc] ? (double)((decimal)dr2[type]) : dic[xc * 1000000 + yc];
//                                            }
//                                            else
//                                            {
//                                                dic[xc * 1000000 + yc] = rsrp < dic[xc * 1000000 + yc] ? rsrp : dic[xc * 1000000 + yc];
//                                            }

//                                        }
//                                        else
//                                        {
//                                            if (dr2[2] == DBNull.Value) { }
//                                            else
//                                            {
//                                                dic[xc * 1000000 + yc] = rsrp;
//                                            }

//                                        }
//                                        numy += (DateTime.Now - date4).TotalMilliseconds;
//                                        if (i == tLen)
//                                        {
//                                            finish = true;
//                                        }
//                                    }
//                                    if (finish)
//                                    {
//                                        //System.Diagnostics.Debug.WriteLine("ccc");
//                                        numo += (DateTime.Now - date3).TotalMilliseconds;
//                                        break;
//                                    }
//                                    numo += (DateTime.Now - date3).TotalMilliseconds;
//                                }
//                            }
//                        }
//                        //System.Diagnostics.Debug.WriteLine("while循环:"+numi);
//                        //System.Diagnostics.Debug.WriteLine("for循环:"+numj);
//                        //System.Diagnostics.Debug.WriteLine("if循环："+numk);
//                        //System.Diagnostics.Debug.WriteLine("foreach:"+dic.Count);
//                        //System.Diagnostics.Debug.WriteLine("while循环总时间:"+(DateTime.Now-date1).TotalMilliseconds);
//                        //System.Diagnostics.Debug.WriteLine("if毫秒数:" + numg);
//                        //System.Diagnostics.Debug.WriteLine("for循环毫秒数:"+numo);
//                        //System.Diagnostics.Debug.WriteLine("for-if循环毫秒数:" + numy);
//                        date1 = DateTime.Now;
//                        #endregion
//                        foreach (var d in dic)
//                        {
//                            var x10 = d.Key / 1000000;
//                            var y10 = d.Key % 1000000;
//                            //var lonl = x10 * wb2;
//                            var latl = (HEIGHT.Value - y10) * hb2 + lat1;
//                            var m50plon = 0.05 / ((40076 / 360.0) * Math.Cos(latl / 180 * Math.PI)) / wb2;
//                            var m50plat = 0.05 / (40009 / 360.0) / hb2;
//                            //var m50plonI = m50plon < 1 ? 1 : (int)m50plon;
//                            //var m50platI = m50plat < 1 ? 1 : (int)m50plat;
//                            var m50plonI = m50plon < 1 ? 1 : (int)m50plon + (m50plon - (int)m50plon >= 0.5 ? 1 : 0);
//                            var m50platI = m50plat < 1 ? 1 : (int)m50plat + (m50plat - (int)m50plat >= 0.5 ? 1 : 0);
//                            Rectangle r = new Rectangle(x10 - m50plonI / 2, y10 - m50platI / 2, m50plonI, m50platI);
//                            Brush bpix = new SolidBrush(getNBColor(d.Value));
//                            ig.FillRectangle(bpix, r);
//                        }
//                        System.Diagnostics.Debug.WriteLine((DateTime.Now - date1).TotalMilliseconds);
//                        MemoryStream stream1 = new MemoryStream();
//                        image.Save(stream1, ImageFormat.Png);
//                        return File(stream1.ToArray(), "application/x-png");
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                LOG.WriteLog(ex.Message + ":" + ex.StackTrace);
//                return null;
//            }
//            return null;
//            //**************************************************************************************//
//        }

//        public ActionResult Get2(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
//        {
//            try
//            {
//                if (REQUEST == "GetCapabilities")
//                {
//                    var resxml = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"GetCapabilities\GetCapabilities.txt"));
//                    var bres = System.Text.Encoding.GetEncoding("utf-8").GetBytes(resxml.ToString());
//                    Response.AddHeader("Access-Control-Allow-Origin", "*");
//                    return File(bres, "text/xml");
//                }
//                else if (REQUEST == "GetMap")
//                {
//                    ActionResult ar = null;
//                    if (!qx(WIDTH.Value, HEIGHT.Value, out ar))
//                    {
//                        return ar;
//                    }
//                    //**************************************************************************************//
//                    var rrect = BBOX.Split(',');
//#if CESHI
//                    var lon1 = double.Parse(rrect[0]);
//                    var lat1 = double.Parse(rrect[1]);
//                    var lon2 = double.Parse(rrect[2]);
//                    var lat2 = double.Parse(rrect[3]);
//#else
//                    var lon1 = double.Parse(rrect[1]);
//                    var lat1 = double.Parse(rrect[0]);
//                    var lon2 = double.Parse(rrect[3]);
//                    var lat2 = double.Parse(rrect[2]);
//#endif
//                    if (CRS == "EPSG:900913")
//                    {
//                        lon1 = lon1 / 20037508.34 * 180;
//                        lat1 = lat1 / 20037508.34 * 180;
//                        lat1 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat1 * Math.PI / 180)) - Math.PI / 2);

//                        lon2 = lon2 / 20037508.34 * 180;
//                        lat2 = lat2 / 20037508.34 * 180;
//                        lat2 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat2 * Math.PI / 180)) - Math.PI / 2);

//                        BBOX = lon1 + "," + lat1 + "," + lon2 + "," + lat2;
//                    }
//                    var w = lon2 - lon1;//经度跨度
//                    var h = lat2 - lat1;//纬度跨度
//                    var wb = WIDTH.Value / w;//像素经度
//                    var hb = HEIGHT.Value / h;//像素纬度
//                    Rectangle rect = new Rectangle(0, 0, WIDTH.Value, HEIGHT.Value);//矩形位置和大小
//                    Bitmap image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
//                    Graphics ig = Graphics.FromImage(image);
//                    var wb2 = w / WIDTH.Value;
//                    var hb2 = h / HEIGHT.Value;
//                    Dictionary<int, double> dic = new Dictionary<int, double>();
//                    Dictionary<int, int> dic2 = new Dictionary<int, int>();

//                    Bitmap output = null;
//                    {
//                        var tablei = 0;
//                        if (w > 11)
//                        {
//                            tablei = 1;
//                        }
//                        else if (w > 7)
//                        {
//                            tablei = 2;
//                        }
//                        else if (w > 3)
//                        {
//                            tablei = 3;
//                        }
//                        else if (w > 1)
//                        {
//                            tablei = 4;
//                        }
//                        else if (w > 0.9)
//                        {
//                            tablei = 5;
//                        }
//                        else if (w > 0.4)
//                        {
//                            tablei = 6;
//                        }

//                        #region 索引寻址
//                        var lastLat = lat1;

//                        var minInd = 0;

//                        var nbdt = GetDataClass.GetTableNBRSRPYD(tablei);
//                        var maxInd = nbdt.Rows.Count - 1;

//                        var tLen = maxInd;
//                        DateTime date1 = DateTime.Now;
//                        if (lat1 <= (double)(decimal)nbdt.Rows[maxInd]["sc_lat"] && lat2 >= (double)(decimal)nbdt.Rows[minInd]["sc_lat"])
//                        {

//                            while (lastLat < lat2)
//                            {
//                                var minR = nbdt.Rows[minInd];
//                                var maxR = nbdt.Rows[maxInd];
//                                if (!(minInd == 0 && lastLat <= (double)(decimal)minR[1] || minInd > 0 && lastLat < (double)(decimal)minR[1] && lastLat >= (double)(decimal)nbdt.Rows[minInd - 1][1]))
//                                {
//                                    var midInd = (maxInd - minInd) / 2 + minInd;
//                                    var midR = nbdt.Rows[midInd];
//                                    if (lastLat < (double)(decimal)midR[1])
//                                    {
//                                        maxInd = midInd;
//                                        if (maxInd - minInd == 1)
//                                        {
//                                            minInd++;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        minInd = midInd;
//                                        if (maxInd == minInd) break;
//                                        if (maxInd - minInd == 1)
//                                        {
//                                            minInd++;
//                                        }

//                                    }
//                                }
//                                else
//                                {
//                                    var finish = false;
//                                    DateTime date3 = DateTime.Now;
//                                    for (int i = minInd; i < tLen + 1; i++)
//                                    {
//                                        var dr2 = nbdt.Rows[i];
//                                        double lon = (double)((decimal)dr2[0]);
//                                        double lat = (double)((decimal)dr2[1]);
//                                        double rsrp = (double)((decimal)dr2[2]);
//                                        lastLat = lat;
//                                        if (lat < lat1 || lat > lat2)
//                                        {
//                                            finish = true;
//                                            break;
//                                        }
//                                        if (lon < lon1)
//                                        {
//                                            continue;
//                                        }
//                                        if (lon > lon2)
//                                        {
//                                            minInd = i;
//                                            maxInd = tLen;
//                                            break;
//                                        }
//                                        var xc = ((int)(float)((lon - lon1) * wb));
//                                        var yc = ((int)(HEIGHT.Value - (float)((lat - lat1) * hb)));
//                                        DateTime date4 = DateTime.Now;
//                                        if (dic.ContainsKey(xc * 1000000 + yc))
//                                        {
//                                            //dic[xc * 1000000 + yc]+= (double)((decimal)dr2[type]);
//                                            if (dr2[2] == DBNull.Value)
//                                            {
//                                                //dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]) < dic[xc * 1000000 + yc] ? (double)((decimal)dr2[type]) : dic[xc * 1000000 + yc];
//                                            }
//                                            else
//                                            {
//                                                dic[xc * 1000000 + yc] = rsrp < dic[xc * 1000000 + yc] ? rsrp : dic[xc * 1000000 + yc];
//                                            }

//                                        }
//                                        else
//                                        {
//                                            if (dr2[2] == DBNull.Value) { }
//                                            else
//                                            {
//                                                dic[xc * 1000000 + yc] = rsrp;
//                                            }

//                                        }

//                                        if (i == tLen)
//                                        {
//                                            finish = true;
//                                        }
//                                    }
//                                    if (finish)
//                                    {
//                                        break;
//                                    }
//                                }
//                            }
//                        }
//                        #endregion
//                        foreach (var d in dic)
//                        {
//                            var x10 = d.Key / 1000000;
//                            var y10 = d.Key % 1000000;
//                            //var lonl = x10 * wb2;
//                            var latl = (HEIGHT.Value - y10) * hb2 + lat1;
//                            var m50plon = 0.05 / ((40076 / 360.0) * Math.Cos(latl / 180 * Math.PI)) / wb2;
//                            var m50plat = 0.05 / (40009 / 360.0) / hb2;
//                            //var m50plonI = m50plon < 1 ? 1 : (int)m50plon;
//                            //var m50platI = m50plat < 1 ? 1 : (int)m50plat;
//                            var m50plonI = m50plon < 1 ? 1 : (int)m50plon + (m50plon - (int)m50plon >= 0.5 ? 1 : 0);
//                            var m50platI = m50plat < 1 ? 1 : (int)m50plat + (m50plat - (int)m50plat >= 0.5 ? 1 : 0);
//                            Rectangle r = new Rectangle(x10 - m50plonI / 2, y10 - m50platI / 2, m50plonI, m50platI);
//                            Brush bpix = new SolidBrush(getNBColor(d.Value));
//                            ig.FillRectangle(bpix, r);
//                        }
//                        MemoryStream stream1 = new MemoryStream();
//                        image.Save(stream1, ImageFormat.Png);
//                        return File(stream1.ToArray(), "application/x-png");
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                LOG.WriteLog(ex.Message + ":" + ex.StackTrace);
//                return null;
//            }
//            return null;
//            //**************************************************************************************//
//        }
//        public ActionResult Get3(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
//        {
//            try
//            {
//                if (REQUEST == "GetCapabilities")
//                {
//                    var resxml = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"GetCapabilities\GetCapabilities.txt"));
//                    var bres = System.Text.Encoding.GetEncoding("utf-8").GetBytes(resxml.ToString());
//                    Response.AddHeader("Access-Control-Allow-Origin", "*");
//                    return File(bres, "text/xml");
//                }
//                else if (REQUEST == "GetMap")
//                {
//                    ActionResult ar = null;
//                    if (!qx(WIDTH.Value, HEIGHT.Value, out ar))
//                    {
//                        return ar;
//                    }
//                    //**************************************************************************************//
//                    var rrect = BBOX.Split(',');
//#if CESHI
//                    var lon1 = double.Parse(rrect[0]);
//                    var lat1 = double.Parse(rrect[1]);
//                    var lon2 = double.Parse(rrect[2]);
//                    var lat2 = double.Parse(rrect[3]);
//#else
//                    var lon1 = double.Parse(rrect[1]);
//                    var lat1 = double.Parse(rrect[0]);
//                    var lon2 = double.Parse(rrect[3]);
//                    var lat2 = double.Parse(rrect[2]);
//#endif
//                    if (CRS == "EPSG:900913")
//                    {
//                        lon1 = lon1 / 20037508.34 * 180;
//                        lat1 = lat1 / 20037508.34 * 180;
//                        lat1 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat1 * Math.PI / 180)) - Math.PI / 2);

//                        lon2 = lon2 / 20037508.34 * 180;
//                        lat2 = lat2 / 20037508.34 * 180;
//                        lat2 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat2 * Math.PI / 180)) - Math.PI / 2);

//                        BBOX = lon1 + "," + lat1 + "," + lon2 + "," + lat2;
//                    }
//                    var w = lon2 - lon1;//经度跨度
//                    var h = lat2 - lat1;//纬度跨度
//                    var wb = WIDTH.Value / w;//像素经度
//                    var hb = HEIGHT.Value / h;//像素纬度
//                    Rectangle rect = new Rectangle(0, 0, WIDTH.Value, HEIGHT.Value);//矩形位置和大小
//                    Bitmap image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
//                    Graphics ig = Graphics.FromImage(image);
//                    var wb2 = w / WIDTH.Value;
//                    var hb2 = h / HEIGHT.Value;
//                    Dictionary<int, double> dic = new Dictionary<int, double>();
//                    Dictionary<int, int> dic2 = new Dictionary<int, int>();

//                    Bitmap output = null;
//                    {
//                        var tablei = 0;
//                        if (w > 11)
//                        {
//                            tablei = 1;
//                        }
//                        else if (w > 7)
//                        {
//                            tablei = 2;
//                        }
//                        else if (w > 3)
//                        {
//                            tablei = 3;
//                        }
//                        else if (w > 1)
//                        {
//                            tablei = 4;
//                        }
//                        else if (w > 0.9)
//                        {
//                            tablei = 5;
//                        }
//                        else if (w > 0.4)
//                        {
//                            tablei = 6;
//                        }

//                        #region 索引寻址
//                        var lastLat = lat1;

//                        var minInd = 0;

//                        var nbdt = GetDataClass.GetTableNBRSRPLT(tablei);
//                        var maxInd = nbdt.Rows.Count - 1;

//                        var tLen = maxInd;
//                        DateTime date1 = DateTime.Now;
//                        if (lat1 <= (double)(decimal)nbdt.Rows[maxInd]["sc_lat"] && lat2 >= (double)(decimal)nbdt.Rows[minInd]["sc_lat"])
//                        {

//                            while (lastLat < lat2)
//                            {
//                                var minR = nbdt.Rows[minInd];
//                                var maxR = nbdt.Rows[maxInd];
//                                if (!(minInd == 0 && lastLat <= (double)(decimal)minR[1] || minInd > 0 && lastLat < (double)(decimal)minR[1] && lastLat >= (double)(decimal)nbdt.Rows[minInd - 1][1]))
//                                {
//                                    var midInd = (maxInd - minInd) / 2 + minInd;
//                                    var midR = nbdt.Rows[midInd];
//                                    if (lastLat < (double)(decimal)midR[1])
//                                    {
//                                        maxInd = midInd;
//                                        if (maxInd - minInd == 1)
//                                        {
//                                            minInd++;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        minInd = midInd;
//                                        if (maxInd == minInd) break;
//                                        if (maxInd - minInd == 1)
//                                        {
//                                            minInd++;
//                                        }

//                                    }
//                                }
//                                else
//                                {
//                                    var finish = false;
//                                    DateTime date3 = DateTime.Now;
//                                    for (int i = minInd; i < tLen + 1; i++)
//                                    {
//                                        var dr2 = nbdt.Rows[i];
//                                        double lon = (double)((decimal)dr2[0]);
//                                        double lat = (double)((decimal)dr2[1]);
//                                        double rsrp = (double)((decimal)dr2[2]);
//                                        lastLat = lat;
//                                        if (lat < lat1 || lat > lat2)
//                                        {
//                                            finish = true;
//                                            break;
//                                        }
//                                        if (lon < lon1)
//                                        {
//                                            continue;
//                                        }
//                                        if (lon > lon2)
//                                        {
//                                            minInd = i;
//                                            maxInd = tLen;
//                                            break;
//                                        }
//                                        var xc = ((int)(float)((lon - lon1) * wb));
//                                        var yc = ((int)(HEIGHT.Value - (float)((lat - lat1) * hb)));
//                                        DateTime date4 = DateTime.Now;
//                                        if (dic.ContainsKey(xc * 1000000 + yc))
//                                        {
//                                            //dic[xc * 1000000 + yc]+= (double)((decimal)dr2[type]);
//                                            if (dr2[2] == DBNull.Value)
//                                            {
//                                                //dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]) < dic[xc * 1000000 + yc] ? (double)((decimal)dr2[type]) : dic[xc * 1000000 + yc];
//                                            }
//                                            else
//                                            {
//                                                dic[xc * 1000000 + yc] = rsrp < dic[xc * 1000000 + yc] ? rsrp : dic[xc * 1000000 + yc];
//                                            }

//                                        }
//                                        else
//                                        {
//                                            if (dr2[2] == DBNull.Value) { }
//                                            else
//                                            {
//                                                dic[xc * 1000000 + yc] = rsrp;
//                                            }

//                                        }

//                                        if (i == tLen)
//                                        {
//                                            finish = true;
//                                        }
//                                    }
//                                    if (finish)
//                                    {
//                                        break;
//                                    }
//                                }
//                            }
//                        }
//                        #endregion
//                        foreach (var d in dic)
//                        {
//                            var x10 = d.Key / 1000000;
//                            var y10 = d.Key % 1000000;
//                            //var lonl = x10 * wb2;
//                            var latl = (HEIGHT.Value - y10) * hb2 + lat1;
//                            var m50plon = 0.05 / ((40076 / 360.0) * Math.Cos(latl / 180 * Math.PI)) / wb2;
//                            var m50plat = 0.05 / (40009 / 360.0) / hb2;
//                            //var m50plonI = m50plon < 1 ? 1 : (int)m50plon;
//                            //var m50platI = m50plat < 1 ? 1 : (int)m50plat;
//                            var m50plonI = m50plon < 1 ? 1 : (int)m50plon + (m50plon - (int)m50plon >= 0.5 ? 1 : 0);
//                            var m50platI = m50plat < 1 ? 1 : (int)m50plat + (m50plat - (int)m50plat >= 0.5 ? 1 : 0);
//                            Rectangle r = new Rectangle(x10 - m50plonI / 2, y10 - m50platI / 2, m50plonI, m50platI);
//                            Brush bpix = new SolidBrush(getNBColor(d.Value));
//                            ig.FillRectangle(bpix, r);
//                        }
//                        MemoryStream stream1 = new MemoryStream();
//                        image.Save(stream1, ImageFormat.Png);
//                        return File(stream1.ToArray(), "application/x-png");
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                LOG.WriteLog(ex.Message + ":" + ex.StackTrace);
//                return null;
//            }
//            return null;
//            //**************************************************************************************//
//        }
        #endregion

        public ActionResult Get1(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS) {
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
                    byte[] byteArr = GetData.get(10+int.Parse(LAYERS), WIDTH.Value, HEIGHT.Value, double.Parse(bboxs[0]), double.Parse(bboxs[1]), double.Parse(bboxs[2]), double.Parse(bboxs[3]), crs);
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
        public ActionResult Get2(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
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
                    byte[] byteArr = GetData.get(20+int.Parse(LAYERS), WIDTH.Value, HEIGHT.Value, double.Parse(bboxs[0]), double.Parse(bboxs[1]), double.Parse(bboxs[2]), double.Parse(bboxs[3]), crs);
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
        public ActionResult Get3(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
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
                    byte[] byteArr = GetData.get(30+int.Parse(LAYERS), WIDTH.Value, HEIGHT.Value, double.Parse(bboxs[0]), double.Parse(bboxs[1]), double.Parse(bboxs[2]), double.Parse(bboxs[3]), crs);
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
        


        private Color getARGBBrushC(double bili)
        {
            //if (bili <= 0.1)
            //{
            //    return Color.FromArgb((int)(bili/0.1 * 255), Color.Green);
            //}
            var tmmax = 200;
            var bbb = 66;
            var aaa = tmmax - bbb;
            var tmr = aaa * bili + bbb;

            if (bili <= 0.25)
            {
                return Color.FromArgb((int)(tmr), Color.Green);
            }
            if (bili <= 0.5)
            {
                return Color.FromArgb((int)(tmr), 0, (int)(255 - (bili - 0.25) / 0.25 * 255), (int)((bili - 0.25) / 0.25 * 127 + 128));
            }
            if (bili <= 0.75)
            {
                return Color.FromArgb((int)(tmr), (int)((bili - 0.5) / 0.25 * 255), (int)((bili - 0.5) / 0.25 * 255), (int)(255 - (bili - 0.5) / 0.25 * 255));
            }
            if (bili <= 1)
            {
                return Color.FromArgb((int)(tmr), 255, 255 - (int)((bili - 0.75) / 0.25 * 255), 0);
            }
            return Color.FromArgb((int)(255), Color.Red);
        }


        private ColorBlend getColorBlend(double bili, bool fan = false)
        {
            bili = bili > 1 ? 1 : bili;
            ColorBlend colors = new ColorBlend(3);

            // Set brush stops.


            // The intensity value adjusts alpha of gradient colors.
            if (false)
            {
                colors.Positions = new float[3] { 0, 0.6f, 1 };
                colors.Colors = new Color[3]
                {
                    Color.FromArgb((int)((1-bili)*255), Color.Black),
                    //// The following colors can be any color - Only the alpha  value is used.
                    Color.FromArgb(1, Color.Black),
                    Color.FromArgb(1, Color.Black)
                };
            }
            else
            {
                colors.Positions = new float[3] { 0, 0.6f, 1 };
                colors.Colors = new Color[3]
                {
                    Color.FromArgb(0, Color.Black),
                    //// The following colors can be any color - Only the alpha  value is used.
                    Color.FromArgb((int)(bili*255*0.75), Color.Black),
                    Color.FromArgb((int)(bili*255), Color.Black)
                };
            }
            return colors;
        }



        private Color getNBColor(double v)
        {
            if (v >= -87)
            {
                return Color.FromArgb(96, 0, 255, 0);
            }
            if (v < -120)
            {
                return Color.FromArgb(96, 255, 0, 0);
            }
            if (v >= -97 && v < -87)
            {
                return Color.FromArgb(96, 0, 0, 255);
            }
            if (v >= -110 && v < -97)
            {
                return Color.FromArgb(96, 255, 255, 0);
            }
            if (v >= -120 && v < -110)
            {
                return Color.FromArgb(96, 255, 128, 0);
            }
            return Color.Red;
        }

        private bool qx(int w, int h, out ActionResult ar)
        {
            ar = null;
//#if CESHI
//            return true;
//#endif
            if (Session["per"] != null && Session["per"].ToString() != "3")
            {
                return true;
            }
            var msg = "";
            if (Session["per"] == null)
            {
                msg = "登录超时，请重新登录！";
            }
            Bitmap image = new Bitmap(w, h, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
            Graphics ig = Graphics.FromImage(image);
            ig.DrawString(msg, new Font("", 24), new SolidBrush(Color.Red), 50, 50);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);

            ar = File(stream.ToArray(), "application/x-png");
            return false;
        }
    }
}
