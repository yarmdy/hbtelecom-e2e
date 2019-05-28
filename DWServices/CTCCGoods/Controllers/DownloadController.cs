using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.OleDb;

namespace CTCCGoods.Controllers
{
    [Breadcrumb(Auth = "0")]
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Buytemplate() {
            var user = (cuser)Session["loginuser"];
            var mytemp = "temp/"+user.id;
            var temp = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,mytemp);
            var originfile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "template/buy_template.xlsx");
            var tempfile = System.IO.Path.Combine(temp,"buy_template.xlsx");
            if (!System.IO.Directory.Exists(temp))
            {
                System.IO.Directory.CreateDirectory(temp);
            }
            if (System.IO.File.Exists(tempfile)) {
                System.IO.File.Delete(tempfile);
            }
            System.IO.File.Copy(originfile,tempfile);

            var constr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + tempfile + ";" + "Extended Properties=Excel 12.0;";
            OleDbConnection con = new OleDbConnection(constr);
            con.Open();
            var tb = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            con.Close();
            var tbn = tb.Rows[0]["TABLE_NAME"].ToString();
            var tb2 = tb.Rows[1]["TABLE_NAME"].ToString();
            var tb3 = tb.Rows[2]["TABLE_NAME"].ToString();
            var citys = DB.Query("select * from cwarehouse");
            var goods = DB.Query("select * from cgoods");
            con.Open();
            OleDbCommand com = new OleDbCommand("", con);
            foreach (DataRow dr in citys.Rows)
            {
                com.CommandText = "insert into [" + tb2 + "] values ('" + dr["name"] + "')";
                com.ExecuteNonQuery();
            }
            foreach (DataRow dr in goods.Rows)
            {
                com.CommandText = "insert into [" + tb3 + "] values ('" + dr["name"] + "')";
                com.ExecuteNonQuery();
            }
            con.Close();
            return File(tempfile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","购买模板.xlsx");
        }
    }
}
