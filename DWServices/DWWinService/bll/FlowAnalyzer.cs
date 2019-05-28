using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    class FlowAnalyzer : Analyzer
    {
        public override string GetKeyQuarter(Dictionary<string, object> dic)
        {
            return "河北省";
        }

        public override void CombineRowQuarter(Dictionary<string, Dictionary<string, object>> dics, Dictionary<string, object> dic)
        {
            var dic_exist = dics[GetKeyQuarter(dic)];
            dic_exist["BFLOW"] = O2.O2L(dic_exist["BFLOW"]) + O2.O2L(dic["BFLOW"]);
            dic_exist["TFLOW"] = O2.O2L(dic_exist["TFLOW"]) + O2.O2L(dic["TFLOW"]);
        }

        public override bool IsZhiCha(Dictionary<string, object> dic)
        {
            return true;
        }

        public override string DataConvert(string col, string val)
        {
            if (col == "ECGI")
            {
                long v;
                if (val.Length < 5)
                {
                    return val;
                }
                if (long.TryParse(val.Substring(5), System.Globalization.NumberStyles.AllowHexSpecifier, null, out v))
                {
                    val = v.ToString();
                }
            }
            if (col == "BFLOW")
            {
                val = (O2.O2L(val)).ToString();
            }
            if (col == "TFLOW")
            {
                val = (O2.O2L(val)).ToString();
            }
            return val;
        }

        public override string Str2DTStr(string str)
        {
            return DateTime.Parse("1970/1/1 8:30:00").AddSeconds(O2.O2I(str)).ToString("yyyy-MM-dd HH:mm:ss");
        }
        public override bool Valid(Dictionary<string, object> dic)
        {
            if (!string.IsNullOrEmpty(dic["ECGI"].ToString()) && !string.IsNullOrEmpty(dic["START_TIME"].ToString()))
            {

                int ecgi = O2.O2I(dic["ECGI"]);
                int two = ecgi >> 20;
                int one = ecgi % 0x100 / 0x10;
                if ((two <= 0x17 && two >= 0x10) || two == 0xE3)
                {
                    if (one == 0x0 || one == 0x1 || one == 0x3 || one == 0X5 || one == 0x6)
                    {
                        return true;
                    }
                }
                else if ((two <= 0x8D && two >= 0x87) || two == 0xF1 || two == 0xF2)
                {
                    if (one == 0x8 || one == 0x9 || one == 0xB || one == 0xD || one == 0xE)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public override void BeforeImportDB(System.Data.DataTable dt)
        {
            DB.Query("truncate table " + cfg.dbtables.table_min);
        }
        public override void AfterImportDB(String time)
        {

        }
        public override void DoDay()
        {

        }
    }
}
