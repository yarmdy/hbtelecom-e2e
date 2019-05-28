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
    /// ThreeNet 的摘要说明
    /// </summary>
    public class ThreeNet : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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
            string type = context.Request["type"];
            
            if(type == "query")
            {
                string sql1 = "";
                string sql2 = "";
                string date1 = context.Request["date1"];
                string date2 = context.Request["date2"];
                if(date1 == date2)
                {
                    date2 = DateTime.ParseExact(date2, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture).AddDays(1).ToString("yyyy-MM-dd");
                }
                //date1 = "2018-05-12";
                //date2 = "2018-05-13";
                sql1 = @"select
                          city,
                          OPERATOR,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)     as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)            as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                   as AckDelay
                        from PERDATA_CITY
                        where ctime >= to_date('"+date1+@"', 'yyyy-mm-dd') and ctime < to_date('"+date2+ @"', 'yyyy-mm-dd')
                        group by CITY, OPERATOR
                        union all
                        select
                          N'全省'                                                             city,
                          OPERATOR,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)       as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)             as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                 as AckDelay
                        from PERDATA_CITY
                        where ctime >= to_date('" + date1 + @"', 'yyyy-mm-dd') and ctime < to_date('" + date2 + @"', 'yyyy-mm-dd')
                        group by OPERATOR";
                sql2 = @"select
                          PCONTENT,
                          OPERATOR,
                          PTYPE,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)     as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)            as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                   as AckDelay
                        from PERDATA_CONTENT
                        where ctime >= to_date('" + date1 + @"', 'yyyy-mm-dd') and ctime < to_date('" + date2 + @"', 'yyyy-mm-dd')
                        group by PCONTENT, OPERATOR,PTYPE";


                DataSet ds = new DataSet();
                DataTable table1 = OraConnect.ReadData(sql1);
                DataTable table2 = OraConnect.ReadData(sql2);
                if (table1 != null)
                {
                    table1.TableName = "city";
                    ds.Tables.Add(table1.Copy());
                }
                if (table2 != null)
                {
                    table2.TableName = "app";
                    ds.Tables.Add(table2.Copy());
                }
                string s = GetJsonByDataset(ds);
                context.Response.Write(s);
            }
            else if (type == "exportcity") {
                context.Response.ContentType = "application/octet-stream";
                context.Response.Headers["Content-Disposition"] = "attachment;filename=app城市统计表.csv";
                //context.Response.Headers["Content-Encoding"] = "utf-8";
                string sql1 = "";
                string sql2 = "";
                string date1 = context.Request["date1"];
                string date2 = context.Request["date2"];
                if (date1 == date2)
                {
                    date2 = DateTime.ParseExact(date2, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture).AddDays(1).ToString("yyyy-MM-dd");
                }
                //date1 = "2018-05-12";
                //date2 = "2018-05-13";
                sql1 = @"select
                          city,
                          OPERATOR,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)     as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)            as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                   as AckDelay
                        from PERDATA_CITY
                        where ctime >= to_date('" + date1 + @"', 'yyyy-mm-dd') and ctime < to_date('" + date2 + @"', 'yyyy-mm-dd')
                        group by CITY, OPERATOR
                        union all
                        select
                          N'全省'                                                             city,
                          OPERATOR,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)       as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)             as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                 as AckDelay
                        from PERDATA_CITY
                        where ctime >= to_date('" + date1 + @"', 'yyyy-mm-dd') and ctime < to_date('" + date2 + @"', 'yyyy-mm-dd')
                        group by OPERATOR";
                sql2 = @"select
                          PCONTENT,
                          OPERATOR,
                          PTYPE,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)     as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)            as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                   as AckDelay
                        from PERDATA_CONTENT
                        where ctime >= to_date('" + date1 + @"', 'yyyy-mm-dd') and ctime < to_date('" + date2 + @"', 'yyyy-mm-dd')
                        group by PCONTENT, OPERATOR,PTYPE";


                DataSet ds = new DataSet();
                DataTable table1 = OraConnect.ReadData(sql1);
                //DataTable table2 = OraConnect.ReadData(sql2);
                if (table1 != null)
                {
                    table1.TableName = "city";
                    ds.Tables.Add(table1.Copy());
                }
                //if (table2 != null)
                //{
                //    table2.TableName = "app";
                //    ds.Tables.Add(table2.Copy());
                //}
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                sb1.Append("城市,标识,页面打开时延,首包时延,首屏时延,视频平均速率,视频卡顿频率,即时通讯成功率,游戏时延\r\n");
                sb2.Append("城市,内容,标识,分类,页面打开时延,首包时延,首屏时延,视频平均速率,视频卡顿频率,即时通讯成功率,游戏时延\r\n");
                foreach(DataRow row in table1.Rows){
                    sb1.Append(row["city"] + "," + row["OPERATOR"] + "," + row["PageOpenDelay"] + "," + row["FirstByteDelay"] + "," + row["FirstScreenDelay"] + "," + row["VideoAvgSpeed"] + "," + row["CacheRate"] + "," + row["ImSendRate"] + "," + row["AckDelay"]+ "\r\n");
                }
                //foreach (DataRow row in table2.Rows)
                //{
                //    sb2.Append(row["city"] + "," + row["PCONTENT"] + "," + row["OPERATOR"] + "," + row["PTYPE"] + "," + row["PageOpenDelay"] + "," + row["FirstByteDelay"] + "," + row["FirstScreenDelay"] + "," + row["VideoAvgSpeed"] + "," + row["CacheRate"] + "," + row["ImSendRate"] + "," + row["AckDelay"] + "\r\n");
                //}
                //System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "exptmp\app城市统计表"+DateTime.Now.ToString("yyyyMMddhhmmss")+".csv"), sb1.ToString());
                //System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "exptmp\app内容统计表" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv"), sb2.ToString());

                context.Response.BinaryWrite(Encoding.GetEncoding("gbk").GetBytes(sb1.ToString()));
            }else if(type == "exportcontent") {
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.Headers["Content-Disposition"] = "attachment;filename=app内容统计表.csv";
                    context.Response.Headers["Content-Encoding"] = "utf-8";
                    string sql1 = "";
                    string sql2 = "";
                    string date1 = context.Request["date1"];
                    string date2 = context.Request["date2"];
                    if (date1 == date2)
                    {
                        date2 = DateTime.ParseExact(date2, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture).AddDays(1).ToString("yyyy-MM-dd");
                    }
                    //date1 = "2018-05-12";
                    //date2 = "2018-05-13";
                    sql1 = @"select
                          city,
                          OPERATOR,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)     as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)            as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                   as AckDelay
                        from PERDATA_CITY
                        where ctime >= to_date('" + date1 + @"', 'yyyy-mm-dd') and ctime < to_date('" + date2 + @"', 'yyyy-mm-dd')
                        group by CITY, OPERATOR
                        union all
                        select
                          N'全省'                                                             city,
                          OPERATOR,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)       as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)             as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                 as AckDelay
                        from PERDATA_CITY
                        where ctime >= to_date('" + date1 + @"', 'yyyy-mm-dd') and ctime < to_date('" + date2 + @"', 'yyyy-mm-dd')
                        group by OPERATOR";
                    sql2 = @"select
                          PCONTENT,
                          OPERATOR,
                          PTYPE,
                          round(sum(PAGEOPENDELAY) / sum(PAGEOPENDELAY_COUNT), 2)     as PageOpenDelay,
                          round(sum(FIRSTBYTEDELAY) / sum(FIRSTBYTEDELAY_COUNT), 2)     as FirstByteDelay,
                          round(sum(FIRSTSCREENDELAY) / sum(FIRSTSCREENDELAY_COUNT), 2) as FirstScreenDelay,
                          round(sum(VIDEOAVGSPEED) / sum(VIDEOAVGSPEED_COUNT), 2)       as VideoAvgSpeed,
                          round(sum(CACHERATE) / sum(CACHERATE_COUNT), 2)               as CacheRate,
                          round(sum(IMSENDRATE) / sum(IMSENDRATE_COUNT), 2)            as ImSendRate,
                          round(sum(ACKDELAY) / sum(ACKDELAY_COUNT), 2)                   as AckDelay
                        from PERDATA_CONTENT
                        where ctime >= to_date('" + date1 + @"', 'yyyy-mm-dd') and ctime < to_date('" + date2 + @"', 'yyyy-mm-dd')
                        group by PCONTENT, OPERATOR,PTYPE";


                    DataSet ds = new DataSet();
                    //DataTable table1 = OraConnect.ReadData(sql1);
                    DataTable table2 = OraConnect.ReadData(sql2);
                    //if (table1 != null)
                    //{
                    //    table1.TableName = "city";
                    //    ds.Tables.Add(table1.Copy());
                    //}
                    if (table2 != null)
                    {
                        table2.TableName = "app";
                        ds.Tables.Add(table2.Copy());
                    }
                    //StringBuilder sb1 = new StringBuilder();
                    StringBuilder sb2 = new StringBuilder();
                    //sb1.Append("城市,标识,页面打开时延,首包时延,首屏时延,视频平均速率,视频卡顿频率,即时通讯成功率,游戏时延\r\n");
                    sb2.Append("内容,标识,分类,页面打开时延,首包时延,首屏时延,视频平均速率,视频卡顿频率,即时通讯成功率,游戏时延\r\n");
                    //foreach (DataRow row in table1.Rows)
                    //{
                    //    sb1.Append(row["city"] + "," + row["OPERATOR"] + "," + row["PageOpenDelay"] + "," + row["FirstByteDelay"] + "," + row["FirstScreenDelay"] + "," + row["VideoAvgSpeed"] + "," + row["CacheRate"] + "," + row["ImSendRate"] + "," + row["AckDelay"] + "\r\n");
                    //}
                    foreach (DataRow row in table2.Rows)
                    {
                        sb2.Append(row["PCONTENT"] + "," + row["OPERATOR"] + "," + row["PTYPE"] + "," + row["PageOpenDelay"] + "," + row["FirstByteDelay"] + "," + row["FirstScreenDelay"] + "," + row["VideoAvgSpeed"] + "," + row["CacheRate"] + "," + row["ImSendRate"] + "," + row["AckDelay"] + "\r\n");
                    }
                    //System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "exptmp\app城市统计表" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv"), sb1.ToString());
                    //System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "exptmp\app内容统计表" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv"), sb2.ToString());
                    context.Response.BinaryWrite(Encoding.GetEncoding("gbk").GetBytes(sb2.ToString()));
                
            }
            
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