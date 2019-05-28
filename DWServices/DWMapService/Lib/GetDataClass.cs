using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using DWMapService.Controllers;
using System.Threading;
using DWMapService.Models;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Text;

namespace DWMapService
{
    public static class GetDataClass
    {
        private static DataSet _ds = null;
        public static DataSet ds
        {
            get
            {
                return _ds;
            }
        }
        private static string _token = null;
        public static string token
        {
            get
            {
                return _token;
            }
        }
        public static void StartGetData()
        {
            Thread th = new Thread(new ThreadStart(GetData));
            th.Start();
            Thread th2 = new Thread(new ThreadStart(getToken));
            th2.Start();
        }
        public static void GetData()
        {
            LOG.WriteLog("getdata");
            var runtime = DateTime.Now.Date.AddDays(-1);
            bool isdone = false;
            while (true)
            {
                try
                {
                    var time = System.Configuration.ConfigurationManager.AppSettings["GetDataTime"];
                    int myhour = int.Parse(time.Split(':')[0]);
                    int myminute = int.Parse(time.Split(':')[1]);
                    int nowhour = DateTime.Now.Hour;
                    int nowminute = DateTime.Now.Minute;
                    //判断时间，执行日期是昨天说明今天没执行过
                    if (runtime < DateTime.Now.Date && myhour < nowhour && myminute < nowminute && !isdone)
                    {
                        getDataJXY();
                        getDataGCK();
                        getDataHSY();
                        

                        runtime = DateTime.Now.Date;

                        isdone = true;
                    }
                    if (runtime != DateTime.Now.Date)
                    {
                        isdone = false;
                    }

                }
                catch
                {
                }
                Thread.Sleep(1000);
            }
        }

        public static DataTable Get24ByEci(int eci)
        {
#if CESHI
            string sql = "select to_char(time,'hh24') hour,round(DOWN_PDCP,2) DOWN_PDCP,round(DOWN_PRB,2) DOWN_PRB from DATA_KPIINFO where eci = " + eci+" and time >=to_date('2017-06-12','yyyy-MM-dd') and time <to_date('2017-06-13','yyyy-MM-dd') order by time";
#else
            string sql = "select to_char(time,'hh24') hour,round(DOWN_PDCP,2) DOWN_PDCP,round(DOWN_PRB,2) DOWN_PRB from DATA_KPIINFO where  time >=trunc(sysdate-1) and time<trunc(sysdate) and eci = " + eci + " order by time";
#endif

            DataSet ds = DB.Query(sql);
            return ds.Tables[0];
        }
        public static DataSet CDMA = null;
        public static DataSet L800M = null;
        public static DataSet L1_8G = null;
        public static DataSet L2_1G = null;
        public static DataSet L2_6G = null;
        public static DataSet NB_IoT = null;
        public static DataSet CityFG = null;
        public static DataTable nbdt1 = null;
        public static DataTable nbdt2 = null;
        public static DataTable nbdt3 = null;
        public static DataTable nbdt4 = null;
        public static DataTable nbdt5 = null;
        public static DataTable nbdt6 = null;
        public static DataTable evtdt = null;
        public static DataTable evtGdt = null;

        public static DataTable nbdt1dx = null;
        public static DataTable nbdt2dx = null;
        public static DataTable nbdt3dx = null;
        public static DataTable nbdt4dx = null;
        public static DataTable nbdt5dx = null;
        public static DataTable nbdt6dx = null;
        public static DataTable nbdt7dx = null;

        public static DataTable nbdt1yd = null;
        public static DataTable nbdt2yd = null;
        public static DataTable nbdt3yd = null;
        public static DataTable nbdt4yd = null;
        public static DataTable nbdt5yd = null;
        public static DataTable nbdt6yd = null;
        public static DataTable nbdt7yd = null;

        public static DataTable nbdt1lt = null;
        public static DataTable nbdt2lt = null;
        public static DataTable nbdt3lt = null;
        public static DataTable nbdt4lt = null;
        public static DataTable nbdt5lt = null;
        public static DataTable nbdt6lt = null;
        public static DataTable nbdt7lt = null;

