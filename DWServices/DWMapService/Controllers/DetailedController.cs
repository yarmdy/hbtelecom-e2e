using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using DWMapService.Lib;

namespace DWMapService.Controllers
{
    public class DetailedController : Controller
    {
        //
        // GET: /Detailed/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getDetailedData(string name) {
            string sqltime = "select t.data_time from DATA_DISPLAY t where t.data_name='WIFI' AND T.DATA_STATUS='CURRENT'";
            DataTable dttime = DB.QueryAsDt(sqltime);
            string time = Convert.ToString(dttime.Rows[0]["data_time"]);
            string timeq = DateTime.Parse(time).AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
#if CESHI
            string desql = @"select p.* FROM 
                            (select s.city cityname,trunc(t.BFLOW / (1024 * 1024 * 1024), 4) BFLOW_G, t.* 
                                from KQI_MIN t left join v_workparameter s on t.ecgi = s.eci
                                where t.start_time >to_date('2017-9-18 15:15:00', 'yyyy-mm-dd hh24:mi:ss')
                                and t.start_time <=to_date('2017-9-18 15:30:00', 'yyyy-mm-dd hh24:mi:ss')) p
                            where p.BFLOW_G > 0.0003 and p.cityname='" + name + "'";
#else
            string desql = @"select p.* FROM 
                            (select s.city cityname,trunc(t.BFLOW / (1024 * 1024 * 1024), 4) BFLOW_G, t.* ,s.HOTSPOTCLASS,s.HOTSPOTNAME,s.SC_NAME 
                                from BUSYTIME_MIN t left join v_workparameter s on t.ecgi = s.eci
                                ) p where p.cityname='" + name + "'";
#endif
            DataTable dt = DB.QueryAsDt(desql);
            String result = DatatableToJson.GetJsonByDataset(dt);
            return Content(result);
        }

        public ActionResult getAlarmData(string name)
        {
            string sql = "select * from ALARM_MIN where city='"+name+"'";
            DataSet ds = DB.Query(sql);
            String result = DatatableToJson.GetJsonByDataset(ds.Tables[0]);
            return Content(result);
        }
    }
}
