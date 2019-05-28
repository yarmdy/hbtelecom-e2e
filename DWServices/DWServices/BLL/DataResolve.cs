using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.BLL
{
    public class DataResolve
    {
        public DataTable GetData(string time, string time2) {
            string sql = @"SELECT CREATETIME, CELLNAME, ECI, CITY, CASE STATE WHEN 0 THEN '新增' WHEN 1 THEN '未解决' when 2 then '已解决' ELSE '解决中' END AS STATE, TYPE FROM PDTABLE WHERE CREATETIME >= to_date('" + time + "', 'yyyy-mm-dd') AND CREATETIME <= to_date('" + time2 + "', 'yyyy-mm-dd') order by PDTABLE.state";
            DataTable table = OraConnect.ReadData(sql);
            table.Columns.Add("QUERY", typeof(string));
            foreach (DataRow dr in table.Rows)
                dr["QUERY"] = "<a class='query_rate' style='cursor:pointer;' data-toggle='modal' data-target='#myModal'>查询</a>";
            return table;
        }

        public DataTable GetDataRate(int eci, string type) { 
            string sql = "select * from UNSOLVED_INFO where ECGI = " + eci + " AND TYPE='" + type+"' order by GTIME";
            DataTable table = OraConnect.ReadData(sql);
            return table;
        }
    }
}