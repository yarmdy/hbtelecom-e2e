using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DWWinService
{
    class PlayAnalyzer : Analyzer
    {
        public override string GetKeyQuarter(Dictionary<string, object> dic)
        {
            return dic["IP_ADDRESS"].ToString();
        }
        public override string GetKeyDay(Dictionary<string, object> dic)
        {
            return dic["IP_ADDRESS"].ToString();
        }
        public override void CombineRowQuarter(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyQuarter(dic)];
            if (O2.O2I(dic_exist["PLAY_RATE"]) < O2.O2I(dic["PLAY_RATE"]))
            {
                dic_exist = dic;
            }
        }
        public override void CombineRowDay(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyDay(dic)];
            dic_exist["PLAY_RATE"] = O2.O2I(dic_exist["PLAY_RATE"]) + O2.O2I(dic["PLAY_RATE"]);
            dic_exist["PLAY_COUNT"] = O2.O2I(dic_exist["PLAY_COUNT"]) + O2.O2I(dic["PLAY_COUNT"]);
        }

        public override bool IsZhiCha(Dictionary<string, object> dic)
        {
            double gameRate = 1;
            gameRate = 1-O2.O2I(dic["PLAY_RATE"]).Div0(O2.O2I(dic["PLAY_COUNT"]));
            if(gameRate<0.9)
                return true;
            return false;
        }
        public override bool IsZhiChaDay(Dictionary<string, object> dic)
        {
            return IsZhiCha(dic);
        }
       
        public override string Str2DTStr(string str)
        {
           return DateTime.Parse("1970/1/1 8:30:00").AddSeconds(O2.O2I(str)).ToString("yyyy-MM-dd HH:mm:ss");;
        }
       
        public override void AfterImportDB(String time)
        {
            time = time.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":");
            //string timeq = DateTime.Parse(time).AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
            //string timeds = DateTime.Parse(time).ToString("yyyy-MM-dd");
            //string timede = DateTime.Parse(time).AddDays(1).ToString("yyyy-MM-dd");
            //string sql = "select count(*) count from  (select * from KQI_MIN t where t.start_time > to_date('" + timeq + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss'))";
            //string sql1 = "select count(*) count from (select ECGI from KQI_MIN t where t.start_time >= to_date('" + timeds + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time < to_date('" + timede + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ECGI HAVING count(t.ECGI) >= 3 and count(t.ECGI) <= 5)";
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
            //string sql = "select count(*) count from  (select * from KQI_DAY t where trunc(t.start_time)=to_date('" + time + "', 'yyyy-mm-dd'))";
            //string sql2 = "select count(*) count from (select ECGI from KQI_DAY t where t.start_time >= to_date('" + times + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ECGI HAVING count(t.ECGI) > 3)";
            //DataSet ds = DB.Query(sql);
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
