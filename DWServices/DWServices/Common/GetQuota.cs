using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.Common
{
    public class GetQuota
    {
        public static List<String> getAllQuotaproj()
        {
            List<String> list = new List<string>();
            DataTable data = OraConnect.ReadData("select distinct t.quotaproj from DATA_QUOTA t");
            for (int i = 0; i < data.Rows.Count; i++)
            {
                String value = data.Rows[i]["quotaproj"].ToString();
                list.Add(value);
            }
            return list;
        }

        public static List<String> getAllQuota()
        {
            List<String> list = new List<string>();
            DataTable data = OraConnect.ReadData("select t.thequota from DATA_QUOTA t");
            for (int i = 0; i < data.Rows.Count; i++)
            {
                String value = data.Rows[i]["thequota"].ToString();
                list.Add(value);
            }
            return list;
        }
        public static List<String> getAllQuota(String quotaproj)
        {
            List<String> list = new List<string>();
            DataTable data = OraConnect.ReadData("select t.thequota from DATA_QUOTA t  where t.quotaproj='" + quotaproj + "'");
            for (int i = 0; i < data.Rows.Count; i++)
            {
                String value = data.Rows[i]["thequota"].ToString();
                list.Add(value);
            }
            return list;
        }

        public static DataTable getAllQuotainfo()
        {
            DataTable data = OraConnect.ReadData("select * from DATA_QUOTA t");
            return data;
        }
        public static DataTable getAllQuotainfo(String quotaproj,String eci)
        {
            String sql = "select * from DATA_QUOTA t  where t.quotaproj='" + quotaproj + "'";
            if (quotaproj == "容量问题（小区自忙时）")
            {
                if (ECIAndIDConvert.getflowclass(eci))
                    sql = "select * from DATA_QUOTA t  where t.quotaproj='" + quotaproj + "' and SUBQUOTAPROJ='800M'";
                else
                    sql = "select * from DATA_QUOTA t  where t.quotaproj='" + quotaproj + "' and SUBQUOTAPROJ='1800M'";
            }
            DataTable data = OraConnect.ReadData(sql);;
            return data; ;
        }
        public static DataTable getQuotainfo(String mykey)
        {
            DataTable data = OraConnect.ReadData("select * from DATA_QUOTA t where mykey='" + mykey + "'");
            return data;
        }

        public static String maxValue(String quotaStr)
        {
            List<Object> list = OraConnect.GetRecord("select t.maxvalue from DATA_QUOTA t where t.thequota='" + quotaStr + "'");
            String value=null;
            if (list.Count > 0)
            {
                value = list[0].ToString();
            }
            if (value != null)
                return value;
            else
                return null;
        }

        public static String minValue(String quotaStr)
        {
            List<Object> list = OraConnect.GetRecord("select t.minvalue from DATA_QUOTA t where t.thequota='" + quotaStr + "'");
            String value = null;
            if (list.Count > 0)
            {
                value = list[0].ToString();
            }
            if (value != null)
                return value;
            else
                return null;
        }
    }
}