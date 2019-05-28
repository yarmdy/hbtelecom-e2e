using DWServices.Common;
using DWServices.DAL;
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
    /// CaseUpfile 的摘要说明
    /// </summary> 
    public class CaseUpFile : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        HttpContext context = null;
        public void ProcessRequest(HttpContext context)
        {
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null || user.Permissions != "1" && user.Permissions != "2")
            {
                context.Response.Write("{\"info\":\"请先登录\"}");
                return;
            }
            if (context.Request["eci"] == null)
            {
                caseUpfile(context);
            }
            else {
                GetCaseInterface(context);
            }
        }
        private void caseUpfile(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //PostParameter paramter = PostParameter.getParameter(context);
            String result = "";
            try
            {
                this.context = context;
                //HttpFileCollection files = System.Web.HttpContext.Current.Request.Files; 
                String optaior = context.Request["optaior"];
                if (optaior != null && optaior.Equals("save"))
                {
                    HttpFileCollection httpFileCollection = context.Request.Files;
                    String filename = "";
                    String filepath = "";
                    HttpPostedFile file = null;
                    for (int i = 0; i < httpFileCollection.Count; i++)
                    {
                        file = httpFileCollection[i];
                        if (file != null)
                        {
                            filename = file.FileName;
                            filepath = SaveFile(filename, file);
                        }
                    }
                    bool b = SaveToCase(filename, filepath, context.Request["checkeboxvalue"], context.Request["checkeboxname"]);
                    context.Response.Write("{\"result\":\"" + b + "\"}");
                }
                else if (null != optaior && optaior.IndexOf("down", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    this.DownLoadFile(context.Request.Params["id"]);
                }
                else if (null != optaior && optaior.IndexOf("delete", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    context.Response.Write("{\"result\":\"" + deleteData(context.Request.Params["id"]) + "\"}");
                }
                else
                {
                    String filename = context.Request["filename"];
                    String keyname = context.Request["keyname"];
                    context.Response.Write(getCaseData(filename, keyname));
                }

            }
            catch (Exception exp)
            {
                context.Response.Clear();
                context.Response.Write("{\"info\":\"" + exp.Message + "\"}");
            }
        }
        private void GetCaseInterface(HttpContext context)
        {
            var eci = context.Request["eci"];
            var dt = DateTime.Parse( context.Request["datePicker"]);
            var list= GetThCase(eci, dt);
            string strjson = "{\"result\":true,data:[{0}]}";
            Random r = new Random();
            var items = list.Select(a => "{name:'" + a["FILE_NAME"].ToString() + "',url:'../services/CaseUpfile.ashx?optaior=download&id="+a["ID"]+"&r="+r.Next()+"'}").ToArray();
            var resstr = strjson.Replace("{0}",string.Join(",",items));
            context.Response.Clear();
            context.Response.Write(resstr);
        }

        public bool SaveToCase(String filename, String filePath, String checkeboxvalue, String checkname)
        {
            String[] checkeboxvaluearr = checkeboxvalue.Split(',');
            Guid guid = Guid.NewGuid();
            String uiid = guid.ToString();
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.AppendFormat("insert into CASE_DATA(id,file_name,file_path,down_count,keyword,is_delete,keys_name)");
            sql.AppendFormat("values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", uiid, filename.Trim(), filePath.Replace(@"\", "/"), 0, checkeboxvalue, 0, checkname.Trim());
            bool isResult = OraConnect.ExecuteSQL(sql.ToString());
            return isResult;
        }

        public String getCaseData(String filename, String keyname)
        {

            String sqlstr = "select * from CASE_DATA where  is_delete=0  ";
            if (filename != null && !filename.Equals(""))
            {
                sqlstr += " and file_name like '%" + filename + "%'";
            }
            if (keyname != null && !keyname.Equals(""))
            {
                sqlstr += " and keys_name like '%" + keyname + "%'";
            }
            DataTable data = OraConnect.ReadData(sqlstr);
            String result = DataTableConvertJson.DataTableToJson("data", data);
            return result;
        }

        public bool deleteData(String id)
        {

            String sqlstr = "update  CASE_DATA set  is_delete= 1 where id= '"+id+"'"; 
            bool result = OraConnect.ExecuteSQL(sqlstr);
            return result;
        }
        private String SaveFile(string filename, HttpPostedFile file)
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
                return HttpContext.Current.Server.MapPath("~/Tmp/") + filename;
            }
            catch
            {
                throw;
            }
        }
        private void DownLoadFile(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            String sqlstr = string.Format("select * from CASE_DATA where  id='{0}' ", id);
            DataTable data = OraConnect.ReadData(sqlstr);
            string fileName = data.Rows[0]["file_path"].ToString();
            sqlstr = "update  CASE_DATA set  down_count= down_count+1 where id= '" + id + "'";
            OraConnect.ExecuteSQL(sqlstr);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            this.context.Response.Clear();
            this.context.Response.ClearContent();
            this.context.Response.ClearHeaders();
            this.context.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileInfo.Name);
            this.context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            this.context.Response.AddHeader("Content-Transfer-Encoding", "binary");
            this.context.Response.ContentType = "application/octet-stream";
            this.context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            this.context.Response.WriteFile(fileInfo.FullName);
            this.context.Response.Flush();
            //this.context.Response.End();
        }
        public bool IsReusable
        {
            get { return false; }
        }

        //获取三条案例数据
        public List<Dictionary<string, string>> GetThCase(string eci, DateTime datetime)
        {
            string[] arrayA = { "告警", "干扰", "容量", "覆盖" };
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            string sql = "select  * from DATA_KQIINFO t where t.eci='" + eci + "' and t.createtime=to_date('" + datetime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and rownum<=1";
            DataTable data = OraConnect.ReadData(sql);
            if (data != null && data.Rows.Count > 0)
            {
                string keyword = data.Rows[0]["REASON"].ToString();
                keyword = keyword != null ? keyword.Replace("\n", "").Replace("\r", "").Replace(" ","") : "";
                string[] sArray = keyword.Split(new char[]{'、'},StringSplitOptions.RemoveEmptyEntries);
                //DataTable sdata;
               
                // Dictionary<string, string> dc;
                if (sArray.Length == 1)
                {
                    //dc=new Dictionary<string,string>();
                    string msql = "select * from (select * from CASE_DATA c  order by c.DOWN_COUNT DESC) t where t.keys_name like '%" + sArray[0] + "%' and t.is_delete=0 and rownum<=3 order by t.DOWN_COUNT";
                    DataTable sdata = OraConnect.ReadData(msql);
                    if (sdata != null && sdata.Rows.Count > 0)
                    {
                        foreach (var row in sdata.Rows)
                        {
                            Dictionary<string, string> dc = new Dictionary<string, string>();
                            dc.Add("FILE_NAME", sdata.Rows[0]["FILE_NAME"].ToString());
                            dc.Add("FILE_PATH", sdata.Rows[0]["FILE_PATH"].ToString());
                            dc.Add("ID", sdata.Rows[0]["ID"].ToString());
                            list.Add(dc);
                        }
                    }
                }
                else if (sArray.Length == 2)
                {
                    Dictionary<string, string> dc = new Dictionary<string, string>();
                    string reasonOne = "";
                    string msql1 = "select * from (select * from CASE_DATA c  order by c.DOWN_COUNT DESC) t where t.keys_name like '%" + sArray[0] + "%' and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                    DataTable sdata1 = OraConnect.ReadData(msql1);
                    if (sdata1 != null && sdata1.Rows.Count > 0)
                    {
                        dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                        dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                        dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                        list.Add(dc);
                    }
                   

                    Dictionary<string, string> dc2 = new Dictionary<string, string>();
                    string msql2 = "select * from (select * from CASE_DATA c  order by c.DOWN_COUNT DESC) t where t.keys_name like '%" + sArray[1] + "%' and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                    DataTable sdata2 = OraConnect.ReadData(msql2);
                    if (sdata2 != null && sdata2.Rows.Count > 0)
                    {
                        dc2.Add("FILE_NAME", sdata2.Rows[0]["FILE_NAME"].ToString());
                        dc2.Add("FILE_PATH", sdata2.Rows[0]["FILE_PATH"].ToString());
                        dc2.Add("ID", sdata2.Rows[0]["ID"].ToString());
                        list.Add(dc2);
                    }
                    

                    if (sArray[0] == "本基站告警" || sArray[0] == "相邻基站故障" || sArray[1] == "本基站告警" || sArray[1] == "相邻基站故障")
                    {
                        reasonOne = "设备告警";
                    }
                    else if (sArray[0] == "下行模三干扰" || sArray[0] == "上行干扰" || sArray[1] == "下行模三干扰" || sArray[1] == "上行干扰")
                    {
                        reasonOne = "干扰";
                    }
                    else if (sArray[0] == "上行PRB平均利用率" || sArray[0] == "下行PRB平均利用率" || sArray[0] == "RRC连接用户数" || sArray[0] == "下行流量" || sArray[0] == "上行流量" || sArray[1] == "上行PRB平均利用率" || sArray[1] == "下行PRB平均利用率" || sArray[1] == "RRC连接用户数" || sArray[1] == "下行流量" || sArray[1] == "上行流量")
                    {
                        reasonOne = "容量";
                    }
                    else if (sArray[0] == "弱覆盖" || sArray[0] == "重叠覆盖" || sArray[0] == "过覆盖" || sArray[1] == "弱覆盖" || sArray[1] == "重叠覆盖" || sArray[1] == "过覆盖")
                    {
                        reasonOne = "覆盖";
                    }


                    Dictionary<string, string> dc3 = new Dictionary<string, string>();
                    if (reasonOne == "设备告警")
                    {

                        string msql3 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%本基站告警%' or t.keys_name like '%相邻基站故障%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                        DataTable sdata3 = OraConnect.ReadData(msql3);
                        if (sdata3 != null && sdata3.Rows.Count > 0)
                        {
                            dc3.Add("FILE_NAME", sdata3.Rows[0]["FILE_NAME"].ToString());
                            dc3.Add("FILE_PATH", sdata3.Rows[0]["FILE_PATH"].ToString());
                            dc3.Add("ID", sdata3.Rows[0]["ID"].ToString());
                        }
                        
                    }
                    else if (reasonOne == "干扰")
                    {
                        string msql3 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%下行模三干扰%' or t.keys_name like '%上行干扰%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                        DataTable sdata3 = OraConnect.ReadData(msql3);
                        if (sdata3 != null && sdata3.Rows.Count > 0)
                        {
                            dc3.Add("FILE_NAME", sdata3.Rows[0]["FILE_NAME"].ToString());
                            dc3.Add("FILE_PATH", sdata3.Rows[0]["FILE_PATH"].ToString());
                            dc3.Add("ID", sdata3.Rows[0]["ID"].ToString());
                        }
                        
                    }
                    else if (reasonOne == "容量")
                    {
                        string msql3 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%上行PRB平均利用率%' or t.keys_name like '%下行PRB平均利用率%' or t.keys_name like '%RRC连接用户数%' or t.keys_name like '%下行流量%' or t.keys_name like '%上行流量%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                        DataTable sdata3 = OraConnect.ReadData(msql3);
                        if (sdata3 != null && sdata3.Rows.Count > 0)
                        {
                            dc3.Add("FILE_NAME", sdata3.Rows[0]["FILE_NAME"].ToString());
                            dc3.Add("FILE_PATH", sdata3.Rows[0]["FILE_PATH"].ToString());
                            dc3.Add("ID", sdata3.Rows[0]["ID"].ToString());
                        }
                       
                    }
                    else if (reasonOne == "覆盖")
                    {
                        string msql3 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%弱覆盖%' or t.keys_name like '%重叠覆盖%' or t.keys_name like '%过覆盖%') and t.is_delete=0  and rownum<=1 order by t.DOWN_COUNT";
                        DataTable sdata3 = OraConnect.ReadData(msql3);
                        if (sdata3 != null && sdata3.Rows.Count > 0)
                        {
                            dc3.Add("FILE_NAME", sdata3.Rows[0]["FILE_NAME"].ToString());
                            dc3.Add("FILE_PATH", sdata3.Rows[0]["FILE_PATH"].ToString());
                            dc3.Add("ID", sdata3.Rows[0]["ID"].ToString());
                        }
                        
                    }
                    list.Add(dc3);
                }
                else if (sArray.Length == 3)
                {
                    Dictionary<string, string> dc = new Dictionary<string, string>();
                    string msql1 = "select * from (select * from CASE_DATA c  order by c.DOWN_COUNT DESC) t where t.keys_name like '%" + sArray[0] + "%' and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                    DataTable sdata1 = OraConnect.ReadData(msql1);
                    if (sdata1 != null && sdata1.Rows.Count > 0)
                    {
                        dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                        dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                        dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                        list.Add(dc);
                    }
                    
                    Dictionary<string, string> dc2 = new Dictionary<string, string>();
                    string msql2 = "select * from (select * from CASE_DATA c  order by c.DOWN_COUNT DESC) t where t.keys_name like '%" + sArray[1] + "%' and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                    DataTable sdata2 = OraConnect.ReadData(msql2);
                    if (sdata2 != null && sdata2.Rows.Count > 0)
                    {
                        dc2.Add("FILE_NAME", sdata2.Rows[0]["FILE_NAME"].ToString());
                        dc2.Add("FILE_PATH", sdata2.Rows[0]["FILE_PATH"].ToString());
                        dc2.Add("ID", sdata2.Rows[0]["ID"].ToString());
                        list.Add(dc2);
                    }
                    
                    Dictionary<string, string> dc3 = new Dictionary<string, string>();
                    string msql3 = "select * from (select * from CASE_DATA c  order by c.DOWN_COUNT DESC) t where t.keys_name like '%" + sArray[2] + "%' and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                    DataTable sdata3 = OraConnect.ReadData(msql3);
                    if (sdata3 != null && sdata3.Rows.Count > 0)
                    {
                        dc3.Add("FILE_NAME", sdata3.Rows[0]["FILE_NAME"].ToString());
                        dc3.Add("FILE_PATH", sdata3.Rows[0]["FILE_PATH"].ToString());
                        dc3.Add("ID", sdata3.Rows[0]["ID"].ToString());
                        list.Add(dc3);
                    }
                    
                }
                else if (sArray.Length > 3)
                {

                    int j = 0;
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        Dictionary<string, string> dc = new Dictionary<string, string>();
                        if (sArray[i] == "本基站告警" || sArray[i] == "相邻基站故障")
                        {
                            string msql1 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%本基站告警%' or t.keys_name like '%相邻基站故障%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                            DataTable sdata1 = OraConnect.ReadData(msql1);
                            if (sdata1 != null && sdata1.Rows.Count > 0)
                            {
                                dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                j = j + 1;
                                list.Add(dc);
                                break;
                            }
                           
                        }
                    }
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        Dictionary<string, string> dc = new Dictionary<string, string>();
                        if (sArray[i] == "下行模三干扰" || sArray[i] == "上行干扰")
                        {
                            string msql1 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%下行模三干扰%' or t.keys_name like '%上行干扰%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                            DataTable sdata1 = OraConnect.ReadData(msql1);
                            if (j == 1)
                            {
                                if (sdata1 != null && sdata1.Rows.Count > 0)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    j = j + 1;
                                    list.Add(dc);
                                }
                                
                            }
                            else if (j == 0)
                            {
                                if (sdata1 != null && sdata1.Rows.Count > 0)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    j = j + 1;
                                    list.Add(dc);
                                }
                                
                            }

                            break;
                        }
                    }
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        Dictionary<string, string> dc = new Dictionary<string, string>();
                        if (sArray[i] == "上行PRB平均利用率" || sArray[i] == "下行PRB平均利用率" || sArray[i] == "RRC连接用户数" || sArray[i] == "下行流量" || sArray[i] == "上行流量")
                        {
                            string msql1 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where t.keys_name like ('%上行PRB平均利用率%' or t.keys_name like '%下行PRB平均利用率%' or t.keys_name like '%RRC连接用户数%' or t.keys_name like '%下行流量%' or t.keys_name like '%上行流量%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                            DataTable sdata1 = OraConnect.ReadData(msql1);
                            if (sdata1 != null && sdata1.Rows.Count > 0)
                            {
                                if (j == 0)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    list.Add(dc);
                                    j = j + 1;
                                }
                                else if (j == 1)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    list.Add(dc);
                                    j = j + 1;
                                }
                                else if (j == 2)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    list.Add(dc);
                                    j = j + 1;
                                }
                                break;
                            }
                           
                        }
                    }
                    if (j < 3)
                    {
                        for (int i = 0; i < sArray.Length; i++)
                        {
                            Dictionary<string, string> dc = new Dictionary<string, string>();
                            if (sArray[i] == "弱覆盖" || sArray[i] == "重叠覆盖" || sArray[i] == "过覆盖")
                            {
                                string msql1 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%弱覆盖%' or t.keys_name like '%重叠覆盖%' or t.keys_name like '%过覆盖%') and t.is_delete=0  and rownum<=1 order by t.DOWN_COUNT";
                                DataTable sdata1 = OraConnect.ReadData(msql1);
                                if (sdata1 != null && sdata1.Rows.Count > 0)
                                {
                                    if (j == 0)
                                    {
                                        dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                        dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                        dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                        j = j + 1;
                                        list.Add(dc);
                                    }
                                    else if (j == 1)
                                    {
                                        dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                        dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                        dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                        j = j + 1;
                                        list.Add(dc);
                                    }
                                    else if (j == 2)
                                    {
                                        
                                        dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                        dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                        dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                        list.Add(dc);
                                        j = j + 1;
                                    }
                                    break;
                                }
                             
                            }
                        }
                    }
                }
                if (list.Count == 0)
                {
                    int j = 0;
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        Dictionary<string, string> dc = new Dictionary<string, string>();
                        if (sArray[i] == "本基站告警" || sArray[i] == "相邻基站故障")
                        {
                            string msql1 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%本基站告警%' or t.keys_name like '%相邻基站故障%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                            DataTable sdata1 = OraConnect.ReadData(msql1);
                            if (sdata1 != null && sdata1.Rows.Count > 0)
                            {
                                dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                j = j + 1;
                                list.Add(dc);
                                break;
                            }

                        }
                    }
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        Dictionary<string, string> dc = new Dictionary<string, string>();
                        if (sArray[i] == "下行模三干扰" || sArray[i] == "上行干扰")
                        {
                            string msql1 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%下行模三干扰%' or t.keys_name like '%上行干扰%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                            DataTable sdata1 = OraConnect.ReadData(msql1);
                            if (j == 1)
                            {
                                if (sdata1 != null && sdata1.Rows.Count > 0)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    j = j + 1;
                                    list.Add(dc);
                                }

                            }
                            break;
                        }
                    }
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        Dictionary<string, string> dc = new Dictionary<string, string>();
                        if (sArray[i] == "上行PRB平均利用率" || sArray[i] == "下行PRB平均利用率" || sArray[i] == "RRC连接用户数" || sArray[i] == "下行流量" || sArray[i] == "上行流量")
                        {
                            string msql1 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%上行PRB平均利用率%' or t.keys_name like '%下行PRB平均利用率%' or t.keys_name like '%RRC连接用户数%' or t.keys_name like '%下行流量%' or t.keys_name like '%上行流量%') and t.is_delete=0 and rownum<=1 order by t.DOWN_COUNT";
                            DataTable sdata1 = OraConnect.ReadData(msql1);
                            if (sdata1 != null && sdata1.Rows.Count > 0)
                            {
                                if (j == 0)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    j = j + 1;
                                    list.Add(dc);
                                }
                                else if (j == 1)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    j = j + 1;
                                    list.Add(dc);
                                }
                                else if (j == 2)
                                {
                                    dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                    dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                    dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                    j = j + 1;
                                    list.Add(dc);
                                }
                                break;
                            }

                        }
                    }
                    if (list.Count < 3)
                    {
                        for (int i = 0; i < sArray.Length; i++)
                        {
                            Dictionary<string, string> dc = new Dictionary<string, string>();
                            if (sArray[i] == "弱覆盖" || sArray[i] == "重叠覆盖" || sArray[i] == "过覆盖")
                            {
                                string msql1 = "select * from (select * from CASE_DATA c order by c.DOWN_COUNT DESC) t where (t.keys_name like '%弱覆盖%' or t.keys_name like '%重叠覆盖%' or t.keys_name like '%过覆盖%') and t.is_delete=0  and rownum<=1 order by t.DOWN_COUNT";
                                DataTable sdata1 = OraConnect.ReadData(msql1);
                                if (sdata1 != null && sdata1.Rows.Count > 0)
                                {
                                    if (j == 0)
                                    {
                                        dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                        dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                        dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                        j = j + 1;
                                        list.Add(dc);
                                    }
                                    else if (j == 1)
                                    {
                                        dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                        dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                        dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                        j = j + 1;
                                        list.Add(dc);
                                    }
                                    else if (j == 2)
                                    {
                                       
                                        dc.Add("FILE_NAME", sdata1.Rows[0]["FILE_NAME"].ToString());
                                        dc.Add("FILE_PATH", sdata1.Rows[0]["FILE_PATH"].ToString());
                                        dc.Add("ID", sdata1.Rows[0]["ID"].ToString());
                                        j = j + 1;
                                        list.Add(dc);
                                    }
                                    break;
                                }

                            }
                        }
                    }
                }

            }
            list=list.Distinct(new CaseComparer()).ToList();
            return list;
        }
    }
    public class CaseComparer : IEqualityComparer<Dictionary<string,string>>
    {
        public bool Equals(Dictionary<string, string> x, Dictionary<string, string> y)
        {
            if (x!=null&&y!=null&& x.ContainsKey("ID") && y.ContainsKey("ID")) { 
                return x["ID"]==y["ID"];
            }
            return false;
        }
        public int GetHashCode(Dictionary<string, string> obj)
        {
            if (obj != null && obj.ContainsKey("ID"))
            {
                return obj["ID"].GetHashCode();
            }
            return 0;
        }
    }
}