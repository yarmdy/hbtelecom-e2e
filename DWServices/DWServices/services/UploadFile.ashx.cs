using DWServices.BLL;
using DWServices.Common;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// UploadFile 的摘要说明
    /// </summary>
    public class UploadFile : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("{\"info\":\"请先登录\"}");
                return;
            }

            PostParameter paramter = PostParameter.getParameter(context);
            String result = "";
            try
            {
                HttpFileCollection httpFileCollection = context.Request.Files;
                //HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                HttpPostedFile file = null;
                for (int i = 0; i < httpFileCollection.Count; i++)
                {
                    file = httpFileCollection[i];
                    if (file != null)
                    {
                        string filename = file.FileName;
                        SaveFile(filename, file);
                        DataTable exceldata = Com.Netframe.Helpers.OfficeHelper.ReadExcel(HttpContext.Current.Server.MapPath("~/Tmp/") + filename);
                        if (exceldata != null)
                        {
                            exceldata.Columns[0].ColumnName = "ECI";
                            if (exceldata.Rows.Count > 0)
                            {
                                if (exceldata.Rows[0][0].ToString().ToLower() == "eci")
                                {
                                    exceldata.Rows.Remove(exceldata.Rows[0]);
                                }
                            }
                            exceldata = (new Analysis()).analysisCell(exceldata, paramter.QuertyTime);
                            result = DataTableConvertJson.DataTableToJson("data", exceldata);
                        }
                    }
                }
                context.Response.Write(result);
            }
            catch (Exception exp)
            {
                context.Response.Clear();
                context.Response.Write("{\"info\":\"" + exp.Message + "\"}");
            }
        }

        private bool SaveFile(string filename, HttpPostedFile file)
        {
            try
            {
                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Tmp/")))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Tmp/"));
                }
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Tmp/") + filename))
                {
                    File.Delete(HttpContext.Current.Server.MapPath("~/Tmp/") + filename);
                }
                file.SaveAs(HttpContext.Current.Server.MapPath("~/Tmp/") + filename);
                return true;
            }
            catch
            {
                throw;
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