using DWServices.BLL;
using DWServices.Common;
using DWServices.DAL;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// RadarChartServices 的摘要说明
    /// </summary>
    public class RadarChartServices : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        //String[] indexstr = { "xqpjrsrp", "rfgbl", "cdfgk", "gfglqgs", 
        //                        "modgrcd", "sxprbpjlyl18", "xxprbpjlyl18", "pjrrcljyhs18", 
        //                        "pdcpxx18", "pdcpsx18", "pjjhyhs18" };
        String[] indexstr = { "rfgbl", "cdfgk", "gfglqgs", 
                                "sxprbpjlyl18", "xxprbpjlyl18", "pjrrcljyhs18", 
                                "pdcpxx18", "pdcpsx18","modgrcd", "sxgr" };

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //,System.Web.SessionState.IRequiresSessionState
            DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            if (user == null)
            {
                context.Response.Write("");
                return;
            }
            //double xqpjrsrp = 0;
            double rfgbl = 0;
            double cdfgk = 0;
            double gfglqgs = 0;
            double sxprbpjlyl18 = 0;
            double xxprbpjlyl18 = 0;
            double pjrrcljyhs18 = 0;
            double pdcpxx18 = 0;
            double pdcpsx18 = 0;
            //double pjjhyhs18 = 0;
            double modgrcd = 0;
            double sxgr = 0;
            double bgjscore = 0;
            double lgjscore = 0;
            int longtime = 0;
            PostParameter paramter = PostParameter.getParameter(context);
            if(String.IsNullOrEmpty(paramter.ECI))
            {
                if(String.IsNullOrEmpty(paramter.ENBID)||String.IsNullOrEmpty(paramter.LCRID))
                    return;
                else
                    paramter.ECI=(Convert.ToInt32(paramter.ENBID)*256+Convert.ToInt32(paramter.LCRID)).ToString();
            }
            String result = "";
            try
            {
                AlarmAnalysis alarm = new AlarmAnalysis();
                KPIAnalysis kpi = new KPIAnalysis();
                MRAnalysis mr = new MRAnalysis();
                Analysis anal = new Analysis();
                DataTable quotadata = OraConnect.ReadData("select * from DATA_QUOTA t  where mykey in ('xqpjrsrp','rfgbl','cdfgk','gfglqgs','modgrcd','sxprbpjlyl18','xxprbpjlyl18','pjrrcljyhs18','pdcpxx18','pdcpsx18','pjjhyhs18')");
                for (int i = 0; i < quotadata.Rows.Count; i++)
                {
                    String mfild = quotadata.Rows[i]["MFIELD"].ToString();
                    String quota = quotadata.Rows[i]["mykey"].ToString();
                    String quotaname = quotadata.Rows[i]["thequota"].ToString();
                    #region 原算法指标小区平均RSRP
                    //if (quota == "xqpjrsrp")
                    //{
                    //    double minpjrsrp = -126; double maxpjrsrp = -105;
                    //    DataTable data = mr.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, true);
                    //    if (data == null)
                    //        continue;
                    //    if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                    //        continue;
                    //    if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                    //    {
                    //        double v = Convert.ToDouble(data.Rows[0][0].ToString());
                    //        if (v < minpjrsrp)
                    //            xqpjrsrp = 100;
                    //        else
                    //        {
                    //            double temp = (v - minpjrsrp)*100 / (maxpjrsrp - minpjrsrp);
                    //            if (temp > xqpjrsrp)
                    //                xqpjrsrp = temp;
                    //        }
                    //    }
                    //}
                    #endregion
                    if (quota == "rfgbl")
                    {
                        double minlfgbl = 0.2; double maxlfgbl = 1;
                        DataTable data = mr.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, true);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v > maxlfgbl)
                                rfgbl = 100;
                            else if (v < minlfgbl)
                                rfgbl = 0;
                            else
                            {
                                double temp = (v - minlfgbl) * 100 / (maxlfgbl - minlfgbl);
                                if (temp > rfgbl)
                                    rfgbl = temp;
                            }
                        }
                    }
                    else if (quota == "cdfgk")
                    {
                        double mincdfgl = 0.2; double maxcdfgl = 1;
                        DataTable data = mr.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v >= maxcdfgl)
                                cdfgk = 100;
                            else if (v < mincdfgl)
                                cdfgk = 0;
                            else
                            {
                                double temp = (maxcdfgl - v) * 100 / (maxcdfgl - mincdfgl);
                                if (temp > cdfgk)
                                    cdfgk = temp;
                            }
                        }
                    }
                    else if (quota == "gfglqgs")
                    {
                        double mingfglqgs = 11; double maxgfglqgs = 20;
                        DataTable data = mr.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v >= maxgfglqgs)
                                gfglqgs = 100;
                            else if (v <= mingfglqgs)
                                gfglqgs = 0;
                            else
                            {
                                //double temp = (maxgfglqgs - v) * 100 / (maxgfglqgs - mingfglqgs);
                                double temp = (v - mingfglqgs) * 100 / (maxgfglqgs - mingfglqgs);
                                if (temp > gfglqgs)
                                    gfglqgs = temp;
                            }
                        }
                    }
                    else if (quota == "modgrcd")
                    {
                        double minmod = 0.2; double maxmod = 0.8;
                        DataTable data = mr.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v >= maxmod)
                                modgrcd = 100;
                            else if (v <= minmod)
                                modgrcd = 0;
                            else
                            {
                                double temp = (v-minmod) * 100 / (maxmod - minmod);
                                if (temp > modgrcd)
                                    modgrcd = temp;
                            }
                        }
                    }
                    #region 新增指标无数据暂定为0分
                    else if (quota == "sxgr")
                    {
                        try
                        {
                            double minsxgr = 0; double maxsxgr = 0;
                            DataTable data = mr.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                            if (data == null)
                                continue;
                            if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                                continue;
                            if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                            {
                                double v = Convert.ToDouble(data.Rows[0][0].ToString());
                                if (v >= maxsxgr)
                                    sxgr = 100;
                                else
                                {
                                    double temp = (maxsxgr - v) * 100 / (maxsxgr - minsxgr);
                                    if (temp > sxgr)
                                        sxgr = temp;
                                }
                            }
                        }
                        catch
                        {
                            sxgr = 0;
                        }
                        
                    }
                    #endregion
                    else if (quota == "sxprbpjlyl18")
                    {
                        double minsxprb = 0.3; double maxsxprb = 1;
                        DataTable data = kpi.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v >= maxsxprb)
                                sxprbpjlyl18 = 100;
                            else if (v <= minsxprb)
                                sxprbpjlyl18 = 0;
                            else
                            {
                                double temp = (maxsxprb - v) * 100 / (maxsxprb - minsxprb);
                                if (temp > sxprbpjlyl18)
                                    sxprbpjlyl18 = temp;
                            }
                        }
                    }
                    else if (quota == "xxprbpjlyl18")
                    {
                        double minxxprb = 0.3; double maxxxprb = 1;
                        DataTable data = kpi.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v >= maxxxprb)
                                xxprbpjlyl18 = 100;
                            else if (v <= minxxprb)
                                xxprbpjlyl18 = 0;
                            else
                            {
                                double temp = (maxxxprb - v) * 100 / (maxxxprb - minxxprb);
                                if (temp > xxprbpjlyl18)
                                    xxprbpjlyl18 = temp;
                            }
                        }
                    }
                    else if (quota == "pjrrcljyhs18")
                    {
                        double minpjrrc = 50; double maxpjrrc = 1000;
                        if (ECIAndIDConvert.getflowclass(paramter.ECI))
                        {
                            minpjrrc = 30;
                            maxpjrrc = 300;
                        }
                        DataTable data = kpi.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v > maxpjrrc)
                                pjrrcljyhs18 = 100;
                            else if (v <= minpjrrc)
                                pjrrcljyhs18 = 0;
                            else
                            {
                                double temp = (maxpjrrc - v) * 100 / (maxpjrrc - minpjrrc);
                                if (temp > pjrrcljyhs18)
                                    pjrrcljyhs18 = temp;
                            }
                        }
                    }
                    else if (quota == "pdcpxx18")
                    {
                        double minxxpdcp = 4096; double maxxxpdcp = 40960;
                        if (ECIAndIDConvert.getflowclass(paramter.ECI))
                        {
                            minxxpdcp = 1024;
                            maxxxpdcp = 10240;
                        }
                        DataTable data = kpi.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v > maxxxpdcp)
                                pdcpxx18 = 100;
                            else if (v <= minxxpdcp)
                                pdcpxx18 = 0;
                            else
                            {
                                double temp = (maxxxpdcp - v) * 100 / (maxxxpdcp - minxxpdcp);
                                if (temp > pdcpxx18)
                                    pdcpxx18 = temp;
                            }
                        }
                    }
                    else if (quota == "pdcpsx18")
                    {
                        double minsxpdcp = 1536; double maxsxpdcp = 15360;
                        if (ECIAndIDConvert.getflowclass(paramter.ECI))
                        {
                            minsxpdcp = 409.6; 
                            maxsxpdcp = 4096;
                        }
                        DataTable data = kpi.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                        if (data == null)
                            continue;
                        if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                            continue;
                        if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                        {
                            double v = Convert.ToDouble(data.Rows[0][0].ToString());
                            if (v > maxsxpdcp)
                                pdcpsx18 = 100;
                            else if (v <= minsxpdcp)
                                pdcpsx18 = 0;
                            else
                            {
                                double temp = (maxsxpdcp - v) * 100 / (maxsxpdcp - minsxpdcp);
                                if (temp > pdcpsx18)
                                    pdcpsx18 = temp;
                            }
                        }
                    }
                    #region 原算法指标平均激活用户数
                    //else if (quota == "pjjhyhs18")
                    //{
                    //    double minpjjhyhs = 14; double maxpjjhyhs = 100;
                    //    DataTable data = kpi.GetLowData(quotadata.Rows[i]["MFIELD"].ToString(), paramter.ECI, paramter.QuertyTime, longtime, false);
                    //    if (data == null)
                    //        continue;
                    //    if (String.IsNullOrEmpty(data.Rows[0][0].ToString()))
                    //        continue;
                    //    if (!getRowStatic(quotadata.Rows[i], data.Rows[0][0].ToString()))
                    //    {
                    //        double v = Convert.ToDouble(data.Rows[0][0].ToString());
                    //        if (v > maxpjjhyhs)
                    //            pjjhyhs18 = 100;
                    //        else
                    //        {
                    //            double temp = (maxpjjhyhs - v) * 100 / (maxpjjhyhs - minpjjhyhs);
                    //            if (temp > pjjhyhs18)
                    //                pjjhyhs18 = temp;
                    //        }
                    //    }
                    //}
                    #endregion
                    else { }
                }

                if (!String.IsNullOrEmpty(alarm.GetData(paramter.ECI, "800", paramter.QuertyTime, "zcxq")))
                {
                    bgjscore = 100;
                }
                else if (!String.IsNullOrEmpty(alarm.GetData(paramter.ECI, "800", paramter.QuertyTime, "ljxq")))
                {
                    lgjscore = 100;
                }
            }
            catch { }
            result = rfgbl.ToString("0") + "," + cdfgk.ToString("0") + "," + gfglqgs.ToString("0") + "," 
                + sxprbpjlyl18.ToString("0") + "," + xxprbpjlyl18.ToString("0") + "," + pjrrcljyhs18.ToString("0") + "," + pdcpxx18.ToString("0") + ","
                + pdcpsx18.ToString("0") + "," + modgrcd.ToString("0") + "," + sxgr.ToString("0") + "," + bgjscore.ToString("0") + "," + lgjscore.ToString("0");
            context.Response.Write(result);
        }

        private Boolean getRowStatic(DataRow row,String val)
        {
            double max=0, min=0;
            Boolean havemax = false;
            Boolean havemin = false;
            if (row["maxvalue"] != null)
            {
                String value = row["maxvalue"].ToString();
                if (value != "")
                {
                    max = Convert.ToDouble(value);
                    havemax = true;
                }
            }
            if (row["minvalue"] != null)
            {
                String value = row["minvalue"].ToString();
                if (value != "")
                {
                    min = Convert.ToDouble(value);
                    havemin = true;
                }
            }

            double vel = 0; ;
            if (val.Contains("%"))
            {
                val = val.Replace("%", "");
                vel = Convert.ToDouble(val) / 100;
            }
            else
                vel = Convert.ToDouble(val);
            if (havemax && havemin)
            {
                if (vel >= min && vel <= max)
                {
                    return false;
                }
                else
                    return true;
            }
            else if (havemax || havemin)
            {
                if (havemin)
                {
                    if (vel >= min)
                    {
                        return false;
                    }
                    else
                        return true;
                }
                else
                {
                    if (vel <= max)
                    {
                        return false;
                    }
                    else
                        return true;
                }
            }
            else
            {
                return true;
            }
        }

        

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}