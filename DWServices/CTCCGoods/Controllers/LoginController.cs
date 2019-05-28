using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CTCCGoods.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string name,string password, string vercode)
        {
            string sql = "select * from cuser where code = '" + name + "'";
            var data = DB.QueryAsDics(sql);
            if(data == null || data[0]["status"].ToString() == "0")
            {
                return Json(new { status = false, msg = "用户名或密码不正确" }, JsonRequestBehavior.AllowGet);
            }
            if (data == null || data[0]["pwd"].ToString() != password)
            {
                return Json(new { status = false, msg = "用户名或密码不正确" }, JsonRequestBehavior.AllowGet);
            }
            if (vercode.ToLower() != Session["vercode"].ToString().ToLower())
            {
                return Json(new { status = false, msg = "验证码不正确" }, JsonRequestBehavior.AllowGet);
            }
            cuser user = new cuser();
            user.id = int.Parse(data[0]["id"].ToString());
            user.name = data[0]["name"].ToString();
            user.status = int.Parse(data[0]["status"].ToString());
            user.pwd = password;
            user.utype = int.Parse(data[0]["utype"].ToString());
            user.code = data[0]["code"].ToString();
            Session["loginuser"] = user;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Logout()
        {
            Session["loginuser"] = null;
            return Redirect("/login");
        }

        public ActionResult ModifyPwd(string opwd, string npwd)
        {
            cuser user = (cuser)Session["loginuser"];
            if(user.pwd != opwd)
            {
                return Json(new { status = false, msg="原密码错误" }, JsonRequestBehavior.AllowGet);
            }
            //string pattern = @"^(?![a-zA-Z]+$)(?![A-Z0-9]+$)(?![A-Z\W_]+$)(?![a-z0-9]+$)(?![a-z\W_]+$)(?![0-9\W_]+$)[a-zA-Z0-9\W_]{6,}$";
            string pattern = "^.*(?=.{10,})(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).*$";
            if(!Regex.IsMatch(npwd,pattern))
            {
                return Json(new { status = false, msg = "密码不符合规范" }, JsonRequestBehavior.AllowGet);
            }
            string sql = "update cuser set pwd = '" + npwd + "' where id = " + user.id;
            DB.Exec(sql);
            user.pwd = npwd;
            Session["loginuser"] = user;
            return Json(new { status = true, msg = "修改成功" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SecurityCode()
        {
            string oldcode = TempData["SecurityCode"] as string;
            string code = CreateRandomCode(4); //验证码的字符为4个
            Session["vercode"] = code;
            TempData["SecurityCode"] = code; //验证码存放在TempData中
            return File(CreateValidateGraphic(code), "image/Jpeg");
        }

        /// <summary>
        /// 生成随机的字符串
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public string CreateRandomCode(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,a,b,c,d,e,f,g,h,i,g,k,l,m,n,o,p,q,r,F,G,H,I,G,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(35);
                if (temp == t)
                {
                    return CreateRandomCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 16.0), 27);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, x2, y1, y2);
                }
                Font font = new Font("Arial", 13, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);

                //画图片的前景干扰线
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);

                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
