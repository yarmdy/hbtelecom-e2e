using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    class MRAnalyzer : Analyzer
    {
        public override string GetKeyDay(Dictionary<string, object> dic)
        {
            return dic["ECI"].ToString() + dic["SDATE"];
        }

        public override string Str2DTStr(string str)
        {
            str = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
            return str;
        }
        public override string[] FilterFile(string[] files, string timestr)
        {

            return files.Where(a => a.ToLower().Contains("_" + timestr + ".") || a.ToLower().Contains("_" + timestr + "_")).ToArray();
        }
    }
}
