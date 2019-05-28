using DWServices.Common;
using DWServices.DAL;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.BLL
{
    public class AlarmAnalysis
    {
        public String GetData(String eci,string dist,String time,String key)
        {
            String result = "";
            if (!String.IsNullOrEmpty(time))
            {
                DateTime dt = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                if (dt == DateTime.Now.Date)
                    time = DateTime.Now.AddDays(-1).ToString();
            }
            else
            {
                return result;
            }
            if (key == "zcxq")
            {
                List<long> id = ECIAndIDConvert.ConvertToEnbid(eci);
                result = GetAlarmOne(time,id[0].ToString(), id[1].ToString());
            }
            else if (key == "ljxq")
            {
                result = GetAroundAlarm(time,eci, dist);
            }
            return result;
        }

        public String GetAroundAlarm(String time,String cei,string dist)
        {
            String result = "";
            List<long> id;
            DataTable sitedata = OraConnect.ReadData("select t.eci,t.sc_lon,t.sc_lat from V_WORKPARAMETER t where t.eci=" + cei + "");
            if (sitedata != null)
            {
                if (sitedata.Rows.Count > 0)
                {
                    double ddist = 1000;
                    if (String.IsNullOrEmpty(dist))
                        ddist = Convert.ToDouble(dist);
                    double lon = Convert.ToDouble(sitedata.Rows[0]["sc_lon"].ToString());
                    double lat = Convert.ToDouble(sitedata.Rows[0]["sc_lat"].ToString());
                    Latlon latlon = new Latlon(lon, lat);
                    List<String> around = SearchAround.getAroundSite(latlon, ddist);
                    String AllECI = "";
                    String AllENBID = "";
                    for (int i = 0; i < around.Count; i++)
                    {
                        id = ECIAndIDConvert.ConvertToEnbid(around[i]);
                        if (String.IsNullOrEmpty(AllECI))
                        {
                            AllECI = "'" + around[i] + "'";
                            AllENBID = "'" + id[0].ToString() + "'";
                        }
                        else
                        {
                            if (AllECI.Contains(around[i]))
                                continue;
                            AllECI = AllECI+",'" + around[i] + "'";
                            if(AllENBID.Contains(id[0].ToString()))
                                continue;
                            AllENBID = AllENBID+",'" + id[0].ToString() + "'";
                        }
                    }
                    if(!String.IsNullOrEmpty(AllECI)&&!String.IsNullOrEmpty(AllENBID))
                        result=GetAlarmMore(time,AllECI, AllENBID);
                }

            }
            return result;
        }

        public String GetAlarmOne(String time,String enbid,String lcrid)
        {
            String result = "";
            String sqlstr = "select t.sc_enbid,t.sc_lcrid,t.eci,t.city,t.manufactor,t.alarmcode,a.alarmcontext,a.influence,t.createtime,t.cleartime,t.clearstatic "
                + "from DATA_ALARMINFO t,DATA_ALARM a where t.alarmcode = a.alarmcode and ((t.sc_enbid='" + enbid + "' and t.sc_lcrid='" + lcrid + "') or (t.sc_enbid='" 
                + enbid + "' and t.sc_lcrid is null)) and to_char(createtime, 'yyyy-MM-dd')='" + time + "'";
            DataTable Alarmdata = OraConnect.ReadData(sqlstr);
            if(Alarmdata!=null)
            {
                if(Alarmdata.Rows.Count>0){
                    result = DataTableConvertJson.DataTableToJson("data", Alarmdata);
                }
            }
            return result;
        }
        public String GetAlarmMore(String time,String ecis, String enbids)
        {
            String result = "";
            String sqlstr = "select t.sc_enbid,t.sc_lcrid,t.eci,t.city,t.manufactor,t.alarmcode,a.alarmcontext,a.influence,t.createtime,t.cleartime,t.clearstatic "
                + "from DATA_ALARMINFO t,DATA_ALARM a where t.alarmcode = a.alarmcode and ((t.eci in (" + ecis + ")) or (t.sc_enbid in (" + enbids + ") and t.sc_lcrid is null))  and to_char(createtime, 'yyyy-MM-dd')='"+time+"'";
            DataTable Alarmdata = OraConnect.ReadData(sqlstr);
            if (Alarmdata != null)
            {
                if (Alarmdata.Rows.Count > 0)
                {
                    result = DataTableConvertJson.DataTableToJson("data", Alarmdata);
                }
            }
            return result;
        }

        public String GetAlarm(String enbid, String lcrid)
        {
            String result = "";
            String sqlstr = "select t.sc_enbid,t.sc_lcrid,t.eci,t.city,t.manufactor,t.alarmcode,a.alarmcontext,a.influence,t.createtime,t.cleartime,t.clearstatic "
                + "from DATA_ALARMINFO t,DATA_ALARM a where t.alarmcode = a.alarmcode and ((t.sc_enbid='" + enbid + "' and t.sc_lcrid='" + lcrid + "') or (t.sc_enbid='" + enbid + "' and t.sc_lcrid is null))";
            DataTable Alarmdata = OraConnect.ReadData(sqlstr);
            if (Alarmdata != null)
            {
                if (Alarmdata.Rows.Count > 0)
                {
                    result = DataTableConvertJson.DataTableToJson("data", Alarmdata);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取小区类型（密集市区、市区、县城、山区农村、平原农村）
        /// </summary>
        /// <param name="ECI"></param>
        /// <returns></returns>
        public String getCellMRClass(String ECI)
        {
            String result = "";
            DataTable MRClassData = OraConnect.ReadData("select mrclass from V_WORKPARAMETER where eci='" + ECI + "'");
            if (MRClassData != null)
            {
                if (MRClassData.Rows.Count > 0)
                {
                    result = MRClassData.Rows[0]["mrclass"].ToString();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取小区周边分析距离（密集市区、市区、县城、山区农村、平原农村）
        /// </summary>
        /// <param name="eci"></param>
        /// <returns></returns>
        public String getAnalysisdist(String eci)
        {
            String dist = "1000";
            String mrclass = getCellMRClass(eci);
            if (!String.IsNullOrEmpty(mrclass.Trim()))
            {
                if (mrclass == "密集市区")
                    dist = "700";
                else if (mrclass == "市区")
                    dist = "1000";
                else if (mrclass == "县城")
                    dist = "1500";
                else if (mrclass == "山区农村")
                    dist = "4000";
                else if (mrclass == "平原农村")
                    dist = "4000";
                else
                    dist = "1000";
            }
            return dist;
        }
    }
}