        public static DataSet GetDataSet(string station)
        {
            switch (station)
            {
                case "L800M": return L800M;
                case "L1.8G": return L1_8G;
                case "L2.1G": return L2_1G;
                case "L2.6G": return L2_6G;
                case "NB-IoT": return NB_IoT;
                case "CDMA": return CDMA;
                default: return null;
            }
        }
        public static DataTable GetTableNB(int i) {
            switch (i)
            {
                case 0:
                    return NB_IoT.Tables[0];
                    break;
                case 1:
                    return nbdt1;
                    break;
                case 2:
                    return nbdt2;
                    break;
                case 3:
                    return nbdt3;
                    break;
                case 4:
                    return nbdt4;
                    break;
                case 5:
                    return nbdt5;
                    break;
                case 6:
                    return nbdt6;
                    break;
            }
            return null;
        }

        public static DataTable GetTableNBRSRPDX(int i)
        {
            switch (i)
            {
                case 0:
                    return nbdt1dx;
                case 1:
                    return nbdt2dx;
                case 2:
                    return nbdt3dx;
                case 3:
                    return nbdt4dx;
                case 4:
                    return nbdt5dx;
                case 5:
                    return nbdt6dx;
                case 6:
                    return nbdt7dx;
            }
            return null;
        }
        public static DataTable GetTableNBRSRPYD(int i)
        {
            switch (i)
            {
                case 0:
                    return nbdt1yd;
                case 1:
                    return nbdt2yd;
                case 2:
                    return nbdt3yd;
                case 3:
                    return nbdt4yd;
                case 4:
                    return nbdt5yd;
                case 5:
                    return nbdt6yd;
                case 6:
                    return nbdt7yd;
            }
            return null;
        }
        public static DataTable GetTableNBRSRPLT(int i)
        {
            switch (i)
            {
                case 0:
                    return nbdt1lt;
                case 1:
                    return nbdt2lt;
                case 2:
                    return nbdt3lt;
                case 3:
                    return nbdt4lt;
                case 4:
                    return nbdt5lt;
                case 5:
                    return nbdt6lt;
                case 6:
                    return nbdt7lt;
            }
            return null;
        }


