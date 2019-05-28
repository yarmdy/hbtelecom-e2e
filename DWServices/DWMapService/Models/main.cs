using DWMapService.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWMapService.Models
{
    public class main
    {
        public static void importData()
        {
            cleardata();
            getProTGTra();
            getProFGFlow();
            getPRBOperFac();
            getFGMRCover();
            getFGFlow();
            getOperFac();
            getTGTraffic();
            getMRCoverage();
            getTGOperFac();
        }
        private static void cleardata()
        {
            string sql = "update DATA_STATISTICS t set t.protgtra=0,t.profgflow=0,t.prboperfac=0,t.mrcover=0,t.tgoperfac=0";
            DB.Exec(sql);
        }
        //全省2G话务量
        private static void getProTGTra()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            string btime = time + " 00:00:00";
            string etime = time + " 23:59:59";
        }
        //全省4G流量
        private static void getProFGFlow()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            string btime = time + " 00:00:00";
            string etime = time + " 23:59:59";
            //string btime = "2017/06/12 00:00:00";
            //string etime = "2017/06/12 23:59:59";
            string sql = "select NVL(round(sum(t.DOWN_PDCP),2),0) DOWN_PDCP from DATA_KPIINFO t where t.time<=to_date('" + etime + "','yyyy/mm/dd hh24:mi:ss') and t.time>=to_date('" + btime + "','yyyy/mm/dd hh24:mi:ss')";
            DataTable dt = new DataTable();
            dt = DB.QueryAsDt(sql);
            DataColumn dc = new DataColumn("CITY", typeof(string));
            dc.DefaultValue = "河北省";
            dc.AllowDBNull = true;
            dt.Columns.Add(dc);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var flow = dt.Rows[i]["DOWN_PDCP"];
                string upsql = "update DATA_STATISTICS set PROFGFLOW=" + flow + " where city='河北省'";
                DB.Exec(upsql);
            }

        }
        //全省PRB利用率
        private static void getPRBOperFac()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            string[] bx = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };
            double z = 0;
            for (int i = 0; i < bx.Length; i++)
            {
                string btime = time + bx[i] + ":00:00";
                string etime = time + bx[i] + ":59:59";
                string gsql = "select NVL(round(sum(t.DOWN_PDCP),2),0) DOWN_PDCP from DATA_KPIINFO t where t.time<=to_date('" + etime + "','yyyy/mm/dd hh24:mi:ss') and t.time>=to_date('" + btime + "','yyyy/mm/dd hh24:mi:ss')";
                DataTable gdt = new DataTable();
                gdt = DB.QueryAsDt(gsql);
                if (z < Convert.ToDouble(gdt.Rows[0]["DOWN_PDCP"]))
                {
                    z = Convert.ToDouble(gdt.Rows[0]["DOWN_PDCP"]);
                    string sql = "select NVL(round(sum(t.DOWN_PRB)/count(*),2),0) DOWN_PRB from DATA_KPIINFO t where t.time<=to_date('" + etime + "','yyyy/mm/dd hh24:mi:ss') and t.time>=to_date('" + btime + "','yyyy/mm/dd hh24:mi:ss')";
                    DataTable dt = new DataTable();
                    dt = DB.QueryAsDt(sql);
                    DataColumn dc = new DataColumn("CITY", typeof(string));
                    dc.DefaultValue = "河北省";
                    dc.AllowDBNull = true;
                    dt.Columns.Add(dc);
                    for (int d = 0; d < dt.Rows.Count; d++)
                    {
                        string flow = dt.Rows[d]["DOWN_PRB"].ToString();
                        string upsql = "update DATA_STATISTICS set PRBOPERFAC=" + flow + " where city='河北省'";
                        DB.Exec(upsql);
                    }
                }
            }
        }
        //全省4GMR覆盖率
        private static void getFGMRCover()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            string btime = time + " 00:00:00";
            string etime = time + " 23:59:59";
            //string btime = "2017/06/12 00:00:00";
            //string etime = "2017/06/12 23:59:59";
            string sql = "select NVL(round(sum(t.RSRP_COUNT_TOTAL)/sum(t.RSRP_COUNT_LOW),2),0) CMCC_OVERLAP_RATE from DATA_MR t where t.SDATE<=to_date('" + etime + "','yyyy/mm/dd hh24:mi:ss') and t.SDATE>=to_date('" + btime + "','yyyy/mm/dd hh24:mi:ss')";
            DataTable dt = new DataTable();
            dt = DB.QueryAsDt(sql);
            DataColumn dc = new DataColumn("CITY", typeof(string));
            dc.DefaultValue = "河北省";
            dc.AllowDBNull = true;
            dt.Columns.Add(dc);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string flow = dt.Rows[i]["CMCC_OVERLAP_RATE"].ToString();
                string upsql = "update DATA_STATISTICS set MRCOVER=" + flow + " where city='河北省'";
                DB.Exec(upsql);
            }

        }
        //获取各地市4G流量
        private static void getFGFlow()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            string btime = time + " 00:00:00";
            string etime = time + " 23:59:59";
            //string btime = "2017/06/12 00:00:00";
            //string etime = "2017/06/12 23:59:59";
            string sql = "select  t.city,NVL(round(sum(t.DOWN_PDCP),2),0) DOWN_PDCP from DATA_KPIINFO t where t.time<=to_date('" + etime + "','yyyy/mm/dd hh24:mi:ss') and t.time>=to_date('" + btime + "','yyyy/mm/dd hh24:mi:ss')  group by t.city";
            DataTable dt = new DataTable();
            dt = DB.QueryAsDt(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strName = dt.Rows[i]["CITY"].ToString();
                string flow = dt.Rows[i]["DOWN_PDCP"].ToString();
                if (flow == null)
                {
                    flow = "0";
                }
                string upsql = "update DATA_STATISTICS set PROFGFLOW=" + flow + " where city='" + strName + "'";
                DB.Exec(upsql);
            }
        }
        //各地市RPB利用率
        private static void getOperFac()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            //string btime = time + " 00:00:00";
            //string etime = time + " 23:59:59";
            string[] bx = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };
            string[] city = { "石家庄", "唐山", "邯郸", "保定", "沧州", "邢台", "廊坊", "承德", "张家口", "衡水", "秦皇岛", "雄安" };
            for (int i = 0; i < city.Length; i++)
            {
                double dz = 0;
                for (int j = 0; j < bx.Length; j++)
                {
                    //string btime = "2017/06/12 " + bx[j] + ":00:00";
                    //string etime = "2017/06/12 " + bx[j] + ":59:59";
                    string btime = time + bx[i] + ":00:00";
                    string etime = time + bx[i] + ":59:59";
                    string gsql = "select NVL(round(sum(t.DOWN_PDCP),2),0) DOWN_PDCP from DATA_KPIINFO t where t.city='" + city[i] + "' and t.time<=to_date('" + etime + "','yyyy/mm/dd hh24:mi:ss') and t.time>=to_date('" + btime + "','yyyy/mm/dd hh24:mi:ss')";
                    DataTable gdt = new DataTable();
                    gdt = DB.QueryAsDt(gsql);
                    if (dz < Convert.ToDouble(gdt.Rows[0]["DOWN_PDCP"]))
                    {
                        dz = Convert.ToDouble(gdt.Rows[0]["DOWN_PDCP"]);
                        string sql = "select NVL(round(sum(t.DOWN_PRB)/count(*),2),0) DOWN_PRB from DATA_KPIINFO t where t.city='" + city[i] + "' and t.time<=to_date('" + etime + "','yyyy/mm/dd hh24:mi:ss') and t.time>=to_date('" + btime + "','yyyy/mm/dd hh24:mi:ss')";
                        DataTable dt = new DataTable();
                        dt = DB.QueryAsDt(sql);
                        for (int d = 0; d < dt.Rows.Count; d++)
                        {

                            string flow = dt.Rows[d]["DOWN_PRB"].ToString();
                            string upsql = "update DATA_STATISTICS set PRBOPERFAC=" + flow + " where city='" + city[i] + "'";
                            DB.Exec(upsql);
                        }
                    }
                }
            }
        }
        //各地市MR覆盖率
        private static void getMRCoverage()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            string btime = time + " 00:00:00";
            string etime = time + " 23:59:59";
            //string btime = "2017/06/12 00:00:00";
            //string etime = "2017/06/12 23:59:59";
            string sql = "select  t.city,NVL(round(sum(t.RSRP_COUNT_TOTAL)/sum(t.RSRP_COUNT_LOW),2),0) CMCC_OVERLAP_RATE from DATA_MR t where t.SDATE<=to_date('" + etime + "','yyyy/mm/dd hh24:mi:ss') and t.SDATE>=to_date('" + btime + "','yyyy/mm/dd hh24:mi:ss')  group by t.city ";
            DataTable dt = new DataTable();
            dt = DB.QueryAsDt(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strName = dt.Rows[i]["CITY"].ToString();
                string flow = dt.Rows[i]["CMCC_OVERLAP_RATE"].ToString();
                if (flow == null)
                {
                    flow = "0";
                }
                string upsql = "update DATA_STATISTICS set MRCOVER=" + flow + " where city='" + strName + "'";
                DB.Exec(upsql);
            }
        }
        //各地市2G话务量
        private static void getTGTraffic()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            string btime = time + " 00:00:00";
            string etime = time + " 23:59:59";
        }
        //各地市2G资源利用率
        private static void getTGOperFac()
        {
            string time = DateTime.Now.AddDays(-1).ToShortDateString().ToString();
            string btime = time + " 00:00:00";
            string etime = time + " 23:59:59";
        }
    }
}