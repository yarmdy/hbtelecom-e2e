using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DWServices.services
{
    /// <summary>
    /// DataStatus 的摘要说明
    /// </summary>
    public class DataStatus : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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

            string s = DWClientLib.Client.GetAnalyzersStatus();
            //s = "<?xml version=\"1.0\" encoding=\"utf - 8\" ?>" + s;
            if (s == "") {
                context.Response.Write("{ok:false}");
                return;
            }
            XDocument xml = XDocument.Parse(s);
            string data = "";
            if (xml.ToString() != "")
            {
                data = data + "{ok:true,status:[";
                var filtered = from analyzer in xml.Descendants("analyzer")
                               select new
                               {
                                   name = (string)analyzer.Element("name"),
                                   sttsd = (string)analyzer.Element("sttsd"),
                                   sstsq = (string)analyzer.Element("sttsq"),
                                   errd = (string)analyzer.Element("errd"),
                                   errq = (string)analyzer.Element("sstsq"),
                               };
                foreach (var a in filtered)
                {
                    data = data + "{name:'" + a.name + "',sttsd:'" + a.sttsd + "',sttsq:'" + a.sstsq + "',errd:'" + a.errd + "',errq:'" + a.errq + "'},";
                }
                data = data.Remove(data.Length - 1, 1);
                data = data + "]}";
            }
            else
            {
                data = "{ok:false}";
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