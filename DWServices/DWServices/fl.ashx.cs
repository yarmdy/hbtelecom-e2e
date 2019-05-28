using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using DWServices.DAL;
using System.Data;
using DWServices.Common;

namespace DWServices
{
    /// <summary>
    /// fl 的摘要说明
    /// </summary>
    public class fl : IHttpHandler, IRequiresSessionState 
    {

        public void ProcessRequest(HttpContext context)
        {
            var __user = context.Request["user"];
            var __password = context.Request["password"];
            var __fromUrl = context.Request["fromUrl"];

            if (string.IsNullOrEmpty(__user) || string.IsNullOrEmpty(__password))
            {
                context.Response.Write("登录失败！");
                return;
            }
            __fromUrl = __fromUrl + "";
            __user = __user.Replace("\'", "").Replace("\\", "");
            __password = __password.Replace("\'", "").Replace("\\", "");
            String sqlstr = "select id,usercode,orgname, username ,password,permissions  from DATA_USER where usercode=  '" + __user + "' and password='" + __password + "'";
            
            DataTable data = OraConnect.ReadData(sqlstr);
            //String result = DataTableConvertJson.DataTableToJson("data", data);
            if (data.Rows.Count > 0)
            {
                User user = new User();
                DataRow row = data.Rows[0];
                user.Id = row["id"].ToString();
                user.Username = row["username"].ToString();
                user.Password = row["password"].ToString();
                user.Permissions = row["permissions"].ToString();
                user.Orgname = row["Orgname"].ToString();
                user.Usercode = row["usercode"].ToString();
                context.Session["user"] = user;
                context.Response.Redirect("/page/index.aspx");
            }
            else
            {
                context.Response.Write("登录失败");
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