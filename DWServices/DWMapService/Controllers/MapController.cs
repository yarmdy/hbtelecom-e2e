using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using DWMapService.Lib;
using System.Security.Cryptography;

namespace DWMapService.Controllers
{
    public class MapController : Controller
    {
        //
        // GET: /Map/

        public ActionResult Index(string token)
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
                        return Redirect("map");
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
        public ActionResult Evt(string token) {
            return View();
        }

        public ActionResult Get(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
        {
            try {
                if (REQUEST == "GetCapabilities")
                {
                    var resxml = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"GetCapabilities\GetCapabilities.txt"));
                    var bres = System.Text.Encoding.GetEncoding("utf-8").GetBytes(resxml.ToString());
                    Response.AddHeader("Access-Control-Allow-Origin", "*");
                    return File(bres, "text/xml");
                }
                else if (REQUEST == "GetMap")
                {
                    string station = LAYERS.Split('_')[0];
                    string type = LAYERS.Split('_')[1];
                    ActionResult ar = null;
                    if (!qx(station, type, WIDTH.Value, HEIGHT.Value, out ar))
                    {
                        return ar;
                    }
                    DataSet ds = GetDataClass.GetDataSet(station);
                    if (type == "流量") type = "flow";
                    else if (type == "PRB利用率") type = "prb";
                    else if (type == "RRC连接数") type = "rrc";
                    else if (type == "覆盖") type = "rsrp";

                    //**************************************************************************************//
                    var rrect = BBOX.Split(',');
#if CESHI
                var lon1 = double.Parse(rrect[0]);
                var lat1 = double.Parse(rrect[1]);
                var lon2 = double.Parse(rrect[2]);
                var lat2 = double.Parse(rrect[3]);
#else
                    var lon1 = double.Parse(rrect[1]);
                    var lat1 = double.Parse(rrect[0]);
                    var lon2 = double.Parse(rrect[3]);
                    var lat2 = double.Parse(rrect[2]);
#endif
                    if (CRS == "EPSG:900913")
                    {
                        lon1 = lon1 / 20037508.34 * 180;
                        lat1 = lat1 / 20037508.34 * 180;
                        lat1 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat1 * Math.PI / 180)) - Math.PI / 2);

