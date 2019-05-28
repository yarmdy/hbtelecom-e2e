using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTCCGoods.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        [Breadcrumb(Auth = "0")]
        public ActionResult Index()
        {
            var whs = DB.QueryAsDics("select * from cwarehouse");
            if (whs == null) { 
                whs=new Dictionary<string,object>[0];
            }
            ViewBag.whs = whs;
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Select()
        {
            var data = DB.QueryAsDics("select d.uid,u.name as uname, d.wid,c.name as wname from cdesignset d left join cuser u on d.uid = u.id left join cwarehouse c on d.wid = c.id");
            var sjy = DB.QueryAsDics("select id, name from cuser where utype = 2 and status =1 ");
            var ds = DB.QueryAsDics("select id, name from cwarehouse");
            if (data == null)
            {
                data = new Dictionary<string, object>[0];
            }
            if (sjy == null)
            {
                sjy = new Dictionary<string, object>[0];
            }
            if (ds == null)
            {
                ds = new Dictionary<string, object>[0];
            }
            ViewBag.sda = data;
            ViewBag.sjy = sjy;
            ViewBag.ds = ds;
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Fu() {
            var js = DB.QueryAsDics("select u.id,u.code,u.pwd,u.name,u.utype,u.status,isnull(u.wid,-1) wid,w.name wname,u.tel,u.contacts from cuser u left join cwarehouse w on u.wid=w.id");
            return Json(new {total=js.Length,data=js },JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth="0")]
        public ActionResult Eu(cuser u)
        {
            try
            {
                //string pattern = @"^(?![a-zA-Z]+$)(?![A-Z0-9]+$)(?![A-Z\W_]+$)(?![a-z0-9]+$)(?![a-z\W_]+$)(?![0-9\W_]+$)[a-zA-Z0-9\W_]{6,}$";
                string pattern = "^.*(?=.{10,})(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).*$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(u.pwd, pattern))
                {
                    return Json(new { status = false, msg = "修改失败，密码不符合规范" }, JsonRequestBehavior.AllowGet);
                }
                var ut = DB.Query("select * from cuser where id='" + u.id + "'");
                if (ut == null || ut.Rows.Count <= 0)
                {
                    return Json(new { ok = false, msg = "修改失败，修改的用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                ut = DB.Query("select * from cuser where id<>'" + u.id + "' and code='" + u.code + "'");
                if (ut != null && ut.Rows.Count > 0)
                {
                    return Json(new { ok = false, msg = "修改失败，登录名已存在" }, JsonRequestBehavior.AllowGet);
                }
                int count = DB.Exec("update cuser set code='" + u.code + "',name='" + u.name + "',utype='"+u.utype+"',status='"+u.status+"',wid='"+u.wid+"',tel='"+u.tel+"',contacts='"+u.contacts+"', pwd = '"+u.pwd+"' where id='" + u.id + "'");
                if (count < 1)
                {
                    return Json(new { ok = false, msg = "修改失败。" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { ok = true, msg = "修改成功。" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { ok = false, msg = "修改失败。" }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Au(cuser u)
        {
            if (u == null)
            {
                return Json(new { ok = false, msg = "新增失败，没有数据" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(u.code) || string.IsNullOrWhiteSpace(u.name))
            {
                return Json(new { ok = false, msg = "新增失败，登录名和姓名不能为空" }, JsonRequestBehavior.AllowGet);
            }
            string pattern = @"^(?![a-zA-Z]+$)(?![A-Z0-9]+$)(?![A-Z\W_]+$)(?![a-z0-9]+$)(?![a-z\W_]+$)(?![0-9\W_]+$)[a-zA-Z0-9\W_]{6,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(u.pwd, pattern))
            {
                return Json(new { status = false, msg = "新增失败，密码不符合规范" }, JsonRequestBehavior.AllowGet);
            }
            var ua = DB.QueryOne("select * from cuser where code='" + u.code + "'");
            if (ua != null)
            {
                return Json(new { ok = false, msg = "新增失败，已存在登录名：" + u.code }, JsonRequestBehavior.AllowGet);
            }
            int identity = DB.Insert("insert into cuser values('" + u.code + "','" + u.name + "','" + u.utype + "','" + u.status + "','" + (string.IsNullOrWhiteSpace(u.pwd)?"Hbdx1331":u.pwd) + "','" + u.wid + "','" + u.tel + "','" + u.contacts + "')");
            if (identity < 1)
            {
                return Json(new { ok = false, msg = "新增失败。" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ok = true, msg = "新增成功", id = identity }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult ChangeSjy(int type, int sid, int did)
        {
            string sql = "";
            if(type == 0)
            {
                sql = "insert into cdesignset (uid, wid) values ("+sid+", "+did+")";
            }
            else
            {
                sql = "delete from cdesignset where uid = "+sid+" and wid = "+did;
            }
            int n = DB.Exec(sql);
            if(n>0)
            {
                return Json(new { ok = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ok = false, msg = "操作异常" }, JsonRequestBehavior.AllowGet);
        }
    }
}
