using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    class VideoAnalyzer : Analyzer
    {
        public override string GetKeyQuarter(Dictionary<string, object> dic)
        {
            return dic["SERER_IP"].ToString() + "_" + Str2DTStr(dic["START_TIME"].ToString());
        }

        public override string GetKeyDay(Dictionary<string, object> dic)
        {
            return dic["SERER_IP"].ToString() + "_" + Str2DTStr(dic["START_TIME"].ToString()).Substring(0, 10);
        }

        public override string Str2DTStr(string str)
        {
            return DateTime.Parse("1970/1/1 8:30:00").AddSeconds(O2.O2I(str)).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override void CombineRowQuarter(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyQuarter(dic)];
            if(O2.O2I(dic_exist["STREAM_REQUEST"])<O2.O2I(dic["STREAM_REQUEST"]))
            {
                dic_exist = dic;
            }
        }

        public override void CombineRowDay(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyDay(dic)];
            dic_exist["STREAM_REQUEST"] = O2.O2I(dic_exist["STREAM_REQUEST"]) + O2.O2I(dic["STREAM_REQUEST"]);
            dic_exist["STREAM_DL_GOOD_TIMES"] = O2.O2I(dic_exist["STREAM_DL_GOOD_TIMES"]) + O2.O2I(dic["STREAM_DL_GOOD_TIMES"]);
            dic_exist["STREAM_HALT_GOOD_TIMES"] = O2.O2I(dic_exist["STREAM_HALT_GOOD_TIMES"]) + O2.O2I(dic["STREAM_HALT_GOOD_TIMES"]);
        }

        public override bool IsZhiCha(Dictionary<string, object> dic)
        {
            double videoRate = 1;
            int request = O2.O2I(dic["STREAM_REQUEST"]);
            videoRate = ((1-O2.O2I(dic["STREAM_DL_GOOD_TIMES"]).Div0(request)) * 0.8) + ((1-O2.O2I(dic["STREAM_HALT_GOOD_TIMES"]).Div0(request))*0.2);
            if (request >= 50 && videoRate < 0.8)
                return true;
            return false;
        }

        public override bool IsZhiChaDay(Dictionary<string, object> dic)
        {
            double videoRate = 1;
            videoRate = ((1-O2.O2I(dic["STREAM_DL_GOOD_TIMES"]).Div0(O2.O2I(dic["STREAM_REQUEST"]))) * 0.8) + ((1-O2.O2I(dic["STREAM_HALT_GOOD_TIMES"]).Div0(O2.O2I(dic["STREAM_REQUEST"]))) * 0.2);
            if (O2.O2I(dic["STREAM_REQUEST"]) >= 1000 && videoRate < 0.8)
                return true;
            return false;
        }

        public override void AfterImportDB(String time)
        {
            time = time.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":");
            //string timeq = DateTime.Parse(time).AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
            //string timeds = DateTime.Parse(time).ToString("yyyy-MM-dd");
            //string timede = DateTime.Parse(time).AddDays(1).ToString("yyyy-MM-dd");
            //string sql = "select count(*) count from  (select * from VIDEO_MIN t where t.start_time > to_date('" + timeq + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss'))";
            //string sql1 = "select count(*) count from (select SERER_IP from VIDEO_MIN t where t.start_time >= to_date('" + timeds + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time < to_date('" + timede + "', 'yyyy-mm-dd hh24:mi:ss') group by t.SERER_IP HAVING count(t.SERER_IP) >= 3 and count(t.SERER_IP) <= 5)";
            //DataSet ds = DB.Query(sql);
            //int num = O2.O2I(ds.Tables[0].Rows[0][0]);
            int[] inp = WebData.AfterImportDBMin(time);
            string sql2 = "update DATA_DISPLAY set DATA_COUNT = " + inp[0] + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'APP' and DATA_STATUS = 'CURRENT'";
            DB.Exec(sql2);
            //DataSet dss = DB.Query(sql1);
            //int numm = O2.O2I(dss.Tables[0].Rows[0][0]);
            string sql3 = "update DATA_DISPLAY set DATA_COUNT = " + inp[1] + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'APP' and DATA_STATUS = 'MORE'";
            DB.Exec(sql3);
        }

        public override void AfterImportDBDay(String time)
        {
            time = time.Insert(4, "-").Insert(7, "-");
            //string times = DateTime.Parse(time).AddDays(-6).ToString("yyyy-MM-dd");
            //string vsql = "select count(*) count from  (select * from VIDEO_DAY t where trunc(t.start_time)=to_date('" + time + "', 'yyyy-mm-dd'))";
            //string sql2 = "select count(*) count from (select SERER_IP from VIDEO_DAY t where t.start_time >= to_date('" + times + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') group by t.SERER_IP HAVING count(t.SERER_IP) > 3)";
            //DataSet ds = DB.Query(vsql);
            //int num = O2.O2I(ds.Tables[0].Rows[0][0]);
            int[] inp = WebData.AfterImportDBDay(time);
            string sql1 = "update DATA_DISPLAY set DATA_COUNT = " + inp[0] + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'APP' and DATA_STATUS = 'DAY'";
            DB.Exec(sql1);
            //DataSet dss = DB.Query(sql2);
            //int numm = O2.O2I(dss.Tables[0].Rows[0][0]);
            string sql3 = "update DATA_DISPLAY set DATA_COUNT = " + inp[1] + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'APP' and DATA_STATUS = 'WEEK'";
            DB.Exec(sql3);

            int s = WebData.AfterImportDBLong(DateTime.Now);
            string sql4 = "update DATA_DISPLAY set DATA_COUNT = " + s + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'APP' and DATA_STATUS = 'LONG'";
            DB.Exec(sql4);
        }
    }
}