                        lon2 = lon2 / 20037508.34 * 180;
                        lat2 = lat2 / 20037508.34 * 180;
                        lat2 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat2 * Math.PI / 180)) - Math.PI / 2);

                        BBOX = lon1 + "," + lat1 + "," + lon2 + "," + lat2;
                    }
                    var w = lon2 - lon1;//经度跨度
                    var h = lat2 - lat1;//纬度跨度
                    var wb = WIDTH.Value / w;//像素经度
                    var hb = HEIGHT.Value / h;//像素纬度
                    Rectangle rect = new Rectangle(0, 0, WIDTH.Value, HEIGHT.Value);//矩形位置和大小
                    Bitmap image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
                    Graphics ig = Graphics.FromImage(image);
                    var wb2 = w / WIDTH.Value;
                    var hb2 = h / HEIGHT.Value;
                    Dictionary<int, double> dic = new Dictionary<int, double>();
                    Dictionary<int, int> dic2 = new Dictionary<int, int>();

                    Bitmap output = null;
                    if (station == "NB-IoT")
                    {
                        var tablei = 0;
                        if (w > 11)
                        {
                            tablei = 1;
                        }
                        else if (w > 7)
                        {
                            tablei = 2;
                        }
                        else if (w > 3)
                        {
                            tablei = 3;
                        }
                        else if (w > 1)
                        {
                            tablei = 4;
                        }
                        else if (w > 0.9)
                        {
                            tablei = 5;
                        }
                        else if (w > 0.4)
                        {
                            tablei = 6;
                        }
                        //foreach (System.Data.DataRow dr2 in nbdt.Rows)
                        //{
                        //    double lon = (double)((decimal)dr2["sc_lon"]);
                        //    double lat = (double)((decimal)dr2["sc_lat"]);

                        //    if (lon < lon1 || lon > lon2 || lat < lat1 || lat > lat2)
                        //    {
                        //        continue;
                        //    }
                        //    var xc = ((int)(float)((lon - lon1) * wb));
                        //    var yc = ((int)(HEIGHT.Value - (float)((lat - lat1) * hb)));
                        //    if (dic.ContainsKey(xc * 1000000 + yc))
                        //    {
                        //        //dic[xc * 1000000 + yc]+= (double)((decimal)dr2[type]);
                        //        if (dr2["nbrsrp"] == DBNull.Value)
                        //        {
                        //            //dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]) < dic[xc * 1000000 + yc] ? (double)((decimal)dr2[type]) : dic[xc * 1000000 + yc];
                        //        }
                        //        else
                        //        {
                        //            dic[xc * 1000000 + yc] = (double)((decimal)dr2["nbrsrp"]) < dic[xc * 1000000 + yc] ? (double)((decimal)dr2["nbrsrp"]) : dic[xc * 1000000 + yc];
                        //        }

                        //    }
                        //    else
                        //    {
                        //        if (dr2["nbrsrp"] == DBNull.Value) { }
                        //        else
                        //        {
                        //            dic[xc * 1000000 + yc] = (double)((decimal)dr2["nbrsrp"]);
                        //        }

                        //    }
                        //}

                        #region 索引寻址
                        var lastLat = lat1;

                        var minInd = 0;
                        //LOG.WriteLog(ds.Tables.Count+" tables");
                        var nbdt = GetDataClass.GetTableNB(tablei);
                        var maxInd = nbdt.Rows.Count - 1;

                        var tLen = maxInd;
                        if (lat1 <= (double)(decimal)nbdt.Rows[maxInd]["sc_lat"] && lat2 >= (double)(decimal)nbdt.Rows[minInd]["sc_lat"])
                        {
                            while (lastLat < lat2)
                            {
                                //numi++;
                                var minR = nbdt.Rows[minInd];
                                var maxR = nbdt.Rows[maxInd];
                                if (!(minInd == 0 && lastLat <= (double)(decimal)minR[1] || minInd > 0 && lastLat < (double)(decimal)minR[1] && lastLat >= (double)(decimal)nbdt.Rows[minInd - 1][1]))
                                {
                                    //DateTime date2 = DateTime.Now;
                                    //numk++;
                                    //System.Diagnostics.Debug.WriteLine("aaaa");
                                    var midInd = (maxInd - minInd) / 2 + minInd;
                                    var midR = nbdt.Rows[midInd];
                                    if (lastLat < (double)(decimal)midR[1])
                                    {
                                        maxInd = midInd;
                                        if (maxInd - minInd == 1)
                                        {
                                            minInd++;
                                        }
                                    }
                                    else
                                    {
                                        minInd = midInd;
                                        if (maxInd == minInd)
                                            break;
                                        if (maxInd - minInd == 1)
                                        {
                                            minInd++;
                                        }

                                    }
                                    //numg += (DateTime.Now - date2).TotalMilliseconds;
                                }
                                else
                                {
                                    var finish = false;
                                    //DateTime date3 = DateTime.Now;
                                    for (int i = minInd; i < tLen + 1; i++)
                                    {
                                        //numj++;
                                        var dr2 = nbdt.Rows[i];
                                        double lon = (double)((decimal)dr2[0]);
                                        double lat = (double)((decimal)dr2[1]);
                                        double rsrp = (double)((decimal)dr2[2]);
                                        lastLat = lat;
                                        if (lat < lat1 || lat > lat2)
                                        {
                                            finish = true;
                                            //numo += (DateTime.Now - date3).TotalMilliseconds;
                                            break;
                                        }
                                        if (lon < lon1)
                                        {
                                            //numo += (DateTime.Now - date3).TotalMilliseconds;
                                            continue;
                                        }
                                        if (lon > lon2)
                                        {
                                            minInd = i;
                                            maxInd = tLen;
                                            //lastLat = (double)(decimal)nbdt.Rows[i - 1]["sc_lat"];
                                            //numo += (DateTime.Now - date3).TotalMilliseconds;
                                            break;
                                        }
                                        var xc = ((int)(float)((lon - lon1) * wb));
                                        var yc = ((int)(HEIGHT.Value - (float)((lat - lat1) * hb)));
                                        DateTime date4 = DateTime.Now;
                                        if (dic.ContainsKey(xc * 1000000 + yc))
                                        {
                                            //dic[xc * 1000000 + yc]+= (double)((decimal)dr2[type]);
                                            if (dr2[2] == DBNull.Value)
                                            {
                                                //dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]) < dic[xc * 1000000 + yc] ? (double)((decimal)dr2[type]) : dic[xc * 1000000 + yc];
                                            }
                                            else
                                            {
                                                dic[xc * 1000000 + yc] = rsrp < dic[xc * 1000000 + yc] ? rsrp : dic[xc * 1000000 + yc];
                                            }

                                        }
                                        else
                                        {
                                            if (dr2[2] == DBNull.Value) { }
                                            else
                                            {
                                                dic[xc * 1000000 + yc] = rsrp;
                                            }

                                        }
                                        //numy += (DateTime.Now - date4).TotalMilliseconds;
                                        if (i == tLen)
                                        {
                                            finish = true;
                                        }
                                    }
                                    if (finish)
                                    {
                                        //System.Diagnostics.Debug.WriteLine("ccc");
                                        //numo += (DateTime.Now - date3).TotalMilliseconds;
                                        break;
                                    }
                                    //numo += (DateTime.Now - date3).TotalMilliseconds;
                                }
                            }
                        }
                        #endregion
                        foreach (var d in dic)
                        {
                            var x10 = d.Key / 1000000;
                            var y10 = d.Key % 1000000;
                            //var lonl = x10 * wb2;
                            var latl = (HEIGHT.Value - y10) * hb2 + lat1;
                            var m50plon = 0.05 / ((40076 / 360.0) * Math.Cos(latl / 180 * Math.PI)) / wb2;
                            var m50plat = 0.05 / (40009 / 360.0) / hb2;
                            //var m50plonI = m50plon < 1 ? 1 : (int)m50plon;
                            //var m50platI = m50plat < 1 ? 1 : (int)m50plat;
                            var m50plonI = m50plon < 1 ? 1 : (int)m50plon + (m50plon - (int)m50plon >= 0.5 ? 1 : 0);
                            var m50platI = m50plat < 1 ? 1 : (int)m50plat + (m50plat - (int)m50plat >= 0.5 ? 1 : 0);
                            Rectangle r = new Rectangle(x10 - m50plonI / 2, y10 - m50platI / 2, m50plonI, m50platI);
                            Brush bpix = new SolidBrush(getNBColor(d.Value));
                            ig.FillRectangle(bpix, r);
                        }
                        MemoryStream stream1 = new MemoryStream();
                        image.Save(stream1, ImageFormat.Png);
                        return File(stream1.ToArray(), "application/x-png");
                    }
                    if (w >= 0.04)
                    {
                        output = new Bitmap(WIDTH.Value, HEIGHT.Value, PixelFormat.Format32bppArgb);
                        foreach (System.Data.DataRow dr2 in ds.Tables[0].Rows)
                        {
                            double lon = (double)((decimal)dr2["sc_lon"]);
                            double lat = (double)((decimal)dr2["sc_lat"]);

                            if (lon < lon1 || lon > lon2 || lat < lat1 || lat > lat2)
                            {
                                continue;
                            }
                            var xc = ((int)(float)((lon - lon1) * wb)) / 10;
                            var yc = ((int)(HEIGHT.Value - (float)((lat - lat1) * hb))) / 10;
                            if (dic.ContainsKey(xc * 1000000 + yc))
                            {
                                //dic[xc * 1000000 + yc]+= (double)((decimal)dr2[type]);
                                if (dr2[type] == DBNull.Value)
                                {
                                    //dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]) < dic[xc * 1000000 + yc] ? (double)((decimal)dr2[type]) : dic[xc * 1000000 + yc];
                                }
                                else
                                {
                                    if (type == "rsrp" || type == "prb" || type == "flow")
                                    {
                                        //dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]) < dic[xc * 1000000 + yc] ? (double)((decimal)dr2[type]) : dic[xc * 1000000 + yc];
                                        dic2[xc * 1000000 + yc]++;
                                        dic[xc * 1000000 + yc] = (dic[xc * 1000000 + yc] * (dic2[xc * 1000000 + yc] - 1) + (double)((decimal)dr2[type])) / dic2[xc * 1000000 + yc];
                                    }
                                    else
                                    {
                                        dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]) > dic[xc * 1000000 + yc] ? (double)((decimal)dr2[type]) : dic[xc * 1000000 + yc];
                                    }

                                }

                            }
                            else
                            {
                                if (dr2[type] == DBNull.Value) { }
                                else
                                {
                                    dic[xc * 1000000 + yc] = (double)((decimal)dr2[type]);
                                    dic2[xc * 1000000 + yc] = 1;
                                }

                            }
                        }

                        GraphicsPath pt = new GraphicsPath();
                        int rad = 12;
                        var x1 = 0.0002;
                        var x2 = 0.2197;
                        var y1 = 10.0;
                        var y2 = 1000.0;

                        var a = (y2 - y1) / (x2 - y1);
                        var b = y1 - a * x1;
                        var maxV = 100.0;
                        if (type == "flow")
                        {
                            switch (station)
                            {
                                case "L800M": maxV = 1024 * 1.2; break;
                                case "L1.8G": maxV = 1024 * 6; break;
                                case "L2.1G": maxV = 1024 * 8; break;
                                case "L2.6G": maxV = 1024 * 8; break;
                                case "NB-IoT": maxV = 1024 * 1.2; break;
                                default: maxV = 100; break;
                            }
                        }
                        else
                        {
                            switch (type)
                            {
                                case "prb": maxV = 50; break;
                                case "rrc": maxV = 200; break;
                                case "rsrp": maxV = 0.9; break;
                                default: maxV = 100; break;
                            }
                        }
                        foreach (var d in dic)
                        {
                            //if (d.Value <= 0 && type!="rsrp") continue;
                            var x10 = d.Key / 1000000;
                            var y10 = d.Key % 1000000;
                            Rectangle r = new Rectangle(x10 * 10 - rad, y10 * 10 - rad, rad * 2, rad * 2);


                            var bili = d.Value / maxV;
                            if (type == "rsrp")
                            {
                                if (d.Value >= 0.9)
                                {
                                    bili = 0.001;
                                }
                                else if (d.Value < 0.45)
                                {
                                    bili = 1;
                                }
                                else
                                {
                                    bili = (0.9 - d.Value) / 0.45;
                                }
                            }
                            if (bili < 0.1) bili = 0.1;

                            var ellipsePath = new GraphicsPath();
                            ellipsePath.AddEllipse(r);
                            PathGradientBrush br = new PathGradientBrush(ellipsePath);
                            ColorBlend gradientSpecifications = getColorBlend(bili, type == "rsrp");
                            br.InterpolationColors = gradientSpecifications;
                            ig.FillEllipse(br, r);
                        }
                        for (int x = 0; x < WIDTH.Value; x++)
                        {
                            for (int y = 0; y < HEIGHT.Value; y++)
                            {
                                var alp = image.GetPixel(x, y).A;
                                if (alp <= 0) continue;
                                if (false)
                                {
                                    output.SetPixel(x, y
                                    , getARGBBrushC(1 - alp / 255f));
                                }
                                else
                                {
                                    output.SetPixel(x, y
                                        , getARGBBrushC(alp / 255f));
                                }
                            }
                        }
                    }

                    MemoryStream stream = new MemoryStream();
                    if (w >= 0.04)
                    {
                        output.Save(stream, ImageFormat.Png);
                    }
                    else
                    {
                        image.Save(stream, ImageFormat.Png);
                    }

                    return File(stream.ToArray(), "application/x-png");
                }

                return null;
            }
            catch (Exception ex) {
                LOG.WriteLog(ex.Message+":"+ex.StackTrace);
                return null;
            }
            
            //**************************************************************************************//
        }

        public ActionResult GetFinal(double minLat, double minLon, double maxLat, double maxLon, string station, string type)
        {
            ActionResult ar = null;
            if (!qx(station, type, 100, 100, out ar))
            {
                return null;
            }
            if (station == "NB-IoT")
            {
                station = "L800M";
            }
            int l18 = 1;
            if (station == "L1.8G") {
                l18 = 1;
            }
            DataSet ds = GetDataClass.GetDataSet(station);
            if (ds == null) return null;
            DataTable dt = ds.Tables[0];
            DataTable table = ds.Tables[0].Copy();
            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = table.Rows[i];
                double enb_lat = enb_lat = (double)((decimal)dr[7]);
                double enb_lon = enb_lon = (double)((decimal)dr[8]);
                if (dr[6]==DBNull.Value ||enb_lat > maxLat || enb_lat < minLat || enb_lon > maxLon || enb_lon < minLon) dr.Delete();
            }
            table.AcceptChanges();
            var ecis = table.AsEnumerable().Select(a => (int)((decimal)a["eci"])).ToArray();
            var ecisstring = string.Join(",", ecis);
            var ecistable = DB.QueryAsDt("select to_char(time,'hh24') hour,round(case manufactor when '诺基亚' then DOWN_PDCP/"+l18+" else DOWN_PDCP end,2) DOWN_PDCP,round(DOWN_PRB,2) DOWN_PRB,eci from DATA_KPIINFO where time >=trunc(sysdate-1) and time<trunc(sysdate) and eci in (" + ecisstring + ") order by eci,time");
            if (station != "CDMA")
            {
                table.Columns.Add("FLOW24", typeof(string));
                table.Columns.Add("PRB24", typeof(string));
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    int eci = (int)((decimal)table.Rows[i]["eci"]);
                    //DataTable dt24 = GetDataClass.Get24ByEci(eci);
                    var view = ecistable.DefaultView;
                    view.RowFilter = "eci=" + eci;
                    DataTable dt24 = view.ToTable();
                    StringBuilder flow24 = new StringBuilder();
                    StringBuilder prb24 = new StringBuilder();
                    foreach (DataRow dr in dt24.Rows)
                    {
                        if (dr[1] != DBNull.Value)
                            flow24.Append(dr[0]).Append("时:").Append(Math.Round((double)((decimal)dr[1] / 1024), 2)).Append("G<br/>");
                        if (dr[2] != DBNull.Value)
                            prb24.Append(dr[0]).Append("时:").Append(dr[2]).Append("%<br/>");
                    }
                    table.Rows[i]["FLOW24"] = flow24;
                    table.Rows[i]["PRB24"] = prb24;
                }
            }
            string result = DatatableToJson.GetJsonByDataset(table);
            return Content(result);
        }


        public ActionResult Query(string enbid, string cellid, string station, string type)
        {
            int eci = 0;
            try
            {
                var cval = 256;
                if (station == "CDMA") cval = 4096;
                eci = int.Parse(enbid) * cval + int.Parse(cellid);
            }
            catch (Exception e)
            {
                return Content("{\"ok\":false,\"eci\":\"无法解析\"}");
            }
            if (station == "NB-IoT")
            {
                station = "L800M";
            }
            DataSet ds = GetDataClass.GetDataSet(station);
            if (ds == null)
            {
                return Content("{\"ok\":false,\"eci\":" + eci + ",或不存在}");
            }
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                if ((int)((decimal)dr["ECI"]) == eci)
                {
                    return Content("{\"ok\":true,\"enb_lat\":" + (double)((decimal)dr[7]) + ",\"enb_lon\":" + (double)((decimal)dr[8]) + "}");
                }
            }
            return Content("{\"ok\":false,\"eci\":" + eci + ",或不存在}");
        }

        private Brush getARGBBrush(double bili)
        {
            if (bili <= 0.1)
            {
                return new SolidBrush(Color.FromArgb((int)(192 * bili), Color.Blue));
            }
            if (bili <= 0.33)
            {
                return new SolidBrush(Color.FromArgb(192, 0, (int)((bili - 0.1) / 0.23 * 127 + 128), 0));
            }
            if (bili <= 0.66)
            {
                return new SolidBrush(Color.FromArgb(192, (int)((bili - 0.33) / 0.33 * 255), 255, 0));
            }
            if (bili <= 1)
            {
                return new SolidBrush(Color.FromArgb(192, 255, 255 - (int)((bili - 0.66) / 0.34 * 255), 0));
            }
            return new SolidBrush(Color.FromArgb(192, Color.Red));
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
        public Bitmap BitMapToBlur(Bitmap orBitmap)
        {
            //返回图像  
            Bitmap reBitmap = new Bitmap(orBitmap.Width, orBitmap.Height);
            Color pixel1, pixel2, pixel3, pixel4;
            int Strength = 1;
            for (int i = 0; i < orBitmap.Height; i++)
            {
                for (int j = 0; j < orBitmap.Width; j++)
                {
                    int r = 0, g = 0, b = 0;
                    int x = i, y = j;
                    if (x == 0) x = 1;
                    if (y == 0) y = 1;
                    if (x == orBitmap.Height - 1) x = orBitmap.Height - 2;
                    if (y == orBitmap.Width - 1) y = orBitmap.Width - 2;
                    pixel1 = orBitmap.GetPixel(y - Strength, x - Strength);
                    pixel2 = orBitmap.GetPixel(y + Strength, x + Strength);
                    pixel3 = orBitmap.GetPixel(y - Strength, x + Strength);
                    pixel4 = orBitmap.GetPixel(y + Strength, x - Strength);
                    pixel2 = Color.FromArgb((pixel2.R + pixel1.R + pixel3.R + pixel4.R) / 4, (pixel2.G + pixel1.G + pixel3.G + pixel4.G) / 4, (pixel2.B + pixel1.B + pixel3.B + pixel4.B) / 4);
                    r = pixel2.R;
                    g = pixel2.G;
                    b = pixel2.B;
                    //防止溢出  
                    if (r > 255) r = 255;
                    if (r < 0) r = 0;
                    if (g > 255) g = 255;
                    if (g < 0) g = 0;
                    if (b > 255) b = 255;
                    if (b < 0) b = 0;
                    reBitmap.SetPixel(j, i, Color.FromArgb(r, g, b));
                }
            }
            return reBitmap;
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
        private bool qx(string station, string type, int w, int h, out ActionResult ar)
        {
            ar = null;
#if CESHI
            return true;
#endif
            if (Session["per"] != null && Session["per"].ToString() != "3")
            {
                return true;
            }
            var msg = "";
            if (Session["per"] == null)
            {
                msg = "登录超时，请重新登录！";
            }
            else if (station == "NB-IoT")
            {
                return true;
            }
            else
            {
                msg = "您没有权限查看此模块！";
            }
            Bitmap image = new Bitmap(w, h, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
            Graphics ig = Graphics.FromImage(image);
            ig.DrawString(msg, new Font("", 24), new SolidBrush(Color.Red), 50, 50);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);

            ar = File(stream.ToArray(), "application/x-png");
            return false;
        }
        public ActionResult Evtget(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
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
                    //string station = LAYERS.Split('_')[0];
                    //string type = LAYERS.Split('_')[1];



                    var dt = GetDataClass.evtdt;
                    //if (type == "流量") type = "flow";
                    //else if (type == "PRB利用率") type = "prb";
                    //else if (type == "RRC连接数") type = "rrc";
                    //else if (type == "覆盖") type = "rsrp";

                    //**************************************************************************************//
                    var rrect = BBOX.Split(',');
#if CESHI
                    var lon1 = double.Parse(rrect[0]);
                    var lat1 = double.Parse(rrect[1]);
                    var lon2 = double.Parse(rrect[2]);
                    var lat2 = double.Parse(rrect[3]);
#else
                    var lon1 = double.Parse(rrect[1]);
                    var lat1 = double.Parse(rrect[0]);
                    var lon2 = double.Parse(rrect[3]);
                    var lat2 = double.Parse(rrect[2]);
#endif
                    if (CRS == "EPSG:900913")
                    {
                        lon1 = lon1 / 20037508.34 * 180;
                        lat1 = lat1 / 20037508.34 * 180;
                        lat1 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat1 * Math.PI / 180)) - Math.PI / 2);

                        lon2 = lon2 / 20037508.34 * 180;
                        lat2 = lat2 / 20037508.34 * 180;
                        lat2 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat2 * Math.PI / 180)) - Math.PI / 2);

                        BBOX = lon1 + "," + lat1 + "," + lon2 + "," + lat2;
                    }
                    var w = lon2 - lon1;//经度跨度
                    var h = lat2 - lat1;//纬度跨度
                    var wb = WIDTH.Value / w;//像素经度
                    var hb = HEIGHT.Value / h;//像素纬度
                    Rectangle rect = new Rectangle(0, 0, WIDTH.Value, HEIGHT.Value);//矩形位置和大小
                    Bitmap image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
                    Graphics ig = Graphics.FromImage(image);
                    var wb2 = w / WIDTH.Value;
                    var hb2 = h / HEIGHT.Value;
                    Bitmap output = null;
                    Dictionary<int, double> dic = new Dictionary<int, double>();
                    Dictionary<int, int> dic2 = new Dictionary<int, int>();
                    if (w >= 0.04)
                    {
                        output = new Bitmap(WIDTH.Value, HEIGHT.Value, PixelFormat.Format32bppArgb);
                        foreach (System.Data.DataRow dr2 in dt.Rows)
                        {
                            double lon = (double)((decimal)dr2["LONGITUDE"]);
                            double lat = (double)((decimal)dr2["LATITUDE"]);

                            if (lon < lon1 || lon > lon2 || lat < lat1 || lat > lat2)
                            {
                                continue;
                            }
                            var xc = ((int)(float)((lon - lon1) * wb)) / 10;
                            var yc = ((int)(HEIGHT.Value - (float)((lat - lat1) * hb))) / 10;
                            if (dic.ContainsKey(xc * 1000000 + yc))
                            {
                                dic2[xc * 1000000 + yc]++;
                                dic[xc * 1000000 + yc] ++;

                            }
                            else
                            {
                                dic[xc * 1000000 + yc] = 1;
                                dic2[xc * 1000000 + yc] = 1;
                            }
                        }

                        GraphicsPath pt = new GraphicsPath();
                        int rad = 12;
                        var x1 = 0.0002;
                        var x2 = 0.2197;
                        var y1 = 10.0;
                        var y2 = 1000.0;

                        var a = (y2 - y1) / (x2 - y1);
                        var b = y1 - a * x1;
                        var maxV = 100.0;
                        maxV = 100;
                        if (w > 11)
                        {
                            maxV = 30;
                        }
                        else if (w > 7)
                        {
                            maxV = 25;
                        }
                        else if (w > 3)
                        {
                            maxV = 20;
                        }
                        else if (w > 1)
                        {
                            maxV = 15;
                        }
                        else if (w > 0.9)
                        {
                            maxV = 10;
                        }
                        else if (w > 0.4)
                        {
                            maxV = 5;
                        }
                        else {
                            maxV = 3;
                        }
                        foreach (var d in dic)
                        {
                            //if (d.Value <= 0 && type!="rsrp") continue;
                            var x10 = d.Key / 1000000;
                            var y10 = d.Key % 1000000;
                            Rectangle r = new Rectangle(x10 * 10 - rad, y10 * 10 - rad, rad * 2, rad * 2);


                            var bili = d.Value / maxV;
                            
                            if (bili < 0.1) bili = 0.1;

                            var ellipsePath = new GraphicsPath();
                            ellipsePath.AddEllipse(r);
                            PathGradientBrush br = new PathGradientBrush(ellipsePath);
                            ColorBlend gradientSpecifications = getColorBlend(bili, false);
                            br.InterpolationColors = gradientSpecifications;
                            ig.FillEllipse(br, r);
                        }
                        for (int x = 0; x < WIDTH.Value; x++)
                        {
                            for (int y = 0; y < HEIGHT.Value; y++)
                            {
                                var alp = image.GetPixel(x, y).A;
                                if (alp <= 0) continue;
                                if (false)
                                {
                                    output.SetPixel(x, y
                                    , getARGBBrushC(1 - alp / 255f));
                                }
                                else
                                {
                                    output.SetPixel(x, y
                                        , getARGBBrushC(alp / 255f));
                                }
                            }
                        }
                    }

                    MemoryStream stream = new MemoryStream();
                    if (w >= 0.04)
                    {
                        output.Save(stream, ImageFormat.Png);
                    }
                    else
                    {
                        image.Save(stream, ImageFormat.Png);
                    }

                    return File(stream.ToArray(), "application/x-png");
                }

                return null;
            }
            catch (Exception ex)
            {
                LOG.WriteLog(ex.Message + ":" + ex.StackTrace);
                return null;
            }

            //**************************************************************************************//
        }
        public ActionResult Getevtobj(double minLat, double minLon, double maxLat, double maxLon, string station, string type)
        {

            var dt = GetDataClass.evtdt;
            if (dt == null) return null;
            DataTable table = dt.Copy();
            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = table.Rows[i];
                double enb_lat = enb_lat = (double)((decimal)dr[5]);
                double enb_lon = enb_lon = (double)((decimal)dr[4]);
                if ( enb_lat > maxLat || enb_lat < minLat || enb_lon > maxLon || enb_lon < minLon) dr.Delete();
            }
            table.AcceptChanges();
            //var ecis = table.AsEnumerable().Select(a => (int)((decimal)a["LTECI"])).ToArray();
            //var ecisstring = string.Join(",", ecis);
            //var ecistable = DB.QueryAsDt("select to_char(time,'hh24') hour,round(DOWN_PDCP,2) DOWN_PDCP,round(DOWN_PRB,2) DOWN_PRB,eci from DATA_KPIINFO where time >=trunc(sysdate-1) and time<trunc(sysdate) and eci in (" + ecisstring + ") order by eci,time");
            
            string result = DatatableToJson.GetJsonByDataset(table);
            return Content(result);
        }
        public ActionResult Downevt() {
            StringBuilder sb = new StringBuilder();
            var tt = "";
            foreach (DataColumn cel in GetDataClass.evtGdt.Columns) {
                tt += ","+cel.ColumnName;
            }
            tt = tt.Substring(1);
            sb.Append(tt+"\r\n");
            foreach (DataRow row in GetDataClass.evtGdt.Rows) {
                var rowstr = string.Join(",",row.ItemArray);
                sb.Append(rowstr+"\r\n");
            }
            
            var csvdata = System.Text.Encoding.GetEncoding("gbk").GetBytes(sb.ToString());
            return File(csvdata, "application/octet-stream", "事件统计" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
        }
        Dictionary<int, string> cityNames = new Dictionary<int, string>() { { 0, "石家庄" }, { 1, "廊坊" }, { 2, "保定" }, { 3, "邯郸" }, { 4, "沧州" }, { 5, "衡水" }, { 6, "邢台" }, { 7, "唐山" }, { 8, "秦皇岛" }, { 9, "张家口" }, { 10, "承德" }, { 11, "雄安新区" }, { 12, "全省" } };
        public ActionResult Down(int id)
        {
            StringBuilder sb = new StringBuilder();
            var city = cityNames[id];
            sb.Append("eci,sc_name,city,flow,prb,rrc,rsrp,channel\r\n");
            if (GetDataClass.L800M != null && GetDataClass.L800M.Tables.Count > 0)
            {
                foreach (DataRow row in GetDataClass.L800M.Tables[0].Rows)
                {
                    if (city == "全省" || city == row[18].ToString())
                    {
                        var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\r\n", row[0], row[10], row[18], row[1], row[2], row[3], row[6], "800M");
                        sb.Append(line);
                    }
                }
            }
            if (GetDataClass.L1_8G != null && GetDataClass.L1_8G.Tables.Count > 0)
            {
                foreach (DataRow row in GetDataClass.L800M.Tables[0].Rows)
                {
                    if (city == "全省" || city == row[18].ToString())
                    {
                        var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\r\n", row[0], row[10], row[18], row[1], row[2], row[3], row[6], "1.8G");
                        sb.Append(line);
                    }
                }
            }
            if (GetDataClass.L2_1G != null && GetDataClass.L2_1G.Tables.Count > 0)
            {
                foreach (DataRow row in GetDataClass.L800M.Tables[0].Rows)
                {
                    if (city == "全省" || city == row[18].ToString())
                    {
                        var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\r\n", row[0], row[10], row[18], row[1], row[2], row[3], row[6], "2.1G");
                        sb.Append(line);
                    }
                }
            }
            if (GetDataClass.L2_6G != null && GetDataClass.L2_6G.Tables.Count > 0)
            {
                foreach (DataRow row in GetDataClass.L800M.Tables[0].Rows)
                {
                    if (city == "全省" || city == row[18].ToString())
                    {
                        var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\r\n", row[0], row[10], row[18], row[1], row[2], row[3], row[6], "2.6G");
                        sb.Append(line);
                    }
                }
            }
            var data = Encoding.GetEncoding("gbk").GetBytes(sb.ToString());
            return File(data, "application/octet-stream", city + "-监控数据.csv");
        }
    }
}
