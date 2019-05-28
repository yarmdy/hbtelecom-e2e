using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    class IpAnalyzer : Analyzer
    {
        public override string[] TmpFiles
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var filec in cfg.csvs)
                {
                    if (filec.filetmp.path != "")
                    {
                        var files = Directory.GetFiles(filec.filetmp.path, "*.txt");
                        foreach (var file in files)
                        {
                            if(file.ToLower().Contains("ping_")||file.ToLower().Contains("flux_"))
                            res.Add(file);
                        }
                    }
                }
                return res.Distinct().ToArray();
            }
        }

        public override string[] TargetFiles
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var filec in cfg.csvs)
                {
                    if (filec.filetarget.path != "")
                    {
                        var files = Directory.GetFiles(filec.filetarget.path, "*.txt");
                        foreach (var file in files)
                        {
                            res.Add(file);
                        }
                    }
                }
                return res.Distinct().ToArray();
            }
        }

        public override DataTable Analysis(bool bQuarter, string[] files)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            using (OracleConnection con = new OracleConnection(DB.ConnectStr))
            {
                OracleDataAdapter da = new OracleDataAdapter("select * from IPRAN_DAY where rownum<1", con);
                da.Fill(ds);
                dt = ds.Tables[0];
            }

            foreach (var file in files)
            {
                var filestr = File.ReadAllText(file, Encoding.GetEncoding("gb2312"));
                filestr = FileHead + filestr;
                var filelines = filestr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string type = getTypeAndTime(file)[0];
                string date = getTypeAndTime(file)[1];
                for (int i = 0; i < filelines.Length; i++)
                {
                    var row = dt.NewRow();
                    string city = filelines[i].Split(':')[0];
                    string ip = filelines[i].Split(':')[1];
                    var temp = dt.Select("CITY='" + city + "' and IP='" + ip+"'");
                    if (temp.Length > 0) continue;
                    row["CITY"] = city;
                    row["IP"] = ip;
                    row["KIND"] = type;
                    row["CREATEDATE"] = DateTime.Parse(date);
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        public string[] getTypeAndTime(string file)
        {
            string name = file.Split('\\')[file.Split('\\').Length - 1].Split('.')[0];
            string date = "";
            string type = "";
            if (name.ToLower().Contains("ping"))
            {
                type = "Aping";
                date = name.Substring(5).Insert(4, "-").Insert(7, "-");
            }
            else
            {
                type = "Aliuliang";
                date = name.Substring(5).Insert(4, "-").Insert(7, "-");
            }
            return new string[] { type, date };
        }

        public override void AfterImportDBDay(string time)
        {
            time = time.Insert(4, "-").Insert(7, "-");
            string week = DateTime.Parse(time).AddDays(-6).ToString("yyyy-MM-dd");
            string sql = @"select distinct a.IPRAN_A IP from
                    (select g.IPRAN_A,count(1) ratenum
                     from
                     KQI85_DAY k join V_WORKPARAMETER g on k.ecgi = g.eci
                     where k.START_TIME >= trunc(sysdate-1) and k.START_TIME < trunc(sysdate)
                   and g.ipran_a is not null
                     group by g.Ipran_a
                      ) a
                     join 
                     (select g.IPRAN_A,count(g.eci) ECINUM from V_WORKPARAMETER g
                  where g.ipran_a is not null
                     group by g.IPRAN_A) b
                     on a.IPRAN_A = b.IPRAN_A where a.ratenum/b.ECINUM > 0.8 and a.ratenum>1";
            DataSet dsq = DB.Query(sql);
            DataTable dt = dsq.Tables[0];
            dt.Columns.Add("CREATEDATE",typeof(DateTime));
            dt.Columns.Add("KIND");
            dt.Columns.Add("CITY");
            foreach (DataRow row in dt.Rows)
            {
                row["CREATEDATE"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                row["KIND"] = "Arate";
            }
            Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(DB.ConnectStr);
            Oracle.DataAccess.Client.OracleBulkCopy bulkCopy = null;
            try
            {
                connection.Open();
                bulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(connection, Oracle.DataAccess.Client.OracleBulkCopyOptions.UseInternalTransaction);
                bulkCopy.DestinationTableName = "IPRAN_DAY";
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
                throw ex;
            }
            finally
            {
                connection.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }
            string sqlDay = "select count(0) from IPRAN_DAY t  where trunc(t.createdate)=to_date('" + time + "', 'yyyy-mm-dd')";
            string sqlWeek = "select count(*) count from (select IP from IPRAN_DAY t  where trunc(t.createdate) >= to_date('" + week + "', 'yyyy-mm-dd') and  trunc(t.createdate) <= to_date('" + time + "', 'yyyy-mm-dd') group by t.IP HAVING count(t.IP) >= 3 and max(trunc(t.createdate))=to_date('" + time + "', 'yyyy-mm-dd')) t";
            DataSet ds = DB.Query(sqlDay);
            int numDay = O2.O2I(ds.Tables[0].Rows[0][0]);
            string sqlSaveDay = "update DATA_DISPLAY set DATA_COUNT = " + numDay + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'RAN' and DATA_STATUS = 'DAY'";
            DB.Exec(sqlSaveDay);
            ds = DB.Query(sqlWeek);
            int numWeek = O2.O2I(ds.Tables[0].Rows[0][0]);
            string sqlSaveWeek = "update DATA_DISPLAY set DATA_COUNT = " + numWeek + ",DATA_TIME = to_date('" + time + "', 'yyyy-mm-dd hh24:mi:ss') where DATA_NAME = 'RAN' and DATA_STATUS = 'WEEK'";
            DB.Exec(sqlSaveWeek);
        }
    }
}
