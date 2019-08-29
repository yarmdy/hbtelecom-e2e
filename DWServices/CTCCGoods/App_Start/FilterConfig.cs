using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace CTCCGoods
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MyAuthorizeAttribute());
            filters.Add(new BreadcrumbAttribute());
        }
    }

    public class BreadcrumbAttribute : ActionFilterAttribute
    {
        public string Auth { get; set; }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //base.OnResultExecuting(filterContext);
            if (filterContext.HttpContext.Request.IsAjaxRequest()) return;
            var controller = filterContext.RouteData.Values["controller"].ToString();
            var action = filterContext.RouteData.Values["action"].ToString();
            string ctl=null;
            string act=null;
            switch (controller) { 
                case "goods":
                    ctl = "类型管理";
                    break;
                case "order":
                    ctl = "需求单";
                    break;
                case "stock":
                    ctl = "仓储";
                    break;
                case "user":
                    ctl = "用户管理";
                    break;
                case "trans":
                    ctl = "调货单";
                    break;
                case "plan":
                    ctl = "地市上传任务";
                    break;
                case "rru": 
                    ctl="RRU序列号管理";
                    break;
                case "task":
                    ctl = "后台任务管理";
                    break;
                case "netcap":
                    ctl = "网络容量评估";
                    break;
                default:
                    ctl = null;
                    break;
            }
            switch (action)
            {
                case "cwarehouse":
                    act = "地市管理";
                    break;
                case "cstock":
                    act = "仓库存量";
                    break;
                case "cstockio":
                    act = "入库明细(购买)";
                    break;
                case "cclass":
                    act = "设备类型";
                    break;
                case "cgoods":
                    act = "设备型号";
                    break;
                case "templet":
                    act = "模板管理";
                    break;
                case "enums":
                    act = "枚举管理";
                    break;
                case "t":
                    act = "模板编辑器";
                    break;
                case "report":
                    act = "新发货表";
                    break;
                case "wang":
                    act = "网管详表(最新)";
                    break;
                case "wangtj":
                    act = "网管详表(累计)";
                    break;
                case "wangtongji":
                    act = "对比分析";
                    break;
                case "lterru":
                    act = "历史到货表";
                    break;
                case "krmen":
                    act = "超忙门限";
                    break;
                case "table1":
                    act = "原始数据";
                    break;
                case "sbusycomp":
                    act = "超忙计算";
                    break;
                case "superbusylist":
                    act = "超忙原始清单";
                    break;
                case "superbusyex":
                    act = "超忙对应扩容清单";
                    break;
                case "warning":
                    act = "任务警告处理";
                    break;
                case "completerate":
                    act = "数据完整率";
                    break;
                case "extj":
                    act = "扩容统计";
                    break;
                default:
                    act = null;
                    break;
            }
            filterContext.Controller.ViewBag.ctl = ctl;
            filterContext.Controller.ViewBag.act = act;
            
            var user=(Controllers.cuser)filterContext.HttpContext.Session["loginuser"];
            if (user != null)
            {
                filterContext.Controller.ViewBag.loginuser = user;
                var sql = @"select count(0)num from(
select ROW_NUMBER()over(partition by o.id order by ov.id desc,isnull(ov2.id,0) desc)top1,ROW_NUMBER()over(partition by ov.id order by ov2.id desc)top2,o.status,ov.verifyno,ov2.verifyno verifyno2,isnull(o.sendall,0)sendall,ov.status agree,o.createuid,o.receiveuid,ov.duid,ov.localstatus from corder o
left join corder_verifyflow ov on o.id=ov.oid and ov.verifyno<6
left join corder_verifyflow ov2 on ov.id=ov2.ovid and ov.verifyno<6 and ov2.verifyno>5 --or ov.id=ov2.id and ov.verifyno=5
)o
left join cuser u1 on o.createuid=u1.id
where o.status>=0 and o.status<8 and ISNULL(o.verifyno2,0)<7 and (o.top1=1 or o.verifyno=5 and o.top2=1 or o.sendall=0 and o.verifyno=4 and o.agree=1) {0}
";
                var where = "";
                switch ((Controllers.crole)user.utype.Value)
                {
                    case Controllers.crole.admin:
                        where = " and (o.status=" + (int)Controllers.orderstatus.submit + " or o.createuid=" + user.id + " and (o.status=" + (int)Controllers.orderstatus.create + " or ISNULL(o.verifyno2,0)=" + (int)Controllers.orderstatus.cjreceive + "))";
                        break;
                    case Controllers.crole.city:
                        where = " and (o.createuid=" + user.id + " and (o.status=" + (int)Controllers.orderstatus.create + " or ISNULL(o.verifyno2,0)=" + (int)Controllers.orderstatus.cjreceive + "))";
                        break;
                    case Controllers.crole.design:
                        var wids=string.Join(",",user.wids);
                        where = " and (o.status=" + (int)Controllers.orderstatus.cjconfirm + " and u1.wid in (" + wids + "))";
                        break;
                    case Controllers.crole.manufactor:
                        where = " and (o.receiveuid=" + user.id + " and (o.status=" + (int)Controllers.orderstatus.sjyverify + " or o.status=" + (int)Controllers.orderstatus.sgsverify + " or o.verifyno=" + (int)Controllers.orderstatus.sjyverify + "))";
                        break;
                    case Controllers.crole.supervisor:
                        where = " and (ISNULL(o.duid,0)=" + user.id + " and (ISNULL(o.localstatus,0)=" + (int)Controllers.orderstatus.cjsend + "))";
                        break;
                }
                sql = string.Format(sql, where);
                var rows = Controllers.DB.QueryOne(sql);
                var sql2 = @"select
  count(0) num
from (
       select
         ROW_NUMBER()
         over ( partition by o.id
           order by ov.id desc ) top1,
         o.status,
         o.sendall,
         o.createuid,
         o.receiveuid,
         o.changid,
         ov.uid,
         ov.verifyno,
         ov.status               agree
       from ctorder o
         left join ctorder_verifyflow ov on o.id = ov.oid and ov.verifyno < 5
     ) o
  left join cuser u1 on o.createuid = u1.id
where o.status >= 0 and o.status < 6 and (o.top1 = 1 or o.verifyno = 5) {0}";
                var where2 = "";
                switch ((Controllers.crole)user.utype.Value)
                {
                    case Controllers.crole.admin:
                        where2 = " and (o.status=" + (int)Controllers.torderstatus.submit + " or o.createuid=" + user.id + " and (o.status=" + (int)Controllers.torderstatus.create + "))";
                        break;
                    case Controllers.crole.city:
                        where2 = " and ((o.createuid = " + user.id + " and (o.status = " + (int)Controllers.torderstatus.create + " or o.status = " + (int)Controllers.torderstatus.cjsend + ")) or(o.receiveuid = " + user.id + " and o.status = " + (int)Controllers.torderstatus.cjconfirm + ") and o.sendall is null)";
                        break;
                    case Controllers.crole.manufactor:
                        where2 = " and (o.changid=" + user.id + " and o.verifyno=" + (int)Controllers.torderstatus.sgsverify + "  and o.agree= 1)";
                        break;
                    default:
                        where2 = " 1 = 2";
                        break;
                }
                sql2 = string.Format(sql2, where2);
                var rows2 = Controllers.DB.QueryOne(sql2);
                if (rows != null)
                {
                    filterContext.Controller.ViewBag.tasknum = Controllers.O2.O2I(rows["num"]) + (rows2 == null ? 0 : Controllers.O2.O2I(rows2["num"]));
                }
            }
            else {
                filterContext.Controller.ViewBag.loginuser = new CTCCGoods.Controllers.cuser() { utype=-1};
            }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Auth = string.IsNullOrWhiteSpace(Auth) ? "" : Auth.Trim();
            var user=(Controllers.cuser)filterContext.HttpContext.Session["loginuser"];
            var auth = user == null ? "-1" : user.utype.Value+"";
            if (Auth!="" && auth != "0") {
                if (Auth.IndexOf(auth) < 0) {
                    if (!filterContext.HttpContext.Request.IsAjaxRequest()) {
                        filterContext.Result = new RedirectResult("/");
                    } else {
                        var json = new JsonResult();
                        json.Data = new { ok = false, msg = "您没有权限" };
                        filterContext.Result = json;
                    }
                }
            }
        }
    }
    public class MyAuthorizeAttribute : AuthorizeAttribute {
        public bool NoCheck { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //httpContext.Session["loginuser"] = new Controllers.cuser { id=1};
            if (NoCheck) return true;
            Controllers.cuser user = null;
            var usetoken = Controllers.O2.O2B(System.Configuration.ConfigurationManager.AppSettings["usetoken"]);
            var token = httpContext.Request["tk"];
            if (usetoken && !string.IsNullOrWhiteSpace(token))
            {
                try {
                    var deskey = "e10adc3949ba59abbe56e057f20f883e";

                    var tokenb = System.Convert.FromBase64String(token);
                    var token64 = System.Text.Encoding.GetEncoding("utf-8").GetString(tokenb);
                    tokenb = System.Convert.FromBase64String(token64);
                    System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
                    var stream = new System.IO.MemoryStream();
                    des.Mode = System.Security.Cryptography.CipherMode.ECB;
                    des.Key = System.Text.Encoding.GetEncoding("utf-8").GetBytes(deskey.Substring(0,8));
                    des.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                    stream = new System.IO.MemoryStream();
                    var cstream=new System.Security.Cryptography.CryptoStream(stream,des.CreateDecryptor(),System.Security.Cryptography.CryptoStreamMode.Write);
                    cstream.Write(tokenb, 0, tokenb.Length);
                    cstream.FlushFinalBlock();
                    token = System.Text.Encoding.GetEncoding("utf-8").GetString(stream.ToArray());

                    var tks = token.Split(new char[] { '-' });
                    if (tks.Length != 5) return false;
                    if (tks[4].Length != 14) return false;

                    System.DateTime indt = System.DateTime.ParseExact(tks[4], "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                    var seconds=(System.DateTime.Now - indt).TotalSeconds;
                    if (seconds > 30 || seconds<-30) return false;
                    string sql;
                    if (tks[2] == "0")
                    {
                        sql = "select * from cuser where utype=0";
                    }
                    else {
                        sql = "select * from cuser where utype=1 and wid="+tks[3];
                    }
                    var userdic = Controllers.DB.QueryAsDics(sql);
                    if (userdic == null) return false;

                    user = new Controllers.cuser();
                    user.id = int.Parse(userdic[0]["id"].ToString());
                    user.name = userdic[0]["name"].ToString();
                    user.status = int.Parse(userdic[0]["status"].ToString());
                    user.pwd = userdic[0]["pwd"].ToString();
                    user.utype = int.Parse(userdic[0]["utype"].ToString());
                    user.code = userdic[0]["code"].ToString();

                    httpContext.Session["loginuser"] = user;
                }
                catch {
                    return false;
                }
            }
            else {
                user = (Controllers.cuser)httpContext.Session["loginuser"];
            }
            if (user == null) return false;
            if (!user.id.HasValue) return false;
            var userd = Controllers.DB.QueryOne("select u.*,w.name wname from cuser u left join cwarehouse w on u.wid=w.id where u.id=" + user.id);
            if (userd == null) return false;
            if (!userd["status"].Equals(1)) return false;
            user.code = userd["code"].ToString();
            user.name = userd["name"].ToString();
            user.utype = int.Parse(userd["utype"].ToString());
            user.status = int.Parse(userd["status"].ToString());
            user.pwd = userd["pwd"].ToString();
            user.wid = userd["wid"] == System.DBNull.Value ? (int?)null : int.Parse(userd["wid"].ToString());
            user.wname = userd["wname"].ToString();
            user.tel = userd["tel"].ToString();
            user.contacts = userd["contacts"].ToString();
            user.wids = new System.Collections.Generic.List<int>();
            if (user.utype == 2) {
                var wids = Controllers.DB.QueryAsDics("select * from cdesignset where uid="+user.id);
                if (wids != null) {
                    foreach (var wid in wids) {
                        user.wids.Add(Controllers.O2.O2I(wid["wid"]));
                    }
                }
            }
            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest()) {
                //filterContext.HttpContext.Response.Redirect("/login", true);
                var usetoken = Controllers.O2.O2B(System.Configuration.ConfigurationManager.AppSettings["usetoken"]);
                if (usetoken) {
                    filterContext.Result = new ContentResult() { Content="参数无效"};
                } else {
                    filterContext.Result = new RedirectResult("/login");
                }
            } else {
                //filterContext.HttpContext.Response.ContentType = "application/json";
                //filterContext.HttpContext.Response.Write("{ok:false,msg:'您没有登录'}");
                //filterContext.HttpContext.Response.End();
                var json = new JsonResult();
                json.Data = new { ok = false, msg = "您没有权限" };
                filterContext.Result = json;
            }
        }
    }
}