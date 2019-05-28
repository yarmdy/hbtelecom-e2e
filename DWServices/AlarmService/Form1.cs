using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace AlarmService
{
    public partial class Form1 : Form
    {
        #region 成员
        Dictionary<int, Dictionary<string, object>> gc = new Dictionary<int, Dictionary<string, object>>();
        Dictionary<int, Dictionary<string, int>> gcrru = new Dictionary<int, Dictionary<string, int>>();
        Dictionary<string, int> gcname = new Dictionary<string, int>();
        //Dictionary<int, int> fecis = new Dictionary<int, int>();
        Dictionary<int, int> nokiaeci = new Dictionary<int, int>();
        List<int> gczbpx = null;
        Dictionary<int, Dictionary<string, object>> kpi = new Dictionary<int, Dictionary<string, object>>();
        Dictionary<string, object> kpinull = new Dictionary<string, object>();
        Dictionary<int, Dictionary<int, Dictionary<string, object>>> alarm = new Dictionary<int, Dictionary<int, Dictionary<string, object>>>();
        Dictionary<int, Dictionary<int, Dictionary<string, object>>> alarmenb = new Dictionary<int, Dictionary<int, Dictionary<string, object>>>();
        Dictionary<int, Dictionary<int, string>> alarmCache = new Dictionary<int, Dictionary<int, string>>();

        Dictionary<int, Dictionary<string, object>> althw = new Dictionary<int, Dictionary<string, object>>();
        Dictionary<int, Dictionary<string, object>> altnk = new Dictionary<int, Dictionary<string, object>>();
        Dictionary<int, Dictionary<string, object>> altzt = new Dictionary<int, Dictionary<string, object>>();

        ArrayList actlist = new ArrayList();

        AutoResetEvent are = new AutoResetEvent(true);
        AutoResetEvent are2 = new AutoResetEvent(false);

        ArrayList bitlist = new ArrayList();
        AutoResetEvent arebit = new AutoResetEvent(true);
        ManualResetEvent arebit2 = new ManualResetEvent(false);

        Dictionary<int, Dictionary<string, object>> kpiyest = new Dictionary<int, Dictionary<string, object>>();
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> quota = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        Dictionary<int, Dictionary<string, object>> mr = new Dictionary<int, Dictionary<string, object>>();
        #endregion
        
        /// <summary>
        /// 初始化
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            gcread();
            Thread thData = new Thread(new ThreadStart(dataRead));
            thData.Start();
            Thread thDataDay = new Thread(new ThreadStart(dataReadDay));
            thDataDay.Start();
            Thread thSocket = new Thread(new ThreadStart(listen));
            thSocket.Start();
            int thcount = 30;
            for (int i = 0; i < thcount; i++) {
                Thread thsend = new Thread(new ThreadStart(send));
                thsend.Start();
            }
            int thbitcount = 1;
            for (int i = 0; i < thbitcount; i++)
            {
                Thread thbit = new Thread(new ThreadStart(probitpix));
                thbit.Start();
            }
            Thread thWX = new Thread(new ThreadStart(GetDataClass.LoadData));
            thWX.Start();

            Thread thTN = new Thread(new ThreadStart(TNGoodRate.LoadData));
            thTN.Start();
        }
        #region 数据读取
        #region 初始化读取
        void gcread()
        {
            StreamReader sr = new StreamReader(gcpath, Encoding.GetEncoding("gbk"));
            var tt = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var cells = line.Split(new string[] { "," }, StringSplitOptions.None);
                var eci = O2.O2I(cells[1]);
                if (eci == 0) continue;
                if (gc.ContainsKey(eci)) continue;
                if (O2.O2D(cells[6]) == 0 || O2.O2D(cells[7]) == 0) continue;
                Dictionary<string, object> gci = new Dictionary<string, object>();
                gci["eci"] = eci;
                gci["enbid"] = O2.O2I(cells[0]);
                gci["cellid"] = O2.O2I(cells[2]);
                gci["fid"] = O2.O2I(cells[24]);
                gci["scname"] = cells[5];
                gci["city"] = cells[3];
                gci["lon"] = O2.O2D(cells[6]);
                gci["lat"] = O2.O2D(cells[7]);
                gci["angle"] = O2.O2D(cells[11]);
                gci["zangle"] = O2.O2D(cells[12]);
                gci["dangle"] = O2.O2D(cells[14]);

                gc[eci] = gci;
            }
            sr.Close();
            sr.Dispose();
            gc = gc.OrderBy(a => (double)a.Value["lat"] * 1000000 + (double)a.Value["lon"]).ToDictionary(a => a.Key, a => a.Value);
            gczbpx = gc.Keys.ToList();

            kpinull["rrc"] = "null";
            kpinull["upflow"] = "null";
            kpinull["downflow"] = "null";
            kpinull["upprb"] = "null";
            kpinull["downprb"] = "null";
            kpinull["erab"] = "null";
            kpinull["cqi"] = "null";

            var altlines = File.ReadAllLines(altpath, Encoding.GetEncoding("gbk"));
            foreach (var line in altlines)
            {
                var cells = line.Split(new string[] { "," }, StringSplitOptions.None);
                switch (cells[2].Trim())
                {
                    case "华为":
                        althw[O2.O2I(cells[0])] = new Dictionary<string, object>();
                        althw[O2.O2I(cells[0])]["no"] = O2.O2I(cells[0]);
                        althw[O2.O2I(cells[0])]["name"] = cells[1];
                        althw[O2.O2I(cells[0])]["cj"] = cells[2];
                        althw[O2.O2I(cells[0])]["lvl"] = cells[3];
                        break;
                    case "诺基亚":
                        altnk[O2.O2I(cells[0])] = new Dictionary<string, object>();
                        altnk[O2.O2I(cells[0])]["no"] = O2.O2I(cells[0]);
                        altnk[O2.O2I(cells[0])]["name"] = cells[1];
                        altnk[O2.O2I(cells[0])]["cj"] = cells[2];
                        altnk[O2.O2I(cells[0])]["lvl"] = cells[3];
                        break;
                    case "中兴":
                        altzt[O2.O2I(cells[0])] = new Dictionary<string, object>();
                        altzt[O2.O2I(cells[0])]["no"] = O2.O2I(cells[0]);
                        altzt[O2.O2I(cells[0])]["name"] = cells[1];
                        altzt[O2.O2I(cells[0])]["cj"] = cells[2];
                        altzt[O2.O2I(cells[0])]["lvl"] = cells[3];
                        break;
                }
            }

            sr = new StreamReader(gcrrupath, Encoding.GetEncoding("gbk"));
            tt = sr.ReadLine();
            while (!sr.EndOfStream)
            {

                var line = sr.ReadLine();
                var cells = line.Split(new string[] { "," }, StringSplitOptions.None);
                var eci = O2.O2I(cells[0]);
                if (eci == 0) continue;
                if (!gc.ContainsKey(eci)) continue;
                var eci2 = O2.O2I(cells[1]);

                if (!gcrru.ContainsKey(eci2))
                {
                    gcrru[eci2] = new Dictionary<string, int>();
                }
                if (!gcrru[eci2].ContainsKey("num"))
                {
                    gcrru[eci2]["num"] = 0;
                }
                gcrru[eci2]["num"]++;
                gcrru[eci2][(gcrru[eci2]["num"] - 1) + ""] = eci;
            }
            sr.Close();
            sr.Dispose();

            foreach (var g in gc)
            {
                var scname = g.Value["scname"].ToString();
                var eci = (int)g.Value["eci"];
                gcname[scname] = eci;
            }
            sr = new StreamReader(nokiaecipath, Encoding.GetEncoding("gbk"));
            while (!sr.EndOfStream)
            {

                var line = sr.ReadLine();
                var cells = line.Split(new string[] { "," }, StringSplitOptions.None);
                nokiaeci[O2.O2I(cells[0]) * 256 + O2.O2I(cells[1])] = O2.O2I(cells[0]) * 256 + O2.O2I(cells[2]);

            }
            sr.Close();
            sr.Dispose();

            sr = new StreamReader(quotapath, Encoding.GetEncoding("gbk"));
            sr.ReadLine();
            while (!sr.EndOfStream)
            {

                var line = sr.ReadLine();
                var cells = line.Split(new string[] { "," }, StringSplitOptions.None);
                var key = cells[8];
                if (!quota.ContainsKey(key))
                {
                    quota[key] = new Dictionary<string, Dictionary<string, string>>();
                }
                var subproj = cells[1];
                if (subproj == "800M")
                {
                    subproj = "1";
                }
                else if (subproj == "1800M")
                {
                    subproj = "2";
                }
                else
                {
                    subproj = "0";
                }
                if (!quota[key].ContainsKey(subproj))
                {
                    quota[key][subproj] = new Dictionary<string, string>();
                }
                quota[key][subproj]["proj"] = cells[0];
                quota[key][subproj]["subproj"] = cells[1];
                quota[key][subproj]["name"] = cells[2];
                quota[key][subproj]["min"] = cells[3];
                quota[key][subproj]["max"] = cells[4];
                quota[key][subproj]["colname"] = cells[7];
                quota[key][subproj]["key"] = cells[8];
                quota[key][subproj]["direct"] = cells[9];
            }
            sr.Close();
            sr.Dispose();

            //GetDataClass.LoadData();
        }
        #endregion

        #region 小时读取
        void dataRead()
        {
            bool first = true;
            while (true)
            {
                if (DateTime.Now.Minute != 5 && !first)
                {
                    Thread.Sleep(30000);
                    continue;
                }
                first = false;
                try
                {
                    ///kpi处理
                    var kpifile = Directory.GetFiles(kpipath, "SH_PERF_CELL_L_*.csv").OrderByDescending(a => a).FirstOrDefault();
                    StreamReader sr = new StreamReader(kpifile, Encoding.GetEncoding("gbk"));
                    var tt = sr.ReadLine();
                    tt = sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var cells = line.Split(new string[] { "," }, StringSplitOptions.None);
                        int enbid = O2.O2I(cells[2]);
                        int cellid = O2.O2I(cells[4]);
                        if (enbid == 0) continue;
                        int eci = enbid * 256 + cellid;
                        if (!gc.ContainsKey(eci)) continue;
                        if (!kpi.ContainsKey(eci))
                        {
                            kpi[eci] = new Dictionary<string, object>();
                        }
                        Dictionary<string, object> kpii = kpi[eci];
                        kpii["rrc"] = O2.O2D(cells[13]);
                        kpii["upflow"] = O2.O2D(cells[11]);
                        kpii["downflow"] = O2.O2D(cells[10]);
                        kpii["upprb"] = O2.O2D(cells[7]);
                        kpii["downprb"] = O2.O2D(cells[8]);
                        kpii["erab"] = O2.O2D(cells[20]);
                        kpii["cqi"] = O2.O2D(cells[24]);
                        kpi[eci] = kpii;
                    }
                    sr.Close();
                    sr.Dispose();

                    ///alarm处理
                    alarmenb.Clear();
                    alarm.Clear();
                    var huawei = Directory.GetFiles(abcpath, "huawei_alarm_*.csv").OrderByDescending(a => a).FirstOrDefault();
                    var huaweic = Directory.GetFiles(defpath, "huawei_conf_*.csv").OrderByDescending(a => a).FirstOrDefault();

                    var nokia = Directory.GetFiles(abcpath, "nokia_alarm_*.csv").OrderByDescending(a => a).FirstOrDefault();
                    var nokiac = Directory.GetFiles(defpath, "nokia_conf_*.csv").OrderByDescending(a => a).FirstOrDefault();

                    var zte = Directory.GetFiles(abcpath, "zte_alarm_*.csv").OrderByDescending(a => a).FirstOrDefault();
                    var ztec = Directory.GetFiles(defpath, "zte_conf_*.csv").OrderByDescending(a => a).FirstOrDefault();

                    sr = new StreamReader(huaweic, Encoding.GetEncoding("gbk"));
                    var ttl = sr.ReadLine();
                    var ttr = ttl.Split(new char[] { '@' }, StringSplitOptions.None);
                    Dictionary<int, string> dichuawei = new Dictionary<int, string>();
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            var line = sr.ReadLine();
                            var cells = line.Split(new char[] { '@' }, StringSplitOptions.None);
                            dichuawei[O2.O2I(cells[1]) * 256 + O2.O2I(cells[5])] = cells[2].Trim();
                        }
                        catch { }
                    }

                    sr = new StreamReader(huawei, Encoding.GetEncoding("gbk"));
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            var line = sr.ReadLine();
                            var cells = line.Split(new char[] { '@' }, StringSplitOptions.None);
                            var alarmno = O2.O2I(cells[2]);
                            if (!althw.ContainsKey(alarmno)) continue;
                            var lastcell = cells[cells.Length - 2];
                            var enbid = O2.O2I(lastcell.Substring(lastcell.IndexOf("eNodeBId=") + "eNodeBId=".Length).Trim());
                            if (enbid == 0) continue;
                            int cellid = 0;
                            var lvl = althw[alarmno]["lvl"].ToString();
                            var eci = 0;
                            if (lvl == "BTS")
                            {
                                if (!alarmenb.ContainsKey(enbid))
                                {
                                    alarmenb[enbid] = new Dictionary<int, Dictionary<string, object>>();
                                }
                                if (!alarmenb[enbid].ContainsKey(alarmno))
                                {
                                    alarmenb[enbid][alarmno] = new Dictionary<string, object>();
                                }
                                alarmenb[enbid][alarmno]["info"] = cells[3];
                                continue;
                            }
                            else if (lvl == "CELL")
                            {
                                var houtxt = cells[6].Substring(cells[6].IndexOf("小区标识=") + "小区标识=".Length);
                                houtxt = houtxt.Substring(houtxt.IndexOf("小区标识=") + "小区标识=".Length);
                                cellid = O2.O2I(houtxt.Substring(0, houtxt.IndexOf(",") < 0 ? houtxt.Length : houtxt.IndexOf(",")));
                                if (cellid == 0) continue;
                                eci = enbid * 256 + cellid;
                                if (!gc.ContainsKey(eci)) continue;
                                if (!alarm.ContainsKey(eci))
                                {
                                    alarm[eci] = new Dictionary<int, Dictionary<string, object>>();
                                }
                                if (!alarm[eci].ContainsKey(alarmno))
                                {
                                    alarm[eci][alarmno] = new Dictionary<string, object>();
                                }
                                alarm[eci][alarmno]["info"] = cells[3];
                            }
                            else if (lvl == "RRU")
                            {
                                var houtxt = cells[5].Substring(cells[5].IndexOf("框号=") + "框号=".Length);
                                var rack = O2.O2I(houtxt.Substring(0, houtxt.IndexOf(",") < 0 ? houtxt.Length : houtxt.IndexOf(",")));
                                if (rack == 0) continue;
                                var ecirru = enbid * 256 + rack;
                                if (!dichuawei.ContainsKey(ecirru)) continue;
                                if (!gcname.ContainsKey(dichuawei[ecirru])) continue;
                                eci = gcname[dichuawei[ecirru]];
                                if (!alarm.ContainsKey(eci))
                                {
                                    alarm[eci] = new Dictionary<int, Dictionary<string, object>>();
                                }
                                if (!alarm[eci].ContainsKey(alarmno))
                                {
                                    alarm[eci][alarmno] = new Dictionary<string, object>();
                                }
                                alarm[eci][alarmno]["info"] = cells[3];
                            }
                        }
                        catch { continue; }
                    }

                    // zx
                    sr = new StreamReader(zte, Encoding.GetEncoding("gbk"));
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            var line = sr.ReadLine();
                            var cells = line.Split(new char[] { '@' }, StringSplitOptions.None);
                            var alarmno = O2.O2I(cells[6]);
                            if (!altzt.ContainsKey(alarmno)) continue;
                            var enbcells = cells[3].Split(',');
                            var enbid = O2.O2I(enbcells[0].Substring(enbcells[0].IndexOf("NodeMe=") + "NodeMe=".Length));
                            if (enbid == 0) continue;
                            int cellid = 0;
                            var lvl = altzt[alarmno]["lvl"].ToString();
                            var eci = 0;
                            if (lvl == "BTS")
                            {
                                if (!alarmenb.ContainsKey(enbid))
                                {
                                    alarmenb[enbid] = new Dictionary<int, Dictionary<string, object>>();
                                }
                                if (!alarmenb[enbid].ContainsKey(alarmno))
                                {
                                    alarmenb[enbid][alarmno] = new Dictionary<string, object>();
                                }
                                alarmenb[enbid][alarmno]["info"] = cells[7];
                                continue;
                            }
                            else if (lvl == "CELL")
                            {
                                var houtxt = cells[11].Substring(cells[11].IndexOf("小区标识:") + "小区标识=".Length);
                                cellid = O2.O2I(houtxt.Substring(0, houtxt.IndexOf(";") < 0 ? houtxt.Length : houtxt.IndexOf(";")));
                                if (cellid == 0) continue;
                                eci = enbid * 256 + cellid;
                                if (!gc.ContainsKey(eci)) continue;
                                if (!alarm.ContainsKey(eci))
                                {
                                    alarm[eci] = new Dictionary<int, Dictionary<string, object>>();
                                }
                                if (!alarm[eci].ContainsKey(alarmno))
                                {
                                    alarm[eci][alarmno] = new Dictionary<string, object>();
                                }
                                alarm[eci][alarmno]["info"] = cells[7];

                            }
                            else if (lvl == "RRU")
                            {
                                var rackcells = cells[11];
                                var rack = O2.O2I(rackcells.Substring(rackcells.IndexOf("小区ID=") + "小区ID=".Length));
                                if (rack == 0) continue;
                                eci = enbid * 256 + rack;
                                if (!gc.ContainsKey(eci)) continue;
                                if (!alarm.ContainsKey(eci))
                                {
                                    alarm[eci] = new Dictionary<int, Dictionary<string, object>>();
                                }
                                if (!alarm[eci].ContainsKey(alarmno))
                                {
                                    alarm[eci][alarmno] = new Dictionary<string, object>();
                                }
                                alarm[eci][alarmno]["info"] = cells[7];
                            }
                        }
                        catch
                        {
                            continue;
                        }

                    }
                    //nk 
                    sr = new StreamReader(nokia, Encoding.GetEncoding("gbk"));
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            var line = sr.ReadLine();
                            var cells = line.Split(new char[] { '@' }, StringSplitOptions.None);
                            var alarmno = O2.O2I(cells[4]);
                            if (!altnk.ContainsKey(alarmno)) continue;
                            var lastcell = cells[cells.Length - 1];
                            var enbid = O2.O2I(cells[7]);
                            if (enbid == 0) continue;
                            int cellid = 0;
                            var lvl = altnk[alarmno]["lvl"].ToString();
                            var eci = 0;
                            if (lvl == "BTS")
                            {
                                if (!alarmenb.ContainsKey(enbid))
                                {
                                    alarmenb[enbid] = new Dictionary<int, Dictionary<string, object>>();
                                }
                                if (!alarmenb[enbid].ContainsKey(alarmno))
                                {
                                    alarmenb[enbid][alarmno] = new Dictionary<string, object>();
                                }
                                alarmenb[enbid][alarmno]["info"] = cells[6];
                                continue;
                            }
                            else if (lvl == "CELL")
                            {
                                cellid = O2.O2I(cells[cells.Length - 1]);
                                if (!nokiaeci.ContainsKey(cellid + enbid * 256)) continue;
                                eci = nokiaeci[cellid + enbid * 256];
                                if (!gc.ContainsKey(eci)) continue;
                                if (!alarm.ContainsKey(eci))
                                {
                                    alarm[eci] = new Dictionary<int, Dictionary<string, object>>();
                                }
                                if (!alarm[eci].ContainsKey(alarmno))
                                {
                                    alarm[eci][alarmno] = new Dictionary<string, object>();
                                }
                                alarm[eci][alarmno]["info"] = cells[5];
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    alarmCache.Clear();
                    foreach (var d in gczbpx)
                    {
                        bool isAlarm = alarmenb.ContainsKey(O2.O2I(gc[d]["enbid"]));
                        isAlarm = isAlarm || alarm.ContainsKey(d);
                        string nbmsg = "基站告警：";
                        string msg = "小区告警：";
                        if (isAlarm)
                        {
                            if (alarmenb.ContainsKey(O2.O2I(gc[d]["enbid"])))
                            {
                                foreach (var a in alarmenb[O2.O2I(gc[d]["enbid"])])
                                {
                                    nbmsg += a.Value["info"] + " ; ";
                                }
                            }
                            if (alarm.ContainsKey(d))
                            {
                                foreach (var a in alarm[d])
                                {
                                    msg += a.Value["info"] + " ; ";
                                }
                            }
                        }
                        alarmCache[d] = new Dictionary<int, string>();
                        alarmCache[d][0] = isAlarm ? "1" : "0";
                        alarmCache[d][1] = (alarmenb.ContainsKey(O2.O2I(gc[d]["enbid"])) ? nbmsg : "") + (alarm.ContainsKey(d) ? msg : "");
                    }
                }
                catch { }
            }
        }
        #endregion

        #region 天读取
        object S2D(string v)
        {
            if ((v + "").Trim() == "")
            {
                return null;
            }
            else
            {
                return O2.O2D(v);
            }
        }
        object S2D100(string v)
        {
            if ((v + "").Trim() == "")
            {
                return null;
            }
            else
            {
                return O2.O2D(v) / 100;
            }
        }
        void kpiadd(Dictionary<string, object> kpii, string name, string v, bool min, bool bf = false)
        {
            object vt = null;
            var v1 = bf ? S2D100(v) : S2D(v);
            object v2 = kpii.ContainsKey(name) ? kpii[name] : null;
            if (v2 == null || v1 == null)
            {
                vt = v1 == null ? v2 : v1;
            }
            else if (min)
            {
                vt = (double)v1 < (double)v2 ? v1 : v2;
            }
            else
            {
                vt = (double)v1 > (double)v2 ? v1 : v2;
            }
            kpii[name] = vt;
        }
        bool todaymr = false;
        void dataReadDay()
        {
            bool first = true;
            while (true)
            {
                var dtnw = DateTime.Now;
                if ((dtnw.Minute != 5 || dtnw.Hour != 8) && !first)
                {
                    Thread.Sleep(30000);
                    continue;
                }
                first = false;
                try
                {
                    StreamReader sr = null;
                    ///kpi处理
                    Dictionary<int, Dictionary<string, object>> kpiyestt = new Dictionary<int, Dictionary<string, object>>();
                    var kpifiles = Directory.GetFiles(kpipath, "SH_PERF_CELL_L_" + dtnw.AddDays(-1).ToString("yyyyMMdd") + "*.csv");
                    foreach (var kpifile in kpifiles)
                    {
                        sr = new StreamReader(kpifile, Encoding.GetEncoding("gbk"));
                        var tt = sr.ReadLine();
                        tt = sr.ReadLine();
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var cells = line.Split(new string[] { "," }, StringSplitOptions.None);
                            if (cells.Length < 35) continue;
                            int enbid = O2.O2I(cells[2]);
                            int cellid = O2.O2I(cells[4]);
                            if (enbid == 0) continue;
                            int eci = enbid * 256 + cellid;
                            //if (!gc.ContainsKey(eci)) continue;
                            if (!kpiyestt.ContainsKey(eci))
                            {
                                kpiyestt[eci] = new Dictionary<string, object>();
                            }
                            Dictionary<string, object> kpii = kpiyestt[eci];

                            kpii["eci"] = eci;
                            kpiadd(kpii, "rrcljcgl", cells[13], true, true);
                            kpiadd(kpii, "rrcsbcs", cells[14], false);
                            kpiadd(kpii, "xhysl", cells[15], false);
                            kpiadd(kpii, "xhyscs", cells[31], false);
                            kpiadd(kpii, "rrcljcjbl", cells[16], false);
                            kpiadd(kpii, "rrcljcjcgcs", cells[17], false);
                            kpiadd(kpii, "uesxwdxl", cells[18], false);
                            kpiadd(kpii, "uesxycsfcs", cells[19], false);
                            kpiadd(kpii, "erabdxl", cells[20], false);
                            kpiadd(kpii, "erabdxcs", cells[21], false);
                            kpiadd(kpii, "rrccjbl", cells[22], false);
                            kpiadd(kpii, "rrccjqqcs", cells[23], false);
                            kpiadd(kpii, "xtnqhcgl", cells[30], true);
                            kpiadd(kpii, "xtnqsbcs", cells[32], false);
                            kpiadd(kpii, "cqibl", cells[24], true);
                            kpiadd(kpii, "xxprbslzb", cells[25], true);
                            kpiadd(kpii, "yhtysxpjsl", cells[26], true);
                            kpiadd(kpii, "yhtyxxpjsl", cells[27], true);

                            kpiadd(kpii, "sxprbpjlyl18", cells[7], false);
                            kpiadd(kpii, "xxprbpjlyl18", cells[8], false);
                            kpiadd(kpii, "pjrrcljyhs18", cells[9], false);
                            kpiadd(kpii, "pdcpxx18", cells[10], false);
                            kpiadd(kpii, "pdcpsx18", cells[11], false);
                            kpiadd(kpii, "pjjhyhs18", cells[12], false);

                            kpiyestt[eci] = kpii;
                        }
                        sr.Close();
                        sr.Dispose();
                    }
                    kpiyest = kpiyestt;

                    //mr
                    Dictionary<int, Dictionary<string, object>> mrt = new Dictionary<int, Dictionary<string, object>>();
                    
                    string[] mrfiles = null;
                    string mrfile = null;
                    ftpHelper ftp = new ftpHelper();
                    ftp.fsv = new FileServer();
                    try
                    {
                        mrfiles = ftp.filelist;
                        mrfile = mrfiles.FirstOrDefault(a => a.Contains("_" + dtnw.AddDays(-1).ToString("yyyyMMdd") + "_") && a.Contains(".csv"));
                    }
                    catch { }

                    var mrtmp = System.AppDomain.CurrentDomain.BaseDirectory + @"\mrtmp\";
                    var mrtmpfile = "";
                    if (mrfile != null)
                    {
                        mrtmpfile = System.AppDomain.CurrentDomain.BaseDirectory + @"\mrtmp\" + mrfile;
                        if (!Directory.Exists(mrtmp))
                        {
                            Directory.CreateDirectory(mrtmp);
                        }
                        ftp.download(new string[] { mrfile }, new string[] { mrtmpfile });
                    }
                    else {
                        mrfile = Directory.GetFiles(mrfilepath,"*.csv").OrderByDescending(a => a).FirstOrDefault();
                        mrtmpfile = System.AppDomain.CurrentDomain.BaseDirectory + @"\mrtmp\" + mrfile.Substring(mrfile.LastIndexOf("\\")+1);
                        File.Copy(mrfile,mrtmpfile,true);
                    }
                    sr = new StreamReader(mrtmpfile, Encoding.GetEncoding("gbk"));
                    var tt2 = sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var cells = line.Split(new string[] { "," }, StringSplitOptions.None);
                        if (cells.Length < 26) continue;
                        int enbid = O2.O2I(cells[4]);
                        int cellid = O2.O2I(cells[5]);
                        if (enbid == 0) continue;
                        int eci = enbid * 256 + cellid;
                        if (!mrt.ContainsKey(eci))
                        {
                            mrt[eci] = new Dictionary<string, object>();
                        }
                        Dictionary<string, object> mrr = mrt[eci];
                        mrr["eci"] = eci;
                        kpiadd(mrr, "xqpjrsrp", cells[8], true);
                        kpiadd(mrr, "rfgbl", cells[15], true);
                        kpiadd(mrr, "pjjl", cells[24], false);
                        kpiadd(mrr, "cdfgk", cells[17], false);
                        kpiadd(mrr, "gfglqgs", cells[20], false);
                        kpiadd(mrr, "modgrcd", cells[21], false);

                        kpiadd(mrr, "TADV_COUNT_SUM", cells[25], false);

                        mrt[eci] = mrr;
                    }
                    mr = mrt;
                    sr.Close();
                    sr.Dispose();
                    File.Delete(mrtmpfile);
                }
                catch
                {
                    int j = 0;
                }
            }
        }
        #endregion
        
        #endregion

        #region socket相关
        Socket s = null;
        void listen()
        {
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 3390);
            s.Bind(ipep);
            s.Listen(200);
            while (true)
            {
                req rq = new req();
                try
                {
                    //byte[] re = new byte[1024];
                    Socket client = s.Accept();
                    //int len = client.Receive(re, 48, SocketFlags.None);
                    //if (len < 48) { continue; }

                    //MemoryStream ms = new MemoryStream(re);
                    //BinaryReader br = new BinaryReader(ms);

                    //rq.ACT = br.ReadInt32();
                    //rq.WIDTH = br.ReadInt32();
                    //rq.HEIGHT = br.ReadInt32();
                    //rq.BBOXx = br.ReadDouble();
                    //rq.BBOXy = br.ReadDouble();
                    //rq.BBOXx2 = br.ReadDouble();
                    //rq.BBOXy2 = br.ReadDouble();
                    //rq.CRS = br.ReadInt32();
                    //br.Close();
                    rq.EP = client;
                }
                catch (Exception ex)
                {
                    var err = ex.Message;
                    continue;
                }
                are.WaitOne();
                actlist.Add(rq);
                are.Set();
                are2.Set();
            }
        }
        int bufferlen = 1024 * 1024;
        void send(Socket skt, byte[] data)
        {
            var len = data.Length;
            var offset = 0;
            while (offset < len)
            {
                var falen = Math.Min(len - offset, bufferlen);
                var slen = skt.Send(data, offset, falen, SocketFlags.None);
                offset += slen;
            }
        }
        void send()
        {
            while (true)
            {
                try
                {
                    are.WaitOne();
                    var rq = lastreq;
                    are.Set();
                    if (rq == null)
                    {
                        are2.WaitOne();
                        continue;
                    }
                    byte[] re = new byte[48];
                    Receive(rq.EP,re);
                    MemoryStream ms = new MemoryStream(re);
                    BinaryReader br = new BinaryReader(ms);

                    rq.ACT = br.ReadInt32();
                    rq.WIDTH = br.ReadInt32();
                    rq.HEIGHT = br.ReadInt32();
                    rq.BBOXx = br.ReadDouble();
                    rq.BBOXy = br.ReadDouble();
                    rq.BBOXx2 = br.ReadDouble();
                    rq.BBOXy2 = br.ReadDouble();
                    rq.CRS = br.ReadInt32();
                    br.Close();
                    byte[] data = getSendData(rq);
                    var datalen = data == null ? 0 : data.Length;
                    //rq.EP.Send(BitConverter.GetBytes(datalen));
                    //byte[] xy=new byte[1];
                    //rq.EP.Receive(xy);
                    //for (int i = 0; i < datalen; i += 1024*10)
                    //{
                    //    int count = Math.Min(1024*10, datalen - i);
                    //    int sendlen= rq.EP.Send(data, i, count, SocketFlags.None);
                    //    if (sendlen != count)
                    //    {
                    //        int o = 0;
                    //    }
                    //    if (datalen - i > 1024*10||true)
                    //    {
                    //        //rq.EP.Receive(xy);
                    //    }
                    //}

                    var lendata = BitConverter.GetBytes(datalen);
                    send(rq.EP, lendata);
                    send(rq.EP, data);
                    //rq.EP.Shutdown(SocketShutdown.Both);
                    //rq.EP.Close();
                }
                catch (Exception ex)
                {
                    var err = ex.Message;
                }

            }
        }

        byte[] getSendData(req rq)
        {
            byte[] data = null;
            if (rq.ACT == 1)
            {
                data = getMapDate(rq);
            }
            else if (rq.ACT == 2)
            {
                data = getFinDate(rq);
            }
            else if (rq.ACT == 3)
            {
                data = getCityDate(rq);
            }
            else if (rq.ACT == 4)
            {
                data = getPos(rq);
            }
            else if (rq.ACT == 101)
            {
                data = getZHICHA(rq);
            }
            else if (rq.ACT > 9 && rq.ACT < 40) {
                data = GetDataClass.GetWxfgdata(rq);
            }
            else if (rq.ACT >=40 && rq.ACT < 100)
            {
                data = TNGoodRate.GetTNGoodData(rq);
            }
            return data;
        }
        #endregion

        #region 业务
        #region 地图业务
        byte[] getPos(req rq)
        {
            byte[] data = null;
            int enbid = rq.WIDTH;
            int cellid = rq.HEIGHT;
            int eci = enbid * 256 + cellid;
            if (!gc.ContainsKey(eci))
            {
                data = Encoding.GetEncoding("utf-8").GetBytes("{\"ok\":false,\"msg\":\"小区不存在\"}");
            }
            else
            {
                data = Encoding.GetEncoding("utf-8").GetBytes("{\"ok\":true,\"enb_lon\":" + gc[eci]["lon"] + ",\"enb_lat\":" + gc[eci]["lat"] + "}");
            }
            return data;
        }
        byte[] getMapDate(req rq)
        {
            var dt1 = DateTime.Now;
            byte[] data = null;
            var lon1 = rq.BBOXx;
            var lat1 = rq.BBOXy;
            var lon2 = rq.BBOXx2;
            var lat2 = rq.BBOXy2;
            if (rq.CRS == 1)
            {
                lon1 = lon1 / 20037508.34 * 180;
                lat1 = lat1 / 20037508.34 * 180;
                lat1 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat1 * Math.PI / 180)) - Math.PI / 2);

                lon2 = lon2 / 20037508.34 * 180;
                lat2 = lat2 / 20037508.34 * 180;
                lat2 = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat2 * Math.PI / 180)) - Math.PI / 2);
            }
            else
            {
                lon1 = rq.BBOXy;
                lat1 = rq.BBOXx;
                lon2 = rq.BBOXy2;
                lat2 = rq.BBOXx2;
            }
            var w = lon2 - lon1;//经度跨度
            var h = lat2 - lat1;//纬度跨度
            var wb = rq.WIDTH / w;//像素经度
            var hb = rq.HEIGHT / h;//像素纬度
            Rectangle rect = new Rectangle(0, 0, rq.WIDTH, rq.HEIGHT);//矩形位置和大小
            Bitmap image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
            Graphics ig = Graphics.FromImage(image);
            var wb2 = w / rq.WIDTH;
            var hb2 = h / rq.HEIGHT;
            Dictionary<int, double> dic = new Dictionary<int, double>();
            Dictionary<int, int> dic2 = new Dictionary<int, int>();

            Bitmap output = null;

            var lastLat = lat1;
            var minInd = 0;
            var maxInd = gczbpx.Count - 1;
            var tLen = maxInd;

            if (lat1 <= (double)gc[gczbpx[maxInd]]["lat"] && lat2 >= (double)gc[gczbpx[minInd]]["lat"])
            {
                while (lastLat < lat2)
                {
                    var minR = gc[gczbpx[minInd]];
                    var maxR = gc[gczbpx[maxInd]];
                    if (!(minInd == 0 && lastLat <= (double)minR["lat"] || minInd > 0 && lastLat < (double)minR["lat"] && lastLat >= (double)gc[gczbpx[minInd - 1]]["lat"]))
                    {
                        var midInd = (maxInd - minInd) / 2 + minInd;
                        var midR = gc[gczbpx[midInd]];
                        if (lastLat < (double)midR["lat"])
                        {
                            maxInd = midInd;
                            if (maxInd - minInd == 1)
                            {
                                minInd++;
                            }
                        }
                        else
                        {
                            minInd = midInd;
                            if (maxInd == minInd)
                                break;
                            if (maxInd - minInd == 1)
                            {
                                minInd++;
                            }

                        }
                    }
                    else
                    {
                        var finish = false;
                        for (int i = minInd; i < tLen + 1; i++)
                        {
                            var dr2 = gc[gczbpx[i]];
                            double lon = (double)(dr2["lon"]);
                            double lat = (double)(dr2["lat"]);
                            lastLat = lat;
                            if (lat < lat1 || lat > lat2)
                            {
                                finish = true;
                                break;
                            }
                            if (lon < lon1)
                            {
                                continue;
                            }
                            if (lon > lon2)
                            {
                                minInd = i;
                                maxInd = tLen;
                                break;
                            }
                            var xc = ((int)(float)((lon - lon1) * wb)) / 10;
                            var yc = ((int)(rq.HEIGHT - (float)((lat - lat1) * hb))) / 10;
                            bool isAlarm = alarmenb.ContainsKey(gczbpx[i] / 256);
                            isAlarm = isAlarm || alarm.ContainsKey(gczbpx[i]);
                            int val = !alarm.ContainsKey(gczbpx[i]) ? 0 : 1;
                            if (dic.ContainsKey(xc * 1000000 + yc))
                            {
                                dic[xc * 1000000 + yc] = val > dic[xc * 1000000 + yc] ? val : dic[xc * 1000000 + yc];
                                //dic2[xc * 1000000 + yc]++;
                                //dic[xc * 1000000 + yc] = (dic[xc * 1000000 + yc] * (dic2[xc * 1000000 + yc] - 1) + val) / dic2[xc * 1000000 + yc];
                            }
                            else
                            {
                                dic[xc * 1000000 + yc] = val;
                                //dic2[xc * 1000000 + yc] = 1;
                                //dic[xc * 1000000 + yc] = val;
                            }
                            if (i == tLen)
                            {
                                finish = true;
                            }
                        }
                        if (finish)
                        {
                            break;
                        }
                    }
                }
            }
            //Random rnd = new Random(DateTime.Now.Millisecond);
            if (w >= 0.04)
            {
                output = new Bitmap(rq.WIDTH, rq.HEIGHT, PixelFormat.Format32bppArgb);
                GraphicsPath pt = new GraphicsPath();
                int rad = 12;
                var x1 = 0.0002;
                var x2 = 0.2197;
                var y1 = 10.0;
                var y2 = 1000.0;

                var a = (y2 - y1) / (x2 - y1);
                var b = y1 - a * x1;
                var maxV = 1.0;

                foreach (var d in dic)
                {
                    var x10 = d.Key / 1000000;
                    var y10 = d.Key % 1000000;
                    Rectangle r = new Rectangle(x10 * 10 - rad, y10 * 10 - rad, rad * 2, rad * 2);


                    var bili = d.Value / maxV;
                    if (bili <= 0)
                    {
                        bili = 0.1;
                    }

                    var ellipsePath = new GraphicsPath();
                    ellipsePath.AddEllipse(r);
                    PathGradientBrush br = new PathGradientBrush(ellipsePath);
                    ColorBlend gradientSpecifications = getColorBlend(bili);
                    br.InterpolationColors = gradientSpecifications;
                    ig.FillEllipse(br, r);
                }

                //for (int x = 0; x < rq.WIDTH; x++)
                //{
                //    for (int y = 0; y < rq.HEIGHT; y++)
                //    {
                //        var alp = image.GetPixel(x, y).A;
                //        if (alp <= 0) continue;
                //        output.SetPixel(x, y, getARGBBrushC(alp / 255f));
                //    }
                //}

                Rectangle rectbit = new Rectangle(0, 0, rq.WIDTH, rq.HEIGHT);
                BitmapData imgData = image.LockBits(rectbit, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData outData = output.LockBits(rectbit, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                var ptrimg = imgData.Scan0;
                var ptrout = outData.Scan0;
                var imglen = imgData.Stride * rq.HEIGHT;
                var imgdata = new byte[imglen];
                var outlen = outData.Stride * rq.HEIGHT;
                var outdata = new byte[outlen];
                System.Runtime.InteropServices.Marshal.Copy(ptrimg, imgdata, 0, imglen);
                System.Runtime.InteropServices.Marshal.Copy(ptrout, outdata, 0, outlen);
                for (int y = 0; y < rq.HEIGHT; y++)
                {
                    for (int x = 0; x < rq.WIDTH; x++)
                    {
                        int ind = x * 4;
                        var alp = imgdata[y * imgData.Stride + ind + 3];
                        if (alp == 0) continue;

                        var outcolor = getARGBBrushC(alp / 255f);
                        outdata[y * outData.Stride + ind] = outcolor.B;
                        outdata[y * outData.Stride + ind + 1] = outcolor.G;
                        outdata[y * outData.Stride + ind + 2] = outcolor.R;
                        outdata[y * outData.Stride + ind + 3] = outcolor.A;
                    }
                }
                //SetPixBitmap(imgdata, outdata, rq.WIDTH,rq.HEIGHT);
                System.Runtime.InteropServices.Marshal.Copy(outdata, 0, ptrout, outlen);
                image.UnlockBits(imgData);
                output.UnlockBits(outData);


            }
            MemoryStream stream = new MemoryStream();
            if (w >= 0.04)
            {
                output.Save(stream, ImageFormat.Png);
            }
            else
            {
                image.Save(stream, ImageFormat.Png);
            }

            image.Save(stream, ImageFormat.Png);
            data = stream.ToArray();
            var times = (DateTime.Now - dt1).TotalMilliseconds;
            return data;
        }
        byte[] getFinDate(req rq)
        {
            byte[] data = null;

            List<int> resdic = new List<int>();
            var lon1 = rq.BBOXy;
            var lat1 = rq.BBOXx;
            var lon2 = rq.BBOXy2;
            var lat2 = rq.BBOXx2;

            var lastLat = lat1;
            var minInd = 0;
            var maxInd = gczbpx.Count - 1;
            var tLen = maxInd;

            if (lat1 <= (double)gc[gczbpx[maxInd]]["lat"] && lat2 >= (double)gc[gczbpx[minInd]]["lat"])
            {

                while (lastLat < lat2)
                {
                    var minR = gc[gczbpx[minInd]];
                    var maxR = gc[gczbpx[maxInd]];
                    if (!(minInd == 0 && lastLat <= (double)minR["lat"] || minInd > 0 && lastLat < (double)minR["lat"] && lastLat >= (double)gc[gczbpx[minInd - 1]]["lat"]))
                    {
                        var midInd = (maxInd - minInd) / 2 + minInd;
                        var midR = gc[gczbpx[midInd]];
                        if (lastLat < (double)midR["lat"])
                        {
                            maxInd = midInd;
                            if (maxInd - minInd == 1)
                            {
                                minInd++;
                            }
                        }
                        else
                        {
                            minInd = midInd;
                            if (maxInd == minInd)
                                break;
                            if (maxInd - minInd == 1)
                            {
                                minInd++;
                            }

                        }
                    }
                    else
                    {
                        var finish = false;
                        for (int i = minInd; i < tLen + 1; i++)
                        {
                            var dr2 = gc[gczbpx[i]];
                            double lon = (double)(dr2["lon"]);
                            double lat = (double)(dr2["lat"]);
                            lastLat = lat;
                            if (lat < lat1 || lat > lat2)
                            {
                                finish = true;
                                break;
                            }
                            if (lon < lon1)
                            {
                                continue;
                            }
                            if (lon > lon2)
                            {
                                minInd = i;
                                maxInd = tLen;
                                break;
                            }
                            resdic.Add(gczbpx[i]);
                            if (i == tLen)
                            {
                                finish = true;
                            }
                        }
                        if (finish)
                        {
                            break;
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"ok\":true,\"table\":[");
            int j = 0;
            foreach (var d in resdic)
            {
                var kpid = kpinull;
                if (kpi.ContainsKey(d))
                {
                    kpid = kpi[d];
                }
                var row = "";

                try
                {
                    row = string.Format("\"eci\":{0},\"enbid\":{1},\"cellid\":{2},\"scname\":\"{3}\",\"city\":\"{4}\",\"lon\":{5},\"lat\":{6},\"angle\":{7},\"zangle\":{8},\"dangle\":{9},\"rrc\":{10},\"upflow\":{11},\"downflow\":{12},\"upprb\":{13},\"downprb\":{14},\"erab\":{15},\"cqi\":{16},\"bad\":{17},\"badinfo\":\"{18}\"",
                                               d, gc[d]["enbid"], gc[d]["cellid"], gc[d]["scname"], gc[d]["city"], gc[d]["lon"], gc[d]["lat"], gc[d]["angle"], gc[d]["zangle"], gc[d]["dangle"]
                                               , kpid["rrc"], kpid["upflow"], kpid["downflow"], kpid["upprb"], kpid["downprb"], kpid["erab"], kpid["cqi"], alarmCache[d][0] == "0" ? "false" : "true", alarmCache[d][1]
                        );
                }
                catch (Exception ex)
                {
                    var a = ex.Message;
                }
                sb.Append("{" + row + "},");
                j++;
            }
            if (j > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]}");

            data = Encoding.GetEncoding("utf-8").GetBytes(sb.ToString());
            return data;
        }
        Dictionary<int, string> cityNames = new Dictionary<int, string>() { { 0, "石家庄" }, { 1, "廊坊" }, { 2, "保定" }, { 3, "邯郸" }, { 4, "沧州" }, { 5, "衡水" }, { 6, "邢台" }, { 7, "唐山" }, { 8, "秦皇岛" }, { 9, "张家口" }, { 10, "承德" }, { 11, "雄安新区" }, { 12, "全省" } };
        byte[] getCityDate(req rq)
        {
            byte[] data = null;
            //row = string.Format("\"eci\":{0},\"enbid\":{1},\"cellid\":{2},\"scname\":\"{3}\",\"city\":\"{4}\",\"lon\":{5},\"lat\":{6},\"angle\":{7},\"zangle\":{8},\"dangle\":{9},\"rrc\":{10},\"upflow\":{11},\"downflow\":{12},\"upprb\":{13},\"downprb\":{14},\"erab\":{15},\"cqi\":{16},\"bad\":{17},\"badinfo\":\"{18}\"",
            //                                   d, gc[d]["enbid"], gc[d]["cellid"], gc[d]["scname"], gc[d]["city"], gc[d]["lon"], gc[d]["lat"], gc[d]["angle"], gc[d]["zangle"], gc[d]["dangle"]
            //                                   , kpid["rrc"], kpid["upflow"], kpid["downflow"], kpid["upprb"], kpid["downprb"], kpid["erab"], kpid["cqi"], alarmCache[d][0] == "0" ? "false" : "true", alarmCache[d][1]
            //            );
            StringBuilder sb = new StringBuilder("城市,小区名称,eci,enbid,cellid,经度,纬度,方位角,总下倾角,电子下倾角,rrc,上行流量,下行流量,上行PRB利用率,下行PRB利用率,erab,CQI,是否故障,故障信息\n");

            string city = cityNames[rq.CRS];
            foreach (var d in gczbpx)
            {
                var kpid = kpinull;
                if (kpi.ContainsKey(d))
                {
                    kpid = kpi[d];
                }
                try
                {
                    if (city == "全省")
                    {
                        sb.Append(gc[d]["city"]).Append(",").Append(gc[d]["scname"]).Append(",").Append(d).Append(",").Append(gc[d]["enbid"]).Append(",").Append(gc[d]["cellid"]).Append(",")
                            .Append(gc[d]["lon"]).Append(",").Append(gc[d]["lat"]).Append(",").Append(gc[d]["angle"]).Append(",").Append(gc[d]["zangle"]).Append(",").Append(gc[d]["dangle"]).Append(",")
                            .Append(kpid["rrc"]).Append(",").Append(kpid["upflow"]).Append(",").Append(kpid["downflow"]).Append(",").Append(kpid["upprb"]).Append(",").Append(kpid["downprb"]).Append(",")
                            .Append(kpid["erab"]).Append(",").Append(kpid["cqi"]).Append(",").Append(alarmCache[d][0] == "0" ? "否" : "是").Append(",").Append(alarmCache[d][1]).Append("\n");
                    }
                    else
                    {
                        if (gc[d]["city"].ToString() == city)
                        {
                            sb.Append(gc[d]["city"]).Append(",").Append(gc[d]["scname"]).Append(",").Append(d).Append(",").Append(gc[d]["enbid"]).Append(",").Append(gc[d]["cellid"]).Append(",")
                            .Append(gc[d]["lon"]).Append(",").Append(gc[d]["lat"]).Append(",").Append(gc[d]["angle"]).Append(",").Append(gc[d]["zangle"]).Append(",").Append(gc[d]["dangle"]).Append(",")
                            .Append(kpid["rrc"]).Append(",").Append(kpid["upflow"]).Append(",").Append(kpid["downflow"]).Append(",").Append(kpid["upprb"]).Append(",").Append(kpid["downprb"]).Append(",")
                            .Append(kpid["erab"]).Append(",").Append(kpid["cqi"]).Append(",").Append(alarmCache[d][0] == "0" ? "否" : "是").Append(",").Append(alarmCache[d][1]).Append("\n");
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            data = Encoding.GetEncoding("gbk").GetBytes(sb.ToString());
            return data;
        }
        private ColorBlend getColorBlend(double bili)
        {
            bili = bili > 1 ? 1 : bili;
            ColorBlend colors = new ColorBlend(3);

            colors.Positions = new float[3] { 0, 0.6f, 1 };
            colors.Colors = new Color[3]
            {
                Color.FromArgb(10, Color.Black),
                Color.FromArgb((int)(bili*245*0.5)+10, Color.Black),
                Color.FromArgb((int)(bili*245)+10, Color.Black)
            };
            return colors;
        }
        private Color getARGBBrushC(double bili)
        {
            var tmmax = 200;
            var bbb = 66;
            var aaa = tmmax - bbb;
            var tmr = aaa * bili + bbb;

            if (bili <= 0.25)
            {
                return Color.FromArgb((int)(tmr), Color.Green);
            }
            if (bili <= 0.5)
            {
                return Color.FromArgb((int)(tmr), 0, (int)(255 - (bili - 0.25) / 0.25 * 255), (int)((bili - 0.25) / 0.25 * 127 + 128));
            }
            if (bili <= 0.75)
            {
                return Color.FromArgb((int)(tmr), (int)((bili - 0.5) / 0.25 * 255), (int)((bili - 0.5) / 0.25 * 255), (int)(255 - (bili - 0.5) / 0.25 * 255));
            }
            if (bili <= 1)
            {
                return Color.FromArgb((int)(tmr), 255, 255 - (int)((bili - 0.75) / 0.25 * 255), 0);
            }
            return Color.FromArgb((int)(255), Color.Red);
        }
        #endregion

        #region 质差业务

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="skt"></param>
        /// <param name="data"></param>
        void Receive(Socket skt, byte[] data)
        {
            var len = data.Length;
            int offset = 0;
            while (offset < len)
            {
                var jielen = Math.Min(len - offset, bufferlen);
                var slen = skt.Receive(data, offset, jielen, SocketFlags.None);
                offset += slen;
            }
        }
        /// <summary>
        /// 判断质差总方法
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        byte[] getZHICHA(req rq)
        {
            byte[] data = null;
            int datalen = rq.WIDTH;

            byte[] xmldata = new byte[datalen];
            rq.EP.ReceiveTimeout = 5000;
            Receive(rq.EP, xmldata);
            var xmlstr = Encoding.GetEncoding("gbk").GetString(xmldata);
            try {
                var doc = XDocument.Parse(xmlstr);
                var infos = doc.Document.Descendants("alarmInfo");
                foreach (var info in infos)
                {
                    try
                    {
                        //var alarmType = info.Element("alarmType");
                        //if (alarmType == null || alarmType.Value != "无线告警") continue;
                        var desc = info.Element("alarmDescript");
                        var Enodeb_ID = "Enodeb ID：";
                        var CellID = "CellID：";
                        var enodebidindex = desc.Value.IndexOf(Enodeb_ID);
                        var cellidindex = desc.Value.IndexOf(CellID);

                        if (enodebidindex < 0 || cellidindex < 0) continue;
                        var elast = desc.Value.Substring(enodebidindex + Enodeb_ID.Length);
                        var enbid = O2.O2I(elast.Substring(0, elast.IndexOf("；")));
                        var clast = desc.Value.Substring(cellidindex + CellID.Length);
                        var cellid = O2.O2I(clast.Substring(0, clast.IndexOf("\n")));
                        if (enbid <= 0) continue;


                        var d2dstr = getZHICHA(enbid, cellid);
                        if (d2dstr == "") continue;
                        var d2dinfo = new XElement("d2dinfo", new XCData(d2dstr));
                        info.Add(d2dinfo);
                    }
                    catch { }
                }

                return Encoding.GetEncoding("gbk").GetBytes(doc.Declaration + doc.ToString(SaveOptions.DisableFormatting));
            }
            catch {
                return Encoding.GetEncoding("gbk").GetBytes(xmlstr);
            }
            
        }
        /// <summary>
        /// 800M小区判断
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        string xq8001800(int cellid)
        {
            return cellid % 0x10 == 1 || cellid % 0x10 == 9 ? "1" : "2";
        }
        /// <summary>
        /// 单小区质差方法
        /// </summary>
        /// <param name="enbid"></param>
        /// <param name="cellid"></param>
        /// <returns></returns>
        string getZHICHA(int enbid, int cellid)
        {
            var eci = enbid * 256 + cellid;
            //kpi
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            var kpizb = "";
            List<string> kpizbarr = new List<string>();
            if (kpiyest.ContainsKey(eci))
            {
                var rrcljcgl = getZHICHA("rrcljcgl", kpiyest[eci]["rrcljcgl"], 0);
                var rrcsbcs = getZHICHA("rrcsbcs", kpiyest[eci]["rrcsbcs"], 0);
                var xhysl = getZHICHA("xhysl", kpiyest[eci]["xhysl"], 0);
                var xhyscs = getZHICHA("xhyscs", kpiyest[eci]["xhyscs"], 0);
                var rrcljcjbl = getZHICHA("rrcljcjbl", kpiyest[eci]["rrcljcjbl"], 0);
                var rrcljcjcgcs = getZHICHA("rrcljcjcgcs", kpiyest[eci]["rrcljcjcgcs"], 0);
                var uesxwdxl = getZHICHA("uesxwdxl", kpiyest[eci]["uesxwdxl"], 0);
                var uesxycsfcs = getZHICHA("uesxycsfcs", kpiyest[eci]["uesxycsfcs"], 0);
                var erabdxl = getZHICHA("erabdxl", kpiyest[eci]["erabdxl"], 0);
                var erabdxcs = getZHICHA("erabdxcs", kpiyest[eci]["erabdxcs"], 0);
                var rrccjbl = getZHICHA("rrccjbl", kpiyest[eci]["rrccjbl"], 0);
                var rrccjqqcs = getZHICHA("rrccjqqcs", kpiyest[eci]["rrccjqqcs"], 0);
                var xtnqhcgl = getZHICHA("xtnqhcgl", kpiyest[eci]["xtnqhcgl"], 0);
                var xtnqsbcs = getZHICHA("xtnqsbcs", kpiyest[eci]["xtnqsbcs"], 0);
                var cqibl = getZHICHA("cqibl", kpiyest[eci]["cqibl"], 0);
                var xxprbslzb = getZHICHA("xxprbslzb", kpiyest[eci]["xxprbslzb"], 0);
                var yhtysxpjsl = getZHICHA("yhtysxpjsl", kpiyest[eci]["yhtysxpjsl"], 0);
                var yhtyxxpjsl = getZHICHA("yhtyxxpjsl", kpiyest[eci]["yhtyxxpjsl"], 0);

                if (rrcljcgl && rrcsbcs)
                {
                    kpizbarr.Add(quota["rrcljcgl"]["0"]["name"].ToString());
                }
                if (xhysl && xhyscs)
                {
                    kpizbarr.Add(quota["xhysl"]["0"]["name"].ToString());
                }
                if (rrcljcjbl && rrcljcjcgcs)
                {
                    kpizbarr.Add(quota["rrcljcjbl"]["0"]["name"].ToString());
                }
                if (uesxwdxl && uesxycsfcs)
                {
                    kpizbarr.Add(quota["uesxwdxl"]["0"]["name"].ToString());
                }
                if (erabdxl && erabdxcs)
                {
                    kpizbarr.Add(quota["erabdxl"]["0"]["name"].ToString());
                }
                if (rrccjbl && rrccjqqcs)
                {
                    kpizbarr.Add(quota["rrccjbl"]["0"]["name"].ToString());
                }
                if (xtnqhcgl && xtnqsbcs)
                {
                    kpizbarr.Add(quota["xtnqhcgl"]["0"]["name"].ToString());
                }
                if (cqibl)
                {
                    kpizbarr.Add(quota["cqibl"]["0"]["name"].ToString());
                }
                if (xxprbslzb)
                {
                    kpizbarr.Add(quota["xxprbslzb"]["0"]["name"].ToString());
                }
                if (yhtysxpjsl)
                {
                    kpizbarr.Add(quota["yhtysxpjsl"]["0"]["name"].ToString());
                }
                if (yhtyxxpjsl)
                {
                    kpizbarr.Add(quota["yhtyxxpjsl"]["0"]["name"].ToString());
                }
                kpizb += string.Join("、", kpizbarr);
            }
            kpizb += "";

            //容量覆盖
            List<string> mrzbarr = new List<string>();
            var mrzb = "";
            if (kpiyest.ContainsKey(eci))
            {
                var l800 = xq8001800(cellid);
                var l800int = O2.O2I(l800);
                var sxprbpjlyl18 = getZHICHA("sxprbpjlyl18", kpiyest[eci]["sxprbpjlyl18"], l800int);
                var xxprbpjlyl18 = getZHICHA("xxprbpjlyl18", kpiyest[eci]["xxprbpjlyl18"], l800int);
                var pjrrcljyhs18 = getZHICHA("pjrrcljyhs18", kpiyest[eci]["pjrrcljyhs18"], l800int);
                var pdcpxx18 = getZHICHA("pdcpxx18", kpiyest[eci]["pdcpxx18"], l800int);
                var pdcpsx18 = getZHICHA("pdcpsx18", kpiyest[eci]["pdcpsx18"], l800int);
                var pjjhyhs18 = getZHICHA("pjjhyhs18", kpiyest[eci]["pjjhyhs18"], l800int);
                if (sxprbpjlyl18)
                {
                    mrzbarr.Add(quota["sxprbpjlyl18"][l800]["name"].ToString());
                }
                if (xxprbpjlyl18)
                {
                    mrzbarr.Add(quota["xxprbpjlyl18"][l800]["name"].ToString());
                }
                if (pjrrcljyhs18)
                {
                    mrzbarr.Add(quota["pjrrcljyhs18"][l800]["name"].ToString());
                }
                if (pdcpxx18)
                {
                    mrzbarr.Add(quota["pdcpxx18"][l800]["name"].ToString());
                }
                if (pdcpsx18)
                {
                    mrzbarr.Add(quota["pdcpsx18"][l800]["name"].ToString());
                }
                if (pjjhyhs18)
                {
                    mrzbarr.Add(quota["pjjhyhs18"][l800]["name"].ToString());
                }
            }
            if (mr.ContainsKey(eci))
            {
                var xqpjrsrp = getZHICHA("xqpjrsrp", mr[eci]["xqpjrsrp"], 0);
                var rfgbl = getZHICHA("rfgbl", mr[eci]["rfgbl"], 0);
                var pjjl = getZHICHA("pjjl", mr[eci]["pjjl"], 0);
                var cdfgk = getZHICHA("cdfgk", mr[eci]["cdfgk"], 0);
                var gfglqgs = getZHICHA("gfglqgs", mr[eci]["gfglqgs"], 0);
                var modgrcd = getZHICHA("modgrcd", mr[eci]["modgrcd"], 0);
                if (xqpjrsrp)
                {
                    mrzbarr.Add(quota["xqpjrsrp"]["0"]["name"].ToString());
                }
                if (rfgbl)
                {
                    mrzbarr.Add(quota["rfgbl"]["0"]["name"].ToString());
                }
                if (pjjl)
                {
                    mrzbarr.Add(quota["pjjl"]["0"]["name"].ToString());
                }
                if (cdfgk)
                {
                    mrzbarr.Add(quota["cdfgk"]["0"]["name"].ToString());
                }
                if (gfglqgs)
                {
                    mrzbarr.Add(quota["gfglqgs"]["0"]["name"].ToString());
                }
                if (modgrcd)
                {
                    mrzbarr.Add(quota["modgrcd"]["0"]["name"].ToString());
                }
            }
            mrzb += string.Join("、", mrzbarr);
            mrzb += "";

            //解决措施
            var jjcs = "";
            List<string> jjcsarr = new List<string>();
            if (kpiyest.ContainsKey(eci))
            {
                double DOWN_PRB = kpiyest[eci]["xxprbpjlyl18"] == null ? 0 : (double)kpiyest[eci]["xxprbpjlyl18"];
                double UP_PRB = kpiyest[eci]["sxprbpjlyl18"] == null ? 0 : (double)kpiyest[eci]["sxprbpjlyl18"];
                double DOWN_PDCP = kpiyest[eci]["pdcpxx18"] == null ? 0 : (double)kpiyest[eci]["pdcpxx18"];
                double UP_PDCP = kpiyest[eci]["pdcpsx18"] == null ? 0 : (double)kpiyest[eci]["pdcpsx18"];
                double RRC = kpiyest[eci]["pjrrcljyhs18"] == null ? 0 : (double)kpiyest[eci]["pjrrcljyhs18"];
                var rljjcs = RLMeasure(eci, DOWN_PRB, UP_PRB, DOWN_PDCP, UP_PDCP, RRC);
                if (rljjcs != "")
                {
                    jjcsarr.Add(rljjcs);
                }
            }
            if (mr.ContainsKey(eci))
            {
                double RSRP = mr[eci]["xqpjrsrp"] == null ? 0 : (double)mr[eci]["xqpjrsrp"];
                double FGL = mr[eci]["rfgbl"] == null ? 0 : (double)mr[eci]["rfgbl"];
                double tadist = mr[eci]["TADV_COUNT_SUM"] == null ? 0 : (double)mr[eci]["TADV_COUNT_SUM"]; 
                double dipangle = gc.ContainsKey(eci)?(double)gc[eci]["zangle"]:0;
                double cdfgl = mr[eci]["cdfgk"] == null ? 0 : (double)mr[eci]["cdfgk"];
                double gfglqs = mr[eci]["gfglqgs"] == null ? 0 : (double)mr[eci]["gfglqgs"];
                double mod3 = mr[eci]["modgrcd"] == null ? 0 : (double)mr[eci]["modgrcd"];
                var rfg = FGMeasure(RSRP, FGL, tadist, dipangle);
                var gfg = GFGMeasure(cdfgl, gfglqs, mod3, dipangle);
                var grs = GRMeasure(cdfgl, gfglqs, mod3, dipangle);
                var cdfg = CDFG(cdfgl);

                if (!string.IsNullOrEmpty(gfg) && (!string.IsNullOrEmpty(rfg) || !string.IsNullOrEmpty(cdfg)))
                {
                    gfg = "优先解决" + gfg + "，观察调整效果后再解决";
                    //rfg=string.IsNullOrEmpty(rfg)?"":("观察后再解决"+rfg);
                    //cdfg = string.IsNullOrEmpty(cdfg) ? "" : ("观察后再解决" + cdfg);
                    if (!string.IsNullOrEmpty(rfg))
                    {
                        gfg += "弱覆盖";
                        rfg = "";
                    }
                    if (!string.IsNullOrEmpty(cdfg))
                    {
                        gfg += "重叠覆盖";
                        cdfg = "";
                    }
                }
                
                if (!string.IsNullOrEmpty(gfg))
                {
                    jjcsarr.Add(gfg);
                }
                if (!string.IsNullOrEmpty(rfg))
                {
                    jjcsarr.Add(rfg);
                }
                if (!string.IsNullOrEmpty(cdfg))
                {
                    jjcsarr.Add(cdfg);
                }
                if (!string.IsNullOrEmpty(grs))
                {
                    jjcsarr.Add(grs);
                }
            }
            else
            {

            }
            jjcs += string.Join("、", jjcsarr);
            jjcs += "";

            if (string.IsNullOrEmpty(kpizb) && !string.IsNullOrEmpty(mrzb))
            {
                kpizb = mrzb;
            }
            if (String.IsNullOrEmpty(kpizb))
            {
                kpizb = "";
                mrzb = "";
                jjcs = "";
            }
            else
            {
                if (String.IsNullOrEmpty(mrzb))
                    mrzb = "";
                if (String.IsNullOrEmpty(jjcs))
                    jjcs = "正常，无需处理";
            }
            if (!mrzb.Contains("弱覆盖") && !mrzb.Contains("过覆盖"))
            {
                jjcs.Replace("弱覆盖：核查工参，进行覆盖调整", "");
            }
            else if (!mrzb.Contains("弱覆盖") && mrzb.Contains("过覆盖"))
            {
                jjcs.Replace("弱覆盖：核查工参，进行覆盖调整", "过覆盖：核查工参，进行覆盖调整");
            }
            else if (mrzb.Contains("弱覆盖") && mrzb.Contains("过覆盖"))
            {
                jjcs.Replace("弱覆盖：核查工参，进行覆盖调整", "弱覆盖过覆盖：核查工参，进行覆盖调整");
            }
            sb.Append("KPI差指标：");
            sb.Append(kpizb);
            sb.Append("|\n");

            sb.Append("容量覆盖差指标：");
            sb.Append(mrzb);
            sb.Append("|\n");

            sb.Append("解决措施：");
            sb.Append(jjcs);
            sb.Append("|\n");

            var res = sb.ToString();
            if (kpizb == "" && mrzb == "" && jjcs == "" || jjcs == "正常，无需处理") {
                res = "";
            }
            return res;
        }
        /// <summary>
        /// 指标质差判断
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool getZHICHA(string name, object value, int type)
        {
            bool res = false;
            if (value == null) return false;
            Dictionary<string, string> dic = quota[name][type + ""];
            double? min = dic["min"] == "" ? null : (double?)O2.O2D(dic["min"]);
            double? max = dic["max"] == "" ? null : (double?)O2.O2D(dic["max"]);
            double v = (double)value;
            if (min.HasValue && max.HasValue)
            {
                res = v >= min.Value && v <= max.Value;
            }
            else if (min.HasValue)
            {
                res = v >= min.Value;
            }
            else if (max.HasValue)
            {
                res = v <= max.Value;
            }

            return res;
        }
        /// <summary>
        /// 容量解决措施分类
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private string getMeasureTxt(int count)
        {
            if (count == 0)
                return "";
            else if (count == 1)
                return "容量：一项标准达到预警，密切观察";
            else if (count == 2)
                return "容量：两项标准达到预警，天馈/功率调整优化小区覆盖范围，如效果不明显，载波扩容或新建基站";
            else
                return "容量：三项标准达到预警，进行小区分裂，载频扩容或新建基站分流话务";
        }
        /// <summary>
        /// 容量解决措施
        /// </summary>
        /// <param name="eci"></param>
        /// <param name="DOWN_PRB"></param>
        /// <param name="UP_PRB"></param>
        /// <param name="DOWN_PDCP"></param>
        /// <param name="UP_PDCP"></param>
        /// <param name="RRC"></param>
        /// <returns></returns>
        public String RLMeasure(int eci, double DOWN_PRB, double UP_PRB, double DOWN_PDCP, double UP_PDCP, double RRC)
        {
            Boolean Is800M = xq8001800(eci % 256) == "1";
            int count = 0;
            if (Is800M)
            {
                if (DOWN_PRB >= 50 || UP_PRB >= 50 || UP_PDCP >= 0.75 * 1024.0 || DOWN_PDCP >= 2 * 1024 || RRC >= 200)
                    return "容量：达到扩容标准，进行小区分裂，载频扩容或新建基站分流话务";
                else
                {
                    if (DOWN_PRB >= 30)
                        count = count + 1;
                    if (UP_PRB >= 30)
                        count = count + 1;
                    if (UP_PDCP >= 0.4 * 1024)
                        count = count + 1;
                    if (DOWN_PDCP >= 1024)
                        count = count + 1;
                    if (RRC >= 30)
                        count = count + 1;
                    return getMeasureTxt(count);
                }
            }
            else
            {
                if (DOWN_PRB >= 50 || UP_PRB >= 50 || UP_PDCP >= 3 * 1024 || DOWN_PDCP >= 8 * 1024 || RRC >= 200)
                    return "容量：达到扩容标准，进行小区分裂，载频扩容或新建基站分流话务";
                else
                {
                    if (DOWN_PRB >= 30)
                        count = count + 1;
                    if (UP_PRB >= 30)
                        count = count + 1;
                    if (UP_PDCP >= 1.5 * 1024)
                        count = count + 1;
                    if (DOWN_PDCP >= 4 * 1024)
                        count = count + 1;
                    if (RRC >= 50)
                        count = count + 1;
                    return getMeasureTxt(count);
                }
            }
        }
        /// <summary>
        /// 覆盖判决
        /// </summary>
        /// <param name="RSRP">小区级平均RSRP</param>
        /// <param name="FGL">弱覆盖比例%</param>
        /// <param name="tadist">平均TA距离</param>
        /// <param name="dipangle">下倾角</param>
        /// <returns></returns>
        public static String FGMeasure(double RSRP, double FGL, double tadist, double dipangle)
        {
            FGL = FGL * 100;
            //改!!!
            //if (FGL >= 0 && FGL <= 20)
            if (FGL > 0 && FGL <= 20)
            {
                if (RSRP <= -105)
                {
                    if (dipangle >= 5)
                        return "弱覆盖：减少下倾角2°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
                else
                    return "";
            }
            else if (FGL > 20 && FGL <= 40)
            {
                if (dipangle >= 5)
                    //return "弱覆盖：减少下倾角4°或功率提升一倍，增加boost3db";
                    return "弱覆盖：减少下倾角4°或功率提升一倍";
                else
                    return "弱覆盖：功率提升一倍";
            }
            else if (FGL > 40 && FGL <= 60)
            {
                if (tadist <= 1)
                {
                    if (dipangle >= 7)
                        //return "弱覆盖：减少下倾角4°或功率提升一倍，增加boost3db";
                        return "弱覆盖：减少下倾角4°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
                else
                {
                    if (dipangle >= 7)
                        //return "弱覆盖：减少下倾角6°或功率提升一倍，增加boost3db";
                        return "弱覆盖：减少下倾角6°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
            }
            else if (FGL > 60 && FGL <= 100)
            {
                if (tadist <= 1)
                {
                    if (dipangle >= 7)
                        //return "弱覆盖：减少下倾角6°或功率提升一倍，增加boost3db";
                        return "弱覆盖：减少下倾角6°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
                else
                {
                    if (dipangle >= 7)
                        //return "弱覆盖：减少下倾角6°或功率提升一倍，增加boost3db";
                        return "弱覆盖：减少下倾角6°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 干扰判决
        /// </summary>
        /// <param name="cdfgl">重覆盖率（6db）</param>
        /// <param name="gfglqs">过覆盖领区个数</param>
        /// <param name="mod3">MOD3干扰比例</param>
        /// <param name="dipangle">下倾角</param>
        /// <returns></returns>
        public static String GLMeasure(double cdfgl, double gfglqs, double mod3, double dipangle)
        {
            String result = CDFG(cdfgl);
            if (gfglqs >= 12)
            {
                if (dipangle < 7)
                {
                    if (String.IsNullOrEmpty(result))
                        result = "过覆盖：增加下倾角3°或功率降低一倍";
                    else
                        result = result + "、过覆盖：增加下倾角3°或功率降低一倍";
                }
                else
                {
                    if (String.IsNullOrEmpty(result))
                        result = "过覆盖：功率降低一倍";
                    else
                        result = result + "、过覆盖：功率降低一倍";
                }
            }
            if (mod3 >= 20)
            {
                if (String.IsNullOrEmpty(result))
                    result = "干扰：建议分析PCI模3干扰，根据PCI优化工具进行调整";
                else
                    result = result + "、干扰：建议分析PCI模3干扰，根据PCI优化工具进行调整";
            }
            return result;
        }
        public static String GFGMeasure(double cdfgl, double gfglqs, double mod3, double dipangle)
        {
            var result = "";
            if (gfglqs >= 12)
            {
                if (dipangle < 7)
                {
                    result = "过覆盖：增加下倾角3°或功率降低一倍";
                }
                else
                {
                    result = "过覆盖：功率降低一倍";
                }
            }
            return result;
        }
        public static String GRMeasure(double cdfgl, double gfglqs, double mod3, double dipangle)
        {
            mod3 = mod3 * 100;
            var result = "";
            if (mod3 >= 20)
            {
                result = "干扰：建议分析PCI模3干扰，根据PCI优化工具进行调整";
            }
            return result;
        }
        public static String CDFG(double cdfg)
        {
            cdfg = cdfg * 100.00;
            if (cdfg >= 20)
                return "重叠覆盖：周边基站RF优化";
            else
                return "";
        }
        #endregion
        #endregion

        #region 多线程处理图像
        void probitpix()
        {
            while (true)
            {
                int offset;
                bitmaps btms = GetLastbit(out offset);
                try
                {

                    if (btms != null)
                    {
                        int ind = offset/btms.height+offset%(btms.width)*4;
                        var alp = btms.bt1[ind + 3];
                        if (alp != 0)
                        {
                            var outcolor = getARGBBrushC(alp / 255f);
                            btms.bt2[ind] = outcolor.R;
                            btms.bt2[ind + 1] = outcolor.G;
                            btms.bt2[ind + 2] = outcolor.B;
                            btms.bt2[ind + 3] = outcolor.A;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var err = ex;
                }
                if (btms != null)
                {
                    SetbitComplete(btms);
                }
            }
        }
        void SetPixBitmap(byte[] bt1, byte[] bt2, int width, int height)
        {
            AutoResetEvent aret = new AutoResetEvent(false);
            Addtobitlist(bt1, bt2, width, height, aret);
        }
        void Addtobitlist(byte[] bt1, byte[] bt2, int width, int height, AutoResetEvent aret)
        {
            arebit.WaitOne();
            bitlist.Add(new bitmaps { bt1 = bt1, bt2 = bt2, width = width, height = height, offset = -1, procount = 0, are = aret });
            arebit.Set();
            arebit2.Set();
            aret.WaitOne();
        }
        bitmaps GetLastbit(out int offset)
        {
            bitmaps btms = null;
            offset = 0;
            arebit.WaitOne();
            for (int i = 0; i < bitlist.Count; i++)
            {
                var bts = (bitmaps)bitlist[i];
                if (bts.offset + 1 < bts.width * bts.height)
                {
                    btms = bts;
                    bts.offset++;
                    break;
                }
                else
                {
                    bitlist.RemoveAt(i);
                    i--;
                }
            }
            arebit.Set();
            if (btms == null)
            {
                arebit2.Reset();
                arebit2.WaitOne();
            }
            return btms;
        }
        void SetbitComplete(bitmaps bts)
        {
            arebit.WaitOne();
            bts.procount++;
            if (bts.procount >= bts.width * bts.height)
            {
                bts.are.Set();
            }
            arebit.Set();
        }
        #endregion

        #region 配置
        req lastreq
        {
            get
            {
                req res = null;
                if (actlist.Count > 0)
                {
                    res = (req)actlist[0];
                    actlist.RemoveAt(0);
                }
                return res;
            }
        }
        string kpipath
        {
            get
            {
                return ConfigurationManager.AppSettings["kpipath"];
            }
        }
        string abcpath
        {
            get
            {
                return ConfigurationManager.AppSettings["abcpath"];
            }
        }
        string defpath
        {
            get
            {
                return ConfigurationManager.AppSettings["defpath"];
            }
        }
        string gcpath
        {
            get
            {
                return ConfigurationManager.AppSettings["gcpath"];
            }
        }
        string altpath
        {
            get
            {
                return ConfigurationManager.AppSettings["altpath"];
            }
        }
        string gcrrupath
        {
            get
            {
                return ConfigurationManager.AppSettings["gcrrupath"];
            }
        }
        string nokiaecipath
        {
            get
            {
                return ConfigurationManager.AppSettings["nokiaecipath"];
            }
        }
        string quotapath
        {
            get
            {
                return ConfigurationManager.AppSettings["quotapath"];
            }
        }
        string mrfilepath
        {
            get
            {
                return ConfigurationManager.AppSettings["mrfilepath"];
            }
        }
        #endregion
        
    }

    #region 格式转换类
    public static class O2
    {
        public static string O2S(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return "";
            }
            if (o.GetType() == typeof(XElement))
            {
                return ((XElement)o).Value;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                return ((XAttribute)o).Value;
            }
            return o.ToString();
        }
        public static bool O2B(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return false;
            }
            if (o.GetType() == typeof(XElement))
            {
                bool res;
                bool.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                bool res;
                bool.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                bool res;
                bool.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static DateTime O2DT(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return new DateTime();
            }
            if (o.GetType() == typeof(XElement))
            {
                DateTime res;
                DateTime.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                DateTime res;
                DateTime.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                DateTime res;
                DateTime.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static int O2I(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                int res;
                int.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                int res;
                int.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                int res;
                int.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static long O2L(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                long res;
                long.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                long res;
                long.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                long res;
                long.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static double O2D(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                double res;
                double.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                double res;
                double.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                double res;
                double.TryParse(o.ToString(), out res);
                return res;
            }
        }
    }
    #endregion

    #region 报文类
    public class req
    {
        public int ACT;
        public int WIDTH; public int HEIGHT; public double BBOXx; public double BBOXy; public double BBOXx2; public double BBOXy2; public int CRS;
        public Socket EP;
        public req()
        {

        }
        public req(int act, int width, int height, double bboxx, double bboxy, double bboxx2, double bboxy2, int crs, Socket ep)
        {
            ACT = act;
            WIDTH = width;
            HEIGHT = height;
            BBOXx = bboxx;
            BBOXy = bboxy;
            BBOXx2 = bboxx2;
            BBOXy2 = bboxy2;
            CRS = crs;
            EP = ep;
        }
    }
    #endregion

    #region 图像多线程类
    public class bitmaps
    {
        public byte[] bt1 { get; set; }
        public byte[] bt2 { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public long offset { get; set; }
        public long procount { get; set; }
        public AutoResetEvent are { get; set; }
    }
    #endregion
}
