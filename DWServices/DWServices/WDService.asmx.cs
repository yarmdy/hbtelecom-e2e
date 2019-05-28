using Com.Netframe.Helpers;
using DWServices.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DWServices
{
    /// <summary>
    /// WDService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WDService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public String AnalysisWD()
        {
            try
            {
                Analysis analysis = new Analysis();
                if (analysis.AnalysisAll())
                    return "分析结束！";
                else
                    return "分析失败！请查看日志。";
            }
            catch (Exception err)
            {
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                return err.ToString();
            }
        }
    }
}
