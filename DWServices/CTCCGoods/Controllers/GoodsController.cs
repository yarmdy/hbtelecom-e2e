using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTCCGoods.Controllers
{
    public class GoodsController : Controller
    {
        //
        // GET: /Goods/
        [Breadcrumb(Auth = "1234")]
        public ActionResult Index()
        {
            return Redirect("/goods/cgoods");
        }
        [Breadcrumb(Auth = "1234")]
        public ActionResult Cgoods()
        {
            var classes = DB.QueryAsDics("select id as cid, name as cname from cclass");
            var changjia = DB.QueryAsDics("select id,name from cuser where utype = 3 and status = 1");
            if (classes == null)
            {
                classes = new Dictionary<string, object>[0];
            }
            if (changjia == null)
            {
                changjia = new Dictionary<string, object>[0];
            }
            ViewBag.classbag = classes;
            ViewBag.changjia = changjia;
            return View();
        }
        [Breadcrumb(Auth = "1234")]
        public ActionResult Cclass()
        {
            return View();
        }

        [Breadcrumb(Auth = "1234")]
        public ActionResult Query(int limit, int offset, string sort, string order)
        {
            string sql = "select id,name,code from cclass";
            var data = DB.QueryAsDics(sql);
            if (data == null)
            {
                return Json(new { total = 0, rows = 0 }, JsonRequestBehavior.AllowGet);
            }
            var total = data.Length;
            var rows = data.ToList();
            if (sort != null)
            {
                if (order == "asc")
                {
                    rows = rows.OrderBy(a => a[sort]).ToList();
                }
                else
                {
                    rows = rows.OrderByDescending(a => a[sort]).ToList();
                }
            }
            rows = rows.Skip(offset).Take(limit).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "1234")]
        public ActionResult QueryGood(int limit, int offset, string sort, string order)
        {
            string sql = "select g.id, g.name, g.code, g.cid, c.name as cname,g.class2,g.pid,u.name pname from cgoods g left join cclass c on g.cid = c.id left join cuser u on g.pid = u.id";
            var data = DB.QueryAsDics(sql);
            if (data == null)
            {
                return Json(new { total = 0, rows = 0 }, JsonRequestBehavior.AllowGet);
            }
            var total = data.Length;
            var rows = data.ToList();
            if (sort != null)
            {
                if (sort == "cid")
                {
                    sort = "cname";
                }
                if (sort == "pid")
                {
                    sort = "pname";
                }
                if (order == "asc")
                {
                    rows = rows.OrderBy(a => a[sort]).ToList();
                }
                else
                {
                    rows = rows.OrderByDescending(a => a[sort]).ToList();
                }
            }
            rows = rows.Skip(offset).Take(limit).ToList();

            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Update(int id, string name, string code)
        {

            string sql = "select * from cclass where name = '" + name + "' and id <> '" + id + "'";
            if (DB.QueryOne(sql) != null)
            {
                return Json(new { status = "error", msg = "名称已存在" }, JsonRequestBehavior.AllowGet);
            }
            sql = "update cclass set  name ='" + name + "',code = '" + code + "' where id=" + id;
            DB.Exec(sql);
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult UpdateGood(int id, string name, string code, string pid, string cid, string class2)
        {
            string sql = "select * from cgoods where name = '" + name + "' and id <> '" + id + "'";
            if (DB.QueryOne(sql) != null)
            {
                return Json(new { status = "error", msg = "名称已存在" }, JsonRequestBehavior.AllowGet);
            }
            sql = "select id from cclass where id = '" + cid + "'";
            var cido = DB.QueryAsDics(sql);
            if (cido == null)
            {
                return Json(new { status = "error", msg = "分类名称不存在" }, JsonRequestBehavior.AllowGet);
            }
            sql = "update cgoods set class2 = '" + class2 + "',  name ='" + name + "',code = '" + code + "',cid = '" + cid + "',pid = " + pid + " where id=" + id;
            DB.Exec(sql);

            return Json(new { status = "success", cid = cid }, JsonRequestBehavior.AllowGet);
        }

        public int Maxid()
        {
            string sql = "select max(id) as id from cclass";
            var data = DB.QueryAsDics(sql);
            return int.Parse(data[0]["id"].ToString());
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Insert(string name, string code)
        {
            string sql = "select * from cclass where name = '" + name + "'";
            if (DB.QueryOne(sql) != null)
            {
                return Json(new { status = "error", msg = "类型已存在" }, JsonRequestBehavior.AllowGet);
            }
            sql = "insert into cclass (name, code) values ('" + name + "','" + code + "');";
            int id = DB.Insert(sql);
            return Json(new { status = "success", id = id }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult InsertGood(string name, string code, string cid, string pid, string class2)
        {
            string sql = "select * from cgoods where name = '" + name + "'";
            if (DB.QueryOne(sql) != null)
            {
                return Json(new { status = "error", msg = "型号已存在" }, JsonRequestBehavior.AllowGet);
            }
            sql = "select id from cclass where id = '" + cid + "'";
            var cido = DB.QueryAsDics(sql);
            if (cido == null)
            {
                return Json(new { status = "error", msg = "设备类型不存在" }, JsonRequestBehavior.AllowGet);
            }
            sql = "insert into cgoods (name, code, cid, pid, class2) values ('" + name + "','" + code + "','" + cid + "'," + pid + ", '" + class2 + "');";
            var id = DB.Insert(sql);
            return Json(new { status = "success", id = id, cid = cid }, JsonRequestBehavior.AllowGet);
        }

        [Breadcrumb(Auth = "0")]
        public ActionResult DeleteClass(int id)
        {
            var dt = DB.QueryOne("select * from cclass where id = " + id);
            if (dt == null)
            {
                return Json(new { ok = false, msg = "设备类型不存在！" });
            }
            var data = DB.QueryAsDics("select name from cgoods where cid = " + id);
            if (data != null)
            {
                return Json(new { ok = false, msg = "该类型下存在以下型号，无法删除！", data = data });
            }
            DB.Exec("delete from cclass where id = " + id);
            return Json(new { ok = true, msg = "删除成功！" });
        }

        [Breadcrumb(Auth = "0")]
        public ActionResult Rg(int id)
        {
            var good = DB.QueryOne("select * from cgoods where id=" + id);
            if (good == null)
            {
                return Json(new { ok = false, msg = "设备型号不存在！" });
            }
            List<string> err = new List<string>();
            var orders = DB.QueryOne("select count(0)num from corder_goods where gid=" + id);
            var ordernum = O2.O2I(orders["num"]);
            if (ordernum > 0)
            {
                err.Add("单据");
            }
            var stocks = DB.QueryOne("select count(0)num from cstock where gid=" + id);
            var stocknum = O2.O2I(stocks["num"]);
            if (stocknum > 0)
            {
                err.Add("库存");
            }
            if (err.Count > 0)
            {
                return Json(new { ok = false, msg = "设备型号在" + (string.Join("和", err)) + "中已存在！" });
            }
            var res = DB.Exec("delete cgoods where id=" + id);
            if (res <= 0)
            {
                return Json(new { ok = false, msg = "未知错误！" });
            }
            return Json(new { ok = true, msg = "" });
        }

    }
}
