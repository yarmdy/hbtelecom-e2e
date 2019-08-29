using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using DWServices.Common;

namespace DWServices.services
{
    /// <summary>
    /// Tk 的摘要说明
    /// </summary>
    public class Tk : IHttpHandler, IRequiresSessionState 
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            var act = context.Request["act"];
            if (string.IsNullOrEmpty(act)) {
                return;
            }
            switch (act) { 
                case "t":
                    var now = DateTime.Now;
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(now.Ticks);
                    break;
                case "l":
                    {
                        var tk = context.Request["tk"];
                        if (string.IsNullOrEmpty(tk)) return;
                        var url = context.Request["url"];
                        if (string.IsNullOrEmpty(url)) return;
                        var username = "";
                        var permissions = "";
                        try
                        {
                            var deskey = "xyhsygck";
                            tk = Uri.UnescapeDataString(tk);
                            url = Uri.UnescapeDataString(url);
                            var tokenb = System.Convert.FromBase64String(tk);
                            System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
                            var stream = new System.IO.MemoryStream();
                            des.Mode = System.Security.Cryptography.CipherMode.ECB;
                            des.Key = System.Text.Encoding.GetEncoding("utf-8").GetBytes(deskey.Substring(0, 8));
                            des.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                            stream = new System.IO.MemoryStream();
                            var cstream = new System.Security.Cryptography.CryptoStream(stream, des.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                            cstream.Write(tokenb, 0, tokenb.Length);
                            cstream.FlushFinalBlock();
                            tk = System.Text.Encoding.GetEncoding("utf-8").GetString(stream.ToArray());

                            var tks = tk.Split(new char[] { '-' });
                            if (tks.Length != 3) return;

                            System.DateTime indt = new DateTime(long.Parse(tks[2]));
                            var seconds = (System.DateTime.Now - indt).TotalSeconds;
                            if (seconds > 30) return;

                            username = tks[0];
                            username = tks[1];

                        }
                        catch
                        {
                            return;
                        }

                        User user = new User();
                        user.Id = "1";
                        user.Username = username;
                        user.Password = "";
                        user.Permissions = permissions;
                        user.Orgname = "";
                        user.Usercode = "";
                        context.Session["user"] = user;
                        context.Response.Redirect(url);
                    }
                    break;
                case "r":
                    {
                        //var ticks = DateTime.Now.Ticks;
                        //var tk = "nihao-1-" + ticks;
                        //var deskey = "xyhsygck";
                        //var tokenb = System.Text.Encoding.GetEncoding("utf-8").GetBytes(tk);
                        //System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
                        //var stream = new System.IO.MemoryStream();
                        //des.Mode = System.Security.Cryptography.CipherMode.ECB;
                        //des.Key = System.Text.Encoding.GetEncoding("utf-8").GetBytes(deskey.Substring(0, 8));
                        //des.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                        //stream = new System.IO.MemoryStream();
                        //var cstream = new System.Security.Cryptography.CryptoStream(stream, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                        //cstream.Write(tokenb, 0, tokenb.Length);
                        //cstream.FlushFinalBlock();
                        //tk = Convert.ToBase64String(stream.ToArray());
                        //tk = Uri.EscapeDataString(tk);
                        //var url = "/page/list.html";
                        //url = Uri.EscapeDataString(url);
                        //context.Response.ContentType = "text/plain";
                        //context.Response.Write("/services/tk.ashx?act=l&tk="+tk+"&url="+url);
                    }
                    break;
                default:
                    return;
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