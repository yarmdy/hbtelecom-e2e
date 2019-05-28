using Com.Netframe.DatabaseUtil;
using Com.Netframe.Helpers;
using Com.Netframe.Models;
using DWServices.BLL;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            //try
            //{
            //    OdpHelper _DatabaseUtility = null;
            //    List<Connection> dbConnectionList = null;
            //    Connection ghmisDbConnection = null;
            //    if (_DatabaseUtility == null)
            //    {
            //        dbConnectionList = XmlSerializer.Deserialize<List<Connection>>("config\\database-dwdb.xml", "DbConnectionList");
            //        if (dbConnectionList != null && dbConnectionList.Count > 0)
            //        {
            //            ghmisDbConnection = dbConnectionList[0];
            //        }
            //        else
            //        {
            //            LogHelper.WriteErrorLog("不能读取到数据库配置文件，路径为：" +"config\\database-dwdb.xml.xml");
            //        }
            //        _DatabaseUtility = new OdpHelper(ghmisDbConnection);
            //    }
            //    _DatabaseUtility.GetDbConnection();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            String result = "";
            try
            {
                try
                {
                    Analysis analysis = new Analysis();
                    if (analysis.AnalysisAll())
                        result = "分析结束！";
                    else
                        result = "分析失败！请查看日志。";
                }
                catch (Exception err)
                {
                    LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                    result = err.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
