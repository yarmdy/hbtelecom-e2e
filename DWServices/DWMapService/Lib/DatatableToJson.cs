using System;
using System.Web;
using System.Data;
using System.Linq;
using System.Text;

namespace DWMapService.Lib
{
    public class DatatableToJson
    {
        public static string GetJsonByDataset(DataTable dt)
        {
            if (dt.Rows == null || dt.Rows.Count <= 0)
            {
                //如果查询到的数据为空则返回标记ok:false
                return "{\"ok\":false}";
            }
            bool isnull = true;

            if (dt.Rows.Count > 0)
            {
                isnull = false;
            }

            if (isnull)
            {
                return "{\"ok\":false}";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"ok\":true,");
            sb.Append(string.Format("\"{0}\":[", dt.TableName));
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sb.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        sb.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sb.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
            }
            sb.Append("],");
            sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append("}");
            return sb.ToString();
        }
       
    }
}
