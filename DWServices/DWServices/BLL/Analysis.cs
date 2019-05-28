using Com.Netframe.Helpers;
using DWServices.Common;
using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DWServices.BLL
{
    public class Analysis
    {
        public DataTable analysisCell(DataTable data,String time)
        {
            if (data == null || String.IsNullOrEmpty(time))
                return null;
            DataTable result = null;
            String ecis = "";
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if(i==0)
                    ecis = data.Rows[i][0].ToString();
                else
                    ecis = ecis+","+data.Rows[i][0].ToString();
            }
            String sqlstr = "select * from DATA_KQIINFO where eci in (" + ecis + ") and createtime=to_date('" + time + "','yyyy-MM-dd')";
            result = OraConnect.ReadData(sqlstr);
            if (result == null)
                return null;
            else
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    DataRow[] rows = data.Select("ECI=" + result.Rows[i]["ECI"].ToString());
                    if (rows.Length > 0)
                    {
                        result.Rows[i]["KQIINFO"] = rows[0][3];
                    }
                }
            }

            return result;
        }

        public DataTable GetQuotaData(DataTable data,String eci, String mfield, String sfield)
        {
            if (data == null)
                return null;
            data.Columns.Add(mfield + "_q");
            double max = 0, min = 0;
            Boolean havemax = false;
            Boolean havemin = false;
            String sql = "select t.thequota ,t.maxvalue,t.minvalue from DATA_QUOTA t  where t.mfield='" + sfield + "'";
            if (sfield == "UP_PRB" || sfield == "DOWN_PRB" || sfield == "RRC_CONNCOUNT" || sfield == "DOWN_PDCP" || sfield == "UP_PDCP" || sfield == "USERCOUNT")
            {
                if (ECIAndIDConvert.getflowclass(eci))
                    sql = "select t.thequota ,t.maxvalue,t.minvalue from DATA_QUOTA t  where t.mfield='" + sfield + "' and  SUBQUOTAPROJ='800M'";
                else
                    sql = "select t.thequota ,t.maxvalue,t.minvalue from DATA_QUOTA t  where t.mfield='" + sfield + "' and  SUBQUOTAPROJ='1800M'";
            }
            DataTable quotadata = OraConnect.ReadData(sql);
            if (quotadata != null)
            {
                if (quotadata.Rows.Count > 0)
                {
                    if (quotadata.Rows[0]["maxvalue"] != null)
                    {
                        String value = quotadata.Rows[0]["maxvalue"].ToString();
                        if (value != "")
                        {
                            max = Convert.ToDouble(value);
                            havemax = true;
                        }
                    }
                    if (quotadata.Rows[0]["minvalue"] != null)
                    {
                        String value = quotadata.Rows[0]["minvalue"].ToString();
                        if (value != "")
                        {
                            min = Convert.ToDouble(value);
                            havemin = true;
                        }
                    }
                }
            }
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[i][mfield] != null)
                {
                    if (data.Rows[i][mfield].ToString() == "")
                    {
                        data.Rows[i][mfield + "_q"] = "true";
                        continue;
                    }
                    String value = data.Rows[i][mfield].ToString().Trim();
                    double vel = 0; ;
                    if (value.Contains("%"))
                    {
                        value=value.Replace("%", "");
                        vel = Convert.ToDouble(value) / 100;
                    }
                    else
                        vel = Convert.ToDouble(value);
                    if (havemax && havemin)
                    {
                        if (vel >= min && vel <= max)
                        {
                            data.Rows[i][mfield + "_q"] = "false";
                        }
                        else
                            data.Rows[i][mfield + "_q"] = "true";
                    }
                    else if (havemax || havemin)
                    {
                        if (havemin)
                        {
                            if (vel >= min)
                            {
                                data.Rows[i][mfield + "_q"] = "false";
                            }
                            else
                                data.Rows[i][mfield + "_q"] = "true";
                        }
                        else
                        {
                            if (vel <= max)
                            {
                                data.Rows[i][mfield + "_q"] = "false";
                            }
                            else
                                data.Rows[i][mfield + "_q"] = "true";
                        }
                    }
                    else
                    {
                        data.Rows[i][mfield + "_q"] = "true";
                    }
                }
            }
            return data;
        }

        public bool Analysisindex(DataTable data1, DataTable data2,String mykey,String time)
        {
            Boolean result = true;
            if (!data1.Columns.Contains("value_q") || !data2.Columns.Contains("value_q"))
            {
                return true;
            }
            else
            {
                DataRow[] frows1 = data1.Select("sdate='" + time + "'");
                DataRow[] frows2 = data2.Select("sdate='" + time + "'");
                 if (frows1.Length > 0)
                 {
                     if (frows1[0]["value_q"].ToString() == "true" || frows2[0]["value_q"].ToString() == "true")
                     {
                         return true;
                     }
                 }
                DateTime dt = DateTime.Now.AddDays(-1);
                if (!String.IsNullOrEmpty(time))
                {
                    dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                    if (dt == DateTime.Now.Date)
                        dt = DateTime.Now.AddDays(-1);
                }
                DateTime starttime = dt.AddDays(-6);
                DateTime endtime = dt.AddDays(1);
                int count = 0;
                String fal = "false";

                for (DateTime i = starttime; i < endtime; )
                {
                    String str = i.ToString("yyyy-MM-dd");
                    DataRow[] rows1 = data1.Select("sdate='" + str + "'");
                    DataRow[] rows2 = data2.Select("sdate='" + str + "'");
                    if (rows1.Length >0 && rows2.Length > 0)
                    {
                        if (fal.Equals(rows1[0]["value_q"]) || fal.Equals(rows2[0]["value_q"]))
                            count = count + 1;
                    }
                    i = i.AddDays(1);
                }
                if (count >= 3)
                {
                    result = false;
                }
                return result;
            }
        }

        public Boolean getQuotaStatic(DataTable data,String time)
        {
            DataRow[] rows = data.Select("sdate='" + time + "'");
            if (rows.Length > 0)
            {
                if (rows[0]["value_q"].ToString() == "true")
                {
                    return true;
                }
                else
                    return false;
                #region 旧算法最近一天质差且过去六天至少两天质差
                //int count = 0;
                //for (int j = 0; j < data.Rows.Count; j++)
                //{
                //    if (data.Rows[j]["value_q"].ToString() == "false")
                //        ++count;
                //}
                //if (count >= 3)
                //    return false;
                #endregion
            }
            return true;
        }

        #region 异步线程处理代码
        String kpiindex = "";
        String reson = "";
        String measure = "";

        delegate String CalculateKPIMethod(String ECI,String time);
        CalculateKPIMethod kpiMethod;
        public String Calculatekpi(String eci, String time)
        {
            String kpiresult = "";
            KPIAnalysis kpi = new KPIAnalysis();
            if(time=="")
                time = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            List<String> list = kpi.GetLowIndex("KPI指标", time, eci);
            for (int i = 0; i < list.Count; i++)
            {
                if (String.IsNullOrEmpty(kpiindex))
                    kpiresult = list[i];
                else
                    kpiresult = kpiresult + "、" + list[i];
            }
            return kpiresult;
        }
        public void kpiTaskFinished(IAsyncResult result)
        {
            kpiindex = kpiMethod.EndInvoke(result);
        }

        delegate String CalculateMRMethod(String ECI, String time);
        CalculateMRMethod mrMethod;
        public String Calculatemr(String eci, String time)
        {
            String resonresult = "";
            KPIAnalysis kpi = new KPIAnalysis();
            MRAnalysis mr = new MRAnalysis();
            if(time=="")
                time = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            List<String> list = new List<string>();
            list = kpi.GetLowIndex("容量问题（小区自忙时）", time, eci);
            for (int i = 0; i < list.Count; i++)
            {
                if (String.IsNullOrEmpty(resonresult))
                    resonresult = list[i];
                else
                    resonresult = resonresult + "、" + list[i];
            }
            list.Clear();
            list = mr.GetLowIndex(time, eci);
            for (int i = 0; i < list.Count; i++)
            {

                if (String.IsNullOrEmpty(resonresult))
                    resonresult = list[i];
                else
                    resonresult = resonresult + "、" + list[i];
            }
            String dist = "1000";
            AlarmAnalysis analysis = new AlarmAnalysis();
            dist = analysis.getAnalysisdist(eci);
            String alarm = (new AlarmAnalysis()).GetData(eci, dist, time, "zcxq");
            String alarm2 = (new AlarmAnalysis()).GetData(eci, dist, time, "ljxq");
            if (!String.IsNullOrEmpty(alarm))
            {
                if (String.IsNullOrEmpty(resonresult))
                    resonresult = "本基站故障";
                else
                    resonresult = resonresult + "、" + "本基站故障";
            }
            if (!String.IsNullOrEmpty(alarm2))
            {
                if (String.IsNullOrEmpty(resonresult))
                    resonresult = "相邻基站故障";
                else
                    resonresult = resonresult + "、" + "相邻基站故障";
            }
            return resonresult;
        }
        public void mrTaskFinished(IAsyncResult result)
        {
            reson = mrMethod.EndInvoke(result);
        }

        delegate String CalculateMSMethod(String ECI, String time);
        CalculateMSMethod measureMethod;
        public String Calculatemeasure(String eci, String time)
        {
            String measureresult = "";
            if(time=="")
                time = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            measureresult = MeasureAnalysis.GetMeasure(eci, time);
            String dist = "1000";
            AlarmAnalysis analysis = new AlarmAnalysis();
            dist = analysis.getAnalysisdist(eci);
            String alarm = (new AlarmAnalysis()).GetData(eci, dist, time, "zcxq");
            String alarm2 = (new AlarmAnalysis()).GetData(eci, dist, time, "ljxq");
            if (!String.IsNullOrEmpty(alarm))
            {
                if (String.IsNullOrEmpty(measureresult))
                    measureresult = "处理告警";
                else
                    measureresult = measureresult + "、" + "处理告警";
            }
            if (!String.IsNullOrEmpty(alarm2))
            {
                if (String.IsNullOrEmpty(measureresult))
                    measureresult = "处理相邻基站告警";
                else
                    measureresult = measureresult + "、" + "处理相邻基站告警";
            }
            return measureresult;
        }
        public void measureTaskFinished(IAsyncResult result)
        {
            measure = measureMethod.EndInvoke(result);
        }
        #endregion

        public Boolean AnalysisAll()
        {
            try
            {
                String eci = "";
                String time =DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                KPIAnalysis kpi = new KPIAnalysis();
                MRAnalysis mr = new MRAnalysis();
                DataTable data = OraConnect.ReadData("select distinct(ECI) from DATA_KQIINFO t where t.createtime=to_date('" + time + "','yyyy-MM-dd')");
                if (data != null)
                {
                    for (int r = 0; r < data.Rows.Count; r++)
                    {
                        kpiindex = "";
                        reson = "";
                        measure = "";
                        eci = data.Rows[r]["ECI"].ToString();
                        if (String.IsNullOrEmpty(eci))
                            continue;
                        //var mytask = new Task(() =>
                        //{
                        //    IAsyncResult kpiasyncResult = null;
                        //    kpiMethod = new CalculateKPIMethod(Calculatekpi);
                        //    kpiasyncResult = kpiMethod.BeginInvoke(eci, new AsyncCallback(kpiTaskFinished), null);
                        //    while (!kpiasyncResult.IsCompleted)
                        //    {
                        //        Console.WriteLine("异步正在进行中");
                        //        Thread.Sleep(300);
                        //    }
                        //});
                        //mytask.Start();
                        //var result = mytask.ContinueWith<string>(task =>
                        //{
                        //    if (String.IsNullOrEmpty(kpiindex))
                        //        return "";
                        //    IAsyncResult mrasyncResult = null;
                        //    mrMethod = new CalculateMRMethod(Calculatemr);
                        //    mrasyncResult = mrMethod.BeginInvoke(eci, new AsyncCallback(mrTaskFinished), null);

                        //    IAsyncResult msResult = null;
                        //    measureMethod = new CalculateMSMethod(Calculatemeasure);
                        //    msResult = measureMethod.BeginInvoke(eci, new AsyncCallback(measureTaskFinished), null);

                        //    while (!mrasyncResult.IsCompleted || !msResult.IsCompleted)
                        //    {
                        //        Console.WriteLine("异步正在进行中");
                        //        Thread.Sleep(300);
                        //    }
                        //    if (String.IsNullOrEmpty(kpiindex))
                        //        kpiindex = "正常";
                        //    if (String.IsNullOrEmpty(reson))
                        //        kpiindex = "暂无";
                        //    if (String.IsNullOrEmpty(measure))
                        //        measure = "正常，无需处理";
                        //    String is800 = "0";
                        //    if (ECIAndIDConvert.getflowclass(eci))
                        //        is800 = "1";
                        //    bool b = OraConnect.ExecuteSQL("update DATA_KQIINFO t set kqiindex='" + kpiindex + "',reason='" + reson + "',MEASURES='" + measure + "',ISEIGHT='" + is800 + "' where eci='" + eci + "' and to_char(t.createtime,'yyyy-MM-dd')='" + time + "'");
                        //    return "";
                        //});

                        IAsyncResult kpiasyncResult = null;
                        kpiMethod = new CalculateKPIMethod(Calculatekpi);
                        kpiasyncResult = kpiMethod.BeginInvoke(eci,time, new AsyncCallback(kpiTaskFinished), null);

                        IAsyncResult mrasyncResult = null;
                        mrMethod = new CalculateMRMethod(Calculatemr);
                        mrasyncResult = mrMethod.BeginInvoke(eci, time, new AsyncCallback(mrTaskFinished), null);

                        IAsyncResult msResult = null;
                        measureMethod = new CalculateMSMethod(Calculatemeasure);
                        msResult = measureMethod.BeginInvoke(eci, time, new AsyncCallback(measureTaskFinished), null);

                        while (!kpiasyncResult.IsCompleted || !mrasyncResult.IsCompleted || !msResult.IsCompleted)
                        {
                            Console.WriteLine("异步正在进行中");
                            Thread.Sleep(300);
                        }
                        if (string.IsNullOrEmpty(kpiindex) && !string.IsNullOrEmpty(reson))
                        {
                            kpiindex = reson;
                        }
                        if (String.IsNullOrEmpty(kpiindex))
                        {
                            kpiindex = "";
                            reson = "";
                            measure = "";
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(reson))
                                reson = "";
                            if (String.IsNullOrEmpty(measure))
                                measure = "正常，无需处理";
                        }
                        if (!reson.Contains("弱覆盖") && !reson.Contains("过覆盖"))
                        {
                            measure.Replace("弱覆盖：核查工参，进行覆盖调整", "");
                        }
                        else if (!reson.Contains("弱覆盖") && reson.Contains("过覆盖"))
                        {
                            measure.Replace("弱覆盖：核查工参，进行覆盖调整", "过覆盖：核查工参，进行覆盖调整");
                        }
                        else if (reson.Contains("弱覆盖") && reson.Contains("过覆盖"))
                        {
                            measure.Replace("弱覆盖：核查工参，进行覆盖调整", "弱覆盖过覆盖：核查工参，进行覆盖调整");
                        }

                        String is800 = "0";
                        if (ECIAndIDConvert.getflowclass(eci))
                            is800 = "1";
                        
                        bool b = OraConnect.ExecuteSQL("update DATA_KQIINFO t set kqiindex='" + kpiindex + "',reason='" + reson + "',MEASURES='" + measure + "',ISEIGHT='" + is800 + "' where eci='" + eci + "' and to_char(t.createtime,'yyyy-MM-dd')='" + time + "'");
                    }

                    #region 废弃
                    //Parallel.For(0, data.Rows.Count, (i) =>
                    //{
                    //    kpiindex = "";
                    //    reson = "";
                    //    measure = "";
                    //    eci = data.Rows[i]["ECI"].ToString();
                    //    if (!String.IsNullOrEmpty(eci))
                    //    {
                    //        IAsyncResult kpiasyncResult = null;
                    //        kpiMethod = new CalculateKPIMethod(Calculatekpi);
                    //        kpiasyncResult = kpiMethod.BeginInvoke(eci, new AsyncCallback(kpiTaskFinished), null);

                    //        IAsyncResult mrasyncResult = null;
                    //        mrMethod = new CalculateMRMethod(Calculatemr);
                    //        mrasyncResult = mrMethod.BeginInvoke(eci, new AsyncCallback(mrTaskFinished), null);

                    //        IAsyncResult msResult = null;
                    //        measureMethod = new CalculateMSMethod(Calculatemeasure);
                    //        msResult = measureMethod.BeginInvoke(eci, new AsyncCallback(measureTaskFinished), null);

                    //        while (!kpiasyncResult.IsCompleted || !mrasyncResult.IsCompleted || !msResult.IsCompleted)
                    //        {
                    //            Console.WriteLine("异步正在进行中");
                    //            Thread.Sleep(300);
                    //        }
                    //        bool b = OraConnect.ExecuteSQL("update DATA_KQIINFO set kqiindex='" + kpiindex + "',reason='" + reson + "',MEASURES='" + measure + "' where eci='" + eci + "' and createtime=to_date('" + time + "','yyyy-MM-dd')");
                    //    }
                    //});
                    //    //List<String> list = new List<string>();
                    //    //list = kpi.GetLowIndex("容量问题（小区自忙时）", time, eci);
                    //    //for (int i = 0; i < list.Count; i++)
                    //    //{
                    //    //    if (String.IsNullOrEmpty(reson))
                    //    //        reson = list[i];
                    //    //    else
                    //    //        reson = reson + "、" + list[i];
                    //    //}
                    //    //list.Clear();
                    //    //list = mr.GetLowIndex(time, eci);
                    //    //for (int i = 0; i < list.Count; i++)
                    //    //{

                    //    //    if (String.IsNullOrEmpty(reson))
                    //    //        reson = list[i];
                    //    //    else
                    //    //        reson = reson + "、" + list[i];
                    //    //}
                    //    //measure = MeasureAnalysis.GetMeasure(eci, time);
                    //    //String alarm = (new AlarmAnalysis()).GetData(eci, "800", time, "zcxq");
                    //    //String alarm2 = (new AlarmAnalysis()).GetData(eci, "800", time, "ljxq");
                    //    //if (!String.IsNullOrEmpty(alarm) || !String.IsNullOrEmpty(alarm2))
                    //    //{
                    //    //    if (String.IsNullOrEmpty(reson))
                    //    //        reson = "告警";
                    //    //    else
                    //    //        reson = reson + "、" + "告警";
                    //    //    if (String.IsNullOrEmpty(measure))
                    //    //        reson = "处理告警";
                    //    //    else
                    //    //        reson = reson + "、" + "处理告警";
                    //    //}
                    //    bool b = OraConnect.ExecuteSQL("update DATA_KQIINFO set kqiindex='" + kpiindex + "',reason='" + reson + "',MEASURES='" + measure + "' where eci='" + eci + "' and createtime=to_date('" + time + "','yyyy-MM-dd')");
                    //}
                    #endregion
                }
                return true;
            }
            catch (Exception err)
            {
                LogHelper.WriteErrorLog(err.ToString(), null, true, true, AssemblyHelper.GetDomainBaseDirectory() + "\\log");
                return false;
            }
        }
    }
}