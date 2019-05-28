using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// citygoodrate 的摘要说明
    /// </summary>
    public class citygoodrate : IHttpHandler,System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{\"ok\":false}");
                return;
            }
            string type = context.Request["type"];

            if(type == "datetime")
            {
                string date = GetDateTime();
                string[] d = date.Split(' ');
                string year = d[0];
                string time = d[1].Split(':')[0] + ":00";
                string result = "{\"year\":\"" + year+"\",\"time\":\""+time+"\"}";
                context.Response.Write(result);
            }else if(type == "one")
            {
                string date = context.Request["date"];
                string time = context.Request["time"];
                string datetime = date + " " + time;
                string sql = @"select a.city,a.ctime,
                                trunc(a.web,4)*100 web,case when a.web-b.web<-0.01 then 1 else 0 end webbad,
                                trunc(a.video,4)*100 video,case when a.video-b.video<-0.01 then 1 else 0 end videobad,
                                trunc(a.game,4)*100 game,case when a.game-b.game<-0.01 then 1 else 0 end gamebad,
                                trunc(a.im,4)*100 im,case when a.im-b.im<-0.01 then 1 else 0 end imbad 
                                from(
                                select city,ctime,
                                (decode(page_times, 0, 0.8, 0.8 * (1 - 1.0 * page_bad_times / page_times))+ 0.2 * (decode(fst_times, 0, 1, 1-1.0 * fst_bad_times / fst_times))) web,
                                (decode(VIDEO_times, 0, 0.8, 0.8 * (1-1.0 * VIDEO_ratebad_times / VIDEO_times)) + 0.2 *(decode(VIDEO_times, 0, 1, 1-1.0 * VIDEO_stallbad_times / VIDEO_times))) video,
                                (decode(GAME_times, 0, 1, 1-1.0 * GAME_bad_times / GAME_times)) game,
                                (decode(im_times, 0, 1, 1-1.0 * im_bad_times / im_times)) im
                                from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')
                                )a left join(
                                select city,ctime,
                                (decode(page_times, 0, 0.8, 0.8 * (1 - 1.0 * page_bad_times / page_times))+ 0.2 * (decode(fst_times, 0, 1, 1-1.0 * fst_bad_times / fst_times))) web,
                                (decode(VIDEO_times, 0, 0.8, 0.8 * (1-1.0 * VIDEO_ratebad_times / VIDEO_times)) + 0.2 *(decode(VIDEO_times, 0, 1, 1-1.0 * VIDEO_stallbad_times / VIDEO_times))) video,
                                (decode(GAME_times, 0, 1, 1-1.0 * GAME_bad_times / GAME_times)) game,
                                (decode(im_times, 0, 1, 1-1.0 * im_bad_times / im_times)) im
                                from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')-1
                                )b on a.city=b.city";
                string sql2 = @"select
                                a.city,a.ctime,
                                trunc(decode(page_times, 0, 0.8, 0.8 * (1 - 1.0 * page_bad_times / page_times))+ 0.2 * (decode(fst_times, 0, 1, 1-1.0 * fst_bad_times / fst_times)),4)*100 web,
                                trunc(decode(VIDEO_times, 0, 0.8, 0.8 * (1-1.0 * VIDEO_ratebad_times / VIDEO_times)) + 0.2 *(decode(VIDEO_times, 0, 1, 1-1.0 * VIDEO_stallbad_times / VIDEO_times)),4)*100 video,
                                trunc(decode(GAME_times, 0, 1, 1-1.0 * GAME_bad_times / GAME_times),4)*100 game,
                                trunc(decode(im_times, 0, 1, 1-1.0 * im_bad_times / im_times),4)*100 im
                                from(
                                select * from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')-1
                                union all
                                select * from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')-2
                                union all
                                select * from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')-3
                                union all
                                select * from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')-4
                                union all
                                select * from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')-5
                                union all
                                select * from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')-6
                                union all
                                select * from city_goodrate where ctime=to_date('" + datetime + @"','yyyy/mm/dd hh24:mi:ss')-7)a order by a.city,a.ctime";
                
                DataTable dt1 = OraConnect.ReadData(sql);
                DataTable dt2 = OraConnect.ReadData(sql2);
                DataSet ds = new DataSet();
                if (dt1 != null && dt2 != null)
                {
                    dt1.TableName = "city";
                    dt2.TableName = "timer";
                    ds.Tables.Add(dt1.Copy());
                    ds.Tables.Add(dt2.Copy());
                }
                string result = GetJsonByDataset(ds);
                context.Response.Write(result);
            }
            else if(type=="two")
            {
                string dateo = context.Request["dateo"];
                string datet = context.Request["datet"];
                bool isEq = false;
                if(dateo == datet)
                {
                    datet = DateTime.ParseExact(datet, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture).AddDays(1).ToString("yyyy/MM/dd");
                    isEq = true;
                }
                string sql3 = @"select city,
                            trunc(decode(sum(page_times), 0, 0.8, 0.8 * (1 - 1.0 * sum(page_bad_times) / sum(page_times)))+ 0.2 * (decode(sum(fst_times), 0, 1, 1-1.0 * sum(fst_bad_times) / sum(fst_times))),4)*100 web,
                            trunc(decode(sum(VIDEO_times), 0, 0.8, 0.8 * (1-1.0 * sum(VIDEO_ratebad_times) / sum(VIDEO_times))) + 0.2 *(decode(sum(VIDEO_times), 0, 1, 1-1.0 * sum(VIDEO_stallbad_times) / sum(VIDEO_times))),4)*100 video,
                            trunc(decode(sum(GAME_times), 0, 1, 1-1.0 * sum(GAME_bad_times) / sum(GAME_times)),4)*100 game,
                            trunc(decode(sum(im_times), 0, 1, 1-1.0 * sum(im_bad_times) / sum(im_times)),4)*100 im
                            from city_goodrate where 
                            ctime>=to_date('"+dateo+ @"','yyyy/mm/dd') and ctime<to_date('" + datet + @"','yyyy/mm/dd')+1
                            group by city";
                string sql4 = "";
                if(isEq)
                {
                    sql4 = @"select city,trunc(ctime) ctime,
                            trunc(decode(sum(page_times), 0, 0.8, 0.8 * (1 - 1.0 * sum(page_bad_times) / sum(page_times)))+ 0.2 * (decode(sum(fst_times), 0, 1, 1-1.0 * sum(fst_bad_times) / sum(fst_times))),4)*100 web,
                            trunc(decode(sum(VIDEO_times), 0, 0.8, 0.8 * (1-1.0 * sum(VIDEO_ratebad_times) / sum(VIDEO_times))) + 0.2 *(decode(sum(VIDEO_times), 0, 1, 1-1.0 * sum(VIDEO_stallbad_times) / sum(VIDEO_times))),4)*100 video,
                            trunc(decode(sum(GAME_times), 0, 1, 1-1.0 * sum(GAME_bad_times) / sum(GAME_times)),4)*100 game,
                            trunc(decode(sum(im_times), 0, 1, 1-1.0 * sum(im_bad_times) / sum(im_times)),4)*100 im
                            from city_goodrate where 
                            ctime>=to_date('" + dateo + @"','yyyy/mm/dd')-7 and ctime<to_date('" + datet + @"','yyyy/mm/dd')
                            group by city,trunc(ctime)
                            order by city,ctime";
                }
                else
                {
                    sql4 = @"select city,trunc(ctime) ctime,
                            trunc(decode(sum(page_times), 0, 0.8, 0.8 * (1 - 1.0 * sum(page_bad_times) / sum(page_times)))+ 0.2 * (decode(sum(fst_times), 0, 1, 1-1.0 * sum(fst_bad_times) / sum(fst_times))),4)*100 web,
                            trunc(decode(sum(VIDEO_times), 0, 0.8, 0.8 * (1-1.0 * sum(VIDEO_ratebad_times) / sum(VIDEO_times))) + 0.2 *(decode(sum(VIDEO_times), 0, 1, 1-1.0 * sum(VIDEO_stallbad_times) / sum(VIDEO_times))),4)*100 video,
                            trunc(decode(sum(GAME_times), 0, 1, 1-1.0 * sum(GAME_bad_times) / sum(GAME_times)),4)*100 game,
                            trunc(decode(sum(im_times), 0, 1, 1-1.0 * sum(im_bad_times) / sum(im_times)),4)*100 im
                            from city_goodrate where 
                            ctime>=to_date('" + dateo + @"','yyyy/mm/dd') and ctime<to_date('" + datet + @"','yyyy/mm/dd')+1
                            group by city,trunc(ctime)
                            order by city,ctime";
                }
                DataSet ds2 = new DataSet();
                DataTable dt3 = OraConnect.ReadData(sql3);
                DataTable dt4 = OraConnect.ReadData(sql4);
                if(dt3 != null && dt4 != null)
                {
                    dt3.TableName = "city2";
                    dt4.TableName = "timer2";
                    ds2.Tables.Add(dt3.Copy());
                    ds2.Tables.Add(dt4.Copy());
                }
                string result2 = GetJsonByDataset(ds2);
                context.Response.Write(result2);
            }
        }

        private string GetDateTime()
        {
            DateTime d = DateTime.Now;
            return d.ToString("yyyy-MM-dd HH:mm:ss");
        }



        public static string GetJsonByDataset(DataSet ds)
        {
            if (ds == null || ds.Tables.Count <= 0)
            {
                //如果查询到的数据为空则返回标记ok:false
                return "{\"ok\":false}";
            }
            bool isnull = true;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (ds.Tables[i].Rows.Count > 0)
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
            foreach (DataTable dt in ds.Tables)
            {
                sb.Append(string.Format("\"{0}\":[", dt.TableName));

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



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}