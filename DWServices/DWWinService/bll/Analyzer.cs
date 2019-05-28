using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Threading;
using Oracle.DataAccess.Client;
using System.Xml;
using System.Xml.Linq;

namespace DWWinService
{
    /// <summary>
    /// 下载分析入库基类 所有逻辑代码都写在这里，要通用模式
    /// </summary>
    class Analyzer : IAnalyzer
    {
        #region 私有属性

        AnalyzerStatus _status_quarter;
        AnalyzerStatus _status_day;

        string _error_quarter = "";
        string _error_day = "";

        #endregion

        #region 属性

        /// <summary>
        /// 分析器配置
        /// </summary>
        public AnalyzerCfg cfg { get; set; }

        /// <summary>
        /// 分析器状态
        /// </summary>
        public AnalyzerStatus status_quarter
        {
            get
            {
                return _status_quarter;
            }
            set
            {
                if (OnStatusChange != null && status_quarter != value)
                {
                    OnStatusChange(this, value, LoopType.quarter);
                }
                _status_quarter = value;
            }
        }
        public AnalyzerStatus status_day
        {
            get
            {
                return _status_day;
            }
            set
            {
                if (OnStatusChange != null && status_day != value)
                {
                    OnStatusChange(this, value, LoopType.day);
                }
                _status_day = value;
            }
        }

        public string error_quarter
        {
            get
            {
                return _error_quarter;
            }
        }
        public string error_day
        {
            get
            {
                return _error_day;
            }
        }

