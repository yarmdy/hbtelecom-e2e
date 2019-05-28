using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    interface IAnalyzer
    {
        AnalyzerCfg cfg{get;set;}
        void DoQuarter();
        void DoDay();
        AnalyzerStatus status_quarter { get; set; }
        AnalyzerStatus status_day { get; set; }
        string error_quarter { get; }
        string error_day { get; }
        event OnStatusChangeHandler OnStatusChange;
    }
}
