using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DWMapService.Controllers
{
    public class DB
    {
        public static string ConnectStr { get {
            return System.Configuration.ConfigurationManager.AppSettings["ConnectString"]; ;
        } }
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
            DataTable ds = new DataTable();
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
    }
}