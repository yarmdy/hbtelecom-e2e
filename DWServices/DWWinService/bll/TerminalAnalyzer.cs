using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    class TerminalAnalyzer : Analyzer
    {
        public override string GetKeyDay(Dictionary<string, object> dic)
        {
            return dic["MODEL"].ToString() + "_" + Str2DTStr(dic["START_TIME"].ToString()).Substring(0, 10);
        }

        public override void CombineRowDay(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyDay(dic)];
            dic_exist["FST_SCREEN_GOOD_TIMES"] = O2.O2I(dic_exist["FST_SCREEN_GOOD_TIMES"]) + O2.O2I(dic["FST_SCREEN_GOOD_TIMES"]);
            dic_exist["FST_SCREEN_TIMES"] = O2.O2I(dic_exist["FST_SCREEN_TIMES"]) + O2.O2I(dic["FST_SCREEN_TIMES"]);
            dic_exist["GAME_DELAY_GOOD_TIMES"] = O2.O2I(dic_exist["GAME_DELAY_GOOD_TIMES"]) + O2.O2I(dic["GAME_DELAY_GOOD_TIMES"]);
            dic_exist["GAME_REQUEST_TIMES"] = O2.O2I(dic_exist["GAME_REQUEST_TIMES"]) + O2.O2I(dic["GAME_REQUEST_TIMES"]);
            dic_exist["IM_SENT_REQUEST_TIMES"] = O2.O2I(dic_exist["IM_SENT_REQUEST_TIMES"]) + O2.O2I(dic["IM_SENT_REQUEST_TIMES"]);
            dic_exist["IM_SENT_SUC_TIMES"] = O2.O2I(dic_exist["IM_SENT_SUC_TIMES"]) + O2.O2I(dic["IM_SENT_SUC_TIMES"]);
            dic_exist["PAGE_OPEN_GOOD_TIMES"] = O2.O2I(dic_exist["PAGE_OPEN_GOOD_TIMES"]) + O2.O2I(dic["PAGE_OPEN_GOOD_TIMES"]);
            dic_exist["PAGE_OPEN_TIMES"] = O2.O2I(dic_exist["PAGE_OPEN_TIMES"]) + O2.O2I(dic["PAGE_OPEN_TIMES"]);
            dic_exist["STREAM_RATE_GOOD_TIMES"] = O2.O2I(dic_exist["STREAM_RATE_GOOD_TIMES"]) + O2.O2I(dic["STREAM_RATE_GOOD_TIMES"]);
            dic_exist["STREAM_STALL_GOOD_TIMES"] = O2.O2I(dic_exist["STREAM_STALL_GOOD_TIMES"]) + O2.O2I(dic["STREAM_STALL_GOOD_TIMES"]);
            dic_exist["STREAM_REQUEST_TIMES"] = O2.O2I(dic_exist["STREAM_REQUEST_TIMES"]) + O2.O2I(dic["STREAM_REQUEST_TIMES"]);
        }

        public override string Str2DTStr(string str)
        {
            return DateTime.Parse("1970/1/1 8:30:00").AddSeconds(O2.O2I(str)).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override bool IsZhiChaDay(Dictionary<string, object> dic)
        {
            //bool isBad = false;
            //int[] nums = { O2.O2I(dic["FST_SCREEN_TIMES"]), O2.O2I(dic["GAME_REQUEST_TIMES"]), O2.O2I(dic["IM_SENT_REQUEST_TIMES"]), O2.O2I(dic["PAGE_OPEN_TIMES"]), O2.O2I(dic["STREAM_REQUEST_TIMES"]), O2.O2I(dic["STREAM_REQUEST_TIMES"]) };
            //int[] vals = { O2.O2I(dic["FST_SCREEN_GOOD_TIMES"]), O2.O2I(dic["GAME_DELAY_GOOD_TIMES"]), O2.O2I(dic["IM_SENT_SUC_TIMES"]), O2.O2I(dic["PAGE_OPEN_GOOD_TIMES"]), O2.O2I(dic["STREAM_RATE_GOOD_TIMES"]), O2.O2I(dic["STREAM_STALL_GOOD_TIMES"]) };
            //for (int i = 0; i < nums.Length; i++)
            //{
            //    if (nums[i] < 6000)
            //        continue;
            //    if (1.0 * vals[i] / nums[i] < 0.7)
            //    {
            //        isBad = true;
            //        break;
            //    }
            //}
            double webRate = 1;
            double page_open = O2.O2I(dic["PAGE_OPEN_TIMES"]);
            double fst_times = O2.O2I(dic["FST_SCREEN_TIMES"]);
            webRate = 0.8 * (1 - O2.O2I(dic["PAGE_OPEN_GOOD_TIMES"]).Div0(page_open)) + 0.2 * (1 - O2.O2I(dic["FST_SCREEN_GOOD_TIMES"]).Div0(fst_times));
            double videoRate = 1;
            double sr_times = O2.O2I(dic["STREAM_REQUEST_TIMES"]);
            videoRate = (1 - O2.O2I(dic["STREAM_RATE_GOOD_TIMES"]).Div0(sr_times)) * 0.8 + (1 - O2.O2I(dic["STREAM_STALL_GOOD_TIMES"]).Div0(sr_times)) * 0.2;
            double imRate = 1;
            double isr_times = O2.O2I(dic["IM_SENT_REQUEST_TIMES"]);
            imRate = 1 - O2.O2I(dic["IM_SENT_SUC_TIMES"]).Div0(isr_times);
            double gameRate = 1;
            double gr_times = O2.O2I(dic["GAME_REQUEST_TIMES"]);
            gameRate = 1 - O2.O2I(dic["GAME_DELAY_GOOD_TIMES"]).Div0(gr_times);
            if ((page_open > 8000 || fst_times > 8000) && webRate < 0.7 ||
                sr_times > 1000 && videoRate < 0.7 ||
                isr_times > 1000 && imRate < 0.7 ||
                gr_times > 1000 && gameRate < 0.7)
            {
                return true;
            }
            return false;
        }

        public override void AfterImportDBDay(String time)
        {
            time = time.Insert(4, "-").Insert(7, "-");
            string times = DateTime.Parse(time).AddDays(-6).ToString("yyyy-MM-dd");
            string sql = "select count(*) count from  (select * from TERMINAL_DAY t where trunc(t.start_time)=to_date('" + time + "', 'yyyy-mm-dd'))";
            string sql2 = "select count(*) count from (select MODEL from TERMINAL_DAY t where trunc(t.start_time) >= to_date('" + times + "', 'yyyy-mm-dd') and trunc(t.start_time) <= to_date('" + time + "', 'yyyy-mm-dd') group by t.MODEL HAVING count(t.MODEL) >= 3 and max(trunc(t.start_time))=to_date('" + time + "', 'yyyy-mm-dd'))";
            DataSet ds = DB.Query(sql);
            int num = O2.O2I(ds.Tables[0].Rows[0][0]);
            string sql1 = "update DATA_DISPLAY set DATA_COUNT = " + num + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'TERMINAL' and DATA_STATUS = 'DAY'";
            DB.Exec(sql1);
            DataSet dss = DB.Query(sql2);
            int numm = O2.O2I(dss.Tables[0].Rows[0][0]);
            string sql3 = "update DATA_DISPLAY set DATA_COUNT = " + numm + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'TERMINAL' and DATA_STATUS = 'WEEK'";
            DB.Exec(sql3);
            int s = GetTerminalLongData(DateTime.Now);
            string sql4 = "update DATA_DISPLAY set DATA_COUNT = " + s + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'TERMINAL' and DATA_STATUS = 'LONG'";
            DB.Exec(sql4);
        }
        public int GetTerminalLongData(DateTime time)
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
                string sql = "select * from TERMINAL_DAY where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and MODEL in (select MODEL from TERMINAL_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.MODEL HAVING count(t.MODEL) > 3 )";
                //DataTable webdata = OraConnect.ReadData(sql);
                DataTable webdata = DB.QueryAsDt(sql);
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
            int s = 0;
            if (ds != null && ds.Tables.Count > 0)
            {
                s = ds.Tables[0].Rows.Count;
            }
            return s;
        }
    }
}
