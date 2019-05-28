using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace DWWinService
{
    class NBAnalyzer:Analyzer
    {
        public override void DoQuarter()
        {
            
        }
        public override string[] DownloadCsv(string time)
        {
            List<string> tempFiles = new List<string>();
            Csv[] csvs = cfg.csvs.ToArray();
            for (int i = 0; i < csvs.Length; i++)
            {
                FileServer fileServer = csvs[i].fileserver;
                Downloader down = new Downloader(fileServer);
                string[] files = down.filelist;
                if (i==0) {
                    time = DateTime.Now.Date.ToString("yyyyMMdd");
                } else {
                    time = DateTime.Now.Date.AddDays(-1).ToString("yyyyMMdd");
                }
                files = FilterFile(files, time);
                var tmpFiles = files.Select(a => Path.Combine(csvs[i].filetmp.path, a)).ToArray();
                down.download(files, tmpFiles);

                tempFiles.AddRange(tmpFiles);
            }
            return tempFiles.ToArray();
        }
        public override void AfterDownloadCsv()
        {
            var zipfiles = TmpFiles.Where(a=>a.IndexOf(".zip")>=0).ToArray();
            foreach (var file in zipfiles) {
                ZIP.unzip(file,file.Substring(0,file.LastIndexOf('\\')+1));
                File.Delete(file);
            }
        }
        public override DataTable Analysis(bool bQuarter, string[] files)
        {
            DataTable dt = DB.QueryAsDt("select * from " + (bQuarter ? cfg.dbtables.table_min : cfg.dbtables.table_day) + " where rownum<1");
            

            StringBuilder sb = new StringBuilder("\r\n");
            foreach (var file in files) {
                if (file.IndexOf("NB") < 0) {
                    continue;
                }
                if (file.ToUpper().IndexOf(cfg.csvs[0].filetmp.path.ToUpper()) >= 0)
                {
                    var at = getDateFromXls(file);
                    foreach (DataRow dr in at.Rows)
                    {
                        if (dr != null && (double)dr[6] >= 50 && ((double)dr[6] / (double)dr[7]) <= 0.93)
                        {
                            string sql = "select city from V_WORKPARAMETER where SC_ENBID = " + dr[1] + " and rownum = 1";
                            var city = DB.GetV(sql);
                            if (city == null) { continue; }
                            //sb.Append(DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")
                            //    + "," + dr[1]
                            //    + "," + dr[3]
                            //    + "," + ((O2.O2I(dr[1])) * 256 + (O2.O2I(dr[3])))
                            //    + "," + city
                            //    + "," + ""
                            //    + "," + dr[4]
                            //    + "," + ("连接建立成功次数：" + dr[6] + "；连接建立请求次数：" + dr[7] + "；连接成功率：" + ((double)dr[6] / (double)dr[7]))
                            //    + "," + "NBIOT"
                            //    + "," + "RRC连接成功率"
                            //    + "," + "连接成功率低"
                            //    + "\r\n"
                            //    );
                            var dat = dt.NewRow();
                            dat["CREATETIME"] = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                            dat["SC_ENBID"] =dr[1];
                            dat["SC_LCRID"] =dr[3];
                            dat["ECI"] =((O2.O2I(dr[1])) * 256 + (O2.O2I(dr[3])));
                            dat["CITY"] =city;
                            dat["MANUFACYOR"] ="";
                            dat["CELLNAME"] =dr[4];
                            dat["KQIINFO"] ="连接建立成功次数：" + dr[6] + "；连接建立请求次数：" + dr[7] + "；连接成功率：" + ((double)dr[6] / (double)dr[7]);
                            dat["KQIINDEX"] ="NBIOT";
                            dat["REASON"] ="RRC连接成功率";
                            dat["MEASURES"] ="连接成功率低";
                            dat["SUCCOUNT"] =dr[6];
                            dat["ALLCOUNT"] =dr[7];
                            dat["SUCRATE"] =((double)dr[6] / (double)dr[7]);
                            dt.Rows.Add(dat);
                        }
                    }
                }
                else if (file.ToUpper().IndexOf(cfg.csvs[1].filetmp.path.ToUpper()) >= 0)
                {
                    DataTable at = getDateFromCsv(file);
                    foreach (DataRow dr in at.Rows)
                    {
                        if (dr != null && double.Parse(dr[8].ToString()) >= 50 && double.Parse(dr[9].ToString()) <= 0.93)
                        {
                            //sb.Append(DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")
                            //    + "," + dr[3]
                            //    + "," + dr[5]
                            //    + "," + ((double.Parse(dr[3].ToString())) * 256 + (double.Parse(dr[5].ToString())))
                            //    + "," + dr[1]
                            //    + "," + ""
                            //    + "," + dr[6]
                            //    + "," + ("连接建立成功次数：" + dr[8] + "；连接建立请求次数：" + dr[7] + "；连接成功率：" + dr[9])
                            //    + "," + "NBIOT"
                            //    + "," + "RRC连接成功率"
                            //    + "," + "连接成功率低"
                            //    + "\r\n"
                            //    );
                            var dat = dt.NewRow();
                            dat["CREATETIME"] = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                            dat["SC_ENBID"] =dr[3];
                            dat["SC_LCRID"] =dr[5];
                            dat["ECI"] =(double.Parse(dr[3].ToString())) * 256 + (double.Parse(dr[5].ToString()));
                            dat["CITY"] =dr[1];
                            dat["MANUFACYOR"] ="";
                            dat["CELLNAME"] =dr[6];
                            dat["KQIINFO"] ="连接建立成功次数：" + dr[8] + "；连接建立请求次数：" + dr[7] + "；连接成功率：" + dr[9];
                            dat["KQIINDEX"] ="NBIOT";
                            dat["REASON"] ="RRC连接成功率";
                            dat["MEASURES"] ="连接成功率低";
                            dat["SUCCOUNT"] =dr[8];
                            dat["ALLCOUNT"] =dr[7];
                            dat["SUCRATE"] =dr[9];
                            dt.Rows.Add(dat);
                        }
                    }
                }
                else if (file.ToUpper().IndexOf(cfg.csvs[2].filetmp.path.ToUpper()) >= 0) {
                    DataTable at = getDateFromCsv(file);
                    foreach (DataRow dr in at.Rows)
                    {
                        if (dr != null && O2.O2D(dr[13]) >= 50 && O2.O2D(dr[12]) / O2.O2D(dr[13]) <= 0.93)
                        {
                            //sb.Append(DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")
                            //    + "," + dr[8]
                            //    + "," + dr[10]
                            //    + "," + ((O2.O2D(dr[8])) * 256 + (O2.O2D(dr[10])))
                            //    + "," + getCity(dr[8].ToString())
                            //    + "," + ""
                            //    + "," + dr[11]
                            //    + "," + ("连接建立成功次数：" + dr[12] + "；连接建立请求次数：" + dr[13] + "；连接成功率：" + O2.O2D(dr[12]) / O2.O2D(dr[13]))
                            //    + "," + "NBIOT"
                            //    + "," + "RRC连接成功率"
                            //    + "," + "连接成功率低"
                            //    + "\r\n"
                            //    );
                            var city = getCity(dr[8].ToString());
                            if (city == "") {
                                continue;
                            }
                            var dat = dt.NewRow();
                            dat["CREATETIME"] = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                            dat["SC_ENBID"] =dr[8];
                            dat["SC_LCRID"] =dr[10];
                            dat["ECI"] =(O2.O2D(dr[8])) * 256 + (O2.O2D(dr[10]));
                            dat["CITY"] = city;
                            dat["MANUFACYOR"] ="";
                            dat["CELLNAME"] =dr[11];
                            dat["KQIINFO"] ="连接建立成功次数：" + dr[12] + "；连接建立请求次数：" + dr[13] + "；连接成功率：" + O2.O2D(dr[12]) / O2.O2D(dr[13]);
                            dat["KQIINDEX"] ="NBIOT";
                            dat["REASON"] ="RRC连接成功率";
                            dat["MEASURES"] ="连接成功率低";
                            dat["SUCCOUNT"] =dr[12];
                            dat["ALLCOUNT"] =dr[13];
                            dat["SUCRATE"] = O2.O2D(dr[12]) / O2.O2D(dr[13]);
                            dt.Rows.Add(dat);
                        }
                    }
                }
            }
            //var filename = cfg.custom["zjpath"] + "KQI_" + DateTime.Now.AddDays(-1).Date.ToString("yyyyMMdd") + ".csv";
            //FileStream fs = new FileStream(filename,FileMode.Append);
            //StreamWriter sw = new StreamWriter(fs,Encoding.GetEncoding("gbk"));
            //sw.Write(sb.ToString());
            //sw.Flush();
            //sw.Close();
            //fs.Close();
            return dt;
        }
       
        public DataTable getDateFromXls(string filename) {
            var constr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filename + ";" + "Extended Properties=Excel 12.0;";
            OleDbConnection con = new OleDbConnection(constr);
            con.Open();
            var tb = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            con.Close();
            var tbn = tb.Rows[0]["TABLE_NAME"].ToString();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter("select * from [" + tbn + "]", con);

            da.Fill(dt);
            return dt;
        }
        public DataTable getDateFromCsv(string filename) 
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        //public DataTable getDateFromCsv2(string filename)
        //{
        //    DataTable dt = new DataTable();
        //    FileStream fs = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //    StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
        //    //记录每次读取的一行记录
        //    string strLine = "";
        //    //记录每行记录中的各字段内容
        //    string[] aryLine;
        //    //标示列数
        //    int columnCount = 0;
        //    //标示是否是读取的第一行
        //    bool IsFirst = true;
        //    while ((strLine = sr.ReadLine()) != null)
        //    {
        //        aryLine = strLine.Split(',');
        //        if (IsFirst == true)
        //        {
        //            IsFirst = false;
        //            columnCount = aryLine.Length;
        //            //创建列
        //            for (int i = 0; i < columnCount; i++)
        //            {
        //                DataColumn dc = new DataColumn(aryLine[i]);
        //                dt.Columns.Add(dc);
        //            }
        //        }
        //        else
        //        {
        //            DataRow dr = dt.NewRow();
        //            for (int j = 0; j < columnCount; j++)
        //            {
        //                dr[j] = aryLine[j];
        //            }
        //            dt.Rows.Add(dr);
        //        }
        //    }

        //    sr.Close();
        //    fs.Close();
        //    return dt;
        //}
        public override string[] TimeStr
        {
            get
            {
                DateTime dt = DateTime.Now;
                string min = dt.ToString("yyyyMMddHH");
                var minute = (dt.Minute / 15) * 15;
                min += minute.ToString().PadLeft(2, '0');
                string day = dt.ToString("yyyyMMdd");
                return new string[] { min, day };
            }
        }
        public override string[] TmpFiles
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var filec in cfg.csvs)
                {
                    if (filec.filetmp.path != "")
                    {
                        var files = Directory.GetFiles(filec.filetmp.path, "*.*");
                        foreach (var file in files)
                        {
                            res.Add(file);
                        }
                    }
                }
                return res.Distinct().ToArray();
            }
        }
        private string getCity(string enbid) {
            string sql = "select city from V_WORKPARAMETER t where sc_enbid='" + enbid + "' and rownum=1";
            var o= DB.GetV(sql);
            return o == null ? "" : o.ToString();
        }
    }
}