        /// <summary>
        /// 获取时间字符串 可重写
        /// </summary>
        public virtual string[] TimeStr
        {
            get
            {
                DateTime dt = DateTime.Now;
                string min = dt.ToString("yyyyMMddHH");
                var minute = (dt.Minute / 15) * 15;
                min += minute.ToString().PadLeft(2, '0');
                string day = dt.AddDays(-1).ToString("yyyyMMdd");
                return new string[] { min, day };
            }
        }
        /// <summary>
        /// 临时目录所有文件
        /// </summary>
        public virtual string[] TmpFiles
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var filec in cfg.csvs)
                {
                    if (filec.filetmp.path != "")
                    {
                        var files = Directory.GetFiles(filec.filetmp.path, "*.csv");
                        foreach (var file in files)
                        {
                            res.Add(file);
                        }
                    }
                }
                return res.Distinct().ToArray();
            }
        }

        /// <summary>
        /// 目标目录所有文件
        /// </summary>
        public virtual string[] TargetFiles
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var filec in cfg.csvs)
                {
                    if (filec.filetarget.path != "")
                    {
                        var files = Directory.GetFiles(filec.filetarget.path, "*.csv");
                        foreach (var file in files)
                        {
                            res.Add(file);
                        }
                    }
                }
                return res.Distinct().ToArray();
            }
        }

        #endregion

        public virtual string FileHead
        {
            get
            {
                return "";
            }
        }

        #region 循环方法

        /// <summary>
        /// 15分钟循环 可重写
        /// </summary>
        public virtual void DoQuarter()
        {
            while (true)
            {
                var tmpfiles = TmpFiles;
                try
                {

                    status_quarter = AnalyzerStatus.wait;
                    string quarterMin = TimeStr[0];
                    var analyIsComplete = FilterFile(TargetFiles, quarterMin).Count();
                    if (analyIsComplete > 0)
                    {
                        DateTime dt = DateTime.Now;
                        int min = dt.Minute;
                        int sec = dt.Second;
                        int nextMin = ((min / 15) + 1) * 15;
                        Thread.Sleep((nextMin - min) * 60 * 1000 - sec * 1000);
                    }
                    Downloader loader = new Downloader(cfg.csvs[0].fileserver);
                    var fileList = loader.filelist;
                    var downIsComplete = FilterFile(fileList, quarterMin).Count();
                    if (downIsComplete > 0)
                    {
                        executeQuarter();
                    }
                    Thread.Sleep(60 * 1000);
                }
                catch (Exception e)
                {
                    try
                    {
                        foreach (var file in tmpfiles)
                        {
                            File.Delete(file);
                        }
                    }
                    catch { }
                    string colon = ":";
                    if (e.Message.StartsWith("-"))
                    {
                        colon = "";
                    }
                    LOG.WriteLog(cfg.dec + "-15分钟分析器" + colon + e.Message);
                    _error_quarter = e.Message;
                    status_quarter = AnalyzerStatus.error;
                    Thread.Sleep(30 * 1000);
                    continue;
                }
            }
        }
        /// <summary>
        /// 天循环 可重写
        /// </summary>
        public virtual void DoDay()
        {
            var tmpfiles = TmpFiles;
            Boolean flag = true;
            while (true)
            {
                try
                {
                    status_day = AnalyzerStatus.wait;
                    DateTime dt = DateTime.Now;
                    int hour = dt.Hour;
                    int min = dt.Minute;
                    if (hour == cfg.daytime.Hour && min == cfg.daytime.Minute && flag)
                    {
                        //执行
                        executeDay();
                        Thread.Sleep(60 * 1000);
                        flag = false;
                    }
                    if (hour >= cfg.daytime.Hour && min != cfg.daytime.Minute && !flag)
                    {
                        flag = true;
                    }
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    try
                    {
                        if (!cfg.quarter)
                        {
                            foreach (var file in TmpFiles)
                            {
                                File.Delete(file);
                            }
                        }
                    }
                    catch { }
                    string colon = ":";
                    if (e.Message.StartsWith("-"))
                    {
                        colon = "";
                    }
                    LOG.WriteLog(cfg.dec + "-天分析器" + colon + e.Message);
                    _error_day = e.Message;
                    status_day = AnalyzerStatus.error;
                    Thread.Sleep(60 * 1000);
                    continue;
                }

            }
        }

        #endregion

        #region 业务方法

        /// <summary>
        /// 下载csv 可重写
        /// </summary>
        /// <param name="time">时间字符串</param>
        /// <returns>返回下载的文件名</returns>
        public virtual string[] DownloadCsv(string time)
        {
            string[] tempFiles = null;
            Csv[] csvs = cfg.csvs.ToArray();
            for (int i = 0; i < csvs.Length; i++)
            {
                FileServer fileServer = csvs[i].fileserver;
                Downloader down = new Downloader(fileServer);
                string[] files = down.filelist;
                files = FilterFile(files, time);
                tempFiles = files.Select(a => Path.Combine(csvs[i].filetmp.path, a)).ToArray();
                down.download(files, tempFiles);
            }
            return tempFiles;
        }
        /// <summary>
        /// 下载后执行方法，一般为解压缩或简单处理 可重写
        /// </summary>
        public virtual void AfterDownloadCsv()
        {
            if (TmpFiles.Length <= 0)
            {
                throw new Exception("文件下载失败");
            }
        }
        /// <summary>
        /// 分析方法 可重写
        /// </summary>
        /// <returns>返回分析结果的数据表</returns>
        //public virtual DataTable Analysis()
        //{

        //    return null;
        //}
        /// <summary>
        /// 入库方法 可重写
        /// </summary>
        /// <param name="dt">分析后的结果</param>
        /// <returns>返回是否成功</returns>
        public virtual bool ImportDB(DataTable dt, bool bol)
        {
            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(DB.ConnectStr))
            {
                Oracle.DataAccess.Client.OracleBulkCopy bulkCopy = null;
                try
                {
                    connection.Open();
                    bulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(connection, Oracle.DataAccess.Client.OracleBulkCopyOptions.UseInternalTransaction);
                    //bulkCopy.BatchSize = 1000;

                    bulkCopy.DestinationTableName = bol ? cfg.dbtables.table_day : cfg.dbtables.table_min;
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        bulkCopy.BulkCopyTimeout = 3600;
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
            }
            return false;
        }
        /// <summary>
        /// 删除临时文件方法 可重写
        /// </summary>
        public virtual void DeleteTmp()
        {
            Csv[] csvs = cfg.csvs.ToArray();
            for (int i = 0; i < csvs.Length; i++)
            {
                foreach (var file in Directory.GetFiles(csvs[i].filetmp.path))
                {
                    string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                    File.Copy(file, Path.Combine(csvs[i].filetarget.path, fileName), true);
                    File.Delete(file);
                    //File.Move(file, Path.Combine(csvs[i].filetarget.path,fileName));
                }
            }
        }

        public virtual void BeforeImportDB(DataTable dt)
        {

        }
        public virtual void AfterImportDB(String time)
        {

        }

        public virtual void AfterImportDBDay(String time)
        {

        }
        public virtual bool IsDoQuarter(string time) {
            return true;
        }
        public void executeQuarter()
        {
            var time = TimeStr[0];
            if (!IsDoQuarter(time)) {
                return;
            }
            DataTable table = null;
            try
            {
                //执行下载
                status_quarter = AnalyzerStatus.download;
                DownloadCsv(time);
            }
            catch (Exception e)
            {
                throw new Exception("-下载：" + e.Message);
            }
            try
            {
                //下载完成
                AfterDownloadCsv();
            }
            catch (Exception e)
            {
                throw new Exception("-下载后：" + e.Message);
            }
            try
            {
                //执行分析
                status_quarter = AnalyzerStatus.analysis;
                table = Analysis(true, TmpFiles);
            }
            catch (Exception e)
            {
                throw new Exception("-分析：" + e.Message);
            }
            try
            {
                BeforeImportDB(table);
            }
            catch (Exception e)
            {
                throw new Exception("-入库前：" + e.Message);
            }
            try
            {
                //入库
                status_quarter = AnalyzerStatus.import;
                ImportDB(table, false);
            }
            catch (Exception e)
            {
                throw new Exception("-入库：" + e.Message);
            }
            try
            {
                //入库之后
                AfterImportDB(time);
            }
            catch (Exception e)
            {
                throw new Exception("-入库后：" + e.Message);
            }
            try
            {
                //文件移动
                DeleteTmp();
            }
            catch (Exception e)
            {
                throw new Exception("-文件移动：" + e.Message);
            }
        }

        public void executeDay()
        {
            DataTable table = null;
            var time = TimeStr[1];
            if (cfg.quarter)
            {
                try
                {
                    //分析
                    status_day = AnalyzerStatus.analysis;
                    string[] files = FilterFile(TargetFiles, time);
                    table = Analysis(false, files);
                }
                catch (Exception e)
                {
                    throw new Exception("-分析：" + e.Message);
                }
                try
                {
                    BeforeImportDB(table);
                }
                catch (Exception e)
                {
                    throw new Exception("-入库前：" + e.Message);

                }
                try
                {
                    //入库
                    status_day = AnalyzerStatus.import;
                    ImportDB(table, true);
                }
                catch (Exception e)
                {
                    throw new Exception("-入库：" + e.Message);
                }
                try
                {
                    //入库之后
                    AfterImportDBDay(time);
                }
                catch (Exception e)
                {
                    throw new Exception("-入库后：" + e.Message);
                }
            }
            else
            {
                try
                {
                    //执行下载
                    status_day = AnalyzerStatus.download;
                    DownloadCsv(time);
                }
                catch (Exception e)
                {
                    throw new Exception("-执行下载：" + e.Message);
                }
                try
                {
                    //下载完成
                    AfterDownloadCsv();
                }
                catch (Exception e)
                {
                    throw new Exception("-下载完成：" + e.Message);
                }
                try
                {
                    //执行分析
                    status_day = AnalyzerStatus.analysis;
                    table = Analysis(false, TmpFiles);
                }
                catch (Exception e)
                {
                    throw new Exception("-执行分析：" + e.Message);
                }
                try
                {
                    BeforeImportDB(table);
                }
                catch (Exception e)
                {
                    throw new Exception("-入库前：" + e.Message);
                }
                try
                {
                    //入库
                    status_day = AnalyzerStatus.import;
                    ImportDB(table, true);
                }
                catch (Exception e)
                {
                    throw new Exception("-入库：" + e.Message);
                }
                try
                {
                    //入库之后
                    AfterImportDBDay(time);
                }
                catch (Exception e)
                {
                    throw new Exception("-入库之后：" + e.Message);
                }
                try
                {
                    //文件移动
                    DeleteTmp();
                }
                catch (Exception e)
                {
                    throw new Exception("-文件移动：" + e.Message);
                }
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 判断是否可以进行下次循环
        /// </summary>
        /// <param name="time">时间字符串</param>
        /// <returns>可以返回true，不可以返回false</returns>
        private bool ready(string time)
        {
            return false;
        }
        /// <summary>
        /// 分析业务流程 可重写
        /// </summary>
        /// <returns>返回分析结果数据表</returns>
        public virtual DataTable Analysis(bool bQuarter, string[] files)
        {
            Dictionary<string, Dictionary<string, object>> dics = new Dictionary<string, Dictionary<string, object>>();
            DataSet ds = new DataSet();
            DataTable dt = null;
            using (OracleConnection con = new OracleConnection(DB.ConnectStr))
            {
                OracleDataAdapter da = new OracleDataAdapter("select * from " + (bQuarter ? cfg.dbtables.table_min : cfg.dbtables.table_day) + " where rownum<1", con);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            //foreach (var col in cfg.mapping)
            //{
            //    dt.Columns.Add(col.Key);
            //}
            foreach (var file in files)
            {
                var filestr = File.ReadAllText(file, Encoding.GetEncoding("gb2312"));
                filestr = FileHead + filestr;
                var filelines = filestr.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (filelines.Length < 2)
                {
                    continue;
                }
                var bfirst = true;
                string[] cols = null;
                foreach (var row in filelines)
                {
                    if (bfirst)
                    {
                        bfirst = false;
                        cols = row.Split(',');
                        for (int i = 0; i < cols.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(cfg.mapping[cols[i]].Key))
                            {
                                cols[i] = cfg.mapping[cols[i]].Key;
                            }
                        }
                    }
                    else
                    {
                        var vals = row.Split(',');
                        if (vals.Length < cols.Length)
                        {
                            continue;
                        }
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        for (int i = 0; i < cols.Length; i++)
                        {
                            if (cols[i] == "")
                            {
                                continue;
                            }
                            dic.Add(cols[i], DataConvert(cols[i], vals[i]));
                        }
                        var key = bQuarter ? GetKeyQuarter(dic) : GetKeyDay(dic);
                        var valid = Valid(dic);
                        if (valid)
                        {
                            if (dics.ContainsKey(key))
                            {
                                if (bQuarter)
                                {
                                    CombineRowQuarter(dics, dic);
                                }
                                else
                                {
                                    CombineRowDay(dics, dic);
                                }
                            }
                            else
                            {
                                AddRow(dics, key, dic);
                            }
                        }
                    }
                }
            }
            foreach (var dic in dics)
            {
                var row = dt.NewRow();
                if (bQuarter && !IsZhiCha(dic.Value) || (!bQuarter) && !IsZhiChaDay(dic.Value))
                {
                    continue;
                }
                foreach (var col in dic.Value)
                {
                    if (!dt.Columns.Contains(col.Key))
                    {
                        continue;
                    }
                    if (col.Value == "")
                    {
                        row[col.Key] = DBNull.Value;
                    }
                    else if (dt.Columns[col.Key].DataType == typeof(DateTime))
                    {
                        row[col.Key] = Str2DTStr(col.Value.ToString());
                    }
                    else
                    {
                        row[col.Key] = col.Value;
                    }
                }
                ProcessRow(row, dic.Value);
                dt.Rows.Add(row);
            }

            //string sp = dt.DataSet.GetXml();
            //var spxml = XDocument.Parse(sp);
            //StringBuilder sb = new StringBuilder();
            //foreach (var ele in spxml.Descendants("Table")) {
            //    string sbrow = "";
            //    foreach (var r in ele.Elements()) {
            //        sbrow+= r.Value + "\t";
            //    }
            //    sb.Append(sbrow+"\r\n");
            //}
            //var sbstr = sb.ToString();
            //StringBuilder sb = new StringBuilder();
            //foreach (var dic2 in dics)
            //{
            //    StringBuilder sb2 = new StringBuilder();
            //    sb2.Append(dic2.Key + "\t");
            //    foreach (var dic3 in dic2.Value)
            //    {
            //        sb2.Append(dic3.Value + "\t");
            //    }
            //    sb.Append(sb2.ToString() + "\r\n");
            //}
            //var strsb = sb.ToString();

            return dt;
        }

        #endregion

        #region 一般都要重写的方法

        /// <summary>
        /// 获取主键，唯一值
        /// </summary>
        /// <param name="dic">当前行数据</param>
        /// <returns>返回key字符串</returns>
        public virtual string GetKeyQuarter(Dictionary<string, object> dic)
        {
            return dic["IP_ADDRESS"].ToString();
        }
        public virtual string GetKeyDay(Dictionary<string, object> dic)
        {
            return dic["IP_ADDRESS"].ToString();
        }
        /// <summary>
        /// 合并数据方法
        /// </summary>
        /// <param name="dics">数据集合</param>
        /// <param name="dic">当前数据行</param>
        public virtual void CombineRowQuarter(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            dics[GetKeyQuarter(dic)] = dic;
        }
        public virtual void CombineRowDay(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            dics[GetKeyDay(dic)] = dic;
        }
        public virtual void AddRow(Dictionary<string, Dictionary<string, object>> dics, string key, Dictionary<string, object> dic)
        {
            dics.Add(key, dic);
        }
        /// <summary>
        /// 是否质差
        /// </summary>
        /// <param name="dic">当前行数据</param>
        /// <returns></returns>
        public virtual bool IsZhiCha(Dictionary<string, object> dic)
        {
            return true;
        }
        public virtual bool IsZhiChaDay(Dictionary<string, object> dic)
        {
            return IsZhiCha(dic);
        }

        public virtual string Str2DTStr(string str)
        {
            DateTime dt;
            if (DateTime.TryParse(str, out dt))
            {
                return str;
            }
            if (str.Length < 12)
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return string.Format("{0}-{1}-{2} {3}:{4}:00", str.Substring(0, 4), str.Substring(4, 2), str.Substring(6, 2), str.Substring(8, 2), str.Substring(10, 2));
        }
        public virtual string DataConvert(string col, string val)
        {
            return val;
        }
        public virtual bool Valid(Dictionary<string, object> dic)
        {
            return true;
        }

        public virtual string[] FilterFile(string[] files, string timestr)
        {
            return files.Where(a => a.Contains(timestr)).ToArray();
        }
        public virtual void ProcessRow(DataRow dr, Dictionary<string, object> dic)
        {

        }
        #endregion

        #region 事件

        public event OnStatusChangeHandler OnStatusChange;

        #endregion
    }

}
