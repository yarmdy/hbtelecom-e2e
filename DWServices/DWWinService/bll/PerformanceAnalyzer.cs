using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;

namespace DWWinService
{
    class PerformanceAnalyzer : Analyzer
    {
        public override void DoQuarter()
        {
            //do nothing
        }
        public override void AfterDownloadCsv()
        {
            //下载后解压和转换为csv删除rar文件
            var tmppath = cfg.csvs[0].filetmp.path;
            var tmpfiles = Directory.GetFiles(tmppath, "*.rar");
            var pathExe = AppDomain.CurrentDomain.BaseDirectory + "WinRAR.exe";
            try
            {
                bool hasDone = false;
                for (var i = 0; i < tmpfiles.Length; i++)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = pathExe;
                    p.StartInfo.WorkingDirectory = tmppath;
                    string cmd = string.Format("x -dr {0} {1} -y", tmpfiles[i], tmppath);
                    p.StartInfo.Arguments = cmd;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.Start();
                    p.WaitForExit();
                    if (i == tmpfiles.Length - 1 && p.HasExited)
                    {
                        hasDone = true;
                    }
                }
                if (hasDone)
                {
                    string[] xmls = Directory.GetFiles(tmppath, "*.xml");
                    string target = cfg.csvs[0].filetarget.path;
                    DateTime dtime = DateTime.Now.AddDays(-1);
                    string newdir = target + dtime.ToString("yyyyMM") + @"\";
                    if (!Directory.Exists(newdir))
                    {
                        Directory.CreateDirectory(newdir);
                    }
                    for (var i = 0; i < xmls.Length; i++)
                    {
                        try
                        {
                            Convert(xmls[i], newdir);
                        }
                        catch { }
                    }
                    var allfiles = Directory.GetFiles(tmppath);
                    for (var i = 0; i < allfiles.Length; i++)
                    {
                        File.Delete(allfiles[i]);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Convert(string file, string target)
        {
            XDocument xd = XDocument.Load(file);
            var nodes = xd.Descendants("Message");
            DataTable dt = new DataTable();
            foreach (var node in nodes)
            {
                var row = dt.NewRow();
                foreach (var n1 in node.Elements())
                {
                    foreach (var n2 in n1.Elements())
                    {
                        var name = n1.Name.ToString() + "_" + n2.Name.ToString();
                        var val = n2.Value;
                        if (!dt.Columns.Contains(name))
                        {
                            dt.Columns.Add(name);
                        }
                        row[name] = val;
                    }
                }
                dt.Rows.Add(row);
            }
            var csvstr = Xml2Csv(dt);
            string[] ps = file.Split('\\');
            string filename = ps[ps.Length - 1].Split('.')[0];

            File.WriteAllText(target + filename + ".csv", csvstr, Encoding.GetEncoding("gbk"));
        }
        private string Xml2Csv(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn col in dt.Columns)
            {
                sb.Append(col.ColumnName + ",");
            }
            sb.Append("\r\n");
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(dr[i] + ",");
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }


        public override string[] DownloadCsv(string time)
        {
            while (true)
            {
                string downloadurl = cfg.custom["downloadurl"];
                DateTime dtime = DateTime.Now.AddDays(-1);
                string prefix = "河北_" + dtime.ToString("yyyyMMdd");
                string[] filenames = {
                prefix+"_group_02-01.rar",
                prefix + "_group_03-01.rar",
                prefix + "_group_04-01.rar",
                prefix + "_group_05-01.rar",
                prefix + "_group_cmcc_02.rar",
                prefix + "_group_cmcc_03.rar",
                prefix + "_group_cmcc_04.rar",
                prefix + "_group_cmcc_05.rar",
                prefix+ "_group_unicom_02.rar",
                prefix + "_group_unicom_03.rar",
                prefix+ "_group_unicom_04.rar",
                prefix + "_group_unicom_05.rar",

                prefix + "_group_06-01.rar",
                prefix + "_group_cmcc_06.rar",
                prefix + "_group_unicom_06.rar",
            };
                int errc = 0;
                FileStream fs = null;
                for (int i = 0; i < filenames.Length; i++)
                {
                    try
                    {
                        string filename = filenames[i];
                        string tempPath = cfg.csvs[0].filetmp.path;
                        string tempFile = tempPath + filename + ".temp"; //临时文件
                        if (System.IO.File.Exists(tempFile))
                        {
                            System.IO.File.Delete(tempFile);    //存在则删除
                        }
                        string url = downloadurl + dtime.ToString("yyyyMM") + @"/" + filename;
                        //string url = @"http://192.168.1.104/hbyssj/201802/河北_20180223_group_02-01.rar";
                        fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        // 设置参数
                        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                        //发送请求并获取相应回应数据
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        //直到request.GetResponse()程序才开始向目标网页发送Post请求
                        Stream responseStream = response.GetResponseStream();
                        //创建本地文件写入流
                        byte[] bArr = new byte[1024];
                        int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        while (size > 0)
                        {
                            fs.Write(bArr, 0, size);
                            size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        }
                        fs.Close();
                        fs = null;
                        responseStream.Close();
                        System.IO.File.Move(tempFile, tempPath + filename);
                    }
                    catch (Exception ex) {
                        if (fs != null) {
                            fs.Close();
                            fs.Dispose();
                        }
                        fs = null;
                        errc++;
                    }
                }
                if (errc >= 10) {
                    continue;
                }
                return filenames;

            }
        }

        private string GetEvtName(string code) {
            var res = "";
            switch (code) { 
                case "5008":
                    res = "弱覆盖事件";
                    break;
                case "5009":
                    res = "无覆盖事件";
                    break;
                case "2002":
                    res = "数据掉线事件";
                    break;
                case "2005":
                    res = "数据连接建立失败事件";
                    break;
                case "5021":
                    res = "4G回落3G事件";
                    break;
                case "5020":
                    res = "4G回落2G事件";
                    break;
                case "5023":
                    res = "网络频繁切换事件";
                    break;
                default:
                    res = "其他事件";
                    break;
            }
            return res;
        }
        public override DataTable Analysis(bool bQuarter, string[] files)
        {
            //分析方法必须重写，包括分析采集、筛选和统计
            var dtyue = DateTime.Now.Date.AddDays(-1);
            var dttt = dtyue;
            dtyue = dtyue.AddDays(-dtyue.Day + 1);
            #region 获取要分析的文件
            var tarpath = cfg.csvs[0].filetarget.path + dttt.ToString("yyyyMM") + "\\";
            var tfiles = System.IO.Directory.GetFiles(tarpath, "*.csv");
            var webfiles = tfiles.Where(a => a.IndexOf("_02") > 0).ToArray();
            var videofiles = tfiles.Where(a => a.IndexOf("_03") > 0).ToArray();
            var imfiles = tfiles.Where(a => a.IndexOf("_04") > 0).ToArray();
            var gamefiles = tfiles.Where(a => a.IndexOf("_05") > 0).ToArray();

            var evtfiles = tfiles.Where(a => a.IndexOf("_06") > 0).ToArray();
            #endregion
            #region 创建字典
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> dicsweb = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            Dictionary<string, Dictionary<string, object>> dicswebctcc = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> dicswebcmcc = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> dicswebcucc = new Dictionary<string, Dictionary<string, object>>();
            dicsweb.Add("ctcc", dicswebctcc);
            dicsweb.Add("cmcc", dicswebcmcc);
            dicsweb.Add("cucc", dicswebcucc);

            Dictionary<string, Dictionary<string, Dictionary<string, object>>> dicsvideo = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            Dictionary<string, Dictionary<string, object>> dicsvideoctcc = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> dicsvideocmcc = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> dicsvideocucc = new Dictionary<string, Dictionary<string, object>>();
            dicsvideo.Add("ctcc", dicsvideoctcc);
            dicsvideo.Add("cmcc", dicsvideocmcc);
            dicsvideo.Add("cucc", dicsvideocucc);

            Dictionary<string, Dictionary<string, Dictionary<string, object>>> dicsim = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            Dictionary<string, Dictionary<string, object>> dicsimctcc = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> dicsimcmcc = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> dicsimcucc = new Dictionary<string, Dictionary<string, object>>();
            dicsim.Add("ctcc", dicsimctcc);
            dicsim.Add("cmcc", dicsimcmcc);
            dicsim.Add("cucc", dicsimcucc);

            Dictionary<string, Dictionary<string, Dictionary<string, object>>> dicsgame = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            Dictionary<string, Dictionary<string, object>> dicsgamectcc = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> dicsgamecmcc = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> dicsgamecucc = new Dictionary<string, Dictionary<string, object>>();
            dicsgame.Add("ctcc", dicsgamectcc);
            dicsgame.Add("cmcc", dicsgamecmcc);
            dicsgame.Add("cucc", dicsgamecucc);

            Dictionary<string, Dictionary<string, Dictionary<string, long>>> dicsevt = new Dictionary<string, Dictionary<string, Dictionary<string, long>>>();
            Dictionary<string, Dictionary<string, long>> dicsevtctcc = new Dictionary<string, Dictionary<string, long>>();
            Dictionary<string, Dictionary<string, long>> dicsevtcmcc = new Dictionary<string, Dictionary<string, long>>();
            Dictionary<string, Dictionary<string, long>> dicsevtcucc = new Dictionary<string, Dictionary<string, long>>();
            dicsevt.Add("ctcc", dicsevtctcc);
            dicsevt.Add("cmcc", dicsevtcmcc);
            dicsevt.Add("cucc", dicsevtcucc);
            #endregion
            #region 读取配置
            var webtar = cfg.custom["webtar"].Split(new char[] { '|' }).ToList();

            var webtartop = cfg.custom["webtartop"].Split(new char[] { '|' }).ToList();

            var videotar = cfg.custom["videotar"].Split(new char[] { '|' }).ToList();
            var imtar = cfg.custom["imtar"].Split(new char[] { '|' }).ToList();
            var gametar = cfg.custom["gametar"].Split(new char[] { '|' }).ToList();
            var nettype = cfg.custom["nettype"].Split(new char[] { '|' }).ToList();
            var webopen = O2.O2I(cfg.custom["webopen"].Split(new char[] { '|' }).ToList()[0].Replace(">", ""));

            var webopenhttps = O2.O2I(cfg.custom["webopen"].Split(new char[] { '|' }).ToList()[1].Replace(">", ""));

            var firstdelay = O2.O2I(cfg.custom["firstdelay"].Replace(">", ""));
            var videodown = O2.O2I(cfg.custom["videodown"].Split(new char[] { '|' }).ToList()[0].Replace("<", ""));

            var videodown800m = O2.O2I(cfg.custom["videodown"].Split(new char[] { '|' }).ToList()[1].Replace("<", ""));

            var videorate = O2.O2D(cfg.custom["videorate"].Replace(">", ""));
            var gamedelay = O2.O2I(cfg.custom["gamedelay"].Replace(">", ""));
            var webcount = O2.O2I(cfg.custom["webcount"]);
            var videocount = O2.O2I(cfg.custom["videocount"]);
            var imcount = O2.O2I(cfg.custom["imcount"]);
            var gamecount = O2.O2I(cfg.custom["gamecount"]);
            var exportpath = cfg.custom["exportpath"];

            var wugao = readwugaoDic();
            #endregion
            #region 读入字典并筛选数据
            //web分析
            foreach (var file in webfiles)
            {
                Dictionary<string, Dictionary<string, object>> dics = dicsweb["ctcc"];
                var key = "ctcc";
                if (file.IndexOf("cmcc") > 0)
                {
                    dics = dicsweb["cmcc"];
                    key = "cmcc";
                }
                if (file.IndexOf("unicom") > 0)
                {
                    dics = dicsweb["cucc"];
                    key = "ctcc";
                }
                var filestr = File.ReadAllText(file, Encoding.GetEncoding("gbk"));
                var filelines = filestr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var firstline = true;
                int PhoneInfo_MEID = 0, PositionInfo_City = 0, NetInfo_NetType = 0, NetInfo_LteCi = 0,
                    TestResult_WebsiteName = 0, TestResult_PageURL = 0, TestResult_PageSurfTime = 0,
                    TestResult_FirstByteDelay = 0, TestResult_PageOpenDelay = 0, TestResult_RRCSetupDelay = 0,
                    TestResult_DnsDelay = 0, TestResult_ConnDelay = 0, TestResult_ReqDelay = 0, TestResult_ResDelay = 0,
                    TestResult_TCLASS = 0, TestResult_Success = 0, TestResult_PageAvgSpeed = 0, TestResult_FirstScreenDelay = 0;
                foreach (var line in filelines)
                {
                    if (firstline)
                    {
                        firstline = false;
                        var cols = line.Split(new char[] { ',' }).ToList();
                        PhoneInfo_MEID = cols.IndexOf("PhoneInfo_MEID");
                        PositionInfo_City = cols.IndexOf("PositionInfo_City");
                        NetInfo_NetType = cols.IndexOf("NetInfo_NetType");
                        NetInfo_LteCi = cols.IndexOf("NetInfo_LteCi");

                        TestResult_WebsiteName = cols.IndexOf("TestResult_WebsiteName");
                        TestResult_PageURL = cols.IndexOf("TestResult_PageURL");
                        TestResult_PageSurfTime = cols.IndexOf("TestResult_PageSurfTime");

                        TestResult_FirstByteDelay = cols.IndexOf("TestResult_FirstByteDelay");
                        TestResult_PageOpenDelay = cols.IndexOf("TestResult_PageOpenDelay");
                        TestResult_RRCSetupDelay = cols.IndexOf("TestResult_RRCSetupDelay");

                        TestResult_DnsDelay = cols.IndexOf("TestResult_DnsDelay");
                        TestResult_ConnDelay = cols.IndexOf("TestResult_ConnDelay");
                        TestResult_ReqDelay = cols.IndexOf("TestResult_ReqDelay");
                        TestResult_ResDelay = cols.IndexOf("TestResult_ResDelay");

                        TestResult_TCLASS = cols.IndexOf("TestResult_TCLASS");
                        TestResult_Success = cols.IndexOf("TestResult_Success");
                        TestResult_PageAvgSpeed = cols.IndexOf("TestResult_PageAvgSpeed");
                        TestResult_FirstScreenDelay = cols.IndexOf("TestResult_FirstScreenDelay");
                    }
                    else
                    {
                        var cels = line.Split(new char[] { ',' }).ToList();
                        if (!string.IsNullOrWhiteSpace(cels[PositionInfo_City]) && !string.IsNullOrWhiteSpace(cels[TestResult_PageSurfTime]) && DateTime.Parse(cels[TestResult_PageSurfTime]).ToString("yyyyMM") == dttt.ToString("yyyyMM") && !string.IsNullOrWhiteSpace(cels[NetInfo_LteCi]))
                        {
                            if (!dicsevt[key].ContainsKey(cels[PositionInfo_City])) {
                                dicsevt[key][cels[PositionInfo_City]] = new Dictionary<string, long>();
                                dicsevt[key][cels[PositionInfo_City]]["web"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game"] = 0;

                                dicsevt[key][cels[PositionInfo_City]]["web_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["web_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game_fwg"] = 0;
                            }
                            dicsevt[key][cels[PositionInfo_City]]["web"]++;
                            var ltecit=O2.O2I(cels[NetInfo_LteCi]);
                            if (wugao.ContainsKey(ltecit))
                            {
                                dicsevt[key][cels[PositionInfo_City]]["web_wg"]++;
                            }
                            else {
                                dicsevt[key][cels[PositionInfo_City]]["web_fwg"]++;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(cels[PhoneInfo_MEID]) ||
                            string.IsNullOrWhiteSpace(cels[PositionInfo_City]) ||
                            nettype.IndexOf(cels[NetInfo_NetType]) >= 0 ||
                            string.IsNullOrWhiteSpace(cels[NetInfo_LteCi]) ||
                            webtar.IndexOf(cels[TestResult_PageURL].Trim()) < 0 ||
                            string.IsNullOrWhiteSpace(cels[TestResult_PageSurfTime]) ||

                            DateTime.Parse(cels[TestResult_PageSurfTime]).ToString("yyyyMM") != dttt.ToString("yyyyMM") ||

                            cels[TestResult_TCLASS].Trim() != "1" ||
                            cels[TestResult_Success].Trim() != "1" ||
                            dics.ContainsKey(cels[PhoneInfo_MEID] + "_" + cels[TestResult_PageSurfTime])

                            //剔除
                            || cels[TestResult_FirstScreenDelay].Trim() == "" || O2.O2I(cels[TestResult_FirstScreenDelay])<0
                            )
                        {
                            continue;
                        }
                        var dic = new Dictionary<string, object>();
                        if (O2.O2I(cels[TestResult_FirstScreenDelay]) > 30000) {
                            cels[TestResult_FirstScreenDelay] = "30000";
                        }
                        dic["PhoneInfo_MEID"] = cels[PhoneInfo_MEID];
                        dic["PositionInfo_City"] = cels[PositionInfo_City];
                        dic["NetInfo_NetType"] = cels[NetInfo_NetType];
                        dic["NetInfo_LteCi"] = cels[NetInfo_LteCi];

                        dic["TestResult_WebsiteName"] = cels[TestResult_WebsiteName];
                        dic["TestResult_PageURL"] = cels[TestResult_PageURL];
                        dic["TestResult_PageSurfTime"] = cels[TestResult_PageSurfTime];

                        dic["TestResult_FirstByteDelay"] = cels[TestResult_FirstByteDelay];
                        dic["TestResult_PageOpenDelay"] = cels[TestResult_PageOpenDelay];
                        dic["TestResult_RRCSetupDelay"] = cels[TestResult_RRCSetupDelay];

                        dic["TestResult_DnsDelay"] = cels[TestResult_DnsDelay];
                        dic["TestResult_ConnDelay"] = cels[TestResult_ConnDelay];
                        dic["TestResult_ReqDelay"] = cels[TestResult_ReqDelay];
                        dic["TestResult_ResDelay"] = cels[TestResult_ResDelay];

                        dic["TestResult_TCLASS"] = cels[TestResult_TCLASS];
                        dic["TestResult_Success"] = cels[TestResult_Success];
                        dic["TestResult_PageAvgSpeed"] = cels[TestResult_PageAvgSpeed];
                        dic["TestResult_FirstScreenDelay"] = cels[TestResult_FirstScreenDelay];

                        dic["ISZHICHA1"] = (O2.O2I(cels[TestResult_PageOpenDelay]) > webopen) ? "1" : "0";
                        if (cels[TestResult_PageURL].IndexOf("baidu") >= 0 || cels[TestResult_PageURL].IndexOf("taobao") >= 0)
                        {
                            dic["ISZHICHA1"] = (O2.O2I(cels[TestResult_PageOpenDelay]) > webopenhttps) ? "1" : "0";
                        }
                        
                        dic["ISZHICHA2"] = "0";
                        if (webtartop.IndexOf(cels[TestResult_PageURL]) >= 0) {
                            dic["ISZHICHA2"] = (O2.O2I(cels[TestResult_FirstScreenDelay]) > firstdelay) ? "1" : "0";
                        }

                        var lteci = O2.O2I(dic["NetInfo_LteCi"]);
                        dic["wugao"] = wugao.ContainsKey(lteci) ? 1 : 0;

                        dics[cels[PhoneInfo_MEID] + "_" + cels[TestResult_PageSurfTime]] = dic;
                    }
                }
            }

            //video分析
            foreach (var file in videofiles)
            {
                Dictionary<string, Dictionary<string, object>> dics = dicsvideo["ctcc"];
                var key = "ctcc";
                if (file.IndexOf("cmcc") > 0)
                {
                    dics = dicsvideo["cmcc"];
                    key = "cmcc";
                }
                if (file.IndexOf("unicom") > 0)
                {
                    dics = dicsvideo["cucc"];
                    key = "cucc";
                }
                var filestr = File.ReadAllText(file, Encoding.GetEncoding("gbk"));
                var filelines = filestr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var firstline = true;
                int PhoneInfo_MEID = 0, PositionInfo_City = 0, NetInfo_NetType = 0, NetInfo_LteCi = 0,
                    TestResult_VideoName = 0, TestResult_VideoURL = 0, TestResult_VideoTestTime = 0,
                    TestResult_VideoAvgSpeed = 0, TestResult_VideoPeakSpeed = 0, TestResult_TCLASS = 0, TestResult_VideoTotleTraffic = 0,
                    TestResult_CacheRate = 0;
                foreach (var line in filelines)
                {
                    if (firstline)
                    {
                        firstline = false;
                        var cols = line.Split(new char[] { ',' }).ToList();
                        PhoneInfo_MEID = cols.IndexOf("PhoneInfo_MEID");
                        PositionInfo_City = cols.IndexOf("PositionInfo_City");
                        NetInfo_NetType = cols.IndexOf("NetInfo_NetType");
                        NetInfo_LteCi = cols.IndexOf("NetInfo_LteCi");

                        TestResult_VideoName = cols.IndexOf("TestResult_VideoName");
                        TestResult_VideoURL = cols.IndexOf("TestResult_VideoURL");
                        TestResult_VideoTestTime = cols.IndexOf("TestResult_VideoTestTime");

                        TestResult_VideoAvgSpeed = cols.IndexOf("TestResult_VideoAvgSpeed");
                        TestResult_VideoPeakSpeed = cols.IndexOf("TestResult_VideoPeakSpeed");
                        TestResult_TCLASS = cols.IndexOf("TestResult_TCLASS");

                        TestResult_VideoTotleTraffic = cols.IndexOf("TestResult_VideoTotleTraffic");
                        TestResult_CacheRate = cols.IndexOf("TestResult_CacheRate");
                    }
                    else
                    {
                        var cels = line.Split(new char[] { ',' }).ToList();
                        if (!string.IsNullOrWhiteSpace(cels[PositionInfo_City]) && !string.IsNullOrWhiteSpace(cels[TestResult_VideoTestTime]) && DateTime.Parse(cels[TestResult_VideoTestTime]).ToString("yyyyMM") == dttt.ToString("yyyyMM") && !string.IsNullOrWhiteSpace(cels[NetInfo_LteCi]))
                        {
                            if (!dicsevt[key].ContainsKey(cels[PositionInfo_City]))
                            {
                                dicsevt[key][cels[PositionInfo_City]] = new Dictionary<string, long>();
                                dicsevt[key][cels[PositionInfo_City]]["web"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game"] = 0;

                                dicsevt[key][cels[PositionInfo_City]]["web_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["web_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game_fwg"] = 0;
                            }
                            dicsevt[key][cels[PositionInfo_City]]["video"]++;

                            var ltecit = O2.O2I(cels[NetInfo_LteCi]);
                            if (wugao.ContainsKey(ltecit))
                            {
                                dicsevt[key][cels[PositionInfo_City]]["video_wg"]++;
                            }
                            else
                            {
                                dicsevt[key][cels[PositionInfo_City]]["video_fwg"]++;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(cels[PhoneInfo_MEID]) ||
                            string.IsNullOrWhiteSpace(cels[PositionInfo_City]) ||
                            nettype.IndexOf(cels[NetInfo_NetType]) >= 0 ||
                            string.IsNullOrWhiteSpace(cels[NetInfo_LteCi]) ||
                            videotar.IndexOf(cels[TestResult_VideoName].Trim()) < 0 ||
                            string.IsNullOrWhiteSpace(cels[TestResult_VideoTestTime]) ||

                            DateTime.Parse(cels[TestResult_VideoTestTime]).ToString("yyyyMM") != dttt.ToString("yyyyMM") ||

                            cels[TestResult_TCLASS].Trim() != "1" ||
                            O2.O2D(cels[TestResult_VideoAvgSpeed]) <= 0 ||
                            O2.O2D(cels[TestResult_VideoPeakSpeed]) <= 0 ||
                            dics.ContainsKey(cels[PhoneInfo_MEID] + "_" + cels[TestResult_VideoTestTime])

                            //剔除
                            || O2.O2D(cels[TestResult_VideoPeakSpeed]) <= 0 || O2.O2D(cels[TestResult_VideoAvgSpeed]) <= 0
                            || O2.O2D(cels[TestResult_VideoPeakSpeed]) > 307200 || O2.O2D(cels[TestResult_VideoAvgSpeed]) > 307200
                            || cels[TestResult_CacheRate].Trim()=="" || O2.O2D(cels[TestResult_VideoTotleTraffic])<500
                            )
                        {
                            continue;
                        }
                        var dic = new Dictionary<string, object>();
                        dic["PhoneInfo_MEID"] = cels[PhoneInfo_MEID];
                        dic["PositionInfo_City"] = cels[PositionInfo_City];
                        dic["NetInfo_NetType"] = cels[NetInfo_NetType];
                        dic["NetInfo_LteCi"] = cels[NetInfo_LteCi];

                        dic["TestResult_VideoName"] = cels[TestResult_VideoName];
                        dic["TestResult_VideoURL"] = cels[TestResult_VideoURL];
                        dic["TestResult_VideoTestTime"] = cels[TestResult_VideoTestTime];

                        dic["TestResult_VideoAvgSpeed"] = cels[TestResult_VideoAvgSpeed];
                        dic["TestResult_VideoPeakSpeed"] = cels[TestResult_VideoPeakSpeed];
                        dic["TestResult_TCLASS"] = cels[TestResult_TCLASS];

                        dic["TestResult_VideoTotleTraffic"] = cels[TestResult_VideoTotleTraffic];
                        dic["TestResult_CacheRate"] = cels[TestResult_CacheRate];

                        dic["ISZHICHA1"] = (O2.O2D(cels[TestResult_VideoAvgSpeed]) < videodown) ? "1" : "0";
                        var cell1 = O2.O2I(cels[NetInfo_LteCi]) % 256 / 16;

                        if (cell1 == 1 || cell1 == 9) {
                            dic["ISZHICHA1"] = (O2.O2D(cels[TestResult_VideoAvgSpeed]) < videodown800m) ? "1" : "0";
                        }

                        

                        dic["ISZHICHA2"] = (O2.O2D(cels[TestResult_CacheRate]) > videorate) ? "1" : "0";

                        var lteci = O2.O2I(dic["NetInfo_LteCi"]);
                        dic["wugao"] = wugao.ContainsKey(lteci) ? 1 : 0;

                        dics[cels[PhoneInfo_MEID] + "_" + cels[TestResult_VideoTestTime]] = dic;
                    }
                }
            }

            //im分析
            foreach (var file in imfiles)
            {
                Dictionary<string, Dictionary<string, object>> dics = dicsim["ctcc"];
                var key = "ctcc";
                if (file.IndexOf("cmcc") > 0)
                {
                    dics = dicsim["cmcc"];
                    key = "cmcc";
                }
                if (file.IndexOf("unicom") > 0)
                {
                    dics = dicsim["cucc"];
                    key = "cucc";
                }
                var filestr = File.ReadAllText(file, Encoding.GetEncoding("gbk"));
                var filelines = filestr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var firstline = true;
                int PhoneInfo_MEID = 0, PositionInfo_City = 0, NetInfo_NetType = 0, NetInfo_LteCi = 0,
                    TestResult_ImName = 0, TestResult_ImTestTime = 0,
                    TestResult_ImSendCount = 0, TestResult_ImSendRate = 0, TestResult_ImSendDelay = 0, TestResult_TCLASS = 0;
                foreach (var line in filelines)
                {
                    if (firstline)
                    {
                        firstline = false;
                        var cols = line.Split(new char[] { ',' }).ToList();
                        PhoneInfo_MEID = cols.IndexOf("PhoneInfo_MEID");
                        PositionInfo_City = cols.IndexOf("PositionInfo_City");
                        NetInfo_NetType = cols.IndexOf("NetInfo_NetType");
                        NetInfo_LteCi = cols.IndexOf("NetInfo_LteCi");

                        TestResult_ImName = cols.IndexOf("TestResult_ImName");
                        TestResult_ImTestTime = cols.IndexOf("TestResult_ImTestTime");

                        TestResult_ImSendCount = cols.IndexOf("TestResult_ImSendCount");
                        TestResult_ImSendRate = cols.IndexOf("TestResult_ImSendRate");
                        TestResult_ImSendDelay = cols.IndexOf("TestResult_ImSendDelay");

                        TestResult_TCLASS = cols.IndexOf("TestResult_TCLASS");
                    }
                    else
                    {
                        var cels = line.Split(new char[] { ',' }).ToList();
                        if (!string.IsNullOrWhiteSpace(cels[PositionInfo_City]) && !string.IsNullOrWhiteSpace(cels[TestResult_ImTestTime]) && DateTime.Parse(cels[TestResult_ImTestTime]).ToString("yyyyMM") == dttt.ToString("yyyyMM") && !string.IsNullOrWhiteSpace(cels[NetInfo_LteCi]))
                        {
                            if (!dicsevt[key].ContainsKey(cels[PositionInfo_City]))
                            {
                                dicsevt[key][cels[PositionInfo_City]] = new Dictionary<string, long>();
                                dicsevt[key][cels[PositionInfo_City]]["web"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game"] = 0;

                                dicsevt[key][cels[PositionInfo_City]]["web_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["web_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game_fwg"] = 0;
                            }
                            dicsevt[key][cels[PositionInfo_City]]["im"]++;
                            var ltecit = O2.O2I(cels[NetInfo_LteCi]);
                            if (wugao.ContainsKey(ltecit))
                            {
                                dicsevt[key][cels[PositionInfo_City]]["im_wg"]++;
                            }
                            else
                            {
                                dicsevt[key][cels[PositionInfo_City]]["im_fwg"]++;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(cels[PhoneInfo_MEID]) ||
                            string.IsNullOrWhiteSpace(cels[PositionInfo_City]) ||
                            nettype.IndexOf(cels[NetInfo_NetType]) >= 0 ||
                            string.IsNullOrWhiteSpace(cels[NetInfo_LteCi]) ||
                            imtar.IndexOf(cels[TestResult_ImName].Trim()) < 0 ||
                            string.IsNullOrWhiteSpace(cels[TestResult_ImTestTime]) ||

                            DateTime.Parse(cels[TestResult_ImTestTime]).ToString("yyyyMM") != dttt.ToString("yyyyMM") ||

                            cels[TestResult_TCLASS].Trim() != "1" ||
                            dics.ContainsKey(cels[PhoneInfo_MEID] + "_" + cels[TestResult_ImTestTime])

                            //剔除
                            || cels[TestResult_ImSendRate].Trim() == "" || O2.O2D(cels[TestResult_ImSendRate]) < 0 || O2.O2D(cels[TestResult_ImSendRate])>100
                            )
                        {
                            continue;
                        }
                        var dic = new Dictionary<string, object>();
                        dic["PhoneInfo_MEID"] = cels[PhoneInfo_MEID];
                        dic["PositionInfo_City"] = cels[PositionInfo_City];
                        dic["NetInfo_NetType"] = cels[NetInfo_NetType];
                        dic["NetInfo_LteCi"] = cels[NetInfo_LteCi];

                        dic["TestResult_ImName"] = cels[TestResult_ImName];
                        dic["TestResult_ImTestTime"] = cels[TestResult_ImTestTime];

                        dic["TestResult_ImSendCount"] = cels[TestResult_ImSendCount];
                        dic["TestResult_ImSendRate"] = O2.O2D(cels[TestResult_ImSendRate]) / 100;
                        dic["TestResult_ImSendDelay"] = cels[TestResult_ImSendDelay];

                        dic["TestResult_TCLASS"] = cels[TestResult_TCLASS];

                        dic["ISZHICHA1"] = "0";
                        dic["ISZHICHA2"] = "1";

                        var lteci = O2.O2I(dic["NetInfo_LteCi"]);
                        dic["wugao"] = wugao.ContainsKey(lteci) ? 1 : 0;

                        dics[cels[PhoneInfo_MEID] + "_" + cels[TestResult_ImTestTime]] = dic;
                    }
                }
            }

            //game分析
            foreach (var file in gamefiles)
            {
                Dictionary<string, Dictionary<string, object>> dics = dicsgame["ctcc"];
                var key = "ctcc";
                if (file.IndexOf("cmcc") > 0)
                {
                    dics = dicsgame["cmcc"];
                    key = "cmcc";
                }
                if (file.IndexOf("unicom") > 0)
                {
                    dics = dicsgame["cucc"];
                    key = "cucc";
                }
                var filestr = File.ReadAllText(file, Encoding.GetEncoding("gbk"));
                var filelines = filestr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var firstline = true;
                int PhoneInfo_MEID = 0, PositionInfo_City = 0, NetInfo_NetType = 0, NetInfo_LteCi = 0,
                    TestResult_gameName = 0, TestResult_gameTestTime = 0,
                    TestResult_AckDelay = 0, TestResult_TCLASS = 0;
                foreach (var line in filelines)
                {
                    if (firstline)
                    {
                        firstline = false;
                        var cols = line.Split(new char[] { ',' }).ToList();
                        PhoneInfo_MEID = cols.IndexOf("PhoneInfo_MEID");
                        PositionInfo_City = cols.IndexOf("PositionInfo_City");
                        NetInfo_NetType = cols.IndexOf("NetInfo_NetType");
                        NetInfo_LteCi = cols.IndexOf("NetInfo_LteCi");

                        TestResult_gameName = cols.IndexOf("TestResult_gameName");
                        TestResult_gameTestTime = cols.IndexOf("TestResult_gameTestTime");

                        TestResult_AckDelay = cols.IndexOf("TestResult_AckDelay");

                        TestResult_TCLASS = cols.IndexOf("TestResult_TCLASS");
                    }
                    else
                    {
                        var cels = line.Split(new char[] { ',' }).ToList();
                        if (!string.IsNullOrWhiteSpace(cels[PositionInfo_City]) && !string.IsNullOrWhiteSpace(cels[TestResult_gameTestTime]) && DateTime.Parse(cels[TestResult_gameTestTime]).ToString("yyyyMM") == dttt.ToString("yyyyMM") && !string.IsNullOrWhiteSpace(cels[NetInfo_LteCi]))
                        {
                            if (!dicsevt[key].ContainsKey(cels[PositionInfo_City]))
                            {
                                dicsevt[key][cels[PositionInfo_City]] = new Dictionary<string, long>();
                                dicsevt[key][cels[PositionInfo_City]]["web"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game"] = 0;

                                dicsevt[key][cels[PositionInfo_City]]["web_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game_wg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["web_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["video_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["im_fwg"] = 0;
                                dicsevt[key][cels[PositionInfo_City]]["game_fwg"] = 0;
                            }
                            dicsevt[key][cels[PositionInfo_City]]["game"]++;
                            var ltecit = O2.O2I(cels[NetInfo_LteCi]);
                            if (wugao.ContainsKey(ltecit))
                            {
                                dicsevt[key][cels[PositionInfo_City]]["game_wg"]++;
                            }
                            else
                            {
                                dicsevt[key][cels[PositionInfo_City]]["game_fwg"]++;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(cels[PhoneInfo_MEID]) ||
                            string.IsNullOrWhiteSpace(cels[PositionInfo_City]) ||
                            nettype.IndexOf(cels[NetInfo_NetType]) >= 0 ||
                            string.IsNullOrWhiteSpace(cels[NetInfo_LteCi]) ||
                            gametar.IndexOf(cels[TestResult_gameName].Trim()) < 0 ||
                            string.IsNullOrWhiteSpace(cels[TestResult_gameTestTime]) ||

                            DateTime.Parse(cels[TestResult_gameTestTime]).ToString("yyyyMM") != dttt.ToString("yyyyMM") ||

                            cels[TestResult_TCLASS].Trim() != "1" ||
                            dics.ContainsKey(cels[PhoneInfo_MEID] + "_" + cels[TestResult_gameTestTime])

                            //剔除
                            || cels[TestResult_AckDelay].Trim() == "" || O2.O2D(cels[TestResult_AckDelay]) < 0
                            )
                        {
                            continue;
                        }
                        var dic = new Dictionary<string, object>();
                        dic["PhoneInfo_MEID"] = cels[PhoneInfo_MEID];
                        dic["PositionInfo_City"] = cels[PositionInfo_City];
                        dic["NetInfo_NetType"] = cels[NetInfo_NetType];
                        dic["NetInfo_LteCi"] = cels[NetInfo_LteCi];

                        dic["TestResult_gameName"] = cels[TestResult_gameName];
                        dic["TestResult_gameTestTime"] = cels[TestResult_gameTestTime];

                        dic["TestResult_AckDelay"] = cels[TestResult_AckDelay];

                        dic["TestResult_TCLASS"] = cels[TestResult_TCLASS];

                        dic["ISZHICHA1"] = (O2.O2I(cels[TestResult_AckDelay]) > gamedelay) ? "1" : "0";
                        dic["ISZHICHA2"] = "0";

                        var lteci = O2.O2I(dic["NetInfo_LteCi"]);
                        dic["wugao"] = wugao.ContainsKey(lteci) ? 1 : 0;

                        dics[cels[PhoneInfo_MEID] + "_" + cels[TestResult_gameTestTime]] = dic;
                    }
                }
            }

            //event分析
            foreach (var file in evtfiles)
            {
                Dictionary<string, Dictionary<string, long>> dics = dicsevt["ctcc"];
                var key = "ctcc";
                if (file.IndexOf("cmcc") > 0)
                {
                    dics = dicsevt["cmcc"];
                    key = "cmcc";
                }
                if (file.IndexOf("unicom") > 0)
                {
                    dics = dicsevt["cucc"];
                    key = "cucc";
                }
                var filestr = File.ReadAllText(file, Encoding.GetEncoding("gbk"));
                var filelines = filestr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var firstline = true;
                int EvtID = 0, EvtTIME = 0, City = 0, LteCi = 0;
                foreach (var line in filelines)
                {
                    if (firstline)
                    {
                        firstline = false;
                        var cols = line.Split(new char[] { ',' }).ToList();
                        EvtID = cols.IndexOf("Event_EvtID");
                        EvtTIME = cols.IndexOf("Event_EvtTIME");
                        City = cols.IndexOf("Event_City");
                        LteCi = cols.IndexOf("Event_LteCi");
                    }
                    else
                    {
                        var cels = line.Split(new char[] { ',' }).ToList();

                        if (string.IsNullOrWhiteSpace(cels[EvtID]) ||
                            string.IsNullOrWhiteSpace(cels[EvtTIME]) ||

                            DateTime.Parse(cels[EvtTIME]).ToString("yyyyMM") != dttt.ToString("yyyyMM") ||

                            string.IsNullOrWhiteSpace(cels[City])||
                            string.IsNullOrWhiteSpace(cels[LteCi])
                            )
                        {
                            continue;
                        }
                        if (!dicsevt[key].ContainsKey(cels[City]))
                        {
                            dicsevt[key][cels[City]] = new Dictionary<string, long>();
                        }
                        if (!dicsevt[key][cels[City]].ContainsKey(cels[EvtID]))
                        {
                            dicsevt[key][cels[City]][cels[EvtID]] = 0;

                            dicsevt[key][cels[City]][cels[EvtID]+"_wg"] = 0;
                            dicsevt[key][cels[City]][cels[EvtID] + "_fwg"] = 0;
                        }

                        dicsevt[key][cels[City]][cels[EvtID]]++;

                        if (wugao.ContainsKey(LteCi))
                        {
                            dicsevt[key][cels[City]][cels[EvtID] + "_wg"]++;
                        }
                        else {
                            dicsevt[key][cels[City]][cels[EvtID] + "_fwg"]++;
                        }
                    }
                }
            }

            //导出event
            foreach (var dic in dicsevt) {
                dic.Value["全省"] = new Dictionary<string, long>();
                dic.Value["全省"]["5008"] = 0;
                dic.Value["全省"]["5009"] = 0;
                dic.Value["全省"]["2002"] = 0;
                dic.Value["全省"]["2005"] = 0;
                dic.Value["全省"]["5021"] = 0;
                dic.Value["全省"]["5020"] = 0;
                dic.Value["全省"]["5023"] = 0;
                dic.Value["全省"]["web"] = 0;
                dic.Value["全省"]["video"] = 0;
                dic.Value["全省"]["im"] = 0;
                dic.Value["全省"]["game"] = 0;
                dic.Value["全省"]["eevt"] =0;
                dic.Value["全省"]["ebus"] =0;

                dic.Value["全省"]["5008_wg"] = 0;
                dic.Value["全省"]["5009_wg"] = 0;
                dic.Value["全省"]["2002_wg"] = 0;
                dic.Value["全省"]["2005_wg"] = 0;
                dic.Value["全省"]["5021_wg"] = 0;
                dic.Value["全省"]["5020_wg"] = 0;
                dic.Value["全省"]["5023_wg"] = 0;
                dic.Value["全省"]["web_wg"] = 0;
                dic.Value["全省"]["video_wg"] = 0;
                dic.Value["全省"]["im_wg"] = 0;
                dic.Value["全省"]["game_wg"] = 0;
                dic.Value["全省"]["eevt_wg"] = 0;
                dic.Value["全省"]["ebus_wg"] = 0;

                dic.Value["全省"]["5008_fwg"] = 0;
                dic.Value["全省"]["5009_fwg"] = 0;
                dic.Value["全省"]["2002_fwg"] = 0;
                dic.Value["全省"]["2005_fwg"] = 0;
                dic.Value["全省"]["5021_fwg"] = 0;
                dic.Value["全省"]["5020_fwg"] = 0;
                dic.Value["全省"]["5023_fwg"] = 0;
                dic.Value["全省"]["web_fwg"] = 0;
                dic.Value["全省"]["video_fwg"] = 0;
                dic.Value["全省"]["im_fwg"] = 0;
                dic.Value["全省"]["game_fwg"] = 0;
                dic.Value["全省"]["eevt_fwg"] = 0;
                dic.Value["全省"]["ebus_fwg"] = 0;

                var evtcsv = new StringBuilder();
                evtcsv.Append("地市,弱覆盖事件,无覆盖事件,数据掉线事件,数据连接建立失败事件,4G回落3G事件,4G回落2G事件,网络频繁切换事件,异常事件总数,LTE浏览总数,LTE视频总数,LTE即时消息发送次数,LTE游戏总数,业务总次数,异常事件率\r\n");
                foreach (var dic2 in dic.Value) {
                    var e5008=dic2.Value.ContainsKey("5008")?dic2.Value["5008"]:0;
                    var e5009 = dic2.Value.ContainsKey("5009") ? dic2.Value["5009"] : 0;
                    var e2002 = dic2.Value.ContainsKey("2002") ? dic2.Value["2002"] : 0;
                    var e2005 = dic2.Value.ContainsKey("2005") ? dic2.Value["2005"] : 0;
                    var e5021 = dic2.Value.ContainsKey("5021") ? dic2.Value["5021"] : 0;
                    var e5020 = dic2.Value.ContainsKey("5020") ? dic2.Value["5020"] : 0;
                    var e5023 = dic2.Value.ContainsKey("5023") ? dic2.Value["5023"] : 0;

                    var e5008_wg = dic2.Value.ContainsKey("5008_wg") ? dic2.Value["5008_wg"] : 0;
                    var e5009_wg = dic2.Value.ContainsKey("5009_wg") ? dic2.Value["5009_wg"] : 0;
                    var e2002_wg = dic2.Value.ContainsKey("2002_wg") ? dic2.Value["2002_wg"] : 0;
                    var e2005_wg = dic2.Value.ContainsKey("2005_wg") ? dic2.Value["2005_wg"] : 0;
                    var e5021_wg = dic2.Value.ContainsKey("5021_wg") ? dic2.Value["5021_wg"] : 0;
                    var e5020_wg = dic2.Value.ContainsKey("5020_wg") ? dic2.Value["5020_wg"] : 0;
                    var e5023_wg = dic2.Value.ContainsKey("5023_wg") ? dic2.Value["5023_wg"] : 0;

                    var e5008_fwg = dic2.Value.ContainsKey("5008_fwg") ? dic2.Value["5008_fwg"] : 0;
                    var e5009_fwg = dic2.Value.ContainsKey("5009_fwg") ? dic2.Value["5009_fwg"] : 0;
                    var e2002_fwg = dic2.Value.ContainsKey("2002_fwg") ? dic2.Value["2002_fwg"] : 0;
                    var e2005_fwg = dic2.Value.ContainsKey("2005_fwg") ? dic2.Value["2005_fwg"] : 0;
                    var e5021_fwg = dic2.Value.ContainsKey("5021_fwg") ? dic2.Value["5021_fwg"] : 0;
                    var e5020_fwg = dic2.Value.ContainsKey("5020_fwg") ? dic2.Value["5020_fwg"] : 0;
                    var e5023_fwg = dic2.Value.ContainsKey("5023_fwg") ? dic2.Value["5023_fwg"] : 0;

                    var eweb = dic2.Value.ContainsKey("web") ? dic2.Value["web"] : 0;
                    var evideo = dic2.Value.ContainsKey("video") ? dic2.Value["video"] : 0;
                    var eim = dic2.Value.ContainsKey("im") ? dic2.Value["im"] : 0;
                    var egame = dic2.Value.ContainsKey("game") ? dic2.Value["game"] : 0;

                    var eweb_wg = dic2.Value.ContainsKey("web_wg") ? dic2.Value["web_wg"] : 0;
                    var evideo_wg = dic2.Value.ContainsKey("video_wg") ? dic2.Value["video_wg"] : 0;
                    var eim_wg = dic2.Value.ContainsKey("im_wg") ? dic2.Value["im_wg"] : 0;
                    var egame_wg = dic2.Value.ContainsKey("game_wg") ? dic2.Value["game_wg"] : 0;

                    var eweb_fwg = dic2.Value.ContainsKey("web_fwg") ? dic2.Value["web_fwg"] : 0;
                    var evideo_fwg = dic2.Value.ContainsKey("video_fwg") ? dic2.Value["video_fwg"] : 0;
                    var eim_fwg = dic2.Value.ContainsKey("im_fwg") ? dic2.Value["im_fwg"] : 0;
                    var egame_fwg = dic2.Value.ContainsKey("game_fwg") ? dic2.Value["game_fwg"] : 0;


                    if (dic2.Key != "全省") {
                        dic.Value["全省"]["5008"] += e5008;
                        dic.Value["全省"]["5009"] += e5009;
                        dic.Value["全省"]["2002"] += e2002;
                        dic.Value["全省"]["2005"] += e2005;
                        dic.Value["全省"]["5021"] += e5021;
                        dic.Value["全省"]["5020"] += e5020;
                        dic.Value["全省"]["5023"] += e5023;
                        dic.Value["全省"]["web"] += eweb;
                        dic.Value["全省"]["video"] += evideo;
                        dic.Value["全省"]["im"] += eim;
                        dic.Value["全省"]["game"] += egame;

                        dic.Value["全省"]["5008_wg"] += e5008_wg;
                        dic.Value["全省"]["5009_wg"] += e5009_wg;
                        dic.Value["全省"]["2002_wg"] += e2002_wg;
                        dic.Value["全省"]["2005_wg"] += e2005_wg;
                        dic.Value["全省"]["5021_wg"] += e5021_wg;
                        dic.Value["全省"]["5020_wg"] += e5020_wg;
                        dic.Value["全省"]["5023_wg"] += e5023_wg;
                        dic.Value["全省"]["web_wg"] += eweb_wg;
                        dic.Value["全省"]["video_wg"] += evideo_wg;
                        dic.Value["全省"]["im_wg"] += eim_wg;
                        dic.Value["全省"]["game_wg"] += egame_wg;

                        dic.Value["全省"]["5008_fwg"] += e5008_fwg;
                        dic.Value["全省"]["5009_fwg"] += e5009_fwg;
                        dic.Value["全省"]["2002_fwg"] += e2002_fwg;
                        dic.Value["全省"]["2005_fwg"] += e2005_fwg;
                        dic.Value["全省"]["5021_fwg"] += e5021_fwg;
                        dic.Value["全省"]["5020_fwg"] += e5020_fwg;
                        dic.Value["全省"]["5023_fwg"] += e5023_fwg;
                        dic.Value["全省"]["web_fwg"] += eweb_fwg;
                        dic.Value["全省"]["video_fwg"] += evideo_fwg;
                        dic.Value["全省"]["im_fwg"] += eim_fwg;
                        dic.Value["全省"]["game_fwg"] += egame_fwg;
                    }
                    var eevt=e5008 + e5009 + e2002 + e2005 + e5021 + e5020 + e5023;
                    var ebus=eweb+evideo+eim+egame;
                    dic2.Value["eevt"] = eevt;
                    dic2.Value["ebus"] = ebus;
                    dic.Value["全省"]["eevt"] += eevt;
                    dic.Value["全省"]["ebus"] += ebus;

                    var eevt_wg = e5008_wg + e5009_wg + e2002_wg + e2005_wg + e5021_wg + e5020_wg + e5023_wg;
                    var ebus_wg = eweb_wg + evideo_wg + eim_wg + egame_wg;
                    dic2.Value["eevt_wg"] = eevt_wg;
                    dic2.Value["ebus_wg"] = ebus_wg;
                    dic.Value["全省"]["eevt_wg"] += eevt_wg;
                    dic.Value["全省"]["ebus_wg"] += ebus_wg;

                    var eevt_fwg = e5008_fwg + e5009_fwg + e2002_fwg + e2005_fwg + e5021_fwg + e5020_fwg + e5023_fwg;
                    var ebus_fwg = eweb_fwg + evideo_fwg + eim_fwg + egame_fwg;
                    dic2.Value["eevt_fwg"] = eevt_fwg;
                    dic2.Value["ebus_fwg"] = ebus_fwg;
                    dic.Value["全省"]["eevt_fwg"] += eevt_fwg;
                    dic.Value["全省"]["ebus_fwg"] += ebus_fwg;
                    
                    var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}\r\n",
                        dic2.Key, e5008, e5009, e2002, e2005, e5021, e5020, e5023,
                        eevt,
                        eweb,evideo,eim,egame,
                        ebus,
                        ebus<=0?0.0f:((float)eevt)/ebus
                        );
                    evtcsv.Append(line);
                }
                var path = exportpath + "evtdata\\" + dttt.ToString("yyyyMM") + "\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.WriteAllText(path + dttt.ToString("yyyyMMdd") + "_" + dic.Key + ".csv", evtcsv.ToString(), Encoding.GetEncoding("gbk"));
            }

            #endregion
            #region 写入dataset
            var dsweb = toDics2Ds(dicsweb, 1);
            var dsvideo = toDics2Ds(dicsvideo, 2);
            var dsim = toDics2Ds(dicsim, 3);
            var dsgame = toDics2Ds(dicsgame, 4);
            #endregion
            #region 根据数量筛选每个datatable
            for (int i = 0; i < dsweb.Tables.Count; i++)
            {
                var dt = dsweb.Tables[0];
                if (dt.Rows.Count <= 0)
                {
                    dsweb.Tables.Remove(dt);
                    dsweb.Tables.Add(dt);
                    continue;
                }
                var de = dt.AsEnumerable();
                var delCi = de.GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, count = a.Count(b => true) }).Where(a => a.count <= webcount).Select(a => a.ci).ToDictionary(a => a);
                var dt2a = dt.AsEnumerable().Where(a => !delCi.ContainsKey(a["NetInfo_LteCi"])).ToArray();
                if (dt2a.Length <= 0)
                {
                    dt.Clear();
                    dsweb.Tables.Remove(dt);
                    dsweb.Tables.Add(dt);
                    continue;
                }
                var dt2 = dt2a.CopyToDataTable();
                dt2.TableName = dt.TableName;
                dsweb.Tables.Remove(dt);
                dsweb.Tables.Add(dt2);
            }
            for (int i = 0; i < dsvideo.Tables.Count; i++)
            {
                var dt = dsvideo.Tables[0];
                if (dt.Rows.Count <= 0)
                {
                    dsvideo.Tables.Remove(dt);
                    dsvideo.Tables.Add(dt);
                    continue;
                }
                var de = dt.AsEnumerable();
                var delCi = de.GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, count = a.Count(b => true) }).Where(a => a.count <= videocount).Select(a => a.ci).ToDictionary(a => a);
                var dt2a = dt.AsEnumerable().Where(a => !delCi.ContainsKey(a["NetInfo_LteCi"])).ToArray();
                if (dt2a.Length <= 0)
                {
                    dt.Clear();
                    dsvideo.Tables.Remove(dt);
                    dsvideo.Tables.Add(dt);
                    continue;
                }
                var dt2 = dt2a.CopyToDataTable();
                dt2.TableName = dt.TableName;
                dsvideo.Tables.Remove(dt);
                dsvideo.Tables.Add(dt2);
            }
            for (int i = 0; i < dsim.Tables.Count; i++)
            {
                var dt = dsim.Tables[0];
                if (dt.Rows.Count <= 0)
                {
                    dsim.Tables.Remove(dt);
                    dsim.Tables.Add(dt);
                    continue;
                }
                var de = dt.AsEnumerable();
                var delCi = de.GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, count = a.Count(b => true) }).Where(a => a.count <= imcount).Select(a => a.ci).ToDictionary(a => a);
                var dt2a = dt.AsEnumerable().Where(a => !delCi.ContainsKey(a["NetInfo_LteCi"])).ToArray();
                if (dt2a.Length <= 0)
                {
                    dt.Clear();
                    dsim.Tables.Remove(dt);
                    dsim.Tables.Add(dt);
                    continue;
                }
                var dt2 = dt2a.CopyToDataTable();
                dt2.TableName = dt.TableName;
                dsim.Tables.Remove(dt);
                dsim.Tables.Add(dt2);
            }
            for (int i = 0; i < dsgame.Tables.Count; i++)
            {
                var dt = dsgame.Tables[0];
                if (dt.Rows.Count <= 0)
                {
                    dsgame.Tables.Remove(dt);
                    dsgame.Tables.Add(dt);
                    continue;
                }
                var de = dt.AsEnumerable();
                var delCi = de.GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, count = a.Count(b => true) }).Where(a => a.count <= gamecount).Select(a => a.ci).ToDictionary(a => a);
                var dt2a = dt.AsEnumerable().Where(a => !delCi.ContainsKey(a["NetInfo_LteCi"])).ToArray();
                if (dt2a.Length <= 0)
                {
                    dt.Clear();
                    dsgame.Tables.Remove(dt);
                    dsgame.Tables.Add(dt);
                    continue;
                }
                var dt2 = dt2a.CopyToDataTable();
                dt2.TableName = dt.TableName;
                dsgame.Tables.Remove(dt);
                dsgame.Tables.Add(dt2);
            }
            #endregion
            #region 优良率统计
            #region 旧方法删除
            //var dicswebgood = new Dictionary<string, Dictionary<string, double>>();
            //var dicsvideogood = new Dictionary<string, Dictionary<string, double>>();
            //var dicsimgood = new Dictionary<string, Dictionary<string, double>>();
            //var dicsgamegood = new Dictionary<string, Dictionary<string, double>>();
            //#region 统计地市各指标优良率
            //foreach (DataTable dt in dsweb.Tables)
            //{
            //    var dexq = dt.AsEnumerable().GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, city = a.Max(b => b["PositionInfo_City"]), goodrate = 0.0 * (1 - a.Sum(b => O2.O2D(b["ISZHICHA1"])).Div0(a.Count())) + 1.0 * (1 - a.Sum(b => O2.O2D(b["ISZHICHA2"])).Div0(a.Count())) });
            //    var dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => b.goodrate);
            //    dicswebgood[dt.TableName] = dicgood;
            //}
            //foreach (DataTable dt in dsvideo.Tables)
            //{
            //    var dexq = dt.AsEnumerable().GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, city = a.Max(b => b["PositionInfo_City"]), goodrate = 0.8 * (1 - a.Sum(b => O2.O2D(b["ISZHICHA1"])).Div0(a.Count())) + 0.2 * (1 - a.Sum(b => O2.O2D(b["ISZHICHA2"])).Div0(a.Count())) });
            //    var dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => b.goodrate);
            //    dicsvideogood[dt.TableName] = dicgood;
            //}
            //foreach (DataTable dt in dsim.Tables)
            //{
            //    var dexq = dt.AsEnumerable().GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, city = a.Max(b => b["PositionInfo_City"]), goodrate = a.Sum(b => ((long)b["TestResult_ImSendCount"]) * ((double)b["TestResult_ImSendRate"])).Div1(a.Sum(b => (long)b["TestResult_ImSendCount"])) });
            //    var dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => b.goodrate);
            //    dicsimgood[dt.TableName] = dicgood;
            //}
            //foreach (DataTable dt in dsgame.Tables)
            //{
            //    var dexq = dt.AsEnumerable().GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, city = a.Max(b => b["PositionInfo_City"]), goodrate = 1 - a.Sum(b => O2.O2D(b["ISZHICHA1"])).Div0(a.Count()) });
            //    var dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => b.goodrate);
            //    dicsgamegood[dt.TableName] = dicgood;
            //}
            #endregion
            
            var dicswebgood = new Dictionary<string, Dictionary<string, Dictionary<string,double>>>();
            var dicsvideogood = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            var dicsimgood = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            var dicsgamegood = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

            //var dicsevtgood = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

            var dicswebgoodqs = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            var dicsvideogoodqs = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            var dicsimgoodqs = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            var dicsgamegoodqs = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

            //var dicsevtgoodqs = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

            #region 统计地市各指标优良率
            foreach (DataTable dt in dsweb.Tables)
            {
                var dexq = dt.AsEnumerable().GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, wugao = a.Max(b => (long)b["wugao"]), city = a.Max(b => b["PositionInfo_City"]), goodrate = 0.7 * (1 - a.Sum(b => O2.O2D(b["ISZHICHA1"])).Div0(a.Count())) + 0.3 * (1 - a.Sum(b => O2.O2D(b["ISZHICHA2"])).Div0(a.Count())) });
                Dictionary<string, Dictionary<string, double>> dicgood = null;
                if (dt.TableName == "ctcc")
                {
                    dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate * (1 - b.wugao)).Div1(a.Sum(b => 1 - b.wugao)), goodratewg = a.Sum(b => b.goodrate * (b.wugao)).Div1(a.Sum(b => b.wugao)) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                else
                {
                    dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()), goodratewg = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                dicswebgood[dt.TableName] = dicgood;

                if (dt.TableName == "ctcc")
                {
                    dicgood = dexq.GroupBy(a => "全省").Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate * (1 - b.wugao)).Div1(a.Sum(b => 1 - b.wugao)), goodratewg = a.Sum(b => b.goodrate * (b.wugao)).Div1(a.Sum(b => b.wugao)) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                else
                {
                    dicgood = dexq.GroupBy(a => "全省").Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()), goodratewg = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                dicswebgoodqs[dt.TableName] = dicgood;
            }
            foreach (DataTable dt in dsvideo.Tables)
            {
                var dexq = dt.AsEnumerable().GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, wugao = a.Max(b => (long)b["wugao"]), city = a.Max(b => b["PositionInfo_City"]), goodrate = 0.8 * (1 - a.Sum(b => O2.O2D(b["ISZHICHA1"])).Div0(a.Count())) + 0.2 * (1 - a.Sum(b => O2.O2D(b["ISZHICHA2"])).Div0(a.Count())) });
                Dictionary<string, Dictionary<string, double>> dicgood = null;
                if (dt.TableName == "ctcc")
                {
                    dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate * (1 - b.wugao)).Div1(a.Sum(b => 1 - b.wugao)), goodratewg = a.Sum(b => b.goodrate * (b.wugao)).Div1(a.Sum(b => b.wugao)) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                else
                {
                    dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()), goodratewg = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                dicsvideogood[dt.TableName] = dicgood;

                if (dt.TableName == "ctcc")
                {
                    dicgood = dexq.GroupBy(a => "全省").Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate * (1 - b.wugao)).Div1(a.Sum(b => 1 - b.wugao)), goodratewg = a.Sum(b => b.goodrate * (b.wugao)).Div1(a.Sum(b => b.wugao)) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                else
                {
                    dicgood = dexq.GroupBy(a => "全省").Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()), goodratewg = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                dicsvideogoodqs[dt.TableName] = dicgood;
            }
            foreach (DataTable dt in dsim.Tables)
            {
                var dexq = dt.AsEnumerable().GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, wugao = a.Max(b => (long)b["wugao"]), city = a.Max(b => b["PositionInfo_City"]), goodrate = a.Sum(b => ((long)b["TestResult_ImSendCount"]) * ((double)b["TestResult_ImSendRate"])).Div1(a.Sum(b => (long)b["TestResult_ImSendCount"])) });
                Dictionary<string, Dictionary<string, double>> dicgood = null;
                if (dt.TableName == "ctcc")
                {
                    dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate * (1 - b.wugao)).Div1(a.Sum(b => 1 - b.wugao)), goodratewg = a.Sum(b => b.goodrate * (b.wugao)).Div1(a.Sum(b => b.wugao)) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                else
                {
                    dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()), goodratewg = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                dicsimgood[dt.TableName] = dicgood;

                if (dt.TableName == "ctcc")
                {
                    dicgood = dexq.GroupBy(a => "全省").Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate * (1 - b.wugao)).Div1(a.Sum(b => 1 - b.wugao)), goodratewg = a.Sum(b => b.goodrate * (b.wugao)).Div1(a.Sum(b => b.wugao)) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                else
                {
                    dicgood = dexq.GroupBy(a => "全省").Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()), goodratewg = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                dicsimgoodqs[dt.TableName] = dicgood;
            }
            foreach (DataTable dt in dsgame.Tables)
            {
                var dexq = dt.AsEnumerable().GroupBy(a => a["NetInfo_LteCi"]).Select(a => new { ci = a.Key, wugao = a.Max(b => (long)b["wugao"]), city = a.Max(b => b["PositionInfo_City"]), goodrate = 1 - a.Sum(b => O2.O2D(b["ISZHICHA1"])).Div0(a.Count()) });
                Dictionary<string, Dictionary<string, double>> dicgood = null;
                if (dt.TableName == "ctcc")
                {
                    dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate * (1 - b.wugao)).Div1(a.Sum(b => 1 - b.wugao)), goodratewg = a.Sum(b => b.goodrate * (b.wugao)).Div1(a.Sum(b => b.wugao)) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                else
                {
                    dicgood = dexq.GroupBy(a => a.city).Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()), goodratewg = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                dicsgamegood[dt.TableName] = dicgood;

                if (dt.TableName == "ctcc")
                {
                    dicgood = dexq.GroupBy(a => "全省").Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate * (1 - b.wugao)).Div1(a.Sum(b => 1 - b.wugao)), goodratewg = a.Sum(b => b.goodrate * (b.wugao)).Div1(a.Sum(b => b.wugao)) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                else
                {
                    dicgood = dexq.GroupBy(a => "全省").Select(a => new { city = a.Key, goodrate = a.Sum(b => b.goodrate).Div1(a.Count()), goodratewg = a.Sum(b => b.goodrate).Div1(a.Count()) }).ToDictionary(a => a.city.ToString(), b => new Dictionary<string, double>() { { "goodrate", b.goodrate }, { "goodratewg", b.goodratewg } });
                }
                dicsgamegoodqs[dt.TableName] = dicgood;
            }
            #endregion
            #region 优良率合并
            var dicsgoodrate = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            #region 旧方法删除
            //foreach (var dic in dicswebgood)
            //{
            //    if (!dicsgoodrate.ContainsKey(dic.Key))
            //    {
            //        dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
            //    }
            //    foreach (var dici in dic.Value)
            //    {
            //        if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
            //        {
            //            dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
            //        }
            //        dicsgoodrate[dic.Key][dici.Key]["webgood"] = dici.Value;
            //    }
            //}
            //foreach (var dic in dicsvideogood)
            //{
            //    if (!dicsgoodrate.ContainsKey(dic.Key))
            //    {
            //        dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
            //    }
            //    foreach (var dici in dic.Value)
            //    {
            //        if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
            //        {
            //            dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
            //        }
            //        dicsgoodrate[dic.Key][dici.Key]["videogood"] = dici.Value;
            //    }
            //}
            //foreach (var dic in dicsimgood)
            //{
            //    if (!dicsgoodrate.ContainsKey(dic.Key))
            //    {
            //        dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
            //    }
            //    foreach (var dici in dic.Value)
            //    {
            //        if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
            //        {
            //            dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
            //        }
            //        dicsgoodrate[dic.Key][dici.Key]["imgood"] = dici.Value;
            //    }
            //}
            //foreach (var dic in dicsgamegood)
            //{
            //    if (!dicsgoodrate.ContainsKey(dic.Key))
            //    {
            //        dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
            //    }
            //    foreach (var dici in dic.Value)
            //    {
            //        if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
            //        {
            //            dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
            //        }
            //        dicsgoodrate[dic.Key][dici.Key]["gamegood"] = dici.Value;
            //    }
            //}
            //foreach (var dic in dicsgoodrate)
            //{
            //    foreach (var dici in dic.Value)
            //    {
            //        var webgood = !dici.Value.ContainsKey("webgood") ? 1 : dici.Value["webgood"];
            //        var videogood = !dici.Value.ContainsKey("videogood") ? 1 : dici.Value["videogood"];
            //        var imgood = !dici.Value.ContainsKey("imgood") ? 1 : dici.Value["imgood"];
            //        var gamegood = !dici.Value.ContainsKey("gamegood") ? 1 : dici.Value["gamegood"];
            //        dici.Value["goodrate"] = 0.5 * webgood + 0.3 * videogood + 0.1 * imgood + 0.1 * gamegood;
            //    }
            //}
            #endregion

