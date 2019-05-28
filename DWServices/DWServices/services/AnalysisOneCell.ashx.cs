using DWServices.BLL;
using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// AnalysisOneCell 的摘要说明
    /// </summary>
    public class AnalysisOneCell : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("请先登录");
                return;
            }

            String eci = "23599666";
            String time = "2017-06-12";
            DataTable data = OraConnect.ReadData("select ECI from DATA_KQIINFO t where t.createtime=to_date('" + time + "','yyyy-MM-dd')");
            KPIAnalysis kpi = new KPIAnalysis();
            MRAnalysis mr = new MRAnalysis();
            if (data != null)
            {
                for (int r = 0; r < data.Rows.Count; r++)
                {
                    String kpiindex = "";
                    String reson = "";
                    String measure = "";
                    eci = data.Rows[r]["ECI"].ToString();
                    if (String.IsNullOrEmpty(eci))
                        continue;
                    List<String> list = kpi.GetLowIndex("KPI指标", time, eci);
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (String.IsNullOrEmpty(kpiindex))
                            kpiindex = list[i];
                        else
                            kpiindex = kpiindex + "、" + list[i];
                    }
                    list.Clear();
                    list = kpi.GetLowIndex("容量问题（小区自忙时）", time, eci);
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (String.IsNullOrEmpty(reson))
                            reson = list[i];
                        else
                            reson = reson + "、" + list[i];
                    }
                    list.Clear();
                    list = mr.GetLowIndex(time, eci);
                    for (int i = 0; i < list.Count; i++)
                    {

                        if (String.IsNullOrEmpty(reson))
                            reson = list[i];
                        else
                            reson = reson + "、" + list[i];
                    }
                    measure = MeasureAnalysis.GetMeasure(eci, time);
                    String alarm = (new AlarmAnalysis()).GetData(eci, "800", time, "zcxq");
                    String alarm2 = (new AlarmAnalysis()).GetData(eci, "800", time, "ljxq");
                    if (!String.IsNullOrEmpty(alarm) || !String.IsNullOrEmpty(alarm2))
                    {
                        if (String.IsNullOrEmpty(reson))
                            reson = "告警";
                        else
                            reson = reson + "、" + "告警";
                        if (String.IsNullOrEmpty(measure))
                            reson = "处理告警";
                        else
                            reson = reson + "、" + "处理告警";
                    }
                    bool b = OraConnect.ExecuteSQL("update DATA_KQIINFO set kqiindex='" + kpiindex + "',reason='" + reson + "',MEASURES='" + measure + "' where eci='" + eci + "' and createtime=to_date('" + time + "','yyyy-MM-dd')");
                }
            }
            context.Response.Write("分析结束！");
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