using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTCCGoods.Controllers
{
    public class TaskController : Controller
    {
        //
        // GET: /Task/
        [Breadcrumb(Auth = "0")]
        public ActionResult Index()
        {
            string allmsg;
            ViewBag.allstatus = ctasksHandle.getallstatus(out allmsg);
            ViewBag.allmsg = allmsg;
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Ft(int limit, int offset)
        {
            var rows = DB.QueryAsDics("select top " + limit + " * from(select a.*,row_number()over(order by iif(tstatus=2,-1,tstatus) desc,iif(tstatus=2,-id, id) asc) top1 from ctasks a) a where top1>" + offset);

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    if (row["createtime"] != DBNull.Value)
                        row["createtime"] = ((DateTime)row["createtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    if (row["starttime"] != DBNull.Value)
                        row["starttime"] = ((DateTime)row["starttime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    if (row["endtime"] != DBNull.Value)
                        row["endtime"] = ((DateTime)row["endtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }

            var totalrow = DB.QueryOne("select count(0) count from ctasks a");
            var total = totalrow == null ? 0 : O2.O2I(totalrow["count"]);
            return Json(new {total=total,rows=rows },JsonRequestBehavior.AllowGet); ;
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Rst() {
            ctasksHandle.reset();
            return Json(new {ok=true,msg="" },JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Dt(int id) {
            var task=DB.QueryOne("select * from ctasks where id="+id);

            if (task == null) {
                return Json(new { ok = false, msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
            var errpath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "taskerr");
            var errfile = task["errurl"].ToString();
            if (!string.IsNullOrEmpty(errfile)) {
                errfile = System.IO.Path.Combine(errpath, errfile);
                if (System.IO.File.Exists(errfile)) {
                    System.IO.File.Delete(errfile);
                }
            }
            DB.Exec("delete ctasks where id="+id);
            return Json(new { ok = true, msg = "已删除("+id+")" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Ttf()
        {
            int ts=DB.Exec("delete ctasks where tdesc='失败'");
            var errpath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "taskerr");
            if (System.IO.Directory.Exists(errpath)) {
                System.IO.Directory.Delete(errpath,true);
            }
            return Json(new { ok = true, msg = "已清空失败"+ts+"条" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Warning(int id) {
            var taskq = DB.QueryOne("select * from ctasks where id='"+id+"'");
            if (taskq == null) {
                return Content("任务不存在");
            }
            var task = ctasksHandle.dic2ctasks(taskq);
            if (task.tdesc != "暂停") {
                return Content("非法操作");
            }
            ViewBag.task = task;
            ViewBag.errinfo = "";
            if (task.errurl != null) {
                var errfile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "taskerr/"+task.errurl);
                ViewBag.errinfo = System.IO.File.ReadAllText(errfile,System.Text.Encoding.GetEncoding("gbk"));
            }
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Prowarning(int id,int conti)
        {
            var taskq = DB.QueryOne("select * from ctasks where id='" + id + "'");
            if (taskq == null)
            {
                return Json(new { ok=false,msg="任务不存在"},JsonRequestBehavior.AllowGet);
            }
            var task = ctasksHandle.dic2ctasks(taskq);
            if (task.tdesc != "暂停")
            {
                return Json(new { ok = false, msg = "非法操作" }, JsonRequestBehavior.AllowGet);
            }
            if (conti == 0) {
                DB.Exec("update ctasks set tdesc='失败' where id='"+id+"'");
                if (task.errurl != null) {
                    var errfile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "taskerr/" + task.errurl);
                    System.IO.FileStream fs = new System.IO.FileStream(errfile,System.IO.FileMode.Append);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs,System.Text.Encoding.GetEncoding("gbk"));
                    sw.Write("任务已中断\r\n");
                    sw.Close();
                    fs.Close();
                }
            } else {
                DB.Exec("update ctasks set tdesc='等待',tstatus=0,rul='ok' where id='" + id + "'");
                ctasksHandle.reset();
            }
            return Json(new { ok = true, msg = "成功" }, JsonRequestBehavior.AllowGet);
        }
    }
}
