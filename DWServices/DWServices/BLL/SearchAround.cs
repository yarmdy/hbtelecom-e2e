using DWServices.DAL;
using DWServices.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DWServices.BLL
{
    public class SearchAround
    {
        public static List<String> getAroundSite(Latlon latlon,double dist)
        {
            List<String> result=new List<string>();
            double latdist = Latlon.GetMlat(dist);
            double londist = Latlon.GetMlon(dist);
            double minlat = latlon.Lat - latdist;
            double maxlat = latlon.Lat + latdist;
            double minlon = latlon.Lon - londist;
            double maxlon = latlon.Lon + londist;
            String sqlStr = "select eci,sc_lon,sc_lat from V_WORKPARAMETER t where t.sc_lon>=" + minlon
                + " and t.sc_lon<=" + maxlon + " and t.sc_lat>=" + minlat + " and t.sc_lat<=" + maxlat + "";
            DataTable data = OraConnect.ReadData(sqlStr);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                double lon = Convert.ToDouble(data.Rows[i]["sc_lon"].ToString());
                double lat = Convert.ToDouble(data.Rows[i]["sc_lat"].ToString());
                Latlon latlon2 = new Latlon(lon, lat);
                double dist2 = Latlon.GetDistance(latlon, latlon2);
                if (dist2 <= dist)
                    result.Add(data.Rows[i]["eci"].ToString());
            }
            return result;
        }
    }
}