using DWServices.BLL;
using DWServices.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// Resolve 的摘要说明
    /// </summary>
    public class Resolve : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{data:null}");
                return;
            }
            string type = context.Request["type"];
            string time = context.Request["datePicker"];
            string time2 = context.Request["datePickerEnd"];
            DataResolve res = new DataResolve();
            DataTable result = null;
            if (type == "query") {
                string type2 = context.Request["type2"];
                int eci = int.Parse(context.Request["eci"].ToString());
                result = res.GetDataRate(eci,type2);
            }
            else
            {
                result = res.GetData(time, time2);
            }
            String strresult = DataTableConvertJson.DataTableToJson("data", result);
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