        static void Getnb3w(string path, out DataTable dt, out DataTable dt1, out DataTable dt2, out DataTable dt3, out DataTable dt4, out DataTable dt5, out DataTable dt6)
        {
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
            dt.Columns.Add("sc_lon", typeof(decimal));
            dt.Columns.Add("sc_lat", typeof(decimal));
            dt.Columns.Add("rsrp", typeof(decimal));
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
            dt1.Columns.Add("sc_lon", typeof(decimal));
            dt1.Columns.Add("sc_lat", typeof(decimal));
            dt1.Columns.Add("rsrp", typeof(decimal));
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
            dt2.Columns.Add("sc_lon", typeof(decimal));
            dt2.Columns.Add("sc_lat", typeof(decimal));
            dt2.Columns.Add("rsrp", typeof(decimal));
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
            dt3.Columns.Add("sc_lon", typeof(decimal));
            dt3.Columns.Add("sc_lat", typeof(decimal));
            dt3.Columns.Add("rsrp", typeof(decimal));
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
            dt4.Columns.Add("sc_lon", typeof(decimal));
            dt4.Columns.Add("sc_lat", typeof(decimal));
            dt4.Columns.Add("rsrp", typeof(decimal));
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
            dt5.Columns.Add("sc_lon", typeof(decimal));
            dt5.Columns.Add("sc_lat", typeof(decimal));
            dt5.Columns.Add("rsrp", typeof(decimal));
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
            dt6.Columns.Add("sc_lon", typeof(decimal));
            dt6.Columns.Add("sc_lat", typeof(decimal));
            dt6.Columns.Add("rsrp", typeof(decimal));
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

        public static void getDataGCK()
        {
#if CESHI
            string today = "2017-09-24";
            string yesterday = "2017-09-23";
            string l800m = @"select * from(
select kpi.eci,NVL(kpi.DOWN_PDCP,0) flow,NVL(case kpi.MANUFACTOR when '诺基亚' then kpi.DOWN_PRB/100 else kpi.DOWN_PRB end,0) prb,NVL(kpi.RRC_CONNCOUNT,0) rrc,
wp.Sc_Lat,wp.sc_lon,NVL(DECODE(mr.RSRP_COUNT_TOTAL,0,0,ROUND(mr.RSRP_COUNT_LOW/mr.RSRP_COUNT_TOTAL,4)),0) as rsrp,wp.enb_lat,wp.enb_lon,NVL(wp.ANTENNAAZIMUTH,0) ANTENNAAZIMUTH,wp.Sc_Name,wp.sc_enbname
,row_number()over(partition by wp.eci order by kpi.DOWN_PDCP desc) top2,wp.SC_ENBID,wp.SC_LCRID,to_char(kpi.time,'hh24') busyhour,NVL(wp.HOTSPOTCLASS,'无') HOTSPOTCLASS,NVL(wp.HOTSPOTNAME,'无') HOTSPOTNAME from workparameter wp
left join
(select * from data_kpiinfo  where time>= to_date('2017-09-23','yyyy-MM-dd') and time<to_date('2017-09-24','yyyy-MM-dd'))kpi on wp.eci=kpi.eci
left join
(select * from data_mr  where sdate>= to_date('2017-09-23','yyyy-MM-dd') and sdate<to_date('2017-09-24','yyyy-MM-dd'))mr on mr.eci=wp.eci
where wp.eci is not null and NVL(trim(substr(to_char(wp.sc_lcrid,'xx'),1,2)),'0') in ('1','9')
) a where top2=1 and  sc_lat <> 0 and sc_lon <> 0 and enb_lat <> 0 and enb_lon <> 0 and eci is not null";
            string l1_8g = @"select * from(
select kpi.eci,NVL(kpi.DOWN_PDCP,0) flow,NVL(case kpi.MANUFACTOR when '诺基亚' then kpi.DOWN_PRB/100 else kpi.DOWN_PRB end,0) prb,NVL(kpi.RRC_CONNCOUNT,0) rrc,
wp.Sc_Lat,wp.sc_lon,NVL(DECODE(mr.RSRP_COUNT_TOTAL,0,0,ROUND(mr.RSRP_COUNT_LOW/mr.RSRP_COUNT_TOTAL,4)),0) as rsrp,wp.enb_lat,wp.enb_lon,NVL(wp.ANTENNAAZIMUTH,0) ANTENNAAZIMUTH,wp.Sc_Name,wp.sc_enbname
,row_number()over(partition by wp.eci order by kpi.DOWN_PDCP desc) top2,wp.SC_ENBID,wp.SC_LCRID,to_char(kpi.time,'hh24') busyhour,NVL(wp.HOTSPOTCLASS,'无') HOTSPOTCLASS,NVL(wp.HOTSPOTNAME,'无') HOTSPOTNAME from workparameter wp
left join
(select * from data_kpiinfo where time>= to_date('"+yesterday+@"','yyyy-MM-dd') and time<to_date('"+today+@"','yyyy-MM-dd'))kpi on wp.eci=kpi.eci
left join
(select * from data_mr where sdate>= to_date('2017-09-23','yyyy-MM-dd') and sdate<to_date('2017-09-24','yyyy-MM-dd'))mr on mr.eci=wp.eci
where wp.eci is not null and NVL(trim(substr(to_char(wp.sc_lcrid,'xx'),1,2)),'0') in ('3','b')
) a where top2=1 and  sc_lat <> 0 and sc_lon <> 0 and enb_lat <> 0 and enb_lon <> 0 and eci is not null";
            string l2_1g = @"select * from(
select kpi.eci,NVL(kpi.DOWN_PDCP,0) flow,NVL(case kpi.MANUFACTOR when '诺基亚' then kpi.DOWN_PRB/100 else kpi.DOWN_PRB end,0) prb,NVL(kpi.RRC_CONNCOUNT,0) rrc,
wp.Sc_Lat,wp.sc_lon,NVL(DECODE(mr.RSRP_COUNT_TOTAL,0,0,ROUND(mr.RSRP_COUNT_LOW/mr.RSRP_COUNT_TOTAL,4)),0) as rsrp,wp.enb_lat,wp.enb_lon,NVL(wp.ANTENNAAZIMUTH,0) ANTENNAAZIMUTH,wp.Sc_Name,wp.sc_enbname
,row_number()over(partition by wp.eci order by kpi.DOWN_PDCP desc) top2,wp.SC_ENBID,wp.SC_LCRID,to_char(kpi.time,'hh24') busyhour,NVL(wp.HOTSPOTCLASS,'无') HOTSPOTCLASS,NVL(wp.HOTSPOTNAME,'无') HOTSPOTNAME from workparameter wp
left join
(select * from data_kpiinfo where time>= to_date('"+yesterday+@"','yyyy-MM-dd') and time<to_date('"+today+@"','yyyy-MM-dd'))kpi on wp.eci=kpi.eci
left join
(select * from data_mr where sdate>= to_date('2017-09-23','yyyy-MM-dd') and sdate<to_date('2017-09-24','yyyy-MM-dd'))mr on mr.eci=wp.eci
where wp.eci is not null and NVL(trim(substr(to_char(wp.sc_lcrid,'xx'),1,2)),'0') in ('0','8')
) a where top2=1 and  sc_lat <> 0 and sc_lon <> 0 and enb_lat <> 0 and enb_lon <> 0 and eci is not null";
            string l2_6g = @"select * from(
select kpi.eci,NVL(kpi.DOWN_PDCP,0) flow,NVL(case kpi.MANUFACTOR when '诺基亚' then kpi.DOWN_PRB/100 else kpi.DOWN_PRB end,0) prb,NVL(kpi.RRC_CONNCOUNT,0) rrc,
wp.Sc_Lat,wp.sc_lon,NVL(DECODE(mr.RSRP_COUNT_TOTAL,0,0,ROUND(mr.RSRP_COUNT_LOW/mr.RSRP_COUNT_TOTAL,4)),0) as rsrp,wp.enb_lat,wp.enb_lon,NVL(wp.ANTENNAAZIMUTH,0) ANTENNAAZIMUTH,wp.Sc_Name,wp.sc_enbname
,row_number()over(partition by wp.eci order by kpi.DOWN_PDCP desc) top2,wp.SC_ENBID,wp.SC_LCRID,to_char(kpi.time,'hh24') busyhour,NVL(wp.HOTSPOTCLASS,'无') HOTSPOTCLASS,NVL(wp.HOTSPOTNAME,'无') HOTSPOTNAME from workparameter wp
left join
(select * from data_kpiinfo where time>= to_date('"+yesterday+@"','yyyy-MM-dd') and time<to_date('"+today+@"','yyyy-MM-dd'))kpi on wp.eci=kpi.eci
left join
(select * from data_mr where sdate>= to_date('2017-09-23','yyyy-MM-dd') and sdate<to_date('2017-09-24','yyyy-MM-dd'))mr on mr.eci=wp.eci
where wp.eci is not null and NVL(trim(substr(to_char(wp.sc_lcrid,'xx'),1,2)),'0') in ('6')
) a where top2=1 and  sc_lat <> 0 and sc_lon <> 0 and enb_lat <> 0 and enb_lon <> 0 and eci is not null";
            string nb_iot = @"select lon sc_lon,lat sc_lat,nbrsrp from nbiot";

            L800M = DB.Query(l800m);
            L1_8G = DB.Query(l1_8g);
            L2_1G = DB.Query(l2_1g);
            L2_6G = DB.Query(l2_6g);
            NB_IoT = DB.Query(nb_iot);
