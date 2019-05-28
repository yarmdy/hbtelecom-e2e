using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace CTCCGoods.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /Order/

        public ActionResult Index()
        {
            var createuser = DB.QueryAsDics("select * from cuser where utype="+((int)crole.admin)+" or utype="+((int)crole.city)+" order by name");
            if (createuser == null) { 
                createuser=new Dictionary<string,object>[0];
            }
            var receiveuser = DB.QueryAsDics("select * from cuser where utype=" + ((int)crole.manufactor)+" order by name");
            if (receiveuser == null)
            {
                receiveuser = new Dictionary<string, object>[0];
            }
            var ostatus = new Dictionary<int, string>();
            foreach (orderstatus os in Enum.GetValues(typeof(orderstatus))) {
                ostatus[(int)os] = O2.GED(os);
            }
            ViewBag.cus = createuser;
            ViewBag.rus = receiveuser;
            ViewBag.oss = ostatus;
            return View();
        }
        public ActionResult Fo(int limit, int offset,string key,int cu,int ru,int os,int os2)
        {
            cuser u = (cuser)Session["loginuser"];
            var where = "where 1=1";
            if (!string.IsNullOrWhiteSpace(key)) {
                where += " and o.code like '%" + key + "%'";
            }
            if (cu > 0) {
                where += " and o.createuid="+cu;
            }
            if (u.utype.Value == (int)crole.city) {
                where += " and o.createuid=" + u.id;
            }
            if (u.utype.Value == (int)crole.design) {
                var wids = u.wids;
                wids.Add(-2);
                var idstr = string.Join(",",wids);
                where += " and cu.wid in (" + idstr + ")";
            }
            if (ru > 0)
            {
                where += " and o.receiveuid=" + ru;
            }
            if (u.utype.Value == (int)crole.manufactor) {
                where += " and o.receiveuid=" + u.id;
            }
            if (os != -10) {
                where += " and o.status=" + os;
            }
            if (os2 != -10)
            {
                where += " and o.status=" + (os2-1);
            }
            var rows = corderfactory.Select(where, limit, offset);
            var total = corderfactory.SelectTotal(where);
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Byid(int? id)
        {
            cuser u = (cuser)Session["loginuser"];
            var users = DB.QueryAsDics("select id, name from cuser where utype = 3");
            if(users == null)
            {
                users = new Dictionary<string, object>[0];
            }
            ViewBag.userbag = users;
            var goods = DB.QueryAsDics("select cg.id, cg.name, cg.cid, cc.name cname, cg.class2, cg.pid, cu.name pname from cgoods cg left join cclass cc on cg.cid = cc.id left join cuser cu on cg.pid = cu.id order by name");
            if (goods == null)
            {
                goods = new Dictionary<string, object>[0];
            }
            ViewBag.goodsbag = goods;
            
            
            //if(u.utype.Value == 0)
            //{
            //    cksql = "select id,name from cwarehouse";
            //}
            
            var dudao = DB.QueryAsDics("select id,name from cuser where utype = 4 and status = 1");
            if (dudao == null)
            {
                dudao = new Dictionary<string, object>[0];
            }
            ViewBag.dudao = dudao;
            //var chsum = DB.QueryAsDics("select s.gid,sum(s.gnum) as sum from (select gid,gnum from corder_sendgoods where ovid in (select id from corder_verifyflow where oid = "+id+")) s group by s.gid");
            if (id.HasValue)
            {
                try
                {
                    var cf = new corderfactory(id.Value);
                    ViewBag.order = cf.order;
                    var chsum = corderfactory.GetCanSendGoodsNum(id.Value);
                    ViewBag.chsum = chsum;
                    string cksql = "select id,name from cwarehouse where id =" + cf.order.wid;
                    var ck = DB.QueryAsDics(cksql);
                    if (ck == null)
                    {
                        ck = new Dictionary<string, object>[0];
                    }
                    ViewBag.ckbag = ck;
                }
                catch (Exception e)
                {
                    return Content(e.Message);
                }
            }
            else
            {
                ViewBag.order = corderfactory.CreateEmptyCorder();
            }
            return View();
        }

        public ActionResult QueryChang(int id)
        {
            string sql = "select id, name from cgoods where pid =" + id+" order by name";
            var cgoods = DB.QueryAsDics(sql);
            return Json(new { ok = true, cgoods = cgoods, msg = "" });
        }


        [Breadcrumb(Auth = "1")]
        public ActionResult Save(int changid, string s, int lid)
        {
            cuser user = (cuser)Session["loginuser"];
            if (!user.wid.HasValue || user.wid == -1) {
                return Json(new { ok = false, msg = "未找到您的所在地市，请联系管理员" });
            }
            int type = user.utype.Value;
            if (s == null || s == "")
            {
                return Json(new { ok = false, msg = "数据填写有误！" });
            }
            List<int> gids = new List<int>();
            if (lid == -1)
            {
                int createid = ((cuser)Session["loginuser"]).id.Value;
                List<corder_goods> cgl = new List<corder_goods>();
                string[] ss = s.Split('|');
                for (int i = 0; i < ss.Length; i++)
                {
                    corder_goods cg = new corder_goods();
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
                corder o = new corder() { createuid = createid, receiveuid = changid };
                corderfactory cf = new corderfactory(o, cgl);
                int id = cf.order.id.Value;
                return Json(new { ok = true, id = id, msg = "" });
            }
            else
            {
                var cf = new corderfactory(lid);
                if (user.id != cf.order.createuid && user.utype.Value != (int)crole.admin) {
                    return Json(new { ok = false, msg = "您没有权限！" });
                }
                corder order = cf.order;
                order.receiveuid = changid;
                order.goods.Clear();
                string[] ss = s.Split('|');
                for (int i = 0; i < ss.Length; i++)
                {
                    corder_goods cg = new corder_goods();
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
                return Json(new { ok = true, id = lid, msg = "" });
            }
        }

        [Breadcrumb(Auth = "1")]
        public ActionResult Submit(string orderid)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            var cf = new corderfactory(int.Parse(orderid));
            if (cf.order.createuid.Value != user.id && type != 0)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            var files = Request.Files;
            if (files["file"].ContentLength == 0)
            {
                return Redirect("/order/byid/" + orderid + "?msg=必须上传附件！");
            }
            List<cattachment> cattachments = new List<cattachment>();
            string temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
            string target = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "upload\\" + orderid);
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
                cattachments.Add(new cattachment { name = filename, url = "/upload/" + orderid + "/" + ticks + "/" + filename });
            }
            cf.submit(user.id.Value, cattachments);
            foreach (var cattachment in cattachments)
            {
                System.IO.File.Move(temp + "\\" + cattachment.name, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + cattachment.url);
            }
            
            return Redirect("/order/byid/" + orderid + "?msg=提交成功！");
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Close(string id, string status)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            var cf = new corderfactory(int.Parse(id));

            if (cf.order.createuid.Value != user.id && type != 0)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            try {
                cf.close(user.id.Value);
            } catch (Exception e) {
                return Json(new { ok = false, msg = e.Message });
            }
            
            return Json(new { ok = true, msg = "关闭成功" });
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Verify(bool status, string opinion, string id)
        {
            if(!status && opinion == "")
            {
                return Json(new { ok = false, msg = "必须填写审批意见或备注！" });
            }
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            corderfactory cf = new corderfactory(int.Parse(id));
            cf.sgsvarify(((cuser)Session["loginuser"]).id.Value, status, opinion);
            return Json(new { ok = true, msg = "" });
        }
        [Breadcrumb(Auth = "3")]
        public ActionResult Verifycjc(string submit, string opinion, string orderid)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            corderfactory cf = new corderfactory(int.Parse(orderid));
            if (cf.order.receiveuid.Value != user.id.Value && type != 0)
            {
                return Redirect("/order/byid/" + orderid + "?msg=您没有相应的操作权限！");
            }
            bool status = true;
            if (submit == "不同意")
            {
                if(opinion == "")
                {
                    return Redirect("/order/byid/" + orderid + "?msg=必须填写审批意见或备注！");
                }
                status = false;
            }
            var files = Request.Files;
            bool fop = true;
            string temp = "";
            if (files["file"].ContentLength == 0)
            {
                fop = false;
            }
            List<cattachment> cattachments = new List<cattachment>();
            if (fop)
            {
                temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
                string target = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "upload\\" + orderid);
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
                    cattachments.Add(new cattachment { name = filename, url = "/upload/" + orderid + "/" + ticks + "/" + filename });
                }
            }
            cf.cjconfirm(((cuser)Session["loginuser"]).id.Value, status, opinion, cattachments);
            if (fop)
            {
                foreach (var cattachment in cattachments)
                {
                    System.IO.File.Move(temp + "\\" + cattachment.name, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + cattachment.url);
                }
            }
            return Redirect("/order/byid/" + orderid);
        }
        [Breadcrumb(Auth = "2")]
        public ActionResult Verifysjy(bool status, string opinion, string id)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            corderfactory cf = new corderfactory(int.Parse(id));
            
            if (!user.wids.Contains(cf.order.wid.Value) && type != 0)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            if (!status && opinion == "")
            {
                return Json(new { ok = false, msg = "必须填写审批意见或备注！" });
            }
            cf.sjyvarify(((cuser)Session["loginuser"]).id.Value, status, opinion, cf.order.wid.Value);

            return Json(new { ok = true, msg = "" });
        }
        [Breadcrumb(Auth = "3")]
        public ActionResult Verifycj(string orderid, string opinion, int[] gid, int[] gnum, int[] gmax, string name, string time, string dudao)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            corderfactory cf = new corderfactory(int.Parse(orderid));
            if (cf.order.receiveuid.Value != user.id.Value && type != 0)
            {
                return Redirect("/order/byid/" + orderid + "?msg=您没有相应的操作权限！");
            }
            if(name == null || name.Trim() == "")
            {
                return Redirect("/order/byid/" + orderid + "?msg=货期名称不能为空！");
            }
            if (time == null || time.Trim() == "")
            {
                return Redirect("/order/byid/" + orderid + "?msg=计划到货时间不能为空！");
            }
            if (dudao == null || dudao.Trim() == "-1")
            {
                return Redirect("/order/byid/" + orderid + "?msg=督导人未指定！");
            }
            DateTime dt = DateTime.Parse(time);
            dt = DateTime.Parse(dt.ToShortDateString().ToString());
            int dudaoId = int.Parse(dudao);
            var files = Request.Files;
            bool fop = true;
            string temp = "";
            if (files["file"].ContentLength == 0)
            {
                fop = false;
            }
            List<cattachment> cattachments = new List<cattachment>();
            if (fop)
            {
                temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
                string target = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "upload\\" + orderid);
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
                    cattachments.Add(new cattachment { name = filename, url = "/upload/" + orderid + "/" + ticks + "/" + filename });
                }
            }
            List<corder_sendgoods> li = new List<corder_sendgoods>();
            for (int i = 0; i < gid.Length; i++)
            {
                corder_sendgoods c1 = new corder_sendgoods();
                int id = gid[i];
                int num = gnum[i];
                int max = gmax[i];
                if(num > max)
                {
                    return Redirect("/order/byid/" + orderid + "?msg=发货数量不得大于剩余发货数量！");
                }
                if (num == 0)
                {
                    continue;
                }
                c1.gid = id;
                c1.gnum = num;
                li.Add(c1);
            }
            cf.cjsend(((cuser)Session["loginuser"]).id.Value, opinion, li, cattachments,dudaoId, name, dt);

            if (fop)
            {
                foreach (var cattachment in cattachments)
                {
                    System.IO.File.Move(temp + "\\" + cattachment.name, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + cattachment.url);
                }
            }
            return Redirect("/order/byid/" + orderid);
        }
        [Breadcrumb(Auth = "4")]
        public ActionResult Verifycjr(string opinion, string id, int ovid, int duid)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            corderfactory cf = new corderfactory(int.Parse(id));
            
            if (cf.order.verifies[ovid].duid != user.id.Value && type != 0)
            {
                return Redirect("/order/byid/" + id + "?msg=您没有相应的操作权限！");
            }
            var files = Request.Files;
            if (files["file"].ContentLength == 0)
            {
                return Redirect("/order/byid/" + id + "?msg=必须上传附件！");
            }
            List<cattachment> cattachments = new List<cattachment>();
            string temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
            string target = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "upload\\" + id);
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
                cattachments.Add(new cattachment { name = filename, url = "/upload/" + id + "/" + ticks + "/" + filename });
            }
            cf.cjreceive(((cuser)Session["loginuser"]).id.Value, opinion, ovid, cattachments);
            foreach (var cattachment in cattachments)
            {
                System.IO.File.Move(temp + "\\" + cattachment.name, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + cattachment.url);
            }
            return Redirect("/order/byid/" + id);
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Verifyds(string opinion, string id, string ck, string ovid)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            corderfactory cf = new corderfactory(int.Parse(id));
            if (user.id != cf.order.createuid && type != 0)
            {
                return Json(new { ok = false, msg = "您没有相应的操作权限！" });
            }
            cf.dsreceive(((cuser)Session["loginuser"]).id.Value, opinion, cf.order.wid.Value, int.Parse(ovid));
            return Json(new { ok = true, msg = "" });
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Discarded(string id, string status)
        {
            cuser user = (cuser)Session["loginuser"];
            int type = user.utype.Value;
            corderfactory cf = new corderfactory(int.Parse(id));
            if (status == "已作废")
            {
                return Json(new { ok = false, msg = "此订单已作废" });
            }
            cf.tovoid(user.id.Value);
            return Json(new { ok = true, msg = "" });
        }

        [Breadcrumb(Auth = "0")]
        public ActionResult DeleteOrder(int orderid)
        {
            corderfactory cf = new corderfactory(orderid);
            cuser user = (cuser)Session["loginuser"];

            if(cf.order.status != orderstatus.bevoid)
            {
                return Json(new { ok = false, msg = "只有已废除的订单可以删除" });
            }
            try
            {
                cf.delete();
            }catch(Exception ex)
            {
                return Json(new { ok = false, msg = ex.Message });
            }
            return Json(new { ok = true, msg = "操作成功，已删除" });
        }

        public ActionResult GetInfoById(int orderid) {
            string sql = "select cgs.name, cg.gnum from corder o join corder_goods cg on o.id = cg.oid join cgoods cgs on cg.gid = cgs.id where o.id = " + orderid;
            var result = DB.QueryAsDics(sql);
            if (result == null) {
                result = new Dictionary<string, object>[0];
            }
            return Json(new { ok = true,data = result, msg = "" });
        }
    }
}
