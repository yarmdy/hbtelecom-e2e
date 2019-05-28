using DWServices.BLL;
using DWServices.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// DataLog 的摘要说明
    /// </summary>
    public class DataLog : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{\"data\":null}");
                return;
            }
            String time = context.Request["datePicker"];
            Datalog log = new Datalog();
            DataTable result = log.GetDatalog(time);
            DataTable lastresult = log.GetDataTable(result);

            String strresult = DataTableConvertJson.DataTableToJson("data",lastresult);
            context.Response.Write(strresult);
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