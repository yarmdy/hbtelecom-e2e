using DWServices.Common;
using DWServices.DAL;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.BLL
{
    public class DecisionDB
    {
       

        #region 业务
        //获取业务十五分钟数据
        public DataSet GetWebMinData(DateTime time)
        {
            DateTime minbegintime = time.AddMinutes(-15);
            DateTime minendtime = time;
            //string sqlweb = "select * from WEB_MIN t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss')";
            string sqlweb = "select trunc(decode(t.WEB_COUNT,0,0.8,0.8 * (1 - t.PAGE_RATE / t.WEB_COUNT)) + decode(t.SCREEN_RATE,0,0.2,0.2 * (1-t.WEB_RATE / t.SCREEN_RATE)), 4) WEBGOODL, t.* from WEB_MIN t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') order by t.IP_ADDRESS,t.CREATETIME";
            //string sqlvideo = "select * from VIDEO_MIN t where t.START_TIME > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss')";
            string sqlvideo = "select trunc(decode(t.STREAM_REQUEST,0,0.8,(1-(t.STREAM_DL_GOOD_TIMES / t.STREAM_REQUEST)) * 0.8) + decode(t.STREAM_REQUEST,0,0.2,(1-(t.STREAM_HALT_GOOD_TIMES / t.STREAM_REQUEST)) * 0.2), 4) VIDEODOOGL,t.* from VIDEO_MIN t where t.START_TIME > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') order by t.SERER_IP,t.START_TIME";
           // string sqlsignal = "select * from SIGNAL_MIN t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss')";
            string sqlsignal = "select trunc(decode(t.SIGNAL_COUNT,0,1,1-t.SIGNAL_FLOW / t.SIGNAL_COUNT), 4) JSTXGOODL,t.* from SIGNAL_MIN t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') order by t.IP_ADDRESS,t.CREATETIME";
           // string sqlplay = "select * from PLAY_MIN t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss')";
            string sqlplay = "select trunc(decode(t.PLAY_COUNT,0,1,1-t.PLAY_RATE / t.PLAY_COUNT), 4) PLAYGOODL,t.* from PLAY_MIN t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') order by t.IP_ADDRESS,t.CREATETIME";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sqlweb);
            webdata.TableName = "WEB_MIN";
            ds.Tables.Add(webdata.Copy());
            DataTable videodata = OraConnect.ReadData(sqlvideo);
            videodata.TableName = "VIDEO_MIN";
            ds.Tables.Add(videodata.Copy());
            DataTable signaldata = OraConnect.ReadData(sqlsignal);
            signaldata.TableName = "SIGNAL_MIN";
            ds.Tables.Add(signaldata.Copy());
            DataTable playdata = OraConnect.ReadData(sqlplay);
            playdata.TableName = "PLAY_MIN";
            ds.Tables.Add(playdata.Copy());

            return ds;
        }
        //获取业务当天多个时段数据
        public DataSet GetWebMinsData(DateTime time)
        {
            string minbegintime = time.ToString("yyy-MM-dd");
            string minendtime = time.AddDays(1).ToString("yyy-MM-dd");
            string sqlweb = "select trunc(decode(s.WEB_COUNT,0,0.8,0.8 * (1 - s.PAGE_RATE / s.WEB_COUNT)) + decode(s.SCREEN_RATE,0,0.2,0.2 * (1-s.WEB_RATE / s.SCREEN_RATE)), 4) WEBGOODL, s.* from WEB_MIN s where createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') and ip_address in (select ip_address from web_min t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ip_address HAVING count(t.ip_address) >= 3 and count(t.ip_address)<=5) order by s.IP_ADDRESS,s.CREATETIME";
            string sqlvideo = "select trunc(decode(s.STREAM_REQUEST,0,0.8,(1-s.STREAM_DL_GOOD_TIMES / s.STREAM_REQUEST) * 0.8) + decode(s.STREAM_REQUEST,0,0.8,(1-s.STREAM_HALT_GOOD_TIMES / s.STREAM_REQUEST) * 0.2), 4) VIDEODOOGL,s.* from VIDEO_MIN s where START_TIME > to_date('" + minbegintime + " ', 'yyyy-mm-dd hh24:mi:ss') and START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') and SERER_IP in (select SERER_IP from VIDEO_MIN t where t.START_TIME > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.SERER_IP HAVING count(t.SERER_IP) >= 3 and count(t.SERER_IP)<=5) order by s.SERER_IP,s.START_TIME";
            string sqlsignal = "select trunc(decode(s.SIGNAL_COUNT,0,1,1-s.SIGNAL_FLOW / s.SIGNAL_COUNT), 4) JSTXGOODL,s.* from SIGNAL_MIN s where createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') and ip_address in (select ip_address from SIGNAL_MIN t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ip_address HAVING count(t.ip_address) >= 3 and count(t.ip_address)<=5) order by s.IP_ADDRESS,s.CREATETIME";
            string sqlplay = "select trunc(decode(s.PLAY_COUNT,0,1,1-s.PLAY_RATE / s.PLAY_COUNT), 4) PLAYGOODL, s.* from PLAY_MIN s where createtime > to_date('" + minbegintime + " ', 'yyyy-mm-dd hh24:mi:ss') and createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') and ip_address in (select ip_address from PLAY_MIN t where t.createtime > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ip_address HAVING count(t.ip_address) >= 3 and count(t.ip_address)<=5) order by s.IP_ADDRESS,s.CREATETIME";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sqlweb);
            webdata.TableName = "WEB_MIN";
            ds.Tables.Add(webdata.Copy());
            DataTable videodata = OraConnect.ReadData(sqlvideo);
            videodata.TableName = "VIDEO_MIN";
            ds.Tables.Add(videodata.Copy());
            DataTable signaldata = OraConnect.ReadData(sqlsignal);
            signaldata.TableName = "SIGNAL_MIN";
            ds.Tables.Add(signaldata.Copy());
            DataTable playdata = OraConnect.ReadData(sqlplay);
            playdata.TableName = "PLAY_MIN";
            ds.Tables.Add(playdata.Copy());
            return ds;
        }
        //获取业务天数据
        public DataSet GetWebDayData(DateTime time)
        {
            string daytime = time.AddDays(-1).ToString("yyy-MM-dd");
            string sqlweb = "select trunc(decode(t.WEB_COUNT,0,1,0.8 * (1 - t.PAGE_RATE / t.WEB_COUNT)) + decode(t.SCREEN_RATE,0,1,0.2 * (1-t.WEB_RATE / t.SCREEN_RATE)), 4) WEBGOODL, t.* from web_day t where trunc(t.createtime)=to_date('" + daytime + "', 'yyyy-mm-dd') order by t.IP_ADDRESS,t.CREATETIME";
            string sqlvideo = "select trunc(decode(t.STREAM_REQUEST,0,0.8,(1-t.STREAM_DL_GOOD_TIMES / t.STREAM_REQUEST) * 0.8) + decode(t.STREAM_REQUEST,0,0.2,(1-t.STREAM_HALT_GOOD_TIMES / t.STREAM_REQUEST) * 0.2), 4) VIDEODOOGL,t.* from VIDEO_DAY t where trunc(t.START_TIME)=to_date('" + daytime + "', 'yyyy-mm-dd') order by t.SERER_IP,t.START_TIME";
            string sqlsignal = "select trunc(decode(t.SIGNAL_COUNT,0,1,1-t.SIGNAL_FLOW / t.SIGNAL_COUNT), 4) JSTXGOODL,t.* from SIGNAL_DAY t where trunc(t.createtime)=to_date('" + daytime + "', 'yyyy-mm-dd') order by t.IP_ADDRESS,t.CREATETIME";
            string sqlplay = "select trunc(decode(t.PLAY_COUNT,0,1,1-t.PLAY_RATE / t.PLAY_COUNT), 4) PLAYGOODL,t.* from PLAY_DAY t where trunc(t.createtime)=to_date('" + daytime + "', 'yyyy-mm-dd') order by t.IP_ADDRESS,t.CREATETIME";

            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sqlweb);
            webdata.TableName = "WEB_MIN";
            ds.Tables.Add(webdata.Copy());
            DataTable videodata = OraConnect.ReadData(sqlvideo);
            videodata.TableName = "VIDEO_MIN";
            ds.Tables.Add(videodata.Copy());
            DataTable signaldata = OraConnect.ReadData(sqlsignal);
            signaldata.TableName = "SIGNAL_MIN";
            ds.Tables.Add(signaldata.Copy());
            DataTable playdata = OraConnect.ReadData(sqlplay);
            playdata.TableName = "PLAY_MIN";
            ds.Tables.Add(playdata.Copy());
            return ds;
        }
        //获取业务周数据
        public DataSet GetWebWeekData(DateTime time)
        {
            string begintimestr = time.AddDays(-8).ToString("yyy-MM-dd");
            string endtimestr = time.AddDays(-1).ToString("yyy-MM-dd");
            string begindaystime = begintimestr;
            string enddaystime = endtimestr;

            string sqlweb = "select trunc(decode(s.WEB_COUNT,0,0.8,0.8 * (1 - s.PAGE_RATE / s.WEB_COUNT)) + decode(s.SCREEN_RATE,0,0.2,0.2 * (1-s.WEB_RATE / s.SCREEN_RATE)), 4) WEBGOODL, s.* from web_day s where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from web_day t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) >= 3 and max(trunc(t.createtime))=to_date('" + enddaystime + "', 'yyyy-mm-dd') ) order by s.IP_ADDRESS,s.CREATETIME";
            string sqlvideo = "select trunc(decode(s.STREAM_REQUEST,0,0.8,(1-s.STREAM_DL_GOOD_TIMES / s.STREAM_REQUEST) * 0.8) + decode(s.STREAM_REQUEST,0,0.2,(1-s.STREAM_HALT_GOOD_TIMES / s.STREAM_REQUEST) * 0.2), 4) VIDEODOOGL,s.* from VIDEO_DAY s where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and SERER_IP in (select SERER_IP from VIDEO_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.SERER_IP HAVING count(t.SERER_IP) >= 3 and max(trunc(t.START_TIME))=to_date('" + enddaystime + "', 'yyyy-mm-dd') ) order by s.SERER_IP,s.START_TIME";
            string sqlsignal = "select trunc(decode(s.SIGNAL_COUNT,0,1,1-s.SIGNAL_FLOW / s.SIGNAL_COUNT), 4) JSTXGOODL,s.* from SIGNAL_DAY s where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from SIGNAL_DAY t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) >= 3 and max(trunc(t.createtime))=to_date('" + enddaystime + "', 'yyyy-mm-dd') ) order by s.IP_ADDRESS,s.CREATETIME";
            string sqlplay = "select trunc(decode(s.PLAY_COUNT,0,1,1-s.PLAY_RATE / s.PLAY_COUNT), 4) PLAYGOODL, s.* from PLAY_DAY s where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from PLAY_DAY t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) >= 3 and max(trunc(t.createtime))=to_date('" + enddaystime + "', 'yyyy-mm-dd') ) order by s.IP_ADDRESS,s.CREATETIME";

            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sqlweb);
            webdata.TableName = "WEB_MIN";
            ds.Tables.Add(webdata.Copy());
            DataTable videodata = OraConnect.ReadData(sqlvideo);
            videodata.TableName = "VIDEO_MIN";
            ds.Tables.Add(videodata.Copy());
            DataTable signaldata = OraConnect.ReadData(sqlsignal);
            signaldata.TableName = "SIGNAL_MIN";
            ds.Tables.Add(signaldata.Copy());
            DataTable playdata = OraConnect.ReadData(sqlplay);
            playdata.TableName = "PLAY_MIN";
            ds.Tables.Add(playdata.Copy());
            return ds;
        }
        //获取业务长期质差数据
        public DataSet GetWebLongData(DateTime time)
        {
            DateTime t1 = DateTime.Now;
            DateTime t2 = DateTime.Parse("2017-08-30");
            System.TimeSpan t3 = t1 - t2;
            double ddays = t3.TotalDays;
            int iweeks = (int)ddays / 7;
            string begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
            string enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
            int j = 0;
            int z = 0;
            int m = 0;
            int n = 0;
            int r = 0;
            int t = 0;
            int p = 0;
            int l = 0;
            DataSet ds = new DataSet();
            DataTable webdata = new DataTable();
            webdata.TableName = "WEB_MIN";
            DataTable videodata = new DataTable();
            videodata.TableName = "VIDEO_MIN";
            DataTable signaldata = new DataTable();
            signaldata.TableName = "SIGNAL_MIN";
            DataTable playdata = new DataTable();
            playdata.TableName = "PLAY_MIN";
            for (int i = 1; i <= iweeks; i++)
            {
                if (i == 1)
                {
                    begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
                }
                else
                {
                    begindaystime = time.AddDays((i * -7) - 1).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(((i - 1) * -7) - 1).ToString("yyy-MM-dd");
                }
                string sqlvideo = "select trunc(decode(s.STREAM_REQUEST,0,0.8,(1-s.STREAM_DL_GOOD_TIMES / s.STREAM_REQUEST) * 0.8) + decode(s.STREAM_REQUEST,0,0.2,(1-s.STREAM_HALT_GOOD_TIMES / s.STREAM_REQUEST) * 0.2), 4) VIDEODOOGL,s.* from VIDEO_DAY s where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and SERER_IP in (select SERER_IP from VIDEO_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.SERER_IP HAVING count(t.SERER_IP) > 3 )";
                videodata = OraConnect.ReadData(sqlvideo);
                if (videodata != null && videodata.Rows.Count > 0)
                {
                    m = m + 1;
                    n = 0;
                    if (m == 3)
                    {
                        if (i == 1)
                        {
                            ds.Tables.Add(videodata.Copy());
                        }
                        else
                        {
                            object[] obj = new object[ds.Tables[0].Columns.Count];
                            for (int g = 0; g < videodata.Rows.Count; i++)
                            {
                                videodata.Rows[i].ItemArray.CopyTo(obj, 0);
                                ds.Tables[0].Rows.Add(obj);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    n = n + 1;
                    m = 0;
                    if (n == 3)
                    {
                        break;
                    }
                }
            }
            for (int i = 1; i <= iweeks; i++)
            {
                if (i == 1)
                {
                    begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
                }
                else
                {
                    begindaystime = time.AddDays((i * -7) - 1).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(((i - 1) * -7) - 1).ToString("yyy-MM-dd");
                }
                string sqlweb = "select trunc(decode(s.WEB_COUNT,0,0.8,0.8 * (1 - s.PAGE_RATE / s.WEB_COUNT)) + decode(s.SCREEN_RATE,0,0.2,0.2 * (1-s.WEB_RATE / s.SCREEN_RATE)), 4) WEBGOODL, s.* from web_day s where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from web_day t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) > 3 )";
                webdata = OraConnect.ReadData(sqlweb);
                if (webdata != null && webdata.Rows.Count > 0)
                {
                    j = j + 1;
                    z = 0;
                    if (j == 3)
                    {
                        if (i == 1)
                        {
                            ds.Tables.Add(webdata.Copy());
                        }
                        else
                        {
                            object[] obj = new object[ds.Tables[0].Columns.Count];
                            for (int g = 0; g < webdata.Rows.Count; i++)
                            {
                                webdata.Rows[i].ItemArray.CopyTo(obj, 0);
                                ds.Tables[0].Rows.Add(obj);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    z = z + 1;
                    j = 0;
                    if (z == 3)
                    {
                        break;
                    }
                }
            }
            for (int i = 1; i <= iweeks; i++)
            {
                if (i == 1)
                {
                    begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
                }
                else
                {
                    begindaystime = time.AddDays((i * -7) - 1).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(((i - 1) * -7) - 1).ToString("yyy-MM-dd");
                }
                string sqlsignal = "select trunc(decode(s.SIGNAL_COUNT,0,1,1-s.SIGNAL_FLOW / s.SIGNAL_COUNT), 4) JSTXGOODL,s.* from SIGNAL_DAY s where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from SIGNAL_DAY t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) > 3 )";
                signaldata = OraConnect.ReadData(sqlsignal);
               
                
                if (signaldata != null && signaldata.Rows.Count > 0)
                {
                    r = r + 1;
                    t = 0;
                    if (r == 3)
                    {
                        if (i == 1)
                        {
                            ds.Tables.Add(signaldata.Copy());
                        }
                        else
                        {
                            object[] obj = new object[ds.Tables[0].Columns.Count];
                            for (int g = 0; g < signaldata.Rows.Count; i++)
                            {
                                signaldata.Rows[i].ItemArray.CopyTo(obj, 0);
                                ds.Tables[0].Rows.Add(obj);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    t = t + 1;
                    r = 0;
                    if (t == 3)
                    {
                        break;
                    }
                }
            }
            for (int i = 1; i <= iweeks; i++)
            {
                if (i == 1)
                {
                    begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
                }
                else
                {
                    begindaystime = time.AddDays((i * -7) - 1).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(((i - 1) * -7) - 1).ToString("yyy-MM-dd");
                }
                string sqlplay = "select trunc(decode(s.PLAY_COUNT,0,1,1-s.PLAY_RATE / s.PLAY_COUNT), 4) PLAYGOODL, s.* from PLAY_DAY s where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from PLAY_DAY t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) > 3 )";
                playdata = OraConnect.ReadData(sqlplay);
                if (playdata != null && playdata.Rows.Count > 0)
                {
                    p = p + 1;
                    l = 0;
                    if (p == 3)
                    {
                        if (i == 1)
                        {
                            ds.Tables.Add(playdata.Copy());
                        }
                        else
                        {
                            object[] obj = new object[ds.Tables[0].Columns.Count];
                            for (int g = 0; g < playdata.Rows.Count; i++)
                            {
                                playdata.Rows[i].ItemArray.CopyTo(obj, 0);
                                ds.Tables[0].Rows.Add(obj);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    l = l + 1;
                    p = 0;
                    if (l == 3)
                    {
                        break;
                    }
                }

            }

            return ds;
        }

        #endregion

        #region 核心网

        //获取核心网十五分钟数据
        public DataSet GetCoreMinData(DateTime time)
        {
            DateTime minbegintime = time.AddMinutes(-15);
            DateTime minendtime = time;
            //string sql = "select * from CORE_MIN t where t.START_TIME > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss')";
            string sql = " select trunc(decode(t.ATTACH_REQUEST,0,1,t.ATTACH_SUC / t.ATTACH_REQUEST), 4） as FZL, trunc(decode(t.SERVICE_REQUEST,0,1,t.SERVICE_SUC / t.SERVICE_REQUEST), 4) as SERVIDEGOOD, trunc(decode(t.TAU_REQUEST,0,1,t.TAU_SUC / t.TAU_REQUEST), 4) as TAUGOOD, t.* from CORE_MIN t where t.START_TIME > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') order by t.MME_ID,t.START_TIME";
            string sqlFlow = "select * from FLOW_MIN";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "CORE_MIN";
            ds.Tables.Add(webdata.Copy());
            var flowdt = OraConnect.ReadData(sqlFlow);
            flowdt.TableName = "FLOW_MIN";
            ds.Tables.Add(flowdt.Copy());
            return ds;
        }
        //获取核心网当天多个时段数据
        public DataSet GetCoreMinsData(DateTime time)
        {
            string minbegintime = time.ToString("yyy-MM-dd");
            string minendtime = time.AddDays(1).ToString("yyy-MM-dd");
            //string sql = "select * from CORE_MIN where START_TIME > to_date('" + minbegintime + " ', 'yyyy-mm-dd hh24:mi:ss') and START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') and MME_ID in (select MME_ID from CORE_MIN t where t.START_TIME > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.MME_ID HAVING count(t.MME_ID) >= 3 and count(t.MME_ID)<=5)";
            string sql = "select trunc(decode(s.ATTACH_REQUEST,0,1,s.ATTACH_SUC / s.ATTACH_REQUEST), 4） as FZL, trunc(decode(s.SERVICE_REQUEST,0,1,s.SERVICE_SUC / s.SERVICE_REQUEST), 4) as SERVIDEGOOD, trunc(decode(s.TAU_REQUEST,0,1,s.TAU_SUC / s.TAU_REQUEST), 4) as TAUGOOD,s.* from CORE_MIN s where START_TIME > to_date('" + minbegintime + " ', 'yyyy-mm-dd hh24:mi:ss') and START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') and MME_ID in (select MME_ID from CORE_MIN t where t.START_TIME > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.MME_ID HAVING count(t.MME_ID) >= 3 and count(t.MME_ID) <= 5) order by s.MME_ID,s.START_TIME";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "CORE_MIN";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取核心网天数据
        public DataSet GetCoreDayData(DateTime time)
        {
            string daytime = time.AddDays(-1).ToString("yyy-MM-dd");
            string sql = " select trunc(decode(t.ATTACH_REQUEST,0,1,t.ATTACH_SUC / t.ATTACH_REQUEST), 4） as FZL, trunc(decode(t.SERVICE_REQUEST,0,1,t.SERVICE_SUC / t.SERVICE_REQUEST), 4) as SERVIDEGOOD, trunc(decode(t.TAU_REQUEST,0,1,t.TAU_SUC / t.TAU_REQUEST), 4) as TAUGOOD, t.* from CORE_DAY t where trunc(t.START_TIME)=to_date('" + daytime + "', 'yyyy-mm-dd') order by t.MME_ID,t.START_TIME";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "CORE_DAY";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取核心网周数据
        public DataSet GetCoreWeekData(DateTime time)
        {
            string begintimestr = time.AddDays(-8).ToString("yyy-MM-dd");
            string endtimestr = time.AddDays(-1).ToString("yyy-MM-dd");
            string begindaystime = begintimestr;
            string enddaystime = endtimestr;
            string sql = "select trunc(decode(s.ATTACH_REQUEST,0,1,s.ATTACH_SUC / s.ATTACH_REQUEST), 4） as FZL, trunc(decode(s.SERVICE_REQUEST,0,1,s.SERVICE_SUC / s.SERVICE_REQUEST), 4) as SERVIDEGOOD, trunc(decode(s.TAU_REQUEST,0,1,s.TAU_SUC / s.TAU_REQUEST), 4) as TAUGOOD,s.* from CORE_DAY s where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and MME_ID in (select MME_ID from CORE_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.MME_ID HAVING count(t.MME_ID) >= 3 and max(trunc(t.START_TIME))=to_date('" + enddaystime + "', 'yyyy-mm-dd') ) order by s.MME_ID,s.START_TIME";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "CORE_DAY";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取核心网长期质差数据
        public DataSet GetCoreLongData(DateTime time)
        {
            DateTime t1 = DateTime.Now;
            DateTime t2 = DateTime.Parse("2017-08-30");
            System.TimeSpan t3 = t1 - t2;
            double ddays = t3.TotalDays;
            int iweeks = (int)ddays /7;
            string begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
            string enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
            int j = 0;
            int z = 0;
            DataSet ds = new DataSet();
            for (int i = 1; i <= iweeks;i++ )
            {
                if (i == 1)
                {
                    begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
                }
                else {
                    begindaystime = time.AddDays((i*-7)-1).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(((i-1)*-7)-1).ToString("yyy-MM-dd");
                }
                string sql = "select trunc(decode(s.ATTACH_REQUEST,0,1,s.ATTACH_SUC / s.ATTACH_REQUEST), 4） as FZL, trunc(decode(s.SERVICE_REQUEST,0,1,s.SERVICE_SUC / s.SERVICE_REQUEST), 4) as SERVIDEGOOD, trunc(decode(s.TAU_REQUEST,0,1,s.TAU_SUC / s.TAU_REQUEST), 4) as TAUGOOD,s.* from CORE_DAY s where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and MME_ID in (select MME_ID from CORE_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.MME_ID HAVING count(t.MME_ID) > 3 )";
                DataTable webdata = OraConnect.ReadData(sql);
                if (webdata != null && webdata.Rows.Count > 0)
                {
                    j = j + 1;
                    z = 0;
                    if (j==3)
                    {
                        if (i == 1)
                        {
                            ds.Tables.Add(webdata.Copy());
                        }
                        else
                        {
                            object[] obj = new object[ds.Tables[0].Columns.Count];
                            for (int g = 0; g < webdata.Rows.Count; i++)
                            {
                                webdata.Rows[i].ItemArray.CopyTo(obj, 0);
                                ds.Tables[0].Rows.Add(obj);
                            }
                        }
                        break;
                    }
                }
                else {
                    z = z + 1;
                    j = 0;
                    if (z==3)
                    {
                        break;
                    }
                }
               
            }

            return ds;
        }
        #endregion

        #region 无线
        //获取无线十五分钟数据
        public DataSet GetWifiMinData(DateTime time,string start,string end)
        {
            DateTime minbegintime = time.AddMinutes(-15);
            DateTime minendtime = time;
            string sql = null;
            if (start == null || start == "" || end == null || end =="") {
                sql = "select V_WORKPARAMETER.HOTSPOTCLASS HOTSPOTCLASS,V_WORKPARAMETER.SC_NAME ,V_WORKPARAMETER.CITY cityname,trunc(0.5 *(decode(KQI_MIN.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * KQI_MIN.PAGE_NUM / KQI_MIN.PAGE_SUM)) + 0.2 * (decode(KQI_MIN.FD_SUM, 0, 1, 1-1.0 * KQI_MIN.FDG_NUM / KQI_MIN.FD_SUM))) + 0.3 * (decode(KQI_MIN.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * KQI_MIN.VIDEO_GNUM / KQI_MIN.VIDEO_SNUM)) + 0.2 * (decode(KQI_MIN.VIDEO_SNUM, 0, 1, 1-1.0 * KQI_MIN.VIDEO_BNUM / KQI_MIN.VIDEO_SNUM))) + 0.1 * (decode(KQI_MIN.GAME_SUM, 0, 1, 1-1.0 * KQI_MIN.GAME_NUM / KQI_MIN.GAME_SUM)) + 0.1 * (decode(KQI_MIN.NEWS_SUM, 0, 1, 1-1.0 * KQI_MIN.NEWS_NUM / KQI_MIN.NEWS_SUM)), 4) WIFIGOOD,KQI_MIN.* from KQI_MIN left join CITYMAPPING on CITYMAPPING.CITYno=KQI_MIN.City join V_WORKPARAMETER on V_WORKPARAMETER.Eci=KQI_MIN.Ecgi where KQI_MIN.start_time > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and KQI_MIN.start_time <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') order by KQI_MIN.Ecgi,KQI_MIN.Start_Time";
            }
            else
            {
                DateTime n = DateTime.Now;
                start = n.Year + "-" + n.Month + "-" + n.Day + " " + start;
                end = n.Year + "-" + n.Month + "-" + n.Day + " " + end;
                DateTime s = DateTime.Parse(start);
                DateTime e = DateTime.Parse(end);
                //DateTime s = DateTime.Parse("2017-9-18 15:00:00");
                //DateTime e = DateTime.Parse("2017-9-18 15:30:00");
                sql = "select V_WORKPARAMETER.HOTSPOTCLASS HOTSPOTCLASS,V_WORKPARAMETER.SC_NAME ,V_WORKPARAMETER.CITY cityname,trunc(0.5 *(decode(KQI_MIN.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * KQI_MIN.PAGE_NUM / KQI_MIN.PAGE_SUM)) + 0.2 * (decode(KQI_MIN.FD_SUM, 0, 1, 1-1.0 * KQI_MIN.FDG_NUM / KQI_MIN.FD_SUM))) + 0.3 * (decode(KQI_MIN.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * KQI_MIN.VIDEO_GNUM / KQI_MIN.VIDEO_SNUM)) + 0.2 * (decode(KQI_MIN.VIDEO_SNUM, 0, 1, 1-1.0 * KQI_MIN.VIDEO_BNUM / KQI_MIN.VIDEO_SNUM))) + 0.1 * (decode(KQI_MIN.GAME_SUM, 0, 1, 1-1.0 * KQI_MIN.GAME_NUM / KQI_MIN.GAME_SUM)) + 0.1 * (decode(KQI_MIN.NEWS_SUM, 0, 1, 1-1.0 * KQI_MIN.NEWS_NUM / KQI_MIN.NEWS_SUM)), 4) WIFIGOOD,KQI_MIN.* from KQI_MIN left join CITYMAPPING on CITYMAPPING.CITYno=KQI_MIN.City join V_WORKPARAMETER on V_WORKPARAMETER.Eci=KQI_MIN.Ecgi where KQI_MIN.start_time > to_date('" + s + "', 'yyyy-mm-dd hh24:mi:ss') and KQI_MIN.start_time <= to_date('" + e + "', 'yyyy-mm-dd hh24:mi:ss') order by KQI_MIN.Ecgi,KQI_MIN.Start_Time";
            }
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "KQI_MIN";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }

        public DataSet GetWifiThisMinData(string start,string end)
        {
            DateTime n = DateTime.Now;
            start = n.Year +"-" + n.Month +"-"+ n.Day+" " + start;
            end = n.Year + "-" + n.Month + "-" + n.Day + " " + end;
            DateTime s = DateTime.Parse(start);
            DateTime e = DateTime.Parse(end);
            //DateTime s = DateTime.Parse("2017-9-18 15:00:00");
            //DateTime e = DateTime.Parse("2017-9-18 15:30:00");
            string sql = "select V_WORKPARAMETER.HOTSPOTCLASS HOTSPOTCLASS,V_WORKPARAMETER.SC_NAME ,V_WORKPARAMETER.CITY cityname,trunc(0.5 *(decode(KQI_MIN.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * KQI_MIN.PAGE_NUM / KQI_MIN.PAGE_SUM)) + 0.2 * (decode(KQI_MIN.FD_SUM, 0, 1, 1-1.0 * KQI_MIN.FDG_NUM / KQI_MIN.FD_SUM))) + 0.3 * (decode(KQI_MIN.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * KQI_MIN.VIDEO_GNUM / KQI_MIN.VIDEO_SNUM)) + 0.2 * (decode(KQI_MIN.VIDEO_SNUM, 0, 1, 1-1.0 * KQI_MIN.VIDEO_BNUM / KQI_MIN.VIDEO_SNUM))) + 0.1 * (decode(KQI_MIN.GAME_SUM, 0, 1, 1-1.0 * KQI_MIN.GAME_NUM / KQI_MIN.GAME_SUM)) + 0.1 * (decode(KQI_MIN.NEWS_SUM, 0, 1, 1-1.0 * KQI_MIN.NEWS_NUM / KQI_MIN.NEWS_SUM)), 4) WIFIGOOD,KQI_MIN.* from KQI_MIN left join CITYMAPPING on CITYMAPPING.CITYno=KQI_MIN.City join V_WORKPARAMETER on V_WORKPARAMETER.Eci=KQI_MIN.Ecgi where KQI_MIN.start_time > to_date('" + s + "', 'yyyy-mm-dd hh24:mi:ss') and KQI_MIN.start_time <= to_date('" + e + "', 'yyyy-mm-dd hh24:mi:ss') order by KQI_MIN.Ecgi,KQI_MIN.Start_Time";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "KQI_MIN";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }

        //获取无线当天多个时段数据
        public DataSet GetWifiMinsData(DateTime time)
        {
            string minbegintime = time.ToString("yyy-MM-dd");
            string minendtime = time.AddDays(1).ToString("yyy-MM-dd");
            //string sql = "select * from KQI_MIN where start_time > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and start_time <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') and ECGI in (select ECGI from KQI_MIN t where t.start_time > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ECGI HAVING count(t.ECGI) >= 3 and count(t.ECGI)<=5)";
            string sql = "select V_WORKPARAMETER.HOTSPOTCLASS HOTSPOTCLASS,V_WORKPARAMETER.SC_NAME ,V_WORKPARAMETER.CITY cityname,trunc(0.5 *(decode(KQI_MIN.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * KQI_MIN.PAGE_NUM / KQI_MIN.PAGE_SUM)) + 0.2 * (decode(KQI_MIN.FD_SUM, 0, 1,1- 1.0 * KQI_MIN.FDG_NUM / KQI_MIN.FD_SUM))) + 0.3 * (decode(KQI_MIN.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * KQI_MIN.VIDEO_GNUM / KQI_MIN.VIDEO_SNUM)) + 0.2 * (decode(KQI_MIN.VIDEO_SNUM, 0, 1, 1-1.0 * KQI_MIN.VIDEO_BNUM / KQI_MIN.VIDEO_SNUM))) + 0.1 * (decode(KQI_MIN.GAME_SUM, 0, 1, 1-1.0 * KQI_MIN.GAME_NUM / KQI_MIN.GAME_SUM)) + 0.1 * (decode(KQI_MIN.NEWS_SUM, 0, 1, 1-1.0 * KQI_MIN.NEWS_NUM / KQI_MIN.NEWS_SUM)), 4) WIFIGOOD,KQI_MIN.* from KQI_MIN left join CITYMAPPING on CITYMAPPING.CITYno=KQI_MIN.City join V_WORKPARAMETER on V_WORKPARAMETER.Eci=KQI_MIN.Ecgi where start_time > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and start_time <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') and ECGI in (select ECGI from KQI_MIN t where t.start_time > to_date('" + minbegintime + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + minendtime + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ECGI HAVING count(t.ECGI) > 5) order by KQI_MIN.Ecgi,KQI_MIN.Start_Time";//涉及两个地方 count(t.ECGI) >= 3 and
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "KQI_MIN";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取无线天数据
        public DataSet GetWifiDayData(DateTime time)
        {
            string daytime = time.AddDays(-1).ToString("yyy-MM-dd");
            //string sql = "select * from KQI_DAY t where trunc(t.start_time)=to_date('" + daytime + "', 'yyyy-mm-dd')";
            string sql = "select V_WORKPARAMETER.HOTSPOTCLASS HOTSPOTCLASS,V_WORKPARAMETER.SC_NAME,V_WORKPARAMETER.CITY cityname,trunc(0.5 *(decode(KQI_DAY.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * KQI_DAY.PAGE_NUM / KQI_DAY.PAGE_SUM)) + 0.2 * (decode(KQI_DAY.FD_SUM, 0, 1, 1-1.0 * KQI_DAY.FDG_NUM / KQI_DAY.FD_SUM))) + 0.3 * (decode(KQI_DAY.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * KQI_DAY.VIDEO_GNUM / KQI_DAY.VIDEO_SNUM)) + 0.2 * (decode(KQI_DAY.VIDEO_SNUM, 0, 1, 1-1.0 * KQI_DAY.VIDEO_BNUM / KQI_DAY.VIDEO_SNUM))) + 0.1 * (decode(KQI_DAY.GAME_SUM, 0, 1, 1-1.0 * KQI_DAY.GAME_NUM / KQI_DAY.GAME_SUM)) + 0.1 * (decode(KQI_DAY.NEWS_SUM, 0, 1, 1-1.0 * KQI_DAY.NEWS_NUM / KQI_DAY.NEWS_SUM)), 4) WIFIGOOD, KQI_DAY.* from KQI_DAY left join CITYMAPPING on CITYMAPPING.CITYno = KQI_DAY.City join V_WORKPARAMETER on V_WORKPARAMETER.Eci = KQI_DAY.Ecgi where trunc(KQI_DAY.start_time) = to_date('" + daytime + "', 'yyyy-mm-dd') order by KQI_DAY.Ecgi, KQI_DAY.Start_Time";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "KQI_DAY";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取无线周数据
        public DataSet GetWifiWeekData(DateTime time)
        {
            string begintimestr = time.AddDays(-8).ToString("yyy-MM-dd");
            string endtimestr = time.AddDays(-1).ToString("yyy-MM-dd");
            string begindaystime = begintimestr;
            string enddaystime = endtimestr;
            string sql = "select V_WORKPARAMETER.HOTSPOTCLASS HOTSPOTCLASS,V_WORKPARAMETER.SC_NAME,V_WORKPARAMETER.CITY cityname,trunc(0.5 *(decode(KQI_DAY.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * KQI_DAY.PAGE_NUM / KQI_DAY.PAGE_SUM)) + 0.2 * (decode(KQI_DAY.FD_SUM, 0, 1, 1-1.0 * KQI_DAY.FDG_NUM / KQI_DAY.FD_SUM))) + 0.3 * (decode(KQI_DAY.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * KQI_DAY.VIDEO_GNUM / KQI_DAY.VIDEO_SNUM)) + 0.2 * (decode(KQI_DAY.VIDEO_SNUM, 0, 1, 1-1.0 * KQI_DAY.VIDEO_BNUM / KQI_DAY.VIDEO_SNUM))) + 0.1 * (decode(KQI_DAY.GAME_SUM, 0, 1, 1-1.0 * KQI_DAY.GAME_NUM / KQI_DAY.GAME_SUM)) + 0.1 * (decode(KQI_DAY.NEWS_SUM, 0, 1, 1-1.0 * KQI_DAY.NEWS_NUM / KQI_DAY.NEWS_SUM)), 4) WIFIGOOD, KQI_DAY.* from KQI_DAY left join CITYMAPPING on CITYMAPPING.CITYno = KQI_DAY.City join V_WORKPARAMETER on V_WORKPARAMETER.Eci = KQI_DAY.Ecgi where  trunc(start_time) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(start_time) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ECGI in (select ECGI from KQI_DAY t where  trunc(t.start_time) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.start_time) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ECGI HAVING count(t.ECGI) >= 3 and max(trunc(t.start_time))=to_date('" + enddaystime + "', 'yyyy-mm-dd')) order by KQI_DAY.Ecgi, KQI_DAY.Start_Time";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "KQI_DAY";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取无线长期质差数据
        public DataSet GetWifiLongData(DateTime time)
        {

            DateTime t1 = DateTime.Now;
            DateTime t2 = DateTime.Parse("2017-08-30");
            System.TimeSpan t3 = t1 - t2;
            double ddays = t3.TotalDays;
            int iweeks = (int)ddays / 7;
            string begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
            string enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
            int j = 0;
            int z = 0;
            DataSet ds = new DataSet();
            for (int i = 1; i <= iweeks; i++)
            {
                if (i == 1)
                {
                    begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
                }
                else
                {
                    begindaystime = time.AddDays((i * -7) - 1).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(((i - 1) * -7) - 1).ToString("yyy-MM-dd");
                }
                string sql = "select V_WORKPARAMETER.HOTSPOTCLASS HOTSPOTCLASS,V_WORKPARAMETER.SC_NAME,V_WORKPARAMETER.CITY cityname,trunc(0.5 *(decode(KQI_DAY.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * KQI_DAY.PAGE_NUM / KQI_DAY.PAGE_SUM)) + 0.2 * (decode(KQI_DAY.FD_SUM, 0, 1,1- 1.0 * KQI_DAY.FDG_NUM / KQI_DAY.FD_SUM))) + 0.3 * (decode(KQI_DAY.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * KQI_DAY.VIDEO_GNUM / KQI_DAY.VIDEO_SNUM)) + 0.2 * (decode(KQI_DAY.VIDEO_SNUM, 0, 1, 1-1.0 * KQI_DAY.VIDEO_BNUM / KQI_DAY.VIDEO_SNUM))) + 0.1 * (decode(KQI_DAY.GAME_SUM, 0, 1, 1-1.0 * KQI_DAY.GAME_NUM / KQI_DAY.GAME_SUM)) + 0.1 * (decode(KQI_DAY.NEWS_SUM, 0, 1, 1-1.0 * KQI_DAY.NEWS_NUM / KQI_DAY.NEWS_SUM)), 4) WIFIGOOD, KQI_DAY.* from KQI_DAY left join CITYMAPPING on CITYMAPPING.CITYno = KQI_DAY.City join V_WORKPARAMETER on V_WORKPARAMETER.Eci = KQI_DAY.Ecgi where  trunc(start_time) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(start_time) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ECGI in (select ECGI from KQI_DAY t where  trunc(t.start_time) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.start_time) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ECGI HAVING count(t.ECGI) > 3 ) order by KQI_MIN.Ecgi, KQI_MIN.Start_Time";
                DataTable webdata = OraConnect.ReadData(sql);
                if (webdata != null && webdata.Rows.Count > 0)
                {
                    j = j + 1;
                    z = 0;
                    if (j == 3)
                    {
                        if (i == 1)
                        {
                            ds.Tables.Add(webdata.Copy());
                        }
                        else
                        {
                            object[] obj = new object[ds.Tables[0].Columns.Count];
                            for (int g = 0; g < webdata.Rows.Count; i++)
                            {
                                webdata.Rows[i].ItemArray.CopyTo(obj, 0);
                                ds.Tables[0].Rows.Add(obj);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    z = z + 1;
                    j = 0;
                    if (z == 3)
                    {
                        break;
                    }
                }

            }

            return ds;
        }

        #endregion

        #region 终端
        //获取终端天数据
        public DataSet GetTerminalDayData(DateTime time)
        {
            string daytime = time.AddDays(-1).ToString("yyy-MM-dd");
            //string sql = "select * from TERMINAL_DAY t where trunc(t.START_TIME)=to_date('" + daytime + "', 'yyyy-mm-dd')";
            string sql = "select trunc(decode(s.PAGE_OPEN_TIMES,0,0.8,0.8 * (1-s.PAGE_OPEN_GOOD_TIMES / s.PAGE_OPEN_TIMES)) + decode(s.FST_SCREEN_TIMES,0,0.2,0.2 * (1-s.FST_SCREEN_GOOD_TIMES / s.FST_SCREEN_TIMES)), 4) WEBGOODL ,trunc(decode(s.STREAM_REQUEST_TIMES,0,1,(1-s.STREAM_RATE_GOOD_TIMES / s.STREAM_REQUEST_TIMES) * 0.8+(1-s.STREAM_STALL_GOOD_TIMES / s.STREAM_REQUEST_TIMES) * 0.2), 4) VIDEODOOGL,trunc(decode(s.IM_SENT_REQUEST_TIMES,0,1,1-s.IM_SENT_SUC_TIMES / s.IM_SENT_REQUEST_TIMES), 4) JSTXGOODL ,trunc(decode(s.GAME_REQUEST_TIMES,0,1,1-s.GAME_DELAY_GOOD_TIMES / s.GAME_REQUEST_TIMES), 4) PLAYGOODL , s.* from TERMINAL_DAY s where trunc(s.START_TIME) = to_date('" + daytime + "', 'yyyy-mm-dd') order by s.BRAND,s.MODEL,s.start_time";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "TERMINAL_DAY";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取终端周数据
        public DataSet GetTerminalWeekData(DateTime time)
        {
            string begintimestr = time.AddDays(-8).ToString("yyy-MM-dd");
            string endtimestr = time.AddDays(-1).ToString("yyy-MM-dd");
            string begindaystime = begintimestr;
            string enddaystime = endtimestr;
            string sql = "select trunc(decode(s.PAGE_OPEN_TIMES,0,0.8,0.8 * (1-s.PAGE_OPEN_GOOD_TIMES / s.PAGE_OPEN_TIMES)) + decode(s.FST_SCREEN_TIMES,0,0.2,0.2 * (1-s.FST_SCREEN_GOOD_TIMES / s.FST_SCREEN_TIMES)), 4) WEBGOODL , trunc(decode(s.STREAM_REQUEST_TIMES,0,1,(1-s.STREAM_RATE_GOOD_TIMES / s.STREAM_REQUEST_TIMES) * 0.8+(1-s.STREAM_STALL_GOOD_TIMES / s.STREAM_REQUEST_TIMES) * 0.2), 4) VIDEODOOGL ,trunc(decode(s.IM_SENT_REQUEST_TIMES,0,1,1-s.IM_SENT_SUC_TIMES / s.IM_SENT_REQUEST_TIMES), 4) JSTXGOODL ,trunc(decode(s.GAME_REQUEST_TIMES,0,1,1-s.GAME_DELAY_GOOD_TIMES / s.GAME_REQUEST_TIMES), 4) PLAYGOODL , s.* from TERMINAL_DAY s where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and MODEL in (select MODEL from TERMINAL_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.MODEL HAVING count(t.MODEL) >= 3 and max(trunc(t.START_TIME))=to_date('" + enddaystime + "', 'yyyy-mm-dd') ) order by BRAND,MODEL,start_time";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "TERMINAL_DAY";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取终端长期质差数据
        public DataSet GetTerminalLongData(DateTime time)
        {

            DateTime t1 = DateTime.Now;
            DateTime t2 = DateTime.Parse("2017-08-30");
            System.TimeSpan t3 = t1 - t2;
            double ddays = t3.TotalDays;
            int iweeks = (int)ddays / 7;
            string begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
            string enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
            int j = 0;
            int z = 0;
            DataSet ds = new DataSet();
            for (int i = 1; i <= iweeks; i++)
            {
                if (i == 1)
                {
                    begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
                }
                else
                {
                    begindaystime = time.AddDays((i * -7) - 1).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(((i - 1) * -7) - 1).ToString("yyy-MM-dd");
                }
                string sql = "select trunc(decode(s.PAGE_OPEN_TIMES,0,0.8,0.8 * (1-s.PAGE_OPEN_GOOD_TIMES / s.PAGE_OPEN_TIMES)) + decode(s.FST_SCREEN_TIMES,0,0.2,0.2 * (1-s.FST_SCREEN_GOOD_TIMES / s.FST_SCREEN_TIMES)), 4) WEBGOODL , trunc(decode(s.STREAM_REQUEST_TIMES,0,1,(1-s.STREAM_RATE_GOOD_TIMES / s.STREAM_REQUEST_TIMES) * 0.8+(1-s.STREAM_STALL_GOOD_TIMES / s.STREAM_REQUEST_TIMES) * 0.2), 4) VIDEODOOGL ,trunc(decode(s.IM_SENT_REQUEST_TIMES,0,1,1-s.IM_SENT_SUC_TIMES / s.IM_SENT_REQUEST_TIMES), 4) JSTXGOODL ,trunc(decode(s.GAME_REQUEST_TIMES,0,1,1-s.GAME_DELAY_GOOD_TIMES / s.GAME_REQUEST_TIMES), 4) PLAYGOODL , s.* from TERMINAL_DAY s where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and MODEL in (select MODEL from TERMINAL_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.MODEL HAVING count(t.MODEL) > 3 ) order by start_time ";
                DataTable webdata = OraConnect.ReadData(sql);
                if (webdata != null && webdata.Rows.Count > 0)
                {
                    j = j + 1;
                    z = 0;
                    if (j == 3)
                    {
                        if (i == 1)
                        {
                            ds.Tables.Add(webdata.Copy());
                        }
                        else {
                            object[] obj = new object[ds.Tables[0].Columns.Count];
                            for (int g = 0; g < webdata.Rows.Count; i++)
                            {
                                webdata.Rows[i].ItemArray.CopyTo(obj, 0);
                                ds.Tables[0].Rows.Add(obj);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    z = z + 1;
                    j = 0;
                    if (z == 3)
                    {
                        break;
                    }
                }

            }

            return ds;
        }

        #endregion

        #region 承载网
        //获取承载网天数据
        public DataSet GetIPRanDayData(DateTime time)
        {
            string daytime = time.AddDays(-1).ToString("yyy-MM-dd");
            //string sql = "select * from TERMINAL_DAY t where trunc(t.START_TIME)=to_date('" + daytime + "', 'yyyy-mm-dd')";
            string sql = @"SELECT t.*,b.CITY CITY2,
                          b.*,TRUNC(0.5 *(DECODE(c.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * c.PAGE_NUM / c.PAGE_SUM)) + 0.2 * (DECODE(c.FD_SUM, 0, 0.2, 1-1.0 * c.FDG_NUM / c.FD_SUM))) + 0.3 * (DECODE(c.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * c.VIDEO_GNUM / c.VIDEO_SNUM)) + 0.2 * (DECODE(c.VIDEO_SNUM, 0, 1, 1-1.0 * c.VIDEO_BNUM / c.VIDEO_SNUM))) + 0.1 * (DECODE(c.GAME_SUM, 0, 1, 1-1.0 * c.GAME_NUM / c.GAME_SUM)) + 0.1 * (DECODE(c.NEWS_SUM, 0, 1, 1-1.0 * c.NEWS_NUM / c.NEWS_SUM)), 4) WIFIGOOD2
                        FROM IPRAN_DAY t
                        JOIN V_WORKPARAMETER b
                        ON t.IP=
                          CASE
                            WHEN SUBSTR(t.KIND,0,1)='A'
                            THEN b.IPRAN_A
                            WHEN SUBSTR(t.KIND,0,1)='B'
                            THEN b.IPRAN_B
                            ELSE 'none'
                          END
                          left join KQI_DAY c on b.eci = c.ecgi and TRUNC(c.start_time) = to_date('" + daytime + @"','yyyy-mm-dd')
                        WHERE TRUNC(t.CREATEDATE)=to_date('" + daytime + @"','yyyy-mm-dd')
                        ORDER BY t.IP,
                          t.CREATEDATE";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "IPRAN_DAY";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取承载网周数据
        public DataSet GetIPRanWeekData(DateTime time)
        {
            string begintimestr = time.AddDays(-8).ToString("yyy-MM-dd");
            string endtimestr = time.AddDays(-1).ToString("yyy-MM-dd");
            string begindaystime = begintimestr;
            string enddaystime = endtimestr;
            string sql = @"select t.*,b.CITY CITY2, b.*,TRUNC(0.5 *(DECODE(c.PAGE_SUM, 0, 0.8, 0.8 * (1 - 1.0 * c.PAGE_NUM / c.PAGE_SUM)) + 0.2 * (DECODE(c.FD_SUM, 0, 1,1- 1.0 * c.FDG_NUM / c.FD_SUM))) + 0.3 * (DECODE(c.VIDEO_SNUM, 0, 0.8, 0.8 * (1-1.0 * c.VIDEO_GNUM / c.VIDEO_SNUM)) + 0.2 * (DECODE(c.VIDEO_SNUM, 0, 1, 1-1.0 * c.VIDEO_BNUM / c.VIDEO_SNUM))) + 0.1 * (DECODE(c.GAME_SUM, 0, 1, 1-1.0 * c.GAME_NUM / c.GAME_SUM)) + 0.1 * (DECODE(c.NEWS_SUM, 0, 1, 1-1.0 * c.NEWS_NUM / c.NEWS_SUM)), 4) WIFIGOOD2
                          from IPRAN_DAY t
                          join V_WORKPARAMETER b
                            on t.IP = case
                                 when substr(t.KIND, 0, 1) = 'A' then
                                  b.IPRAN_A
                                 when substr(t.KIND, 0, 1) = 'B' then
                                  b.IPRAN_B
                                 else
                                  'none'
                               end
                               left join KQI_DAY c on b.eci = c.ecgi and TRUNC(c.start_time) = to_date('" + enddaystime + @"','yyyy-mm-dd')
                         where trunc(t.CREATEDATE) >= to_date('" + begindaystime+ @"', 'yyyy-mm-dd')
                           and trunc(t.CREATEDATE) <= to_date('" + enddaystime + @"', 'yyyy-mm-dd')
                           and t.IP in
                               (select IP
                                  from IPRAN_DAY a
                                 where trunc(CREATEDATE) >= to_date('" + begindaystime + @"', 'yyyy-mm-dd')
                                   and trunc(CREATEDATE) <= to_date('" + enddaystime + @"', 'yyyy-mm-dd')
                                 group by a.IP
                                having count(0) >= 3 and max(CREATEDATE) = to_date('" + enddaystime + @"', 'yyyy-mm-dd'))
                         order by t.IP, t.CREATEDATE";
            DataSet ds = new DataSet();
            DataTable webdata = OraConnect.ReadData(sql);
            webdata.TableName = "IPRAN_DAY";
            ds.Tables.Add(webdata.Copy());
            return ds;
        }
        //获取承载网长期质差数据
        public DataSet GetIPRanLongData(DateTime time)
        {

            DateTime t1 = DateTime.Now;
            DateTime t2 = DateTime.Parse("2017-08-30");
            System.TimeSpan t3 = t1 - t2;
            double ddays = t3.TotalDays;
            int iweeks = (int)ddays / 7;
            string begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
            string enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
            int j = 0;
            int z = 0;
            DataSet ds = new DataSet();
            for (int i = 1; i <= iweeks; i++)
            {
                if (i == 1)
                {
                    begindaystime = time.AddDays(-8).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(-1).ToString("yyy-MM-dd");
                }
                else
                {
                    begindaystime = time.AddDays((i * -7) - 1).ToString("yyy-MM-dd");
                    enddaystime = time.AddDays(((i - 1) * -7) - 1).ToString("yyy-MM-dd");
                }
                string sql = "select trunc(decode(s.PAGE_OPEN_TIMES,0,0.8,0.8 * (s.PAGE_OPEN_GOOD_TIMES / s.PAGE_OPEN_TIMES)) + decode(s.FST_SCREEN_TIMES,0,0.2,0.2 * (1-s.FST_SCREEN_GOOD_TIMES / s.FST_SCREEN_TIMES)), 4) WEBGOODL , trunc(decode(s.STREAM_REQUEST_TIMES,0,0.8,(1-s.STREAM_RATE_GOOD_TIMES / s.STREAM_REQUEST_TIMES) * 0.8) + decode(s.STREAM_REQUEST_TIMES,0,0.2,(1-s.STREAM_STALL_GOOD_TIMES / s.STREAM_REQUEST_TIMES) * 0.2), 4) VIDEODOOGL ,trunc(decode(s.IM_SENT_REQUEST_TIMES,0,1,s.IM_SENT_SUC_TIMES / s.IM_SENT_REQUEST_TIMES), 4) JSTXGOODL ,trunc(decode(s.GAME_REQUEST_TIMES,0,1,s.GAME_DELAY_GOOD_TIMES / s.GAME_REQUEST_TIMES), 4) PLAYGOODL , s.* from TERMINAL_DAY s where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and MODEL in (select MODEL from TERMINAL_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.MODEL HAVING count(t.MODEL) > 3 ) order by start_time ";
                DataTable webdata = OraConnect.ReadData(sql);
                if (webdata != null && webdata.Rows.Count > 0)
                {
                    j = j + 1;
                    z = 0;
                    if (j == 3)
                    {
                        if (i == 1)
                        {
                            ds.Tables.Add(webdata.Copy());
                        }
                        else
                        {
                            object[] obj = new object[ds.Tables[0].Columns.Count];
                            for (int g = 0; g < webdata.Rows.Count; i++)
                            {
                                webdata.Rows[i].ItemArray.CopyTo(obj, 0);
                                ds.Tables[0].Rows.Add(obj);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    z = z + 1;
                    j = 0;
                    if (z == 3)
                    {
                        break;
                    }
                }

            }

            return ds;
        }
        #endregion

    }
}