﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.OpenXml4Net.OPC;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CTCCGoods.Controllers
{
    /// <summary>
    /// 后台任务对象
    /// </summary>
    public class ctasks {
        public int? id { get; set; }
        public int? ttype { get; set; }
        public string filename { get; set; }
        public string tfilename { get; set; }
        public string rul { get; set; }
        public string tdesc { get; set; }
        public int? tstatus { get; set; }
        public DateTime? createtime { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
        public string errurl { get; set; }
    }
    /// <summary>
    /// 后台任务处理类
    /// </summary>
    public class ctasksHandle
    {
        /// <summary>
        /// 线程状态控制
        /// </summary>
        static AutoResetEvent are = new AutoResetEvent(true);
        /// <summary>
        /// 任务状态 0：未执行，1：正在执行
        /// </summary>
        static int allstatus = 0;
        /// <summary>
        /// 最新消息
        /// </summary>
        static string allmsg = "";
        static bool isrun=false;
        /// <summary>
        /// 注册处理类
        /// </summary>
        public static void Register() {
            Thread th = new Thread(new ThreadStart(runctasks));
            th.Start();
            isrun = true;
        }
        /// <summary>
        /// 获取任务状态
        /// </summary>
        /// <param name="m">最新消息</param>
        /// <returns>状态</returns>
        public static int getallstatus(out string m) {
            m = allmsg;
            return allstatus;
        }
        /// <summary>
        /// 强制执行任务
        /// </summary>
        public static void reset() {
            are.Set();
        }
        /// <summary>
        /// 后台任务循环方法
        /// </summary>
        private static void runctasks() {
            while(true){
                allstatus = 0;
                GC.Collect();
                are.WaitOne();
                allstatus = 1;
                try
                {
                    ctasks newone = null;

                    while (null != (newone = getNewone()))
                    {
                        starttask(newone);
                        string msg;
                        var res=dealtask(newone,out msg);
                        if (res)
                        {
                            endtask(newone, "成功");
                        }
                        else {
                            endtask(newone, "失败",msg);
                        }
                        GC.Collect();
                    }
                }
                catch(Exception ex) {
                    allmsg = ex.Message;
                    GC.Collect();
                }
            }
        }
        /// <summary>
        /// 获取最新任务
        /// </summary>
        /// <returns>任务对象</returns>
        private static ctasks getNewone() {
            var taskone= DB.QueryOne("select top 1 * from ctasks where tstatus=0 or tstatus=1 order by tstatus desc,id asc");
            return dic2ctasks(taskone);
        }
        /// <summary>
        /// 开始任务状态
        /// </summary>
        /// <param name="ct">任务对象</param>
        private static void starttask(ctasks ct) {
            var now = DateTime.Now;
            int res = DB.Exec("update ctasks set tdesc='运行',tstatus=1,starttime='"+now.ToString("yyyy-MM-dd HH:mm:ss")+"' where id="+ct.id.Value);
            if (res > 0) {
                ct.tdesc = "运行";
                ct.tstatus = 1;
                ct.starttime = now;
            }
        }
        /// <summary>
        /// 结束任务状态
        /// </summary>
        /// <param name="ct">任务对象</param>
        /// <param name="tdesc">结束消息</param>
        /// <param name="errorlog">错误详细信息</param>
        private static void endtask(ctasks ct,string tdesc,string errorlog=null)
        {
            var now = DateTime.Now;
            string errfile = null;
            if (errorlog != null) {
                errfile = "err"+now.ToString("yyyyMMddHHmmssmsfffffff")+".txt";
                var errpath=System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"taskerr");
                if (!System.IO.Directory.Exists(errpath)) {
                    System.IO.Directory.CreateDirectory(errpath);
                }
                System.IO.File.WriteAllText(System.IO.Path.Combine(errpath,errfile),errorlog,System.Text.Encoding.GetEncoding("gbk"));
            }

            int res = DB.Exec("update ctasks set tdesc='" + tdesc + "',tstatus=2,endtime='" + now.ToString("yyyy-MM-dd HH:mm:ss") + "',errurl="+(errorlog==null?"null":"'"+errfile+"'")+" where id=" + ct.id.Value);
            if (res > 0)
            {
                ct.tdesc = tdesc;
                ct.tstatus = 2;
                ct.endtime = now;
                ct.errurl = errfile;
            }
        }
        /// <summary>
        /// 新建任务
        /// </summary>
        /// <param name="ttype">任务类型 1：发货表导入，2：网管详表导入，3：计算新增，4：lte rru核对表导入</param>
        /// <param name="filename">要处理的文件名</param>
        /// <param name="tfilename">目标文件名</param>
        /// <param name="rul">规则</param>
        public static void addtask(int ttype, string filename, string tfilename, string rul) {
            var now = DateTime.Now;
            DB.Insert("insert into ctasks (ttype,filename,tfilename,rul,tdesc,tstatus,createtime,starttime,endtime,errurl) values (" + ttype + ",'" + filename + "','" + tfilename + "','" + rul + "','等待',0,'" + now.ToString("yyyy-MM-dd HH:mm:ss") + "',null,null,null)");
            are.Set();
        }
        /// <summary>
        /// 字典转实体
        /// </summary>
        /// <param name="dic">字典</param>
        /// <returns>任务对象</returns>
        private static ctasks dic2ctasks(Dictionary<string, object> dic) {
            if (dic == null) return null;
            var ct = new ctasks();
            ct.id = dic.ContainsKey("id")?(int?)O2.O2I(dic["id"]):null;
            ct.ttype = dic.ContainsKey("ttype") ? (int?)O2.O2I(dic["ttype"]) : null;
            ct.filename = dic.ContainsKey("filename") ? dic["filename"].ToString() : null;
            ct.tfilename = dic.ContainsKey("tfilename") ? dic["tfilename"].ToString() : null;
            ct.rul = dic.ContainsKey("rul") ? dic["rul"].ToString() : null;
            ct.tdesc = dic.ContainsKey("tdesc") ? dic["tdesc"].ToString() : null;
            ct.tstatus = dic.ContainsKey("tstatus") ? (int?)O2.O2I(dic["tstatus"]) : null;
            ct.createtime = dic.ContainsKey("createtime") ? (DateTime?)O2.O2DT(dic["createtime"]) : null;
            ct.starttime = dic.ContainsKey("starttime") ? (DateTime?)O2.O2DT(dic["starttime"]) : null;
            ct.endtime = dic.ContainsKey("endtime") ? (DateTime?)O2.O2DT(dic["endtime"]) : null;
            ct.errurl = dic.ContainsKey("errurl") ? dic["errurl"].ToString() : null;

            return ct;
        }
        /// <summary>
        /// 处理任务方法
        /// </summary>
        /// <param name="ct">任务对象</param>
        /// <param name="msg">错误详细信息</param>
        /// <returns>成功失败</returns>
        private static bool dealtask(ctasks ct,out string msg)
        {
            msg = "";
            var res = true;
            try {
                switch (ct.ttype.Value) { 
                    case 1:
                        res=importSend(ct,out msg);
                        break;
                    case 2:
                        res = importNetManager(ct, out msg);
                        break;
                    case 3:
                        res = computationNew(ct, out msg);
                        break;
                    case 4:
                        res = importLterru(ct, out msg);
                        break;
                    case 5:
                        res = computationDuibi(ct,out msg);
                        break;
                    case 11:
                        res = importTable1(ct, out msg);
                        break;
                    default:
                        res = false;
                        msg = "任务类型不匹配";
                        break;
                }
            }
            catch (Exception ex) {
                res = false;
                msg = ex.Message;
            }

            return res;
        }
        //public static string[] constcitys = { "保定", "沧州", "承德", "邯郸", "衡水", "廊坊", "秦皇岛", "石家庄", "唐山", "邢台", "雄安", "张家口" };
        public static List<string> constcitys =new List<string> { "石家庄", "唐山", "秦皇岛", "邯郸", "邢台", "保定", "张家口", "承德", "沧州", "廊坊", "衡水", "雄安" };
        public static List<string> constcitynos =new List<string> { "81301", "81302", "81303", "81304", "81305", "81306", "81307", "81308", "81309", "81310", "81311", "81312" };
        public static string[] constchangs = { "华为", "中兴", "诺基亚" };
        public static string[] constleixing = { "RRU", "PRRU", "AAU", "4T4R" };
        public static string[] constleixing2 = { "RRU", "PRRU", "AAU", "4T4R" };
        public static string[] constpinduan = { "L800M", "L1.8G", "L2.1G","L2.6G","L1.8G+L2.1G" };
        public static string[] constpinduan2 = { "800M", "1.8G", "2.1G", "2.6G", "1.8G+2.1G" };
        public static string[] constoutin = { "室外", "室内" };

        public static string[] constcfugai = {"室外覆盖","室分覆盖","未知" };
        public static string[] constpinduannc = { "800M", "1.8G", "2.1G", "未知" };
        /// <summary>
        /// 发货表导入
        /// </summary>
        /// <param name="ct">任务对象</param>
        /// <param name="msg">错误详细信息</param>
        /// <returns>成功失败</returns>
        private static bool importSend(ctasks ct, out string msg) {
            msg = "";
            var res = true;
            #region 发货表导入代码
            //开始
            var tmppath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\nsendtmp");
            var newfilename = System.IO.Path.Combine(tmppath,ct.filename);
            var exname = ct.filename.Substring(ct.filename.LastIndexOf(".") + 1).ToLower();

            var lastpath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\report");
            var lastfiles = System.IO.Directory.GetFiles(lastpath,"*.csv");
            var lastfilename = lastfiles == null || lastfiles.Length <= 0 ? null : lastfiles.OrderByDescending(a => a).FirstOrDefault();

            if (lastfilename != null) {
                var bjfilename = lastfilename.Substring(lastfilename.LastIndexOf("\\")+1);
                var bjjg = ct.tfilename.CompareTo(bjfilename);
                if (bjjg <= 0) {
                    msg = "文件日期不能早于最晚新发货表日期";
                    File.Delete(newfilename);
                    return false;
                }
            }

            var lastwpath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang");
            var lastwfiles = System.IO.Directory.GetFiles(lastwpath, "*.csv");
            var lastwfilename = lastwfiles == null || lastwfiles.Length <= 0 ? null : lastwfiles.OrderByDescending(a => a).FirstOrDefault();

            Dictionary<string,Dictionary<string, object>> lastdic = new Dictionary<string,Dictionary<string,object>>();
            Dictionary<string,Dictionary<string, object>> newdic = new Dictionary<string, Dictionary<string,object>>();
            Dictionary<string, object> lastwdic = new Dictionary<string, object>();

            if (lastwfilename != null) {
                readwangcsv(lastwfilename, lastwdic);
            }
            if (lastfilename != null) {
                string tmsg;
                readnsendcsv(lastfilename,lastwdic,null,lastdic,out tmsg);
            }
            if (exname == "xlsx") {
                readnsendexcel(newfilename, lastwdic, lastdic, newdic, out msg);
            } else if (exname == "csv") {
                readnsendcsv(newfilename, lastwdic, lastdic, newdic, out msg);
            
            } else {
                msg = "不支持的文件扩展名";
                File.Delete(newfilename);
                return false;
            }
            if (!string.IsNullOrEmpty(msg))
            {
                res = false;
            }
            else {
                StringBuilder sb = new StringBuilder();
                sb.Append("地市,厂家,RRU型号,RRU类型,RRU序列号,发货时间,借货期次,备注,是否八期,是否在网管,是否存在过网管,是否新发货" + "\r\n");
                foreach (var kv in lastdic) {
                    var cols = new List<string>();
                    foreach (var k in kv.Value) {
                        cols.Add(k.Value.GetType()==typeof(DateTime)?((DateTime)k.Value).ToString("yyyy/M/d"):k.Value.ToString());
                    }
                    sb.Append(string.Join(",",cols)+"\r\n");
                }
                foreach (var kv in newdic)
                {
                    var cols = new List<string>();
                    foreach (var k in kv.Value)
                    {
                        cols.Add(k.Value.GetType() == typeof(DateTime) ? ((DateTime)k.Value).ToString("yyyy/M/d") : k.Value.ToString());
                    }
                    sb.Append(string.Join(",", cols) + "\r\n");
                }
                if (!Directory.Exists(lastpath)) {
                    Directory.CreateDirectory(lastpath);
                }
                File.WriteAllText(Path.Combine(lastpath,ct.tfilename),sb.ToString(),Encoding.GetEncoding("gbk"));
            }
            File.Delete(newfilename);
            //结束
            #endregion
            return res;
        }
        /// <summary>
        /// 读取发货表，需要一个网管表字典（只存储rru序列号,用于匹配），对比发货表字典，只读取服务器数据的话留空null，一个用于装发货表数据的字典
        /// </summary>
        /// <param name="filename">发货表文件名带路径</param>
        /// <param name="lastwdic">用于对比的发货表字典，有方法可以直接获取</param>
        /// <param name="dbdic">对比发货表字典，只读取服务器数据的话留空null</param>
        /// <param name="dic">用于装发货表数据的字典</param>
        /// <param name="msg">返回消息，可以忽略</param>
        private static void readnsendcsv(string filename, Dictionary<string, object> lastwdic, Dictionary<string, Dictionary<string, object>> dbdic, Dictionary<string, Dictionary<string, object>> dic, out string msg)
        {
            msg = "";
            var nodb = false;
            if (dbdic == null) {
                nodb = true;
                dbdic = new Dictionary<string, Dictionary<string, object>>();
            }
            var rowno = 1;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("gbk"));
            sr.ReadLine();
            
            while (!sr.EndOfStream)
            {
                rowno++;
                var line = sr.ReadLine();
                Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                
                //var cols = line.Split(new char[] { ',' }, StringSplitOptions.None);
                var cols = reg.Split(line);
                if (cols.Length < 11) {
                    sb.Append("第"+rowno+"行，列数不足"+"\r\n");
                    continue;
                }
                var errline = new List<string>();
                var chong = false;
                var dicin = new Dictionary<string, object>();

                var city = cols[0].Trim();
                dicin["city"] = city;
                if (!constcitys.Contains(city))
                {
                    errline.Add("地市不正确");
                }
                var chang = cols[1].Trim();
                dicin["chang"] = chang;
                if (!constchangs.Contains(chang)) {
                    errline.Add("厂家不正确");
                }
                var xinghao = cols[2].Trim();
                dicin["xinghao"] = xinghao;
                if (string.IsNullOrEmpty(xinghao))
                {
                    errline.Add("RRU型号不能为空");
                }
                var leixing = cols[3].Trim().ToUpper();
                dicin["leixing"] = leixing;
                if (!constleixing.Contains(leixing)) {
                    errline.Add("RRU类型不正确");
                }
                var rruno = cols[4].ToUpper().Trim();
                dicin["rruno"] = rruno;
                if (string.IsNullOrEmpty(rruno)) {
                    errline.Add("rru序列号不能为空");
                    chong = true;
                }
                if (dic.ContainsKey(rruno))
                {
                    errline.Add("rru序列号本表重复");
                    chong = true;
                }
                if (dbdic.ContainsKey(rruno)) {
                    errline.Add("rru序列号旧表已存在");
                    chong = true;
                }
                var sendtime = cols[5].Trim();
                DateTime stime;
                if (string.IsNullOrEmpty(sendtime)|| !DateTime.TryParse(sendtime, out stime))
                {
                    errline.Add("发货时间不正确");
                }
                else {
                    dicin["sendtime"] = stime;
                }
                var qici = cols[6].Trim();
                dicin["qici"] = qici;
                var bz = cols[7].Trim();
                dicin["bz"] = bz;
                var baqi = cols[8].Trim();
                dicin["baqi"] = baqi;

                var sfzwg = "";
                var sfczg = nodb ? cols[10].Trim() : "";
                if (lastwdic.ContainsKey(rruno)) {
                    sfzwg = "1";
                    sfczg = "1";
                }
                dicin["sfzwg"] = sfzwg;
                dicin["sfczg"] = sfczg;

                var xfh = cols[11].Trim();
                dicin["xfh"] = xfh;

                if (errline.Count > 0) {
                    
                    sb.Append("第"+rowno+"行："+string.Join("、",errline)+"\r\n");
                }
                if (chong) continue;
                dic[rruno] = dicin;
            }
            sr.Close();
            fs.Close();
            if (rowno <= 1) {
                sb.Append("文件没有数据！\r\n");
            }
            if (sb.Length > 0)
            {
                msg = sb.ToString();
            }
        }
        /// <summary>
        /// 读取发货表excel，对比的话不需要，只是上传用
        /// </summary>
        /// <param name="filename">发货表文件名带路径</param>
        /// <param name="lastwdic">用于对比的发货表字典，有方法可以直接获取</param>
        /// <param name="dbdic">对比发货表字典，只读取服务器数据的话留空null</param>
        /// <param name="dic">用于装发货表数据的字典</param>
        /// <param name="msg">返回消息，可以忽略</param>
        private static void readnsendexcel(string filename,Dictionary<string, object> lastwdic, Dictionary<string, Dictionary<string, object>> dbdic, Dictionary<string, Dictionary<string, object>> dic, out string msg)
        {
            msg = "";
            var nodb = false;
            if (dbdic == null)
            {
                nodb = true;
                dbdic = new Dictionary<string, Dictionary<string, object>>();
            }
            var rowno = 1;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            var workbook = new XSSFWorkbook(fs);
            var sheet1 = workbook.GetSheetAt(0);
            while(rowno++<sheet1.LastRowNum+1){
                try {
                    var cols = sheet1.GetRow(rowno - 1);
                    var errline = new List<string>();
                    var chong = false;
                    var dicin = new Dictionary<string, object>();
                    var city = getcellval(cols, 0).Trim();
                    dicin["city"] = city;
                    if (!constcitys.Contains(city))
                    {
                        errline.Add("地市不正确");
                    }
                    var chang = getcellval(cols, 1).Trim();
                    dicin["chang"] = chang;
                    if (!constchangs.Contains(chang))
                    {
                        errline.Add("厂家不正确");
                    }
                    var xinghao = getcellval(cols, 2).Trim();
                    dicin["xinghao"] = xinghao;
                    if (string.IsNullOrEmpty(xinghao))
                    {
                        errline.Add("RRU型号不能为空");
                    }
                    var leixing = getcellval(cols, 3).Trim().ToUpper();
                    dicin["leixing"] = leixing;
                    if (!constleixing.Contains(leixing))
                    {
                        errline.Add("RRU类型不正确");
                    }
                    var rruno = getcellval(cols, 4).ToUpper().Trim();
                    dicin["rruno"] = rruno;
                    if (string.IsNullOrEmpty(rruno))
                    {
                        errline.Add("rru序列号不能为空");
                        chong = true;
                    }
                    if (dic.ContainsKey(rruno))
                    {
                        errline.Add("rru序列号本表重复");
                        chong = true;
                    }
                    if (dbdic.ContainsKey(rruno))
                    {
                        errline.Add("rru序列号旧表已存在");
                        chong = true;
                    }
                    var sendtime = getcellval(cols, 5).Trim();
                    DateTime stime;
                    if (string.IsNullOrEmpty(sendtime)|| !DateTime.TryParse(sendtime, out stime))
                    {
                        errline.Add("发货时间不正确");
                    }
                    else
                    {
                        dicin["sendtime"] = stime;
                    }
                    var qici = getcellval(cols, 6).Trim();
                    dicin["qici"] = qici;
                    var bz = getcellval(cols, 7).Trim();
                    dicin["bz"] = bz;
                    var baqi = getcellval(cols, 8).Trim();
                    dicin["baqi"] = baqi;

                    var sfzwg = "";
                    var sfczg = nodb ? getcellval(cols, 9).Trim() : "";
                    if (lastwdic.ContainsKey(rruno))
                    {
                        sfzwg = "1";
                        sfczg = "1";
                    }
                    dicin["sfzwg"] = sfzwg;
                    dicin["sfczg"] = sfczg;

                    var xfh = getcellval(cols, 11).Trim();
                    dicin["xfh"] = xfh;

                    if (errline.Count > 0)
                    {

                        sb.Append("第" + rowno + "行：" + string.Join("、", errline) + "\r\n");
                    }
                    if (chong) continue;
                    dic[rruno] = dicin;
                }
                catch {
                    sb.Append("第" + rowno + "行，数据异常读取失败" + "\r\n");
                    continue;
                }
            }
            

            workbook.Close();
            fs.Close();
            if (rowno <= 2)
            {
                sb.Append("文件没有数据！\r\n");
            }
            if (sb.Length > 0)
            {
                msg = sb.ToString();
            }
        }
        /// <summary>
        /// 获取excel数据值，用做csv文件写入，处理空值、双引号问题
        /// </summary>
        /// <param name="row"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        private static string getcellval(IRow row, int no) {
            var res=row.GetCell(no) + "";
            res = res.Replace("\"","\"\"");
            return res.IndexOf(",") < 0 ? res : "\"" +res+ "\"";
        }
        /// <summary>
        /// 读取网管表,只读rru序列号，用于对比或更新
        /// </summary>
        /// <param name="filename">文件名带路径</param>
        /// <param name="dic">用来装数据的字典</param>
        private static void readwangcsv(string filename, Dictionary<string, object> dic) {
            System.IO.FileStream fs = new System.IO.FileStream(filename,System.IO.FileMode.Open,System.IO.FileAccess.Read);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs,System.Text.Encoding.GetEncoding("gbk"));
            sr.ReadLine();
            while (!sr.EndOfStream) {
                var line=sr.ReadLine();
                Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var cols = reg.Split(line);
                //var cols = line.Split(new char[]{','},StringSplitOptions.None);
                if (cols.Length < 10) continue;
                dic[cols[9].ToUpper().Trim()] = null;
            }
            sr.Close();
            fs.Close();
        }

        //private static void readnsendcsv(string filename, Dictionary<string, object> dic)
        //{
        //    System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //    System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("gbk"));
        //    sr.ReadLine();
        //    while (!sr.EndOfStream)
        //    {
        //        var line = sr.ReadLine();
        //        Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        //        var cols = reg.Split(line);
        //        //var cols = line.Split(new char[]{','},StringSplitOptions.None);
        //        if (cols.Length < 5) continue;
        //        dic[cols[4].ToUpper().Trim()] = null;
        //    }
        //    sr.Close();
        //    fs.Close();
        //}
        /// <summary>
        /// 网管详表导入
        /// </summary>
        /// <param name="ct">任务对象</param>
        /// <param name="msg">错误详细信息</param>
        /// <returns>成功失败</returns>
        private static bool importNetManager(ctasks ct, out string msg)
        {
            msg = "";
            var res = true;
            #region 网管详表导入代码
            //开始
            var d = DateTime.Now;
            if (ct.filename.Substring(ct.filename.LastIndexOf(".") + 1).ToLower() == "xlsx")
            {
                var filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wangtmp\\" + ct.filename);
                string newName = ct.filename.Substring(0, ct.filename.LastIndexOf(".")) + ".csv";
                ct.filename = newName;
                string newPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wangtmp\\" + newName);

                res = xlsx2csv(filename, newPath, out msg);
                if (!res)
                {
                    return res;
                }
            }

            string wangPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wangtmp\\" + ct.filename);

            var lastwpath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang");
            var lastwfiles = System.IO.Directory.GetFiles(lastwpath, "*.csv");
            var lastwfilename = lastwfiles == null || lastwfiles.Length <= 0 ? null : lastwfiles.OrderByDescending(a => a).FirstOrDefault();
            Dictionary<string, object> lastwdic = new Dictionary<string, object>();

            if (lastwfilename != null)
            {
                var bjfilename = lastwfilename.Substring(lastwfilename.LastIndexOf("\\") + 1);
                var bjjg = ct.tfilename.CompareTo(bjfilename);
                if (bjjg <= 0)
                {
                    msg = "文件日期不能早于最晚[网管详表]日期";
                    File.Delete(wangPath);
                    return false;
                }

                readwangcsv(lastwfilename, lastwdic);
            }

            var lastnspath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\report");
            var lastnsfiles = System.IO.Directory.GetFiles(lastnspath, "*.csv");
            var lastnsfilename = lastnsfiles == null || lastnsfiles.Length <= 0 ? null : lastnsfiles.OrderByDescending(a => a).FirstOrDefault();
            var lastnsdic = new Dictionary<string, Dictionary<string, object>>();


            if (lastnsfilename != null)
            {
                var bjfilename = lastnsfilename.Substring(lastnsfilename.LastIndexOf("\\") + 1);
                var bjjg = ct.tfilename.CompareTo(bjfilename);
                if (bjjg <= 0)
                {
                    msg = "文件日期不能早于最晚[新发货表]日期";
                    File.Delete(wangPath);
                    return false;
                }

                string tmsg;
                readnsendcsv(lastnsfilename, lastwdic, null, lastnsdic, out tmsg);
            }

            Dictionary<string, object> newwdic = new Dictionary<string, object>();
            Dictionary<string, Dictionary<string, object>> dic = new Dictionary<string, Dictionary<string, object>>();

            

            readwangcsv(wangPath, lastwdic, lastnsdic, newwdic, dic, out msg);

            if (!string.IsNullOrEmpty(msg))
            {
                File.Delete(wangPath);
                return false;
            }
            var lasttjpath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wangtj");
            var lasttjfiles = System.IO.Directory.GetFiles(lasttjpath, "*.csv");
            var lasttjfilename = lasttjfiles == null || lasttjfiles.Length <= 0 ? null : lasttjfiles.OrderByDescending(a => a).FirstOrDefault();
            var lasttjdic = new Dictionary<string, Dictionary<string, object>>();
            if (lasttjfilename != null)
            {
                string tmsg;
                readwangcsv(lasttjfilename, lastwdic, lastnsdic, null, lasttjdic, out tmsg);
            }
            var ttimes= ct.tfilename.Substring(0,4)+"/"+ct.tfilename.Substring(4,2)+"/"+ct.tfilename.Substring(6,2);
            var ttime = O2.O2DT(ttimes);
            foreach (var v in dic)
            {
                if (!lasttjdic.ContainsKey(v.Key))
                {
                    lasttjdic[v.Key] = new Dictionary<string, object>();
                }
                foreach (var v2 in v.Value)
                {
                    lasttjdic[v.Key][v2.Key] = v2.Value;
                }
                lasttjdic[v.Key]["xwsfcz"] = "是";
                lasttjdic[v.Key]["chtime"] = ttime.ToString("yyyy/M/d");
            }

            #region 删除
            //string wang = File.ReadAllText(wangPath, Encoding.Default);
            //string[] wangs = wang.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            //FileInfo report = null;
            //DirectoryInfo dir = new DirectoryInfo(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\report"));
            //if(dir.Exists)
            //{
            //    FileInfo[] reportFiles = dir.GetFiles();
            //    if(reportFiles.Length > 0)
            //    {
            //        report = reportFiles[0];
            //        foreach(FileInfo fi in reportFiles)
            //        {
            //            if(String.Compare(fi.Name.Substring(0, 14), report.Name.Substring(0,14)) > 0)
            //            {
            //                report = fi;
            //            }
            //        }
            //    }
            //}
            //if (report != null)
            //{
            //    string rep = File.ReadAllText(report.FullName, Encoding.Default);
            //    string[] reps = rep.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            //    List<string> keys = new List<string>();
            //    for (int p=1;p<reps.Length;p++)
            //    {
            //        var c = SpliteLine(reps[p]);
            //        if (c == null) continue;
            //        keys.Add(c[2]);
            //    }
            //    for (int p = 1; p < wangs.Length; p++)
            //    {
            //        var c = SpliteLine(wangs[p]);
            //        if (c == null) continue;
            //        if (keys.Contains(c[6]))
            //        {
            //            c[13] = "是";
            //        }
            //        else
            //        {
            //            c[13] = "否";
            //        }
            //        wangs[p] = String.Join(",", c);
            //    }
            //}
            //string wangSumPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang\\WangSumReport.csv");
            //if(!File.Exists(wangSumPath))
            //{
            //    List<string> exixtsKeys = new List<string>();
            //    FileStream fs = File.Create(wangSumPath);
            //    StringBuilder stb = new StringBuilder();
            //    stb.Append("地市,厂家,BBU名称,机框号,eNodeB ID,频段,RRU型号,RRU类型,RRU名称,RRU序列号,备注,相对2月21日是否新增,生产日期,新增是否从新发货来,现网是否存在,最近出现时间\r\n");
            //    for(int i=1; i<wangs.Length;i++)
            //    {
            //        if (wangs[i] == "") continue;
            //        stb.Append(wangs[i]+",否,"+d+"\r\n");
            //    }
            //    byte[] bs = Encoding.Default.GetBytes(stb.ToString());
            //    fs.Write(bs, 0, bs.Length);
            //    fs.Flush();
            //    fs.Close();
            //}
            //else
            //{
            //    string wangSum = File.ReadAllText(wangSumPath, Encoding.Default);
            //    string[] wangSums = wangSum.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            //    List<int> repeat = new List<int>();
            //    Regex regex = new Regex("\".*?\"");
            //    for (int p = 1; p < wangSums.Length; p++)
            //    {
            //        var c = SpliteLine(wangSums[p]);
            //        if (c == null) continue;
            //        for (int p2 = 1; p2 < wangs.Length; p2++)
            //        {
            //            var c2 = SpliteLine(wangs[p2]);
            //            if (c2 == null) continue;
            //            if (c[6] == c2[6])
            //            {
            //                repeat.Add(p2);
            //                wangSums[p] = wangs[p2];
            //                wangSums[p] += (",是," + d);
            //            }
            //        }
            //    }
            //    List<string> wangSumExtends = new List<string>();
            //    for(int i=1;i<wangs.Length;i++)
            //    {
            //        if (repeat.Contains(i) || wangs[i] == "") continue;
            //        wangSumExtends.Add(wangs[i] + ",否," + d);
            //    }
            //    var newWangSum = String.Join("\r\n", wangSumExtends.Union(new List<string>(wangSums)));
            //    var newWang = String.Join("\r\n", wangs);
            //    File.WriteAllText(wangSumPath, Encoding.Default.GetString(Encoding.Default.GetBytes(newWangSum)));
            //    File.WriteAllText(wangPath, Encoding.Default.GetString(Encoding.Default.GetBytes(newWang)));
            //}
            //var ss = "";
            //var a = new Dictionary<string, object>();
            //var b = new Dictionary<string, Dictionary<string, object>>();
            //foreach (var p in dic.Keys)
            //{
            //    a.Add(p, null);
            //}
            //readnsendcsv(report.FullName, a, null, b, out ss);
            #endregion
            lastnsdic.Clear();
            if (lastnsfilename != null)
            {
                string tmsg;
                readnsendcsv(lastnsfilename, newwdic, null, lastnsdic, out tmsg);
            }

            StringBuilder sbres = new StringBuilder();

            sbres.Append("地市,厂家,BBU名称,机框号,eNodeB ID,频段,RRU型号,RRU类型,RRU名称,RRU序列号,备注,相对上一次是否新增,生产日期,新增是否从新发货来\r\n");
            foreach (var kv in dic)
            {
                var cols = new List<string>();
                foreach (var k in kv.Value)
                {
                    cols.Add(k.Value.GetType() == typeof(DateTime) ? ((DateTime)k.Value).ToString("yyyy/M/d") : k.Value.ToString());
                }
                sbres.Append(string.Join(",", cols) + "\r\n");
            }
            if (!Directory.Exists(lastwpath)) {
                Directory.CreateDirectory(lastwpath);
            }
            File.WriteAllText(Path.Combine(lastwpath, ct.tfilename), sbres.ToString(), Encoding.GetEncoding("gbk"));

            sbres.Clear();
            sbres.Append("地市,厂家,BBU名称,机框号,eNodeB ID,频段,RRU型号,RRU类型,RRU名称,RRU序列号,备注,相对上一次是否新增,生产日期,新增是否从新发货来,现网是否存在,最近出现日期\r\n");
            foreach (var kv in lasttjdic)
            {
                var cols = new List<string>();
                foreach (var k in kv.Value)
                {
                    cols.Add(k.Value.GetType() == typeof(DateTime) ? ((DateTime)k.Value).ToString("yyyy/M/d") : k.Value.ToString());
                }
                sbres.Append(string.Join(",", cols) + "\r\n");
            }
            if (!Directory.Exists(lasttjpath))
            {
                Directory.CreateDirectory(lasttjpath);
            }
            File.WriteAllText(Path.Combine(lasttjpath, ct.tfilename), sbres.ToString(), Encoding.GetEncoding("gbk"));

            sbres.Clear();
            sbres.Append("地市,厂家,RRU型号,RRU类型,RRU序列号,发货时间,借货期次,备注,是否八期,是否在网管,是否存在过网管,是否新发货\r\n");
            foreach (var kv in lastnsdic)
            {
                var cols = new List<string>();
                foreach (var k in kv.Value)
                {
                    cols.Add(k.Value.GetType() == typeof(DateTime) ? ((DateTime)k.Value).ToString("yyyy/M/d") : k.Value.ToString());
                }
                sbres.Append(string.Join(",", cols) + "\r\n");
            }
            if (!Directory.Exists(lastnspath))
            {
                Directory.CreateDirectory(lastnspath);
            }
            File.WriteAllText(Path.Combine(lastnspath, ct.tfilename), sbres.ToString(), Encoding.GetEncoding("gbk"));
            File.Delete(wangPath);

            #region 统计表
            sbres.Clear();
            lastnsdic.Clear();
            newwdic.Clear();
            lastwdic.Clear();

            var lastwdicds = new Dictionary<string, Dictionary<string, object>>();
            if (lastwfilename != null) {
                string tmsg;
                readwangcsv(lastwfilename, lastwdic, lastnsdic, newwdic, lastwdicds, out tmsg);
                newwdic.Clear();
            }
            var dicds = lastwdicds.Where(a => !dic.ContainsKey(a.Key)).GroupBy(a => a.Value["city"] + "_" + a.Value["chang"]).Select(a => new Dictionary<string, object> { 
            { "city", a.Max(b => b.Value["city"]) }, { "chang", a.Max(b => b.Value["chang"]) }, 
            {"rrumu",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString()=="L800M"?1:0)},{"rrugu",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString().EndsWith("G")&&b.Value["pinduan"].ToString()!="L2.6G"?1:0)},{"prruu",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"?1:0)}
            }).ToDictionary(a => a["city"] + "_" + a["chang"]);

            var dictj = dic.GroupBy(a => a.Value["city"] + "_" + a.Value["chang"]).Select(a => new Dictionary<string, object> { 
            { "city", a.Max(b => b.Value["city"]) }, { "chang", a.Max(b => b.Value["chang"]) }, 
            {"rrum",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString()=="L800M"?1:0)},{"rrug",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString().EndsWith("G")&&b.Value["pinduan"].ToString()!="L2.6G"?1:0)},{"prru",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"?1:0)},
            {"rrum2",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString()=="L800M"&&b.Value["xz"].ToString()=="是"?1:0)},{"rrug2",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString().EndsWith("G")&&b.Value["pinduan"].ToString()!="L2.6G"&&b.Value["xz"].ToString()=="是"?1:0)},{"prru2",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"&&b.Value["xz"].ToString()=="是"?1:0)},
            {"rrum3",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString()=="L800M"&&b.Value["xz"].ToString()=="是"&&b.Value["fh"].ToString()=="是"?1:0)},{"rrug3",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString().EndsWith("G")&&b.Value["pinduan"].ToString()!="L2.6G"&&b.Value["xz"].ToString()=="是"&&b.Value["fh"].ToString()=="是"?1:0)},{"prru3",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"&&b.Value["xz"].ToString()=="是"&&b.Value["fh"].ToString()=="是"?1:0)},
            {"rrum4",0},{"rrug4",0},{"prru4",0},
            //{"rrum5",a.Sum(b=>b.Value["leixing"].ToString()=="RRU"&&b.Value["pinduan"].ToString()=="L800M"?1:0)},{"rrug5",a.Sum(b=>b.Value["leixing"].ToString()=="RRU"&&b.Value["pinduan"].ToString().EndsWith("G")?1:0)},{"prru5",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"?1:0)},
            {"rrum5",0},{"rrug5",0},{"prru5",0},
            {"rrumu",0},{"rrugu",0},{"prruu",0},
            }).ToDictionary(a=>a["city"]+"_"+a["chang"]);

            var dictj2 = lasttjdic.GroupBy(a => a.Value["city"] + "_" + a.Value["chang"]).Select(a => new Dictionary<string, object> { 
            { "city", a.Max(b => b.Value["city"]) }, { "chang", a.Max(b => b.Value["chang"]) }, 
            {"rrum4",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString()=="L800M"?1:0)},{"rrug4",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString().EndsWith("G")&&b.Value["pinduan"].ToString()!="L2.6G"?1:0)},{"prru4",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"?1:0)}
            }).ToDictionary(a => a["city"] + "_" + a["chang"]);

            dic.Clear();
            lasttjdic.Clear();

            foreach (var dd in dictj2) {
                if (!dictj.ContainsKey(dd.Key)) {
                    dictj[dd.Key] = new Dictionary<string, object>();
                    dictj[dd.Key]["city"] = dd.Value["city"];
                    dictj[dd.Key]["chang"] = dd.Value["chang"];
                    dictj[dd.Key]["rrum"] = 0;
                    dictj[dd.Key]["rrug"] = 0;
                    dictj[dd.Key]["prru"] = 0;
                    dictj[dd.Key]["rrum2"] = 0;
                    dictj[dd.Key]["rrug2"] = 0;
                    dictj[dd.Key]["prru2"] = 0;
                    dictj[dd.Key]["rrum3"] = 0;
                    dictj[dd.Key]["rrug3"] = 0;
                    dictj[dd.Key]["prru3"] = 0;
                }
                dictj[dd.Key]["rrum4"] = dd.Value["rrum4"];
                dictj[dd.Key]["rrug4"] = dd.Value["rrug4"];
                dictj[dd.Key]["prru4"] = dd.Value["prru4"];

                //dictj[dd.Key]["rrum5"] = (int)dictj[dd.Key]["rrum"] - (int)dictj[dd.Key]["rrum4"];
                //dictj[dd.Key]["rrug5"] = (int)dictj[dd.Key]["rrug"] - (int)dictj[dd.Key]["rrug4"];
                //dictj[dd.Key]["prru5"] = (int)dictj[dd.Key]["prru"] - (int)dictj[dd.Key]["prru4"];
                dictj[dd.Key]["rrum5"] = 0;
                dictj[dd.Key]["rrug5"] = 0;
                dictj[dd.Key]["prru5"] = 0;

                dictj[dd.Key]["rrumu"] = 0;
                dictj[dd.Key]["rrugu"] = 0;
                dictj[dd.Key]["prruu"] = 0;
            }
            foreach (var dd in dicds)
            {
                if (!dictj.ContainsKey(dd.Key))
                {
                    dictj[dd.Key] = new Dictionary<string, object>();
                    dictj[dd.Key]["city"] = dd.Value["city"];
                    dictj[dd.Key]["chang"] = dd.Value["chang"];
                    dictj[dd.Key]["rrum"] = 0;
                    dictj[dd.Key]["rrug"] = 0;
                    dictj[dd.Key]["prru"] = 0;
                    dictj[dd.Key]["rrum2"] = 0;
                    dictj[dd.Key]["rrug2"] = 0;
                    dictj[dd.Key]["prru2"] = 0;
                    dictj[dd.Key]["rrum3"] = 0;
                    dictj[dd.Key]["rrug3"] = 0;
                    dictj[dd.Key]["prru3"] = 0;
                    dictj[dd.Key]["rrum4"] = 0;
                    dictj[dd.Key]["rrug4"] = 0;
                    dictj[dd.Key]["prru4"] = 0;
                    dictj[dd.Key]["rrum5"] = 0;
                    dictj[dd.Key]["rrug5"] = 0;
                    dictj[dd.Key]["prru5"] = 0;
                }


                dictj[dd.Key]["rrumu"] = dd.Value["rrumu"];
                dictj[dd.Key]["rrugu"] = dd.Value["rrugu"];
                dictj[dd.Key]["prruu"] = dd.Value["prruu"];
            }
            sbres.Append("city,chang,rrum,rrug,prru,rrum2,rrug2,prru2,rrum3,rrug3,prru3,rrum4,rrug4,prru4,rrum5,rrug5,prru5,rrumu,rrugu,prruu\r\n");
            foreach (var dd in dictj) {
                sbres.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}\r\n"
                    , dd.Value["city"], dd.Value["chang"]
                    , dd.Value["rrum"], dd.Value["rrug"], dd.Value["prru"]
                    , dd.Value["rrum2"], dd.Value["rrug2"], dd.Value["prru2"]
                    , dd.Value["rrum3"], dd.Value["rrug3"], dd.Value["prru3"]
                    , dd.Value["rrum4"], dd.Value["rrug4"], dd.Value["prru4"]
                    , dd.Value["rrum5"], dd.Value["rrug5"], dd.Value["prru5"]
                    , dd.Value["rrumu"], dd.Value["rrugu"], dd.Value["prruu"]));
            }
            var wangtongjipath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wangtongji");
            if (!Directory.Exists(wangtongjipath))
            {
                Directory.CreateDirectory(wangtongjipath);
            }
            File.WriteAllText(Path.Combine(wangtongjipath, ct.tfilename), sbres.ToString(), Encoding.GetEncoding("gbk"));
            #endregion

            var wangfiles = Directory.GetFiles(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"rrufiles\wang"), "*.csv");
            var top5 = wangfiles.OrderByDescending(a => a).Skip(1).Take(5).ToArray();
            foreach (var top in top5)
            {
                ctasksHandle.addtask(3, ct.tfilename, top.Substring(top.LastIndexOf('\\') + 1), "");
            }
            //结束
            #endregion
            return res;
        }
        private static void readwangcsv(string filename,Dictionary<string,object> lastwdic,Dictionary<string,Dictionary<string,object>> lastnsdic,Dictionary<string,object> newwdic,Dictionary<string,Dictionary<string,object>> dic,out string msg) {
            msg = "";
            var wtj = false;
            if (newwdic == null) {
                wtj = true;
                newwdic = new Dictionary<string, object>();
            }
            var rowno = 1;
            System.Text.StringBuilder sbr = new System.Text.StringBuilder();
            System.IO.FileStream fsm = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.StreamReader sr = new System.IO.StreamReader(fsm, System.Text.Encoding.GetEncoding("gbk"));
            sr.ReadLine();

            while (!sr.EndOfStream)
            {
                rowno++;
                var line = sr.ReadLine();
                Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var cols = reg.Split(line);
                if (cols.Length < 14)
                {
                    sbr.Append("第" + rowno + "行，列数不足" + "\r\n");
                    continue;
                }
                var errline = new List<string>();
                var chong = false;
                var dicin = new Dictionary<string, object>();
                var city = cols[0].Trim();
                dicin["city"] = city;
                if (!constcitys.Contains(city))
                {
                    errline.Add("地市不正确");
                }
                var chang = cols[1].Trim();
                dicin["chang"] = chang;
                if (!constchangs.Contains(chang))
                {
                    errline.Add("厂家不正确");
                }
                var bname = cols[2].Trim();
                dicin["bname"] = bname;
                if (string.IsNullOrEmpty(bname))
                {
                    errline.Add("BBU名称不能为空");
                }

                var knos = cols[3].Trim();
                //int kno;
                //if (!int.TryParse(knos, out kno))
                //{
                //    errline.Add("机框号必须是数字且不能为空");
                //}
                dicin["kno"] = knos;
                if (knos == "") {
                    errline.Add("机框号不能为空");
                }

                var bids = cols[4].Trim();
                int bid;
                if (bids == "") {
                    dicin["bid"] = bids;
                    errline.Add("eNodeBID不能为空");
                }
                else if (!int.TryParse(bids, out bid))
                {
                    errline.Add("eNodeBID必须是数字");
                }
                else {
                    dicin["bid"] = bid;
                }
                
                var pinduan = cols[5].Trim().ToUpper();
                dicin["pinduan"] = pinduan;
                if (!constpinduan.Contains(pinduan))
                {
                    errline.Add("频段不正确");
                }

                var xinghao = cols[6].Trim();
                dicin["xinghao"] = xinghao;
                if (string.IsNullOrEmpty(xinghao))
                {
                    errline.Add("RRU型号不能为空");
                }
                var leixing = cols[7].Trim().ToUpper();
                dicin["leixing"] = leixing;
                if (!constleixing.Contains(leixing))
                {
                    errline.Add("RRU类型不正确");
                }

                var rname = cols[8].Trim();
                dicin["rname"] = rname;
                if (string.IsNullOrEmpty(rname))
                {
                    errline.Add("RRU名称不能为空");
                }

                var rruno = cols[9].ToUpper().Trim();
                dicin["rruno"] = rruno;
                if (string.IsNullOrEmpty(rruno))
                {
                    errline.Add("rru序列号不能为空");
                    chong = true;
                }
                if (dic.ContainsKey(rruno))
                {
                    errline.Add("rru序列号本表重复");
                    chong = true;
                }

                var bz = cols[10].Trim();
                dicin["bz"] = bz;
                var xz = "否";
                if (!lastwdic.ContainsKey(rruno))
                {
                    xz = "是";
                }
                dicin["xz"] = xz;

                //var ctime = cols[12].Trim();
                //dicin["ctime"] = ctime;

                var stimes = cols[12].Trim();
                DateTime stime;
                if (stimes == "") {
                    dicin["ctime"] = stimes;
                }else if(!DateTime.TryParse(stimes, out stime))
                {
                    errline.Add("生产日期不正确");
                }
                else
                {
                    dicin["ctime"] = stime;
                }

                var fh = "否";
                if (lastnsdic.ContainsKey(rruno))
                {
                    fh = "是";
                }
                dicin["fh"] = fh;
                if (wtj) {
                    dicin["xwsfcz"] = "否";
                    dicin["chtime"] = "";
                    var chtime = cols[15].Trim();
                    DateTime dtt;
                    if (DateTime.TryParse(chtime, out dtt)) {
                        dicin["chtime"] = dtt;
                    }
                    
                }
                if (errline.Count > 0)
                {
                    sbr.Append("第" + rowno + "行：" + string.Join("、", errline) + "\r\n");
                }
                if (chong) continue;
                dic[rruno] = dicin;
                newwdic[rruno] = null;
            }
            sr.Close();
            fsm.Close();
            if (rowno <= 1)
            {
                sbr.Append("文件没有数据！\r\n");
            }
            if (sbr.Length > 0)
            {
                msg = sbr.ToString();
            }
        }
        private static string[] SpliteLine(string s)
        {
            if (s == "") return null;
            Regex regex = new Regex("\".*?\"");
            var a = regex.Matches(s).Cast<Match>().Select(m => m.Value).ToList();
            var b = regex.Replace(s, "%_%");
            var c = b.Split(',');
            for (int i = 0, j = 0; i < c.Length && j < a.Count; i++)
            {
                if (c[i] == "%_%")
                {
                    c[i] = a[j++];
                }
            }
            return c;
        }
        /// <summary>
        /// 计算新增
        /// </summary>
        /// <param name="ct">任务对象</param>
        /// <param name="msg">错误详细信息</param>
        /// <returns>成功失败</returns>
        private static bool computationNew(ctasks ct, out string msg)
        {
            msg = "";
            var res = true;
            #region 计算新增代码
            //开始
            var filepath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang");
            var filename = Path.Combine(filepath,ct.filename);
            var tfilename = Path.Combine(filepath, ct.tfilename);

            Dictionary<string, Dictionary<string, object>> filedic = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, object> tfiledic = new Dictionary<string, object>();

            readwangcsv(tfilename,tfiledic);
            string tt;
            readwangcsv(filename,tfiledic,filedic,out tt);

            if (tt.Contains(ct.tfilename.Substring(0, ct.tfilename.LastIndexOf(".")))) {
                msg = "新增计算失败,此文件已经计算过了";
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(tt+",相对于"+ct.tfilename.Substring(0,ct.tfilename.LastIndexOf("."))+"是否新增"+"\r\n");

            foreach (var v in filedic) {
                sb.Append(v.Value["text"]+","+v.Value["xinzeng"]+"\r\n");
            }

            File.WriteAllText(filename,sb.ToString(),Encoding.GetEncoding("gbk"));

            //结束
            #endregion
            return res;
        }
        /// <summary>
        /// 读取网管表,只读rru序列号和其他列的文本，不处理分割列，用于对比或更新
        /// </summary>
        /// <param name="filename">文件名带路径</param>
        /// <param name="tdic">用于对比的网管表字典，用另一个同名方法获取</param>
        /// <param name="dic">用于装数据的字典</param>
        /// <param name="tt">读取标题</param>
        private static void readwangcsv(string filename,Dictionary<string, object> tdic,Dictionary<string, Dictionary<string, object>> dic,out string tt) {
            tt = "";
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("gbk"));
            tt=sr.ReadLine();
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var cols = reg.Split(line);
                if (cols.Length < 10) continue;
                var rruno=cols[9].ToUpper().Trim();
                var dicin = new Dictionary<string, object>();
                if (tdic.ContainsKey(rruno))
                {
                    dicin["xinzeng"] = "否";
                }
                else {
                    dicin["xinzeng"] = "是";
                }
                dicin["text"] = line;
                dic[rruno] = dicin;
            }
            sr.Close();
            fs.Close();
        }
        /// <summary>
        /// lte rru核对表导入
        /// </summary>
        /// <param name="ct">任务对象</param>
        /// <param name="msg">错误详细信息</param>
        /// <returns>成功失败</returns>
        private static bool importLterru(ctasks ct, out string msg)
        {
            msg = "";
            var res = true;
            #region lte rru核对表导入代码
            //开始
            if (ct.filename.Substring(ct.filename.LastIndexOf(".") + 1).ToLower() == "xlsx")
            {
                var filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\lterrutmp\\" + ct.filename);
                string newName = ct.filename.Substring(0, ct.filename.LastIndexOf(".")) + ".csv";
                ct.filename = newName;
                string newPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\lterrutmp\\" + newName);

                res = xlsx2csv(filename, newPath, out msg);
                if (!res)
                {
                    return res;
                }
            }
            var dtres = DB.Query("select top 0 * from r_lterru");
            dtres.Columns.Remove("id");
            var filepath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\lterrutmp");
            var filefullname=Path.Combine(filepath,ct.filename);
            var rowno = 1;
            System.Text.StringBuilder sbr = new System.Text.StringBuilder();
            System.IO.FileStream fsm = new System.IO.FileStream(filefullname, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.StreamReader sr = new System.IO.StreamReader(fsm, System.Text.Encoding.GetEncoding("gbk"));
            sr.ReadLine();

            while (!sr.EndOfStream)
            {
                rowno++;
                var line = sr.ReadLine();
                Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var cols = reg.Split(line);
                var aaa=cols.Select(a => {
                    var b = a.Replace("\"\"", "\"");
                    b = b.IndexOf(",") >= 0 ? b.Substring(1, b.Length - 2) : b;
                    return b; 
                }).ToArray();
                if (cols.Length < 13)
                {
                    sbr.Append("第" + rowno + "行，列数不足" + "\r\n");
                    continue;
                }
                var errline = new List<string>();
                var dicin=dtres.NewRow();

                var jtimes = cols[0].Trim();
                DateTime jtime;
                if (!DateTime.TryParse(jtimes, out jtime))
                {
                    errline.Add("借货时间不正确");
                }
                else
                {
                    dicin["borrowtime"] = jtime;
                }
                var city = cols[1].Trim();
                dicin["city"] = city;
                if (!constcitys.Contains(city))
                {
                    errline.Add("地市不正确");
                }
                dicin["stage"] = cols[2].Trim();

                var outin = cols[3].Trim();
                dicin["outin"] = outin;
                if (!constoutin.Contains(outin))
                {
                    errline.Add("室内/外不正确");
                }
                var leixing = cols[4].Trim().ToUpper();
                dicin["rru_type"] = leixing;
                if (!constleixing2.Contains(leixing)) {
                    errline.Add("类型不正确");
                }
                var pinduan = cols[5].Trim().ToUpper();
                dicin["pinduan"] = pinduan;
                if (!constpinduan2.Contains(pinduan))
                {
                    errline.Add("频段不正确");
                }
                var chang = cols[6].Trim();
                dicin["chang"] = chang;
                if (!constchangs.Contains(chang))
                {
                    errline.Add("厂家不正确");
                }
                var borrows = cols[7].Trim();
                if (borrows == "")
                {
                    dicin["borrow"] = DBNull.Value;
                }
                else { 
                    int borrow;
                    if (int.TryParse(borrows, out borrow))
                    {
                        dicin["borrow"] = borrow;
                    }
                    else {
                        errline.Add("借货必须是数字");
                    }
                }

                var arrives = cols[8].Trim();
                if (arrives == "")
                {
                    dicin["arrive"] = DBNull.Value;
                }
                else
                {
                    int arrive;
                    if (int.TryParse(arrives, out arrive))
                    {
                        dicin["arrive"] = arrive;
                    }
                    else
                    {
                        errline.Add("到货必须是数字");
                    }
                }

                var needs = cols[9].Trim();
                if (needs == "")
                {
                    dicin["need"] = DBNull.Value;
                }
                else
                {
                    int need;
                    if (int.TryParse(needs, out need))
                    {
                        dicin["need"] = need;
                    }
                    else
                    {
                        errline.Add("仍需到货到货必须是数字");
                    }
                }

                dicin["remark"] = cols[10].Trim();

                var frees = cols[11].Trim();
                if (frees == "")
                {
                    dicin["free"] = DBNull.Value;
                }
                else
                {
                    int free;
                    if (int.TryParse(frees, out free))
                    {
                        dicin["free"] = free;
                    }
                    else
                    {
                        errline.Add("减免数必须是数字");
                    }
                }

                var order_num = cols[12].Trim();

                if (errline.Count > 0)
                {
                    sbr.Append("第" + rowno + "行：" + string.Join("、", errline) + "\r\n");
                }
                dtres.Rows.Add(dicin);
            }
            sr.Close();
            fsm.Close();
            if (rowno <= 1)
            {
                sbr.Append("文件没有数据！\r\n");
            }
            if (sbr.Length > 0)
            {
                msg = sbr.ToString();
                return false;
            }
            System.Data.SqlClient.SqlBulkCopy sbc = new System.Data.SqlClient.SqlBulkCopy(System.Configuration.ConfigurationManager.ConnectionStrings["mssql"].ConnectionString);
            sbc.BulkCopyTimeout = 3600;
            sbc.DestinationTableName = "r_lterru";
            foreach (System.Data.DataColumn col in dtres.Columns) {
                sbc.ColumnMappings.Add(col.ColumnName,col.ColumnName);
            }
            sbc.WriteToServer(dtres);
            sbc.Close();
            File.Delete(filefullname);
            #region 删除
            //var filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\lterrutmp\\" + ct.filename);
            //FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            //IWorkbook workbook = new XSSFWorkbook(fileStream);
            //ISheet sheet = workbook.GetSheetAt(0);
            //StringBuilder sb = new StringBuilder();
            //if (sheet.LastRowNum == 0)
            //{
            //    msg += "空白文件\n";
            //    return false;
            //}
            //int length = sheet.GetRow(0).Cells.Count;
            //for (int i = 1; i <= sheet.LastRowNum; i++)
            //{
            //    IRow row = sheet.GetRow(i);
            //    if(row.Cells.Count > length)
            //    {
            //        msg += "第" + (i + 1) + "行列数不匹配\n";
            //        continue;
            //    }
            //    if (!constcitys.Contains(row.GetCell(1)+""))
            //    {
            //        msg += "第" + (i + 1) + "行地市不正确\n";
            //    }
            //    if (!constchangs.Contains(row.GetCell(6)+""))
            //    {
            //        msg += "第" + (i + 1) + "行厂家不正确\n";
            //    }
            //    if (row.GetCell(3)+""!="室内" && row.GetCell(3)+"" != "室外")
            //    {
            //        msg += "第" + (i + 1) + "行室内/外不正确\n";
            //    }
            //    int a;
            //    if (!int.TryParse(row.GetCell(7)+"", out a) && !int.TryParse(row.GetCell(8)+"", out a))
            //    {
            //        msg += "第" + (i + 1) + "行借货/到货格式不正确\n";
            //    }
            //}
            //if(msg != "")
            //{
            //    return false;
            //}
            //string sql = "";
            //for (int i = 1; i <= sheet.LastRowNum; i++)
            //{
            //    IRow row = sheet.GetRow(i);
            //    sql = "insert into r_lterru (borrowtime,city,stage,outin,rru_type,pinduan,chang,borrow,arrive,need,remark,free,order_num) values ('" + row.GetCell(0).DateCellValue.Date.ToString("yyyy-MM-dd") + "', '" + row.GetCell(1) + "','" + row.GetCell(2) + "','" + row.GetCell(3) + "','" + row.GetCell(4) + "','" + row.GetCell(5) + "', '" + row.GetCell(6) + "', " + row.GetCell(7) + ", " + row.GetCell(8) + ", " + row.GetCell(9) + ", '" + row.GetCell(10) + "', " + (row.GetCell(11)+""==""?"0": row.GetCell(11)+"") + "," + row.GetCell(12) + ")";
            //    DB.Exec(sql);
            //}
            //File.Move(filename, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\lterru\\", ct.filename));
            #endregion
            
            //结束
            #endregion
            return res;
        }

        private static bool computationDuibi(ctasks ct, out string msg) {
            msg = "";
            var res = true;

            #region 计算对比代码
            var filepath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang");
            var filename = Path.Combine(filepath, ct.filename);
            var tfilename = Path.Combine(filepath, ct.tfilename);
            Dictionary<string, Dictionary<string, object>> filedic = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> tfiledic = new Dictionary<string, Dictionary<string, object>>();

            var tongjipath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wangtongji");

            var filenameshort = ct.filename.Substring(0,ct.filename.LastIndexOf("."));

            tongjipath = Path.Combine(tongjipath,filenameshort);

            if (!Directory.Exists(tongjipath)) {
                Directory.CreateDirectory(tongjipath);
            }
            var tmpdic=new Dictionary<string,object>();
            var tmpnewdic=new Dictionary<string,object>();
            var tmpnsdic=new Dictionary<string,Dictionary<string,object>>();
            string tmsg;

            var nsfilepath= System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang");
            var nsfilename = Path.Combine(nsfilepath, ct.filename);

            readwangcsv(tfilename,tmpdic);
            readnsendcsv(nsfilename,tmpdic,null,tmpnsdic,out tmsg);

            readwangcsv(filename, tmpdic, tmpnsdic,tmpnewdic,filedic,out tmsg);
            readwangcsv(tfilename, tmpdic, tmpnsdic, tmpnewdic, tfiledic, out tmsg);

            var dicds = tfiledic.Where(a => !filedic.ContainsKey(a.Key)).GroupBy(a => a.Value["city"] + "_" + a.Value["chang"]).Select(a => new Dictionary<string, object> { 
            { "city", a.Max(b => b.Value["city"]) }, { "chang", a.Max(b => b.Value["chang"]) }, 
            {"rrumu",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString()=="L800M"?1:0)},{"rrugu",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString().EndsWith("G")&&b.Value["pinduan"].ToString()!="L2.6G"?1:0)},{"prruu",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"?1:0)}
            }).ToDictionary(a => a["city"] + "_" + a["chang"]);

            var dictj = filedic.GroupBy(a => a.Value["city"] + "_" + a.Value["chang"]).Select(a => new Dictionary<string, object> { 
            { "city", a.Max(b => b.Value["city"]) }, { "chang", a.Max(b => b.Value["chang"]) }, 
            {"rrum2",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString()=="L800M"&&b.Value["xz"].ToString()=="是"?1:0)},{"rrug2",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString().EndsWith("G")&&b.Value["pinduan"].ToString()!="L2.6G"&&b.Value["xz"].ToString()=="是"?1:0)},{"prru2",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"&&b.Value["xz"].ToString()=="是"?1:0)},
            {"rrum3",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString()=="L800M"&&b.Value["xz"].ToString()=="是"&&b.Value["fh"].ToString()=="是"?1:0)},{"rrug3",a.Sum(b=>b.Value["leixing"].ToString()!="PRRU"&&b.Value["pinduan"].ToString().EndsWith("G")&&b.Value["pinduan"].ToString()!="L2.6G"&&b.Value["xz"].ToString()=="是"&&b.Value["fh"].ToString()=="是"?1:0)},{"prru3",a.Sum(b=>b.Value["leixing"].ToString()=="PRRU"&&b.Value["xz"].ToString()=="是"&&b.Value["fh"].ToString()=="是"?1:0)},
            {"rrumu",0},{"rrugu",0},{"prruu",0},
            }).ToDictionary(a => a["city"] + "_" + a["chang"]);

            foreach (var dd in dicds)
            {
                if (!dictj.ContainsKey(dd.Key))
                {
                    dictj[dd.Key] = new Dictionary<string, object>();
                    dictj[dd.Key]["city"] = dd.Value["city"];
                    dictj[dd.Key]["chang"] = dd.Value["chang"];
                    dictj[dd.Key]["rrum"] = 0;
                    dictj[dd.Key]["rrug"] = 0;
                    dictj[dd.Key]["prru"] = 0;
                    dictj[dd.Key]["rrum2"] = 0;
                    dictj[dd.Key]["rrug2"] = 0;
                    dictj[dd.Key]["prru2"] = 0;
                    dictj[dd.Key]["rrum3"] = 0;
                    dictj[dd.Key]["rrug3"] = 0;
                    dictj[dd.Key]["prru3"] = 0;
                    dictj[dd.Key]["rrum4"] = 0;
                    dictj[dd.Key]["rrug4"] = 0;
                    dictj[dd.Key]["prru4"] = 0;
                    dictj[dd.Key]["rrum5"] = 0;
                    dictj[dd.Key]["rrug5"] = 0;
                    dictj[dd.Key]["prru5"] = 0;
                }


                dictj[dd.Key]["rrumu"] = dd.Value["rrumu"];
                dictj[dd.Key]["rrugu"] = dd.Value["rrugu"];
                dictj[dd.Key]["prruu"] = dd.Value["prruu"];
            }
            var sbres = new StringBuilder();
            sbres.Append("city,chang,rrum2,rrug2,prru2,rrum3,rrug3,prru3,rrumu,rrugu,prruu\r\n");
            foreach (var dd in dictj)
            {
                sbres.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}\r\n"
                    , dd.Value["city"], dd.Value["chang"]
                    , dd.Value["rrum2"], dd.Value["rrug2"], dd.Value["prru2"]
                    , dd.Value["rrum3"], dd.Value["rrug3"], dd.Value["prru3"]
                    , dd.Value["rrumu"], dd.Value["rrugu"], dd.Value["prruu"]));
            }
            File.WriteAllText(Path.Combine(tongjipath,ct.tfilename),sbres.ToString(),Encoding.GetEncoding("gbk"));

            #endregion

            return res;
        }

        private static bool xlsx2csv(string filename,string newFilename,out string msg){
            msg = "";
            var res = true;

            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = new XSSFWorkbook(fileStream);
            ISheet sheet = workbook.GetSheetAt(0);
            StringBuilder sb = new StringBuilder();
            if (sheet.LastRowNum == 0)
            {
                msg += "空白文件\r\n";
                return false;
            }
            int length = sheet.GetRow(0).Cells.Count;
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                for (int j = 0; j < length; j++)
                {
                    string c = row.GetCell(j) + "";
                    c = c.Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ");

                    if (c.Contains(","))
                    {
                        c = "\"" + c + "\"";
                    }
                    if (j < length - 1) sb.Append(c + ",");
                    else sb.Append(c + "\r\n");
                }
            }
            workbook.Close();
            fileStream.Close();



            File.WriteAllText(newFilename, sb.ToString(), Encoding.GetEncoding("gbk"));

            
            File.Delete(filename);

            return res;
        }

        private static bool importTable1(ctasks ct, out string msg) {
            msg = "";
            var res = true;

            #region 导入表1代码
            if (ct.filename.Substring(ct.filename.LastIndexOf(".") + 1).ToLower() == "xlsx")
            {
                var filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\table1tmp\\" + ct.filename);
                string newName = ct.filename.Substring(0, ct.filename.LastIndexOf(".")) + ".csv";
                ct.filename = newName;
                string newPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\table1tmp\\" + newName);

                res = xlsx2csv(filename,newPath,out msg);
                if (!res) {
                    
                    return res;
                }
            }
            var t1path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\table1\\");
            var t1tmppath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\table1tmp\\");
            var t1tmpname = Path.Combine(t1tmppath,ct.filename);

            Dictionary<int, Dictionary<string, object>> dic = new Dictionary<int, Dictionary<string, object>>();
            readtable1csv(t1tmpname,dic,out msg);
            if (msg.Length > 0) {
                File.Delete(t1tmpname);
                return false;
            }

            StringBuilder sbres = new StringBuilder();

            sbres.Append("本地网,本地网编码,基站ID,小区ID,小区名称,小区覆盖类别,频段标识,厂家,经度,纬度,PRB上行利用率（%）,PRB下行利用率（%）,RRC连接用户数（个）,小区流量（GB)-上行,小区流量（GB)-下行,计费用户数\r\n");
            foreach (var kv in dic)
            {
                var cols = new List<string>();
                foreach (var k in kv.Value)
                {
                    cols.Add(k.Value.GetType() == typeof(DateTime) ? ((DateTime)k.Value).ToString("yyyy/M/d") : k.Value.ToString());
                }
                sbres.Append(string.Join(",", cols) + "\r\n");
            }
            if (!Directory.Exists(t1path))
            {
                Directory.CreateDirectory(t1path);
            }
            File.WriteAllText(Path.Combine(t1path, ct.tfilename), sbres.ToString(), Encoding.GetEncoding("gbk"));

            sbres.Clear();


            File.Delete(t1tmpname);
            #endregion

            return res;
        }
        private static void readtable1csv(string filename,Dictionary<int,Dictionary<string,object>> dic,out string msg) {
            msg = "";
            
            var rowno = 1;
            System.Text.StringBuilder sbr = new System.Text.StringBuilder();
            System.IO.FileStream fsm = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.StreamReader sr = new System.IO.StreamReader(fsm, System.Text.Encoding.GetEncoding("gbk"));
            sr.ReadLine();

            while (!sr.EndOfStream)
            {
                rowno++;
                var line = sr.ReadLine();
                Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var cols = reg.Split(line);
                if (cols.Length < 16)
                {
                    sbr.Append("第" + rowno + "行，列数不足" + "\r\n");
                    continue;
                }
                var errline = new List<string>();
                var chong = false;
                var dicin = new Dictionary<string, object>();

                var city = cols[0].Trim();
                dicin["city"] = city;
                if (!constcitys.Contains(city))
                {
                    errline.Add("地市不正确");
                }

                var cityno = cols[1].Trim();
                dicin["cityno"] = cityno;
                if (!constcitynos.Contains(cityno))
                {
                    errline.Add("地市编码不正确");
                }

                if (constcitys.IndexOf(city) != constcitynos.IndexOf(cityno)) {
                    errline.Add("地市和编码不匹配");
                }


                var bids = cols[2].Trim();
                int bid;
                if (bids == "")
                {
                    dicin["bid"] = bids;
                    errline.Add("基站ID不能为空");
                }
                else if (!int.TryParse(bids, out bid))
                {
                    errline.Add("基站ID必须是数字");
                }
                else
                {
                    dicin["bid"] = bid;
                }

                var cids = cols[3].Trim();
                int cid;
                if (cids == "")
                {
                    dicin["cid"] = cids;
                    errline.Add("小区ID不能为空");
                }
                else if (!int.TryParse(cids, out cid))
                {
                    errline.Add("小区ID必须是数字");
                }
                else
                {
                    dicin["cid"] = cid;
                }

                var ebcid = O2.O2I(bids) * 256 + O2.O2I(cids);
                if (dic.ContainsKey(ebcid))
                {
                    errline.Add("基站ID小区ID本表重复");
                    chong = true;
                }

                var cname = cols[4].Trim();
                dicin["cname"] = cname;
                if (string.IsNullOrEmpty(cname))
                {
                    errline.Add("小区名称不能为空");
                }

                var fugai = cols[5].Trim().ToUpper();
                dicin["fugai"] = fugai;
                if (!constcfugai.Contains(fugai))
                {
                    errline.Add("小区覆盖类型不正确");
                }

                var pinduan = cols[6].Trim().ToUpper();
                dicin["pinduan"] = pinduan;
                if (!constpinduannc.Contains(pinduan))
                {
                    errline.Add("频段不正确");
                }

                var chang = cols[7].Trim();
                dicin["chang"] = chang;
                if (!constchangs.Contains(chang))
                {
                    errline.Add("厂家不正确");
                }

                var lons = cols[8].Trim();
                float lon;
                dicin["lon"] = "";
                if (lons.Length > 0)
                {
                    if (!float.TryParse(lons, out lon)) {
                        errline.Add("经度必须是数字");
                    }
                    dicin["lon"] = lon;
                }

                var lats = cols[9].Trim();
                float lat;
                dicin["lat"] = "";
                if (lats.Length > 0)
                {
                    if (!float.TryParse(lats, out lat))
                    {
                        errline.Add("纬度必须是数字");
                    }
                    dicin["lat"] = lat;
                }

                var prbups = cols[10].Trim();
                float prbup;
                dicin["prbup"] = "";
                if (prbups.Length > 0)
                {
                    if (!float.TryParse(prbups, out prbup))
                    {
                        errline.Add("PRB上行利用率（%）必须是数字");
                    }
                    dicin["prbup"] = prbup;
                }

                var prbdowns = cols[11].Trim();
                float prbdown;
                dicin["prbdown"] = "";
                if (prbdowns.Length > 0)
                {
                    if (!float.TryParse(prbdowns, out prbdown))
                    {
                        errline.Add("PRB下行利用率（%）必须是数字");
                    }
                    dicin["prbdown"] = prbdown;
                }

                var rrcs = cols[12].Trim();
                int rrc;
                dicin["rrc"] = "";
                if (rrcs.Length > 0)
                {
                    if (!int.TryParse(rrcs, out rrc))
                    {
                        errline.Add("RRC连接用户数（个）必须是整数");
                    }
                    dicin["rrc"] = rrc;
                }

                var flowups = cols[13].Trim();
                float flowup;
                dicin["flowup"] = "";
                if (flowups.Length > 0)
                {
                    if (!float.TryParse(flowups, out flowup))
                    {
                        errline.Add("小区流量（GB)-上行必须是数字");
                    }
                    dicin["flowup"] = flowup;
                }

                var flowdowns = cols[14].Trim();
                float flowdown;
                dicin["flowdown"] = "";
                if (flowdowns.Length > 0)
                {
                    if (!float.TryParse(flowdowns, out flowdown))
                    {
                        errline.Add("小区流量（GB)-上行必须是数字");
                    }
                    dicin["flowdown"] = flowdown;
                }

                var ucounts = cols[15].Trim();
                int ucount;
                dicin["ucount"] = "";
                if (ucounts.Length > 0)
                {
                    if (!int.TryParse(ucounts, out ucount))
                    {
                        errline.Add("计费用户数必须是整数");
                    }
                    dicin["ucount"] = ucount;
                }
                
                if (errline.Count > 0)
                {
                    sbr.Append("第" + rowno + "行：" + string.Join("、", errline) + "\r\n");
                }
                if (chong) continue;
                dic[ebcid] = dicin;
            }
            sr.Close();
            fsm.Close();
            if (rowno <= 1)
            {
                sbr.Append("文件没有数据！\r\n");
            }
            if (sbr.Length > 0)
            {
                msg = sbr.ToString();
            }
        }
    }
}