#else
            string cityfg = "select CITY,ROUND(1-sum(RSRP_COUNT_LOW)/sum(RSRP_COUNT_TOTAL),4) FG　from (select * from data_mr where sdate>=trunc(sysdate-1) and city is not null and city<>'OTHER') a group by city";
            string cdma = @"select BTSNUM*4096+CELLNUM ECI,city,DISTRICT,TOWNSHIP,BSCNUM,BTSNUM,sec_name,SEC_LAT SC_LAT,SEC_LON SC_LON,AZIMUTH ANTENNAAZIMUTH,1 flow,1 rrc,1 rsrp,1 prb from C_WORKPARAMETER where SEC_LAT<>0 and SEC_LON<>0";
            string l800m = @"select * from(
select wp.eci,NVL(case kpi.MANUFACTOR when '诺基亚' then round(kpi.DOWN_PDCP/1,2) else kpi.DOWN_PDCP end,0) flow,NVL(case kpi.MANUFACTOR when '诺基亚' then kpi.DOWN_PRB/100 else kpi.DOWN_PRB end,0) prb,NVL(kpi.RRC_CONNCOUNT,0) rrc,
wp.Sc_Lat,wp.sc_lon,NVL(DECODE(mr.RSRP_COUNT_TOTAL,0,0,ROUND(1-(mr.RSRP_COUNT_LOW/mr.RSRP_COUNT_TOTAL),4)),null) as rsrp,wp.enb_lat,wp.enb_lon,NVL(wp.ANTENNAAZIMUTH,0) ANTENNAAZIMUTH,wp.Sc_Name,wp.sc_enbname
,row_number()over(partition by wp.eci order by kpi.DOWN_PDCP desc) top2,wp.SC_ENBID,wp.SC_LCRID,to_char(kpi.time,'hh24') busyhour,NVL(wp.HOTSPOTCLASS,'无') HOTSPOTCLASS,NVL(wp.HOTSPOTNAME,'无') HOTSPOTNAME,wp.city from v_workparameter wp
left join
(select * from data_kpiinfo where time >=trunc(sysdate-1) and time<trunc(sysdate))kpi on wp.eci=kpi.eci
left join
(select * from data_mr where sdate >=trunc(sysdate-2) and sdate<trunc(sysdate-1))mr on mr.eci=wp.eci
where wp.eci is not null and NVL(trim(substr(to_char(wp.sc_lcrid,'xx'),1,2)),'0') in ('1','9')
) a where top2=1 and  sc_lat <> 0 and sc_lon <> 0 and enb_lat <> 0 and enb_lon <> 0 and eci is not null";
            string l1_8g = @"select * from(
