using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CTCCGoods.Controllers
{
    public static class O2
    {
        public static string O2S(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return "";
            }
            if (o.GetType() == typeof(XElement))
            {
                return ((XElement)o).Value;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                return ((XAttribute)o).Value;
            }
            return o.ToString();
        }
        public static bool O2B(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return false;
            }
            if (o.GetType() == typeof(XElement))
            {
                bool res;
                bool.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                bool res;
                bool.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                bool res;
                bool.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static DateTime O2DT(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return new DateTime();
            }
            if (o.GetType() == typeof(XElement))
            {
                DateTime res;
                DateTime.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                DateTime res;
                DateTime.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                DateTime res;
                DateTime.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static int O2I(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                int res;
                int.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                int res;
                int.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                int res;
                int.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static long O2L(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                long res;
                long.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                long res;
                long.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                long res;
                long.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static double O2D(object o)
        {
            if (o == null || DBNull.Value.Equals(o))
            {
                return 0;
            }
            if (o.GetType() == typeof(XElement))
            {
                double res;
                double.TryParse(((XElement)o).Value, out res);
                return res;
            }
            if (o.GetType() == typeof(XAttribute))
            {
                double res;
                double.TryParse(((XAttribute)o).Value, out res);
                return res;
            }
            else
            {
                double res;
                double.TryParse(o.ToString(), out res);
                return res;
            }
        }
        public static string GED(object o) {
            if(o == null)
            {
                return null;
            }
            return ((EnumDescriptionAttribute)o.GetType().GetField(o.ToString()).GetCustomAttributes(false)[0]).Description;
        }
    }
}