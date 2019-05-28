using Com.Netframe.Helpers;
using DWServices.Common;
using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.BLL
{
    public class KPIAnalysis
    {
        Analysis anal = new Analysis();
        String[] arrindexl = { "rrcljcgl","xhysl", "rrcljcjbl",  "uesxwdxl","erabdxl", "rrccjbl",  "xtnqhcgl",  "cdsbl" };
        String[] arrindexs = {"rrcsbcs", "xhyscs","rrcljcjcgcs", "uesxycsfcs", "erabdxcs","rrccjqqcs","xtnqsbcs", "ltecdxcs"};
        String[] arrnoseas = { "pjjhyhs18"};//质差原因只显示雷达图范围指标屏蔽平均用户激活数
        public List<String> GetLowIndex(String proj,String time, String eci)
        {
            List<String> result = new List<string>();

            DataTable quotadata = GetQuota.getAllQuotainfo(proj,eci);
            for (int i = 0; i < quotadata.Rows.Count; i++)
            {
                String mfild = quotadata.Rows[i]["MFIELD"].ToString();
                String quota = quotadata.Rows[i]["mykey"].ToString();
                String quotaname = quotadata.Rows[i]["thequota"].ToString();
                String DIRECTION = quotadata.Rows[i]["DIRECTION"].ToString();
                if (arrindexs.Contains(quota) || arrnoseas.Contains(quota))
                    continue;
                if (arrindexl.Contains(quota))
                {
                    int l = 0;
                    for (l = 0; l < arrindexl.Length; l++)
                    {
                        if (arrindexl[l] == quota)
                        {
                            if (!GetTwoKPIAnd(time, eci, l))
                            {
                                result.Add(quotaname);
                                continue;
                            }
                        }
                    }
                    continue;
                }
                Boolean Ismin = true;
                if (DIRECTION == "1")
                    Ismin = false;
                DataTable data = ReadData(quotadata.Rows[i]["MFIELD"].ToString(), eci, time, Ismin);

                data = anal.GetQuotaData(data, eci, "value", mfild);
                if (data == null)
                    continue;
                if (!anal.getQuotaStatic(data, time))
                    result.Add(quotaname);
            }
            return result;
        }

        public DataTable ReadData(String mfiled,String eci,String time,Boolean ismin)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            if (!String.IsNullOrEmpty(time))
            {
                dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                if (dt == DateTime.Now.Date)
                    dt = DateTime.Now.AddDays(-1);
            }
            DateTime starttime = dt.AddDays(-6);
            DateTime endtime = dt.AddDays(1);
            String sql = "";
            if (ismin)
            {
                sql = "select to_char(time,'yyyy-MM-dd') as sdate ,min(" + mfiled + ") as value from (select t.time, " + mfiled
                    + " from DATA_KPIINFO t where t.eci = '" + eci + "' and t.time >= to_date('" + starttime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and t.time < to_date('" + endtime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd')"
                    + ") group by to_char(time,'yyyy-MM-dd') order by sdate";
            }
            else
                sql = "select to_char(time,'yyyy-MM-dd') as sdate ,max(" + mfiled + ") as value from (select t.time, " + mfiled
                    + " from DATA_KPIINFO t where t.eci = '" + eci + "' and t.time >= to_date('" + starttime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and t.time < to_date('" + endtime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd')"
                    + ") group by to_char(time,'yyyy-MM-dd') order by sdate";
            DataTable data = OraConnect.ReadData(sql);
            if (data != null)
            {
                for (DateTime i = starttime; i < endtime; )
                {
                    String str = i.ToString("yyyy-MM-dd");
                    DataRow[] rows = data.Select("sdate='" + str + "'");
                    if (rows.Length < 1)
                    {
                        DataRow r = data.NewRow();
                        r["sdate"] = str;
                        data.Rows.Add(r);
                    }
                    i = i.AddDays(1);
                }
                var dv = data.DefaultView;
                dv.Sort = "SDATE asc";
                data = dv.ToTable();
            }
            return data;
        }
        public DataTable GetLowData(String mfiled, String eci, String time,int longtime,Boolean maxormin)
        {
            DataTable data = null;
            try {
                if (longtime == 0)
                    longtime = 1;
                DateTime dt = DateTime.Now.AddDays(-1);
                if (!String.IsNullOrEmpty(time))
                {
                    dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                    if (dt == DateTime.Now.Date)
                        dt = DateTime.Now.AddDays(-1);
                }
                DateTime endtime = dt.AddDays(longtime);
                String sql = "";
                if (maxormin)
                {
                    sql = "select min("
                               + mfiled + ") as value from DATA_KPIINFO t where t.eci=" + eci + " and t.time >=to_date('" + dt.ToString("yyyy-MM-dd")
                               + "','yyyy-MM-dd') and t.time <to_date('" + endtime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') order by time";
                }
                else
                {
                    sql = "select max("
                                 + mfiled + ") as value from DATA_KPIINFO t where t.eci=" + eci + " and t.time >=to_date('" + dt.ToString("yyyy-MM-dd")
                                 + "','yyyy-MM-dd') and t.time <to_date('" + endtime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') order by time";
                }
                data = OraConnect.ReadData(sql);
            }
            catch (Exception err){
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log"); 
            }
            
            return data;
        }

        public String GetData(String time, String eci, String mykey)
        {
            String result = "";
            int l = 0;
            for (l=0;l < arrindexl.Length; l++)
            {
                if (arrindexl[l] == mykey)
                {
                    result = GetTwoKPI(time,eci,l);
                    return result;
                }
            }
            try
            {
                DataTable quotadata = GetQuota.getQuotainfo(mykey);
                if (quotadata == null)
                    return "";
                else
                {
                    for (int i = 0; i < quotadata.Rows.Count; i++)
                    {
                        String mfild = quotadata.Rows[i]["MFIELD"].ToString();
                        String quota = quotadata.Rows[i]["mykey"].ToString();
                        String quotaname = quotadata.Rows[i]["thequota"].ToString();
                        String DIRECTION = quotadata.Rows[i]["DIRECTION"].ToString();
                        Boolean Ismin = true;
                        if (DIRECTION == "1")
                            Ismin = false;
                        DataTable data = ReadData(quotadata.Rows[i]["MFIELD"].ToString(), eci, time,Ismin);

                        if (data != null)
                        {
                            DateTime dt = DateTime.Now.AddDays(-1);
                            if (!String.IsNullOrEmpty(time))
                                dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                            DateTime endtime = dt.AddDays(-6);
                            for (DateTime t = endtime; t <= dt; )
                            {
                                String str = t.ToString("yyyy-MM-dd");
                                DataRow[] rows = data.Select("SDATE='" + str + "'");
                                if (rows.Length < 1)
                                {
                                    DataRow r = data.NewRow();
                                    r["SDATE"] = str;
                                    r["VALUE"] = "0";
                                    data.Rows.Add(r);
                                }
                                t = t.AddDays(1);
                            }
                            var dv = data.DefaultView;
                            dv.Sort = "SDATE asc";
                            data = dv.ToTable();
                        }
                        else
                            return "";

                        data = anal.GetQuotaData(data,eci, "value", mfild);
                        String quotaresult = quotaname + "指标正常";
                        if (!anal.getQuotaStatic(data,time))
                            quotaresult = quotaname + "指标引起小区KQI质差";
                        result = DataTableConvertJson.DataTableToJson("index\":\"" + quota + "\",\"data", data, ",\"message\":\"" + quotaresult + "\"");
                    }
                }
            }
            catch (Exception err){
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return result;
        }


        private String GetTwoKPI(String time, String eci, int l)
        {
            String result = "";
            try {
                DataTable quotadata = OraConnect.ReadData("select * from DATA_QUOTA t  where mykey in ('" + arrindexl[l] + "','" + arrindexs[l] + "')");
                if (quotadata == null)
                    return "";
                else
                {
                    String mfild = quotadata.Rows[0]["MFIELD"].ToString();
                    String quota = quotadata.Rows[0]["mykey"].ToString();
                    String quotaname = quotadata.Rows[0]["thequota"].ToString();
                    String DIRECTION = quotadata.Rows[0]["DIRECTION"].ToString();
                    Boolean Ismin = true;
                    if (DIRECTION == "1")
                        Ismin = false;
                    DataTable data = ReadData(quotadata.Rows[0]["MFIELD"].ToString(), eci, time, Ismin);
                    data = anal.GetQuotaData(data, eci, "value", mfild);
                    String jielun = quotaname;
                    result = DataTableConvertJson.DataTableToJson("index\":\"" + quota + "\",\"data", data);
                    result = result.Substring(2, result.Length - 3);
                    mfild = quotadata.Rows[1]["MFIELD"].ToString();
                    quota = quotadata.Rows[1]["mykey"].ToString();
                    DIRECTION = quotadata.Rows[1]["DIRECTION"].ToString();
                    Ismin = true;
                    if (DIRECTION == "1")
                        Ismin = false;
                    DataTable data2 = ReadData(quotadata.Rows[1]["MFIELD"].ToString(), eci, time, Ismin);
                    data2 = anal.GetQuotaData(data2, eci, "value", mfild);
                    if (data2 != null)
                    {
                        DateTime dt = DateTime.Now.AddDays(-1);
                        if (!String.IsNullOrEmpty(time))
                            dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime endtime = dt.AddDays(-6);
                        for (DateTime t = endtime; t <= dt; )
                        {
                            String str = t.ToString("yyyy-MM-dd");
                            DataRow[] rows = data2.Select("SDATE='" + str + "'");
                            if (rows.Length < 1)
                            {
                                DataRow r = data2.NewRow();
                                r["SDATE"] = str;
                                r["VALUE"] = "0";
                                data2.Rows.Add(r);
                            }
                            t = t.AddDays(1);
                        }
                        var dv = data2.DefaultView;
                        dv.Sort = "SDATE asc";
                        data2 = dv.ToTable();
                    }
                    else
                        return "";
                    if (anal.Analysisindex(data, data2, quotaname, time))
                        jielun = jielun+"性能正常";
                    else
                        jielun = jielun + "性能差";
                    result = DataTableConvertJson.DataTableToJson(result + ",\"data2", data2, ",\"message\":\"" + jielun + "\"");
                }
            }
            catch (Exception err)
            {
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return result;
        }

        private Boolean GetTwoKPIAnd(String time, String eci, int l)
        {
            try
            {
                DataTable quotadata = OraConnect.ReadData("select * from DATA_QUOTA t  where mykey in ('" + arrindexl[l] + "','" + arrindexs[l] + "')");
                if (quotadata == null)
                    return true;
                else
                {
                    String mfild = quotadata.Rows[0]["MFIELD"].ToString();
                    String quota = quotadata.Rows[0]["mykey"].ToString();
                    String quotaname = quotadata.Rows[0]["thequota"].ToString();
                    String DIRECTION = quotadata.Rows[0]["DIRECTION"].ToString();
                    Boolean Ismin = true;
                    if (DIRECTION == "1")
                        Ismin = false;
                    DataTable data = ReadData(quotadata.Rows[0]["MFIELD"].ToString(), eci, time, Ismin);
                    data = anal.GetQuotaData(data, eci, "value", mfild);
                    mfild = quotadata.Rows[1]["MFIELD"].ToString();
                    quota = quotadata.Rows[1]["mykey"].ToString();
                    DIRECTION = quotadata.Rows[1]["DIRECTION"].ToString();
                    Ismin = true;
                    if (DIRECTION == "1")
                        Ismin = false;
                    DataTable data2 = ReadData(quotadata.Rows[1]["MFIELD"].ToString(), eci, time, Ismin);
                    data2 = anal.GetQuotaData(data2, eci, "value", mfild);
                    if (anal.Analysisindex(data, data2, quotaname, time))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception err)
            {
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return true;
        }

        public String GetHourData(String time, String eci, String mykey)
        {
            String result = "";
            try
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                if (!String.IsNullOrEmpty(time))
                {
                    dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                }
                DateTime endtime = dt.AddDays(1);
                DataTable quotadata = GetQuota.getQuotainfo(mykey);
                if (quotadata == null)
                    return "";
                else
                {
                    for (int i = 0; i < quotadata.Rows.Count; i++)
                    {
                        String mfild = quotadata.Rows[i]["MFIELD"].ToString();
                        String quota = quotadata.Rows[i]["mykey"].ToString();
                        String quotaname = quotadata.Rows[i]["thequota"].ToString();
                        DataTable data = OraConnect.ReadData("select a.hh, a.value, b.sdate from (select t.eci,to_char(time, 'hh24') as hh,"
                                       + mfild + " as value from DATA_KPIINFO t where t.eci=" + eci + " and t.time >=to_date('" + dt.ToString("yyyy-MM-dd")
                                       + "','yyyy-MM-dd') and t.time <to_date('" + endtime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd')) a left join (select to_char(sysdate + (rownum - 1) / 24, 'hh24') as sdate"
                                       +" from dual connect by rownum <= 24) b on a.hh = b.sdate order by sdate");
                        result = DataTableConvertJson.DataTableToJson("index\":\"" + quota + "\",\"data", data);
                    }
                }
            }
            catch (Exception err)
            {
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return result;
        }
    }
}