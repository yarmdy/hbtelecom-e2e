﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Data;

namespace CTCCGoods.Controllers
{
    public class NetcapController : Controller
    {
        //
        // GET: /Netcap/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Krmen() {
            return View();
        }
        public ActionResult Fkrm() {
            var js=DB.QueryAsDics("select * from ckrmen order by id");
            return Json(new { total = js==null?0:js.Length, data = js }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Ukrm(int id,string field,string v) {
            field = field.Replace("'","").Replace("--","");
            var res = DB.Exec("update ckrmen set "+field+"='"+v+"' where id="+id);
            if (res > 0) {
                return Json(new { ok = true, msg = "修改成功" });
            }
            return Json(new { ok = false, msg = "修改失败" });
        }
        public ActionResult Table1() {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netcapfiles/table1"));
            cuser user = (cuser)Session["loginuser"];
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                //Array.Sort(files, (f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime));
                Array.Sort(files, (f1, f2) => f1.Name.CompareTo(f2.Name));
                Array.Reverse(files);
                foreach (FileInfo fi in files)
                {
                    var a = new Dictionary<string, string>();
                    if (user.utype != 0 && fi.Name.Contains("[hidden]")) continue;
                    a.Add("code", fi.FullName);
                    a.Add("text", fi.Name);
                    list.Add(a);
                }
            }
            ViewBag.history = list;
            ViewBag.user = user;
            return View();
        }
        public ActionResult Dtable1(string name)
        {
            var nspath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"netcapfiles\table1");
            
            System.IO.File.Delete(Path.Combine(nspath, name));
            return Json(new { ok = true, msg = "已删除" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Table1import(string[] flimport, string txttime)
        {
            if (Request.Files.Count <= 0)
            {
                return Content("<script>alert(\"没有检测到上传文件，上传失败\");location=\"/netcap/table1\"</script>");
            }
            var table1file = Request.Files[0];
            if (table1file.FileName.IndexOf(".") < 0 || !new string[] { "xlsx", "csv" }.Contains(table1file.FileName.Substring(table1file.FileName.LastIndexOf(".") + 1).ToLower()))
            {
                return Content("<script>alert(\"上传失败，请上传xlsx、csv格式文件\");location=\"/netcap/table1\"</script>");
            }
            var uppath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\table1tmp");
            if (!Directory.Exists(uppath))
            {
                Directory.CreateDirectory(uppath);
            }
            var now = DateTime.Now;
            var filename = now.ToString("yyyyMMdd_") + table1file.FileName;
            table1file.SaveAs(Path.Combine(uppath, filename));
            var targetname = now.ToString("yyyyMMdd") + ".csv";
            DateTime tartime;
            if (!string.IsNullOrEmpty(txttime) && DateTime.TryParse(txttime, out tartime))
            {
                targetname = tartime.ToString("yyyyMMdd") +  ".csv";
            }
            ctasksHandle.addtask(11, filename, targetname, "");
            return Content("<script>alert(\"已上传到服务器，进入后台任务\");location=\"/task\"</script>");
        }

        public ActionResult Sbusycomp() {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netcapfiles/table1"));
            cuser user = (cuser)Session["loginuser"];

            var thisfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\sbusycomp\\sbusycomp.csv");
            var thisxml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\sbusycomp\\sbusycomp.xml");

            ViewBag.start = "";
            ViewBag.end = "";
            ViewBag.cm = false;
            ViewBag.cmts = 4;

            var selectedlist = new Dictionary<string, object>();
            
            if (System.IO.File.Exists(thisxml)) {
                var xml=XDocument.Load(thisxml);
                var starts= xml.Descendants("start").FirstOrDefault();
                var ends = xml.Descendants("end").FirstOrDefault();
                var cms = xml.Descendants("cm").FirstOrDefault();
                var cmtss = xml.Descendants("cmts").FirstOrDefault();

                ViewBag.start = starts != null ? starts.Value : "";
                ViewBag.end = ends != null ? ends.Value : "";
                ViewBag.cm = O2.O2B(cms);
                ViewBag.cmts = O2.O2I(cmtss);

                selectedlist = xml.Descendants("item").ToDictionary(a=>a.Value,a=>(object)null);
            }

            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                //Array.Sort(files, (f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime));
                Array.Sort(files, (f1, f2) => {
                    var res=0;
                    res = f1.Name.Substring(0, 6).CompareTo(f2.Name.Substring(0, 6));
                    if (res == 0) {
                        res = f2.Name.Substring(6, 2).CompareTo(f1.Name.Substring(6, 2));
                    }
                    return res;
                });
                Array.Reverse(files);
                foreach (FileInfo fi in files)
                {
                    var a = new Dictionary<string, string>();
                    if (user.utype != 0 && fi.Name.Contains("[hidden]")) continue;
                    a.Add("code", fi.FullName);
                    a.Add("text", fi.Name);
                    var selected = selectedlist.ContainsKey(fi.Name)?"1":"0";
                    a.Add("sel",selected);
                    list.Add(a);
                }
            }
            ViewBag.history = list;
            ViewBag.user = user;
            ViewBag.thisfile = thisfile;
            return View();
        }
        public ActionResult Csbusy(string[] list,int count,string start,string end,bool cm,int? cmts) {

            if (list == null || list.Length <= 0) {
                return Json(new { ok = false, msg = "未选择任何日期" }, JsonRequestBehavior.AllowGet);
            }
            if (cm && (!cmts.HasValue||cmts.Value<=0))
            {
                return Json(new { ok = false, msg = "计算超忙未设置天数" }, JsonRequestBehavior.AllowGet);
            }

            var xml =XDocument.Parse("<root></root>");
            var xcount = new XElement("count",count);
            var xstart = new XElement("start", start);
            var xend = new XElement("end", end);
            var xcm = new XElement("cm", cm);
            var xcmts = new XElement("cmts", cmts);

            var xlist = new XElement("list");
            foreach (var it in list) {
                var xitem = new XElement("item", it);
                xlist.Add(xitem);
            }
            xml.Root.Add(xcount);
            xml.Root.Add(xstart);
            xml.Root.Add(xend);
            xml.Root.Add(xcm);
            xml.Root.Add(xcmts);
            xml.Root.Add(xlist);
            var dt=DateTime.Now;
            var filepath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles/sbusycomptmp");
            var filename = dt.ToString("yyyyMMddHHmmssmsfffffff")+".xml";

            if (!Directory.Exists(filepath)) {
                Directory.CreateDirectory(filepath);
            }
            var dec = new XDeclaration("1.0","gbk","yes");
            xml.Declaration = dec;
            xml.Save(Path.Combine(filepath,filename));
            ctasksHandle.addtask(12, filename,"sbusycomp","");

            return Json(new { ok=true,msg="已加入任务列表，请在后台任务查看任务状态"},JsonRequestBehavior.AllowGet);
        }

        public ActionResult Superbusylist() {
            return View();
        }
        public ActionResult Table2import(string[] flimport, string txttime)
        {
            if (Request.Files.Count <= 0)
            {
                return Content("<script>alert(\"没有检测到上传文件，上传失败\");location=\"/netcap/superbusylist\"</script>");
            }
            var table1file = Request.Files[0];
            if (table1file.FileName.IndexOf(".") < 0 || !new string[] { "xlsx", "csv" }.Contains(table1file.FileName.Substring(table1file.FileName.LastIndexOf(".") + 1).ToLower()))
            {
                return Content("<script>alert(\"上传失败，请上传xlsx、csv格式文件\");location=\"/netcap/superbusylist\"</script>");
            }
            var uppath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\table2tmp");
            if (!Directory.Exists(uppath))
            {
                Directory.CreateDirectory(uppath);
            }
            var now = DateTime.Now;
            var filename = now.ToString("yyyyMMdd_") + table1file.FileName;
            table1file.SaveAs(Path.Combine(uppath, filename));
            var targetname = now.ToString("yyyyMMdd") + ".csv";
            //DateTime tartime;
            //if (!string.IsNullOrEmpty(txttime) && DateTime.TryParse(txttime, out tartime))
            //{
            //    targetname = tartime.ToString("yyyyMMdd") + ".csv";
            //}
            ctasksHandle.addtask(13, filename, targetname, "");
            return Content("<script>alert(\"已上传到服务器，进入后台任务\");location=\"/task\"</script>");
        }
        public ActionResult Ft2(int offset,int limit) {
            cuser user = (cuser)Session["loginuser"];

            var sql = "select period 账期,'河北' 省份,'813' 省份编码,city 地市,cityno 地市ID,enodebid 基站ID,cellid 小区ID,cellname 小区名称,chang 厂家,pinduan 频点,flowdown 计费下行流量GB,rrc 'RRC连接 用户数',ucount 计费用户数,prbdown PRB下行利用率,richurate '高价值用户占比%',richorder 价值优先排序,reportgroup 是否上报集团扩容,cellsetting 现小区配置,advise 省公司建议,advisepinduan 扩容建议频点,buildmode 建设方式,count0 from (SELECT *,ROW_NUMBER()over(order by period,richorder,enodebid,cellid)top1,count(0)over()count0 FROM csuperbusy where 1=1 {0})a where top1>" + offset + " and top1<=" + (offset + limit);
            var where = "";
            if (user.utype != 0) {
                where = " and city='"+user.wname+"'";
            }
            sql=string.Format(sql,where);
            var dt = DB.Query(sql);
            var total = 0;
            Dictionary<string, object>[] rows = new Dictionary<string, object>[0];
            if (dt != null &&dt.Rows.Count>0) {
                total = (int)dt.Rows[0]["count0"];
                dt.Columns.Remove("count0");
                rows = dt.AsEnumerable().Select(a =>
                {
                    var dic = new Dictionary<string, object>();
                    foreach (DataColumn v in dt.Columns)
                    {
                        dic[v.ColumnName] = a[v.ColumnName];
                    }
                    return dic;
                }).ToArray();
            }
            return Json(new { rows = rows, total = total }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Exportt2()
        {
            cuser user = (cuser)Session["loginuser"];

            var sql = "select period 账期,'河北' 省份,'813' 省份编码,city 地市,cityno 地市ID,enodebid 基站ID,cellid 小区ID,cellname 小区名称,chang 厂家,pinduan 频点,flowdown 计费下行流量GB,rrc 'RRC连接 用户数',ucount 计费用户数,prbdown PRB下行利用率,richurate '高价值用户占比%',richorder 价值优先排序,reportgroup 是否上报集团扩容,cellsetting 现小区配置,advise 省公司建议,advisepinduan 扩容建议频点,buildmode 建设方式 from csuperbusy where 1=1 {0}";
            var where = "";
            if (user.utype != 0)
            {
                where = " and city='" + user.wname + "'";
            }
            sql = string.Format(sql, where);
            var dt = DB.Query(sql);
            StringBuilder sb = new StringBuilder();
            sb.Append("账期,省份,省份编码,地市,地市ID,基站ID,小区ID,小区名称,厂家,频点,计费下行流量GB,RRC连接 用户数,计费用户数,PRB下行利用率,高价值用户占比%,价值优先排序,是否上报集团扩容,现小区配置,省公司建议,扩容建议频点,建设方式\r\n");
            if (dt != null) {
                foreach (DataRow dr in dt.Rows) {
                    sb.Append(string.Join(",",dr.ItemArray)+"\r\n");
                }
            }

            return File(Encoding.GetEncoding("gbk").GetBytes(sb.ToString()), "application/octet-stream","超忙原始清单.csv");
        }
        public ActionResult Superbusyex() {

            return View();
        }
        public ActionResult Table3import(string[] flimport, string txttime)
        {
            if (Request.Files.Count <= 0)
            {
                return Content("<script>alert(\"没有检测到上传文件，上传失败\");location=\"/netcap/superbusyex\"</script>");
            }
            var table1file = Request.Files[0];
            if (table1file.FileName.IndexOf(".") < 0 || !new string[] { "xlsx", "csv" }.Contains(table1file.FileName.Substring(table1file.FileName.LastIndexOf(".") + 1).ToLower()))
            {
                return Content("<script>alert(\"上传失败，请上传xlsx、csv格式文件\");location=\"/netcap/superbusyex\"</script>");
            }
            var uppath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles\\table3tmp");
            if (!Directory.Exists(uppath))
            {
                Directory.CreateDirectory(uppath);
            }
            var now = DateTime.Now;
            var filename = now.ToString("yyyyMMdd_") + table1file.FileName;
            table1file.SaveAs(Path.Combine(uppath, filename));
            var targetname = now.ToString("yyyyMMdd") + ".csv";
            //DateTime tartime;
            //if (!string.IsNullOrEmpty(txttime) && DateTime.TryParse(txttime, out tartime))
            //{
            //    targetname = tartime.ToString("yyyyMMdd") + ".csv";
            //}
            ctasksHandle.addtask(14, filename, targetname, "");
            return Content("<script>alert(\"已上传到服务器，进入后台任务\");location=\"/task\"</script>");
        }
        public ActionResult Ft3(int offset, int limit)
        {
            cuser user = (cuser)Session["loginuser"];

            var sql = "select a.* from (select b.period b_period,'河北' b_province,'813' b_provinceno,b.city b_city,b.cityno b_cityno,b.enodebid b_enodebid,b.cellid b_cellid,b.cellname b_cellname,b.chang b_chang,b.pinduan b_pinduan,b.flowdown b_flowdown,b.rrc b_rrc,b.ucount b_ucount,b.prbdown b_prbdown,b.richurate b_richurate,b.richorder b_richorder,b.reportgroup b_reportgroup,b.cellsetting b_cellsetting,b.advise b_advise,b.advisepinduan b_advisepinduan,b.buildmode b_buildmode,a.*,ROW_NUMBER()over(order by b.period,b.richorder,b.enodebid,b.cellid)top1,count(0)over()count0 from csuperbusyex a left join csuperbusy b on a.yenodebid=b.enodebid and a.ycellid=b.cellid where 1=1 {0})a where top1>" + offset + " and top1<=" + (offset + limit);
            var where = "";
            if (user.utype != 0)
            {
                where = " and b.city='" + user.wname + "'";
            }
            sql = string.Format(sql, where);
            var dt = DB.Query(sql);
            var total = 0;
            Dictionary<string, object>[] rows = new Dictionary<string, object>[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                total = (int)dt.Rows[0]["count0"];
                dt.Columns.Remove("count0");
                rows = dt.AsEnumerable().Select(a =>
                {
                    var dic = new Dictionary<string, object>();
                    foreach (DataColumn v in dt.Columns)
                    {
                        if (a[v.ColumnName].GetType() == typeof(DateTime)) {
                            dic[v.ColumnName] = ((DateTime)a[v.ColumnName]).ToString("yyyy/M/d");
                        }
                        else if (a[v.ColumnName].GetType() == typeof(bool))
                        {
                            dic[v.ColumnName] = ((bool)a[v.ColumnName]) ? "是" : "否";
                        }
                        else
                        {
                            dic[v.ColumnName] = a[v.ColumnName];
                        }
                    }
                    return dic;
                }).ToArray();
            }
            return Json(new { rows = rows, total = total }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Exportt3()
        {
            cuser user = (cuser)Session["loginuser"];

            var sql = "select b.period b_period,'河北' b_province,'813' b_provinceno,b.city b_city,b.cityno b_cityno,b.enodebid b_enodebid,b.cellid b_cellid,b.cellname b_cellname,b.chang b_chang,b.pinduan b_pinduan,b.flowdown b_flowdown,b.rrc b_rrc,b.ucount b_ucount,b.prbdown b_prbdown,b.richurate b_richurate,b.richorder b_richorder,b.reportgroup b_reportgroup,b.cellsetting b_cellsetting,b.advise b_advise,b.advisepinduan b_advisepinduan,b.buildmode b_buildmode,a.* from csuperbusyex a left join csuperbusy b on a.yenodebid=b.enodebid and a.ycellid=b.cellid where 1=1 {0}";
            var where = "";
            if (user.utype != 0)
            {
                where = " and b.city='" + user.wname + "'";
            }
            sql = string.Format(sql, where);
            var dt = DB.Query(sql);
            StringBuilder sb = new StringBuilder();
            sb.Append("账期,省份,省份编码,地市,地市编码,基站ID,小区ID,小区名称,厂家,频点,计费下行流量GB,RRC连接 用户数,计费用户数,PRB下行利用率,高价值用户占比%,价值优先排序,是否上报集团扩容,现小区配置,省公司建议,扩容建议频点,建设方式,厂家,小区类型,频点,基站ID,小区ID,小区名称,是否真实扩容,入网时间,是否存在,PRB上行利用率（%）,PRB下行利用率（%）,RRC连接用户数（个）,小区流量（GB)-上行,小区流量（GB)-下行,计费用户数,是否存在,PRB上行利用率（%）,PRB下行利用率（%）,RRC连接用户数（个）,小区流量（GB)-上行,小区流量（GB)-下行,计费用户数,新扩容小区是否过闲,新小区流量/（原小区+新小区流量）,流量负载是否合理,是否合理\r\n");
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var drl = dr.ItemArray.ToList();
                    drl.RemoveAt(21);
                    drl.RemoveAt(21);
                    for (int i = 0; i < drl.Count;i++ )
                    {
                        if (drl[i].GetType() == typeof(DateTime))
                        {
                            drl[i] = ((DateTime)drl[i]).ToString("yyyy/M/d");
                        }
                        else if (drl[i].GetType() == typeof(bool)) {
                            drl[i] = ((bool)drl[i]) ? "是" : "否";
                        }
                    }
                    sb.Append(string.Join(",", drl) + "\r\n");
                }
            }

            return File(Encoding.GetEncoding("gbk").GetBytes(sb.ToString()), "application/octet-stream", "超忙对应扩容清单.csv");
        }
        #region 通用
        public ActionResult File(string b, string r, string n, string city, string chang, string pinduan, string leixing, string xz, string fh)
        {

            string type = Request["type"];
            string d = Request["d"];
            cuser user = (cuser)Session["loginuser"];
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "netcapfiles/" + d));
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                FileInfo file = null;
                int offset = Request["offset"] == null ? 0 : int.Parse(Request["offset"]);
                int limit = Request["limit"] == null ? 10 : int.Parse(Request["limit"]);
                bool found = false;
                foreach (var fi in files)
                {
                    if (fi.FullName == type)
                    {
                        file = fi;
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    return Json(new { total = 0 }, JsonRequestBehavior.AllowGet);
                }
                string content = System.IO.File.ReadAllText(file.FullName, Encoding.Default);
                Dictionary<string, object> result = null;
                if (d == "sbusycomp") {
                    result = RruController.CsvToJson(content, user, limit, offset, 0, new Dictionary<int, string>() { { 0, city }, { 7, chang },{4,n},{2,b==""?"":"{==}"+b},{3,r==""?"":"{==}"+r}},2);
                } else {
                    result=RruController.CsvToJson(content, user, limit, offset, 0, null,2);
                }
                int total = (int)result["total"];
                var rows = result["data"];


                return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Export()
        {
            string file = Request["file"];
            int index = Request["index"] != null ? int.Parse(Request["index"]) : -1;
            cuser user = (cuser)Session["loginuser"];
            if (user.utype == 0)
            {
                return File(file, "application/octet-stream", file.Split('\\')[file.Split('\\').Length - 1]);
            }
            string content = System.IO.File.ReadAllText(file, Encoding.Default);
            string r = RruController.ExportConvert(content, user, index);
            return File(Encoding.Default.GetBytes(r), "application/octet-stream", file.Split('\\')[file.Split('\\').Length - 1]);
        }
        #endregion
    }
}
