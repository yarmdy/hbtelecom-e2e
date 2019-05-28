using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DWWinService
{
    class WebData
    {
        public static int[] AfterImportDBMin(String time)
        {
            int[] inp = new int[2];
            //time = time.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":");
            string timeq = DateTime.Parse(time).AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
            string timeds = DateTime.Parse(time).ToString("yyyy-MM-dd");
            string timede = DateTime.Parse(time).AddDays(1).ToString("yyyy-MM-dd");
            string sqlmin1 = "select count(*) count from  (select * from WEB_MIN t where t.createtime > to_date('" + timeq + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss'))";
            string sqlmin2 = "select count(*) count from  (select * from VIDEO_MIN t where t.START_TIME > to_date('" + timeq + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss'))";
            string sqlmin3 = "select count(*) count from  (select * from SIGNAL_MIN t where t.createtime > to_date('" + timeq + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss'))";
            string sqlmin4 = "select count(*) count from  (select * from PLAY_MIN t where t.createtime > to_date('" + timeq + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss'))";
            string sqlday1 = "select count(*) count from (select ip_address from web_min t where t.createtime > to_date('" + timeds + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + timede + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ip_address HAVING count(t.ip_address) >= 3 and count(t.ip_address)<=5)";
            string sqlday2 = "select count(*) count from (select SERER_IP from VIDEO_MIN t where t.START_TIME > to_date('" + timeds + "', 'yyyy-mm-dd hh24:mi:ss') and t.START_TIME <= to_date('" + timede + "', 'yyyy-mm-dd hh24:mi:ss') group by t.SERER_IP HAVING count(t.SERER_IP) >= 3 and count(t.SERER_IP)<=5)";
            string sqlday3 = "select count(*) count from (select ip_address from SIGNAL_MIN t where t.createtime > to_date('" + timeds + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + timede + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ip_address HAVING count(t.ip_address) >= 3 and count(t.ip_address)<=5)";
            string sqlday4 = "select count(*) count from (select ip_address from PLAY_MIN t where t.createtime > to_date('" + timeds + "', 'yyyy-mm-dd hh24:mi:ss') and t.createtime <= to_date('" + timede + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ip_address HAVING count(t.ip_address) >= 3 and count(t.ip_address)<=5)";
            DataSet ds1 = DB.Query(sqlmin1);
            int num1 = O2.O2I(ds1.Tables[0].Rows[0][0]);
            DataSet ds2 = DB.Query(sqlmin2);
            int num2 = O2.O2I(ds2.Tables[0].Rows[0][0]);
            DataSet ds3 = DB.Query(sqlmin3);
            int num3 = O2.O2I(ds3.Tables[0].Rows[0][0]);
            DataSet ds4 = DB.Query(sqlmin4);
            int num4 = O2.O2I(ds4.Tables[0].Rows[0][0]);
            DataSet ds5 = DB.Query(sqlday1);
            int num5 = O2.O2I(ds5.Tables[0].Rows[0][0]);
            DataSet ds6 = DB.Query(sqlday2);
            int num6 = O2.O2I(ds6.Tables[0].Rows[0][0]);
            DataSet ds7 = DB.Query(sqlday3);
            int num7 = O2.O2I(ds7.Tables[0].Rows[0][0]);
            DataSet ds8 = DB.Query(sqlday4);
            int num8 = O2.O2I(ds8.Tables[0].Rows[0][0]);
            inp[0] = num1 + num2 + num3 + num4;
            inp[1] = num5 + num6 + num7 + num8;
            return inp;
        }
        public static int[] AfterImportDBDay(String time)
        {
            int[] inp = new int[2];
           // time = time.Insert(4, "-").Insert(7, "-");
            string times = DateTime.Parse(time).AddDays(-6).ToString("yyyy-MM-dd");
            string sqlmin1 = "select count(*) count from  (select * from web_day t where trunc(t.createtime)=to_date('" + time + "', 'yyyy-mm-dd'))";
            string sqlmin2 = "select count(*) count from  (select * from VIDEO_DAY t where trunc(t.START_TIME)=to_date('" + time + "', 'yyyy-mm-dd'))";
            string sqlmin3 = "select count(*) count from  (select * from SIGNAL_DAY t where trunc(t.createtime)=to_date('" + time + "', 'yyyy-mm-dd'))";
            string sqlmin4 = "select count(*) count from  (select * from PLAY_DAY t where trunc(t.createtime)=to_date('" + time + "', 'yyyy-mm-dd'))";
            string sqlday1 = "select count(*) count from (select ip_address from web_day t where trunc(t.createtime) >= to_date('" + times + "', 'yyyy-mm-dd') and trunc(t.createtime) <= to_date('" + time + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) >= 3 and max(trunc(t.createtime))=to_date('" + time + "', 'yyyy-mm-dd') )";
            string sqlday2 = "select count(*) count from (select SERER_IP from VIDEO_DAY t where trunc(t.START_TIME) >= to_date('" + times + "', 'yyyy-mm-dd') and trunc(t.START_TIME) <= to_date('" + time + "', 'yyyy-mm-dd') group by t.SERER_IP HAVING count(t.SERER_IP) >= 3 and max(trunc(t.START_TIME))=to_date('" + time + "', 'yyyy-mm-dd') )";
            string sqlday3 = "select count(*) count from (select ip_address from SIGNAL_DAY t where trunc(t.createtime) >= to_date('" + times + "', 'yyyy-mm-dd') and trunc(t.createtime) <= to_date('" + time + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) >= 3 and max(trunc(t.createtime))=to_date('" + time + "', 'yyyy-mm-dd') )";
            string sqlday4 = "select count(*) count from (select ip_address from PLAY_DAY t where trunc(t.createtime) >= to_date('" + times + "', 'yyyy-mm-dd') and trunc(t.createtime) <= to_date('" + time + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) >= 3 and max(trunc(t.createtime))=to_date('" + time + "', 'yyyy-mm-dd') )";
            DataSet ds1 = DB.Query(sqlmin1);
            int num1 = O2.O2I(ds1.Tables[0].Rows[0][0]);
            DataSet ds2 = DB.Query(sqlmin2);
            int num2 = O2.O2I(ds2.Tables[0].Rows[0][0]);
            DataSet ds3 = DB.Query(sqlmin3);
            int num3 = O2.O2I(ds3.Tables[0].Rows[0][0]);
            DataSet ds4 = DB.Query(sqlmin4);
            int num4 = O2.O2I(ds4.Tables[0].Rows[0][0]);
            DataSet ds5 = DB.Query(sqlday1);
            int num5 = O2.O2I(ds5.Tables[0].Rows[0][0]);
            DataSet ds6 = DB.Query(sqlday2);
            int num6 = O2.O2I(ds6.Tables[0].Rows[0][0]);
            DataSet ds7 = DB.Query(sqlday3);
            int num7 = O2.O2I(ds7.Tables[0].Rows[0][0]);
            DataSet ds8 = DB.Query(sqlday4);
            int num8 = O2.O2I(ds8.Tables[0].Rows[0][0]);
            inp[0] = num1 + num2 + num3 + num4;
            inp[1] = num5 + num6 + num7 + num8;
            return inp;
        }
        public static int AfterImportDBLong(DateTime time)
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
                string sqlvideo = "select * from VIDEO_DAY where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and SERER_IP in (select SERER_IP from VIDEO_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.SERER_IP HAVING count(t.SERER_IP) > 3 )";
               // videodata = OraConnect.ReadData(sqlvideo);
                videodata = DB.QueryAsDt(sqlvideo);
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
                string sqlweb = "select * from web_day where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from web_day t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) > 3 )";
                //webdata = OraConnect.ReadData(sqlweb);
                webdata = DB.QueryAsDt(sqlweb);
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
                string sqlsignal = "select * from SIGNAL_DAY where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from SIGNAL_DAY t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) > 3 )";
                //signaldata = OraConnect.ReadData(sqlsignal);
                signaldata = DB.QueryAsDt(sqlsignal);


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
                string sqlplay = "select * from PLAY_DAY where  trunc(createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and ip_address in (select ip_address from PLAY_DAY t where  trunc(t.createtime) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.createtime) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ip_address HAVING count(t.ip_address) > 3 )";
                //playdata = OraConnect.ReadData(sqlplay);
                playdata = DB.QueryAsDt(sqlplay);
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
            int s = 0;
            if (ds != null && ds.Tables.Count > 0)
            {
                s = ds.Tables[0].Rows.Count+ds.Tables[1].Rows.Count+ ds.Tables[2].Rows.Count+ ds.Tables[3].Rows.Count;
            }
            return s;
        }
    }
}
