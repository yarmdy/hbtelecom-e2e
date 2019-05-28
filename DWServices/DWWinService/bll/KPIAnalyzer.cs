using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.OleDb;

namespace DWWinService
{
    class KPIAnalyzer : Analyzer
    {
        public override void DoDay()
        {

        }
        public override string[] TimeStr
        {
            get
            {
                DateTime dt = DateTime.Now.AddHours(-3);
                string min = dt.ToString("yyyyMMddHH");
                string day = dt.AddDays(-1).ToString("yyyyMMdd");
                return new string[] { min, day };
            }
        }
        public override string[] TmpFiles
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var filec in cfg.csvs)
                {
                    if (filec.filetmp.path != "")
                    {
                        var files = Directory.GetFiles(filec.filetmp.path, "*.*");
                        foreach (var file in files)
                        {
                            res.Add(file);
                        }
                    }
                }
                return res.Distinct().ToArray();
            }
        }
        public override void AfterDownloadCsv()
        {
            var delfs = TmpFiles.Where(a => a.IndexOf("NB") >= 0 || a.IndexOf("PM_KPI_" + TimeStr[0]) >= 0);
            foreach (var df in delfs)
            {
                File.Delete(df);
            }
            var zipfiles = TmpFiles.Where(a => a.IndexOf(".zip") >= 0).ToArray();
            foreach (var file in zipfiles)
            {
                ZIP.unzip(file, file.Substring(0, file.LastIndexOf('\\') + 1));
                File.Delete(file);
            }
        }
        public override DataTable Analysis(bool bQuarter, string[] files)
        {
            var dtime = DateTime.Now;
            dtime = dtime.Date.AddHours(dtime.Hour - 4);
            var sbkpi = new StringBuilder();
            sbkpi.Append(",,,,,,,P330143,P330146,C330246,C330591,C330569,C330267,C330287,C330290,C330390,C330398,C330291,C350269,C350274,C330425,C330111,C330127,C330128,C350197,C330741,C330635,C330645,C330736,C330737,C330536,C330510,C330512,C311357,C330554\r\n");
            sbkpi.Append("地市,日期,eNodeBID,eNode名称,小区ID,小区名称,厂家,上行PRB平均利用率,下行PRB平均利用率,4.16_平均RRC连接用户数（个）,8.3_PDCP层下行用户面流量字节数（MByte）,8.1_PDCP层上行用户面流量字节数（MByte）,4.19_平均激活用户数（个）,5.7_RRC连接建立成功率（%）,5.10_RRC连接建立失败次数（其它原因）（次）,5.28_寻呼拥塞率（%）,5.36_RRC连接重建比例（%）,5.11_RRC连接重建请求次数（次）,L1.13UE上下文掉线率,L1.14UE上下文异常释放次数,6.6_E－RAB掉线率（%）,E—RAB异常释放次数,RRC连接重建比例,RRC连接重建成功次数,CQI≥7比例,11.1_下行PRB双流占比（%）,8.13_用户体验上行平均速率（Mbps）,8.14_用户体验下行平均速率（Mbps）,10.1_小区退服时长（s）,10.3_小区可用率（%）,7.11_系统内切换成功率（%）,6.18_E－RAB异常释放次数（网络拥塞）（次）,6.20_E－RAB异常释放次数（切换失败）（次）,4G切3G比例（%）,7.27_LTE重定向到3G的尝试次数（次）\r\n");
            foreach (var file in files)
            {
                //诺基亚
                if (file.IndexOf("PM_KPI") >= 0)
                {
                    var scols = new Dictionary<string, int>();
                    var lines = File.ReadLines(file, System.Text.Encoding.GetEncoding("gbk"));
                    var bhead = true;
                    foreach (var line in lines)
                    {
                        if (bhead)
                        {
                            bhead = false;
                            var cols = line.Split(new char[] { ',' }, StringSplitOptions.None);
                            for (int i = 0; i < cols.Length; i++)
                            {
                                scols[cols[i].Trim()] = i;
                            }
                        }
                        else
                        {
                            var vals = line.Split(new char[] { ',' }, StringSplitOptions.None);
                            var ntime = O2.O2DT(vals[scols["TIME"]]);
                            if (dtime != ntime) continue;
                            var ln = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34}\r\n"
                                , nkcityconvert(vals[scols["CITYNAME"]])
                                , vals[scols["TIME"]]
                                , vals[scols["ENBID"]]
                                , ""
                                , vals[scols["LCELLID"]]
                                , vals[scols["CELLNAME"]]
                                , "诺基亚"
                                , vals[scols["上行PRB平均占用率"]]
                                , vals[scols["下行PRB平均占用率"]]
                                , vals[scols["平均RRC连接用户数"]]
                                , vals[scols["PDCP层下行用户面流量MB"]]
                                , vals[scols["PDCP层上行用户面流量MB"]]
                                , vals[scols["平均激活用户数"]]
                                , vals[scols["RRC连接成功率"]]
                                , vals[scols["RRC失败次数(其它原因)"]]
                                , vals[scols["寻呼拥塞率"]]
                                , vals[scols["RRC重建比率"]]
                                , vals[scols["RRC重建请求次数"]]
                                , vals[scols["UE上下文掉线率"]]
                                , vals[scols["UE上下文异常释放次数"]]
                                , vals[scols["ERAB掉线率"]]
                                , vals[scols["ERAB异常释放次数"]]
                                , vals[scols["RRC重建比率"]]
                                , vals[scols["RRC重建成功次数"]]
                                , vals[scols["CQI优良率"]]
                                , vals[scols["下行PRB双流占比"]]
                                , vals[scols["用户体验上行平均速率"]]
                                , vals[scols["用户体验下行平均速率"]]
                                , ""//10.1_小区退服时长（s）
                                , ""//10.3_小区可用率（%）
                                , ""//7.11_系统内切换成功率（%）
                                , vals[scols["ERAB异常释放(网络拥塞)"]]
                                , vals[scols["ERAB异常释放(切换失败)"]]
                                , vals[scols["4G重定向3G比例16"]]
                                , ""//7.27_LTE重定向到3G的尝试次数（次）
                                );
                            sbkpi.Append(ln);
                        }
                    }
                }
                //中兴
                else if (file.IndexOf("xingneng-wangjian") >= 0)
                {
                    var scols = new Dictionary<string, int>();
                    var lines = File.ReadLines(file, System.Text.Encoding.GetEncoding("gbk"));
                    var bhead = true;
                    foreach (var line in lines)
                    {
                        if (bhead)
                        {
                            bhead = false;
                            var cols = line.Split(new char[] { ',' }, StringSplitOptions.None);
                            for (int i = 0; i < cols.Length; i++)
                            {
                                scols[cols[i].Trim()] = i;
                            }
                        }
                        else
                        {
                            System.Text.RegularExpressions.Regex zz = new System.Text.RegularExpressions.Regex("\"[^\"]*\"");
                            string convertString = zz.Replace(line, a => a.Value.Replace(",", "")).Replace("\"", "");

                            var vals = convertString.Replace("%", "").Split(new char[] { ',' }, StringSplitOptions.None);
                            var ntime = O2.O2DT(vals[scols["结束时间"]]);
                            if (dtime != ntime) continue;
                            var ln = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34}\r\n"
                                , zxcityconvert(vals[scols["子网"]])
                                , vals[scols["结束时间"]]
                                , vals[scols["eNodeB"]]
                                , vals[scols["eNodeB名称"]]
                                , vals[scols["小区"]]
                                , vals[scols["小区名称"]]
                                , "中兴"
                                , vals[scols["上行PRB平均占用率_1"]]
                                , vals[scols["下行PRB平均占用率_1"]]
                                , vals[scols["平均RRC连接用户数_1"]]
                                , vals[scols["附件4-PDCP层下行用户面流量 （MByte）_1496630023284-0-28"]]
                                , vals[scols["附件4-PDCP层上行用户面流量（MByte）_1496630023284-0-27"]]
                                , vals[scols["平均激活用户数_1"]]
                                , vals[scols["YY-RRC连接建立成功率_1496630023283-0-0"]]
                                , vals[scols["RRC连接建立失败次数（其它原因）（次）"]]
                                , vals[scols["[FDD]寻呼拥塞率"]]
                                , vals[scols["附件4- RRC连接重建比例 （%）"]]
                                , vals[scols["附件4-RRC连接重建请求次数"]]
                                , vals[scols["附件4-UE上下文掉线率（%）"]]
                                , vals[scols["附件4-UE上下文异常释放次数"]]
                                , vals[scols["E-RAB掉线率_1_1423038670971"]]
                                , vals[scols["E-RAB异常释放次数-FDD_1501553571415-0-10"]]
                                , vals[scols["附件4- RRC连接重建比例 （%）"]]
                                , vals[scols["附件4-RRC连接重建成功次数"]]
                                , vals[scols["CQI>=7占比-FDD_1511505655907-0-13"]]
                                , vals[scols["下行PRB双流占比"]]
                                , vals[scols["分QCI用户体验上行平均速率（Mbps）_1"]]
                                , vals[scols["分QCI用户体验下行平均速率（Mbps）_1"]]
                                , ""//10.1_小区退服时长（s）
                                , ""//10.3_小区可用率（%）
                                , ""//7.11_系统内切换成功率（%）
                                , vals[scols["E-RAB释放次数，由于ENB小区拥塞导致的释放"]]
                                , vals[scols["E-RAB释放次数，由于UE切换失败"]]
                                , vals[scols["4G切3G比例"]]
                                , vals[scols.ContainsKey("[LTE]LTE重定向到3G的尝试次数") ? scols["[LTE]LTE重定向到3G的尝试次数"] : scols["[LTE]LTE重定向到3G的尝试次数_CTCC"]]//7.27_LTE重定向到3G的尝试次数（次）
                                );
                            sbkpi.Append(ln);
                        }
                    }
                }
                //华为
                else if (file.IndexOf("LTEXN") >= 0)
                {
                    string upname = "8#14用户体验上行平均速率(Mbps）";
                    string downname = "8#14 用户体验下行平均速率(Mbps)";
                    if (file.IndexOf("LTEXN-229") >= 0)
                    {
                        upname = "用户体验上行平均速率";
                        downname = "用户体验下行平均速率";
                    }
                    if (file.IndexOf(".csv") >= 0) {
                        upname = "8.14用户体验上行平均速率(Mbps）";
                        downname = "8.14 用户体验下行平均速率(Mbps)";
                        if (file.IndexOf("LTEXN-229") >= 0)
                        {
                            upname = "用户体验上行平均速率";
                            downname = "用户体验下行平均速率";
                        }
                        var scols = new Dictionary<string, int>();
                        var lines = File.ReadLines(file, System.Text.Encoding.GetEncoding("gbk"));
                        var bhead = 0;
                        foreach (var line in lines)
                        {
                            if (bhead < 6) {
                                bhead++;
                            }
                            else if (bhead==6)
                            {
                                var cols = line.Split(new char[] { ',' }, StringSplitOptions.None);
                                for (int i = 0; i < cols.Length; i++)
                                {
                                    scols[cols[i].Trim()] = i;
                                }
                                bhead++;
                            }
                            else
                            {
                                System.Text.RegularExpressions.Regex zz = new System.Text.RegularExpressions.Regex("\"[^\"]*\"");
                                string convertString = zz.Replace(line, a => a.Value.Replace(",", "")).Replace("\"", "");

                                var vals = convertString.Replace("%", "").Split(new char[] { ',' }, StringSplitOptions.None);
                                if (vals.Length < 10) continue;
                                DateTime ntime = O2.O2DT(vals[scols["日期"]] + " " + vals[scols["时间"]]);
                                var city = hwcityconvert(vals[scols["eNodeB名称"]].ToString());
                                if (dtime != ntime) continue;
                                var ln = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34}\r\n"
                                        , city
                                        , ntime.ToString("yyyy-MM-dd HH:mm:ss")
                                        , convertZero(vals[scols["基站标识"]])
                                        , convertZero(vals[scols["eNodeB名称"]])
                                        , convertZero(vals[scols["小区ID"]])
                                        , convertZero(vals[scols["小区名称"]])
                                        , "华为"
                                        , convertZero(vals[scols["上行PRB平均利用率(%)"]])
                                        , convertZero(vals[scols["下行PRB平均利用率(%)"]])
                                        , convertZero(vals[scols["平均RRC连接用户数"]])
                                        , convertZero(vals[scols["PDCP层下行用户面流量（Mbyte）(兆字节)"]])
                                        , convertZero(vals[scols["PDCP层上行用户面流量（MByte）(兆字节)"]])
                                        , convertZero(vals[scols["平均激活用户数"]])
                                        , convertZero(vals[scols["RRC连接建立成功率(%)"]])
                                        , convertZero(vals[scols["RRC建立失败次数(其他原因)"]])
                                        , convertZero(vals[scols["寻呼拥塞率"]])
                                        , convertZero(vals[scols["2.36-RRC连接重建比例-2"]])
                                        , convertZero(vals[scols["RRC重建请求次数"]])
                                        , convertZero(vals[scols["UE上下文掉线率(%)"]])
                                        , convertZero(vals[scols["UE Context异常释放次数"]])
                                        , convertZero(vals[scols["E-RAB掉线率(%)"]])
                                        , convertZero(vals[scols["E-RAB异常释放次数"]])
                                        , convertZero(vals[scols["2.36-RRC连接重建比例-2"]])
                                        , convertZero(vals[scols["RRC连接建立成功次数"]])
                                        , convertZero(vals[scols["CQI优良比"]])
                                        , scols.ContainsKey("下行PRB双流占比") ? convertZero(vals[scols["下行PRB双流占比"]]) : ""
                                        , convertZero(vals[scols[upname]])
                                        , convertZero(vals[scols[downname]])
                                        , ""//10.1_小区退服时长（s）
                                        , ""//10.3_小区可用率（%）
                                        , ""//7.11_系统内切换成功率（%）
                                        , convertZero(vals[scols["无线网络拥塞导致的E-RAB异常释放次数"]])
                                        , convertZero(vals[scols["切换流程失败导致E-RAB异常释放次数"]])
                                        , convertZero(vals[scols["LTE重定向到3G比例新(%)"]])
                                        , scols.ContainsKey("4G重定向3G的次数") ? convertZero(vals[scols["4G重定向3G的次数"]]) : ""//7.27_LTE重定向到3G的尝试次数（次）
                                        );
                                sbkpi.Append(ln);
                            }
                        }
                    } else {
                        DataTable xmlDt = getDateFromXls(file);
                        
                        foreach (DataRow vals in xmlDt.Rows)
                        {

                            DateTime ntime = O2.O2DT(vals["日期"] + " " + vals["时间"]);
                            var city = hwcityconvert(vals["eNodeB名称"].ToString());
                            if (dtime != ntime) continue;
                            var ln = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34}\r\n"
                                    , city
                                    , ntime.ToString("yyyy-MM-dd HH:mm:ss")
                                    , convertZero(vals["基站标识"])
                                    , convertZero(vals["eNodeB名称"])
                                    , convertZero(vals["小区ID"])
                                    , convertZero(vals["小区名称"])
                                    , "华为"
                                    , convertZero(vals["上行PRB平均利用率(%)"])
                                    , convertZero(vals["下行PRB平均利用率(%)"])
                                    , convertZero(vals["平均RRC连接用户数"])
                                    , convertZero(vals["PDCP层下行用户面流量（Mbyte）(兆字节)"])
                                    , convertZero(vals["PDCP层上行用户面流量（MByte）(兆字节)"])
                                    , convertZero(vals["平均激活用户数"])
                                    , convertZero(vals["RRC连接建立成功率(%)"])
                                    , convertZero(vals["RRC建立失败次数(其他原因)"])
                                    , convertZero(vals["寻呼拥塞率"])
                                    , convertZero(vals["2#36-RRC连接重建比例-2"])
                                    , convertZero(vals["RRC重建请求次数"])
                                    , convertZero(vals["UE上下文掉线率(%)"])
                                    , convertZero(vals["UE Context异常释放次数"])
                                    , convertZero(vals["E-RAB掉线率(%)"])
                                    , convertZero(vals["E-RAB异常释放次数"])
                                    , convertZero(vals["2#36-RRC连接重建比例-2"])
                                    , convertZero(vals["RRC连接建立成功次数"])
                                    , convertZero(vals["CQI优良比"])
                                    , convertZero(vals["下行PRB双流占比"])
                                    , convertZero(vals[upname])
                                    , convertZero(vals[downname])
                                    , ""//10.1_小区退服时长（s）
                                    , ""//10.3_小区可用率（%）
                                    , ""//7.11_系统内切换成功率（%）
                                    , convertZero(vals["无线网络拥塞导致的E-RAB异常释放次数"])
                                    , convertZero(vals["切换流程失败导致E-RAB异常释放次数"])
                                    , convertZero(vals["LTE重定向到3G比例新(%)"])
                                    , convertZero(vals["4G重定向3G的次数"])//7.27_LTE重定向到3G的尝试次数（次）
                                    );
                            sbkpi.Append(ln);
                        }
                    }
                    
                    
                }
            }
            File.WriteAllText(Path.Combine(cfg.custom["zjpath"], "SH_PERF_CELL_L_" + dtime.ToString("yyyyMMddHH") + ".csv"), sbkpi.ToString(), Encoding.GetEncoding("gbk"));
            return null;
        }
        private object convertZero(object o) {
            return o.Equals("/0") ? "" : o;
        }
        public override bool ImportDB(DataTable dt, bool bol)
        {
            return true;
        }
        public override void DeleteTmp()
        {
            Csv[] csvs = cfg.csvs.ToArray();
            for (int i = 0; i < csvs.Length; i++)
            {
                foreach (var file in Directory.GetFiles(csvs[i].filetmp.path))
                {
                    //string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                    //File.Copy(file, Path.Combine(csvs[i].filetarget.path, fileName), true);
                    File.Delete(file);
                    //File.Move(file, Path.Combine(csvs[i].filetarget.path,fileName));
                }
            }
            File.WriteAllText(Path.Combine(cfg.csvs[0].filetarget.path, "SH_PERF_CELL_L_" + TimeStr[0] + ".csv"),"");
        }
        private string nkcityconvert(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return city;
            city = city.ToLower();
            switch (city)
            {
                case "baoding":
                    city = "保定";
                    break;
                case "tangshan":
                    city = "唐山";
                    break;
                case "xingtai":
                    city = "邢台";
                    break;
                case "zhangjiakou":
                    city = "张家口";
                    break;
            }
            return city;
        }
        private string hwcityconvert(string city)
        {
            if (string.IsNullOrWhiteSpace(city)||city.Length < 3) return "";
            city = city.Substring(0,3).ToLower();
            if (city.Contains("bd"))
            {
                city = "保定";
            }
            else if (city.Contains("xt"))
            {
                city = "邢台";
            }
            else if (city.Contains("qhd"))
            {
                city = "秦皇岛";
            }
            else if (city.Contains("hd"))
            {
                city = "邯郸";
            }
            else if (city.Contains("sjz"))
            {
                city = "石家庄";
            }
            else if (city.Contains("cd"))
            {
                city = "承德";
            }
            else if (city.Contains("lf"))
            {
                city = "廊坊";
            }
            else if (city.Contains("ts"))
            {
                city = "唐山";
            }
            else if (city.Contains("hs"))
            {
                city = "衡水";
            }
            else if (city.Contains("xa"))
            {
                city = "雄安";
            }
            else if (city.Contains("cz"))
            {
                city = "沧州";
            }
            else if (city.Contains("zjk"))
            {
                city = "张家口";
            }
            else
            {
                city = "";
            }
            return city;
        }
        private string zxcityconvert(string city)
        {
            if (city == null || city.Length < 6) return city;
            city = city.Substring(2, 2);
            switch (city)
            {
                case "01":
                    city = "石家庄";
                    break;
                case "06":
                    city = "保定";
                    break;
                case "07":
                    city = "张家口";
                    break;
                case "08":
                    city = "承德";
                    break;
                case "12":
                    city = "雄安";
                    break;
            }
            return city;
        }

        public DataTable getDateFromXls(string filename)
        {
            var constr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filename + ";" + "Extended Properties=Excel 12.0;";
            OleDbConnection con = new OleDbConnection(constr);
            con.Open();
            var tb = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            con.Close();
            var tbn = tb.Rows[0]["TABLE_NAME"].ToString();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter("select * from [" + tbn + "]", con);

            da.Fill(dt);
            return dt;

        }
    }
}
