using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;

namespace DWWinService
{
    public class DB
    {
        public static string ConnectStr { get; set; }
        public static DataSet Query(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                using (OracleConnection con = new OracleConnection(ConnectStr))
                {
                    OracleDataAdapter da = new OracleDataAdapter(sql, con);
                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(" + sql + ")");
            }

            return ds;
        }
        public static DataTable QueryAsDt(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                using (OracleConnection con = new OracleConnection(ConnectStr))
                {
                    OracleDataAdapter da = new OracleDataAdapter(sql, con);
                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(" + sql + ")");
            }

            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }
        public static int Exec(string sql)
        {
            var res = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(ConnectStr))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand(sql, con);
                    res = com.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(" + sql + ")");
            }
            return res;
        }
        public static object GetV(string sql)
        {
            object o = null;
            try
            {
                var ds = new DataSet();
                using (OracleConnection con = new OracleConnection(ConnectStr))
                {
                    OracleDataAdapter da = new OracleDataAdapter(sql, con);
                    da.Fill(ds);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        o = ds.Tables[0].Rows[0][0];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "(" + sql + ")");
            }
            return o;
        }
        public static bool ImportDt(DataTable dt)
        {
            bool b = false;
            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(DB.ConnectStr))
            {
                Oracle.DataAccess.Client.OracleBulkCopy bulkCopy = null;
                try
                {
                    connection.Open();
                    bulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(connection, Oracle.DataAccess.Client.OracleBulkCopyOptions.UseInternalTransaction);
                    //bulkCopy.BatchSize = 1000;

                    bulkCopy.DestinationTableName = dt.TableName;
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        bulkCopy.BulkCopyTimeout = 1200;
                        bulkCopy.WriteToServer(dt);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "(导入" + (dt == null ? 0 : dt.Rows.Count) + "条数据)");
                }
                finally
                {
                    connection.Close();
                    if (bulkCopy != null)
                        bulkCopy.Close();
                }
            }
            b = true;
            return b;
        }
    }
}
