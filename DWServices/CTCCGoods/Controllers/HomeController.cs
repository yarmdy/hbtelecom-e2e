using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTCCGoods.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var createuser = DB.QueryAsDics("select * from cuser where utype=" + ((int)crole.admin) + " or utype=" + ((int)crole.city) + " order by name");
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
            foreach (orderstatus os in Enum.GetValues(typeof(orderstatus)))
            {
                ostatus[(int)os] = O2.GED(os);
            }
            var tostatus = new Dictionary<int, string>();
            foreach (torderstatus os in Enum.GetValues(typeof(torderstatus)))
            {
                tostatus[(int)os] = O2.GED(os);
            }
            ViewBag.cus = createuser;
            ViewBag.rus = receiveuser;
            ViewBag.oss = ostatus;
            ViewBag.tos = tostatus;
            return View();
        }
        public ActionResult Ftask() {
            cuser user = (cuser)Session["loginuser"];
            var sql = @"select type='需求单',o.id,o.code,u1.name createuname,u2.name receiveuname,isnull(o.ovtime2,isnull(o.ovtime,o.createtime))createtime,isnull(u4.name,isnull(u3.name,u1.name))uname,ISNULL(o.verifyno2,isnull(o.verifyno,o.status))verifyno,isnull(o.agree2,o.agree)agree,isnull(o.des,o.des2)des,case when agree=1 then ISNULL(o.verifyno2,isnull(o.verifyno,o.status)) else o.status end status from(
select ROW_NUMBER()over(partition by o.id order by ov.id desc,isnull(ov2.id,0) desc)top1,ROW_NUMBER()over(partition by ov.id order by ov2.id desc)top2,o.id,o.code,o.status,o.createuid,o.receiveuid,o.createtime,isnull(o.sendall,0) sendall,ov.id ovid,ov.uid,ov.verifyno,ov.createtime ovtime,ov.status agree,ov.des,ov.duid,ov.localstatus,ov2.id ovid2,ov2.uid uid2,ov2.verifyno verifyno2,ov2.createtime ovtime2,ov2.status agree2,ov2.des des2 from corder o
left join corder_verifyflow ov on o.id=ov.oid and ov.verifyno<6
left join corder_verifyflow ov2 on ov.id=ov2.ovid and ov.verifyno<6 and ov2.verifyno>5 --or ov.id=ov2.id and ov.verifyno=5
)o
left join cuser u1 on o.createuid=u1.id
left join cuser u2 on o.receiveuid=u2.id
left join cuser u3 on o.uid=u3.id
left join cuser u4 on o.uid2=u4.id
where o.status>=0 and o.status<8 and ISNULL(o.verifyno2,0)<7 and (o.top1=1 or o.verifyno=5 and o.top2=1 or o.sendall=0 and o.verifyno=4 and o.agree=1) {0}
order by createtime desc";
            var where = "";
            switch ((crole)user.utype.Value) { 
                case crole.admin:
                    where = " and (o.status=" + (int)orderstatus.submit + " or o.createuid=" + user.id + " and (o.status=" + (int)orderstatus.create + " or ISNULL(o.verifyno2,0)=" + (int)orderstatus.cjreceive + "))";
                    break;
                case crole.city:
                    where = " and (o.createuid=" + user.id + " and (o.status=" + (int)orderstatus.create + " or ISNULL(o.verifyno2,0)=" + (int)orderstatus.cjreceive + "))";
                    break;
                case crole.design:
                    var wids = string.Join(",", user.wids);
                    where = " and (o.status=" + (int)orderstatus.cjconfirm + " and u1.wid in (" + wids + "))";
                    break;
                case crole.manufactor:
                    where = " and (o.receiveuid=" + user.id + " and (o.status=" + (int)orderstatus.sjyverify + " or o.status="+(int)orderstatus.sgsverify+" or o.verifyno=" + (int)orderstatus.sjyverify + "))";
                    break;
                case crole.supervisor:
                    where = " and (ISNULL(o.duid,0)=" + user.id + " and (ISNULL(o.localstatus,0)=" + (int)orderstatus.cjsend + "))";
                    break;
            }
            sql=string.Format(sql,where);
            var rows = DB.QueryAsDics(sql);
            
            var sql2 = @"select
  type='调货单',
  o.id,
  o.code,
  u1.name                                           createuname,
  u2.name                                           receiveuname,
  u4.name                                           changname,
  isnull(u3.name, u1.name)   uname,
  isnull(o.ovtime, o.createtime) createtime,
  isnull(o.verifyno, o.status) verifyno,
  o.des,
  o.agree,
  case when agree = 1
    then isnull(o.verifyno, o.status)
  else o.status end                                 status
