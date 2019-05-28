using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CTCCGoods.Controllers
{
    public class DB
    {
        private SqlConnection conn=null;
        private SqlTransaction tran=null;
        private SqlCommand comm = null;
        private SqlDataAdapter dad = null;
        public DB() {
            conn = new SqlConnection(constr);
            comm = new SqlCommand();
            comm.Connection = conn;
            dad = new SqlDataAdapter(comm);
            conn.Open();
            tran = conn.BeginTransaction();
            comm.Transaction = tran;
        }
        public int Insertobj(string sql) {
            var res = -1;
            try {
                comm.CommandText = sql + " select isnull(@@IDENTITY,-1) id";
                DataTable dt = new DataTable();
                dad.Fill(dt);
                if (dt == null||dt.Rows.Count<=0) {
                    tran.Rollback();
                    conn.Close();
                    throw new Exception("插入失败");
                }
                res = O2.O2I(dt.Rows[0][0]);
            }
            catch(Exception ex) {
                tran.Rollback();
                conn.Close();
                throw ex;
            }
            return res;
        }
        public int Execobj(string sql) {
            var res = -1;
            try
            {
                comm.CommandText = sql;
                res=comm.ExecuteNonQuery();
            }
            catch (Exception ex) {
                tran.Rollback();
                conn.Close();
                throw ex;
            }
            return res;
        }
        public DataTable Queryobj(string sql) {
            var dt = new DataTable();
            try
            {
                comm.CommandText = sql;
                dad.Fill(dt);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                conn.Close();
                throw ex;
            }
            return dt;
        }
        public Dictionary<string, object>[] QueryAsDicsobj(string sql)
        {
            var dt = Queryobj(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            return dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
        }
        public void End(bool commit) {
            try
            {
                if (commit)
                {
                    tran.Commit();
                }
                else
                {
                    tran.Rollback();
                }
                conn.Close();
            }
            catch { }
        }
        static string constr {
            get { 
                return ConfigurationManager.ConnectionStrings["mssql"].ConnectionString;
            }
        }
        public static DataTable Query(string sql) {
            try
            {
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(constr);
                SqlDataAdapter da = new SqlDataAdapter(sql,con);
                da.Fill(dt);
                return dt;
            }
            catch {
                return null;
            }
        }
        public static int Exec(string sql) {
            try {
                SqlConnection con = new SqlConnection(constr);
                SqlCommand com = new SqlCommand(sql,con);
                con.Open();
                var count=com.ExecuteNonQuery();
                con.Close();
                return count;
            }
            catch {
                return -1;
            }
        }
        public static int Insert(string sql) {
            var res = -1;
            sql = sql + " select isnull(@@IDENTITY,-1) id";
            var dt = Query(sql);
            if (dt == null || dt.Rows.Count <= 0) {
                return res;
            }
            res = O2.O2I(dt.Rows[0][0]);
            return res;
        }
        public static Dictionary<string,object>[] QueryAsDics(string sql) {
            var dt = Query(sql);
            if (dt == null||dt.Rows.Count<=0) {
                return null;
            }
            return dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
        }
        public static Dictionary<string, object> QueryOne(string sql) {
            var dt = Query(sql);
            if (dt == null || dt.Rows.Count<=0)
            {
                return null;
            }
            return dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).FirstOrDefault();
        }
        public static DataTable GetXlsxData(string filename)
        {
            var constr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filename + ";" + "Extended Properties=Excel 12.0;";
            System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(constr);
            con.Open();
            var tb = con.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
            con.Close();
            var tbn = tb.Rows[0]["TABLE_NAME"].ToString();
            System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter("select * from [" + tbn + "]", con);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}