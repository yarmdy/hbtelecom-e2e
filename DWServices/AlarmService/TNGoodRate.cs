using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace AlarmService
{
    public static class TNGoodRate
    {
        /// <summary>
        /// 这个方法用来初始化内存，读取三网数据到datatable或者字典来存储，供生成热力图提供数据，从csv读取来的信息存起来，这个方法就是干这个的，用static成员变量来存储
        /// 获取的数据要根据纬度、经度，正序排序，方便二分法查找数据。
        /// 做的时候一定要当数据已经存在了，从文件读取
        /// </summary>
        /// 
        static Dictionary<int, Dictionary<string, Dictionary<int, DataTable>>> gridData = new Dictionary<int, Dictionary<string, Dictionary<int, DataTable>>>();

        static Dictionary<int, Dictionary<string, Dictionary<int, DataTable>>> gridData2 = new Dictionary<int, Dictionary<string, Dictionary<int, DataTable>>>();
        //public static void LoadData() {
        //    for(int i=0;i<3;i++)
        //    {
        //        gridData[i] = new Dictionary<int, DataTable>();
        //    }
        //    DataTable table1, table2, table3, table4, table5, table6, table7, table8, table9;
        //    DataTable[] dxtables = GeTable(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate\\电信"));
        //    table1 = dxtables[0];
        //    table2 = dxtables[1];
        //    table3 = dxtables[2];
        //    gridData[0][0] = table1;
        //    gridData[0][1] = table2;
        //    gridData[0][2] = table3;
        //    DataTable[] ydtables = GeTable(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate\\移动"));
        //    table4 = ydtables[0];
        //    table5 = ydtables[1];
        //    table6 = ydtables[2];
        //    gridData[1][0] = table4;
        //    gridData[1][1] = table5;
        //    gridData[1][2] = table6;
        //    DataTable[] lttables = GeTable(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate\\联通"));
        //    table7 = lttables[0];
        //    table8 = lttables[1];
        //    table9 = lttables[2];
        //    gridData[2][0] = table7;
        //    gridData[2][1] = table8;
        //    gridData[2][2] = table9;
        //}


        public static void LoadData()
        {
            for (int i = 1; i < 4; i++)
            {
                gridData[i] = new Dictionary<string, Dictionary<int, DataTable>>();
                gridData[i]["dx"] = new Dictionary<int, DataTable>();
                gridData[i]["yd"] = new Dictionary<int, DataTable>();
                gridData[i]["lt"] = new Dictionary<int, DataTable>();

                DataTable dtt0, dtt1, dtt2, dtt3, dtt4, dtt5, dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate\\" + i + "\\电信"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                gridData[i]["dx"][0] = dtt0;
                gridData[i]["dx"][1] = dtt1;
                gridData[i]["dx"][2] = dtt2;
                gridData[i]["dx"][3] = dtt3;
                gridData[i]["dx"][4] = dtt4;
                gridData[i]["dx"][5] = dtt5;
                gridData[i]["dx"][6] = dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate\\" + i + "\\移动"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                gridData[i]["yd"][0] = dtt0;
                gridData[i]["yd"][1] = dtt1;
                gridData[i]["yd"][2] = dtt2;
                gridData[i]["yd"][3] = dtt3;
                gridData[i]["yd"][4] = dtt4;
                gridData[i]["yd"][5] = dtt5;
                gridData[i]["yd"][6] = dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate\\" + i + "\\联通"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                gridData[i]["lt"][0] = dtt0;
                gridData[i]["lt"][1] = dtt1;
                gridData[i]["lt"][2] = dtt2;
                gridData[i]["lt"][3] = dtt3;
                gridData[i]["lt"][4] = dtt4;
                gridData[i]["lt"][5] = dtt5;
                gridData[i]["lt"][6] = dtt6;
            }

            for (int i = 1; i < 4; i++)
            {
                gridData2[i] = new Dictionary<string, Dictionary<int, DataTable>>();
                gridData2[i]["dx"] = new Dictionary<int, DataTable>();
                gridData2[i]["yd"] = new Dictionary<int, DataTable>();
                gridData2[i]["lt"] = new Dictionary<int, DataTable>();

                DataTable dtt0, dtt1, dtt2, dtt3, dtt4, dtt5, dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate3h\\" + i + "\\电信"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                gridData2[i]["dx"][0] = dtt0;
                gridData2[i]["dx"][1] = dtt1;
                gridData2[i]["dx"][2] = dtt2;
                gridData2[i]["dx"][3] = dtt3;
                gridData2[i]["dx"][4] = dtt4;
                gridData2[i]["dx"][5] = dtt5;
                gridData2[i]["dx"][6] = dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate3h\\" + i + "\\移动"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                gridData2[i]["yd"][0] = dtt0;
                gridData2[i]["yd"][1] = dtt1;
                gridData2[i]["yd"][2] = dtt2;
                gridData2[i]["yd"][3] = dtt3;
                gridData2[i]["yd"][4] = dtt4;
                gridData2[i]["yd"][5] = dtt5;
                gridData2[i]["yd"][6] = dtt6;
                Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "goodrate3h\\" + i + "\\联通"), out dtt0, out dtt1, out dtt2, out dtt3, out dtt4, out dtt5, out dtt6);
                gridData2[i]["lt"][0] = dtt0;
                gridData2[i]["lt"][1] = dtt1;
                gridData2[i]["lt"][2] = dtt2;
                gridData2[i]["lt"][3] = dtt3;
                gridData2[i]["lt"][4] = dtt4;
                gridData2[i]["lt"][5] = dtt5;
                gridData2[i]["lt"][6] = dtt6;
            }
        }


        static void Getnb3w(string path, out DataTable dt, out DataTable dt1, out DataTable dt2, out DataTable dt3, out DataTable dt4, out DataTable dt5, out DataTable dt6)
        {
            if (!Directory.Exists(path))
            {
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
                    if (cols.Length < 3 || cols[0].ToString() == "" || cols[1].ToString() == "" || cols[2].ToString() == "") continue;
                    var lon = float.Parse(cols[0]);
                    var lat = float.Parse(cols[1]);
                    var rsrp = float.Parse(cols[2])/100;
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

        static DataTable[] GeTable(string path)
        {

            DataTable table1 = null;
            DataTable table2 = null;
            DataTable table3 = null;
            if (!Directory.Exists(path))
            {
                return new DataTable[] { table1, table2, table3 };
            }
            var files = Directory.GetFiles(path, "*.csv");
            
            foreach (var file in files)
            {
                Dictionary<string, Dictionary<int, double>> dics = new Dictionary<string, Dictionary<int, double>>();
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
                    if(cols.Length < 4 ||cols[1].ToString() == "" || cols[2].ToString() == "" || cols[3].ToString() == "")
                    {
                        continue;
                    }
                    var lon = float.Parse(cols[1]);
                    var lat = float.Parse(cols[2]);
                    var rate = float.Parse(cols[3]) / 100;
                    var key = cols[0].ToString();
                    if (!dics.ContainsKey(key))
                    {
                        dics[key] = new Dictionary<int, double>();
                        dics[key][0] = lon;
                        dics[key][1] = lat;
                        dics[key][2] = rate;
                    }
                }
                sr.Close();
                sr.Dispose();
                //dics = dics.OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
                dics = dics.OrderBy(a => (double)a.Value[1] * 1000000 + (double)a.Value[2]).ToDictionary(a => a.Key, a => a.Value);
                if (file.Contains("一季度"))
                {
                    table1 = new DataTable();
                    table1.Columns.Add("sc_lon", typeof(decimal));
                    table1.Columns.Add("sc_lat", typeof(decimal));
                    table1.Columns.Add("rate", typeof(decimal));
                    foreach (var dic in dics)
                    {
                        var dr = table1.NewRow();
                        dr[0] = dic.Value[0];
                        dr[1] = dic.Value[1];
                        dr[2] = dic.Value[2];
                        table1.Rows.Add(dr);
                    }
                    table1.AcceptChanges();
                }
                else if (file.Contains("二季度"))
                {
                    table2 = new DataTable();
                    table2.Columns.Add("sc_lon", typeof(decimal));
                    table2.Columns.Add("sc_lat", typeof(decimal));
                    table2.Columns.Add("rate", typeof(decimal));
                    foreach (var dic in dics)
                    {
                        var dr = table2.NewRow();
                        dr[0] = dic.Value[0];
                        dr[1] = dic.Value[1];
                        dr[2] = dic.Value[2];
                        table2.Rows.Add(dr);
                    }
                    table2.AcceptChanges();
                }
                else
                {
                    table3 = new DataTable();
                    table3.Columns.Add("sc_lon", typeof(decimal));
                    table3.Columns.Add("sc_lat", typeof(decimal));
                    table3.Columns.Add("rate", typeof(decimal));
                    foreach (var dic in dics)
                    {
                        var dr = table3.NewRow();
                        dr[0] = dic.Value[0];
                        dr[1] = dic.Value[1];
                        dr[2] = dic.Value[2];
                        table3.Rows.Add(dr);
                    }
                    table3.AcceptChanges();
                }
            }
            return new DataTable[] { table1, table2, table3 };
        }
        /// <summary>
        /// 这个方法主要是为了处理发来的请求，返回热力图png数据下面方法做了一些前期工作
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        public static byte[] GetTNGoodData(req rq)
        {
            //这里的方法都是固定写法，计算发来的参数，然后把经纬度跟png上的像素坐标相对应，生成覆盖在gis上的png图片，使图片正好和gis坐标对应。
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

            var intSW = rq.ACT / 10-3;//获取电信(1)、移动(2)、联通(3)，根据获取的值选择相应的datatable
            var intJD = rq.ACT % 10;//获取季度数，根据季度数选择相应的datatable来做筛选

            //下面是热力图算法

            //根据二分法算出所有要画的数据、参考form1 944行开始


            //生成热力图，参考form1 1035行开始

            //{
            //    #region 索引寻址
            //    var lastLat = lat1;

            //    var minInd = 0;

            //    DataTable nbdt = null;
            //    nbdt = voltedata[intSW - 1][intJD - 1];
            //    var maxInd = nbdt.Rows.Count - 1;

            //    var tLen = maxInd;
            //    DateTime date1 = DateTime.Now;
            //    if (lat1 <= (double)(decimal)nbdt.Rows[maxInd]["sc_lat"] && lat2 >= (double)(decimal)nbdt.Rows[minInd]["sc_lat"])
            //    {

            //        while (lastLat < lat2)
            //        {
            //            var minR = nbdt.Rows[minInd];
            //            var maxR = nbdt.Rows[maxInd];
            //            if (!(minInd == 0 && lastLat <= (double)(decimal)minR[1] || minInd > 0 && lastLat < (double)(decimal)minR[1] && lastLat >= (double)(decimal)nbdt.Rows[minInd - 1][1]))
            //            {
            //                var midInd = (maxInd - minInd) / 2 + minInd;
            //                var midR = nbdt.Rows[midInd];
            //                if (lastLat < (double)(decimal)midR[1])
            //                {
            //                    maxInd = midInd;
            //                    if (maxInd - minInd == 1)
            //                    {
            //                        minInd++;
            //                    }
            //                }
            //                else
            //                {
            //                    minInd = midInd;
            //                    if (maxInd == minInd)
            //                        break;
            //                    if (maxInd - minInd == 1)
            //                    {
            //                        minInd++;
            //                    }

            //                }
            //            }
            //            else
            //            {
            //                var finish = false;
            //                for (int i = minInd; i < tLen + 1; i++)
            //                {
            //                    var dr2 = nbdt.Rows[i];
            //                    double lon = (double)((decimal)dr2[0]);
            //                    double lat = (double)((decimal)dr2[1]);
            //                    double rate = (double)((decimal)dr2[2]);
            //                    lastLat = lat;
            //                    if (lat < lat1 || lat > lat2)
            //                    {
            //                        finish = true;
            //                        break;
            //                    }
            //                    if (lon < lon1)
            //                    {
            //                        continue;
            //                    }
            //                    if (lon > lon2)
            //                    {
            //                        minInd = i;
            //                        maxInd = tLen;
            //                        break;
            //                    }
            //                    var xc = ((int)(float)((lon - lon1) * wb)) / 10;
            //                    var yc = ((int)(rq.HEIGHT - (float)((lat - lat1) * hb))) / 10;
            //                    if (dic.ContainsKey(xc * 1000000 + yc))
            //                    {
            //                        if (dr2[2] == DBNull.Value)
            //                        {
            //                        }
            //                        else
            //                        {
            //                            dic[xc * 1000000 + yc] = rate < dic[xc * 1000000 + yc] ? rate : dic[xc * 1000000 + yc];
            //                        }

            //                    }
            //                    else
            //                    {
            //                        if (dr2[2] == DBNull.Value) { }
            //                        else
            //                        {
            //                            dic[xc * 1000000 + yc] = rate;
            //                        }

            //                    }
            //                    if (i == tLen)
            //                    {
            //                        finish = true;
            //                    }
            //                }
            //                if (finish)
            //                {
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //    #endregion
            //    output = new Bitmap(rq.WIDTH, rq.HEIGHT, PixelFormat.Format32bppArgb);
            //    GraphicsPath pt = new GraphicsPath();
            //    int rad = 12;
            //    var x1 = 0.0002;
            //    var x2 = 0.2197;
            //    var y1 = 10.0;
            //    var y2 = 1000.0;

            //    var a = (y2 - y1) / (x2 - y1);
            //    var b = y1 - a * x1;
            //    var maxV = 1.0;

            //    foreach (var d in dic)
            //    {
            //        var x10 = d.Key / 1000000;
            //        var y10 = d.Key % 1000000;
            //        Rectangle r = new Rectangle(x10 * 10 - rad, y10 * 10 - rad, rad * 2, rad * 2);


            //        var bili = d.Value / maxV;
            //        if (bili <= 0)
            //        {
            //            bili = 0.1;
            //        }

            //        var ellipsePath = new GraphicsPath();
            //        ellipsePath.AddEllipse(r);
            //        PathGradientBrush br = new PathGradientBrush(ellipsePath);
            //        ColorBlend gradientSpecifications = GetColorBlend(bili);
            //        br.InterpolationColors = gradientSpecifications;
            //        ig.FillEllipse(br, r);
            //    }
            //    Rectangle rectbit = new Rectangle(0, 0, rq.WIDTH, rq.HEIGHT);
            //    BitmapData imgData = image.LockBits(rectbit, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //    BitmapData outData = output.LockBits(rectbit, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            //    var ptrimg = imgData.Scan0;
            //    var ptrout = outData.Scan0;
            //    var imglen = imgData.Stride * rq.HEIGHT;
            //    var imgdata = new byte[imglen];
            //    var outlen = outData.Stride * rq.HEIGHT;
            //    var outdata = new byte[outlen];
            //    System.Runtime.InteropServices.Marshal.Copy(ptrimg, imgdata, 0, imglen);
            //    System.Runtime.InteropServices.Marshal.Copy(ptrout, outdata, 0, outlen);
            //    for (int y = 0; y < rq.HEIGHT; y++)
            //    {
            //        for (int x = 0; x < rq.WIDTH; x++)
            //        {
            //            int ind = x * 4;
            //            var alp = imgdata[y * imgData.Stride + ind + 3];
            //            if (alp == 0) continue;

            //            var outcolor = GetARGBBrushC(alp / 255f);
            //            outdata[y * outData.Stride + ind] = outcolor.B;
            //            outdata[y * outData.Stride + ind + 1] = outcolor.G;
            //            outdata[y * outData.Stride + ind + 2] = outcolor.R;
            //            outdata[y * outData.Stride + ind + 3] = outcolor.A;
            //        }
            //    }
            //    //SetPixBitmap(imgdata, outdata, rq.WIDTH,rq.HEIGHT);
            //    System.Runtime.InteropServices.Marshal.Copy(outdata, 0, ptrout, outlen);
            //    image.UnlockBits(imgData);
            //    output.UnlockBits(outData);
            //}
            //MemoryStream stream = new MemoryStream();


            //output.Save(stream, ImageFormat.Png);
            //image.Save(stream, ImageFormat.Png);
            //data = stream.ToArray();
            //return data;
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
                if (rq.ACT / 10 == 4)
                {
                    nbdt = GetTableDX(tablei, rq.ACT % 10);
                }
                else if (rq.ACT / 10 == 5)
                {
                    nbdt = GetTableYD(tablei, rq.ACT % 10);
                }
                else if (rq.ACT / 10 == 6)
                {
                    nbdt = GetTableLT(tablei, rq.ACT % 10);
                }
                else if (rq.ACT / 10 == 7)
                {
                    nbdt = GetTableDX(tablei, rq.ACT % 10,true);
                }
                else if (rq.ACT / 10 == 8)
                {
                    nbdt = GetTableYD(tablei, rq.ACT % 10,true);
                }
                else if (rq.ACT / 10 == 9)
                {
                    nbdt = GetTableLT(tablei, rq.ACT % 10,true);
                }
                var maxInd = nbdt.Rows.Count - 1;

                var tLen = maxInd;
                if (lat1 <= (double)(float)nbdt.Rows[maxInd]["sc_lat"] && lat2 >= (double)(float)nbdt.Rows[minInd]["sc_lat"])
                {

                    while (lastLat < lat2)
                    {
                        var minR = nbdt.Rows[minInd];
                        var maxR = nbdt.Rows[maxInd];
                        if (!(minInd == 0 && lastLat <= (double)(float)minR[1] || minInd > 0 && lastLat < (double)(float)minR[1] && lastLat >= (double)(float)nbdt.Rows[minInd - 1][1]))
                        {
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
                        }
                        else
                        {
                            var finish = false;
                            for (int i = minInd; i < tLen + 1; i++)
                            {
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
                                if (i == tLen)
                                {
                                    finish = true;
                                }
                            }
                            if (finish)
                            {
                                break;
                            }
                        }
                    }
                }
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
                MemoryStream stream1 = new MemoryStream();
                image.Save(stream1, ImageFormat.Png);
                data = stream1.ToArray();
            }

            return data;
        }


        private static Color getNBColor(double v)
        {
            if (v >= 0.9)
            {
                return Color.FromArgb(96, 0, 255, 0);
            }
            if (v >=0.8)
            {
                return Color.FromArgb(96, 0, 0, 255);
            }
            if (v >=0.7)
            {
                return Color.FromArgb(96, 255, 255, 0);
            }
            if (v >= 0.6)
            {
                return Color.FromArgb(96, 255, 128, 0);
            }
            if (v >= 0.5)
            {
                return Color.FromArgb(96, 255, 0, 0);
            }
            return Color.Red;
        }

        static ColorBlend GetColorBlend(double bili)
        {
            bili = bili > 1 ? 1 : bili;
            ColorBlend colors = new ColorBlend(3);

            colors.Positions = new float[3] { 0, 0.6f, 1 };
            colors.Colors = new Color[3]
            {
                Color.FromArgb(10, Color.Black),
                Color.FromArgb((int)(bili*245*0.5)+10, Color.Black),
                Color.FromArgb((int)(bili*245)+10, Color.Black)
            };
            return colors;
        }

        static Color GetARGBBrushC(double bili)
        {
            var tmmax = 200;
            var bbb = 66;
            var aaa = tmmax - bbb;
            var tmr = aaa * bili + bbb;

            if (bili <= 0.25)
            {
                //return Color.FromArgb((int)(tmr), Color.Green);
                return Color.FromArgb((int)(tmr), 255, 255 - (int)(bili  / 0.25 * 255), 0);
            }
            if (bili <= 0.5)
            {
                //return Color.FromArgb((int)(tmr), 0, (int)(255 - (bili - 0.25) / 0.25 * 255), (int)((bili - 0.25) / 0.25 * 127 + 128));
                return Color.FromArgb((int)(tmr), (int)((bili - 0.25) / 0.25 * 255), (int)(255 - (bili - 0.25) / 0.25 * 255), (int)((bili - 0.25) / 0.25 * 127 + 128));
            }
            if (bili <= 0.75)
            {
                //return Color.FromArgb((int)(tmr), (int)((bili - 0.5) / 0.25 * 255), (int)((bili - 0.5) / 0.25 * 255), (int)(255 - (bili - 0.5) / 0.25 * 255));
                return Color.FromArgb((int)(tmr), 0, (int)((bili - 0.5) / 0.25 * 255), (int)(255 - (bili - 0.5) / 0.25 * 255));
            }
            if (bili <= 1)
            {
                return Color.FromArgb((int)(tmr), Color.Green);
                //return Color.FromArgb((int)(tmr), 255, 255 - (int)((bili - 0.75) / 0.25 * 255), 0);
            }
            return Color.FromArgb((int)(255), Color.Red);
        }


        public static DataTable GetTableDX(int i, int j)
        {
            return gridData[j]["dx"][i];
        }
        public static DataTable GetTableYD(int i, int j)
        {
            return gridData[j]["yd"][i];
        }
        public static DataTable GetTableLT(int i, int j)
        {
            return gridData[j]["lt"][i];
        }

        public static DataTable GetTableDX(int i, int j,bool b)
        {
            return gridData2[j]["dx"][i];
        }
        public static DataTable GetTableYD(int i, int j, bool b)
        {
            return gridData2[j]["yd"][i];
        }
        public static DataTable GetTableLT(int i, int j, bool b)
        {
            return gridData2[j]["lt"][i];
        }
    }
}
