using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace AlarmService
{
    public static class GetDataClass
    {
        static Dictionary<int, Dictionary<string, Dictionary<int, DataTable>>> wxdata = new Dictionary<int, Dictionary<string, Dictionary<int, DataTable>>>();

        public static byte[] GetWxfgdata(req rq)
        {
            var dt1 = DateTime.Now;
            byte[] data = null;
            var lon1 = rq.BBOXx;
            var lat1 = rq.BBOXy;
            var lon2 = rq.BBOXx2;
            var lat2 = rq.BBOXy2;
            if (rq.CRS == 1)
            {
                lon1 = lon1 / 20037508.34 * 180;
                lat1 = lat1 / 20037508.34 * 180;
                lat1 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat1 * Math.PI / 180)) - Math.PI / 2);

                lon2 = lon2 / 20037508.34 * 180;
                lat2 = lat2 / 20037508.34 * 180;
                lat2 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat2 * Math.PI / 180)) - Math.PI / 2);
            }
            else
            {
                lon1 = rq.BBOXy;
                lat1 = rq.BBOXx;
                lon2 = rq.BBOXy2;
                lat2 = rq.BBOXx2;
            }
            var w = lon2 - lon1;//经度跨度
            var h = lat2 - lat1;//纬度跨度
            var wb = rq.WIDTH / w;//像素经度
            var hb = rq.HEIGHT / h;//像素纬度
            Rectangle rect = new Rectangle(0, 0, rq.WIDTH, rq.HEIGHT);//矩形位置和大小
            Bitmap image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
            Graphics ig = Graphics.FromImage(image);
            var wb2 = w / rq.WIDTH;
            var hb2 = h / rq.HEIGHT;
            Dictionary<int, double> dic = new Dictionary<int, double>();
            Dictionary<int, int> dic2 = new Dictionary<int, int>();

            Bitmap output = null;

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

                #region 索引寻址
                var lastLat = lat1;

                var minInd = 0;

                DataTable nbdt = null;
                if (rq.ACT / 10 == 1) {
                    nbdt = GetTableNBRSRPDX(tablei, rq.ACT % 10);
                }
                else if (rq.ACT / 10 == 2) {
                    nbdt = GetTableNBRSRPYD(tablei, rq.ACT % 10);
                }
                else if (rq.ACT / 10 == 3) {
                    nbdt = GetTableNBRSRPLT(tablei, rq.ACT % 10);
                }
                var maxInd = nbdt.Rows.Count - 1;

                var tLen = maxInd;
                int numi = 0;
                int numj = 0;
                int numk = 0;
                double numg = 0;
                double numo = 0;
                double numy = 0;
                DateTime date1 = DateTime.Now;
                if (lat1 <= (double)(float)nbdt.Rows[maxInd]["sc_lat"] && lat2 >= (double)(float)nbdt.Rows[minInd]["sc_lat"])
                {

                    while (lastLat < lat2)
                    {
                        numi++;
                        var minR = nbdt.Rows[minInd];
                        var maxR = nbdt.Rows[maxInd];
                        if (!(minInd == 0 && lastLat <= (double)(float)minR[1] || minInd > 0 && lastLat < (double)(float)minR[1] && lastLat >= (double)(float)nbdt.Rows[minInd - 1][1]))
                        {
                            DateTime date2 = DateTime.Now;
                            numk++;
                            var midInd = (maxInd - minInd) / 2 + minInd;
                            var midR = nbdt.Rows[midInd];
                            if (lastLat < (double)(float)midR[1])
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
                            numg += (DateTime.Now - date2).TotalMilliseconds;
                        }
                        else
                        {
                            var finish = false;
                            DateTime date3 = DateTime.Now;
                            for (int i = minInd; i < tLen + 1; i++)
                            {
                                numj++;
                                var dr2 = nbdt.Rows[i];
                                double lon = (double)((float)dr2[0]);
                                double lat = (double)((float)dr2[1]);
                                double rsrp = (double)((float)dr2[2]);
                                lastLat = lat;
                                if (lat < lat1 || lat > lat2)
                                {
                                    finish = true;
                                    break;
                                }
                                if (lon < lon1)
                                {
                                    continue;
                                }
                                if (lon > lon2)
                                {
                                    minInd = i;
                                    maxInd = tLen;
                                    break;
                                }
                                var xc = ((int)(float)((lon - lon1) * wb));
                                var yc = ((int)(rq.HEIGHT - (float)((lat - lat1) * hb)));
                                DateTime date4 = DateTime.Now;
                                if (dic.ContainsKey(xc * 1000000 + yc))
                                {
                                    if (dr2[2] == DBNull.Value)
                                    {
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
                                numy += (DateTime.Now - date4).TotalMilliseconds;
                                if (i == tLen)
                                {
                                    finish = true;
                                }
                            }
                            if (finish)
                            {
                                numo += (DateTime.Now - date3).TotalMilliseconds;
                                break;
                            }
                            numo += (DateTime.Now - date3).TotalMilliseconds;
                        }
                    }
                }
                date1 = DateTime.Now;
                #endregion
                foreach (var d in dic)
                {
                    var x10 = d.Key / 1000000;
                    var y10 = d.Key % 1000000;
                    var latl = (rq.HEIGHT - y10) * hb2 + lat1;
                    var m50plon = 0.05 / ((40076 / 360.0) * Math.Cos(latl / 180 * Math.PI)) / wb2;
                    var m50plat = 0.05 / (40009 / 360.0) / hb2;
                    var m50plonI = m50plon < 1 ? 1 : (int)m50plon + (m50plon - (int)m50plon >= 0.5 ? 1 : 0);
                    var m50platI = m50plat < 1 ? 1 : (int)m50plat + (m50plat - (int)m50plat >= 0.5 ? 1 : 0);
                    Rectangle r = new Rectangle(x10 - m50plonI / 2, y10 - m50platI / 2, m50plonI, m50platI);
                    Brush bpix = new SolidBrush(getNBColor(d.Value));
                    ig.FillRectangle(bpix, r);
                }
                System.Diagnostics.Debug.WriteLine((DateTime.Now - date1).TotalMilliseconds);
                MemoryStream stream1 = new MemoryStream();
                image.Save(stream1, ImageFormat.Png);
                data = stream1.ToArray();
            }
            
            return data;
        }
        private static Color getNBColor(double v)
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
        public static void LoadData(){
            for (int i = 1; i < 4; i++) {
                wxdata[i] = new Dictionary<string, Dictionary<int, DataTable>>();
                wxdata[i]["dx"] = new Dictionary<int, DataTable>();
                wxdata[i]["yd"] = new Dictionary<int, DataTable>();
                wxdata[i]["lt"] = new Dictionary<int, DataTable>();

                DataTable dtt0, dtt1, dtt2, dtt3, dtt4, dtt5, dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "wxdata\\" + i + "\\电信"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                wxdata[i]["dx"][0] = dtt0;
                wxdata[i]["dx"][1] = dtt1;
                wxdata[i]["dx"][2] = dtt2;
                wxdata[i]["dx"][3] = dtt3;
                wxdata[i]["dx"][4] = dtt4;
                wxdata[i]["dx"][5] = dtt5;
                wxdata[i]["dx"][6] = dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "wxdata\\" + i + "\\移动"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                wxdata[i]["yd"][0] = dtt0;
                wxdata[i]["yd"][1] = dtt1;
                wxdata[i]["yd"][2] = dtt2;
                wxdata[i]["yd"][3] = dtt3;
                wxdata[i]["yd"][4] = dtt4;
                wxdata[i]["yd"][5] = dtt5;
                wxdata[i]["yd"][6] = dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "wxdata\\" + i + "\\联通"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                wxdata[i]["lt"][0] = dtt0;
                wxdata[i]["lt"][1] = dtt1;
                wxdata[i]["lt"][2] = dtt2;
                wxdata[i]["lt"][3] = dtt3;
                wxdata[i]["lt"][4] = dtt4;
                wxdata[i]["lt"][5] = dtt5;
                wxdata[i]["lt"][6] = dtt6;
            }
        }
        public static DataTable GetTableNBRSRPDX(int i,int j)
        {
            return wxdata[j]["dx"][i];
        }
        public static DataTable GetTableNBRSRPYD(int i,int j)
        {
            return wxdata[j]["yd"][i];
        }
        public static DataTable GetTableNBRSRPLT(int i,int j)
        {
            return wxdata[j]["lt"][i];
        }
        static void Getnb3w(string path, out DataTable dt, out DataTable dt1, out DataTable dt2, out DataTable dt3, out DataTable dt4, out DataTable dt5, out DataTable dt6)
        {
            if (!Directory.Exists(path)) {
                dt = null;
                dt1 = null;
                dt2 = null;
                dt3 = null;
                dt4 = null;
                dt5 = null;
                dt6 = null;
                return; 
            }
            var files = Directory.GetFiles(path, "*.csv");
            Dictionary<double, Dictionary<int, object>> dics = new Dictionary<double, Dictionary<int, object>>();
            int count = 0;
            foreach (var file in files)
            {
                //if (count++ > 0) break;
                StreamReader sr = new StreamReader(file, Encoding.GetEncoding("gbk"));
                bool bfirst = true;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (bfirst)
                    {
                        bfirst = false;
                        continue;
                    }
                    var cols = line.Split(new string[] { "," }, StringSplitOptions.None);
                    //var city = cols[0].Replace("\"", "");
                    var lon = float.Parse(cols[1]);
                    var lat = float.Parse(cols[2]);
                    var rsrp = float.Parse(cols[3]);
                    var key = (double)lat * 1000000 + (double)lon;
                    if (!dics.ContainsKey(key))
                    {
                        dics[key] = new Dictionary<int, object>();
                        //dics[key][0] = city;
                        dics[key][0] = lon;
                        dics[key][1] = lat;
                        dics[key][2] = rsrp;
                    }
                }
                sr.Close();
                sr.Dispose();
            }
            dics = dics.OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            dt = new DataTable();
            dt.Columns.Add("sc_lon", typeof(float));
            dt.Columns.Add("sc_lat", typeof(float));
            dt.Columns.Add("rsrp", typeof(float));
            foreach (var dic in dics)
            {
                var dr = dt.NewRow();
                dr[0] = dic.Value[0];
                dr[1] = dic.Value[1];
                dr[2] = dic.Value[2];
                dt.Rows.Add(dr);
            }
            dt.AcceptChanges();
            var dics1 = dics.GroupBy(a => ((int)((float)a.Value[0] / 0.014276766034981675)) * 1000000 + ((int)((float)a.Value[1] / 0.014276766034981674))).ToDictionary(
                a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    return (double)lat * 1000000 + lon;
                }, a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    var rsrp = (float)a.Average(b => (float)b.Value[2]);
                    return new Dictionary<int, object>() { { 0, lon }, { 1, lat }, { 2, rsrp } };
                }
                ).OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            dt1 = new DataTable();
            dt1.Columns.Add("sc_lon", typeof(float));
            dt1.Columns.Add("sc_lat", typeof(float));
            dt1.Columns.Add("rsrp", typeof(float));
            foreach (var dic in dics1)
            {
                var dr = dt1.NewRow();
                dr[0] = dic.Value[0];
                dr[1] = dic.Value[1];
                dr[2] = dic.Value[2];
                dt1.Rows.Add(dr);
            }
            dt1.AcceptChanges();
            dics1.Clear();
            var dics2 = dics.GroupBy(a => ((int)((float)a.Value[0] / 0.009517844023321129)) * 1000000 + ((int)((float)a.Value[1] / 0.0095178440233211325))).ToDictionary(
                a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    return (double)lat * 1000000 + lon;
                }, a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    var rsrp = (float)a.Average(b => (float)b.Value[2]);
                    return new Dictionary<int, object>() { { 0, lon }, { 1, lat }, { 2, rsrp } };
                }
                ).OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            dt2 = new DataTable();
            dt2.Columns.Add("sc_lon", typeof(float));
            dt2.Columns.Add("sc_lat", typeof(float));
            dt2.Columns.Add("rsrp", typeof(float));
            foreach (var dic in dics2)
            {
                var dr = dt2.NewRow();
                dr[0] = dic.Value[0];
                dr[1] = dic.Value[1];
                dr[2] = dic.Value[2];
                dt2.Rows.Add(dr);
            }
            dt2.AcceptChanges();
            dics2.Clear();
            var dics3 = dics.GroupBy(a => ((int)((float)a.Value[0] / 0.0047589220116605472)) * 1000000 + ((int)((float)a.Value[1] / 0.0047589220116605662))).ToDictionary(
                a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    return (double)lat * 1000000 + lon;
                }, a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    var rsrp = (float)a.Average(b => (float)b.Value[2]);
                    return new Dictionary<int, object>() { { 0, lon }, { 1, lat }, { 2, rsrp } };
                }
                ).OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            dt3 = new DataTable();
            dt3.Columns.Add("sc_lon", typeof(float));
            dt3.Columns.Add("sc_lat", typeof(float));
            dt3.Columns.Add("rsrp", typeof(float));
            foreach (var dic in dics3)
            {
                var dr = dt3.NewRow();
                dr[0] = dic.Value[0];
                dr[1] = dic.Value[1];
                dr[2] = dic.Value[2];
                dt3.Rows.Add(dr);
            }
            dt3.AcceptChanges();
            dics3.Clear();
            var dics4 = dics.GroupBy(a => ((int)((float)a.Value[0] / 0.0023794610058302736)) * 1000000 + ((int)((float)a.Value[1] / 0.0023794610058302831))).ToDictionary(
                a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    return (double)lat * 1000000 + lon;
                }, a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    var rsrp = (float)a.Average(b => (float)b.Value[2]);
                    return new Dictionary<int, object>() { { 0, lon }, { 1, lat }, { 2, rsrp } };
                }
                ).OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            dt4 = new DataTable();
            dt4.Columns.Add("sc_lon", typeof(float));
            dt4.Columns.Add("sc_lat", typeof(float));
            dt4.Columns.Add("rsrp", typeof(float));
            foreach (var dic in dics4)
            {
                var dr = dt4.NewRow();
                dr[0] = dic.Value[0];
                dr[1] = dic.Value[1];
                dr[2] = dic.Value[2];
                dt4.Rows.Add(dr);
            }
            dt4.AcceptChanges();
            dics4.Clear();
            var dics5 = dics.GroupBy(a => ((int)((float)a.Value[0] / 0.0011897305029151539)) * 1000000 + ((int)((float)a.Value[1] / 0.0011897305029151292))).ToDictionary(
                a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    return (double)lat * 1000000 + lon;
                }, a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    var rsrp = (float)a.Average(b => (float)b.Value[2]);
                    return new Dictionary<int, object>() { { 0, lon }, { 1, lat }, { 2, rsrp } };
                }
                ).OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            dt5 = new DataTable();
            dt5.Columns.Add("sc_lon", typeof(float));
            dt5.Columns.Add("sc_lat", typeof(float));
            dt5.Columns.Add("rsrp", typeof(float));
            foreach (var dic in dics5)
            {
                var dr = dt5.NewRow();
                dr[0] = dic.Value[0];
                dr[1] = dic.Value[1];
                dr[2] = dic.Value[2];
                dt5.Rows.Add(dr);
            }
            dt5.AcceptChanges();
            dics5.Clear();

            var dics6 = dics.GroupBy(a => ((int)((float)a.Value[0] / 0.000594865251457577)) * 1000000 + ((int)((float)a.Value[1] / 0.000594865251457577))).ToDictionary(
                a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    return (double)lat * 1000000 + lon;
                }, a =>
                {
                    var lat = (float)a.Max(b => (float)b.Value[1]);
                    var lon = (float)a.Max(b => (float)b.Value[0]);
                    var rsrp = (float)a.Average(b => (float)b.Value[2]);
                    return new Dictionary<int, object>() { { 0, lon }, { 1, lat }, { 2, rsrp } };
                }
                ).OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            dics.Clear();
            dt6 = new DataTable();
            dt6.Columns.Add("sc_lon", typeof(float));
            dt6.Columns.Add("sc_lat", typeof(float));
            dt6.Columns.Add("rsrp", typeof(float));
            foreach (var dic in dics6)
            {
                var dr = dt6.NewRow();
                dr[0] = dic.Value[0];
                dr[1] = dic.Value[1];
                dr[2] = dic.Value[2];
                dt6.Rows.Add(dr);
            }
            dt6.AcceptChanges();
        }
    }
}
