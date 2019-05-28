using DWServices.BLL;
using DWServices.Common;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// MRDataServices 的摘要说明
    /// </summary>
    public class MRDataServices : IHttpHandler,System.Web.SessionState.IRequiresSessionState
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
            String key = "";
            PostParameter paramter = PostParameter.getParameter(context);
            if (String.IsNullOrEmpty(paramter.QuertyType))
            {
                context.Response.Write("参数错误！");
            }
            #region 特例因前台变化增加
            else if (context.Request["key"].ToLower() == "sxgr")
            {
                key = context.Request["key"].ToLower();
                String result = (new KPIAnalysis()).GetData(paramter.QuertyTime, paramter.ECI, key);
                context.Response.Write(result);
            }
            #endregion
            else
            {
                if (String.IsNullOrEmpty(paramter.ECI))
                {
                    paramter.ECI = ECIAndIDConvert.ConvertECI(paramter.ENBID, paramter.LCRID).ToString();
                }
                if (!string.IsNullOrEmpty(paramter.ECI))
                {
                    key = context.Request["key"].ToLower();
                    String result = (new MRAnalysis()).GetData(paramter.QuertyTime, paramter.ECI, key);
                    context.Response.Write(result);
                }
                else
                {
                    context.Response.Write("参数错误！");
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