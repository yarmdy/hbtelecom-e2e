using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace DWWinService
{
    class KQIGoodAnalyzer : Analyzer
    {
        public override string GetKeyQuarter(Dictionary<string, object> dic)
        {
            return dic["CITY"].ToString();
        }
        public override void CombineRowQuarter(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyQuarter(dic)];
            dic_exist["FDG_NUM"] = O2.O2I(dic_exist["FDG_NUM"]) + O2.O2I(dic["FDG_NUM"]);
            dic_exist["FD_SUM"] = O2.O2I(dic_exist["FD_SUM"]) + O2.O2I(dic["FD_SUM"]);
            dic_exist["GAME_NUM"] = O2.O2I(dic_exist["GAME_NUM"]) + O2.O2I(dic["GAME_NUM"]);
            dic_exist["GAME_SUM"] = O2.O2I(dic_exist["GAME_SUM"]) + O2.O2I(dic["GAME_SUM"]);
            dic_exist["NEWS_NUM"] = O2.O2I(dic_exist["NEWS_NUM"]) + O2.O2I(dic["NEWS_NUM"]);
            dic_exist["NEWS_SUM"] = O2.O2I(dic_exist["NEWS_SUM"]) + O2.O2I(dic["NEWS_SUM"]);
            dic_exist["PAGE_NUM"] = O2.O2I(dic_exist["PAGE_NUM"]) + O2.O2I(dic["PAGE_NUM"]);
            dic_exist["VIDEO_GNUM"] = O2.O2I(dic_exist["VIDEO_GNUM"]) + O2.O2I(dic["VIDEO_GNUM"]);
            dic_exist["VIDEO_BNUM"] = O2.O2I(dic_exist["VIDEO_BNUM"]) + O2.O2I(dic["VIDEO_BNUM"]);
            dic_exist["VIDEO_SNUM"] = O2.O2I(dic_exist["VIDEO_SNUM"]) + O2.O2I(dic["VIDEO_SNUM"]);
            dic_exist["PAGE_SUM"] = O2.O2I(dic_exist["PAGE_SUM"]) + O2.O2I(dic["PAGE_SUM"]);
            dic_exist["BFLOW"] = O2.O2L(dic_exist["BFLOW"]) + O2.O2L(dic["BFLOW"]);
            dic_exist["TFLOW"] = O2.O2L(dic_exist["TFLOW"]) + O2.O2L(dic["TFLOW"]);
        }
        public override bool IsZhiCha(Dictionary<string, object> dic)
        {
            return true;
        }
        public override string DataConvert(string col, string val)
        {
            if (col == "ECGI")
            {
                long v;
                if (val.Length < 5)
                {
                    return val;
                }
                if (long.TryParse(val.Substring(5), System.Globalization.NumberStyles.AllowHexSpecifier, null, out v))
                {
                    val = v.ToString();
                }
            }
            if (col == "BFLOW")
            {
                val = (O2.O2L(val)).ToString();
            }
            if (col == "TFLOW")
            {
                val = (O2.O2L(val)).ToString();
            }
            return val;
        }
        public override string Str2DTStr(string str)
        {
            return DateTime.Parse("1970/1/1 8:30:00").AddSeconds(O2.O2I(str)).ToString("yyyy-MM-dd HH:mm:ss");
        }
        public override bool Valid(Dictionary<string, object> dic)
        {
            if (!string.IsNullOrEmpty(dic["ECGI"].ToString()) && !string.IsNullOrEmpty(dic["START_TIME"].ToString()))
            {

                int ecgi = O2.O2I(dic["ECGI"]);
                int two = ecgi >> 20;
                int one = ecgi % 0x100 / 0x10;
                if ((two <= 0x17 && two >= 0x10) || two == 0xE3)
                {
                    if (one == 0x0 || one == 0x1 || one == 0x3 || one == 0X5 || one == 0x6)
                    {
                        return true;
                    }
                }
                else if ((two <= 0x8D && two >= 0x87) || two == 0xF1 || two == 0xF2)
                {
                    if (one == 0x8 || one == 0x9 || one == 0xB || one == 0xD || one == 0xE)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public override void DoDay()
        {

        }
        public override bool ImportDB(System.Data.DataTable dt, bool bol)
        {
            var time = TimeStr[0];
            if (time.Substring(10) == "00")
            {
                var resCsv = "地市,网页优良率,视频优良率,游戏优良率,即使通讯优良率,业务感知优良率\r\n";
                var nrow = dt.NewRow();
                dt.Rows.Add(nrow);
                nrow["CITY"] = "全省";
                var timed=str2dt(time);
                foreach (DataRow dr in dt.Rows)
                {
                    dr["CITY"] = getCityStr(dr["CITY"].ToString());
                    if (dr["CITY"].ToString() == "") {
                        dr["CITY"] = "雄安";
                    }
                    dr["START_TIME"] = timed;
                    if (dr["CITY"].ToString() != "全省")
                    {
                        nrow["PAGE_SUM"] = O2.O2I(nrow["PAGE_SUM"]) + O2.O2I(dr["PAGE_SUM"]);
                        nrow["PAGE_NUM"] = O2.O2I(nrow["PAGE_NUM"]) + O2.O2I(dr["PAGE_NUM"]);
                        nrow["FD_SUM"] = O2.O2I(nrow["FD_SUM"]) + O2.O2I(dr["FD_SUM"]);
                        nrow["FDG_NUM"] = O2.O2I(nrow["FDG_NUM"]) + O2.O2I(dr["FDG_NUM"]);
                        nrow["VIDEO_SNUM"] = O2.O2I(nrow["VIDEO_SNUM"]) + O2.O2I(dr["VIDEO_SNUM"]);
                        nrow["VIDEO_GNUM"] = O2.O2I(nrow["VIDEO_GNUM"]) + O2.O2I(dr["VIDEO_GNUM"]);
                        nrow["VIDEO_BNUM"] = O2.O2I(nrow["VIDEO_BNUM"]) + O2.O2I(dr["VIDEO_BNUM"]);
                        nrow["GAME_SUM"] = O2.O2I(nrow["GAME_SUM"]) + O2.O2I(dr["GAME_SUM"]);
                        nrow["GAME_NUM"] = O2.O2I(nrow["GAME_NUM"]) + O2.O2I(dr["GAME_NUM"]);
                        nrow["NEWS_SUM"] = O2.O2I(nrow["NEWS_SUM"]) + O2.O2I(dr["NEWS_SUM"]);
                        nrow["NEWS_NUM"] = O2.O2I(nrow["NEWS_NUM"]) + O2.O2I(dr["NEWS_NUM"]);
                    }

                    double web_rate, fd_rate, vr_rate, vp_rate, play_rate, news_rate;
                    web_rate = fd_rate = vr_rate = vp_rate = play_rate = news_rate = 1;
                    if ((O2.O2I(dr["PAGE_SUM"])) != 0)
                    {
                        web_rate = 1 - 1.0 * O2.O2I(dr["PAGE_NUM"]) / O2.O2I(dr["PAGE_SUM"]);
                    }
                    if ((O2.O2I(dr["FD_SUM"])) != 0)
                    {
                        fd_rate = 1 - 1.0 * O2.O2I(dr["FDG_NUM"]) / O2.O2I(dr["FD_SUM"]);
                    }
                    if ((O2.O2I(dr["VIDEO_SNUM"])) != 0)
                    {
                        vr_rate = 1 - 1.0 * O2.O2I(dr["VIDEO_GNUM"]) / O2.O2I(dr["VIDEO_SNUM"]);
                        vp_rate = 1 - 1.0 * O2.O2I(dr["VIDEO_BNUM"]) / O2.O2I(dr["VIDEO_SNUM"]);
                    }
                    if ((O2.O2I(dr["GAME_SUM"])) != 0)
                    {
                        play_rate = 1 - 1.0 * O2.O2I(dr["GAME_NUM"]) / O2.O2I(dr["GAME_SUM"]);
                    }
                    if ((O2.O2I(dr["NEWS_SUM"])) != 0)
                    {
                        news_rate = 1 - 1.0 * O2.O2I(dr["NEWS_NUM"]) / O2.O2I(dr["NEWS_SUM"]);
                    }
                    double rate = 0.5 * (0.8 * web_rate + 0.2 * fd_rate) + 0.3 * (0.8 * vr_rate + 0.2 * vp_rate) + 0.1 * play_rate + 0.1 * news_rate;
                    resCsv += getCityStr(dr["CITY"].ToString().Trim()) + "," + (0.8 * web_rate + 0.2 * fd_rate) + "," + (0.8 * vr_rate + 0.2 * vp_rate) + "," + play_rate + "," + news_rate + "," + rate + "\r\n";
                }
                File.WriteAllText(cfg.custom["createpath"]+time+".csv", resCsv, Encoding.GetEncoding("gb2312"));
                #region 入库
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(DB.ConnectStr))
                {
                    Oracle.DataAccess.Client.OracleBulkCopy bulkCopy = null;
                    try
                    {
                        connection.Open();
                        bulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(connection, Oracle.DataAccess.Client.OracleBulkCopyOptions.UseInternalTransaction);
                        //bulkCopy.BatchSize = 1000;

                        bulkCopy.DestinationTableName = "CITY_GOODRATE";
                        if (dt != null && dt.Rows.Count != 0)
                        {
                            {
                                //bulkCopy.ColumnMappings.Add("CITY", "CITY");
                                //bulkCopy.ColumnMappings.Add("CTIME", "START_TIME");
                                //bulkCopy.ColumnMappings.Add("FST_BAD_TIMES", "FDG_NUM");
                                //bulkCopy.ColumnMappings.Add("FST_TIMES", "FD_SUM");
                                //bulkCopy.ColumnMappings.Add("PAGE_BAD_TIMES", "PAGE_NUM");
                                //bulkCopy.ColumnMappings.Add("PAGE_TIMES", "PAGE_SUM");
                                //bulkCopy.ColumnMappings.Add("VIDEO_RATEBAD_TIMES", "VIDEO_GNUM");
                                //bulkCopy.ColumnMappings.Add("VIDEO_STALLBAD_TIMES", "VIDEO_BNUM");
                                //bulkCopy.ColumnMappings.Add("VIDEO_TIMES", "VIDEO_SNUM");
                                //bulkCopy.ColumnMappings.Add("GAME_BAD_TIMES", "GAME_NUM");
                                //bulkCopy.ColumnMappings.Add("GAME_TIMES", "GAME_SUM");
                                //bulkCopy.ColumnMappings.Add("IM_BAD_TIMES", "NEWS_NUM");
                                //bulkCopy.ColumnMappings.Add("IM_TIMES", "NEWS_SUM");
                                bulkCopy.ColumnMappings.Add("CITY", "CITY");
                                bulkCopy.ColumnMappings.Add("START_TIME", "CTIME");
                                bulkCopy.ColumnMappings.Add("FDG_NUM", "FST_BAD_TIMES");
                                bulkCopy.ColumnMappings.Add("FD_SUM", "FST_TIMES");
                                bulkCopy.ColumnMappings.Add("PAGE_NUM", "PAGE_BAD_TIMES");
                                bulkCopy.ColumnMappings.Add("PAGE_SUM", "PAGE_TIMES");
                                bulkCopy.ColumnMappings.Add("VIDEO_GNUM", "VIDEO_RATEBAD_TIMES");
                                bulkCopy.ColumnMappings.Add("VIDEO_BNUM", "VIDEO_STALLBAD_TIMES");
                                bulkCopy.ColumnMappings.Add("VIDEO_SNUM", "VIDEO_TIMES");
                                bulkCopy.ColumnMappings.Add("GAME_NUM", "GAME_BAD_TIMES");
                                bulkCopy.ColumnMappings.Add("GAME_SUM", "GAME_TIMES");
                                bulkCopy.ColumnMappings.Add("NEWS_NUM", "IM_BAD_TIMES");
                                bulkCopy.ColumnMappings.Add("NEWS_SUM", "IM_TIMES");
                            }
                            bulkCopy.BulkCopyTimeout = 1200;
                            bulkCopy.WriteToServer(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                        if (bulkCopy != null)
                            bulkCopy.Close();
                    }
                }
                #endregion
            }
            return true;
        }
        public override string[] FilterFile(string[] files, string timestr)
        {
            var dt = str2dt(timestr);
            var hasstr = dt.ToString("yyyyMMddHH")+"00";
            var hasstr2 = dt.AddHours(-1).ToString("yyyyMMddHH");
            var hasnostr = dt.AddHours(-1).ToString("yyyyMMddHH")+"00";
            return files.Where(a=>a.Contains(hasstr2) || a.Contains(hasstr2) && !a.Contains(hasnostr) ).ToArray();
        }
        public override bool IsDoQuarter(string time)
        {
            return time.Substring(10) == "00";
        }
        private string getCityStr(string city) {
            switch (city) { 
                case "11":
                    city = "石家庄";
                    break;
                case "12":
                    city = "唐山";
                    break;
                case "13":
                    city = "张家口";
                    break;
                case "14":
                    city = "保定";
                    break;
                case "15":
                    city = "廊坊";
                    break;
                case "16":
                    city = "沧州";
                    break;
                case "17":
                    city = "邢台";
                    break;
                case "18":
                    city = "邯郸";
                    break;
                case "19":
                    city = "秦皇岛";
                    break;
                case "20":
                    city = "承德";
                    break;
                case "21":
                    city = "衡水";
                    break;
            }
            return city;
        }
        private DateTime str2dt(string s) {
            var dt = DateTime.Parse(s.Substring(0, 4) + "-" + s.Substring(4, 2) + "-" + s.Substring(6, 2) + " " + s.Substring(8, 2) + ":" + s.Substring(8, 2) + ":00");
            if (dt.Minute == 0)
            {
                dt = dt.AddHours(-1);
            }
            else {
                dt=dt.AddMinutes(-dt.Minute);
            }
            return dt;
        }
    }
}