from (
       select
         ROW_NUMBER()
         over ( partition by o.id
           order by ov.id desc) top1,
         o.id,
         o.code,
          o.status,
        o.sendall,
         o.createuid,
         o.receiveuid,
         o.createtime,
         o.changid,
         ov.id                                           ovid,
         ov.uid,
         ov.verifyno,
         ov.createtime                                   ovtime,
         ov.status                                       agree,
         ov.des
       from ctorder o
         left join ctorder_verifyflow ov on o.id = ov.oid and ov.verifyno < 5
           --or ov.id=ov2.id and ov.verifyno=5
     ) o
  left join cuser u1 on o.createuid = u1.id
  left join cuser u2 on o.receiveuid = u2.id
  left join cuser u3 on o.uid = u3.id
  left join cuser u4 on o.changid = u4.id
where o.status >= 0 and o.status < 6 and (o.top1 = 1 or o.verifyno = 5) {0}
order by createtime desc";
            var where2 = "";
            switch ((crole)user.utype.Value)
            {
                case crole.admin:
                    where2 = " and (o.status=" + (int)torderstatus.submit + " or o.createuid=" + user.id + " and (o.status=" + (int)torderstatus.create + "))";
                    break;
                case crole.city:
                    where2 = " and ((o.createuid = "+user.id+" and (o.status = "+ (int)torderstatus.create + " or o.status = "+ (int)torderstatus.cjsend+ ")) or(o.receiveuid = "+user.id+ " and o.status = " + (int)torderstatus.cjconfirm + ") and o.sendall is null)";
                    break;
                case crole.manufactor:
                    where2 = " and (o.changid=" + user.id + " and o.verifyno=" + (int)torderstatus.sgsverify + "  and o.agree= 1)";
                    break;
                default:
                    where2 = " 1 = 2";
                    break;
            }
            sql2 = string.Format(sql2, where2);
            var rows2 = DB.QueryAsDics(sql2);
            if (rows != null && rows2 != null)
            {
                rows = rows.Concat(rows2).ToArray();
            }
            else if(rows2!=null)
            {
                rows = rows2;
            }
            var total = rows == null ? 0 : rows.Length;
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    row["createtime"] = ((DateTime)row["createtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            AddRoomInOrOut(rows);
            return Json(new { total=total,data=rows});
        }
        public ActionResult Fflow()
        {
            cuser user = (cuser)Session["loginuser"];
            var sql = @"select type='需求单',o.id,o.code,u1.name createuname,u2.name receiveuname,isnull(o.ovtime2,isnull(o.ovtime,o.createtime))createtime,isnull(u4.name,isnull(u3.name,u1.name))uname,ISNULL(o.verifyno2,isnull(o.verifyno,o.status))verifyno,isnull(o.agree2,o.agree)agree,isnull(o.des,o.des2)des,case when agree=1 then ISNULL(o.verifyno2,isnull(o.verifyno,o.status)) else o.status end status from(
select ROW_NUMBER()over(partition by o.id order by ov.id desc,isnull(ov2.id,0) desc)top1,ROW_NUMBER()over(partition by ov.id order by ov2.id desc)top2,o.id,o.code,o.status,o.createuid,o.receiveuid,o.createtime,isnull(o.sendall,0) sendall,ov.id ovid,ov.uid,ov.verifyno,ov.createtime ovtime,ov.status agree,ov.des,ov.duid,ov.localstatus,ov2.id ovid2,ov2.uid uid2,ov2.verifyno verifyno2,ov2.createtime ovtime2,ov2.status agree2,ov2.des des2 from corder o
left join corder_verifyflow ov on o.id=ov.oid and ov.verifyno<6
left join corder_verifyflow ov2 on ov.id=ov2.ovid and ov.verifyno<6 and ov2.verifyno>5 --or ov.id=ov2.id and ov.verifyno=5
)o
left join cuser u1 on o.createuid=u1.id
left join cuser u2 on o.receiveuid=u2.id
left join cuser u3 on o.uid=u3.id
left join cuser u4 on o.uid2=u4.id
where o.status>=0 and o.status<8 and (o.top1=1 or o.verifyno=5 and o.top2=1 or o.sendall=0 and o.verifyno=4 and o.agree=1) {0}
order by createtime desc";
            var where = "";
            switch ((crole)user.utype.Value)
            {
                case crole.admin:
                    where = " and not(o.status=" + (int)orderstatus.submit + " or o.createuid=" + user.id + " and (o.status=" + (int)orderstatus.create + " or ISNULL(o.verifyno2,0)=" + (int)orderstatus.cjreceive + "))";
                    break;
                case crole.city:
                    where = " and not(o.createuid=" + user.id + " and (o.status=" + (int)orderstatus.create + " or ISNULL(o.verifyno2,0)=" + (int)orderstatus.cjreceive + ")) and (o.createuid=" + user.id + ")";
                    break;
                case crole.design:
                    var wids = string.Join(",", user.wids);
                    where = " and not(o.status=" + (int)orderstatus.cjconfirm + ") and (o.status>" + (int)orderstatus.cjconfirm + " and u1.wid in (" + wids + "))";
                    break;
                case crole.manufactor:
                    where = " and not(o.receiveuid=" + user.id + " and (o.status=" + (int)orderstatus.sjyverify + " or o.status=" + (int)orderstatus.sgsverify + " or o.verifyno=" + (int)orderstatus.sjyverify + ")) and (o.receiveuid=" + user.id + ")";
                    break;
                case crole.supervisor:
                    where = " and not(ISNULL(o.duid,0)=" + user.id + " and (ISNULL(o.localstatus,0)=" + (int)orderstatus.cjsend + ")) and (o.duid=" + user.id + ")";
                    break;
            }
            sql = string.Format(sql, where);
            var rows = DB.QueryAsDics(sql);
            
            var sql2 = @"select
type='调货单',
  o.id,
  o.code,
  u1.name                        createuname,
  u2.name                        receiveuname,
  u4.name                        changname,
  isnull(o.ovtime, o.createtime) createtime,
  isnull(u3.name, u1.name)       uname,
  isnull(o.verifyno, o.status)   verifyno,
  o.agree,
  o.des,
  case when agree = 1
    then isnull(o.verifyno, o.status)
  else o.status end              status
from (
       select
         ROW_NUMBER()
         over ( partition by o.id
           order by ov.id desc ) top1,
         o.id,
         o.code,
         o.status,
         o.createuid,
         o.receiveuid,
         o.createtime,
         o.changid,
         o.sendall,
         ov.id                   ovid,
         ov.uid,
         ov.verifyno,
         ov.createtime           ovtime,
         ov.status               agree,
         ov.des
       from ctorder o
         left join ctorder_verifyflow ov on o.id = ov.oid and ov.verifyno < 5
         left join ctorder_verifyflow ov2
           on ov.id = ov2.ovid and ov.verifyno < 5 and ov2.verifyno > 4
     ) o
  left join cuser u1 on o.createuid = u1.id
  left join cuser u2 on o.receiveuid = u2.id
  left join cuser u3 on o.uid = u3.id
  left join cuser u4 on o.changid = u4.id
where o.status >= 0 and o.status < 6 and o.top1=1   {0}
order by createtime desc";
            var where2 = "";
            switch ((crole)user.utype.Value)
            {
                case crole.admin:
                    where2 = " and not(o.status=" + (int)torderstatus.submit + " or o.createuid=" + user.id + " and (o.status=" + (int)torderstatus.create +"))";
                    break;
                case crole.city:
                    where2 = " and not(o.createuid="+user.id+" and (o.status="+ (int)torderstatus.create + "  or o.status="+ (int)torderstatus.cjsend+ ")) and not(o.receiveuid = "+user.id+" and o.status = "+ (int)torderstatus.cjconfirm+ ") and (o.receiveuid="+user.id+" or o.createuid = "+user.id+")";
                    break;
                case crole.manufactor:
                    where2 = " and not(o.changid=" + user.id + " and (o.status=" + (int)torderstatus.sgsverify + ")) and (o.changid=" + user.id + ")";
                    break;
                default:
                    where2 = " 1=2";
                    break;
            }
            sql2 = string.Format(sql2, where2);
            var rows2 = DB.QueryAsDics(sql2);
            if (rows != null && rows2 != null)
            {
                rows = rows.Concat(rows2).ToArray();
            }
            else if (rows2 != null)
            {
                rows = rows2;
            }
            var total = rows == null ? 0 : rows.Length;
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    row["createtime"] = ((DateTime)row["createtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            AddRoomInOrOut(rows);
            return Json(new { total = total, data = rows });
        }

        public void AddRoomInOrOut(Dictionary<string, object>[] dic)
        {
            if (dic == null) return;
            foreach(Dictionary<string, object> d in dic)
            {
                string type = d["type"].ToString();
                string code = d["code"].ToString();

                string table = "corder";
                string goodsTable = "corder_goods";
                if (type == "调货单") {
                    table = "ctorder";
                    goodsTable = "ctorder_goods";
                }

                string sql = @"select g.class2 from 
                                cgoods g join (
                                select  top(1) b.gid from (
                                select gid from 
                                "+goodsTable+@"
                                g join
                                (select id from
                                "+ table + @" where code = '"+code+@"') a 
                                on g.oid = a.id) b) c
                                on g.id = c.gid";
                var r = DB.QueryAsDics(sql);
                if (r == null) {
                    continue;
                }
                d.Add("room", r[0]["class2"].ToString());
            }
        }
    }
}