select wp.eci,NVL(case kpi.MANUFACTOR when '诺基亚' then round(kpi.DOWN_PDCP/1,2) else kpi.DOWN_PDCP end,0) flow,NVL(case kpi.MANUFACTOR when '诺基亚' then kpi.DOWN_PRB/100 else kpi.DOWN_PRB end,0) prb,NVL(kpi.RRC_CONNCOUNT,0) rrc,
wp.Sc_Lat,wp.sc_lon,NVL(DECODE(mr.RSRP_COUNT_TOTAL,0,0,ROUND(1-(mr.RSRP_COUNT_LOW/mr.RSRP_COUNT_TOTAL),4)),null) as rsrp,wp.enb_lat,wp.enb_lon,NVL(wp.ANTENNAAZIMUTH,0) ANTENNAAZIMUTH,wp.Sc_Name,wp.sc_enbname
,row_number()over(partition by wp.eci order by kpi.DOWN_PDCP desc) top2,wp.SC_ENBID,wp.SC_LCRID,to_char(kpi.time,'hh24') busyhour,NVL(wp.HOTSPOTCLASS,'无') HOTSPOTCLASS,NVL(wp.HOTSPOTNAME,'无') HOTSPOTNAME,wp.city from v_workparameter wp
left join
(select * from data_kpiinfo where time >=trunc(sysdate-1) and time<trunc(sysdate))kpi on wp.eci=kpi.eci
left join
(select * from data_mr where sdate >=trunc(sysdate-2) and sdate<trunc(sysdate-1))mr on mr.eci=wp.eci
where wp.eci is not null and NVL(trim(substr(to_char(wp.sc_lcrid,'xx'),1,2)),'0') in ('3','b')
) a where top2=1 and  sc_lat <> 0 and sc_lon <> 0 and enb_lat <> 0 and enb_lon <> 0 and eci is not null";
            string l2_1g = @"select * from(
select wp.eci,NVL(case kpi.MANUFACTOR when '诺基亚' then round(kpi.DOWN_PDCP/1,2) else kpi.DOWN_PDCP end,0) flow,NVL(case kpi.MANUFACTOR when '诺基亚' then kpi.DOWN_PRB/100 else kpi.DOWN_PRB end,0) prb,NVL(kpi.RRC_CONNCOUNT,0) rrc,
wp.Sc_Lat,wp.sc_lon,NVL(DECODE(mr.RSRP_COUNT_TOTAL,0,0,ROUND(1-(mr.RSRP_COUNT_LOW/mr.RSRP_COUNT_TOTAL),4)),null) as rsrp,wp.enb_lat,wp.enb_lon,NVL(wp.ANTENNAAZIMUTH,0) ANTENNAAZIMUTH,wp.Sc_Name,wp.sc_enbname
,row_number()over(partition by wp.eci order by kpi.DOWN_PDCP desc) top2,wp.SC_ENBID,wp.SC_LCRID,to_char(kpi.time,'hh24') busyhour,NVL(wp.HOTSPOTCLASS,'无') HOTSPOTCLASS,NVL(wp.HOTSPOTNAME,'无') HOTSPOTNAME,wp.city from v_workparameter wp
left join
(select * from data_kpiinfo where time >=trunc(sysdate-1) and time<trunc(sysdate))kpi on wp.eci=kpi.eci
left join
(select * from data_mr where sdate >=trunc(sysdate-2) and sdate<trunc(sysdate-1))mr on mr.eci=wp.eci
where wp.eci is not null and NVL(trim(substr(to_char(wp.sc_lcrid,'xx'),1,2)),'0') in ('0','8')
) a where top2=1 and  sc_lat <> 0 and sc_lon <> 0 and enb_lat <> 0 and enb_lon <> 0 and eci is not null";
            string l2_6g = @"select * from(
