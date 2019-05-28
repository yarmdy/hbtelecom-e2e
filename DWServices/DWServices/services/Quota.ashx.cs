using DWServices.Common;
using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// Quota 的摘要说明
    /// </summary>
    public class Quota : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/plain";

            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null || user.Permissions!="1")
            {
                context.Response.Write("{\"data\":false}");
                return;
            }

            String quota = context.Request["THEQUOTA"];
            String optaior = context.Request["optaior"];
            String key = context.Request["MYKEY"];
            if (optaior != null && optaior.Equals("update"))
            {
                String sql = " update  DATA_QUOTA  set  ";
                if (context.Request["MAXVALUE"] == "")
                {
                    sql += " MAXVALUE = null";
                }
                else
                {
                    float maxvalue = float.Parse(context.Request["MAXVALUE"]);
                    sql += " MAXVALUE = " + maxvalue;
                }
                if (context.Request["MINVALUE"] == "")
                {
                    sql += " ,MINVALUE = null";
                }
                else
                {
                    float minvalue = float.Parse(context.Request["MINVALUE"]);
                    sql += ", MINVALUE = " + minvalue;
                }
                bool b = OraConnect.ExecuteSQL(sql + " where MYKEY='" + key + "'");
                context.Response.Write("{\"data\":\"" + b + "\"}");
            }
            else
            {
                String sqlstr = "select * from DATA_QUOTA where thequota like '%" + quota + "%' order by MYKEY";
                DataTable data = OraConnect.ReadData(sqlstr);
                String result = DataTableConvertJson.DataTableToJson("data", data);
                context.Response.Write(result);
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