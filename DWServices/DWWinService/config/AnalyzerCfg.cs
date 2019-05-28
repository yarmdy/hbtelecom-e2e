using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DWWinService
{
    class AnalyzerCfgReader
    {
        public static List<AnalyzerCfg> load()
        {
            var cfgname = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config/SysConfig.xml");
            if (!System.IO.File.Exists(cfgname))
            {
                return null;
            }
            try
            {
                var cfgstr = XDocument.Load(cfgname);
                DB.ConnectStr = O2.O2S(cfgstr.Root.Element("server"));
                List<AnalyzerCfg> aclist = new List<AnalyzerCfg>();
                foreach (var node in cfgstr.Descendants("analyzer"))
                {
                    aclist.Add(new AnalyzerCfg(node));
                }
                return aclist;
            }
            catch (Exception e)
            {
                LOG.WriteLog(e.Message);
                return null;
            }
        }
    }
    class AnalyzerCfg
    {
        public AnalyzerCfg(XElement ele)
        {
            _name = O2.O2S(ele.Element("name"));
            _dec = O2.O2S(ele.Element("dec"));
            _class_name = O2.O2S(ele.Element("class_name"));
            _quarter = O2.O2B(ele.Element("quarter"));
            _daytime = O2.O2DT(ele.Element("daytime"));
            _dbtables = new DBTables(O2.O2S(ele.Element("dbtables").Descendants("table_min").FirstOrDefault()), O2.O2S(ele.Element("dbtables").Descendants("table_day").FirstOrDefault()));
            _csvs = new List<Csv>();
            foreach (var node in ele.Descendants("csv"))
            {
                _csvs.Add(new Csv(node));
            }
            _mapping = new Mapping(ele.Descendants("mapping").FirstOrDefault());
            if (ele.Descendants("custom").FirstOrDefault() != null)
            {
                foreach (var node in ele.Descendants("custom").FirstOrDefault().Elements())
                {
                    _custom.Add(node.Name.ToString(), node.Value);
                }
            }
        }
        #region 私有
        string _name;
        string _dec;
        string _class_name;
        bool _quarter;
        DateTime _daytime;
        DBTables _dbtables;
        List<Csv> _csvs;
        Mapping _mapping;
        Dictionary<string, string> _custom = new Dictionary<string, string>();
        #endregion

        #region 属性
        public string name { get { return _name; } }
        public string dec { get { return _dec; } }
        public string class_name { get { return _class_name; } }
        public bool quarter { get { return _quarter; } }
        public DateTime daytime { get { return _daytime; } }
        public DBTables dbtables { get { return _dbtables; } }
        public List<Csv> csvs { get { return _csvs; } }
        public Mapping mapping { get { return _mapping; } }
        public Dictionary<string, string> custom { get { return _custom; } }
        #endregion
    }
    class DBTables
    {
        public DBTables(string min, string day)
        {
            _table_min = min;
            _table_day = day;
        }
        #region 私有
        string _table_min;
        public string _table_day;
        #endregion

        #region 属性
        public string table_min { get { return _table_min; } }
        public string table_day { get { return _table_day; } }
        #endregion
    }
    class Csv
    {
        public Csv(XElement ele)
        {
            _fileserver = new FileServer(ele.Element("fileserver"));
            _filetmp = new FileServer(ele.Element("filetmp"));
            _filetarget = new FileServer(ele.Element("filetarget"));
            _regular = O2.O2S(ele.Element("regular"));
        }
        #region 私有
        FileServer _fileserver;
        FileServer _filetmp;
        FileServer _filetarget;
        string _regular;
        #endregion

        #region 属性
        public FileServer fileserver { get { return _fileserver; } }
        public FileServer filetmp { get { return _filetmp; } }
        public FileServer filetarget { get { return _filetarget; } }
        public string regular { get { return _regular; } }
        #endregion
    }
    class FileServer
    {
        public FileServer(XElement ele)
        {
            _stype = O2.O2S(ele.Element("stype"));
            _ip = O2.O2S(ele.Element("ip"));
            _port = O2.O2I(ele.Element("port"));
            _uid = O2.O2S(ele.Element("uid"));
            _pwd = O2.O2S(ele.Element("pwd"));
            _path = O2.O2S(ele.Element("path"));
        }
        #region 私有
        string _stype;
        string _ip;
        int _port;
        string _uid;
        string _pwd;
        string _path;
        #endregion

        #region 属性
        public string stype { get { return _stype; } }
        public string ip { get { return _ip; } }
        public int port { get { return _port; } }
        public string uid { get { return _uid; } }
        public string pwd { get { return _pwd; } }
        public string path { get { return _path; } }
        #endregion
    }
    class Mapping
    {
        DictionaryX<string> dic = null;
        public Mapping(XElement ele)
        {
            dic = new DictionaryX<string>(ele.Elements(), a => ((XElement)a).Name.ToString(), b => O2.O2S(((XElement)b).Attribute("csvcol")), null);
        }
        public KeyValuePair<string, string> this[string key]
        {
            get
            {
                return dic[key];
            }
        }

        public Dictionary<string, string>.Enumerator GetEnumerator()
        {
            return dic.GetEnumerator();
        }
    }

    class DictionaryX<TKeyValve>
    {
        TKeyValve empty;
        public DictionaryX(System.Collections.IEnumerable list, Func<object, TKeyValve> getkey, Func<object, TKeyValve> getvalue, TKeyValve em)
        {
            empty = em;
            dic1 = new Dictionary<TKeyValve, TKeyValve>();
            dic2 = new Dictionary<TKeyValve, TKeyValve>();
            foreach (var item in list)
            {
                dic1.Add(getkey(item), getvalue(item));
                dic2.Add(getvalue(item), getkey(item));
            }
        }
        public KeyValuePair<TKeyValve, TKeyValve> this[TKeyValve key]
        {
            get
            {
                if (dic1.ContainsKey(key))
                {
                    return new KeyValuePair<TKeyValve, TKeyValve>(key, dic1[key]);
                }
                else if (dic2.ContainsKey(key))
                {
                    return new KeyValuePair<TKeyValve, TKeyValve>(dic2[key], key);
                }
                else
                {
                    return new KeyValuePair<TKeyValve, TKeyValve>(empty, empty);
                }
            }
        }
        Dictionary<TKeyValve, TKeyValve> dic1 = null;
        Dictionary<TKeyValve, TKeyValve> dic2 = null;

        public Dictionary<TKeyValve, TKeyValve>.Enumerator GetEnumerator()
        {
            return dic1.GetEnumerator();
        }
    }

    /// <summary>
    /// 分析器状态枚举
    /// </summary>
    enum AnalyzerStatus
    {
        /// <summary>
        /// 启动
        /// </summary>
        start = 0,
        /// <summary>
        /// 等待
        /// </summary>
        wait = 1,
        /// <summary>
        /// 下载
        /// </summary>
        download = 2,
        /// <summary>
        /// 分析
        /// </summary>
        analysis = 3,
        /// <summary>
        /// 入库
        /// </summary>
        import = 4,
        /// <summary>
        /// 报错
        /// </summary>
        error = 5
    }
    enum LoopType
    {
        quarter = 0,
        day = 1
    }
    delegate void OnStatusChangeHandler(Analyzer analyzer, AnalyzerStatus status, LoopType looptype);

    public static class O2
    {
        public static string O2S(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return "";
            }
            if (o.GetType() == typeof(XElement))
            {
                return ((XElement)o).Value;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                return ((XAttribute)o).Value;
            }
            return o.ToString();
        }
        public static bool O2B(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return false;
            }
            if (o.GetType() == typeof(XElement))
            {
                bool res;
                bool.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                bool res;
                bool.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                bool res;
                bool.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static DateTime O2DT(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return new DateTime();
            }
            if (o.GetType() == typeof(XElement))
            {
                DateTime res;
                DateTime.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                DateTime res;
                DateTime.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                DateTime res;
                DateTime.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static int O2I(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                int res;
                int.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                int res;
                int.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                int res;
                int.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static long O2L(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                long res;
                long.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                long res;
                long.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                long res;
                long.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static double O2D(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                double res;
                double.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                double res;
                double.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                double res;
                double.TryParse(o.ToString(), out res);
                return res;
            }
        }
    }
    public static class IntExtend
    {
        public static double Div1(this int val, int val2)
        {
            if (val2 == 0)
            {
                return 1;
            }
            else
            {
                return 1.0 * val / val2;
            }
        }

        public static double Div1(this int val, double val2)
        {
            if (val2 == 0)
            {
                return 1;
            }
            else
            {
                return 1.0 * val / val2;
            }
        }

        public static double Div1(this double val, int val2)
        {
            if (val2 == 0)
            {
                return 1;
            }
            else
            {
                return 1.0 * val / val2;
            }
        }

        public static double Div1(this double val, double val2)
        {
            if (val2 == 0)
            {
                return 1;
            }
            else
            {
                return 1.0 * val / val2;
            }
        }
        public static double Div0(this int val, int val2)
        {
            if (val2 == 0)
            {
                return 0;
            }
            else
            {
                return 1.0 * val / val2;
            }
        }

        public static double Div0(this int val, double val2)
        {
            if (val2 == 0)
            {
                return 0;
            }
            else
            {
                return 1.0 * val / val2;
            }
        }

        public static double Div0(this double val, int val2)
        {
            if (val2 == 0)
            {
                return 0;
            }
            else
            {
                return 1.0 * val / val2;
            }
        }

        public static double Div0(this double val, double val2)
        {
            if (val2 == 0)
            {
                return 0;
            }
            else
            {
                return 1.0 * val / val2;
            }
        }
    }
}