select wp.eci,NVL(case kpi.MANUFACTOR when '诺基亚' then round(kpi.DOWN_PDCP/1,2) else kpi.DOWN_PDCP end,0) flow,NVL(case kpi.MANUFACTOR when '诺基亚' then kpi.DOWN_PRB/100 else kpi.DOWN_PRB end,0) prb,NVL(kpi.RRC_CONNCOUNT,0) rrc,
wp.Sc_Lat,wp.sc_lon,NVL(DECODE(mr.RSRP_COUNT_TOTAL,0,0,ROUND(1-(mr.RSRP_COUNT_LOW/mr.RSRP_COUNT_TOTAL),4)),null) as rsrp,wp.enb_lat,wp.enb_lon,NVL(wp.ANTENNAAZIMUTH,0) ANTENNAAZIMUTH,wp.Sc_Name,wp.sc_enbname
,row_number()over(partition by wp.eci order by kpi.DOWN_PDCP desc) top2,wp.SC_ENBID,wp.SC_LCRID,to_char(kpi.time,'hh24') busyhour,NVL(wp.HOTSPOTCLASS,'无') HOTSPOTCLASS,NVL(wp.HOTSPOTNAME,'无') HOTSPOTNAME,wp.city from v_workparameter wp
left join
(select * from data_kpiinfo where time >=trunc(sysdate-1) and time<trunc(sysdate))kpi on wp.eci=kpi.eci
left join
(select * from data_mr where sdate >=trunc(sysdate-2) and sdate<trunc(sysdate-1))mr on mr.eci=wp.eci
where wp.eci is not null and NVL(trim(substr(to_char(wp.sc_lcrid,'xx'),1,2)),'0') in ('6')
) a where top2=1 and  sc_lat <> 0 and sc_lon <> 0 and enb_lat <> 0 and enb_lon <> 0 and eci is not null";
            string nb_iot = @"select lon sc_lon,lat sc_lat,nbrsrp from nbiot order by sc_lat,sc_lon";
            string nb_iot1 = @"select max(lon) sc_lon,max(lat) sc_lat,round(avg(nbrsrp),0) nbrsrp from nbiot group by floor(lon/0.014276766034981675),floor(lat/0.014276766034981674) order by sc_lat,sc_lon";
            string nb_iot2 = @"select max(lon) sc_lon,max(lat) sc_lat,round(avg(nbrsrp),0) nbrsrp from nbiot group by floor(lon/0.009517844023321129),floor(lat/0.0095178440233211325) order by sc_lat,sc_lon";
            string nb_iot3 = @"select max(lon) sc_lon,max(lat) sc_lat,round(avg(nbrsrp),0) nbrsrp from nbiot group by floor(lon/0.0047589220116605472),floor(lat/0.0047589220116605662) order by sc_lat,sc_lon";
            string nb_iot4 = @"select max(lon) sc_lon,max(lat) sc_lat,round(avg(nbrsrp),0) nbrsrp from nbiot group by floor(lon/0.0023794610058302736),floor(lat/0.0023794610058302831) order by sc_lat,sc_lon";
            string nb_iot5 = @"select max(lon) sc_lon,max(lat) sc_lat,round(avg(nbrsrp),0) nbrsrp from nbiot group by floor(lon/0.0011897305029151539),floor(lat/0.0011897305029151292) order by sc_lat,sc_lon";
            string nb_iot6 = @"select max(lon) sc_lon,max(lat) sc_lat,round(avg(nbrsrp),0) nbrsrp from nbiot group by floor(lon/0.000594865251457577),floor(lat/0.000594865251457577) order by sc_lat,sc_lon";
            CityFG = DB.Query(cityfg);
            CDMA = DB.Query(cdma);
            L800M = DB.Query(l800m);
            L1_8G = DB.Query(l1_8g);
            L2_1G = DB.Query(l2_1g);
            L2_6G = DB.Query(l2_6g);
            NB_IoT = DB.Query(nb_iot);
            var tmp1 = DB.Query(nb_iot1);
            var tmp2 = DB.Query(nb_iot2);
            var tmp3 = DB.Query(nb_iot3);
            var tmp4 = DB.Query(nb_iot4);
            var tmp5 = DB.Query(nb_iot5);
            var tmp6 = DB.Query(nb_iot6);
            tmp1.Tables[0].TableName = "t1";
            tmp2.Tables[0].TableName = "t2";
            tmp3.Tables[0].TableName = "t3";
            tmp4.Tables[0].TableName = "t4";
            tmp5.Tables[0].TableName = "t5";
            tmp6.Tables[0].TableName = "t6";
            nbdt1 = tmp1.Tables[0].Copy();
            nbdt2 = tmp2.Tables[0].Copy();
            nbdt3 = tmp3.Tables[0].Copy();
            nbdt4 = tmp4.Tables[0].Copy();
            nbdt5 = tmp5.Tables[0].Copy();
            nbdt6 = tmp6.Tables[0].Copy();
            NB_IoT.Tables.Add(nbdt1);
            NB_IoT.Tables.Add(nbdt2);
            NB_IoT.Tables.Add(nbdt3);
            NB_IoT.Tables.Add(nbdt4);
            NB_IoT.Tables.Add(nbdt5);
            NB_IoT.Tables.Add(nbdt6);
            //Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"nbdata\\电信"), out nbdt1dx, out nbdt2dx, out nbdt3dx, out nbdt4dx, out nbdt5dx, out nbdt6dx, out nbdt7dx);
            //Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"nbdata\\移动"), out nbdt1yd, out nbdt2yd, out nbdt3yd, out nbdt4yd, out nbdt5yd, out nbdt6yd, out nbdt7yd);
            //Getnb3w(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"nbdata\\联通"), out nbdt1lt, out nbdt2lt, out nbdt3lt, out nbdt4lt, out nbdt5lt, out nbdt6lt, out nbdt7lt);
