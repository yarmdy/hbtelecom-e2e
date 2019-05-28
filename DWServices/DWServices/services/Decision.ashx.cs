using DWServices.BLL;
using DWServices.Common;
using DWServices.DAL;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
namespace DWServices.services
{
    /// <summary>
    /// AlarmAnalysis 的摘要说明
    /// </summary>
    public class Decision : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string result = "";
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{\"ok\":false}");
                return;
            }

            //DateTime time = DateTime.Now;
            //string year = time.ToString("yyyy-MM-dd");
            //int min = (time.Minute / 15) * 15;
            //string hms = time.Hour + ":" + min + ":00";
            //DateTime dt = DateTime.Parse(year+" "+hms);
            DecisionDB da = new DecisionDB();
            string id = context.Request["id"];
            if (id == "wifi_chaxun")
            {
                string start = context.Request["start_t"];
                string end = context.Request["end_t"];
                DataSet s = da.GetWifiThisMinData(start, end);
                result = GetJsonByDataset(s);
                context.Response.Write(result);
                return;
            }
            var id2 = id.Substring(0, id.Length - 1);
            var idld = id.Substring(id.Length - 1);
            var act = context.Request.Params["act"] == null ? "" : "down";
            var tableid = context.Request.Params["tableid"] == null ? 0 : int.Parse(context.Request.Params["tableid"].ToString());
            switch (idld)
            {
                case "c":
                    idld = "CURRENT";
                    break;
                case "m":
                    idld = "MORE";
                    break;
                case "d":
                    idld = "DAY";
                    break;
                case "w":
                    idld = "WEEK";
                    break;
                case "l":
                    idld = "LONG";
                    break;
            }
            if (id2 == "ter")
            {
                id2 = "TERMINAL";
            }
            id2 = id2.ToUpper();
            string sql = "select DATA_TIME from DATA_DISPLAY where DATA_NAME = '" + id2 + "' and DATA_STATUS='" + idld + "'";
            DataTable timetable = OraConnect.ReadData(sql);
            DateTime dt = DateTime.Parse(timetable.Rows[0][0].ToString());
            DataSet set = null;
            if (id == "appc")
            {
                set = da.GetWebMinData(dt);

            }
            else if (id == "corec")
            {
                set = da.GetCoreMinData(dt);
            }
            else if (id == "wific")
            {
                string start = context.Request["start"];
                string end = context.Request["end"];
                set = da.GetWifiMinData(dt,start,end);
            }
            else if (id == "appm")
            {
                set = da.GetWebMinsData(dt);
            }
            else if (id == "corem")
            {
                set = da.GetCoreMinsData(dt);
            }
            else if (id == "wifim")
            {
                set = da.GetWifiMinsData(dt);
            }
            else if (id == "appd")
            {
                set = da.GetWebDayData(dt.AddDays(1));
            }
            else if (id == "cored")
            {
                set = da.GetCoreDayData(dt.AddDays(1));
            }
            else if (id == "rand")
            {
                set = da.GetIPRanDayData(dt.AddDays(1));
            }
            else if (id == "wifid")
            {
                set = da.GetWifiDayData(dt.AddDays(1));
            }
            else if (id == "terd")
            {
                set = da.GetTerminalDayData(dt.AddDays(1));
            }
            else if (id == "appw")
            {
                set = da.GetWebWeekData(dt.AddDays(1));
            }
            else if (id == "corew")
            {
                set = da.GetCoreWeekData(dt.AddDays(1));
            }
            else if (id == "ranw")
            {
                set = da.GetIPRanWeekData(dt.AddDays(1));
            }
            else if (id == "wifiw")
            {
                set = da.GetWifiWeekData(dt.AddDays(1));
            }
            else if (id == "terw")
            {
                set = da.GetTerminalWeekData(dt.AddDays(1));
            }
            else if (id == "appl")
            {
                set = da.GetWebLongData(dt);
            }
            else if (id == "corel")
            {
                set = da.GetCoreLongData(dt);
            }
            else if (id == "ranl")
            {
                set = da.GetIPRanLongData(dt);
            }
            else if (id == "wifil")
            {
                set = da.GetWifiLongData(dt);
            }
            else if (id == "terl")
            {
                set = da.GetTerminalLongData(dt);
            }

            if (act == "")
            {
                result = GetJsonByDataset(set);
                context.Response.Write(result);
            }
            else {
                //this.DataToCSV(set.Tables[0]);
                this.dataToCSVFile(context, set.Tables[tableid]);
            }
            
        }
        public void dataToCSVFile(HttpContext context, DataTable dt)
        {
            
            context.Response.ClearContent();
            context.Response.AddHeader("content-disposition", "attachment; filename=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
            context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            context.Response.ContentType = "application/excel";
            StringBuilder sb = new StringBuilder();
            string s;

            //Write Field Title  
            s = "";
            List<string> cols = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                //s += dc.ColumnName + ",";
                var colcname = EngtoHZ(dc.ColumnName);
                if (colcname != dc.ColumnName || colcname=="ECGI")
                {
                    cols.Add(dc.ColumnName);
                    s += colcname + ",";
                }
                
            }
            s = s.Substring(0, s.Length - 1) + "\r\n";
            sb.Append(s);

            //Write Row  
            foreach (DataRow dr in dt.Rows)
            {
                s = "";
                foreach (var o in cols)
                {
                    s += dr[o] + ",";
                }
                s = s.Substring(0, s.Length - 1) + "\r\n";
                sb.Append(s);
            }

            context.Response.Write(sb.ToString());
            //var buffer = Encoding.GetEncoding("gb2312").GetBytes(sb.ToString());
            //context.Response.OutputStream.Write(buffer,0,buffer.Length);
            context.Response.End(); 
        }
        public string EngtoHZ(string str) {
            if (str == "START_TIME" || str == "CREATETIME")
            {
                str = "开始时间";
            }
            else if (str == "VIDEODOOGL")
            {
                str = "视频优良率";
            }
            else if (str == "APPLICATION")
            {
                str = "应用ID";
            }
            else if (str == "SERER_IP")
            {
                str = "服务器IP";
            }
            else if (str == "STREAM_REQUEST")
            {
                str = "视频业务请求次数";
            }
            else if (str == "STREAM_DL_GOOD_TIMES")
            {
                str = "视频下载速率质差次数";
            }
            else if (str == "STREAM_HALT_GOOD_TIMES")
            {
                str = "视频播放卡顿质差次数";
            }
            else if (str == "WEBGOODL")
            {
                str = "web优良率";
            }
            else if (str == "APP_NAME")
            {
                str = "应用ID";
            }
            else if (str == "SERVER_NAME")
            {
                str = "服务器";
            }
            else if (str == "IP_ADDRESS")
            {
                str = "服务器IP";
            }
            else if (str == "WEB_FLOW")
            {
                str = "WEB业务下行流量（KB）";
            }
            else if (str == "WEB_COUNT")
            {
                str = "WEB业务次数";
            }
            else if (str == "PAGE_RATE")
            {
                str = "页面打开时延质差次数";
            }
            else if (str == "SCREEN_RATE")
            {
                str = "首屏打开请求次数";
            }
            else if (str == "WEB_RATE")
            {
                str = "首屏打开时延质差次数";
            }
            else if (str == "PLAYGOODL")
            {
                str = "游戏优良率";
            }
           
            else if (str == "PLAY_COUNT")
            {
                str = "游戏业务次数";
            }
            else if (str == "PLAY_RATE")
            {
                str = "游戏时延质差次数";
            }
            else if (str == "JSTXGOODL")
            {
                str = "即时通信优良率";
            }
            else if (str == "SIGNAL_COUNT")
            {
                str = "即时通信业务次数";
            }
            else if (str == "SIGNAL_FLOW")
            {
                str = "即时通信发送成功次数";
            }
            else if (str == "FZL")
            {
                str = "附着优良率";
            }
            else if (str == "SERVIDEGOOD")
            {
                str = "service优良率";
            }
            else if (str == "TAUGOOD")
            {
                str = "TAU优良率";
            }
            else if (str == "MME_ID")
            {
                str = "MME ID";
            }
            else if (str == "ATTACH_REQUEST")
            {
                str = "附着请求次数";
            }
            else if (str == "ATTACH_SUC")
            {
                str = "附着成功次数";
            }
            else if (str == "SERVICE_REQUEST")
            {
                str = "service请求次数";
            }
            else if (str == "SERVICE_SUC")
            {
                str = "service成功次数";
            }
            else if (str == "TAU_REQUEST")
            {
                str = "TAU请求次数";
            }
            else if (str == "TAU_SUC")
            {
                str = "TAU成功次数";
            }
            else if (str == "HOTSPOTCLASS")
            {
                str = "场景";
            }
            else if (str == "SC_NAME")
            {
                str = "小区名称";
            }
            else if (str == "WIFIGOOD")
            {
                str = "无线优良率";
            }
            else if (str == "WIFIGOOD2")
            {
                str = "小区感知优良率(天指标)";
            }
            else if (str == "CITYNAME")
            {
                str = "城市名称";
            }
            else if (str == "CITY")
            {
                str = "城市";
            }
            else if (str == "RAT")
            {
                str = "接入网";
            }
            else if (str == "ECGI")
            {
                str = "ECGI";
            }
            else if (str == "ECI")
            {
                str = "eNodeBID&CellID";
            }
            else if (str == "FDG_NUM")
            {
                str = "首屏显示时延质差次数";
            }
            else if (str == "FD_SUM")
            {
                str = "首屏显示次数";
            }
            else if (str == "GAME_NUM")
            {
                str = "在线游戏时延质差次数";
            }
            else if (str == "GAME_SUM")
            {
                str = "在线游戏业务次数";
            }
            else if (str == "NEWS_SUM")
            {
                str = "即时通信消息发送请求次数";
            }
            else if (str == "NEWS_NUM")
            {
                str = "即时通信消息发送成功次数";
            }
            else if (str == "PAGE_NUM")
            {
                str = "页面打开时延质差次数";
            }
            else if (str == "PAGE_SUM")
            {
                str = "页面打开总次数";
            }
            else if (str == "VIDEO_GNUM")
            {
                str = "视频播放速率质差次数";
            }
            else if (str == "VIDEO_BNUM")
            {
                str = "视频播放卡顿频率质差次数";
            }
            else if (str == "VIDEO_SNUM")
            {
                str = "视频播放次数";
            }
            else if (str == "BFLOW")
            {
                str = "下行流量";
            }
            else if (str == "TFLOW")
            {
                str = "上行流量";
            }
            else if (str == "SCREEMGOOD")
            {
                str = "首屏优良率";
            }
            else if (str == "PLAYGOOD")
            {
                str = "游戏优良率";
            }
            else if (str == "PHONEGOOD")
            {
                str = "即时通信优良率";
            }
            else if (str == "PAGEGOOD")
            {
                str = "页面优良率";
            }
            else if (str == "VIDEODOOD")
            {
                str = "视频速率优良率";
            }
            else if (str == "VIDEOGOODKD")
            {
                str = "视频卡顿优良率";
            }
            else if (str == "BRAND")
            {
                str = "终端品牌";
            }
            else if (str == "MODEL")
            {
                str = "终端型号";
            }
            else if (str == "FST_SCREEN_GOOD_TIMES")
            {
                str = "首屏质差次数";
            }
            else if (str == "FST_SCREEN_TIMES")
            {
                str = "首屏次数";
            }
            else if (str == "GAME_DELAY_GOOD_TIMES")
            {
                str = "游戏时延质差次数";
            }
            else if (str == "GAME_REQUEST_TIMES")
            {
                str = "游戏业务请求次数";
            }
            else if (str == "IM_SENT_REQUEST_TIMES")
            {
                str = "即时通信发送请求次数";
            }
            else if (str == "IM_SENT_SUC_TIMES")
            {
                str = "即时通信发送成功次数";
            }
            else if (str == "PAGE_OPEN_GOOD_TIMES")
            {
                str = "页面打开时延质差次数";
            }
            else if (str == "PAGE_OPEN_TIMES")
            {
                str = "页面打开请求次数";
            }
            else if (str == "STREAM_RATE_GOOD_TIMES")
            {
                str = "视频播放速率质差次数";
            }
            else if (str == "STREAM_STALL_GOOD_TIMES")
            {
                str = "视频播放卡顿质差次数";
            }
            else if (str == "STREAM_REQUEST_TIMES")
            {
                str = "视频播放请求次数";
            }
            else if (str == "MRCLASS") {
                str = "覆盖区域类型";
            }
            else if (str == "HOTSPOTCLASS")
            {
                str = "覆盖热点类型";
            }
            else if (str == "HOTSPOTNAME")
            {
                str = "热点名称";
            }

            return str;
        }
        /// <summary>
        /// 把dataset数据转换成json的格式
        /// </summary>
        /// <param name="ds">dataset数据集</param>
        /// <returns>json格式的字符串</returns>
        public static string GetJsonByDataset(DataSet ds)
        {
            if (ds == null || ds.Tables.Count <= 0 )
            {
                //如果查询到的数据为空则返回标记ok:false
                return "{\"ok\":false}";
            }
            bool isnull = true;
            for (int i=0;i<ds.Tables.Count;i++)
            {
                if (ds.Tables[i].Rows.Count>0)
                {
                    isnull = false;
                    break;
                }
            }
            if (isnull)
            {
                return "{\"ok\":false}";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"ok\":true,");
            //sb.Append("{\"" + jsonName + "\":[");
            foreach (DataTable dt in ds.Tables)
            {
                sb.Append(string.Format("\"{0}\":[", dt.TableName));

                //foreach (DataRow dr in dt.Rows)
                //{
                //    sb.Append("{");
                //    for (int i = 0; i < dr.Table.Columns.Count; i++)
                //    {
                //        sb.AppendFormat("\"{0}\":\"{1}\",", dr.Table.Columns[i].ColumnName.Replace("\"", "\\\"").Replace("\'", "\\\'"), ObjToStr(dr[i]).Replace("\"", "\\\"").Replace("\'", "\\\'")).Replace(Convert.ToString((char)13), "\\r\\n").Replace(Convert.ToString((char)10), "\\r\\n");
                //    }
                //    sb.Remove(sb.ToString().LastIndexOf(','), 1);
                //    sb.Append("},");
                //}

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("{");
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            sb.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\"");
                            if (j < dt.Columns.Count - 1)
                            {
                                sb.Append(",");
                            }
                        }
                        sb.Append("}");
                        if (i < dt.Rows.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }
                }

               // sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append("],");
            }
            sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append("}");
            return sb.ToString();
        }
        public static string ObjToStr(object ob)
        {
            if (ob == null)
            {
                return string.Empty;
            }
            else
                return ob.ToString();
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