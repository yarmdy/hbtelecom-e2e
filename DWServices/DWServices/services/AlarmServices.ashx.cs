using DWServices.BLL;
using DWServices.Common;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// AlarmAnalysis 的摘要说明
    /// </summary>
    public class AlarmServices : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("请先登录");
                return;
            }

            PostParameter paramter = PostParameter.getParameter(context);
            String result = "";
            if (String.IsNullOrEmpty(paramter.ECI))
            {
                paramter.ECI = ECIAndIDConvert.ConvertECI(paramter.ENBID, paramter.LCRID).ToString();
            }
            if (!string.IsNullOrEmpty(paramter.ECI))
            {
                String dist = "1000";
                AlarmAnalysis analysis=new AlarmAnalysis();
                dist = analysis.getAnalysisdist(paramter.ECI);
                result = analysis.GetData(paramter.ECI, dist, paramter.QuertyTime, paramter.QuertyType);
            }
            else
            {
                context.Response.Write("参数错误！");
            }
            context.Response.Write(result);
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