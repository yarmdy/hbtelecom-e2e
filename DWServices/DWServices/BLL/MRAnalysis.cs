using Com.Netframe.Helpers;
using DWServices.Common;
using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DWServices.BLL
{
    public class MRAnalysis
    {
        Analysis anal = new Analysis();
        String[] arrnoseas = { "pjjl", "xqpjrsrp" };//质差原因只显示雷达图范围指标屏蔽一部分
        public List<String> GetLowIndex(String time, String cei)
        {
            List<String> result = new List<string>();
            DataTable quotadata = GetQuota.getAllQuotainfo("覆盖问题",cei);
            DateTime dt = DateTime.Now.AddDays(-1);
            if (!String.IsNullOrEmpty(time))
            {
                dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                if (dt == DateTime.Now.Date)
                    dt = DateTime.Now.AddDays(-1);
            }
            DateTime endtime = dt.AddDays(-6);
            for (int i = 0; i < quotadata.Rows.Count; i++)
            {
                String mfild = quotadata.Rows[i]["MFIELD"].ToString();
                String quota = quotadata.Rows[i]["mykey"].ToString();
                String quotaname = quotadata.Rows[i]["thequota"].ToString();
                if (arrnoseas.Contains(quota))
                    continue;
                DataTable data = OraConnect.ReadData("select t.eci,t.sdate,"
                            + quotadata.Rows[i]["MFIELD"].ToString() + " as value from DATA_MR t where t.eci=" + cei + " and t.sdate >=to_date('" + endtime.ToString("yyyy-MM-dd")
                            + "','yyyy-MM-dd') and t.sdate <=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') order by sdate");
                data = anal.GetQuotaData(data,cei, "value", mfild);
                if (data == null)
                    continue;
                if (!anal.getQuotaStatic(data, time))
                    result.Add(quotaname);
            }
            return result;
        }

        public DataTable GetLowData(String mfiled, String eci, String time, int longtime, Boolean maxormin)
        {
            DataTable data = null;
            try
            {
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
                               + mfiled + ") as value from DATA_MR t where t.eci=" + eci + " and t.sdate >=to_date('" + endtime.ToString("yyyy-MM-dd")
                               + "','yyyy-MM-dd') and t.sdate <=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd')";
                }
                else
                {
                    sql = "select max("
                                 + mfiled + ") as value from DATA_MR t where t.eci=" + eci + " and t.sdate >=to_date('" + endtime.ToString("yyyy-MM-dd")
                                 + "','yyyy-MM-dd') and t.sdate <=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd')";
                }
                data = OraConnect.ReadData(sql);
            }
            catch (Exception err)
            {
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }

            return data;
        }

        public String GetData(String time, String cei,String mykey)
        {
            String result = "";
            try
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                if (!String.IsNullOrEmpty(time))
                    dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                DateTime endtime = dt.AddDays(-6);

                DataTable quotadata = GetQuota.getQuotainfo(mykey);
                if (quotadata == null)
                    return "";
                else
                {
                    for (int i = 0; i < quotadata.Rows.Count; i++)
                    {
                        String quotaname = quotadata.Rows[i]["thequota"].ToString();
                        String mfild = quotadata.Rows[i]["MFIELD"].ToString();
                        String quota = quotadata.Rows[i]["mykey"].ToString();
                        DataTable data = OraConnect.ReadData("select t.eci,t.sdate,"
                            + quotadata.Rows[i]["MFIELD"].ToString() + " as value from DATA_MR t where t.eci=" + cei + " and t.sdate >=to_date('" + endtime.ToString("yyyy-MM-dd")
                            + "','yyyy-MM-dd') and t.sdate <=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') order by sdate");
                        if (data != null)
                        {
                            for (DateTime t = endtime; t <= dt; )
                            {
                                String str = t.ToString("yyyy-MM-dd");
                                DataRow[] rows = data.Select("sdate='" + str + "'");
                                if (rows.Length < 1)
                                {
                                    DataRow r = data.NewRow();
                                    r["sdate"] = str;
                                    r["eci"] = cei;
                                    r["value"] = "0";
                                    data.Rows.Add(r);
                                }
                                t = t.AddDays(1);
                            }
                            var dv = data.DefaultView;
                            dv.Sort = "sdate asc";
                            data = dv.ToTable();
                        }
                        else
                            return "";
                        data = anal.GetQuotaData(data,cei, "value", mfild);
                        String quotaresult = quotaname + "指标正常";
                        if (!anal.getQuotaStatic(data,time))
                            quotaresult = quotaname + "指标引起小区KQI质差";
                        if (quota == "rfgbl" || quota == "cdfgk" || quota == "modgrcd")
                        {
                            if (data != null)
                            {
                                for (DateTime t = endtime; t <= dt; )
                                {
                                    String str = t.ToString("yyyy-MM-dd");
                                    DataRow[] rows = data.Select("sdate='" + str + "'");
                                    if (rows.Length > 0)
                                    {
                                        try
                                        {
                                            if (!String.IsNullOrEmpty(rows[0]["value"].ToString()))
                                            {
                                                double v = Convert.ToDouble(rows[0]["value"].ToString().Trim());
                                                rows[0]["value"] = (v * 100.00).ToString();
                                            }
                                        }
                                        catch { }
                                    }
                                    t = t.AddDays(1);
                                }
                            }
                        }
                        result = DataTableConvertJson.DataTableToJson("index\":\"" + quota + "\",\"data", data, ",\"message\":\"" + quotaresult + "\"");
                    }
                }
            } 
            catch (Exception err){
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return result;
        }
    }
}