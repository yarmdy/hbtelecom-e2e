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
    /// KPIServices 的摘要说明
    /// </summary>
    public class KPIServices : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("请登录");
                return;
            }
            
            String key = "";
            PostParameter paramter = PostParameter.getParameter(context);
            if (String.IsNullOrEmpty(paramter.QuertyType))
            {
                context.Response.Write("参数错误！");
            }
            else
            {
                if (String.IsNullOrEmpty(paramter.ECI))
                {
                    paramter.ECI = ECIAndIDConvert.ConvertECI(paramter.ENBID, paramter.LCRID).ToString();
                }
                if (!string.IsNullOrEmpty(paramter.ECI))
                {

                    if (!String.IsNullOrEmpty(paramter.QuertyScale))
                    {
                        if (paramter.QuertyScale.ToUpper() == "H")
                        {
                            key = context.Request["key"].ToLower();
                            String result = (new KPIAnalysis()).GetHourData(paramter.QuertyTime, paramter.ECI, key);
                            context.Response.Write(result);
                        }
                        else
                        {
                            key = context.Request["key"].ToLower();
                            String result = (new KPIAnalysis()).GetData(paramter.QuertyTime, paramter.ECI, key);
                            context.Response.Write(result);
                        }
                    }
                    else
                    {
                        key = context.Request["key"].ToLower();
                        String result = (new KPIAnalysis()).GetData(paramter.QuertyTime, paramter.ECI, key);
                        context.Response.Write(result);
                    }
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