            foreach (var dic in dicswebgood)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["webgood"] = dici.Value["goodrate"];
                    dicsgoodrate[dic.Key][dici.Key]["webgoodwg"] = dici.Value["goodratewg"];
                }
            }
            foreach (var dic in dicsvideogood)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["videogood"] = dici.Value["goodrate"];
                    dicsgoodrate[dic.Key][dici.Key]["videogoodwg"] = dici.Value["goodratewg"];
                }
            }
            foreach (var dic in dicsimgood)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["imgood"] = dici.Value["goodrate"];
                    dicsgoodrate[dic.Key][dici.Key]["imgoodwg"] = dici.Value["goodratewg"];
                }
            }
            foreach (var dic in dicsgamegood)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["gamegood"] = dici.Value["goodrate"];
                    dicsgoodrate[dic.Key][dici.Key]["gamegoodwg"] = dici.Value["goodratewg"];
                }
            }

            foreach (var dic in dicsevt)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (dici.Key == "全省") continue;
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["evtgood"] = 1-((double)dici.Value["eevt_fwg"]).Div0(dici.Value["ebus_fwg"]);
                    dicsgoodrate[dic.Key][dici.Key]["evtgoodwg"] = 1-((double)dici.Value["eevt_wg"]).Div0(dici.Value["ebus_wg"]);

                    
                }
            }
            

            //全省
            foreach (var dic in dicswebgoodqs)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["webgood"] = dici.Value["goodrate"];
                    dicsgoodrate[dic.Key][dici.Key]["webgoodwg"] = dici.Value["goodratewg"];
                }
            }
            foreach (var dic in dicsvideogoodqs)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["videogood"] = dici.Value["goodrate"];
                    dicsgoodrate[dic.Key][dici.Key]["videogoodwg"] = dici.Value["goodratewg"];
                }
            }
            foreach (var dic in dicsimgoodqs)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["imgood"] = dici.Value["goodrate"];
                    dicsgoodrate[dic.Key][dici.Key]["imgoodwg"] = dici.Value["goodratewg"];
                }
            }
            foreach (var dic in dicsgamegoodqs)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["gamegood"] = dici.Value["goodrate"];
                    dicsgoodrate[dic.Key][dici.Key]["gamegoodwg"] = dici.Value["goodratewg"];
                }
            }
            foreach (var dic in dicsevt)
            {
                if (!dicsgoodrate.ContainsKey(dic.Key))
                {
                    dicsgoodrate[dic.Key] = new Dictionary<string, Dictionary<string, double>>();
                }
                foreach (var dici in dic.Value)
                {
                    if (dici.Key != "全省") continue;
                    if (!dicsgoodrate[dic.Key].ContainsKey(dici.Key))
                    {
                        dicsgoodrate[dic.Key][dici.Key] = new Dictionary<string, double>();
                    }
                    dicsgoodrate[dic.Key][dici.Key]["evtgood"] = 1 - ((double)dici.Value["eevt_fwg"]).Div0(dici.Value["ebus_fwg"]);
                    dicsgoodrate[dic.Key][dici.Key]["evtgoodwg"] = 1 - ((double)dici.Value["eevt_wg"]).Div0(dici.Value["ebus_wg"]);
                }
            }
            foreach (var dic in dicsgoodrate)
            {
                foreach (var dici in dic.Value)
                {
                    #region 旧方法删除
                    //var webgood = !dici.Value.ContainsKey("webgood") ? 1 : dici.Value["webgood"];
                    //var videogood = !dici.Value.ContainsKey("videogood") ? 1 : dici.Value["videogood"];
                    //var imgood = !dici.Value.ContainsKey("imgood") ? 1 : dici.Value["imgood"];
                    //var gamegood = !dici.Value.ContainsKey("gamegood") ? 1 : dici.Value["gamegood"];
                    //dici.Value["goodrate"] = 0.5 * webgood + 0.3 * videogood + 0.1 * imgood + 0.1 * gamegood;
                    #endregion

                    var webgood = !dici.Value.ContainsKey("webgood") ? 1 : dici.Value["webgood"];
                    var videogood = !dici.Value.ContainsKey("videogood") ? 1 : dici.Value["videogood"];
                    var imgood = !dici.Value.ContainsKey("imgood") ? 1 : dici.Value["imgood"];
                    var gamegood = !dici.Value.ContainsKey("gamegood") ? 1 : dici.Value["gamegood"];
                    var evtgood = !dici.Value.ContainsKey("evtgood") ? 1 : dici.Value["evtgood"];

                    var webgoodwg = !dici.Value.ContainsKey("webgoodwg") ? 1 : dici.Value["webgoodwg"];
                    var videogoodwg = !dici.Value.ContainsKey("videogoodwg") ? 1 : dici.Value["videogoodwg"];
                    var imgoodwg = !dici.Value.ContainsKey("imgoodwg") ? 1 : dici.Value["imgoodwg"];
                    var gamegoodwg = !dici.Value.ContainsKey("gamegoodwg") ? 1 : dici.Value["gamegoodwg"];
                    var evtgoodwg = !dici.Value.ContainsKey("evtgoodwg") ? 1 : dici.Value["evtgoodwg"];

                    dici.Value["goodrate"] = 0.4 * webgood + 0.35 * videogood + 0.15 * imgood + 0.1 * gamegood;
                    dici.Value["goodratewg"] = 0.4 * webgoodwg + 0.35 * videogoodwg + 0.15 * imgoodwg + 0.1 * gamegoodwg;

                    dici.Value["goodrateevt"] = dici.Value["goodrate"] * 0.9 + evtgood * 0.1;
                    dici.Value["goodrateevtwg"] = dici.Value["goodratewg"] * 0.9 + evtgoodwg * 0.1;
                }
            }
            #endregion
            #endregion
            #region 导出csv
            foreach (var dic in dicsgoodrate)
            {
                StringBuilder sb = new StringBuilder();
                #region 旧方法删除
                //sb.Append("地市,web优良率,视频优良率,即时通讯优良率,游戏优良率,综合优良率\r\n");
                //foreach (var dici in dic.Value)
                //{
                //    var webgood = !dici.Value.ContainsKey("webgood") ? 1 : dici.Value["webgood"];
                //    var videogood = !dici.Value.ContainsKey("videogood") ? 1 : dici.Value["videogood"];
                //    var imgood = !dici.Value.ContainsKey("imgood") ? 1 : dici.Value["imgood"];
                //    var gamegood = !dici.Value.ContainsKey("gamegood") ? 1 : dici.Value["gamegood"];
                //    var goodrate = !dici.Value.ContainsKey("goodrate") ? 1 : dici.Value["goodrate"];
                //    sb.Append(dici.Key + "," + webgood + "," + videogood + "," + imgood + "," + gamegood + "," + goodrate + "\r\n");
                //}
                #endregion
                sb.Append("地市,web优良率,视频优良率,即时通讯优良率,游戏优良率,综合优良率,五高一地优良率,非五高一地优良率,异常事件优良率,综合带异常事件优良率,五高一地带异常事件优良率,非五高一地带异常事件优良率\r\n");
                foreach (var dici in dic.Value)
                {
                    var webgood = !dici.Value.ContainsKey("webgood") ? 1 : dici.Value["webgood"];
                    var videogood = !dici.Value.ContainsKey("videogood") ? 1 : dici.Value["videogood"];
                    var imgood = !dici.Value.ContainsKey("imgood") ? 1 : dici.Value["imgood"];
                    var gamegood = !dici.Value.ContainsKey("gamegood") ? 1 : dici.Value["gamegood"];

                    var webgoodwg = !dici.Value.ContainsKey("webgoodwg") ? 1 : dici.Value["webgoodwg"];
                    var videogoodwg = !dici.Value.ContainsKey("videogoodwg") ? 1 : dici.Value["videogoodwg"];
                    var imgoodwg = !dici.Value.ContainsKey("imgoodwg") ? 1 : dici.Value["imgoodwg"];
                    var gamegoodwg = !dici.Value.ContainsKey("gamegoodwg") ? 1 : dici.Value["gamegoodwg"];

                    var goodrate = !dici.Value.ContainsKey("goodrate") ? 1 : dici.Value["goodrate"];
                    var goodratewg = !dici.Value.ContainsKey("goodratewg") ? 1 : dici.Value["goodratewg"];

                    var goodratezh = goodrate * 0.5 + goodratewg * 0.5;

                    var evtgood = !dici.Value.ContainsKey("evtgood") ? 1 : dici.Value["evtgood"];
                    var evtgoodwg = !dici.Value.ContainsKey("evtgoodwg") ? 1 : dici.Value["evtgoodwg"];

                    var goodrateevt = !dici.Value.ContainsKey("goodrateevt") ? 1 : dici.Value["goodrateevt"];
                    var goodrateevtwg = !dici.Value.ContainsKey("goodrateevtwg") ? 1 : dici.Value["goodrateevtwg"];

                    sb.Append(dici.Key + "," + (webgood * 0.5 + webgoodwg * 0.5) + "," + (videogood * 0.5 + videogood * 0.5) + "," + (imgood * 0.5 + imgoodwg * 0.5) + "," + (gamegood * 0.5 + gamegoodwg * 0.5) + "," + goodratezh + "," + goodratewg + "," + goodrate + ","+(evtgood*0.5+evtgoodwg*0.5)+","+(goodrateevt*0.5+goodrateevtwg*0.5)+","+goodrateevtwg+","+goodrateevt+"\r\n");
                }
                var path = exportpath + dttt.ToString("yyyyMM") + "\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.WriteAllText(path + dttt.ToString("yyyyMMdd") + "_" + dic.Key + ".csv", sb.ToString(), Encoding.GetEncoding("gbk"));
            }
            #endregion

            #region 地市指标
            var dicswebcity = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            var dicsvideocity = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            var dicsimcity = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            var dicsgamecity = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            foreach (DataTable dt in dsweb.Tables)
            {
                var gobj = dt.AsEnumerable().GroupBy(a => new { ci = a["NetInfo_LteCi"], time = O2.O2DT(a["TestResult_PageSurfTime"].ToString()).Date }).Select(a => new { ci = a.Key.ci, time = a.Key.time, city = a.Max(b => b["PositionInfo_City"]), popen = a.Sum(b => O2.O2D(b["TestResult_PageOpenDelay"])) / a.Count(), pfstbyte = a.Sum(b => O2.O2D(b["TestResult_FirstByteDelay"])) / a.Count(), pfstscreen = a.Sum(b => O2.O2D(b["TestResult_FirstScreenDelay"])) / a.Count() });
                var diczb = gobj.GroupBy(a => new { city = a.city, time = a.time }).Select(a => new Dictionary<string, object>() { { "CITY", a.Key.city }, { "CTIME", a.Key.time }, { "PAGEOPENDELAY", a.Sum(b => b.popen) }, { "FIRSTBYTEDELAY", a.Sum(b => b.pfstbyte) }, { "FIRSTSCREENDELAY", a.Sum(b => b.pfstscreen) }, { "PAGEOPENDELAY_COUNT", a.Count() }, { "FIRSTBYTEDELAY_COUNT", a.Count() }, { "FIRSTSCREENDELAY_COUNT", a.Count() } }).ToDictionary(a => a["CITY"] + "|" + ((DateTime)a["CTIME"]).ToString("yyyyMMdd"), b => b);
                dicswebcity[dt.TableName] = diczb;
            }
            foreach (DataTable dt in dsvideo.Tables)
            {
                var gobj = dt.AsEnumerable().GroupBy(a => new { ci = a["NetInfo_LteCi"], time = O2.O2DT(a["TestResult_VideoTestTime"].ToString()).Date }).Select(a => new { ci = a.Key.ci, time = a.Key.time, city = a.Max(b => b["PositionInfo_City"]), vspeed = a.Sum(b => O2.O2D(b["TestResult_VideoAvgSpeed"])) / a.Count(), vcache = a.Sum(b => O2.O2D(b["TestResult_CacheRate"])) / a.Count() });
                var diczb = gobj.GroupBy(a => new { city = a.city, time = a.time }).Select(a => new Dictionary<string, object>() { { "CITY", a.Key.city }, { "CTIME", a.Key.time }, { "VIDEOAVGSPEED", a.Sum(b => b.vspeed) }, { "CACHERATE", a.Sum(b => b.vcache) }, { "VIDEOAVGSPEED_COUNT", a.Count() }, { "CACHERATE_COUNT", a.Count() } }).ToDictionary(a => a["CITY"] + "|" + ((DateTime)a["CTIME"]).ToString("yyyyMMdd"), b => b);
                dicsvideocity[dt.TableName] = diczb;
            }
            foreach (DataTable dt in dsim.Tables)
            {
                var gobj = dt.AsEnumerable().GroupBy(a => new { ci = a["NetInfo_LteCi"], time = O2.O2DT(a["TestResult_ImTestTime"].ToString()).Date }).Select(a => new { ci = a.Key.ci, time = a.Key.time, city = a.Max(b => b["PositionInfo_City"]), irate = a.Sum(b => O2.O2D(b["TestResult_ImSendCount"]) * O2.O2D(b["TestResult_ImSendRate"])) / a.Sum(b => O2.O2D(b["TestResult_ImSendCount"])) });
                var diczb = gobj.GroupBy(a => new { city = a.city, time = a.time }).Select(a => new Dictionary<string, object>() { { "CITY", a.Key.city }, { "CTIME", a.Key.time }, { "IMSENDRATE", a.Sum(b => b.irate) }, { "IMSENDRATE_COUNT", a.Count() } }).ToDictionary(a => a["CITY"] + "|" + ((DateTime)a["CTIME"]).ToString("yyyyMMdd"), b => b);
                dicsimcity[dt.TableName] = diczb;
            }
            foreach (DataTable dt in dsgame.Tables)
            {
                var gobj = dt.AsEnumerable().GroupBy(a => new { ci = a["NetInfo_LteCi"], time = O2.O2DT(a["TestResult_gameTestTime"].ToString()).Date }).Select(a => new { ci = a.Key.ci, time = a.Key.time, city = a.Max(b => b["PositionInfo_City"]), gdelay = a.Sum(b => O2.O2D(b["TestResult_AckDelay"])) / a.Count() });
                var diczb = gobj.GroupBy(a => new { city = a.city, time = a.time }).Select(a => new Dictionary<string, object>() { { "CITY", a.Key.city }, { "CTIME", a.Key.time }, { "ACKDELAY", a.Sum(b => b.gdelay) }, { "ACKDELAY_COUNT", a.Count() } }).ToDictionary(a => a["CITY"] + "|" + ((DateTime)a["CTIME"]).ToString("yyyyMMdd"), b => b);
                dicsgamecity[dt.TableName] = diczb;
            }
            var dicscity = new Dictionary<string, Dictionary<string, object>>();
            foreach (var dic in dicswebcity)
            {
                foreach (var dic2 in dic.Value)
                {
                    var key = dic2.Key + "|" + dic.Key;
                    if (!dicscity.ContainsKey(key))
                    {
                        dicscity[key] = new Dictionary<string, object>();
                    }
                    foreach (var dic3 in dic2.Value)
                    {
                        dicscity[key][dic3.Key] = dic3.Value;
                    }
                    dicscity[key]["OPERATOR"] = dic.Key;
                }
            }
            foreach (var dic in dicsvideocity)
            {
                foreach (var dic2 in dic.Value)
                {
                    var key = dic2.Key + "|" + dic.Key;
                    if (!dicscity.ContainsKey(key))
                    {
                        dicscity[key] = new Dictionary<string, object>();
                    }
                    foreach (var dic3 in dic2.Value)
                    {
                        dicscity[key][dic3.Key] = dic3.Value;
                    }
                    dicscity[key]["OPERATOR"] = dic.Key;
                }
            }
            foreach (var dic in dicsimcity)
            {
                foreach (var dic2 in dic.Value)
                {
                    var key = dic2.Key + "|" + dic.Key;
                    if (!dicscity.ContainsKey(key))
                    {
                        dicscity[key] = new Dictionary<string, object>();
                    }
                    foreach (var dic3 in dic2.Value)
                    {
                        dicscity[key][dic3.Key] = dic3.Value;
                    }
                    dicscity[key]["OPERATOR"] = dic.Key;
                }
            }
            foreach (var dic in dicsgamecity)
            {
                foreach (var dic2 in dic.Value)
                {
                    var key = dic2.Key + "|" + dic.Key;
                    if (!dicscity.ContainsKey(key))
                    {
                        dicscity[key] = new Dictionary<string, object>();
                    }
                    foreach (var dic3 in dic2.Value)
                    {
                        dicscity[key][dic3.Key] = dic3.Value;
                    }
                    dicscity[key]["OPERATOR"] = dic.Key;
                }
            }

            #endregion
            #region 内容指标
            var dicswebcontent = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            var dicsvideocontent = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            var dicsimcontent = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            var dicsgamecontent = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            foreach (DataTable dt in dsweb.Tables)
            {
                var gobj = dt.AsEnumerable().GroupBy(a => new { ci = a["NetInfo_LteCi"], time = O2.O2DT(a["TestResult_PageSurfTime"].ToString()).Date }).Select(a => new { ci = a.Key.ci, time = a.Key.time, content = a.Max(b => b["TestResult_WebsiteName"]), popen = a.Sum(b => O2.O2D(b["TestResult_PageOpenDelay"])) / a.Count(), pfstbyte = a.Sum(b => O2.O2D(b["TestResult_FirstByteDelay"])) / a.Count(), pfstscreen = a.Sum(b => O2.O2D(b["TestResult_FirstScreenDelay"])) / a.Count() });
                var diczb = gobj.GroupBy(a => new { content = a.content, time = a.time }).Select(a => new Dictionary<string, object>() { { "PCONTENT", a.Key.content }, { "CTIME", a.Key.time }, { "PAGEOPENDELAY", a.Sum(b => b.popen) }, { "FIRSTBYTEDELAY", a.Sum(b => b.pfstbyte) }, { "FIRSTSCREENDELAY", a.Sum(b => b.pfstscreen) }, { "PAGEOPENDELAY_COUNT", a.Count() }, { "FIRSTBYTEDELAY_COUNT", a.Count() }, { "FIRSTSCREENDELAY_COUNT", a.Count() } }).ToDictionary(a => a["PCONTENT"] + "|" + ((DateTime)a["CTIME"]).ToString("yyyyMMdd"), b => b);
                dicswebcontent[dt.TableName] = diczb;
            }
            foreach (DataTable dt in dsvideo.Tables)
            {
                var gobj = dt.AsEnumerable().GroupBy(a => new { ci = a["NetInfo_LteCi"], time = O2.O2DT(a["TestResult_VideoTestTime"].ToString()).Date }).Select(a => new { ci = a.Key.ci, time = a.Key.time, content = a.Max(b => b["TestResult_VideoName"]), vspeed = a.Sum(b => O2.O2D(b["TestResult_VideoAvgSpeed"])) / a.Count(), vcache = a.Sum(b => O2.O2D(b["TestResult_CacheRate"])) / a.Count() });
                var diczb = gobj.GroupBy(a => new { content = a.content, time = a.time }).Select(a => new Dictionary<string, object>() { { "PCONTENT", a.Key.content }, { "CTIME", a.Key.time }, { "VIDEOAVGSPEED", a.Sum(b => b.vspeed) }, { "CACHERATE", a.Sum(b => b.vcache) }, { "VIDEOAVGSPEED_COUNT", a.Count() }, { "CACHERATE_COUNT", a.Count() } }).ToDictionary(a => a["PCONTENT"] + "|" + ((DateTime)a["CTIME"]).ToString("yyyyMMdd"), b => b);
                dicsvideocontent[dt.TableName] = diczb;
            }
            foreach (DataTable dt in dsim.Tables)
            {
                var gobj = dt.AsEnumerable().GroupBy(a => new { ci = a["NetInfo_LteCi"], time = O2.O2DT(a["TestResult_ImTestTime"].ToString()).Date }).Select(a => new { ci = a.Key.ci, time = a.Key.time, content = a.Max(b => b["TestResult_ImName"]), irate = a.Sum(b => O2.O2D(b["TestResult_ImSendCount"]) * O2.O2D(b["TestResult_ImSendRate"])) / a.Sum(b => O2.O2D(b["TestResult_ImSendCount"])) });
                var diczb = gobj.GroupBy(a => new { content = a.content, time = a.time }).Select(a => new Dictionary<string, object>() { { "PCONTENT", a.Key.content }, { "CTIME", a.Key.time }, { "IMSENDRATE", a.Sum(b => b.irate) }, { "IMSENDRATE_COUNT", a.Count() } }).ToDictionary(a => a["PCONTENT"] + "|" + ((DateTime)a["CTIME"]).ToString("yyyyMMdd"), b => b);
                dicsimcontent[dt.TableName] = diczb;
            }
            foreach (DataTable dt in dsgame.Tables)
            {
                var gobj = dt.AsEnumerable().GroupBy(a => new { ci = a["NetInfo_LteCi"], time = O2.O2DT(a["TestResult_gameTestTime"].ToString()).Date }).Select(a => new { ci = a.Key.ci, time = a.Key.time, content = a.Max(b => b["TestResult_gameName"]), gdelay = a.Sum(b => O2.O2D(b["TestResult_AckDelay"])) / a.Count() });
                var diczb = gobj.GroupBy(a => new { content = a.content, time = a.time }).Select(a => new Dictionary<string, object>() { { "PCONTENT", a.Key.content }, { "CTIME", a.Key.time }, { "ACKDELAY", a.Sum(b => b.gdelay) }, { "ACKDELAY_COUNT", a.Count() } }).ToDictionary(a => a["PCONTENT"] + "|" + ((DateTime)a["CTIME"]).ToString("yyyyMMdd"), b => b);
                dicsgamecontent[dt.TableName] = diczb;
            }
            var dicscontent = new Dictionary<string, Dictionary<string, object>>();
            foreach (var dic in dicswebcontent)
            {
                foreach (var dic2 in dic.Value)
                {
                    var key = dic2.Key + "|" + "web" + "|" + dic.Key;
                    if (!dicscontent.ContainsKey(key))
                    {
                        dicscontent[key] = new Dictionary<string, object>();
                    }
                    foreach (var dic3 in dic2.Value)
                    {
                        dicscontent[key][dic3.Key] = dic3.Value;
                    }
                    dicscontent[key]["OPERATOR"] = dic.Key;
                    dicscontent[key]["PTYPE"] = "web";
                }
            }
            foreach (var dic in dicsvideocontent)
            {
                foreach (var dic2 in dic.Value)
                {
                    var key = dic2.Key + "|" + "video" + "|" + dic.Key;
                    if (!dicscontent.ContainsKey(key))
                    {
                        dicscontent[key] = new Dictionary<string, object>();
                    }
                    foreach (var dic3 in dic2.Value)
                    {
                        dicscontent[key][dic3.Key] = dic3.Value;
                    }
                    dicscontent[key]["OPERATOR"] = dic.Key;
                    dicscontent[key]["PTYPE"] = "video";
                }
            }
            foreach (var dic in dicsimcontent)
            {
                foreach (var dic2 in dic.Value)
                {
                    var key = dic2.Key + "|" + "im" + "|" + dic.Key;
                    if (!dicscontent.ContainsKey(key))
                    {
                        dicscontent[key] = new Dictionary<string, object>();
                    }
                    foreach (var dic3 in dic2.Value)
                    {
                        dicscontent[key][dic3.Key] = dic3.Value;
                    }
                    dicscontent[key]["OPERATOR"] = dic.Key;
                    dicscontent[key]["PTYPE"] = "im";
                }
            }
            foreach (var dic in dicsgamecontent)
            {
                foreach (var dic2 in dic.Value)
                {
                    var key = dic2.Key + "|" + "game" + "|" + dic.Key;
                    if (!dicscontent.ContainsKey(key))
                    {
                        dicscontent[key] = new Dictionary<string, object>();
                    }
                    foreach (var dic3 in dic2.Value)
                    {
                        dicscontent[key][dic3.Key] = dic3.Value;
                    }
                    dicscontent[key]["OPERATOR"] = dic.Key;
                    dicscontent[key]["PTYPE"] = "game";
                }
            }

            #endregion
            #region 指标入库
            //dicscity,dicscontent
            var dtcity = new DataTable("PERDATA_CITY");
            var dtcontent = new DataTable("PERDATA_CONTENT");
            foreach (var dic in dicscity)
            {
                var row = dtcity.NewRow();
                foreach (var dic2 in dic.Value)
                {
                    if (!dtcity.Columns.Contains(dic2.Key))
                    {
                        var dtype = dic2.Value.GetType();
                        if (dtype == typeof(double))
                        {
                            dtype = typeof(decimal);
                        }
                        dtcity.Columns.Add(dic2.Key, dtype);
                    }
                    row[dic2.Key] = dic2.Value;
                }
                dtcity.Rows.Add(row);
            }
            foreach (var dic in dicscontent)
            {
                var row = dtcontent.NewRow();
                foreach (var dic2 in dic.Value)
                {
                    if (!dtcontent.Columns.Contains(dic2.Key))
                    {
                        var dtype = dic2.Value.GetType();
                        if (dtype == typeof(double))
                        {
                            dtype = typeof(decimal);
                        }
                        dtcontent.Columns.Add(dic2.Key, dtype);
                    }
                    row[dic2.Key] = dic2.Value;
                }
                dtcontent.Rows.Add(row);
            }

            if (dtcity.Rows.Count > 0)
            {
                DB.Exec("delete PERDATA_CITY where CTIME>=to_date('" + dtyue.ToString("yyyyMMdd") + "','yyyymmdd')");
                DB.ImportDt(dtcity);
            }
            if (dtcontent.Rows.Count > 0)
            {
                DB.Exec("delete PERDATA_CONTENT where CTIME>=to_date('" + dtyue.ToString("yyyyMMdd") + "','yyyymmdd')");
                DB.ImportDt(dtcontent);
            }
            #endregion
            return null;
        }
        public override bool ImportDB(DataTable dt, bool bol)
        {
            //do nothing
            return true;
        }
        public override void DeleteTmp()
        {
            //do nothing
        }

        private DataSet toDics2Ds(Dictionary<string, Dictionary<string, Dictionary<string, object>>> dics, int type)
        {
            DataSet ds = new DataSet();
            foreach (var dic in dics)
            {
                DataTable dt = new DataTable(dic.Key);
                foreach (var row in dic.Value)
                {
                    var dr = dt.NewRow();
                    foreach (var cel in row.Value)
                    {
                        if (!dt.Columns.Contains(cel.Key))
                        {
                            var tp = getColType(cel.Key);
                            if (tp == typeof(int))
                            {
                                tp = typeof(long);
                            }
                            dt.Columns.Add(cel.Key, tp);
                        }
                        dr[cel.Key] = cel.Value.ToString() == "" ? DBNull.Value : cel.Value;
                    }
                    dt.Rows.Add(dr);
                }
                dt.AcceptChanges();
                ds.Tables.Add(dt);
            }
            return ds;
        }

        private Type getColType(string colname)
        {
            Type tp = typeof(string);
            if (colname.ToLower().IndexOf("time") >= 0)
            {
                tp = typeof(DateTime);
            }
            if (colname.ToLower().IndexOf("delay") >= 0 || colname.ToLower().IndexOf("traffic") >= 0 || colname.ToLower().IndexOf("count") >= 0 || colname.ToLower().IndexOf("wugao") >= 0)
            {
                tp = typeof(long);
            }
            if (colname.ToLower().IndexOf("rate") >= 0 || colname.ToLower().IndexOf("speed") >= 0)
            {
                tp = typeof(double);
            }
            return tp;
        }

        private Dictionary<int, object> readwugaoDic()
        {
            var res = new Dictionary<int, object>();
            var wugao=cfg.custom["wugaopath"];
            StreamReader sr = new StreamReader(wugao,Encoding.GetEncoding("gbk"));
            while (!sr.EndOfStream) {
                var line = sr.ReadLine();
                var cells = line.Split(new string[]{","},StringSplitOptions.None);
                if (cells.Length < 3) continue;
                res[O2.O2I(cells[1])] = null;
            }
            sr.Close();
            sr.Dispose();
            return res;
        }
    }
}
