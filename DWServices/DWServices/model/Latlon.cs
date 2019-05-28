using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWServices.model
{
    public class Latlon
    {
        public Latlon(){  }
        public Latlon(double lon,double lat)
        {
            this.m_lon = lon;
            this.m_lat = lat;
        }

        double m_lat;

        public double Lat
        {
            get { return m_lat; }
            set { m_lat = value; }
        }
        double m_lon;

        public double Lon
        {
            get { return m_lon; }
            set { m_lon = value; }
        }

        #region 经纬度与距离转换
        private static double EARTH_RADIUS = 6378137;//赤道半径(单位m)  
        private static double minscale = 0.00001;//经纬度拆分粒度

        /** 
         * 转化为弧度(rad) 
         * */
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /** 
         * 基于余弦定理求两经纬度距离 
         * @param lon1 第一点的精度 
         * @param lat1 第一点的纬度 
         * @param lon2 第二点的精度 
         * @param lat3 第二点的纬度 
         * @return 返回的距离，单位km 
         * */
        public static double LantitudeLongitudeDist(Latlon lonlat1, Latlon lonlat2)
        {
            double lon1=lonlat1.Lon;
            double lat1=lonlat1.Lat;
            double lon2=lonlat2.Lon;
            double lat2 = lonlat2.Lat;
            return LantitudeLongitudeDist(lon1, lat1, lon2, lat2);
        }
        public static double LantitudeLongitudeDist(double lon1, double lat1, double lon2, double lat2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);

            double radLon1 = rad(lon1);
            double radLon2 = rad(lon2);

            if (radLat1 < 0)
                radLat1 = Math.PI / 2 + Math.Abs(radLat1);// south  
            if (radLat1 > 0)
                radLat1 = Math.PI / 2 - Math.Abs(radLat1);// north  
            if (radLon1 < 0)
                radLon1 = Math.PI * 2 - Math.Abs(radLon1);// west  
            if (radLat2 < 0)
                radLat2 = Math.PI / 2 + Math.Abs(radLat2);// south  
            if (radLat2 > 0)
                radLat2 = Math.PI / 2 - Math.Abs(radLat2);// north  
            if (radLon2 < 0)
                radLon2 = Math.PI * 2 - Math.Abs(radLon2);// west  
            double x1 = EARTH_RADIUS * Math.Cos(radLon1) * Math.Sin(radLat1);
            double y1 = EARTH_RADIUS * Math.Sin(radLon1) * Math.Sin(radLat1);
            double z1 = EARTH_RADIUS * Math.Cos(radLat1);

            double x2 = EARTH_RADIUS * Math.Cos(radLon2) * Math.Sin(radLat2);
            double y2 = EARTH_RADIUS * Math.Sin(radLon2) * Math.Sin(radLat2);
            double z2 = EARTH_RADIUS * Math.Cos(radLat2);

            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
            //余弦定理求夹角  
            double theta = Math.Acos((EARTH_RADIUS * EARTH_RADIUS + EARTH_RADIUS * EARTH_RADIUS - d * d) / (2 * EARTH_RADIUS * EARTH_RADIUS));
            double dist = theta * EARTH_RADIUS;
            return dist;
        }


        /** 
         * 基于googleMap中的算法得到两经纬度之间的距离,计算精度与谷歌地图的距离精度差不多，相差范围在0.2米以下 
         * @param lon1 第一点的精度 
         * @param lat1 第一点的纬度 
         * @param lon2 第二点的精度 
         * @param lat3 第二点的纬度 
         * @return 返回的距离，单位km 
         * */
        public static double GetDistance(Latlon lonlat1, Latlon lonlat2)
        {
            double lon1 = lonlat1.Lon;
            double lat1 = lonlat1.Lat;
            double lon2 = lonlat2.Lon;
            double lat2 = lonlat2.Lat;
            return GetDistance(lon1, lat1, lon2, lat2);
        }
        public static double GetDistance(double lon1, double lat1, double lon2, double lat2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lon1) - rad(lon2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            //s = Math.round(s * 10000) / 10000;  
            return s;
        }

        /// <summary>
        /// 将距离转成经度维度差
        /// </summary>
        public static double GetMlon(double dis)
        {
            return dis * minscale;
        }
        public static double GetMlat(double dis)
        {
            return (dis / 1.1) * minscale;
        }
        #endregion
    }
}