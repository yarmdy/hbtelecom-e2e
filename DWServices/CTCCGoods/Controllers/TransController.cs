using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTCCGoods.Controllers
{
    public class TransController : Controller
    {
        //
        // GET: /Trans/
        //首页
        public ActionResult Index()
        {
            var createuser = DB.QueryAsDics("select * from cuser where utype=" + ((int)crole.city) + " and status = 1 order by name");
            if (createuser == null)
            {
                createuser = new Dictionary<string, object>[0];
            }
            var receiveuser = DB.QueryAsDics("select * from cuser where utype=" + ((int)crole.manufactor) + " order by name");
            if (receiveuser == null)
            {
                receiveuser = new Dictionary<string, object>[0];
            }
            var ostatus = new Dictionary<int, string>();
            foreach (torderstatus os in Enum.GetValues(typeof(torderstatus)))
            {
                ostatus[(int)os] = O2.GED(os);
            }
            ViewBag.cus = createuser;
            ViewBag.rus = receiveuser;
            ViewBag.oss = ostatus;
            return View();
        }
        //查询首页
        public ActionResult Fo(int limit, int offset, string key, int cu,int send, int ru, int os, int os2)
        {
            cuser u = (cuser)Session["loginuser"];
            var where = "where 1=1";
            if (!string.IsNullOrWhiteSpace(key))
            {
                where += " and o.code like '%" + key + "%'";
            }
            if (cu > 0)
            {
                where += " and o.createuid=" + cu;
            }
            if(send > 0)
            {
                where += " and o.receiveuid=" + send;
            }
            if (u.utype.Value == (int)crole.city)
            {
                where += " and (o.createuid=" + u.id + " or o.receiveuid=" + u.id + ")";
            }
            if (u.utype.Value == (int)crole.design)
            {
                var wids = u.wids;
                wids.Add(-2);
                var idstr = string.Join(",", wids);
                where += " and (cu.wid in (" + idstr + ") or ru.wid in (" + idstr + "))";
            }
            if (u.utype.Value == (int)crole.supervisor)
            {
                where += " and 1 = 2";
            }
            if (ru > 0)
            {
                where += " and o.changid=" + ru;
            }
            if (u.utype.Value == (int)crole.manufactor)
            {
                where += " and o.changid=" + u.id;
            }
            if (os != -10)
            {
                where += " and o.status=" + os;
            }
            if (os2 != -10)
            {
                where += " and o.status=" + (os2 - 1);
            }
            var rows = ctorderfactory.Select(where, limit, offset);
            var total = ctorderfactory.SelectTotal(where);
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }
        //Byid页面
        public ActionResult Byid(int? id)
        {
            cuser user = (cuser)Session["loginuser"];

            string sql = "select  id, name from cuser where name <> '" + user.name + "' and utype = "+ (int)crole.city;
            var city = DB.QueryAsDics(sql);
            if (city == null)
            {
                city = new Dictionary<string, object>[0];
            }
            ViewBag.city = city;
            sql = "select  id, name from cuser where utype = 3 and status = 1";
            var chang = DB.QueryAsDics(sql);
            if (chang == null)
            {
                chang = new Dictionary<string, object>[0];
            }
            ViewBag.chang = chang;
            sql = "select  id,name from cgoods order by name";
            var good = DB.QueryAsDics(sql);
            if (good == null)
            {
                good = new Dictionary<string, object>[0];
            }
            ViewBag.good = good;
            if (id.HasValue)
            {
                try
                {
                    var cf = new ctorderfactory(id.Value);
                    ViewBag.order = cf.order;
                    var chsum = ctorderfactory.GetCanSendGoodsNum(id.Value);
                    ViewBag.chsum = chsum;
                    sql = string.Format(@"SELECT
	cgoods.id,
	cgoods.name,
cgoods.class2,
cgoods.cid,
cc.name cname,
cgoods.pid,
cu.name pname,
	CASE
WHEN cs.stock < cs.require THEN
	cs.stock
ELSE
	cs.require
END stock
FROM
	cstock cs
LEFT JOIN cgoods ON cs.gid = cgoods.id
left join cclass cc on cc.id = cgoods.cid
left join cuser cu on cu.id = cgoods.pid
WHERE
	cs.stock <> 0
AND cs.wid = (select wid from cuser where id = {0})
AND cgoods.pid = {1}", cf.order.receiveuid,cf.order.changid);
                    ViewBag.goodList = DB.QueryAsDics(sql);
                }
                catch (Exception e)
                {
                    return Content(e.Message);
                }
            }
            else
            {
                ViewBag.order = ctorderfactory.CreateEmptyCtorder();
            }
            return View();
        }

        //public ActionResult ChangeGood(int goodId)
        //{
        //    string sql = string.Format("select cu.id,cu.name from cstock cs left join cwarehouse cw on cs.wid = cw.id left join cuser cu on cw.name = cu.name where cs.gid = {0}", goodId);
        //    var cityList = DB.QueryAsDics(sql);
        //    return Json(new { ok = true, city = cityList, msg = "" }, JsonRequestBehavior.AllowGet);
        //}

        //通过城市ID查询仓库货物
        public ActionResult QueryGoodByCityId(int cityId, int changId)
        {
            string sql = string.Format(@"SELECT
	cgoods.id,
	cgoods.name,
cgoods.class2,
cgoods.cid,
cc.name cname,
cgoods.pid,
cu.name pname,
	CASE
WHEN cs.stock < cs.require THEN
	cs.stock
ELSE
	cs.require
END stock
FROM
	cstock cs
LEFT JOIN cgoods ON cs.gid = cgoods.id
left join cclass cc on cc.id = cgoods.cid
left join cuser cu on cu.id = cgoods.pid
WHERE
	cs.stock <> 0
AND cs.wid = (select wid from cuser where id = {0})
AND cgoods.pid = {1}", cityId, changId);
            var goodList = DB.QueryAsDics(sql);
            return Json(new { ok = true, goodList = goodList, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        //保存
        [Breadcrumb(Auth = "1")]
        public ActionResult Save(int cityId, int changId, string data, int id)
        {
            cuser user = (cuser)Session["loginuser"];
            
            if (!user.wid.HasValue)
            {
                return Json(new { ok = false, msg = "未找到您的所在地市，请联系管理员" });
            }
            int type = user.utype.Value;
            if (type != 1)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            if (data == null || data == "")
            {
                return Json(new { ok = false, msg = "数据填写有误！" });
            }
            List<int> gids = new List<int>();
            if (id == -1)
            {
                int createid = ((cuser)Session["loginuser"]).id.Value;
                List<ctorder_goods> cgl = new List<ctorder_goods>();
                string[] ss = data.Split('|');
                for (int i = 0; i < ss.Length; i++)
                {
                    ctorder_goods cg = new ctorder_goods();
                    cg.gid = int.Parse(ss[i].Split(',')[0]);
                    cg.gnum = int.Parse(ss[i].Split(',')[1]);
                    if (ss[i].Split(',')[1] == "" || ss[i].Split(',')[0] == "")
                    {
                        return Json(new { ok = false, msg = "数据填写有误！" });
                    }
                    cgl.Add(cg);
                    if (gids.Contains(int.Parse(ss[i].Split(',')[0])))
                    {
                        return Json(new { ok = false, msg = "不能存在两笔相同的货物！" });
                    }
                    gids.Add(int.Parse(ss[i].Split(',')[0]));
                }
                string sql = "select name from cuser where id = " + changId;
                string name = (string)DB.QueryOne(sql)["name"];
                ctorder o = new ctorder() { createuid = createid, receiveuid = cityId, changid=changId, changname = name };
                ctorderfactory cf = new ctorderfactory(o, cgl);
                int orderid = cf.order.id.Value;
                return Json(new { ok = true, id = orderid, msg = "" });
            }
            else
            {
                var cf = new ctorderfactory(id);
                ctorder order = cf.order;
                order.receiveuid = cityId;
                order.changid = changId;
                order.goods.Clear();
                string[] ss = data.Split('|');
                for (int i = 0; i < ss.Length; i++)
                {
                    ctorder_goods cg = new ctorder_goods();
                    cg.gid = int.Parse(ss[i].Split(',')[0]);
                    cg.gnum = int.Parse(ss[i].Split(',')[1]);
                    if (ss[i].Split(',')[1] == "" || ss[i].Split(',')[0] == "")
                    {
                        return Json(new { ok = false, msg = "数据填写有误！" });
                    }
                    cg.id = ss[i].Split(',')[2] != "" ? int.Parse(ss[i].Split(',')[2].ToString()) : (int?)null;
                    order.goods.Add(cg.id.HasValue ? cg.id.Value : -cg.gid.Value, cg);
                    if (gids.Contains(int.Parse(ss[i].Split(',')[0])))
                    {
                        return Json(new { ok = false, msg = "不能存在两笔相同的货物！" });
                    }
                    gids.Add(int.Parse(ss[i].Split(',')[0]));
                }
                cf.update();
                return Json(new { ok = true, id = id, msg = "" });
            }
        }
        //提交
        [Breadcrumb(Auth = "1")]
        public ActionResult Submit(int orderid)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            var cf = new ctorderfactory(orderid);
            if (type != 1 || cf.order.createuid.Value != user.id)
            {
                return Redirect("/trans/byid/" + orderid + "?msg=您没有相应的操作权限！");
            }
            var files = Request.Files;
            bool fop = true;
            if (files.Count == 0 || files["file"].ContentLength == 0)
            {
                return Redirect("/trans/byid/" + orderid + "?msg=必须上传附件！");
                //fop = false;
            }
            List<ctattachment> cattachments = new List<ctattachment>();
            string temp = "";
            if (fop) {
                temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
                string target = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "upload\\t" + orderid);
                long ticks = DateTime.Now.Ticks;
                if (!Directory.Exists(target + "\\" + ticks))
                {
                    Directory.CreateDirectory(target + "\\" + ticks);
                }
                if (!Directory.Exists(temp))
                {
                    Directory.CreateDirectory(temp);
                }
                for (int i = 0; i < files.Count; i++)
                {
                    string filename = files[i].FileName.Substring(files[i].FileName.LastIndexOf('\\') + 1);
                    files[i].SaveAs(temp + "\\" + filename);
                    cattachments.Add(new ctattachment { name = filename, url = "/upload/t" + orderid + "/" + ticks + "/" + filename });
                }
            }
            try
            {
                cf.submit(user.id.Value, cattachments);
            }
            catch(Exception ex)
            {
                return Redirect("/trans/byid/" + orderid + "?msg="+ex.Message);
            }
            if (fop)
            {
                foreach (var cattachment in cattachments)
                {
                    System.IO.File.Move(temp + "\\" + cattachment.name, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + cattachment.url);
                }
            }

            return Redirect("/trans/byid/" + orderid + "?msg=提交成功！");
        }

        public ActionResult SgsVerify(bool status, string opinion, int id)
        {
            if (!status && opinion == "")
            {
                return Json(new { ok = false, msg = "必须填写审批意见或备注！" });
            }
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            if (type != 0)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            ctorderfactory cf = new ctorderfactory(id);
            try
            {
                cf.sgsvarify(user.id.Value, status, opinion);
            } catch (Exception ex) {
                return Json(new { ok = false, msg = ex.Message });
            }
            return Json(new { ok = true, msg = "" });
        }


        public ActionResult CjVerify(string submit, string opinion, int orderid)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            ctorderfactory cf = new ctorderfactory(orderid);
            if (type != 3 || cf.order.changid != user.id.Value)
            {
                return Redirect("/trans/byid/" + orderid + "?msg=您没有相应的操作权限！");
            }
            bool status = true;
            var files = Request.Files;
            bool fop = true;
            string temp = "";
            if (files.Count == 0 || files["file"].ContentLength == 0)
            {
                fop = false;
            }
            List<ctattachment> cattachments = new List<ctattachment>();
            if (fop)
            {
                temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
                string target = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "upload\\t" + orderid);
                long ticks = DateTime.Now.Ticks;
                if (!Directory.Exists(target + "\\" + ticks))
                {
                    Directory.CreateDirectory(target + "\\" + ticks);
                }
                if (!Directory.Exists(temp))
                {
                    Directory.CreateDirectory(temp);
                }
                for (int i = 0; i < files.Count; i++)
                {
                    string filename = files[i].FileName.Substring(files[i].FileName.LastIndexOf('\\') + 1);
                    files[i].SaveAs(temp + "\\" + filename);
                    cattachments.Add(new ctattachment { name = filename, url = "/upload/t" + orderid + "/" + ticks + "/" + filename });
                }
            }
            try
            {
                cf.cjconfirm(user.id.Value, status, opinion, cattachments);
            }catch(Exception ex)
            {
                return Redirect("/trans/byid/?msg=" + ex.Message);
            }
            if (fop)
            {
                foreach (var cattachment in cattachments)
                {
                    System.IO.File.Move(temp + "\\" + cattachment.name, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + cattachment.url);
                }
            }
            return Redirect("/trans/byid/" + orderid);
        }

        public ActionResult Dssend(bool submit,int orderid, string opinion, int[] gid, int[] gnum, int[] gmax)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            ctorderfactory cf = new ctorderfactory(orderid);
            if (type != 1 || cf.order.receiveuid.Value != user.id.Value)
            {
                return Redirect("/trans/byid/" + orderid + "?msg=您没有相应的操作权限！");
            }
            if(!submit && opinion == "")
            {
                return Redirect("/trans/byid/" + orderid + "?msg=不同意需填写备注！");
            }
            var files = Request.Files;
            bool fop = true;
            string temp = "";
            if (files.Count == 0 || files["file"].ContentLength == 0)
            {
                fop = false;
            }
            List<ctattachment> cattachments = new List<ctattachment>();
            if (fop)
            {
                temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
                string target = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "upload\\t" + orderid);
                long ticks = DateTime.Now.Ticks;
                if (!Directory.Exists(target + "\\" + ticks))
                {
                    Directory.CreateDirectory(target + "\\" + ticks);
                }
                if (!Directory.Exists(temp))
                {
                    Directory.CreateDirectory(temp);
                }
                for (int i = 0; i < files.Count; i++)
                {
                    string filename = files[i].FileName.Substring(files[i].FileName.LastIndexOf('\\') + 1);
                    files[i].SaveAs(temp + "\\" + filename);
                    cattachments.Add(new ctattachment { name = filename, url = "/upload/t" + orderid + "/" + ticks + "/" + filename });
                }
            }
            List<corder_sendgoods> li = new List<corder_sendgoods>();
            for (int i = 0; i < gid.Length; i++)
            {
                corder_sendgoods c1 = new corder_sendgoods();
                int id = gid[i];
                int num = gnum[i];
                int max = gmax[i];
                if (num > max)
                {
                    return Redirect("/trans/byid/" + orderid + "?msg=发货数量不得大于剩余发货数量！");
                }
                if (num == 0)
                {
                    continue;
                }
                c1.gid = id;
                c1.gnum = num;
                li.Add(c1);
            }
            try
            {
                cf.cjsend(user.id.Value, opinion, cattachments, user.wid.Value, submit);
            }catch(Exception ex)
            {
                return Redirect("/trans/byid/" + orderid + "?msg=" + ex.Message);
            }

            if (fop)
            {
                foreach (var cattachment in cattachments)
                {
                    System.IO.File.Move(temp + "\\" + cattachment.name, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + cattachment.url);
                }
            }
            return Redirect("/trans/byid/" + orderid);
        }

        public ActionResult Verifyds(string opinion, int id)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            ctorderfactory cf = new ctorderfactory(id);
            if (type != 1 || cf.order.createuid != user.id)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            var files = Request.Files;
            bool fop = true;
            string temp = "";
            if (files.Count == 0 || files["file"].ContentLength == 0)
            {
                return Redirect("/trans/byid/" + id + "?msg=必须上传附件！");
            }
            List<ctattachment> cattachments = new List<ctattachment>();
            if (fop)
            {
                temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
                string target = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "upload\\t" + id);
                long ticks = DateTime.Now.Ticks;
                if (!Directory.Exists(target + "\\" + ticks))
                {
                    Directory.CreateDirectory(target + "\\" + ticks);
                }
                if (!Directory.Exists(temp))
                {
                    Directory.CreateDirectory(temp);
                }
                for (int i = 0; i < files.Count; i++)
                {
                    string filename = files[i].FileName.Substring(files[i].FileName.LastIndexOf('\\') + 1);
                    files[i].SaveAs(temp + "\\" + filename);
                    cattachments.Add(new ctattachment { name = filename, url = "/upload/t" + id + "/" + ticks + "/" + filename });
                }
            }
            try
            {
                cf.dsreceive(user.id.Value, opinion, user.wid.Value, cattachments);
            }
            catch(Exception ex)
            {
                return Json(new { ok = false, msg = ex.Message });
            }
            if (fop)
            {
                foreach (var cattachment in cattachments)
                {
                    System.IO.File.Move(temp + "\\" + cattachment.name, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + cattachment.url);
                }
            }
            return Redirect("/trans/byid/" + id);
        }


        public ActionResult Discarded(int id, string status)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            ctorderfactory cf = new ctorderfactory(id);
            if (type != 0)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            if (status == "已作废")
            {
                return Json(new { ok = false, msg = "此订单已作废" });
            }
            cf.tovoid(user.id.Value);
            return Json(new { ok = true, msg = "" });
        }


        [Breadcrumb(Auth = "1")]
        public ActionResult Close(string id, string status)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            var cf = new ctorderfactory(int.Parse(id));

            if (type != 0 && type != 1 || cf.order.createuid.Value != user.id)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            try
            {
                cf.close(user.id.Value);
            }
            catch (Exception e)
            {
                return Json(new { ok = false, msg = e.Message });
            }

            return Json(new { ok = true, msg = "关闭成功" });
        }

        public ActionResult DeleteOrder(int orderid)
        {
            ctorderfactory cf = new ctorderfactory(orderid);
            cuser user = (cuser)Session["loginuser"];
            if (user.utype != 0)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限" });
            }
            if (cf.order.status != torderstatus.bevoid)
            {
                return Json(new { ok = false, msg = "只有已废除的订单可以删除" });
            }
            try
            {
                cf.delete();
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, msg = ex.Message });
            }
            return Json(new { ok = true, msg = "操作成功，已删除" });
        }

        public ActionResult GetInfoByid(int torderid) {
            string sql = "select cgs.name, cg.gnum from ctorder o join ctorder_goods cg on o.id = cg.oid join cgoods cgs on cg.gid = cgs.id where o.id = " + torderid;
            var result = DB.QueryAsDics(sql);
            if (result == null)
            {
                result = new Dictionary<string, object>[0];
            }
            return Json(new { ok = true, data = result, msg = "" });
        }
    }
}