#endif

        }
        private static void getDataHSY()
        {
            main.importData();
        }
        private static void getDataJXY() {
            evtdt = DB.QueryAsDt("select * from DATA_EVENT where evttime>=trunc(sysdate-7) and evttime<trunc(sysdate)");
            evtGdt = DB.QueryAsDt("select lteci,evtid,case evtid when 5008 then '弱覆盖事件' when 5009 then '无覆盖事件' when 2002 then '数据掉线事件' when 2005 then '数据连接建立失败事件' when 5021 then '4G回落3G事件' when 5020 then '4G回落2G事件' when 5023 then '网络频繁切换事件' else '未知事件' end EVTNAME,max(city)city,count(0)num from DATA_EVENT t where evttime>=trunc(sysdate-7) and evttime<trunc(sysdate) group by lteci,evtid order by evtid asc,num desc,city asc,lteci asc");
        }
        public static void getToken()
        {
            while (true)
            {

                try
                {
                    var url = System.Configuration.ConfigurationManager.AppSettings["tokenurl"];
                    var uid = System.Configuration.ConfigurationManager.AppSettings["tokenuid"];
                    var pwd = System.Configuration.ConfigurationManager.AppSettings["tokenpwd"];
                    var src = System.Configuration.ConfigurationManager.AppSettings["tokensrc"];
                    var firsturl = System.Configuration.ConfigurationManager.AppSettings["token1sturl"];
                    var appid = System.Configuration.ConfigurationManager.AppSettings["tokenappid"];
                    url = url + "?username=" + uid + "&password=" + pwd + "&expiration=&referer=" + src + "&mapUrl=" + firsturl + "&appId=" + appid;

                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    var res = req.GetResponse();
                    var stream = res.GetResponseStream();
                    var reader = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));
                    var jsonstr = reader.ReadToEnd();
                    var json = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(jsonstr);
                    _token = json["token"].ToString();
                }
                catch { }
                Thread.Sleep(60 * 60 * 1000 - 1000);
            }
        }
    }
}