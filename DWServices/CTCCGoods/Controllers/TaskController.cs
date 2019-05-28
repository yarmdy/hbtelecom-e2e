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
    }
}
