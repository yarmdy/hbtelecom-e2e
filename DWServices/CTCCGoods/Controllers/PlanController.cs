using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace CTCCGoods.Controllers
{
    public class PlanController : Controller
    {
        //
        // GET: /Plan/
        [Breadcrumb(Auth = "1")]
        public ActionResult Index()
        {
            //var et = DB.QueryAsDics("select id, name from e_templet");
            //if (et == null)
            //{
            //    et = new Dictionary<string, object>[0];
            //}
            //ViewBag.et = et;
            return Redirect("/plan/list");

            //var et = DB.QueryAsDics("select id, name from e_templet");
            //if (et == null)
            //{
            //    et = new Dictionary<string, object>[0];
            //}
            //ViewBag.et = et;
            //return View();
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult List(string id)
        {
            var path = Uri.UnescapeDataString(id+"");
            var et = DB.QueryAsDics("select id, name from e_templet");
            if (et == null)
            {
                et = new Dictionary<string, object>[0];
            }
            ViewBag.et = et;
            ViewBag.path = id+"";
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult enums()
        {
            return View();
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult files()
        {
            cuser user = (cuser)Session["loginuser"];
            ViewBag.utype = user.utype;
            return View();
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Option()
        {
            var et = DB.QueryAsDics("select id, name from e_templet");
            if (et == null)
            {
                et = new Dictionary<string, object>[0];
            }
            var planclass = DB.QueryAsDics("select distinct planclass from e_plan");
            //planclass = planclass.Select(a=>new Dictionary<string,object>{{"planclass" ,a["planclass"].ToString().Substring(0,a["planclass"].ToString().LastIndexOf("/"))}}).ToArray();
            ViewBag.et = et;
            ViewBag.pc = planclass;
            return View();
        }
        //[Breadcrumb(Auth = "1")]
        //public ActionResult Fp()
        //{
        //    //var js = DB.QueryAsDics("select a.id,a.name,a.des,a.etid,b.name etname,a.plannum,a.uploadnum,a.status,a.createtime,a.plantime,a.completetime,a.createuid,c.name createuname from e_plan a left join e_templet b on a.etid=b.id left join cuser c on a.createuid=c.id");
        //    var js = DB.QueryAsDics("SELECT a.id, a.name, a.des,a.planclass, a.etid, b.name etname, a.plannum, a.uploadnum, a.status, CONVERT ( VARCHAR (100), a.createtime, 20 ) createtime, CONVERT (VARCHAR(100), a.plantime, 23) plantime, CONVERT (VARCHAR(100), a.completetime, 23) completetime, a.createuid, c.name createuname, b.filename FROM e_plan a LEFT JOIN e_templet b ON a.etid = b.id LEFT JOIN cuser c ON a.createuid = c.id ORDER BY a.createtime desc");
        //    return Json(new { total = js==null?0:js.Length, data = js }, JsonRequestBehavior.AllowGet);
        //}
        [Breadcrumb(Auth = "1")]
        public ActionResult Fp(string id)
        {
            var path = Uri.UnescapeDataString(id + "");
            //var js = DB.QueryAsDics("select a.id,a.name,a.des,a.etid,b.name etname,a.plannum,a.uploadnum,a.status,a.createtime,a.plantime,a.completetime,a.createuid,c.name createuname from e_plan a left join e_templet b on a.etid=b.id left join cuser c on a.createuid=c.id");
            var js = DB.QueryAsDics("SELECT a.id, a.name, a.des,a.planclass, a.etid, b.name etname, a.plannum, a.uploadnum, a.status, CONVERT ( VARCHAR (100), a.createtime, 20 ) createtime, CONVERT (VARCHAR(100), a.plantime, 23) plantime, CONVERT (VARCHAR(100), a.completetime, 23) completetime, a.createuid, c.name createuname, b.filename FROM e_plan a LEFT JOIN e_templet b ON a.etid = b.id LEFT JOIN cuser c ON a.createuid = c.id ORDER BY a.createtime desc");
            List<Dictionary<string, object>> dirs = null;
            if (string.IsNullOrEmpty(id)) {
                dirs = js.Where(a => !string.IsNullOrEmpty(a["planclass"] + "")).Select(a => a["planclass"].ToString().Substring(0, a["planclass"].ToString().IndexOf("/") < 0 ? a["planclass"].ToString().Length : a["planclass"].ToString().IndexOf("/"))).Distinct().Select(a => new Dictionary<string, object> {{"classname",a},{"classpath",Uri.EscapeDataString(a)} }).OrderBy(a=>a["classname"]+"").ToList();
                dirs.AddRange((js.Where(a => string.IsNullOrEmpty(a["planclass"] + "")).ToList()));
                
            }else{
                dirs = js.Where(a => (a["planclass"] + "").IndexOf(path + "/") == 0).Select(a => {
                    var tmpname = a["planclass"].ToString().Substring(path.Length + 1);

                    return tmpname.Substring(0, tmpname.IndexOf("/") < 0 ? tmpname.Length : tmpname.IndexOf("/"));
                }).Distinct().Select(a => new Dictionary<string, object> { { "classname", a }, { "classpath", Uri.EscapeDataString(path + "/" + a) } }).OrderBy(a => a["classname"] + "").ToList();
                dirs.AddRange((js.Where(a => (a["planclass"] + "")==path).ToList()));
                
            }
            foreach (var dic in dirs) {
                if (dic.ContainsKey("name")) continue;
                var classpath = Uri.UnescapeDataString(dic["classpath"] + "");
                dic["num"] = js.Count(a => (a["planclass"] + "") == classpath + "" || (a["planclass"] + "").IndexOf(classpath + "/") == 0);
                dic["close"] = js.Count(a => ((a["planclass"] + "") == classpath + "" || (a["planclass"] + "").IndexOf(classpath + "/") == 0)&&(int)a["status"]!=0);
                dic["open"] = js.Count(a => ((a["planclass"] + "") == classpath + "" || (a["planclass"] + "").IndexOf(classpath + "/") == 0) && (int)a["status"] == 0);
            }
            js = dirs.ToArray();
            return Json(new { total = js == null ? 0 : js.Length, data = js }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult QueryEnums(int limit, int offset)
        {
            string sql = "select id,name,val,eclass from e_enum";
            var data = DB.QueryAsDics(sql);
            if (data == null)
            {
                return Json(new { total = 0, rows = 0 }, JsonRequestBehavior.AllowGet);
            }
            var total = data.Length;
            var rows = data.ToList();
            rows = rows.Skip(offset).Take(limit).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDate() {
            string dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return Json(new { ok = true, data = dateStr, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult InsertEnum(string name, string val,string eclass)
        {
            string sql = "select * from e_enum where name = '" + name + "'";
            if (DB.QueryOne(sql) != null)
            {
                return Json(new { ok = false, msg = "名称已存在" }, JsonRequestBehavior.AllowGet);
            }
            sql = "insert into e_enum (name, val,eclass) values ('" + name + "','" + val + "','"+eclass+"');";
            int id = DB.Insert(sql);
            return Json(new { ok = true, id = id, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult UpdateEnum(int id, string name, string val,string eclass)
        {
            cuser user = (cuser)Session["loginuser"];
            if(user.utype != 0)
            {
                return Json(new { ok = false, msg = "您没有权限！" }, JsonRequestBehavior.AllowGet);
            }
            string sql = "select * from e_enum where name = '" + name + "' and id <> '" + id + "'";
            if (DB.QueryOne(sql) != null)
            {
                return Json(new { ok = false, msg = "名称已存在" }, JsonRequestBehavior.AllowGet);
            }
            sql = "update e_enum set  name ='" + name + "',val = '" + val + "',eclass='"+eclass+"' where id=" + id;
            DB.Exec(sql);
            return Json(new { ok = true, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult DeleteEnum(int id)
        {
            cuser user = (cuser)Session["loginuser"];
            if (user.utype != 0)
            {
                return Json(new { ok = false, msg = "您没有权限！" }, JsonRequestBehavior.AllowGet);
            }
            var dt = DB.QueryOne("select * from e_enum where id = " + id);
            if (dt == null)
            {
                return Json(new { ok = false, msg = "名称不存在！" });
            }
            DB.Exec("delete from e_enum where id = " + id);
            return Json(new { ok = true, msg = "删除成功！" });
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult GetPlanInfoById(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            //cuser user = (cuser)Session["loginuser"];
            string sql = "SELECT a.id, a.name, a.des, a.etid, b.name etname, a.plannum, a.uploadnum, a.status, CONVERT ( VARCHAR (100), a.completetime, 23 ) completetime, CONVERT ( VARCHAR (100), a.createtime, 20 ) createtime, CONVERT (VARCHAR(100), a.plantime, 23) plantime, a.createuid, c.name createuname, a.planclass FROM e_plan a LEFT JOIN e_templet b ON a.etid = b.id LEFT JOIN cuser c ON a.createuid = c.id WHERE a.id = " + id;
            var info = DB.QueryOne(sql);
            var allet = DB.QueryAsDics("select id, name from e_templet");
            //var name = DB.QueryOne("select et.name from e_templet et join e_plan ep on ep.etid = et.id where ep.id = "+id);
            //var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\combine\\"+id+"\\"+name["name"]+".xlsx");
            //var combined = false;
            //if(System.IO.File.Exists(path))
            //{
            //    combined = true;
            //}
            if (allet == null)
            {
                allet = new Dictionary<string, object>[0];
            }
            //var planfiles = DB.QueryAsDics("select e.*,c.name from e_plan_upload e join cwarehouse c on e.whid = c.id where epid = " + id);
            result.Add("info", info);
            result.Add("allet", allet);
            //result.Add("planfiles", planfiles);
            //result.Add("user", user);
            //result.Add("combined", combined);
            //result.Add("etname", name);
            return Json(new { ok = true,data = result, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult GetDetail(int id)
        {
            cuser user = (cuser)Session["loginuser"];
            Dictionary<string, object> result = new Dictionary<string, object>();
            bool isAdmin = true;
            List<Dictionary<string, object>> detail2 = new List<Dictionary<string, object>>();
            if (user.utype == 0)
            {
                var etidAuth = DB.QueryOne("select etid,name from e_plan where id = " + id);
                //if(O2.O2I(etidAuth["etid"]) == -1)
                //{
                //    string taskName = etidAuth["name"].ToString();
                //    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", taskName);
                //    DirectoryInfo fdir = new DirectoryInfo(p);
                //    foreach (FileInfo f in fdir.GetFiles()) {//显示当前目录所有文件   
                //        int cityId = O2.O2I(Path.GetFileNameWithoutExtension(f.FullName).Split('$')[1]);
                //        string cityName = DB.QueryOne("select name from cuser where id = " + cityId)["name"].ToString();
                //        string time = f.LastWriteTime.ToString();
                //        int status = 9;
                //        string fileName = "/uploadfiles/"+taskName+"/"+f.Name;
                //        Dictionary<string, object> temp = new Dictionary<string, object>();
                //        temp.Add("epid", id);
                //        temp.Add("status", status);
                //        temp.Add("name", cityName);
                //        temp.Add("uptime", time);
                //        temp.Add("filename", fileName);
                //        detail2.Add(temp);
                //    }
                //    result.Add("combined", false);
                //    result.Add("detail", detail2);
                //    result.Add("isAdmin", isAdmin);
                //    result.Add("etname", "");
                //    result.Add("etid", -1);
                //    return Json(new { ok = true, data = result, msg = "" }, JsonRequestBehavior.AllowGet);
                //}
                string sql = "SELECT epu.*,CONVERT ( VARCHAR (100), epu.uploadtime, 20 ) uptime, cw.name FROM e_plan_upload epu JOIN cwarehouse cw ON epu.whid = cw.id WHERE epid = " + id;
                bool combined = false;
                var name = DB.QueryOne("select et.name from e_templet et join e_plan ep on ep.etid = et.id where ep.id = " + id);
                if(name != null)
                {
                    var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\combine\\" + id + "\\" + name["name"] + ".xlsx");
                    if (System.IO.File.Exists(path))
                    {
                        combined = true;
                    }
                }
                string filePath = "plan\\combine\\" + id;
                var detail = DB.QueryAsDics(sql);
                if (detail == null)
                {
                    detail = new Dictionary<string, object>[0];
                }
                sql = "select etid from e_plan where id = " + id;
                var etid = DB.QueryOne("select etid from e_plan where id = " + id);
                result.Add("combined", combined);
                result.Add("detail", detail);
                result.Add("isAdmin", isAdmin);
                result.Add("etname", name);
                result.Add("etid", etid);
                return Json(new { ok = true, data = result, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                isAdmin = false;
                var etidAuth = DB.QueryOne("select etid,name from e_plan where id = " + id);
                int userid = user.id.Value;
                //if (O2.O2I(etidAuth["etid"]) == -1)
                //{
                //    string taskName = etidAuth["name"].ToString();
                //    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", taskName);
                //    DirectoryInfo fdir = new DirectoryInfo(p);
                //    foreach (FileInfo f in fdir.GetFiles())
                //    {//显示当前目录所有文件   
                //        int cityId = O2.O2I(Path.GetFileNameWithoutExtension(f.FullName).Split('$')[1]);
                //        if(cityId == userid)
                //        {
                //            string cityName = DB.QueryOne("select name from cuser where id = " + cityId)["name"].ToString();
                //            string time = f.LastWriteTime.ToString();
                //            int status = 9;
                //            string fileName = "/uploadfiles/" + taskName + "/" + f.Name;
                //            Dictionary<string, object> temp = new Dictionary<string, object>();
                //            temp.Add("epid", id);
                //            temp.Add("status", status);
                //            temp.Add("name", cityName);
                //            temp.Add("uptime", time);
                //            temp.Add("filename", fileName);
                //            detail2.Add(temp);
                //        }
                //    }
                //    result.Add("detail", detail2);
                //    result.Add("isAdmin", isAdmin);
                //    result.Add("etname", "");
                //    result.Add("etid", -1);
                //    return Json(new { ok = true, data = result, msg = "" }, JsonRequestBehavior.AllowGet);
                //}
                //int whid = O2.O2I(DB.QueryOne("select cw.id from cwarehouse cw join cuser cu on cw.name = cu.name where cu.id = " + user.id)["id"]);
                int whid = user.wid.Value;
                string sql = "SELECT epu.*,CONVERT ( VARCHAR (100), epu.uploadtime, 20 ) uptime, cw.name FROM e_plan_upload epu JOIN cwarehouse cw ON epu.whid = cw.id WHERE epid = " + id + " and cw.id = " + whid;
                var detail = DB.QueryAsDics(sql);
                var name = DB.QueryOne("select et.name from e_templet et join e_plan ep on ep.etid = et.id where ep.id = " + id);
                if (detail == null)
                {
                    detail = new Dictionary<string, object>[0];
                }
                sql = "select etid from e_plan where id = " + id;
                var etid = DB.QueryOne("select etid from e_plan where id = " + id);
                result.Add("detail", detail);
                result.Add("isAdmin", isAdmin);
                result.Add("etname", name);
                result.Add("etid", etid);
                return Json(new { ok = true, data = result, msg = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult GetCityInfo()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            cuser user = (cuser)Session["loginuser"];
            string sql = "select id,name from cwarehouse";
            var city = DB.QueryAsDics(sql);
            if (city == null)
            {
                city = new Dictionary<string, object>[0];
            }
            List<object> l = new List<object>();
            Dictionary<string, object> a = new Dictionary<string, object>();
            a.Add("id", 999);
            a.Add("name", "全省");
            l.Add(a);
            foreach (var c in city)
            {
                l.Add(c);
            }
            result.Add("city", l);
            result.Add("user", user);
            return Json(new { ok = true, data = result, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult GetUploadInfo(int whid, int epid) {
            Dictionary<string, object> result = new Dictionary<string, object>();
            string sql = "select * from e_plan_upload epu join cwarehouse cw on epu.whid = cw.id where cw.id = "+whid+" and epu.epid = "+epid;
            bool isUp = true;
            var data = DB.QueryAsDics(sql);
            if (data == null) {
                data = new Dictionary<string, object>[0];
                isUp = false;
            }
            result.Add("isUp", isUp);
            result.Add("data", data);
            return Json(new { ok = true, data = result, msg = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlanClass() {
            string sql = "select DISTINCT planclass from e_plan";
            var result = DB.QueryAsDics(sql);
            return Json(new { ok = true, data = result, msg = "", total=result == null ? 0 : result.Length }, JsonRequestBehavior.AllowGet);
        }

        [Breadcrumb(Auth = "0")]
        public ActionResult InsertTask(string name, int etid, int plannum, string plantime, string planclass)
        {
            cuser user = (cuser)Session["loginuser"];
            if (user.utype != 0)
            {
                return Json(new { ok = false, msg = "您没有权限！" }, JsonRequestBehavior.AllowGet);
            }
            //if(etid == -1)
            //{
            //    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles");
            //    if(!Directory.Exists(p))
            //    {
            //        Directory.CreateDirectory(p);
            //    }
            //    p = Path.Combine(p, name);
            //    Directory.CreateDirectory(p);
            //}
            string sql = "insert into e_plan (name, etid, plannum,uploadnum, createtime, plantime, createuid, status, planclass) values ('" + name + "'," + etid + "," + plannum + ",0,getdate(),'" + plantime + "'," + user.id + ",0,'"+planclass+"')";
            int id = DB.Insert(sql);
            //return Json(new { ok = true, id=id, msg = "" }, JsonRequestBehavior.AllowGet);
            return Redirect("/plan");
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult UpdateTask(int id,string name,int status, int plannum, string plantime, int etid, string donetime, string planclass)
        {
            cuser user = (cuser)Session["loginuser"];
            if (user.utype != 0)
            {
                return Redirect("/plan/option#" + id+"?msg=没有权限");
            }
            string sql = "select * from e_plan where name = '" + name + "' and id <> '" + id + "'";
            if (DB.QueryOne(sql) != null)
            {
                return Redirect("/plan/option#" + id + "?msg=任务名称已存在");
            }
            if (string.IsNullOrEmpty(planclass)) {
                return Redirect("/plan/option#" + id + "?msg=请输入任务分类");
            }
            if (string.IsNullOrEmpty(donetime))
            {
                sql = "update e_plan set name = '" + name + "', plannum = " + plannum + ", plantime = '" + plantime + "', etid = " + etid + ", status =" + status + ", planclass ='"+planclass+"' where id = " + id;
            }
            else
            {
                sql = "update e_plan set name = '" + name + "', plannum = " + plannum + ", plantime = '" + plantime + "', etid = " + etid + ", status =" + status + ", completetime = '" + donetime + "', planclass ='" + planclass + "' where id = " + id;
            }
            DB.Exec(sql);
            return Redirect("/plan/option#"+id);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult DeleteTask(int id)
        {
            cuser user = (cuser)Session["loginuser"];
            if (user.utype != 0)
            {
                return Json(new { ok = false, msg = "您没有权限！" }, JsonRequestBehavior.AllowGet);
            }
            var dt = DB.QueryOne("select * from e_plan where id = " + id);
            if (dt == null)
            {
                return Json(new { ok = false, msg = "任务未保存或不存在！" });
            }
            DB.Exec("delete from e_plan where id = " + id);
            DB.Exec("delete from e_plan_upload where epid = " + id);
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\target\\" + id);
            if(Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\combine\\" + id);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            return Json(new { ok = true, msg = "删除成功！" });
        }
        public ActionResult downplan()
        {
            int epid = O2.O2I(Request.QueryString.Get("epid"));
            int whid = O2.O2I(Request.QueryString.Get("whid"));
            var name = DB.QueryOne("select filename from e_plan_upload where epid = " + epid + " and whid = "+whid);
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, name["filename"].ToString().Substring(1));
            return File(path, "application/octet-stream", Path.GetFileName(path));
        }
        private int SpliteFiles(int id,int whid, HttpFileCollectionBase files, cuser user, string spliteName, int etid)
        {
            if (whid != 999)
            {
                var dt = DB.Query("select * from e_plan_upload where epid = " + id + " and whid = " + whid);
                string fname = files[0].FileName.Substring(files[0].FileName.LastIndexOf('\\') + 1);
                if (dt.Rows.Count != 0)
                {
                    string tempfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dt.Rows[0]["filename"].ToString().Substring(1).Replace('/', '\\'));
                    if (System.IO.File.Exists(tempfile))
                    {
                        System.IO.File.Delete(tempfile);
                    }
                }
                string temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + whid + "E\\" + id);
                if (!Directory.Exists(temp))
                {
                    Directory.CreateDirectory(temp);
                }
                string filename = "";
                for (int i = 0; i < 1; i++)
                {
                    filename = files[i].FileName.Substring(files[i].FileName.LastIndexOf('\\') + 1);
                    files[i].SaveAs(temp + "\\" + filename);
                    fname = "/temp/" + whid + "E/" + id + "/" + filename;
                }
                string sql = "";
                if (dt.Rows.Count == 0)
                {
                    sql = "insert into e_plan_upload (epid, status, uploadtime, filename, upuid, whid) values (" + id + ",0,getdate(),'" + fname + "'," + user.id + ", " + whid + ")";
                    DB.Insert(sql);
                    if (etid == -1)
                    {
                        sql = "update e_plan set uploadnum = uploadnum + 1 where id =" + id;
                        DB.Exec(sql);
                    }
                }
                else
                {
                    sql = "update e_plan_upload set uploadtime = getdate(), filename = '" + fname + "', status = 0 where epid = " + id + " and whid = " + whid;
                    DB.Exec(sql);
                }
                if(etid != -1)
                {
                    etempletfactory.Analysis(whid, id, fname.Substring(1), user.id.Value);
                    var status = DB.QueryOne("select status from e_plan_upload where whid = " + whid + " and epid = " + id);
                    int s = (int)status["status"] * -1 > 0 ? -4 : 4;
                    DB.Exec("update e_plan_upload set status = " + s + " where whid = " + whid + " and epid = " + id);
                }
                else
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup/target/", id + "/" + user.id.Value);
                    if(!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }
                    if(System.IO.File.Exists(Path.Combine(p, filename)))
                    {
                        System.IO.File.Delete(Path.Combine(p, filename));
                    }
                    System.IO.File.Move(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fname.Substring(1)), Path.Combine(p, filename));

                    sql = "update e_plan_upload set uploadtime = getdate(), filename = '" + ("/planup/target/"+id + "/" + user.id.Value + "/" + filename) + "', status = 9 where epid = " + id + " and whid = " + whid;
                    DB.Exec(sql);
                }
                
                
                return 0;
            }
            else
            {
                string fileName = null;
                string fname = files[0].FileName.Substring(files[0].FileName.LastIndexOf('\\') + 1);
                string temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\plantempsum\\admin" + user.id.Value);
                if(!Directory.Exists(temp))
                {
                    Directory.CreateDirectory(temp);
                }
                for (int i = 0; i < 1; i++)
                {
                    string filename = files[i].FileName.Substring(files[i].FileName.LastIndexOf('\\') + 1);
                    files[i].SaveAs(temp + "\\" + filename);
                    fname = filename;
                }
                bool hasClass = true;
                bool hasComments = true;
                IWorkbook workbook = null;
                FileStream fileStream = new FileStream(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\plantempsum\\admin" + user.id.Value, fname), FileMode.Open, FileAccess.Read);
                if (fname.IndexOf(".xlsx") > 0) // 2007版本  
                {
                    workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook  
                }
                else if (fname.IndexOf(".xls") > 0) // 2003版本  
                {
                    workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook  
                }
                ISheet sheet = workbook.GetSheetAt(0);
                if(sheet.LastRowNum < 1)
                {
                    return -1;
                }
                var ids = DB.QueryOne("select Top 1 etc.etid from e_templet_col etc join e_plan ep on etc.etid = ep.etid where ep.id = " + id);
                etempletfactory rules = etid == -1 ? null : rules = new etempletfactory(O2.O2I(ids["etid"].ToString()));
                IRow row = null;
                int start = 3;
                int index = -1;
                int last = hasClass ? sheet.GetRow(1).Cells.Count : sheet.GetRow(0).Cells.Count;
                for (int i=0;i< last;i++)
                {
                    string name = hasClass ? sheet.GetRow(1).GetCell(i).ToString() : sheet.GetRow(0).GetCell(i).ToString();
                    if(name == spliteName)
                    {
                        index = i;
                        break;
                    }
                }
                if(index == -1)
                {
                    return -2;
                }
                List<string> names = new List<string>();
                for (int i = start; i < sheet.LastRowNum + 1; i++)
                {
                    row = sheet.GetRow(i);
                    if (row != null)
                    {
                        for (int j = 0; j < last; j++)
                        {
                            string name = row.GetCell(index).ToString();
                            if(!names.Contains(name))
                            {
                                names.Add(name);
                            }
                        }
                    }
                }
                foreach(string name in names)
                {
                    string path = findPath(name, fname, id);
                    if(!Directory.Exists(Path.GetDirectoryName(path)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    string tempName = Path.GetFileNameWithoutExtension(path);
                    string extension = Path.GetExtension(path);
                    tempName = tempName + "-" + name + extension;
                    path = findPath(name, tempName, id);
                    System.IO.File.Copy(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, temp, fname), path, true);
                    FileStream fs3 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                    IWorkbook wb = new XSSFWorkbook(fs3);
                    ISheet st = wb.GetSheetAt(0);
                    IRow row3 = null;
                    int delcount = 0;
                    for (int i = start; i < st.LastRowNum + 1; i++)
                    {
                        row3 = st.GetRow(i);
                        if (row3.GetCell(index).ToString() != name)
                        {
                            //if (i == st.LastRowNum)
                            //{
                            //    st.RemoveRow(row3);
                            //    continue;
                            //}
                            //st.ShiftRows(i + 1, st.LastRowNum, -1);
                            //i--;
                            st.RemoveRow(row3);
                            delcount++;
                        }
                        else {
                            if (delcount > 0) {
                                st.ShiftRows(i, st.LastRowNum, -delcount);
                                i-=delcount;
                            }

                            delcount = 0;
                        }
                    }
                    string spath = findPath(name, fname, id);
                    FileStream fs4 = System.IO.File.Create(spath);
                    wb.Write(fs4);
                    fs4.Close();
                    var wid = DB.QueryOne("select id from cwarehouse where name = '" + name + "'");
                    System.IO.File.Delete(path);
                    if (etid != -1)
                    {
                        etempletfactory.Analysis(int.Parse(wid["id"].ToString()), id, "temp/" + wid["id"] + "E/" + id + "/" + fname, user.id.Value);
                        var status = DB.QueryOne("select status from e_plan_upload where whid = " + wid["id"] + " and epid = " + id);
                        int s = (int)status["status"] * -1 > 0 ? -4 : 4;
                        DB.Exec("update e_plan_upload set status = " + s + " where whid = " + wid["id"] + " and epid = " + id);

                    }
                    else
                    {
                        var isEx = DB.QueryOne("select status from e_plan_upload where whid = " + wid["id"] + " and epid = " + id);
                        if(isEx == null)
                        {
                            var sql = "insert into e_plan_upload (epid, status, uploadtime, filename, upuid, whid) values (" + id + ",9,getdate(),'" + ("/temp/" + wid["id"] + "E/" + id + "/" + fname) + "'," + user.id + ", " + wid["id"] + ")";
                            DB.Insert(sql);

                        }
                        else
                        {
                            var sql = "update e_plan_upload set uploadtime = getdate(), filename = '" + ("/temp/" + wid["id"] + "E/" + id + "/" + fname) + "', status = 9 where epid = " + id + " and whid = " + wid["id"];
                            DB.Exec(sql);
                        }
                    }
                }

                return 0;
            }
        }
        private string findPath(string name,string fname, int id)
        {
            var whid = DB.QueryOne("select id from cwarehouse where name = '" + name + "'");
            return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp/" + whid["id"] + "E/" + id + "/" + fname);
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult UploadFile(int id, int whid, string spliteName)
        {

            var status = DB.QueryOne("select status from e_plan where id = " + id);
            if(O2.O2I(status["status"]) == 1)
            {
                return Redirect("/plan/option#" + id + "?msg=任务已关闭，无法上传！");
            }
            cuser user = (cuser)Session["loginuser"];
            var dt = DB.Query("select * from e_plan_upload where epid = " + id + " and whid = " + whid);
            
            var files = Request.Files;
            if (files["file"] == null || files["file"].ContentLength == 0)
            {
                return Redirect("/plan/option#" + id + "?msg=必须上传附件！");
            }
            string fname = files[0].FileName.Substring(files[0].FileName.LastIndexOf('\\') + 1);
            var etid = DB.QueryOne("select etid from e_plan where id =" + id );
           if(O2.O2I(etid["etid"]) != -1)
            {
                if (!(fname.EndsWith(".xls") || fname.EndsWith(".xlsx")))
                {
                    return Redirect("/plan/option#" + id + "?msg=上传文件类型必须是 .xls 或 .xlsx！");
                }
            }
            string sql = "";
            if (user.utype.Value== 0)
            {
                SpliteFiles(id, whid, files, user, spliteName, O2.O2I(etid["etid"]));
                //else
                //{
                //    var isEx = DB.QueryOne("select 1 from e_plan_upload where whid = " + whid + " and epid = " + id);
                //    if(isEx == null)
                //    {
                //        sql = "insert into e_plan_upload (epid, status, uploadtime, filename, upuid, whid) values (" + id + ",9,getdate(),'" + fname + "'," + user.id + ", " + whid + ")";
                //        DB.Insert(sql);
                //    }else
                //    {

                //        sql = "update e_plan_upload set status = 9 where epid = " + id + " and whid = " + whid;
                //        DB.Exec(sql);
                //    }
                //}
                
                return Redirect("/plan/option#" + id + "?msg=已上传！请查看校验结果。");
            }
            if (dt.Rows.Count != 0)
            {
                string tempfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dt.Rows[0]["filename"].ToString().Substring(1).Replace('/', '\\'));
                if (System.IO.File.Exists(tempfile)) {
                    System.IO.File.Delete(tempfile);
                }
            }
            string temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\"+ whid + "E\\" + id );
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            string filename = "";
            for (int i = 0; i < 1; i++)
            {
                filename = files[i].FileName.Substring(files[i].FileName.LastIndexOf('\\') + 1);
                files[i].SaveAs(temp + "\\" + filename);
                fname = "/temp/" + whid + "E/" + id + "/" + filename;
            }
           
            if (dt.Rows.Count == 0) {
                sql = "insert into e_plan_upload (epid, status, uploadtime, filename, upuid, whid) values (" + id + ",0,getdate(),'" + fname + "'," + user.id + ", " + whid + ")";
                DB.Insert(sql);
                if (O2.O2I(etid["etid"]) == -1)
                {
                    sql = "update e_plan set uploadnum = uploadnum + 1 where id =" + id;
                    DB.Exec(sql);
                }
            }
            else
            {
                sql = "update e_plan_upload set uploadtime = getdate(), filename = '"+ fname + "', status = 0 where epid = " + id + " and whid = " + whid ;
                DB.Exec(sql);
            }
            if (O2.O2I(etid["etid"]) != -1)
            {
                etempletfactory.Analysis(whid, id, fname.Substring(1), user.id.Value);
            }
            else
            {
                string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup/target/", id + "/" + user.id.Value);
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
                if (System.IO.File.Exists(Path.Combine(p, filename)))
                {
                    System.IO.File.Delete(Path.Combine(p, filename));
                }
                System.IO.File.Move(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fname.Substring(1)), Path.Combine(p, filename));
                sql = "update e_plan_upload set uploadtime = getdate(), filename = '" + ("/planup/target/" + id + "/" + user.id.Value + "/" + filename) + "', status = 9 where epid = " + id + " and whid = " + whid;
                DB.Exec(sql);
            }
            
            return Redirect("/plan/option#" + id + "?msg=已上传！请查看校验结果。");
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Templet() {
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Ft() {
            var js = DB.QueryAsDics("select * from e_templet");
            return Json(new { total = js == null ? 0 : js.Length, data = js }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult T(int? id) {
            var enums = DB.QueryAsDics("select * from e_enum order by eclass,name");
            if (enums == null) {
                enums = new Dictionary<string, object>[0];
            }
            ViewBag.enums = enums;
            if (!id.HasValue)
            {
                ViewBag.templet = etempletfactory.CreateEmptyEtemplet();
            }
            else {
                try {
                    var templet = new etempletfactory(id.Value);
                    ViewBag.templet = templet.templet;
                }
                catch (Exception ex) {
                    return Content(ex.Message);
                }
            }
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Ftc(int? etid) {
            if (!etid.HasValue) {
                return Json(new { total = 0, data = new Dictionary<string,object>[0] }, JsonRequestBehavior.AllowGet);
            }
            var js = DB.QueryAsDics("select etid,ordernum,name,classname,comments,etctype,eeid,notnull,expr,ecc from e_templet_col where etid="+etid);
            return Json(new { total = js == null ? 0 : js.Length, data = js }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Ut(e_templet et,string cols2) {
            var etid = -1;
            //try
            //{
                et.cols = new Dictionary<int, e_templet_col>();
                var cols = ((Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(cols2));
                foreach (var col in cols)
                {
                    var colo = (e_templet_col)Newtonsoft.Json.JsonConvert.DeserializeObject(col.ToString(), typeof(e_templet_col));
                    et.cols[colo.ordernum.Value] = colo;
                }

                if (et.id.HasValue)
                {
                    etid = et.id.Value;
                    var tf = new etempletfactory(etid);
                    tf.templet.name = et.name;
                    tf.templet.des = et.des;
                    tf.templet.cols = et.cols;
                    tf.templet.filename = @"/plantemp/"+etid+"/"+tf.templet.name+".xlsx";
                    tf.update();
                    excelhelper.CreateTemplet(tf.templet, System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "plantemp\\" + etid + "\\" + tf.templet.name + ".xlsx"));
                }
                else
                {
                    var tf = new etempletfactory(et);
                    etid = tf.templet.id.Value;
                    tf.templet.filename = @"/plantemp/" + etid + "/" + tf.templet.name + ".xlsx";
                    tf.update();
                    excelhelper.CreateTemplet(tf.templet, System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "plantemp\\" + etid + "\\" + tf.templet.name + ".xlsx"));
                }
            //}
            //catch (Exception ex) {
            //    return Json(new {ok=false,msg=ex.Message });
            //}
            return Json(new { ok=true,etid=etid});
        }

        public ActionResult DeleteTemplate(int id) {
            string sql = "select name from e_plan where etid = " + id;
            var data = DB.QueryAsDics(sql);
            if (data != null) {
                return Json(new { ok = false, data=data, msg="" });
            }
            string name = "plantemp\\" + id;
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, name);
            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
            sql = "delete from e_templet where id = " + id;
            DB.Exec(sql);
            sql = "delete from e_templet_col where etid = " + id;
            DB.Exec(sql);
            return Json(new { ok = true, msg = "" });
        }

        public ActionResult ImportTemp()
        {
            var file = Request.Files["upfile"];
            if (file == null)
            {
                return Json(new { ok = false, msg = "必须上传附件" });
            }
            string fname = file.FileName.Substring(file.FileName.LastIndexOf('\\') + 1);
            if (!(fname.EndsWith(".xls") || fname.EndsWith(".xlsx")))
            {
                return Json(new { ok = false, msg = "上传文件类型必须是 .xls 或 .xlsx！" });
            }
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            file.SaveAs(path+"\\"+fname);
            IWorkbook workbook = null;
            FileStream fileStream = new FileStream(path + "\\" + fname, FileMode.Open, FileAccess.Read);
            if (fname.IndexOf(".xlsx") > 0)
            {
                workbook = new XSSFWorkbook(fileStream);
            }
            else if (fname.IndexOf(".xls") > 0) // 2003版本  
            {
                workbook = new HSSFWorkbook(fileStream);
            }
            ISheet sheet = workbook.GetSheetAt(0);
            IRow row = sheet.GetRow(0);
            if (row == null) {
                return Json(new { ok = false, msg = "未找到任何数据" });
            }
            #region 模板扩展
            var errlist = new List<string>();
            var dic_etctype = new Dictionary<string, int>();
            foreach (var et in Enum.GetValues(typeof(e_etctype)))
            {
                dic_etctype[O2.GED(et)] = (int)et;
            }
            var dic_eenums = new Dictionary<string, int>();
            var db_eenums = DB.Query("select id,name from e_enum");
            if (db_eenums != null) {
                dic_eenums = db_eenums.AsEnumerable().ToDictionary(a=>a["name"]+"",a=>(int)a["id"]);
            }
            #endregion
            
            try
            {
                bool hasClass = sheet.LastRowNum > 0;
                bool hasComment = sheet.LastRowNum > 1;
                bool hasOther = sheet.LastRowNum > 2;
                int rows = 0;
                int cols = 0;
                List<string> classNames = new List<string>();
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                if (hasClass)
                {
                    for (int i = 0; i <= row.LastCellNum; i += cols)
                    {
                        ICell cell = row.GetCell(i);
                        if (cell != null)
                        {
                            NPOI.ExcelExtension.IsMergeCell(cell, out rows, out cols);
                            for (int j = 0; j < cols; j++)
                            {
                                classNames.Add(cell+"");
                            }
                        }
                    }
                }
                row = sheet.GetRow(hasClass ? 1 : 0);
                for (int i = 0; i <= row.LastCellNum; i++)
                {
                    ICell cell = row.GetCell(i);
                    if (cell != null)
                    {
                        Dictionary<string, object> temp = new Dictionary<string, object>();
                        temp.Add("data", cell+"");
                        temp.Add("class", hasClass ? classNames[i] : "");
                        temp.Add("comments", hasComment ? sheet.GetRow(2).GetCell(i) == null ? "" : sheet.GetRow(2).GetCell(i)+"" : "");

                        #region 其他列解析
                        if (hasOther) {
                            var col_ecttype = sheet.GetRow(3).GetCell(i)+"";
                            temp.Add("etctype", !dic_etctype.ContainsKey(col_ecttype) ? 0 : dic_etctype[col_ecttype]);

                            var col_notnull = sheet.GetRow(5).GetCell(i)+"";
                            temp.Add("notnull", col_notnull=="必填"?true:false);

                            var col_expr = sheet.GetRow(6).GetCell(i)+"";
                            temp.Add("expr", col_expr==""?"":col_expr[0]=='='?col_expr:"="+col_expr);

                            var col_ecc = sheet.GetRow(7).GetCell(i) + "";
                            temp.Add("ecc", col_ecc);


                            var col_eeid = -1;
                            var col_eedesc = sheet.GetRow(4).GetCell(i) + "";
                            var col_eearry = col_eedesc.Split(new char[]{'|'});
                            if (col_eedesc == "")
                            {
                                col_eeid = -1;
                            }
                            else {
                                if (dic_eenums.ContainsKey(col_eearry[0]))
                                {
                                    col_eeid = dic_eenums[col_eearry[0]];
                                }
                                else {
                                    if (col_eearry.Length < 2)
                                    {
                                        errlist.Add("枚举“" + col_eearry[0] + "”不存在且模板没有定义");
                                    }
                                    else {
                                        col_eeid = -2;
                                    }
                                }
                            }
                            temp.Add("eeid", col_eeid);
                            temp.Add("eedesc", col_eedesc);
                        }
                        #endregion

                        result.Add(temp);
                    }
                }
                if (errlist.Count > 0) {
                    return Json(new { ok = false, msg = "上传的文件结构未能识别，（" + string.Join(",", errlist) + "）" });
                }
                return Json(new { ok = true, data = result, msg = "" });
            } catch (Exception e) {
                return Json(new { ok = false, msg = "上传的文件结构未能识别，（"+string.Join(",",errlist)+"）" });
            }
        }

        public ActionResult GetFiles()
        {
            cuser user = (cuser)Session["loginuser"];
            string rootpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles");
            if(!Directory.Exists(rootpath))
            {
                Directory.CreateDirectory(rootpath);
            }
            Dictionary<string, List<Dictionary<string, object>>> res = new Dictionary<string, List<Dictionary<string, object>>>();
            res.Add(Path.GetFileName(rootpath), new List<Dictionary<string, object>>());
            FindAllFiles(rootpath, res, user.utype.Value, user.id.Value);
            return Json(new { ok = true, data = res }, JsonRequestBehavior.AllowGet);
        }

        private void FindAllFiles(string path, Dictionary<string, List<Dictionary<string, object>>> res,int utype, int id)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                if(utype == 0 || Path.GetFileNameWithoutExtension(files[i]).EndsWith("$"+id))
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("type", "file");
                    dic.Add("name", Path.GetFileName(files[i]));
                    res[Path.GetFileName(path)].Add(dic);
                }
            }
            if (dirs.Length == 0) return;
            for (int i=0;i< dirs.Length;i++)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("type", "dir");
                dic.Add("name", Path.GetFileName(dirs[i]));
                Dictionary<string, List<Dictionary<string, object>>> n = new Dictionary<string, List<Dictionary<string, object>>>();
                n.Add(Path.GetFileName(dirs[i]), new List<Dictionary<string, object>>());
                dic.Add("children", n);
                res[Path.GetFileName(path)].Add(dic);
                FindAllFiles(dirs[i], n, utype, id);
            }
        }

        public ActionResult CreateDir(string name, string path)
        {
            if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path))
            {
                return Json(new { ok=false, message="路径或名称不存在" });
            }
            string distPath = "";
            if(path == "root")
            {
                distPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", name);
            }
            else
            {
                distPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", path, name);
            }
             
            try
            {
                Directory.CreateDirectory(distPath);
                return Json(new { ok = true});
            }
            catch(Exception e)
            {
                return Json(new { ok = false, message = "目标文件夹创建失败" });
            }
        }

        public ActionResult DeleteFile(string path) {
            if(string.IsNullOrEmpty(path))
            {
                return Json(new { ok=false, message="路径不存在" });
            }
            string distPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", path);
            if(Directory.Exists(distPath))
            {
                Directory.Delete(distPath, true);
                return Json(new { ok = true });
            }
            else if(System.IO.File.Exists(distPath))
            {
                System.IO.File.Delete(distPath);
                return Json(new { ok = true });
            }
            else
            {
                return Json(new { ok = false, message="未找到对应的文件，删除失败" });
            }
        }

        public ActionResult UploadFiles()
        {
            cuser user = (cuser)Session["loginuser"];
            int cityid = user.id.Value;
            if(user.utype.Value == 0)
            {
                int whid = O2.O2I(Request.Form["whid"]);
                var a = DB.QueryOne("select cu.id from cuser cu  where utype=1 and cu.wid =" + whid);
                cityid = O2.O2I(a["id"]);
            }
            var files = Request.Files;
            int id = O2.O2I(Request.Form["id"]);
            var taskName = DB.QueryOne("select name from e_plan where id = " + id);
            string pathDIr = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", taskName["name"].ToString());
            DirectoryInfo fdir = new DirectoryInfo(pathDIr);
            foreach (FileInfo f in fdir.GetFiles())
            {//显示当前目录所有文件   
                int cityId = O2.O2I(Path.GetFileNameWithoutExtension(f.FullName).Split('$')[1]);
                if(cityId == user.id.Value)
                {
                    System.IO.File.Delete(f.FullName);
                }
            }
            if (files.Count == 0)
            {
                return Redirect("/plan/option#" + id+ "-n?message=未选择文件");
            }
            try
            {
                string fname = files[0].FileName.Substring(files[0].FileName.LastIndexOf('\\') + 1);
                string distPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", taskName["name"].ToString(), fname);
                string name = Path.GetFileNameWithoutExtension(distPath) + "$" + cityid;
                string extension = Path.GetExtension(distPath);
                string dirName = Path.GetDirectoryName(distPath);
                distPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", dirName, name + extension);
                files[0].SaveAs(distPath);
                return Redirect("/plan/option#" + id + "-n?message=上传成功");
            }
            catch(Exception e)
            {
                return Redirect("/plan/option#" + id + "-n?message=上传失败");
            }
        }

        public ActionResult DownLoadFile(string path)
        {
            cuser user = (cuser)Session["loginuser"];
            if(user.utype.Value == 0)
            {
                return Json(new { ok = true });
            }
            else
            {
                string distPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "uploadfiles", path);
                string name = Path.GetFileNameWithoutExtension(distPath);
                if(name.EndsWith("$"+user.id.Value))
                {
                    return Json(new { ok = true });
                }
                else
                {
                    return Json(new { ok = false});
                }
            }
        }
    }
}
