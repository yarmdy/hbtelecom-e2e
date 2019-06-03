using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
                    result = RruController.CsvToJson(content, user, limit, offset, 0, new Dictionary<int, string>() { { 0, city }, { 7, chang },{4,n},{2,b==""?"":"{==}"+b},{3,r==""?"":"{==}"+r}});
                } else {
                    result=RruController.CsvToJson(content, user, limit, offset, 0, new Dictionary<int, string>() { { 0, city }, { 1, chang }, { 2, b }, { 5, pinduan }, { 7, leixing }, { 8, r }, { 9, n }, { 11, xz }, { 13, fh } });
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
