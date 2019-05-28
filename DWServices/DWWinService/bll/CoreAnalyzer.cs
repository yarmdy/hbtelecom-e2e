using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    class CoreAnalyzer : Analyzer
    {
        public override string GetKeyQuarter(Dictionary<string, object> dic)
        {
            return dic["MME_ID"].ToString()+"_"+ Str2DTStr(dic["START_TIME"].ToString());
        }

        public override string GetKeyDay(Dictionary<string, object> dic)
        {
            return dic["MME_ID"].ToString() + "_" + Str2DTStr(dic["START_TIME"].ToString()).Substring(0, 10);
        }

        public override void CombineRowQuarter(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyQuarter(dic)];
            if(O2.O2I(dic_exist["ATTACH_REQUEST"])<O2.O2I(dic["ATTACH_REQUEST"]))
            {
                dic_exist = dic;
            }
        }

        public override void CombineRowDay(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyDay(dic)];
            dic_exist["ATTACH_REQUEST"] = O2.O2I(dic_exist["ATTACH_REQUEST"]) + O2.O2I(dic["ATTACH_REQUEST"]);
            dic_exist["ATTACH_SUC"] = O2.O2I(dic_exist["ATTACH_SUC"]) + O2.O2I(dic["ATTACH_SUC"]);
            dic_exist["SERVICE_REQUEST"] = O2.O2I(dic_exist["SERVICE_REQUEST"]) + O2.O2I(dic["SERVICE_REQUEST"]);
            dic_exist["SERVICE_SUC"] = O2.O2I(dic_exist["SERVICE_SUC"]) + O2.O2I(dic["SERVICE_SUC"]);
            dic_exist["TAU_REQUEST"] = O2.O2I(dic_exist["TAU_REQUEST"]) + O2.O2I(dic["TAU_REQUEST"]);
            dic_exist["TAU_SUC"] = O2.O2I(dic_exist["TAU_SUC"]) + O2.O2I(dic["TAU_SUC"]);
        }

        public override string Str2DTStr(string str)
        {
            return DateTime.Parse("1970/1/1 8:30:00").AddSeconds(O2.O2I(str)).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override bool IsZhiCha(Dictionary<string, object> dic)
        {
            int[] nums = { O2.O2I(dic["ATTACH_REQUEST"]), O2.O2I(dic["SERVICE_REQUEST"]), O2.O2I(dic["TAU_REQUEST"]) };
            int[] vals = { O2.O2I(dic["ATTACH_SUC"]), O2.O2I(dic["SERVICE_SUC"]), O2.O2I(dic["TAU_SUC"]) };
            bool isBad = false;
            for (int i = 0; i < nums.Length; i++)
            {
                if (vals[i] * 1.0 / nums[i] < 0.92)
                {
                    isBad = true;
                    break;
                }
            }
            return isBad;
        }

        public override bool IsZhiChaDay(Dictionary<string, object> dic)
        {
            return IsZhiCha(dic);
        }

        public override void AfterImportDB(String time)
        {
            time = time.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":");
            string timeq = DateTime.Parse(time).AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
            string timeds = DateTime.Parse(time).ToString("yyyy-MM-dd");
            string timede = DateTime.Parse(time).AddDays(1).ToString("yyyy-MM-dd");
            string sql = "select count(*) count from  (select * from CORE_MIN t where t.start_time > to_date('" + timeq + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss'))";
            string sql1 = "select count(*) count from (select MME_ID from CORE_MIN t where t.start_time > to_date('" + timeds + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + timede + "', 'yyyy-mm-dd hh24:mi:ss') group by t.MME_ID HAVING count(t.MME_ID) >= 3 and count(t.MME_ID) <= 5)";
            DataSet ds = DB.Query(sql);
            int num = O2.O2I(ds.Tables[0].Rows[0][0]);
            string sql2 = "update DATA_DISPLAY set DATA_COUNT = " + num + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'CORE' and DATA_STATUS = 'CURRENT'";
            DB.Exec(sql2);
            DataSet dss = DB.Query(sql1);
            int numm = O2.O2I(dss.Tables[0].Rows[0][0]);
            string sql3 = "update DATA_DISPLAY set DATA_COUNT = " + numm + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'CORE' and DATA_STATUS = 'MORE'";
            DB.Exec(sql3);
        }

        public override void AfterImportDBDay(String time)
        {
            time = time.Insert(4, "-").Insert(7, "-");
            string times = DateTime.Parse(time).AddDays(-6).ToString("yyyy-MM-dd");
            string sql = "select count(*) count from  (select * from CORE_DAY t where trunc(t.start_time)=to_date('" + time + "', 'yyyy-mm-dd'))";
            string sql2 = "select count(*) count from (select MME_ID from CORE_DAY t where  trunc(t.start_time) >= to_date('" + times + "', 'yyyy-mm-dd') and  trunc(t.start_time) <= to_date('" + time + "', 'yyyy-mm-dd') group by t.MME_ID HAVING count(t.MME_ID) >= 3 and max(trunc(t.start_time))=to_date('" + time + "', 'yyyy-mm-dd'))";
            DataSet ds = DB.Query(sql);
            int num = O2.O2I(ds.Tables[0].Rows[0][0]);
            string sql1 = "update DATA_DISPLAY set DATA_COUNT = " + num + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'CORE' and DATA_STATUS = 'DAY'";
            DB.Exec(sql1);
            DataSet dss = DB.Query(sql2);
            int numm = O2.O2I(dss.Tables[0].Rows[0][0]);
            string sql3 = "update DATA_DISPLAY set DATA_COUNT = " + numm + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'CORE' and DATA_STATUS = 'WEEK'";
            DB.Exec(sql3);
            int s = GetCoreLongData(DateTime.Now);
            string sql4 = "update DATA_DISPLAY set DATA_COUNT = " + s + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'CORE' and DATA_STATUS = 'LONG'";
            DB.Exec(sql4);
        }
        public int GetCoreLongData(DateTime time)
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
                string sql = "select * from CORE_DAY where  trunc(START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and MME_ID in (select MME_ID from CORE_DAY t where  trunc(t.START_TIME) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.START_TIME) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.MME_ID HAVING count(t.MME_ID) > 3 )";
               // DataTable webdata = OraConnect.ReadData(sql);
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
