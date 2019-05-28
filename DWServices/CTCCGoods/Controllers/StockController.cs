using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Text;

namespace CTCCGoods.Controllers
{
    public class StockController : Controller
    {
        //
        // GET: /Stock/
        public ActionResult Index()
        {
            return Redirect("/stock/cstock");
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Cwarehouse() {
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Fwh()
        {
            var js = DB.QueryAsDics("select * from cwarehouse");
            return Json(new { total=js.Length,data=js},JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Ewh(cwarehouse wh) {
            try {
                var wht = DB.Query("select * from cwarehouse where id='" + wh.id + "'");
                if (wht == null || wht.Rows.Count <= 0) {
                    return Json(new { ok = false, msg = "修改失败，修改的地市不存在" }, JsonRequestBehavior.AllowGet);
                }
                //wht = DB.Query("select * from cwarehouse where id<>'"+wh.id+"' and code='"+wh.code+"'");
                //if (wht != null && wht.Rows.Count> 0)
                //{
                //    return Json(new { ok = false, msg = "修改失败，仓库代码已存在" }, JsonRequestBehavior.AllowGet);
                //}
                wht = DB.Query("select * from cwarehouse where id<>'" + wh.id + "' and name='" + wh.name + "'");
                if (wht != null && wht.Rows.Count > 0)
                {
                    return Json(new { ok = false, msg = "修改失败，地市名称已存在" }, JsonRequestBehavior.AllowGet);
                }
                int count=DB.Exec("update cwarehouse set code='"+wh.code+"',name='"+wh.name+"' where id='"+wh.id+"'");
                if (count < 1) {
                    return Json(new { ok = false, msg = "修改失败。" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { ok = true, msg = "修改成功。" }, JsonRequestBehavior.AllowGet);
                
            }
            catch {
                return Json(new { ok=false,msg="修改失败。"},JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Awh(cwarehouse wh) {
            if (wh == null) {
                return Json(new { ok = false, msg = "新增失败，没有数据" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(wh.name)) {
                return Json(new { ok = false, msg = "新增失败，名称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            //var wha = DB.QueryOne("select * from cwarehouse where code='"+wh.code+"'");
            //if (wha != null) {
            //    return Json(new { ok = false, msg = "新增失败，已存在代码："+wh.code }, JsonRequestBehavior.AllowGet);
            //}
            var wha = DB.QueryOne("select * from cwarehouse where name='" + wh.name + "'");
            if (wha != null)
            {
                return Json(new { ok = false, msg = "新增失败，已存在名称：" + wh.name }, JsonRequestBehavior.AllowGet);
            }
            int identity = DB.Insert("insert into cwarehouse values('"+wh.code+"','"+wh.name+"')");
            if (identity < 1)
            {
                return Json(new { ok = false, msg = "新增失败。" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ok = true, msg = "新增成功。", id = identity }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "13")]
        public ActionResult Cstock() {
            var vwarehouse = DB.QueryAsDics("select * from cwarehouse");
            if (vwarehouse == null) {
                vwarehouse = new Dictionary<string, object>[0];
            }
            var vclass = DB.QueryAsDics("select * from cclass");
            if (vclass == null) {
                vclass = new Dictionary<string, object>[0];
            }
            var vgoods = DB.QueryAsDics("select * from cgoods");
            if (vgoods == null) {
                vgoods = new Dictionary<string, object>[0];
            }
            var changjia = DB.QueryAsDics("select id, name from cuser where utype = 3 and status = 1");
            if (changjia == null)
            {
                changjia = new Dictionary<string, object>[0];
            }
            var class2s = DB.QueryAsDics("select distinct class2 from cgoods ");
            if (class2s == null)
            {
                class2s = new Dictionary<string, object>[0];
            }
            ViewBag.vwarehouse = vwarehouse;
            ViewBag.vclass = vclass;
            ViewBag.vgoods = vgoods;
            ViewBag.changjia = changjia;
            ViewBag.class2s = class2s;
            return View();
        }
        [Breadcrumb(Auth = "13")]
        public ActionResult Fs(int searchtype, int searchwh, int searchc, string searchc2, int searchg, int changjia, string date)
        {
            cuser user = (cuser)Session["loginuser"];
            if (user.utype == (int?)crole.city) {
                searchwh = user.wid.Value;
            }
            if (user.utype == (int)crole.manufactor) {
                changjia = user.id.Value;
            }
            Dictionary<string, object>[] rows = null;
            var sql = "";
            if(date == "")
            {
                date = DateTime.Now.AddDays(1).ToString();
            }else
            {
                date = DateTime.Parse(date).AddDays(1).ToString();
            }
            switch (searchtype) { 
                case 1:
                case 2:
                    sql = string.Format(@"select null id,max(w.id) wid,max(w.code) wcode,MAX(w.name) wname,max(g.pname) pname,g.cid{7},max(c.code) ccode,max(c.name) cname,{5} gid,max(g.code) gcode,max(g.name) gname,sum(s.stock)stock,sum(s.purchased)purchased,sum(s.require)require from (
       select
         wid,
         gid,
         sum(ionumber)  stock,
         sum(purchased) purchased,
         sum(require) require
       from cstockio
       where createtime <= '{0}'
       group by wid, gid having sum(require) <> 0 or sum(ionumber) <> 0 or sum(purchased) <> 0
     ) s
left join (select
               t.*,
               a.name as pname
             from cgoods t
               join cuser a on t.pid = a.id) g on s.gid=g.id
left join cclass c on g.cid=c.id
left join cwarehouse w on s.wid=w.id
where s.wid={1} and g.cid={2} and s.gid={3} and g.pid={6} {8}
group by g.cid{4} order by g.cid,gid
", date, searchwh == -1 ? "s.wid" : searchwh + "", searchc == -1 ? "g.cid" : searchc + "", searchg == -1 ? "s.gid" : searchg + "", searchtype == 1 ? "" : ",g.id,g.class2", searchtype == 1 ? "max(g.id)" : "g.id", changjia == -1 ? "g.pid" : changjia + "", searchtype == 1 ? "" : ",g.class2", searchc2 == "-1" ? "and g.class2 = g.class2" : "and g.class2 = '" + searchc2 + "'");
                    break;
                case 3:
                    sql = string.Format(@"select s.wid, w.code wcode, w.name wname,g.class2, g.cid, g.pname, c.code ccode, c.name cname, s.gid, g.code gcode, g.name gname, s.stock, s.purchased,s.require
                                        from (
                                               select
                                                 wid,
                                                 gid,
                                                 sum(ionumber)  stock,
                                                 sum(purchased) purchased,
                                                 sum(require) require
                                               from cstockio
                                               where createtime <= '{0}'
                                               group by wid, gid having sum(require) <> 0 or sum(ionumber) <> 0 or sum(purchased) <> 0
                                             ) s
                                          left join (select
                                                       t.*,
                                                       a.name as pname
                                                     from cgoods t
                                                       join cuser a on t.pid = a.id) g on s.gid = g.id
                                          left join cclass c on g.cid = c.id
                                          left join cwarehouse w on s.wid = w.id
                                        where s.wid ={1} and g.cid={2} and s.gid={3} and g.pid={4} and g.class2 = {5}
                                        order by s.wid, g.cid, s.gid", date,
                            searchwh == -1 ? "s.wid" : searchwh + "", searchc == -1 ? "g.cid" : searchc + "", 
                            searchg == -1 ? "s.gid" : searchg + "", changjia == -1 ? "g.pid" : changjia + "", searchc2 == "-1" ? "g.class2" : "'" +searchc2 + "'");
                    break;
            }
            rows = DB.QueryAsDics(sql);
            return Json(new {total=rows==null?0:rows.Length,data=rows==null?null:rows });
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Cstockio() {
            var whinfo = DB.QueryAsDics("select * from cwarehouse");
            var gdinfo = DB.QueryAsDics("select * from cgoods");
            ViewBag.whinfo = whinfo == null ? new Dictionary<string, object>[0] : whinfo;
            ViewBag.gdinfo = gdinfo == null ? new Dictionary<string, object>[0] : gdinfo;
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Fsio(int limit, int offset)
        {
            var sql = string.Format(@"select top {0} * from (
select count(0)over(partition by 1) total,row_number()over(order by sio.id desc) top1,sio.id,sio.wid,w.code wcode,w.name wname,sio.gid,g.code gcode,g.name gname,sio.ionumber,sio.purchased,sio.require,sio.createtime,sio.uid,u.code ucode,u.name uname,sio.stype,sio.oid,isnull(o.code,t.code) ocode from cstockio sio
left join cwarehouse w on sio.wid=w.id
left join cgoods g on sio.gid=g.id
left join cuser u on sio.uid=u.id
left join corder o on sio.oid=o.id and sio.stype = 0
left join ctorder t on sio.oid = t.id and sio.stype = 2

)a where top1>{1}", limit,offset);
            var rows = DB.QueryAsDics(sql);
            var total=rows==null||rows.Length<=0?0:O2.O2I(rows[0]["total"]);
            if (rows != null) {
                foreach (var row in rows) {
                    row["createtime"] = ((DateTime)row["createtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return Json(new { total=total,rows=rows});
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Asio(int siotype, int wid, int gid, int ionumber, int pur)
        {
            var res = false;
            var msg = "未知错误";
            var user = (cuser)Session["loginuser"];
            if (ionumber == 0 && pur == 0) {
                msg = "没有任何调整，调整失败";
                return Json(new { ok = res, msg = msg });
            }
            var wh = DB.QueryOne("select * from cwarehouse where id="+wid);
            if (wh == null) {
                msg = "未找到选择的仓库";
                return Json(new { ok = res, msg = msg });
            }
            var gd = DB.QueryOne("select * from cgoods where id="+gid);
            if (gd == null)
            {
                msg = "未找到选择的产品";
                return Json(new { ok = res, msg = msg });
            }
            var stock = DB.QueryOne("select * from cstock where wid="+wid+" and gid="+gid);
            var hasstock = stock != null;
            var ion = hasstock ? O2.O2I(stock["stock"]) : 0;
            var purn = hasstock ? O2.O2I(stock["purchased"]) : 0;
            var req = hasstock ? O2.O2I(stock["require"]) : 0;
            ion = ion < req ? ion : req;
            if (ion + ionumber < 0) {
                msg = "仓库现有数量："+ion+"，入库数量："+ionumber+"，无法入库";
                return Json(new { ok = res, msg = msg });
            }
            if (ion + ionumber - purn < pur) {
                msg = "仓库可购买数量：" + (ion + ionumber - purn) + "，购买数量：" + pur + "，无法购买";
                return Json(new { ok = res, msg = msg });
            }
            var ctime = DateTime.Now;
            DB db = new DB();
            try {
                if (hasstock)
                {
                    db.Execobj("update cstock set stock=stock+" + ionumber + ",require=require+"+ionumber+",purchased=purchased+" + pur + " where id=" + stock["id"]);
                }
                else {
                    db.Execobj("insert into cstock(wid,gid,stock,purchased,require)values(" + wid + "," + gid + "," + ionumber + "," + pur + ","+ionumber+")");
                }
                db.Execobj("insert into cstockio(wid,gid,ionumber,purchased,createtime,uid,stype,oid,require)values(" + wid + "," + gid + "," + ionumber + "," + pur + ",'" + ctime.ToString("yyyy-MM-dd HH:mm:ss") + "'," + user.id + "," + siotype + ",null,"+ionumber+")");
                db.End(true);
            }
            catch (Exception ex) {
                db.End(false);
                msg = "数据库写入失败！";
            }
            res = true;
            msg = "调整成功";
            return Json(new {ok=res,msg=msg });
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult UpLoad()
        {
            try {
                cuser user = (cuser)Session["loginuser"];
                var files = Request.Files;
                if (files["file"].ContentLength == 0)
                {
                    return Redirect("/stock/cstockio/" + "?msg=您没有选择文件");
                }
                string temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp\\" + user.id);
                if (!Directory.Exists(temp))
                {
                    Directory.CreateDirectory(temp);
                }
                for (int i = 0; i < files.Count; i++)
                {
                    string filename = files[i].FileName.Substring(files[i].FileName.LastIndexOf('\\') + 1);
                    temp += "\\" + filename;
                    files[i].SaveAs(temp);
                }
                DataTable dt = DB.GetXlsxData(temp);
                var dtarr = dt.AsEnumerable().ToList();
                Dictionary<string, Dictionary<string, object>> dics = new Dictionary<string, Dictionary<string, object>>();
                for(int i=0;i<dtarr.Count;i++)
                {
                    if(dics.ContainsKey(dtarr[i][0]+"_"+ dtarr[i][1]))
                    {
                        ((List<int>)dics[dtarr[i][0] + "_" + dtarr[i][1]]["list"]).Add(i+2);
                    }
                    else
                    {
                        dics[dtarr[i][0] + "_" + dtarr[i][1]] = new Dictionary<string, object>();
                        dics[dtarr[i][0] + "_" + dtarr[i][1]]["list"] = new List<int>();
                        ((List<int>)dics[dtarr[i][0] + "_" + dtarr[i][1]]["list"]).Add(i+2);
                        dics[dtarr[i][0] + "_" + dtarr[i][1]]["city"] = dtarr[i][0];
                        dics[dtarr[i][0] + "_" + dtarr[i][1]]["name"] = dtarr[i][1];
                    }
                }
                string errmsg = "";
                foreach (var dic in dics)
                {
                    if(((List<int>)dic.Value["list"]).Count>1)
                    {
                        errmsg += dic.Key + ":";
                        foreach (int li in (List<int>)dic.Value["list"]) {
                            errmsg += li+",";
                        }
                        errmsg += "行重复<br/>";
                    }
                }
                if(errmsg != "")
                {
                    return Content(errmsg);
                }
                var houses = DB.Query("select name from cwarehouse");
                var products = DB.Query("select name from cgoods");
                var nums = DB.Query("select c.name cname,g.name pname,iif(s.stock<s.require,s.stock,s.require)-s.purchased as num from cstock s left join cgoods g on s.gid = g.id left join cwarehouse c on s.wid = c.id");
                List<string> errors = new List<string>();
                bool isError = false;
                bool ishouse = true;
                bool ispro = true;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ispro = true;
                    if (houses.Select("name='" + dt.Rows[i][0] + "'").Length <= 0)
                    {
                        ishouse = false;
                        isError = true;
                        errors.Add("*第" + (i + 2) + "行，仓库不存在:[" + dt.Rows[i][0] + "]。");
                    }
                    if (products.Select("name='" + dt.Rows[i][1] + "'").Length <= 0)
                    {
                        ispro = false;
                        isError = true;
                        errors.Add("*第" + (i + 2) + "行，产品不存在:[" + dt.Rows[i][1] + "]。");
                    }
                    if (ispro && ishouse && int.Parse(dt.Rows[i][2].ToString()) > int.Parse(nums.Select("cname='" + dt.Rows[i][0] + "' and pname = '" + dt.Rows[i][1] + "'")[0][2].ToString()))
                    {
                        isError = true;
                        errors.Add("*第" + (i + 2) + "行，数量不正确:[" + dt.Rows[i][2] + "]。");
                    }
                }
                if (isError)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < errors.Count; i++)
                    {
                        sb.Append(errors[i].ToString()).Append("<br/>");
                    }
                    return Content(sb.ToString());
                }
                string time = DateTime.Now.ToString();
                DB db = new DB();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string sql = "update cstock set purchased = purchased + " + dt.Rows[i][2] + " where wid = (select id from cwarehouse where name = '" + dt.Rows[i][0] + "') and gid = (select id from cgoods where name = '" + dt.Rows[i][1] + "')";
                    db.Execobj(sql);
                    sql = @"insert into cstockio (wid,gid,ionumber,purchased,createtime,uid,stype) values 
                            ((select id from cwarehouse where name = '" + dt.Rows[i][0] + "')," +
                                "(select id from cgoods where name = '" + dt.Rows[i][1] + "'),0," + dt.Rows[i][2] + ",'" +
                                time + "', " + user.id.Value + ",1)";
                    db.Execobj(sql);
                }
                db.End(true);
                return Redirect("/stock/cstockio/" + "?msg=导入成功！");
            }catch(Exception e)
            {
                return Content("您上传的文件发生错误！");
            }
        }

        public ActionResult GetDate()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return Json(new { ok=true, date=date, msg=""}, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Search(int limit, int offset, string start, string end)
        {
            var sql = string.Format(@"select top {0} * from (
select count(0)over(partition by 1) total,row_number()over(order by sio.id desc) top1,sio.id,sio.wid,w.code wcode,w.name wname,sio.gid,g.code gcode,g.name gname,sio.ionumber,sio.purchased,sio.require,sio.createtime,sio.uid,u.code ucode,u.name uname,sio.stype,sio.oid,isnull(o.code,t.code) ocode from cstockio sio
left join cwarehouse w on sio.wid=w.id
left join cgoods g on sio.gid=g.id
left join cuser u on sio.uid=u.id
left join corder o on sio.oid=o.id and sio.stype = 0
left join ctorder t on sio.oid = t.id and sio.stype = 2 )a where top1 > {1}", limit, offset);
            DateTime s = DateTime.Now;
            if (!string.IsNullOrEmpty(start))
            {
                s = DateTime.Parse(start);
                sql += " and a.createtime >= '" + s.ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrEmpty(end))
            {
                s = DateTime.Parse(end);
                sql += " and a.createtime <= '" + s.AddDays(1).ToString("yyyy-MM-dd") + "'";
            }
            var rows = DB.QueryAsDics(sql);
            var total = rows == null || rows.Length <= 0 ? 0 : O2.O2I(rows[0]["total"]);
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    row["createtime"] = ((DateTime)row["createtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return Json(new {ok=true, total = total, rows = rows, msg="" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Export(string start, string end)
        {
            var sql = string.Format(@"select w.name wname,g.name gname,sio.ionumber,sio.purchased,sio.require,sio.createtime,u.name uname,sio.stype,isnull(o.code,t.code) ocode from cstockio sio
left join cwarehouse w on sio.wid=w.id
left join cgoods g on sio.gid=g.id
left join cuser u on sio.uid=u.id
left join corder o on sio.oid=o.id and sio.stype = 0
left join ctorder t on sio.oid = t.id and sio.stype = 2
where 1=1 ");
            DateTime s = DateTime.Now;
            if(!string.IsNullOrEmpty(start))
            {
                s = DateTime.Parse(start);
                sql += " and sio.createtime >= '" + s.ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrEmpty(end))
            {
                s = DateTime.Parse(end);
                sql += " and sio.createtime <= '" + s.AddDays(1).ToString("yyyy-MM-dd") + "'";
            }
            var rows = DB.QueryAsDics(sql);
            if (rows != null)
            {
                StringBuilder sb = new StringBuilder("地市,型号,总需求数,出入库数量,购买数量,出入库时间,操作人,类型,单号\r\n");
                foreach (var row in rows)
                {
                    string stype = "";
                    if(O2.O2I(row["stype"])==-1)
                    {
                        stype = "期初录入";
                    }else if(O2.O2I(row["stype"]) == 0)
                    {
                        stype = "需求单";
                    }
                    else if (O2.O2I(row["stype"]) == 2)
                    {
                        stype = "调货单";
                    }else
                    {
                        stype = "管理员调整";
                    }
                    sb.Append(row["wname"]).Append(",").Append(row["gname"]).Append(",").Append(row["require"]).Append(",").Append(row["ionumber"]).Append(",").Append(row["purchased"]).Append(",")
                        .Append(row["createtime"]).Append(",").Append(row["uname"]).Append(",").Append(stype).Append(",").Append("["+row["ocode"]+"]").Append("\r\n");
                }
                var data=Encoding.GetEncoding("gbk").GetBytes(sb.ToString());
                return File(data, "application/octet-stream", "库存明细.csv");
                
            }
            else
            {
                return Json(new { ok = false, msg = "未搜索到符合条件的信息" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInfoById(string code, int table)
        {
            if(table == 0)
            {
                string sql = "select cgs.name, cg.gnum from corder o join corder_goods cg on o.id = cg.oid join cgoods cgs on cg.gid = cgs.id where o.code = '" + code + "'";
                var result = DB.QueryAsDics(sql);
                if (result == null)
                {
                    result = new Dictionary<string, object>[0];
                }
                return Json(new { ok = true, data = result, msg = "" });
            }
            else
            {
                string sql = "select cgs.name, cg.gnum from ctorder o join ctorder_goods cg on o.id = cg.oid join cgoods cgs on cg.gid = cgs.id where o.code = '" + code +"'";
                var result = DB.QueryAsDics(sql);
                if (result == null)
                {
                    result = new Dictionary<string, object>[0];
                }
                return Json(new { ok = true, data = result, msg = "" });
            }
            return null;
        }
    }
}
