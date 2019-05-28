using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace DWWinService
{
    class AllKQIDataAnalyzer : Analyzer
    {
        public override string GetKeyDay(Dictionary<string, object> dic)
        {
            return dic["ECI"].ToString() + "_" + Str2DTStr(dic["CREATETIME"].ToString()).Substring(0, 10);
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
        public override bool IsZhiChaDay(Dictionary<string, object> dic)
        {
            return true;
        }
        public override string DataConvert(string col, string val)
        {
            if (col == "ECI")
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
            if (!string.IsNullOrEmpty(dic["ECI"].ToString()) && !string.IsNullOrEmpty(dic["CREATETIME"].ToString()))
            {

                int ecgi = O2.O2I(dic["ECI"]);
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
        public override void AfterImportDBDay(String time)
        {
            var sqlinsert = @"insert into DATA_KQITJ
select to_date('{0}','yyyymmdd') cdate,a.ECI,a.SC_NAME,a.CITY,b.goodrate,d.DOWN_PDCP flow,case d.MANUFACTOR when '诺基亚' then d.DOWN_PRB/100 else d.DOWN_PRB end prb,d.RRC_CONNCOUNT rrc,NVL(DECODE(c.RSRP_COUNT_TOTAL,0,0,1-(c.RSRP_COUNT_LOW/c.RSRP_COUNT_TOTAL)),null) rsrp from v_workparameter a
left join kqigood_day b on b.createtime=to_date('{0}','yyyymmdd') and a.eci=b.eci
left join data_mr c on c.sdate=to_date('{0}','yyyymmdd') and a.eci=c.eci
left join (select * from (select t.eci,t.DOWN_PDCP,t.manufactor,t.down_prb,t.rrc_conncount,row_number()over(partition by t.eci order by t.down_pdcp desc) top1 from data_kpiinfo t where t.time>=to_date('{0}','yyyymmdd') and t.time <to_date('{0}','yyyymmdd')+1))d on d.top1=1 and a.eci=d.eci";
            sqlinsert = sqlinsert.Replace("{0}", time);
            DB.Exec(sqlinsert);
            var sql = "select cdate,eci,sc_name,city,round(goodrate,8)goodrate,flow,prb,rrc,round(rsrp,8)rsrp from DATA_KQITJ where cdate=to_date('" + time + "','yyyymmdd') order by city,eci";
            var dt = DB.QueryAsDt(sql);
            var csv = new StringBuilder();
            foreach (DataColumn col in dt.Columns)
            {
                csv.Append(col.ColumnName + ",");
            }
            csv.Append("\r\n");
            foreach (DataRow dr in dt.Rows)
            {
                for (int col=0;col<9;col++)
                {
                    csv.Append(dr[col] + ",");
                }
                csv.Append("\r\n");
            }
            File.WriteAllText(cfg.custom["createpath"] + "ALLKQIINFO" + time + ".csv", csv.ToString(), Encoding.GetEncoding("gb2312"));
        }
        public override void DoQuarter()
        {

        }
        public override void ProcessRow(DataRow dr, Dictionary<string, object> dic)
        {
            double web_rate, fd_rate, vr_rate, vp_rate, play_rate, news_rate;
            web_rate = fd_rate = vr_rate = vp_rate = play_rate = news_rate = 1;
            if ((O2.O2I(dic["PAGE_SUM"])) != 0)
            {
                web_rate = 1 - 1.0 * O2.O2I(dic["PAGE_NUM"]) / O2.O2I(dic["PAGE_SUM"]);
            }
            if ((O2.O2I(dic["FD_SUM"])) != 0)
            {
                fd_rate = 1 - 1.0 * O2.O2I(dic["FDG_NUM"]) / O2.O2I(dic["FD_SUM"]);
            }
            if ((O2.O2I(dic["VIDEO_SNUM"])) != 0)
            {
                vr_rate = 1 - 1.0 * O2.O2I(dic["VIDEO_GNUM"]) / O2.O2I(dic["VIDEO_SNUM"]);
                vp_rate = 1 - 1.0 * O2.O2I(dic["VIDEO_BNUM"]) / O2.O2I(dic["VIDEO_SNUM"]);
            }
            if ((O2.O2I(dic["GAME_SUM"])) != 0)
            {
                play_rate = 1 - 1.0 * O2.O2I(dic["GAME_NUM"]) / O2.O2I(dic["GAME_SUM"]);
            }
            if ((O2.O2I(dic["NEWS_SUM"])) != 0)
            {
                news_rate = 1 - 1.0 * O2.O2I(dic["NEWS_NUM"]) / O2.O2I(dic["NEWS_SUM"]);
            }
            double bflow = 1.0 * O2.O2L(dic["BFLOW"]) / Math.Pow(1024, 3);
            double rate = 0.5 * (0.8 * web_rate + 0.2 * fd_rate) + 0.3 * (0.8 * vr_rate + 0.2 * vp_rate) + 0.1 * play_rate + 0.1 * news_rate;
            dr["GOODRATE"] = rate;
            if (dr["CREATETIME"] != DBNull.Value)
            {
                dr["CREATETIME"] = ((DateTime)dr["CREATETIME"]).Date;
            }
        }
    }
}
