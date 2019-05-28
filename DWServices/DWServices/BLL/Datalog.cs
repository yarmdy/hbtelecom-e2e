using DWServices.Common;
using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.BLL
{
    public class Datalog
    {
        public DataTable GetDatalog(String time)
        {
            DataTable datalog = null;
            DateTime dt = DateTime.Now.AddDays(-1);
            if (!String.IsNullOrEmpty(time))
                dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
            DateTime endtime = dt.AddDays(1);
            DataTable quootadata = OraConnect.ReadData("select quotaproj,thequota,mfield,mykey from DATA_QUOTA");
            datalog = new DataTable();
            String tableName="";
            String timefild = "";

            datalog.Columns.Add("city");
            datalog.Columns.Add("quota");
            datalog.Columns.Add("time");
            datalog.Columns.Add("datacount");

            for (int i = 0; i < quootadata.Rows.Count; i++)
            {
                String projstr = quootadata.Rows[i]["quotaproj"].ToString();
                String quota = quootadata.Rows[i]["mykey"].ToString();
                String field = quootadata.Rows[i]["mfield"].ToString();
                if (projstr == "覆盖问题")
                {
                    tableName = "DATA_MR";
                    timefild = "sdate";
                }
                else if (projstr == "KPI指标" || projstr == "容量问题（小区自忙时）")
                {
                    tableName = "DATA_KPIINFO";
                    timefild = "time";
                }
                else{}
                DataTable temp = OraConnect.ReadData("select city,count(*) as counts from " + tableName + " where " + field + " is not null and " + timefild + ">=to_date('"
                    + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and " + timefild + "<to_date('" + endtime.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') group by city");
                if (temp == null)
                    continue;
                for (int j = 0; j < temp.Rows.Count; j++)
                {
                    DataRow row= datalog.NewRow();
                    row["city"] = GetCityName(temp.Rows[j]["city"].ToString());
                    row["quota"] = quota;
                    row["time"] = time;
                    row["datacount"] = temp.Rows[j]["counts"];
                    datalog.Rows.Add(row);
                }
            }
            return datalog;
        }
        private string GetCityName(string city) {
            var arr = (new string[]{ "邢台", "保定", "承德", "沧州", "唐山", "邯郸", "衡水", "廊坊", "石家庄", "张家口", "秦皇岛" }).ToList();
            var arr2 = new string[]{ "XINGTAI", "BAODING", "CHENGDE", "CANGZHOU", "TANGSHAN", "HANDAN", "HENGSHUI", "LANGFANG", "SHIJIAZHUANG", "ZHANGJIAKOU", "QINHUANGDAO" }.ToList();
            if (arr.IndexOf(city)<0)
            {
                var indx = arr2.IndexOf(city);
                if (indx >= 0) { 
                    city=arr[indx];
                }
            }
            return city;
        }
        public DataTable GetDataTable(DataTable data)
        {
            DataTable result = new DataTable();
            result.Columns.Add("CITY");
            DataTable quootadata = OraConnect.ReadData("select quotaproj,thequota,mfield,mykey from DATA_QUOTA");
            for (int i = 0; i < quootadata.Rows.Count; i++)
            {
                String quota = quootadata.Rows[i]["mykey"].ToString();
                if (!result.Columns.Contains(quota))
                {
                    result.Columns.Add(quota);
                }
            }
            String[] arr = { "邢台", "保定", "承德", "沧州", "唐山", "邯郸", "衡水", "廊坊", "石家庄", "张家口", "秦皇岛" };
            for (int i = 0; i < arr.Length; i++)
            {
                DataRow row= result.NewRow();
                for (int j = 0; j < result.Columns.Count; j++)
                {
                    String columnname = result.Columns[j].ColumnName;
                    if (columnname == "CITY")
                        row["CITY"] = arr[i];
                    else
                    {
                        try {
                            DataRow[] rows = data.Select("city='" + arr[i] + "' and quota='" + columnname+"'");
                            if(rows.Length>0)
                                row[columnname] = rows[0]["datacount"];
                            else
                                row[columnname] = 0;
                        }
                        catch
                        {
                            row[columnname] = 0;
                        }
                    }
                }
                result.Rows.Add(row);
            }
            return result;
        }
    }
}