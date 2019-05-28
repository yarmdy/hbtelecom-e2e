using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DWMapService.Controllers
{
    public class GoodRateController : Controller
    {
        //
        // GET: /VolteMap./

        public ActionResult GoodRate()
        {
            ViewBag.t3hn = "";
            return View();
        }

        public ActionResult G3h() {
            ViewBag.t3hn = "室内";
            return View("GoodRate");
        }

        public ActionResult Get1(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
        {
            try
            {
                if (REQUEST == "GetCapabilities")
                {
                    var resxml = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"GetCapabilities\GetCapabilities.txt"));
                    var bres = System.Text.Encoding.GetEncoding("utf-8").GetBytes(resxml.ToString());
                    Response.AddHeader("Access-Control-Allow-Origin", "*");
                    return File(bres, "text/xml");
                }
                else if (REQUEST == "GetMap")
                {
                    int crs = CRS == "EPSG:900913" ? 1 : 2;
                    string[] bboxs = BBOX.Split(',');
                    var act = 40;
                    var lay = int.Parse(LAYERS);
                    if (lay >= 10) {
                        act += 30;
                    }
                    lay = lay % 10;
                    act = act + lay;
                    byte[] byteArr = GetData.get(act, WIDTH.Value, HEIGHT.Value, double.Parse(bboxs[0]), double.Parse(bboxs[1]), double.Parse(bboxs[2]), double.Parse(bboxs[3]), crs);
                    //System.IO.File.WriteAllBytes(@"C:\Users\guoco\Desktop\1.png", byteArr);
                    return File(byteArr, "application/x-png");
                }
                return null;
            }
            catch (Exception ex)
            {
                LOG.WriteLog(ex.Message + ":" + ex.StackTrace);
                return null;
            }
        }
        public ActionResult Get2(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
        {
            try
            {
                if (REQUEST == "GetCapabilities")
                {
                    var resxml = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"GetCapabilities\GetCapabilities.txt"));
                    var bres = System.Text.Encoding.GetEncoding("utf-8").GetBytes(resxml.ToString());
                    Response.AddHeader("Access-Control-Allow-Origin", "*");
                    return File(bres, "text/xml");
                }
                else if (REQUEST == "GetMap")
                {
                    int crs = CRS == "EPSG:900913" ? 1 : 2;
                    string[] bboxs = BBOX.Split(',');
                    var act = 50;
                    var lay = int.Parse(LAYERS);
                    if (lay >= 10)
                    {
                        act += 30;
                    }
                    lay = lay % 10;
                    act = act + lay;
                    byte[] byteArr = GetData.get(act, WIDTH.Value, HEIGHT.Value, double.Parse(bboxs[0]), double.Parse(bboxs[1]), double.Parse(bboxs[2]), double.Parse(bboxs[3]), crs);
                    //System.IO.File.WriteAllBytes(@"C:\Users\guoco\Desktop\1.png", byteArr);
                    return File(byteArr, "application/x-png");
                }
                return null;
            }
            catch (Exception ex)
            {
                LOG.WriteLog(ex.Message + ":" + ex.StackTrace);
                return null;
            }
        }
        public ActionResult Get3(string REQUEST, int? WIDTH, int? HEIGHT, string BBOX, string FORMAT, string STYLES, bool? TRANSPARENT, string CRS, string LAYERS)
        {
            try
            {
                if (REQUEST == "GetCapabilities")
                {
                    var resxml = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"GetCapabilities\GetCapabilities.txt"));
                    var bres = System.Text.Encoding.GetEncoding("utf-8").GetBytes(resxml.ToString());
                    Response.AddHeader("Access-Control-Allow-Origin", "*");
                    return File(bres, "text/xml");
                }
                else if (REQUEST == "GetMap")
                {
                    int crs = CRS == "EPSG:900913" ? 1 : 2;
                    string[] bboxs = BBOX.Split(',');
                    var act = 60;
                    var lay = int.Parse(LAYERS);
                    if (lay >= 10)
                    {
                        act += 30;
                    }
                    lay = lay % 10;
                    act = act + lay;
                    byte[] byteArr = GetData.get(act, WIDTH.Value, HEIGHT.Value, double.Parse(bboxs[0]), double.Parse(bboxs[1]), double.Parse(bboxs[2]), double.Parse(bboxs[3]), crs);
                    //System.IO.File.WriteAllBytes(@"C:\Users\guoco\Desktop\1.png", byteArr);
                    return File(byteArr, "application/x-png");
                }
                return null;
            }
            catch (Exception ex)
            {
                LOG.WriteLog(ex.Message + ":" + ex.StackTrace);
                return null;
            }
        }



        private Color getARGBBrushC(double bili)
        {
            //if (bili <= 0.1)
            //{
            //    return Color.FromArgb((int)(bili/0.1 * 255), Color.Green);
            //}
            var tmmax = 200;
            var bbb = 66;
            var aaa = tmmax - bbb;
            var tmr = aaa * bili + bbb;

            if (bili <= 0.25)
            {
                return Color.FromArgb((int)(tmr), Color.Green);
            }
            if (bili <= 0.5)
            {
                return Color.FromArgb((int)(tmr), 0, (int)(255 - (bili - 0.25) / 0.25 * 255), (int)((bili - 0.25) / 0.25 * 127 + 128));
            }
            if (bili <= 0.75)
            {
                return Color.FromArgb((int)(tmr), (int)((bili - 0.5) / 0.25 * 255), (int)((bili - 0.5) / 0.25 * 255), (int)(255 - (bili - 0.5) / 0.25 * 255));
            }
            if (bili <= 1)
            {
                return Color.FromArgb((int)(tmr), 255, 255 - (int)((bili - 0.75) / 0.25 * 255), 0);
            }
            return Color.FromArgb((int)(255), Color.Red);
        }


        private ColorBlend getColorBlend(double bili, bool fan = false)
        {
            bili = bili > 1 ? 1 : bili;
            ColorBlend colors = new ColorBlend(3);

            // Set brush stops.


            // The intensity value adjusts alpha of gradient colors.
            if (false)
            {
                colors.Positions = new float[3] { 0, 0.6f, 1 };
                colors.Colors = new Color[3]
                {
                    Color.FromArgb((int)((1-bili)*255), Color.Black),
                    //// The following colors can be any color - Only the alpha  value is used.
                    Color.FromArgb(1, Color.Black),
                    Color.FromArgb(1, Color.Black)
                };
            }
            else
            {
                colors.Positions = new float[3] { 0, 0.6f, 1 };
                colors.Colors = new Color[3]
                {
                    Color.FromArgb(0, Color.Black),
                    //// The following colors can be any color - Only the alpha  value is used.
                    Color.FromArgb((int)(bili*255*0.75), Color.Black),
                    Color.FromArgb((int)(bili*255), Color.Black)
                };
            }
            return colors;
        }



        private Color getNBColor(double v)
        {
            if (v >= -87)
            {
                return Color.FromArgb(96, 0, 255, 0);
            }
            if (v < -120)
            {
                return Color.FromArgb(96, 255, 0, 0);
            }
            if (v >= -97 && v < -87)
            {
                return Color.FromArgb(96, 0, 0, 255);
            }
            if (v >= -110 && v < -97)
            {
                return Color.FromArgb(96, 255, 255, 0);
            }
            if (v >= -120 && v < -110)
            {
                return Color.FromArgb(96, 255, 128, 0);
            }
            return Color.Red;
        }

        private bool qx(int w, int h, out ActionResult ar)
        {
            ar = null;
            //#if CESHI
            //            return true;
            //#endif
            if (Session["per"] != null && Session["per"].ToString() != "3")
            {
                return true;
            }
            var msg = "";
            if (Session["per"] == null)
            {
                msg = "登录超时，请重新登录！";
            }
            Bitmap image = new Bitmap(w, h, PixelFormat.Format32bppArgb);//用于处理由像素数据定义的图像的对象
            Graphics ig = Graphics.FromImage(image);
            ig.DrawString(msg, new Font("", 24), new SolidBrush(Color.Red), 50, 50);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);

            ar = File(stream.ToArray(), "application/x-png");
            return false;
        }


    }
}
