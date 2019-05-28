using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWWinService
{
    class AlarmAnalyzer: Analyzer
    {
        public override string GetKeyQuarter(Dictionary<string, object> dic)
        {
            return dic["ECI"].ToString();
        }

        public override void BeforeImportDB(DataTable dt)
        {
            string sql = "truncate Table "+cfg.dbtables.table_min;
            DB.Exec(sql);
        }

        public override void AfterImportDB(string time)
        {
            string sql = @"update DATA_STATISTICS s set alarm = (select count(*) num from ALARM_MIN where city = s.city)";
            DB.Exec(sql);
            sql = "update DATA_STATISTICS s set alarm = (select count(*) num from ALARM_MIN) where city = '河北省'";
            DB.Exec(sql);
        }
    }
}
