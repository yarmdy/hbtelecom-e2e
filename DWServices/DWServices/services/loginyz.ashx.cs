using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// loginyz 的摘要说明
    /// </summary>
    public class loginyz : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{ok:false}");
            }
            else
            {
                context.Response.Write("{ok:true}");
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