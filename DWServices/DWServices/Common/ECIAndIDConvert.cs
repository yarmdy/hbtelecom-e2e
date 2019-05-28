using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWServices.Common
{
    public class ECIAndIDConvert
    {
        public static long ConvertECI(String enbid, String lcrid)
        {
            if (String.IsNullOrEmpty(enbid) || String.IsNullOrEmpty(lcrid))
            {
                return 0;
            }
            else
            {
                long d_enbid = Convert.ToInt64(enbid);
                long d_lcrid = Convert.ToInt64(lcrid);
                return ConvertECI(d_enbid, d_lcrid);
            }
        }

        public static long ConvertECI(long d_enbid, long d_lcrid)
        {
            return d_enbid * 256 + d_lcrid;
        }

        public static List<long> ConvertToEnbid(String eci)
        {
            List<long> list = new List<long>();
            long l_enbid = Convert.ToInt64(eci);
            String eci_x8 = l_enbid.ToString("x8");
            String enbid = eci_x8.Substring(0,eci_x8.Length-2);
            String lcrid = eci_x8.Substring(eci_x8.Length - 2, 2);

            long i_enbid = Convert.ToInt64(enbid, 16);
            long i_lcrid = Convert.ToInt64(lcrid, 16);
            list.Add(i_enbid);
            list.Add(i_lcrid);
            return list;
        }

        /// <summary>
        /// 判断小区是否为800M小区
        /// </summary>
        /// <param name="eci"></param>
        /// <returns>800M返回true</returns>
        public static bool getflowclass(String eci)
        {
            long l_enbid = Convert.ToInt64(eci);
            String eci_x8 = l_enbid.ToString("x8");
            String lcrid = eci_x8.Substring(eci_x8.Length - 2, 1);
            if (lcrid == "1")
                return true;
            else
                return false;
        }

        /// <summary>
        /// 判断小区是否为800M小区
        /// </summary>
        /// <param name="cellid"></param>
        /// <param name="enbid"></param>
        /// <returns>800M返回true</returns>
        public static bool getflowclass(String cellid,String enbid="")
        {
            long l_enbid = Convert.ToInt64(cellid);
            String eci_x8 = l_enbid.ToString("x8");
            String lcrid = eci_x8.Substring(0, 1);
            if (lcrid == "1")
                return true;
            else
                return false;
        }
    }
}