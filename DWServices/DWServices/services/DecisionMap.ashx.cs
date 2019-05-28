using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// DecisionMap 的摘要说明
    /// </summary>
    public class DecisionMap : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{\"ok\":false}");
                return;
            }

            string id = context.Request["id"];
            string sql = "";
            DateTime dt = DateTime.Now;
            string time = dt.ToString("yyyy-MM-dd HH:mm:ss");
            string otime = dt.ToString("yyyy-MM-dd");
            string ztime = dt.AddDays(-1).ToString("yyyy-MM-dd");
            if (id == "current")
            {
                string sql2 = "select distinct DATA_TIME from DATA_DISPLAY where DATA_NAME = 'WIFI' and DATA_STATUS='CURRENT'";
                DataTable timetable = OraConnect.ReadData(sql2);
                DateTime dt8 = DateTime.Parse(timetable.Rows[0][0].ToString());
                DateTime temptime = dt8;
                dt = dt8.AddMinutes(-15);
                int min = (temptime.Minute / 15) * 15;
                int min2 = (dt.Minute / 15) * 15;
                string begintime = dt.ToString("yyyy-MM-dd HH") + ":" + min2 + ":00";
                string endtime = temptime.ToString("yyyy-MM-dd HH") + ":" + min + ":00";
                sql = "select w.city, w.sc_lon, w.sc_lat ,w.SC_ENBNAME,k.ecgi from(select * from KQI_MIN t "
                    + "where t.start_time > to_date('" + begintime + "', 'yyyy-mm-dd hh24:mi:ss') "
                    + "and t.start_time <= to_date('" + endtime + "', 'yyyy-mm-dd hh24:mi:ss')) k "
                    + "join V_WORKPARAMETER w on k.ECGI = w.ECI where w.sc_lon is not null and w.sc_lat is not null";
            }
            else if (id == "more")
            {
                string thistime = dt.ToString("yyyy-MM-dd");
                sql = "select distinct w.city, w.sc_lon, w.sc_lat, w.SC_ENBNAME,k.ecgi from(select * from KQI_MIN "
                    + "where trunc(start_time) = to_date('" + thistime + "', 'yyyy-mm-dd hh24:mi:ss') "
                    + "and ECGI in (select ECGI from KQI_MIN t where trunc(t.start_time) = "
                    + "to_date('" + thistime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ECGI "
                    + "HAVING count(t.ECGI) >= 3 and count(t.ECGI) <= 5)) k join V_WORKPARAMETER w "
                    + "on k.ECGI = w.ECI where w.sc_lon is not null and w.sc_lat is not null";
            }
            else if (id == "day")
            {
                string thistime = dt.AddDays(-1).ToString("yyyy-MM-dd");
                sql = "select distinct w.city, w.sc_lon, w.sc_lat,w.SC_ENBNAME,k.ecgi from(select * from KQI_DAY t "
                    + "where trunc(t.start_time) = to_date('" + thistime + "', 'yyyy-mm-dd')) k "
                    + "join V_WORKPARAMETER w on k.ECGI = w.ECI where w.sc_lon is not null and w.sc_lat is not null";
            }
            else if (id == "week")
            {
                string begintime = dt.AddDays(-7).ToString("yyyy-MM-dd");
                sql = "select distinct w.city,w.sc_lon,w.sc_lat,w.SC_ENBNAME,k.ecgi from (select ECGI "
                    + "from KQI_DAY where trunc(start_time) >=to_date('"
                    + begintime + "', 'yyyy-mm-dd hh24:mi:ss') "
                    + "and trunc(start_time)<to_date('" + otime + "', 'yyyy-mm-dd hh24:mi:ss') "
                    + "and ECGI in (select ECGI from KQI_DAY t where trunc(t.start_time) >= "
                    + "to_date('" + begintime + "', 'yyyy-mm-dd hh24:mi:ss') "
                    + "and trunc(t.start_time) < to_date('" + otime + "', 'yyyy-mm-dd hh24:mi:ss') "
                    + "group by t.ECGI HAVING count(t.ECGI) >= 3 and max(trunc(t.start_time))=to_date('" + ztime + "','yyyy-mm-dd'))) k join V_WORKPARAMETER w on k.ECGI = w.ECI where w.sc_lon is not null and w.sc_lat is not null";
            }
            else
            {
                context.Response.Write("{\"ok\":false}");
                return;
            }
            DataSet ds = new DataSet();
            DataTable table = OraConnect.ReadData(sql);
            string s = GetJsonByDataTable(table);
            context.Response.Write(s);
        }

        public static string GetJsonByDataTable(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return "{\"ok\":false}";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"ok\":true,\"data\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i != dt.Rows.Count - 1)
                {
                    sb.Append("{\"name\":\"" + dt.Rows[i][0] + "\",\"value\":[" + dt.Rows[i][1] + "," + dt.Rows[i][2] + "],\"jzmc\":\""+dt.Rows[i][3]+"\"},");
                }
                else
                {
                    sb.Append("{\"name\":\"" + dt.Rows[i][0] + "\",\"value\":[" + dt.Rows[i][1] + "," + dt.Rows[i][2] + "],\"jzmc\":\"" + dt.Rows[i][3] + "\"}]}");
                }
            }
            string p = sb.ToString();
            return sb.ToString();
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