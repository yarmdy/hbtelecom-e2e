using DWServices.Common;
using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace DWServices
{
    /// <summary>
    /// Login 的摘要说明
    /// </summary>
    public class Login : IHttpHandler,IRequiresSessionState 
    { 
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request["logout"] == "yes")
            {
                context.Session["user"] = null;
                context.Response.Redirect("login.html");
            }
            else if (context.Request["act"] == "cpwd") {
                context.Response.ContentType = "text/plain";
                var ok = true;
                var msg = "修改成功";
                var opwd = context.Request["opwd"]+"";
                var npwd = context.Request["npwd"]+"";
                var npwd2 = context.Request["npwd2"]+"";
                opwd = opwd.Replace("\'", "").Replace("\\", "");
                npwd = npwd.Replace("\'", "").Replace("\\", "");
                npwd2 = npwd2.Replace("\'", "").Replace("\\", "");
                if (ok) {
                    if (context.Session["user"] == null) {
                        ok = false;
                        msg = "会话超时请重新登录";
                    }
                }
                if (ok) {
                    if (string.IsNullOrEmpty(opwd) || string.IsNullOrEmpty(npwd) || string.IsNullOrEmpty(npwd2))
                    {
                        ok = false;
                        msg = "密码不能为空";
                    }
                }
                if (ok) {
                    if (npwd!=npwd2)
                    {
                        ok = false;
                        msg = "新密码和确认码不一致";
                    }
                }
                if (ok) {
                    var user = (User)context.Session["user"];
                    var sqlpwd = "select id,usercode,orgname, username ,password,permissions  from DATA_USER where id=  '" + user.Id + "' and password='" + opwd + "'";
                    DataTable data = OraConnect.ReadData(sqlpwd);
                    if (data.Rows.Count <= 0)
                    {
                        ok = false;
                        msg = "旧密码不正确";
                    }
                    else {
                        sqlpwd = "update DATA_USER set password='"+npwd+"' where id=  '" + user.Id + "'";
                        OraConnect.ExecuteSQL(sqlpwd);
                    }
                }
                context.Response.Write("{\"ok\":"+ok.ToString().ToLower()+",\"msg\":\""+msg+"\"}");
            } 
            else {
                context.Response.ContentType = "text/plain";
                String usercode = context.Request["usercode"]+"";
                String password = context.Request["password"]+"";
                usercode = usercode.Replace("\'", "").Replace("\\","");
                password = password.Replace("\'", "").Replace("\\", "");
                String sqlstr = "select id,usercode,orgname, username ,password,permissions  from DATA_USER where usercode=  '" + usercode + "' and password='" + password + "'";
                DataTable data = OraConnect.ReadData(sqlstr);
                String result = DataTableConvertJson.DataTableToJson("data", data);
                if(data.Rows.Count>0){
                    User user=new User();
                    DataRow row =data.Rows[0];
                    user.Id = row["id"].ToString();
                    user.Username=row["username"].ToString();
                    user.Password = row["password"].ToString();
                    user.Permissions = row["permissions"].ToString();
                    user.Orgname = row["Orgname"].ToString();
                    user.Usercode = row["usercode"].ToString();
                    context.Session["user"] = user;
                    context.Response.Write("{\"result\":\"success\"}");
                }else{
                    context.Response.Write("{\"result\":\"error\"}");
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}