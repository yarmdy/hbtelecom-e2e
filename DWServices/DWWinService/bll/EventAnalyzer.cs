using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace DWWinService
{
    class EventAnalyzer : Analyzer
    {
        public override void DoQuarter()
        {
            //do nothing
        }
        public override void AfterDownloadCsv()
        {
            //下载后解压和转换为csv删除rar文件
            var tmppath = cfg.csvs[0].filetmp.path;
            var tmpfiles = Directory.GetFiles(tmppath, "*.rar");
            var pathExe = AppDomain.CurrentDomain.BaseDirectory + "WinRAR.exe";
            try
            {
                bool hasDone = false;
                for (var i = 0; i < tmpfiles.Length; i++)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = pathExe;
                    p.StartInfo.WorkingDirectory = tmppath;
                    string cmd = string.Format("x -dr {0} {1} -y", tmpfiles[i], tmppath);
                    p.StartInfo.Arguments = cmd;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.Start();
                    p.WaitForExit();
                    if (i == tmpfiles.Length - 1 && p.HasExited)
                    {
                        hasDone = true;
                    }
                }
                if (hasDone)
                {
                    string[] xmls = Directory.GetFiles(tmppath, "*.xml");
                    string target = cfg.csvs[0].filetarget.path;
                    DateTime dtime = DateTime.Now.AddDays(-1);
                    string newdir = target + dtime.ToString("yyyyMM") + @"\";
                    if (!Directory.Exists(newdir))
                    {
                        Directory.CreateDirectory(newdir);
                    }
                    for (var i = 0; i < xmls.Length; i++)
                    {
                        Convert(xmls[i], newdir);
                    }
                    var allfiles = Directory.GetFiles(tmppath);
                    for (var i = 0; i < allfiles.Length; i++)
                    {
                        File.Delete(allfiles[i]);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public DataTable getDataFromCsv(string filename)
        {
            DataTable dt = new DataTable();
            var ckdt= DB.QueryAsDt("select * from DATA_EVENT where rownum<1");
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
            var lastnull = false;
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
                        if (columnCount - i <= 1 && string.IsNullOrWhiteSpace(aryLine[i]))
                        {
                            lastnull = true;
                            continue;
                        }
                        //DataColumn dc = new DataColumn(aryLine[i]);
                        //if (aryLine[i].ToLower().Contains("time")) {
                        //    dc.DataType = typeof(DateTime);
                        //}
                        //dt.Columns.Add(dc);
                        if (ckdt.Columns.Contains(aryLine[i].ToUpper())) {
                            DataColumn dc = new DataColumn(aryLine[i]);
                            dc.DataType = ckdt.Columns[aryLine[i].ToUpper()].DataType;
                            dt.Columns.Add(dc);
                        }
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (columnCount - j <= 1 && lastnull) {
                            continue;
                        }
                        if (string.IsNullOrEmpty(aryLine[j]))
                        {
                            dr[j] = DBNull.Value;
                        }
                        else {
                            dr[j] = aryLine[j];
                        }
                        
                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        private void Convert(string file, string target)
        {
            XDocument xd = XDocument.Load(file);
            var nodes = xd.Descendants("Message");
            DataTable dt = new DataTable();
            foreach (var node in nodes)
            {
                var row = dt.NewRow();
                foreach (var n1 in node.Elements())
                {
                    foreach (var n2 in n1.Elements())
                    {
                        //var name = n1.Name.ToString() + "_" + n2.Name.ToString();
                        var name = n2.Name.ToString();
                        var val = n2.Value;
                        if (!dt.Columns.Contains(name))
                        {
                            dt.Columns.Add(name);
                        }
                        row[name] = val;
                    }
                }
                dt.Rows.Add(row);
            }
            var csvstr = Xml2Csv(dt);
            string[] ps = file.Split('\\');
            string filename = ps[ps.Length - 1].Split('.')[0];
            if (File.Exists(target + filename + ".csv")) {
                File.Delete(target + filename + ".csv");
            }
            File.WriteAllText(target + filename + ".csv", csvstr, Encoding.GetEncoding("gbk"));
        }
        private string Xml2Csv(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn col in dt.Columns)
            {
                sb.Append(col.ColumnName + ",");
            }
            sb.Append("\r\n");
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(dr[i] + ",");
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }


        public override string[] DownloadCsv(string time)
        {
            var dcount = 0;
            FileStream fs = null;
            while (true)
            {
                try
                {
                    string downloadurl = cfg.custom["downloadurl"];
                    DateTime dtime = DateTime.Now.AddDays(-1);
                    string prefix = "河北_" + dtime.ToString("yyyyMMdd");
                    string[] filenames = {
                    prefix+"_group_06-01.rar"
                    };
                    for (int i = 0; i < filenames.Length; i++)
                    {
                        string filename = filenames[i];
                        string tempPath = cfg.csvs[0].filetmp.path;
                        string tempFile = tempPath + filename + ".temp"; //临时文件
                        if (System.IO.File.Exists(tempFile))
                        {
                            System.IO.File.Delete(tempFile);    //存在则删除
                        }
                        string url = downloadurl + dtime.ToString("yyyyMM") + @"/" + filename;

                        fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        // 设置参数
                        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                        //发送请求并获取相应回应数据
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        //直到request.GetResponse()程序才开始向目标网页发送Post请求
                        Stream responseStream = response.GetResponseStream();
                        //创建本地文件写入流
                        byte[] bArr = new byte[1024];
                        int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        while (size > 0)
                        {
                            fs.Write(bArr, 0, size);
                            size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        }
                        fs.Close();
                        fs = null;
                        responseStream.Close();
                        if (File.Exists(tempPath + filename))
                        {
                            File.Delete(tempPath + filename);
                        }
                        System.IO.File.Move(tempFile, tempPath + filename);
                    }
                    return filenames;
                }
                catch (Exception ex) {
                    if (fs != null) {
                        fs.Close();
                        fs.Dispose();
                    }
                    fs = null;
                    dcount++;
                    if (dcount >= 10) {
                        throw ex;
                    }
                    System.Threading.Thread.Sleep(1800000);
                }
            }
        }
        public override DataTable Analysis(bool bQuarter, string[] files)
        {
            var dtyue = DateTime.Now.Date.AddDays(-1);
            DateTime dtime = DateTime.Now.AddDays(-1);
            dtyue = dtyue.AddDays(-dtyue.Day + 1);
            #region 获取要分析的文件
            var tarpath = cfg.csvs[0].filetarget.path + DateTime.Now.AddDays(-1).ToString("yyyyMM") + "\\";
            var tfiles = System.IO.Directory.GetFiles(tarpath, "*.csv");
            string prefix = "河北_" + dtime.ToString("yyyyMMdd");
            string filenames = prefix + "_group_06-01.csv";
            var afiles = tfiles.Where(a => a.IndexOf(prefix) >= 0).ToArray();
            //Dictionary<string, Dictionary<string, object>> dics = new Dictionary<string, Dictionary<string, object>>();
            DataTable data = null;
            foreach (var file in afiles) {
                data= getDataFromCsv(file);
                
            }
            //data.DefaultView.RowFilter = "isnull(Longitude,'')<>'' and isnull(Latitude,'')<>'' and isnull(LteCi,'')<>''";
            data.DefaultView.RowFilter = "Longitude is not null and Latitude is not null and LteCi is not null";
            var datamap = data.DefaultView.ToTable();
            //data.DefaultView.RowFilter = "isnull(LteCi,'')<>''";
            //var datapd = data.DefaultView.ToTable();

            //var pdtongji = data.AsEnumerable().GroupBy(a => a["LteCi"] + "_" + a["EvtID"]).Select(a => new Dictionary<string, object>() { { "key", a.Key }, { "LteCi", a.Max(b => b["LteCi"]) }, { "EvtID", a.Max(b => b["EvtID"]) }, { "City", a.Max(b => b["City"]) }, { "Num", a.Count() } }).ToDictionary(a=>a["key"].ToString());
            datamap.TableName = "DATA_EVENT";
            //var datasource = DB.QueryAsDt("select * from DATA_EVENT");
            DB.ImportDt(datamap);

            #endregion
            return null;
        }
        public override bool ImportDB(DataTable dt, bool bol)
        {
            //do nothing
            return true;
        }
        public override void DeleteTmp()
        {
            //do nothing
        }

        private DataSet toDics2Ds(Dictionary<string, Dictionary<string, Dictionary<string, object>>> dics, int type)
        {
            DataSet ds = new DataSet();
            foreach (var dic in dics)
            {
                DataTable dt = new DataTable(dic.Key);
                foreach (var row in dic.Value)
                {
                    var dr = dt.NewRow();
                    foreach (var cel in row.Value)
                    {
                        if (!dt.Columns.Contains(cel.Key))
                        {
                            var tp = getColType(cel.Key);
                            if (tp == typeof(int))
                            {
                                tp = typeof(long);
                            }
                            dt.Columns.Add(cel.Key, tp);
                        }
                        dr[cel.Key] = cel.Value == "" ? DBNull.Value : cel.Value;
                    }
                    dt.Rows.Add(dr);
                }
                dt.AcceptChanges();
                ds.Tables.Add(dt);
            }
            return ds;
        }

        private Type getColType(string colname)
        {
            Type tp = typeof(string);
            if (colname.ToLower().IndexOf("time") >= 0)
            {
                tp = typeof(DateTime);
            }
            if (colname.ToLower().IndexOf("delay") >= 0 || colname.ToLower().IndexOf("traffic") >= 0 || colname.ToLower().IndexOf("count") >= 0)
            {
                tp = typeof(long);
            }
            if (colname.ToLower().IndexOf("rate") >= 0 || colname.ToLower().IndexOf("speed") >= 0)
            {
                tp = typeof(double);
            }
            return tp;
        }
    }
}
