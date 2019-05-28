using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using DWMapService.Lib;

namespace DWMapService.Controllers
{
    public class ReqController : Controller
    {
        //
        // GET: /Req/

        public ActionResult Index()
        {
            DataTable dt = new DataTable();
            string sql = "select * from DATA_STATISTICS";
            dt = DB.QueryAsDt(sql);
            String result = DatatableToJson.GetJsonByDataset(dt);
            //context.Response.Write(result);
            return Content(result);
        }

    }
}
