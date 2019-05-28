using DWServices.Common;
using DWServices.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.BLL
{
    public class MeasureAnalysis
    {
        public static String GetMeasure(String ECI, String time)
        {
            String result = "";
            KPIAnalysis kpi = new KPIAnalysis();
            double DOWN_PRB = 0;
            double UP_PRB = 0;
            double DOWN_PDCP = 0; 
            double UP_PDCP = 0;
            double RRC = 0;
            DateTime dt = DateTime.Now.AddDays(-1);
            if (!String.IsNullOrEmpty(time))
            {
                dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                if (dt == DateTime.Now.Date)
                    dt = DateTime.Now.AddDays(-1);
            }
            else
                return "";
            DateTime endtime = dt.AddDays(1);
            //DataTable kpidata = OraConnect.ReadData("select * from (select DOWN_PRB,UP_PRB,DOWN_PDCP,UP_PDCP,RRC_CONNCOUNT from DATA_KPIINFO t "
            //    + " where t.time=to_date('" + time + "','yyyy-MM-dd') and eci='" + ECI + "' order by DOWN_PRB desc) where rownum <=1");

            DataTable kpidata = OraConnect.ReadData("select max(DOWN_PRB) as value from DATA_KPIINFO t "
                + " where t.time>=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and t.time<=to_date('" + endtime.ToString("yyyy-MM-dd") 
                + "','yyyy-MM-dd') and eci='" + ECI + "'");

            if (kpidata != null)
                if (kpidata.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(kpidata.Rows[0]["value"].ToString()))
                        DOWN_PRB = Convert.ToDouble(kpidata.Rows[0]["value"].ToString());
                }
            kpidata = OraConnect.ReadData("select max(UP_PRB) as value from DATA_KPIINFO t "
                + " where t.time>=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and t.time<=to_date('" + endtime.ToString("yyyy-MM-dd")
                + "','yyyy-MM-dd') and eci='" + ECI + "'");
            if (kpidata != null)
                if (kpidata.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(kpidata.Rows[0]["value"].ToString()))
                        UP_PRB = Convert.ToDouble(kpidata.Rows[0]["value"].ToString());
                }
            kpidata = OraConnect.ReadData("select max(DOWN_PDCP) as value from DATA_KPIINFO t "
                + " where t.time>=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and t.time<=to_date('" + endtime.ToString("yyyy-MM-dd")
                + "','yyyy-MM-dd') and eci='" + ECI + "'");
            if (kpidata != null)
                if (kpidata.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(kpidata.Rows[0]["value"].ToString()))
                        DOWN_PDCP = Convert.ToDouble(kpidata.Rows[0]["value"].ToString());
                }
            kpidata = OraConnect.ReadData("select max(UP_PDCP) as value from DATA_KPIINFO t "
                + " where t.time>=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and t.time<=to_date('" + endtime.ToString("yyyy-MM-dd")
                + "','yyyy-MM-dd') and eci='" + ECI + "'");
            if (kpidata != null)
                if (kpidata.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(kpidata.Rows[0]["value"].ToString()))
                        UP_PDCP = Convert.ToDouble(kpidata.Rows[0]["value"].ToString());
                }
            kpidata = OraConnect.ReadData("select max(RRC_CONNCOUNT) as value from DATA_KPIINFO t "
                + " where t.time>=to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-MM-dd') and t.time<=to_date('" + endtime.ToString("yyyy-MM-dd")
                + "','yyyy-MM-dd') and eci='" + ECI + "'");
            if (kpidata != null)
                if (kpidata.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(kpidata.Rows[0]["value"].ToString()))
                        RRC = Convert.ToDouble(kpidata.Rows[0]["value"].ToString());
                }
            result = RLMeasure(ECI, DOWN_PRB, UP_PRB, DOWN_PDCP, UP_PDCP, RRC);



            DataTable mrdata = OraConnect.ReadData("select a.DIPANGLE,t.LTESCRSRP,t.RSRP_LOW_RATE,t.TADV_COUNT_SUM,t.CMCC_OVERLAP_RATE,"
                +"t.CMCC_OVERSHOOT_ADJ_COUNT,t.MRO_RATE_MOD3 from DATA_MR t,V_WORKPARAMETER a where t.eci=a.eci and t.eci='" 
                + ECI + "' and t.sdate = to_date('" + time + "','yyyy-MM-dd')");
            if (mrdata != null)
            {
                if (mrdata.Rows.Count > 0)
                {
                    double RSRP = 0;
                    if (!String.IsNullOrEmpty(mrdata.Rows[0]["LTESCRSRP"].ToString()))
                        RSRP = Convert.ToDouble(mrdata.Rows[0]["LTESCRSRP"].ToString());

                    double FGL = 0;
                    if (!String.IsNullOrEmpty(mrdata.Rows[0]["RSRP_LOW_RATE"].ToString()))
                        FGL = Convert.ToDouble(mrdata.Rows[0]["RSRP_LOW_RATE"].ToString());
                    double tadist = 0;
                    if (!String.IsNullOrEmpty(mrdata.Rows[0]["TADV_COUNT_SUM"].ToString()))
                        tadist = Convert.ToDouble(mrdata.Rows[0]["TADV_COUNT_SUM"].ToString());
                    double dipangle = 0;
                    if (!String.IsNullOrEmpty(mrdata.Rows[0]["DIPANGLE"].ToString()))
                        dipangle = Convert.ToDouble(mrdata.Rows[0]["DIPANGLE"].ToString());
                    double cdfgl = 0;
                    if (!String.IsNullOrEmpty(mrdata.Rows[0]["CMCC_OVERLAP_RATE"].ToString()))
                        cdfgl = Convert.ToDouble(mrdata.Rows[0]["CMCC_OVERLAP_RATE"].ToString());
                    double gfglqs = 0;
                    if (!String.IsNullOrEmpty(mrdata.Rows[0]["CMCC_OVERSHOOT_ADJ_COUNT"].ToString()))
                        gfglqs = Convert.ToDouble(mrdata.Rows[0]["CMCC_OVERSHOOT_ADJ_COUNT"].ToString());
                    double mod3 = 0;
                    if (!String.IsNullOrEmpty(mrdata.Rows[0]["MRO_RATE_MOD3"].ToString()))
                        mod3 = Convert.ToDouble(mrdata.Rows[0]["MRO_RATE_MOD3"].ToString());
                    //修改过覆盖、弱覆盖、重叠覆盖关系先后顺序
                    var rfg = FGMeasure(RSRP, FGL, tadist, dipangle);
                    var gfg = GFGMeasure(cdfgl, gfglqs, mod3, dipangle);
                    var grs = GRMeasure(cdfgl, gfglqs, mod3, dipangle);
                    var cdfg = CDFG(cdfgl);

                    if (!string.IsNullOrEmpty(gfg) && (!string.IsNullOrEmpty(rfg) || !string.IsNullOrEmpty(cdfg))) {
                        gfg = "优先解决" + gfg + "，观察调整效果后再解决";
                        //rfg=string.IsNullOrEmpty(rfg)?"":("观察后再解决"+rfg);
                        //cdfg = string.IsNullOrEmpty(cdfg) ? "" : ("观察后再解决" + cdfg);
                        if (!string.IsNullOrEmpty(rfg)) {
                            gfg += "弱覆盖";
                            rfg = "";
                        }
                        if (!string.IsNullOrEmpty(cdfg))
                        {
                            gfg += "重叠覆盖";
                            cdfg = "";
                        }
                    }
                    List<string> fggr = new List<string>();
                    if (!string.IsNullOrEmpty(gfg)) {
                        fggr.Add(gfg);
                    }
                    if (!string.IsNullOrEmpty(rfg))
                    {
                        fggr.Add(rfg);
                    }
                    if (!string.IsNullOrEmpty(cdfg))
                    {
                        fggr.Add(cdfg);
                    }
                    if (!string.IsNullOrEmpty(grs))
                    {
                        fggr.Add(grs);
                    }
                    var fggrstr = string.Join("、",fggr);
                    if (String.IsNullOrEmpty(result))
                        result = fggrstr;
                    else
                        result = result + "<br/>" + fggrstr;

                    //String fg = FGMeasure(RSRP, FGL, tadist, dipangle);
                    //String gr = GLMeasure(cdfgl, gfglqs, mod3, dipangle);
                    //if (String.IsNullOrEmpty(result))
                    //    result = fg;
                    //else
                    //    result = result + "<br/>" + fg;
                    //if (String.IsNullOrEmpty(result))
                    //    result = gr;
                    //else
                    //{
                    //    if (String.IsNullOrEmpty(fg))
                    //        result = result + gr;
                    //    else
                    //    {
                    //        result = result + "、" + gr;
                    //    }
                    //}
                }
                else
                {
                    DataTable gcdata = OraConnect.ReadData("select a.DIPANGLE from V_WORKPARAMETER a where eci='" + ECI + "'");
                    if (gcdata != null)
                    {
                        if (gcdata.Rows.Count == 0)
                        {
                            if (String.IsNullOrEmpty(result))
                                result = "弱覆盖：核查工参，进行覆盖调整";
                            else
                                result = result + "<br/>" + "弱覆盖：核查工参，进行覆盖调整";
                            DataTable cddata = OraConnect.ReadData("select t.CMCC_OVERLAP_RATE from DATA_MR t where t.eci='" + ECI + "' and t.sdate = to_date('" + time + "','yyyy-MM-dd')");
                            double cdfgl = 0;
                            try
                            {
                                if (!String.IsNullOrEmpty(cddata.Rows[0]["CMCC_OVERLAP_RATE"].ToString()))
                                    cdfgl = Convert.ToDouble(cddata.Rows[0]["CMCC_OVERLAP_RATE"].ToString());
                            }
                            catch { }
                            String cdfgjl = CDFG(cdfgl);
                            if (String.IsNullOrEmpty(result))
                                result = cdfgjl;
                            else
                            {
                                if(!String.IsNullOrEmpty(cdfgjl))
                                    result = result + "、" + CDFG(cdfgl);
                            }
                            
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 容量判决
        /// </summary>
        /// <param name="PRB">下行PRB利用率（注：35%相当于35）</param>
        /// <param name="cellLL">小区流量</param>
        /// <param name="RRC">平均RRC连接数</param>
        /// <returns></returns>
        public static String RLMeasure(String eci,double DOWN_PRB,double UP_PRB,double DOWN_PDCP,double UP_PDCP,double RRC)
        {
            Boolean Is800M = false;
            Is800M = ECIAndIDConvert.getflowclass(eci);
            int count = 0;
            if (Is800M)
            {
                if (DOWN_PRB >= 50 || UP_PRB >= 50 || UP_PDCP >= 0.75 * 1024.0 || DOWN_PDCP >= 2 * 1024 || RRC >= 200)
                    return "容量：达到扩容标准，进行小区分裂，载频扩容或新建基站分流话务";
                else
                {
                    if (DOWN_PRB >= 30)
                        count = count + 1;
                    if (UP_PRB >= 30)
                        count = count + 1;
                    if (UP_PDCP >= 0.4*1024)
                        count = count + 1;
                    if (DOWN_PDCP >= 1024)
                        count = count + 1;
                    if (RRC >= 30)
                        count = count + 1;
                    return getMeasureTxt(count);
                }
            }
            else
            {
                if (DOWN_PRB >= 50 || UP_PRB >= 50 || UP_PDCP >= 3 * 1024 || DOWN_PDCP >= 8 * 1024 || RRC >= 200)
                    return "容量：达到扩容标准，进行小区分裂，载频扩容或新建基站分流话务";
                else
                {
                    if (DOWN_PRB >= 30)
                        count = count + 1;
                    if (UP_PRB >= 30)
                        count = count + 1;
                    if (UP_PDCP >= 1.5 * 1024)
                        count = count + 1;
                    if (DOWN_PDCP >= 4*1024)
                        count = count + 1;
                    if (RRC >= 50)
                        count = count + 1;
                    return getMeasureTxt(count);
                }
            }

            #region 原判决方法
            //if (PRB>=35)
            //{
            //    if (cellLL >= 4)
            //    {
            //        if (RRC >= 40)
            //            return "小区硬件扩容，如载波扩容";
            //        else
            //            return "小区整体流量较大，PRB负荷较高，用户流量需求被压抑，可考虑进行硬件扩容";
            //    }
            //    else
            //    {
            //        if (RRC >= 40)
            //            return "继续观察";
            //        else
            //            return "PRB利用率高，说明空口资源利用率较高，但小区流量或激活用户数不高，可能是较多用户处于边缘，结合用户TA分布，进行覆盖分析";
            //    }
            //}
            //else
            //{
            //    if (cellLL >= 4)
            //    {
            //        if (RRC >= 40)
            //            return "继续观察";
            //        else
            //            return "小区流量超门限，为流量价值区；若PRB利用率不高，可能较多用户处于小区中心，继续观察指标";
            //    }
            //    else
            //    {
            //        if (RRC >= 40)
            //            return "RRC连接数据超门限，但PRB利用率不高，可能用户短时拥塞，继续观察指标";
            //        else
            //            return "不需要调整";
            //    }
            //}
            #endregion
        }

        private static string getMeasureTxt(int count)
        {
            if (count == 0)
                return "";
            else if (count == 1)
                return "容量：一项标准达到预警，密切观察";
            else if (count == 2)
                return "容量：两项标准达到预警，天馈/功率调整优化小区覆盖范围，如效果不明显，载波扩容或新建基站";
            else
                return "容量：三项标准达到预警，进行小区分裂，载频扩容或新建基站分流话务";
        }

        /// <summary>
        /// 覆盖判决
        /// </summary>
        /// <param name="RSRP">小区级平均RSRP</param>
        /// <param name="FGL">弱覆盖比例%</param>
        /// <param name="tadist">平均TA距离</param>
        /// <param name="dipangle">下倾角</param>
        /// <returns></returns>
        public static String FGMeasure(double RSRP, double FGL, double tadist, double dipangle)
        {
            FGL = FGL * 100;
            //改!!!
            //if (FGL >= 0 && FGL <= 20)
            if (FGL > 0 && FGL <= 20)
            {
                if (RSRP <= -105)
                {
                    if (dipangle >= 5)
                        return "弱覆盖：减少下倾角2°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
                else
                    return "";
            }
            else if (FGL > 20 && FGL <= 40)
            {
                if (dipangle >= 5)
                    //return "弱覆盖：减少下倾角4°或功率提升一倍，增加boost3db";
                    return "弱覆盖：减少下倾角4°或功率提升一倍";
                else
                    return "弱覆盖：功率提升一倍";
            }
            else if (FGL > 40 && FGL <= 60)
            {
                if (tadist <=1)
                {
                    if (dipangle >= 7)
                        //return "弱覆盖：减少下倾角4°或功率提升一倍，增加boost3db";
                        return "弱覆盖：减少下倾角4°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
                else
                {
                    if (dipangle >= 7)
                        //return "弱覆盖：减少下倾角6°或功率提升一倍，增加boost3db";
                        return "弱覆盖：减少下倾角6°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
            }
            else if (FGL > 60 && FGL <= 100)
            {
                if (tadist <= 1)
                {
                    if (dipangle >= 7)
                        //return "弱覆盖：减少下倾角6°或功率提升一倍，增加boost3db";
                        return "弱覆盖：减少下倾角6°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
                else
                {
                    if (dipangle >= 7)
                        //return "弱覆盖：减少下倾角6°或功率提升一倍，增加boost3db";
                        return "弱覆盖：减少下倾角6°或功率提升一倍";
                    else
                        return "弱覆盖：功率提升一倍";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 干扰判决
        /// </summary>
        /// <param name="cdfgl">重覆盖率（6db）</param>
        /// <param name="gfglqs">过覆盖领区个数</param>
        /// <param name="mod3">MOD3干扰比例</param>
        /// <param name="dipangle">下倾角</param>
        /// <returns></returns>
        public static String GLMeasure(double cdfgl, double gfglqs, double mod3, double dipangle)
        {
            String result = CDFG(cdfgl);
            if (gfglqs >= 12)
            {
                if (dipangle < 7)
                {
                    if (String.IsNullOrEmpty(result))
                        result = "过覆盖：增加下倾角3°或功率降低一倍";
                    else
                        result = result + "、过覆盖：增加下倾角3°或功率降低一倍";
                }
                else
                {
                    if (String.IsNullOrEmpty(result))
                        result = "过覆盖：功率降低一倍";
                    else
                        result = result + "、过覆盖：功率降低一倍";
                }
            }
            if (mod3 >= 20)
            {
                if (String.IsNullOrEmpty(result))
                    result = "干扰：建议分析PCI模3干扰，根据PCI优化工具进行调整";
                else
                    result = result + "、干扰：建议分析PCI模3干扰，根据PCI优化工具进行调整";
            }
            return result;
        }
        public static String GFGMeasure(double cdfgl, double gfglqs, double mod3, double dipangle)
        {
            var result = "";
            if (gfglqs >= 12)
            {
                if (dipangle < 7)
                {
                    result = "过覆盖：增加下倾角3°或功率降低一倍";
                }
                else
                {
                    result = "过覆盖：功率降低一倍";
                }
            }
            return result;
        }
        public static String GRMeasure(double cdfgl, double gfglqs, double mod3, double dipangle)
        {
            mod3 = mod3 * 100;
            var result = "";
            if (mod3 >= 20)
            {
                result = "干扰：建议分析PCI模3干扰，根据PCI优化工具进行调整";
            }
            return result;
        }
        public static String CDFG(double cdfg)
        {
            cdfg = cdfg * 100.00;
            if (cdfg >= 20)
                return "重叠覆盖：周边基站RF优化";
            else
                return "";
        }
    }
}