using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace DWServices.services
{
    /// <summary>
    /// Display 的摘要说明
    /// </summary>
    public class Display : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{\"ok\":false}");
                return;
            }

            
            string sql = "select * from DATA_DISPLAY";
            DataTable table = OraConnect.ReadData(sql);
            string data = "{ok:true,time:'"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"',data:[";
            for (int i=0;i<table.Rows.Count;i++)
            {
                DataRow row = table.Rows[i];
                string name = row["DATA_NAME"].ToString() + row["DATA_STATUS"].ToString();
                int count = Int32.Parse(row["DATA_COUNT"].ToString());
                string date = DateTime.Parse(row["DATA_TIME"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                if (i < table.Rows.Count - 1)
                {
                    data = data + "{name:'" + name + "',count:" + count + ",date:'" + date + "'},";
                }
                else
                {
                    data = data + "{name:'" + name + "',count:" + count + ",date:'" + date + "'}]}";
                }
            }
            context.Response.Write(data);
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