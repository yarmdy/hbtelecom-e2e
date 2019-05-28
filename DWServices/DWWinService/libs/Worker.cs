using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DWWinService
{
    class Worker
    {
        public Worker(AnalyzerCfg c, OnStatusChangeHandler OnStatusChange)
        {
            _anzr = (IAnalyzer)Activator.CreateInstance(Type.GetType(c.class_name));
            _anzr.cfg = c;
            if (OnStatusChange != null) {
                _anzr.OnStatusChange += OnStatusChange;
            }
            if (cfg.quarter) {
                Thread th1 = new Thread(new ThreadStart(_anzr.DoQuarter));
                th1.Name = cfg.name + "[min]";
                th1.Start();
            }
            Thread th2 = new Thread(new ThreadStart(_anzr.DoDay));
            th2.Name = cfg.name + "[day]";
            th2.Start();
        }
        #region 私有
        IAnalyzer _anzr;
        #endregion

        #region 属性
        public AnalyzerCfg cfg { get { return _anzr.cfg; } }

        public string ErrQuarter { get { return _anzr.error_quarter; } }
        public string ErrDay { get { return _anzr.error_day; } }
        public AnalyzerStatus StatusQuarter { get { return _anzr.status_quarter; } }
        public AnalyzerStatus StatusDay { get { return _anzr.status_day; } }
        #endregion
    }
}
