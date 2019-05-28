using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO; 

namespace DWWinService
{
    class WifiAnalyzer : Analyzer
    {
        public override string GetKeyQuarter(Dictionary<string, object> dic)
        {
            return dic["ECGI"].ToString() + "_" + Str2DTStr(dic["START_TIME"].ToString());
        }

        public override string GetKeyDay(Dictionary<string, object> dic)
        {
            return dic["ECGI"].ToString() + "_" + Str2DTStr(dic["START_TIME"].ToString()).Substring(0, 10);
        }

        public override void CombineRowQuarter(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyQuarter(dic)];
            if (O2.O2I(dic_exist["FD_SUM"]) < O2.O2I(dic["FD_SUM"]))
            {
                dic_exist = dic;
            }
        }

        public override void CombineRowDay(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyDay(dic)];
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
            double web_rate, fd_rate, vr_rate, vp_rate, play_rate, news_rate;
            web_rate = fd_rate = vr_rate = vp_rate = play_rate = news_rate = 1;
            if ((O2.O2I(dic["PAGE_SUM"])) != 0)
            {
                web_rate = 1-1.0*O2.O2I(dic["PAGE_NUM"]) / O2.O2I(dic["PAGE_SUM"]);
            }
            if ((O2.O2I(dic["FD_SUM"])) != 0)
            {
                fd_rate = 1 - 1.0 * O2.O2I(dic["FDG_NUM"]) / O2.O2I(dic["FD_SUM"]);
            }
            if ((O2.O2I(dic["VIDEO_SNUM"])) != 0)
            {
                vr_rate = 1-1.0 * O2.O2I(dic["VIDEO_GNUM"]) / O2.O2I(dic["VIDEO_SNUM"]);
                vp_rate = 1-1.0 * O2.O2I(dic["VIDEO_BNUM"]) / O2.O2I(dic["VIDEO_SNUM"]);
            }
            if ((O2.O2I(dic["GAME_SUM"])) != 0)
            {
                play_rate = 1-1.0 * O2.O2I(dic["GAME_NUM"]) / O2.O2I(dic["GAME_SUM"]);
            }
            if ((O2.O2I(dic["NEWS_SUM"])) != 0)
            {
                news_rate = 1-1.0 * O2.O2I(dic["NEWS_NUM"]) / O2.O2I(dic["NEWS_SUM"]);
            }
            double rate = 0.5 * (0.8 * web_rate + 0.2 * fd_rate) + 0.3 * (0.8 * vr_rate + 0.2 * vp_rate) + 0.1 * play_rate + 0.1 * news_rate;
            //涉及两个地方
            if (rate < 0.8 && O2.O2I(dic["FD_SUM"])>=500)
                return true;
            else
                return false;
        }

        public override bool IsZhiChaDay(Dictionary<string, object> dic)
        {
            double web_rate, fd_rate, vr_rate, vp_rate, play_rate, news_rate;
            web_rate = fd_rate = vr_rate = vp_rate = play_rate = news_rate = 1;
            if ((O2.O2I(dic["PAGE_SUM"])) != 0)
            {
                web_rate = 1 - 1.0 * O2.O2I(dic["PAGE_NUM"]) / O2.O2I(dic["PAGE_SUM"]);
            }
            if ((O2.O2I(dic["FD_SUM"])) != 0)
            {
                fd_rate = 1-1.0 * O2.O2I(dic["FDG_NUM"]) / O2.O2I(dic["FD_SUM"]);
            }
            if ((O2.O2I(dic["VIDEO_SNUM"])) != 0)
            {
                vr_rate = 1-1.0 * O2.O2I(dic["VIDEO_GNUM"]) / O2.O2I(dic["VIDEO_SNUM"]);
                vp_rate = 1-1.0 * O2.O2I(dic["VIDEO_BNUM"]) / O2.O2I(dic["VIDEO_SNUM"]);
            }
            if ((O2.O2I(dic["GAME_SUM"])) != 0)
            {
                play_rate = 1-1.0 * O2.O2I(dic["GAME_NUM"]) / O2.O2I(dic["GAME_SUM"]);
            }
            if ((O2.O2I(dic["NEWS_SUM"])) != 0)
            {
                news_rate = 1-1.0 * O2.O2I(dic["NEWS_NUM"]) / O2.O2I(dic["NEWS_SUM"]);
            }
            double bflow = 1.0 * O2.O2L(dic["BFLOW"]) / Math.Pow(1024, 3);
            double rate = 0.5 * (0.8 * web_rate + 0.2 * fd_rate) + 0.3 * (0.8 * vr_rate + 0.2 * vp_rate) + 0.1 * play_rate + 0.1 * news_rate;
            //涉及两个地方
            if (rate < 0.8 && O2.O2I(dic["FD_SUM"])>=3500 /*&& bflow>=3*/)
                return true;
            else
                return false;
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
            if (col == "BFLOW") {
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
            if(!string.IsNullOrEmpty(dic["ECGI"].ToString()) && !string.IsNullOrEmpty(dic["START_TIME"].ToString()))
            {
                
                int ecgi = O2.O2I(dic["ECGI"]);
                int two = ecgi >> 20;
                int one = ecgi % 0x100 / 0x10; 
                if ((two<=0x17 && two>=0x10) || two==0xE3)
                {
                    if(one==0x0 || one==0x1 || one==0x3 || one==0X5 || one==0x6)
                    {
                        return true;
                    }
                }
                else if((two<=0x8D && two>=0x87) || two==0xF1 || two==0xF2)
                {
                    if(one==0x8 || one==0x9 || one==0xB || one==0xD || one==0xE)
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

        public override void AfterImportDB(String time)
        {
            time = time.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":");
            string timeq = DateTime.Parse(time).AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
            string timeds = DateTime.Parse(time).ToString("yyyy-MM-dd");
            string timede = DateTime.Parse(time).AddDays(1).ToString("yyyy-MM-dd");
            string sql = "select count(*) count from  (select * from KQI_MIN t join V_WORKPARAMETER v on t.ecgi = v.eci where t.start_time > to_date('" + timeq + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss'))";
            string sql1 = "select count(*) count from (select ECGI from KQI_MIN t join V_WORKPARAMETER v on t.ecgi = v.eci where t.start_time > to_date('" + timeds + "', 'yyyy-mm-dd hh24:mi:ss') and t.start_time <= to_date('" + timede + "', 'yyyy-mm-dd hh24:mi:ss') group by t.ECGI HAVING count(t.ECGI) > 5)";//涉及两个地方 count(t.ECGI) >= 3 and
            DataSet ds = DB.Query(sql);
            int num = O2.O2I(ds.Tables[0].Rows[0][0]);
            string sql2 = "update DATA_DISPLAY set DATA_COUNT = "+ num+",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'WIFI' and DATA_STATUS = 'CURRENT'";
            DB.Exec(sql2);
            DataSet dss = DB.Query(sql1);
            int numm = O2.O2I(dss.Tables[0].Rows[0][0]);
            string sql3 = "update DATA_DISPLAY set DATA_COUNT = " + numm + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'WIFI' and DATA_STATUS = 'MORE'";
            DB.Exec(sql3);
            

        }

        public override void AfterImportDBDay(String time)
        {
            time = time.Insert(4, "-").Insert(7, "-");
            string times = DateTime.Parse(time).AddDays(-6).ToString("yyyy-MM-dd");
            string sql = "select count(*) count from  (select * from KQI_DAY t join V_WORKPARAMETER v on t.ecgi = v.eci where trunc(t.start_time)=to_date('" + time + "', 'yyyy-mm-dd'))";
            string sql2 = "select count(*) count from (select ECGI from KQI_DAY t join V_WORKPARAMETER v on t.ecgi = v.eci where trunc(t.start_time) >= to_date('" + times + "', 'yyyy-mm-dd') and trunc(t.start_time) <= to_date('" + time + "', 'yyyy-mm-dd') group by t.ECGI HAVING count(t.ECGI) >= 3 and max(trunc(t.start_time))=to_date('" + time + "', 'yyyy-mm-dd'))";
            DataSet ds = DB.Query(sql);
            int num = O2.O2I(ds.Tables[0].Rows[0][0]);
            string sql1 = "update DATA_DISPLAY set DATA_COUNT = " + num + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'WIFI' and DATA_STATUS = 'DAY'";
            DB.Exec(sql1);
            DataSet dss = DB.Query(sql2);
            int numm = O2.O2I(dss.Tables[0].Rows[0][0]);
            string sql3 = "update DATA_DISPLAY set DATA_COUNT = " + numm + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'WIFI' and DATA_STATUS = 'WEEK'";
            DB.Exec(sql3);


            string datatime = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd");
            string sqlcsv = "select t.ecgi 小区编号,a.sc_name 小区名称,a.sc_lon YPOS,a.sc_lat XPOS,a.antennaazimuth 方向角,to_char(trunc(t.start_time),'yyyy/fmmm/dd') 时间,a.city 地市,'' 质差类型,'' 页面打开时延,'' 视频下载速率,'' 页面响应时延,'' 场景,''定界,'' 质差话单次数,'' 访问网站,'' 导入文件类型,'' 导入时间  from V_WORKPARAMETER a join kqi_day t on a.eci=t.ecgi where trunc(t.start_time)=to_date('" + datatime + "', 'yyyy/mm/dd')";
            //"select * from WORKPARAMETER a join kqi_day t on a.eci=t.ecgi where trunc(t.start_time)=to_date('" + datatime + "', 'yyyy/mm/dd')";


            DataSet dscsv = DB.Query(sqlcsv);

            DataTable dtcsv = dscsv.Tables[0];
            //dtcsv.Columns["定界"].DefaultValue="无线侧质差";
            for (int i = 0; i < dtcsv.Rows.Count; i++)
            {
                int j = O2.O2I(dtcsv.Rows[i]["小区编号"]);
                string str = "46011" + j.ToString("X");
                dtcsv.Rows[i]["小区编号"] = str;
                //Convert.ToString(dtcsv.Rows[i]["小区编号"], 16);
                dtcsv.Rows[i]["定界"] = "无线侧质差";
                string sqld = "select * from kqi_day t where t.ecgi= " + j + " and trunc(start_time)=to_date('" + dtcsv.Rows[i]["时间"] + "','yyyy-mm-dd')";
                DataTable dst = DB.QueryAsDt(sqld);
                if (dst.Rows.Count > 0 && dst != null)
                {
                    double webstrzd = 0.8 * (1 - O2.O2I(dst.Rows[0]["PAGE_NUM"]).Div0(O2.O2I(dst.Rows[0]["PAGE_SUM"]))) + 0.2 * (1 - O2.O2I(dst.Rows[0]["FDG_NUM"]).Div0(O2.O2I(dst.Rows[0]["FD_SUM"])));
                    double videostrzd = ((1 - O2.O2I(dst.Rows[0]["VIDEO_GNUM"]).Div0(O2.O2I(dst.Rows[0]["VIDEO_SNUM"]))) * 0.8) + ((1 - O2.O2I(dst.Rows[0]["VIDEO_BNUM"]).Div0(O2.O2I(dst.Rows[0]["VIDEO_SNUM"]))) * 0.2);
                    double signalstrzd = 1 - O2.O2I(dst.Rows[0]["NEWS_NUM"]).Div0(O2.O2I(dst.Rows[0]["NEWS_SUM"]));
                    double playstrzd = 1 - O2.O2I(dst.Rows[0]["GAME_NUM"]).Div0(O2.O2I(dst.Rows[0]["GAME_SUM"]));
                    string strzd = "WEB：" + String.Format("{0:P}", webstrzd) + ";视频：" + String.Format("{0:P}", videostrzd) + ";即时通信：" + String.Format("{0:P}", signalstrzd) + ";游戏：" + String.Format("{0:P}", playstrzd) + ";";
                    dtcsv.Rows[i]["质差类型"] = strzd;
                }

            }
            DataToCSV(dtcsv);

            int s = GetWifiLongData(DateTime.Now);
            string sql4 = "update DATA_DISPLAY set DATA_COUNT = " + s + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'WIFI' and DATA_STATUS = 'LONG'";
            DB.Exec(sql4);

            
        }
        public void DataToCSV(DataTable dt) {
            string strfilename = "无线侧质差筛选";
            string path = "";
            strfilename = strfilename + DateTime.Now.AddDays(-1).ToString("yyyy-MMdd");
            strfilename = strfilename + ".csv";
            path = cfg.custom["createpath"] + strfilename;
            //path = "ftp://" + ip + ":" + port + path + strfilename;
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
            //写出列名称
            string data = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }

        public int GetWifiLongData(DateTime time)
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
                string sql = "select * from KQI_DAY k join V_WORKPARAMETER v on k.ecgi = v.eci where  trunc(k.start_time) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and trunc(k.start_time) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') and k.ECGI in (select ECGI from KQI_DAY t where  trunc(t.start_time) >= to_date('" + begindaystime + "', 'yyyy-mm-dd') and  trunc(t.start_time) <= to_date('" + enddaystime + "', 'yyyy-mm-dd') group by t.ECGI HAVING count(t.ECGI) > 3 )";
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
            if (ds!=null&&ds.Tables.Count>0)
            {
                s = ds.Tables[0].Rows.Count;
            }
            return s;
        }
      
    }
}
