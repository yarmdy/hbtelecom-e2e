using Com.Netframe.Models;
using Com.Netframe.DatabaseUtil;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Com.Netframe.Helpers;

namespace DWServices.DAL
{
    public class OraConnect
    {
        private static OracleConnection m_oracleConnection;
        private static OdpHelper _DatabaseUtility = null;
        static object obj = new object();
        public OraConnect() { }
        public OraConnect(OracleConnection oracleConnection)
        {
            m_oracleConnection = oracleConnection;
        }

        private static OracleConnection GetConnection()
        {
            if (m_oracleConnection == null)
            {
                m_oracleConnection = GetOracleConnection();
                if (m_oracleConnection == null)
                    return null;
                try
                {
                    if (m_oracleConnection.State == System.Data.ConnectionState.Closed)
                    {
                        m_oracleConnection.Open();
                    }
                    m_oracleConnection.Close();
                    return m_oracleConnection;
                }
                catch (Exception ex)
                {
                    m_oracleConnection.Close();
                    return null;
                }
            }
            else
                return m_oracleConnection;
        }

        /// <summary>
        /// 获取Oracle数据库连接
        /// </summary>
        public static OracleConnection GetOracleConnection()
        {
            try
            {
                List<Connection> dbConnectionList = null;
                Connection ghmisDbConnection = null;
                if (_DatabaseUtility == null)
                {
                    String configpath = "";
                    try {
                        configpath = HttpContext.Current.Server.MapPath("~/");
                    }
                    catch {
                        configpath = AssemblyHelper.GetSystemNamePath() + "\\"; 
                    }
                    dbConnectionList = XmlSerializer.Deserialize<List<Connection>>(configpath + "config\\database-dwdb.xml", "DbConnectionList");
                    if (dbConnectionList != null && dbConnectionList.Count > 0)
                    {
                        ghmisDbConnection = dbConnectionList[0];
                    }
                    else
                    {
                        LogHelper.WriteErrorLog("不能读取到数据库配置文件，路径为：" + configpath + "config\\database-dwdb.xml.xml");
                    }
                    _DatabaseUtility = new OdpHelper(ghmisDbConnection);
                }
                return _DatabaseUtility.GetDbConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ExecuteSQL(string sql)
        {
            if (m_oracleConnection == null)
                GetConnection();
            if (m_oracleConnection.State != ConnectionState.Open)
                m_oracleConnection.Open();
            try
            {
                OracleCommand command = new OracleCommand(sql, m_oracleConnection);
                int i = command.ExecuteNonQuery();
                if (i > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public static bool ExecuteBatchSQL(List<string> listsql)
        {
            List<string> m_listsql = listsql;
            if (m_oracleConnection == null)
                GetConnection();
            if (m_oracleConnection == null) LogHelper.WriteErrorLog("数据库连接丢失！", null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            try
            {
                if (m_oracleConnection.State != ConnectionState.Open)
                    m_oracleConnection.Open();
                for (int i = 0; i < m_listsql.Count; i++)
                {
                    string sql = m_listsql[i].ToString();
                    OracleCommand command = new OracleCommand(sql, m_oracleConnection);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex, null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return false;
        }

        public static DataTable ReadData(string sql)
        {
            if (m_oracleConnection == null)
                GetConnection();
            if (m_oracleConnection == null)
            {
                LogHelper.WriteErrorLog("数据库连接丢失！", null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                return null;
            }
            try
            {
                if (m_oracleConnection.State != ConnectionState.Open)
                    m_oracleConnection.Open();
                DataSet ds = new DataSet();
                OracleDataAdapter adapt = new OracleDataAdapter(sql, m_oracleConnection);
                adapt.Fill(ds);
                if (ds.Tables.Count <= 0) return null;
                ds.Tables[0].TableName = "table";
                return ds.Tables[0];
            }
            catch (Exception err)
            {
                LogHelper.WriteErrorLog(err, null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return null;
        }

        #region 执行SQL语句，返回记录第一条记录
        /// 执行SQL语句，返回记录总数数  
        /// sql语句  
        /// 返回记录总条数  
        public static List<Object> GetRecord(string sql)
        {
            List<Object> result = new List<object>();
            if (m_oracleConnection == null)
                GetConnection();
            if (m_oracleConnection == null)
            {
                LogHelper.WriteErrorLog("数据库连接丢失！", null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                return null;
            }
            if (m_oracleConnection == null)
                return null;
            m_oracleConnection.Open();
            OracleCommand command;
            OracleDataReader dataread = null;
            try
            {
                command = new OracleCommand(sql, m_oracleConnection);
                dataread = command.ExecuteReader();
                dataread.Read();
                result.Add(dataread[0]);
                dataread.Close();
            }
            catch (Exception ex)
            {
                if (dataread != null)
                    dataread.Close();
                LogHelper.WriteErrorLog(ex, null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return result;
        }

        public static DataTable GetOneRecord(string sql)
        {
            DataTable result = new DataTable();
            if (m_oracleConnection == null)
                GetConnection();
            if (m_oracleConnection == null)
            {
                LogHelper.WriteErrorLog("数据库连接丢失！", null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                return null;
            }
            if (m_oracleConnection == null)
                return null;
            m_oracleConnection.Open();
            OracleCommand command;
            OracleDataReader dataread = null;
            try
            {
                command = new OracleCommand(sql, m_oracleConnection);
                dataread = command.ExecuteReader();
                dataread.Read();
                result.Load(dataread);
                dataread.Close();
            }
            catch (Exception ex)
            {
                if (dataread != null)
                    dataread.Close();
                LogHelper.WriteErrorLog(ex, null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return result;
        }
        #endregion


        public bool SExecuteBatchSQL(List<string> listsql)
        {

            if (m_oracleConnection == null)
                GetConnection();
            if (m_oracleConnection == null)
            {
                LogHelper.WriteErrorLog("数据库连接丢失！", null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                return false;
            }
            try
            {
                if (m_oracleConnection.State == System.Data.ConnectionState.Closed)
                {
                    m_oracleConnection.Open();
                }
                for (int i = 0; i < listsql.Count; i++)
                {
                    string sql = listsql[i].ToString();
                    if (string.IsNullOrEmpty(sql))
                        continue;
                    OracleCommand command = new OracleCommand(sql, m_oracleConnection);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteErrorLog(ex, null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex, null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                return false;
            }
        }
        public DataTable SReadData(string sql)
        {
            OracleConnection m_pConnection = GetConnection();
            if (m_pConnection == null)
                return null;
            if (m_pConnection.State != ConnectionState.Open)
                m_pConnection.Open();
            try
            {
                DataSet ds = new DataSet();
                OracleDataAdapter adapt = new OracleDataAdapter(sql, m_pConnection);
                adapt.Fill(ds, "table");
                return ds.Tables["table"];
            }
            catch (Exception err)
            {
                LogHelper.WriteErrorLog(err, null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
            }
            return null;
        }
        public static bool CloseDatabase()
        {
            if (m_oracleConnection != null)
            {
                if (m_oracleConnection.State == System.Data.ConnectionState.Open)
                    m_oracleConnection.Close();
            }
            return true;
        }
    }
}