using DWServices.BLL;
using DWServices.Common;
using DWServices.DAL;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Web.SessionState;

namespace DWServices
{
    /// <summary>
    /// index 的摘要说明
    /// </summary>
    public class index : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{\"data\":null}");
                return;
            }

            PostParameter paramter = PostParameter.getParameter(context);
            String action = paramter.QuertyType;
            if (!string.IsNullOrEmpty(action))
            {
                if (String.IsNullOrEmpty(paramter.QuertyTime))
                    paramter.QuertyTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                if (action.ToUpper() == "DATA")
                {
                    if (!String.IsNullOrEmpty(paramter.ECI))
                    {
                        String sqlstr = "select * from DATA_KQIINFO where eci='" + paramter.ECI + "' and to_char(createtime, 'yyyy-MM-dd') = '" + paramter.QuertyTime + "'";
                        DataTable data = OraConnect.ReadData(sqlstr);
                        String result = DataTableConvertJson.DataTableToJson("data", data);
                        context.Response.Write(result);
                    }
                    else if ((!String.IsNullOrEmpty(paramter.ENBID)) && (!String.IsNullOrEmpty(paramter.LCRID)))
                    {
                        String sqlstr = "select * from DATA_KQIINFO where SC_ENBID='" + paramter.ENBID + "' and SC_LCRID='" + paramter.LCRID + "' and to_char(createtime, 'yyyy-MM-dd') = '" + paramter.QuertyTime + "'";
                        DataTable data = OraConnect.ReadData(sqlstr);
                        if (data == null)
                            return;
                        if (data.Rows.Count < 1)
                        {
                            String eci = ECIAndIDConvert.ConvertECI(paramter.ENBID, paramter.LCRID).ToString();
                            data = OraConnect.ReadData("select city,manufactor,SC_NAME as cellname,eci,sc_enbid,sc_lcrid from V_WORKPARAMETER where eci='" + eci + "'");
                            if (data != null)
                            {
                                data.Columns.Add("kqiinfo");
                                data.Columns.Add("kqiindex");
                                data.Columns.Add("reason");
                                data.Columns.Add("measures");
                                data.Columns.Add("createtime");
                                if (data.Rows.Count > 0)
                                {
                                    data.Rows[0]["createtime"] = paramter.QuertyTime;
                                    List<String> kpiindex = (new KPIAnalysis()).GetLowIndex("KPI指标", paramter.QuertyTime, eci);
                                    if (kpiindex.Count > 0)
                                    {
                                        String kpistr = "";
                                        for (int i = 0; i < kpiindex.Count; i++)
                                        {
                                            if (String.IsNullOrEmpty(kpistr))
                                                kpistr = kpiindex[i];
                                            else
                                                kpistr = "、" + kpiindex[i];
                                        }
                                        if (String.IsNullOrEmpty(kpistr))
                                            data.Rows[0]["kqiindex"] = kpistr;
                                        else
                                        {
                                            data.Rows[0]["kqiinfo"] = "正常";
                                            data.Rows[0]["kqiindex"] = "正常";
                                        }
                                        List<String> mrindex = (new MRAnalysis()).GetLowIndex(paramter.QuertyTime, eci);
                                        if (mrindex.Count > 0)
                                        {
                                            String mrstr = "";
                                            for (int i = 0; i < mrindex.Count; i++)
                                            {
                                                if (String.IsNullOrEmpty(mrstr))
                                                    mrstr = mrindex[i];
                                                else
                                                    mrstr = "、" + mrindex[i];
                                            }
                                            if (String.IsNullOrEmpty(mrstr))
                                                data.Rows[0]["reason"] = mrstr;
                                            else
                                            {
                                                data.Rows[0]["reason"] = "正常";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        String result = DataTableConvertJson.DataTableToJson("data", data);
                        context.Response.Write(result);
                    }
                    else
                    {
                        //String sqlstr = "select * from DATA_KQIINFO where to_char(createtime, 'yyyy-MM-dd') = '" + paramter.QuertyTime + "'  and kqiindex is not null and kqiindex<>'暂无' and REASON is not null";//createtime=to_date('" + paramter.QuertyTime + "','yyyy-MM-dd')";
                        String sqlstr = "select a.*,b.hotspotclass,b.hotspotname,b.sfgtx,b.ANTENNAH,b.ANTENNAAZIMUTH,b.DIPANGLE,b.EDIPANGLE,MPDIPANGLE,MRCLASS from DATA_KQIINFO a left join v_workparameter b on a.eci=b.eci where to_char(createtime, 'yyyy-MM-dd') = '" + paramter.QuertyTime + "'  and kqiindex is not null and kqiindex<>'暂无' and REASON is not null order by a.city";
                        DataTable data = OraConnect.ReadData(sqlstr);
                        String result = DataTableConvertJson.DataTableToJson("data", data);
                        context.Response.Write(result);
                    }
                }
                else if (action.ToUpper() == "FLOW")
                {
                    String sqlstr = "select city,sum(userflow)/1024 as value from DATA_KPIINFO where  to_char(time, 'yyyy-MM-dd') = '" + paramter.QuertyTime + "' group by city";
                    DataTable flowdata = OraConnect.ReadData(sqlstr);
                    String result = DataTableConvertJson.DataTableToJson("data", flowdata);
                    context.Response.Write(result);
                }
                else if (action.ToUpper() == "MR")
                {
                    String sqlstr = "select city,sum(LTESCRSRP) as value from DATA_MR where to_char(SDATE, 'yyyy-MM-dd') = '" + paramter.QuertyTime + "' group by city ";
                    DataTable flowdata = OraConnect.ReadData(sqlstr);
                    String result = DataTableConvertJson.DataTableToJson("data", flowdata);
                    context.Response.Write(result);
                }
                else if (action.ToUpper() == "LOW")
                {
                    String sqlstr = "select city,count(*) as value from DATA_KQIINFO where to_char(createtime, 'yyyy-MM-dd') = '" + paramter.QuertyTime + "'  and kqiindex is not null and kqiindex<>'暂无' and REASON is not null group by city ";
                    DataTable flowdata = OraConnect.ReadData(sqlstr);
                    String result = DataTableConvertJson.DataTableToJson("data", flowdata);
                    context.Response.Write(result);
                }
                else if (action.ToUpper() == "MAPURL") {
                    var info = (((DWServices.Common.User)context.Session["user"]).Id.ToString().PadLeft(8, '0') + DateTime.Now.Ticks.ToString().PadLeft(18, '0'));
                    DESCryptoServiceProvider dec = new DESCryptoServiceProvider();
                    var keyb = System.Text.Encoding.GetEncoding("utf-8").GetBytes("xyhsygck");
                    var infob = System.Text.Encoding.GetEncoding("utf-8").GetBytes(info);
                    var stream = new System.IO.MemoryStream();
                    var cstream = new CryptoStream(stream, dec.CreateEncryptor(keyb, keyb), CryptoStreamMode.Write);
                    cstream.Write(infob, 0, infob.Length);
                    cstream.FlushFinalBlock();
                    var mima = Convert.ToBase64String(stream.ToArray());
                    mima = System.Uri.EscapeDataString(mima);
                    var url = System.Configuration.ConfigurationManager.AppSettings["dwmapserviceurl"];
                    context.Response.Write(url + "?token=" + mima);
                }
                else if (action.ToUpper() == "MAPURL2")
                {
                    var info = (((DWServices.Common.User)context.Session["user"]).Id.ToString().PadLeft(8, '0') + DateTime.Now.Ticks.ToString().PadLeft(18, '0'));
                    DESCryptoServiceProvider dec = new DESCryptoServiceProvider();
                    var keyb = System.Text.Encoding.GetEncoding("utf-8").GetBytes("xyhsygck");
                    var infob = System.Text.Encoding.GetEncoding("utf-8").GetBytes(info);
                    var stream = new System.IO.MemoryStream();
                    var cstream = new CryptoStream(stream, dec.CreateEncryptor(keyb, keyb), CryptoStreamMode.Write);
                    cstream.Write(infob, 0, infob.Length);
                    cstream.FlushFinalBlock();
                    var mima = Convert.ToBase64String(stream.ToArray());
                    mima = System.Uri.EscapeDataString(mima);
                    var url = System.Configuration.ConfigurationManager.AppSettings["dwmapserviceurl"];
                    context.Response.Write(url.Replace("/map","/threemap/threemap") + "?token=" + mima);
                }
                else { }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}