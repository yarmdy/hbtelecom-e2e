using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DWMapService.Controllers
{
    public class GjsController : Controller
    {
        //
        // GET: /Gjs/

        public ActionResult token()
        {
            string token = "//"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"\r\nwindow.token=\""+GetDataClass.token+"\";\r\n";
            token += "window.minzoom="+System.Configuration.ConfigurationManager.AppSettings["minzoom"]+";\r\n"+"window.maxzoom="+System.Configuration.ConfigurationManager.AppSettings["maxzoom"]+";";
            if (GetDataClass.CityFG != null && GetDataClass.CityFG.Tables[0].Rows.Count>0) {
                var cityfg = "\r\nwindow.fgstr=\"";
                foreach (System.Data.DataRow city in GetDataClass.CityFG.Tables[0].Rows) {
                    cityfg += getcity(city["CITY"].ToString()) + ":" + (((decimal)city["FG"]) * 100).ToString("0.00") + "%&nbsp;";
                }
                cityfg += "\"\r\n";
                token += cityfg;
            }
            Response.AddHeader("Cache-Control", "no-cache");
            return File(System.Text.Encoding.GetEncoding("utf-8").GetBytes(token), "application/x-javascript");
        }
        private string getcity(string city){
            city=city.ToLower();
            switch (city) { 
                case "shijiazhuang":
                    city = "石家庄";
                    break;
                case "baoding":
                    city = "保定";
                    break;
                case "handan":
                    city = "邯郸";
                    break;
                case "chengde":
                    city = "承德";
                    break;
                case "zhangjiakou":
                    city = "张家口";
                    break;
                case "tangshan":
                    city = "唐山";
                    break;
                case "langfang":
                    city = "廊坊";
                    break;
                case "cangzhou":
                    city = "沧州";
                    break;
                case "hengshui":
                    city = "衡水";
                    break;
                case "xingtai":
                    city = "邢台";
                    break;
                case "qinhuangdao":
                    city = "秦皇岛";
                    break;
                case "xionganxinqu":
                    city = "雄安";
                    break;
            }
            return city;
        }

    }
}
