using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// Performance 的摘要说明
    /// </summary>
    public class Performance : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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
            string date = context.Request["date"];
            var rtp = context.Request["rtp"];
            date = string.Join("",date.Split('-'));
            try {
                string path = AppDomain.CurrentDomain.BaseDirectory + "perdata/" + date.Substring(0, 6);
                if (rtp == "1") {
                    path = AppDomain.CurrentDomain.BaseDirectory + "perdata/evtdata/" + date.Substring(0, 6);
                }
                string[] files = Directory.GetFiles(path, "*" + date + "*.csv");
                string result = "{";
                for (int i = 0; i < files.Length; i++)
                {
                    if (i < files.Length - 1)
                    {
                        result += "\"" + files[i].Split('_')[1].Split('.')[0] + "\":" + CsvRead(0, files[i]) + ",";
                    }
                    else
                    {
                        result += "\"" + files[i].Split('_')[1].Split('.')[0] + "\":" + CsvRead(0, files[i]) + "}";
                    }
                }
                context.Response.Write(result);
            } catch (Exception e) {
                context.Response.Write("不存在这天的数据");
            }
        }



        public string CsvRead(int n, string filePath)
        {
            DataTable dt = new DataTable();
            string name = filePath.Split('_')[1].Split('.')[0];
            String csvSplitBy = "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)";
            StreamReader reader = new StreamReader(filePath, System.Text.Encoding.Default, false);
            int i = 0, m = 0;
            reader.Peek();
            while (reader.Peek() > 0)
            {
                m = m + 1;
                string str = reader.ReadLine();
                if (m >= n + 1)
                {
                    if (m == n + 1) //如果是字段行，则自动加入字段。    
                    {
                        MatchCollection mcs = Regex.Matches(str, csvSplitBy);
                        foreach (Match mc in mcs)
                        {
                            dt.Columns.Add(mc.Value); //增加列标题    
                        }

                    }
                    else
                    {
                        MatchCollection mcs = Regex.Matches(str, "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                        i = 0;
                        System.Data.DataRow dr = dt.NewRow();
                        foreach (Match mc in mcs)
                        {
                            dr[i] = mc.Value;
                            i++;
                        }
                        dt.Rows.Add(dr);  //DataTable 增加一行         
                    }

                }
            }
            string excelJson = DataTableToJson(dt);
            return excelJson;
        }




        public static string DataTableToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = string.Format(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append("\""+strValue + "\",");
                    }
                    else
                    {
                        jsonString.Append("\""+strValue+"\"");
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
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