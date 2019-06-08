using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CTCCGoods.Controllers
{
    public class RruController : Controller
    {
        [Breadcrumb(Auth = "1")]
        public ActionResult Index()
        {
            return View();
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Report()
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rrufiles/report"));
            cuser user = (cuser)Session["loginuser"];
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                //Array.Sort(files, (f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime));
                Array.Sort(files, (f1, f2) => f1.Name.CompareTo(f2.Name));
                Array.Reverse(files);
                foreach (FileInfo fi in files)
                {
                    var a = new Dictionary<string, string>();
                    if (user.utype != 0 && fi.Name.Contains("[hidden]")) continue;
                    a.Add("code", fi.FullName);
                    a.Add("text", fi.Name);
                    list.Add(a);
                }
            }
            ViewBag.history = list;
            ViewBag.user = user;
            return View();
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Lterru()
        {
            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Jsuan(string val,string rg) {
            Regex r = new Regex(rg);
            var res=r.Split(val);
            return Json(new {ok=true,msg=string.Join("<br />",res) });
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Wang()
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rrufiles/wang"));
            cuser user = (cuser)Session["loginuser"];
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                Array.Sort(files, (f1, f2) => f1.Name.CompareTo(f2.Name));
                Array.Reverse(files);
                foreach (FileInfo fi in files)
                {
                    var a = new Dictionary<string, string>();
                    if (user.utype != 0 && fi.Name.Contains("[hidden]")) continue;
                    a.Add("code", fi.FullName);
                    a.Add("text", fi.Name);
                    list.Add(a);
                }
            }
            ViewBag.history = list;
            ViewBag.user = user;
            return View();
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Wangtj() {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rrufiles/wangtj"));
            cuser user = (cuser)Session["loginuser"];
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                Array.Sort(files, (f1, f2) => f1.Name.CompareTo(f2.Name));
                Array.Reverse(files);
                foreach (FileInfo fi in files)
                {
                    var a = new Dictionary<string, string>();
                    if (user.utype != 0 && fi.Name.Contains("[hidden]")) continue;
                    a.Add("code", fi.FullName);
                    a.Add("text", fi.Name);
                    list.Add(a);
                }
            }
            ViewBag.history = list;
            ViewBag.user = user;
            return View();
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult File(string b, string r, string n, string city, string chang,string pinduan,string leixing,string xz,string fh)
        {
            
            string type = Request["type"];
            string d = Request["d"];
            cuser user = (cuser)Session["loginuser"];
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles/" + d));
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                FileInfo file = null;
                int offset = Request["offset"] == null ? 0 : int.Parse(Request["offset"]);
                int limit = Request["limit"] == null ? 10 : int.Parse(Request["limit"]);
                foreach (var fi in files)
                {
                    if (fi.FullName == type)
                    {
                        file = fi;
                    }
                }
                string content = System.IO.File.ReadAllText(file.FullName, Encoding.Default);
                var result = CsvToJson(content, user, limit, offset, 0, new Dictionary<int, string>() {{0,city},{1,chang}, { 2, b }, {5,pinduan},{7,leixing},{ 8, r }, { 9, n },{11,xz},{13,fh} });
                int total = (int)result["total"];
                var rows = result["data"];

                

                
                return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0 }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Filetongji(string type, string d, int offset, int limit, string b, string r, string n, string c, string s, int g)
        {
            object rows;
            var total = Filetongji(type,d,offset,limit,b, r, n, c, s, g, out rows);
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        [Breadcrumb(Auth = "1")]
        public ActionResult Exporttongji(string type, string d, int offset, int limit, string b, string r, string n, string c, string s, int g)
        {
            object rows;
            var total = Filetongji(type, d, offset, limit, b, r, n, c, s, g, out rows);
            if (rows == null) {
                return Content("导出失败");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(",,历史到货数,,,网管有序列号数(最新),,,网管有序列号（最新）-历史到货数,,,对比20190501_124022日RRU序列号新增数,,,其中：序列号新增新发货来数,,,对比20190501_124022日RRU序列号丢失数,,,累计RRU序列号出现数,,,累计RRU序列号出现数-历史到货,,\r\n");
            sb.Append("地市,厂家,RRU-800M,RRU-1.8G&2.1G,PRRU,RRU-800M,RRU-1.8G&2.1G,PRRU,RRU-800M,RRU-1.8G&2.1G,PRRU,RRU-800M,RRU-1.8G&2.1G,PRRU,RRU-800M,RRU-1.8G&2.1G,PRRU,RRU-800M,RRU-1.8G&2.1G,PRRU,RRU-800M,RRU-1.8G&2.1G,PRRU,RRU-800M,RRU-1.8G&2.1G,PRRU\r\n");
            foreach (var i in (List<Dictionary<string,object>>)rows) {
                var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25}\r\n",
                    i["city"],i["chang"],
                    i["rrumd"],i["rrugd"],i["prrud"],
                    i["rrum"],i["rrug"],i["prru"],
                    i["rrum5"],i["rrug5"],i["prru5"],
                    i["rrum2"],i["rrug2"],i["prru2"],
                    i["rrum3"],i["rrug3"],i["prru3"],
                    i["rrumu"],i["rrugu"],i["prruu"],
                    i["rrum4"],i["rrug4"],i["prru4"],
                    i["rrumj"],i["rrugj"],i["prruj"]
                    );
                sb.Append(line);
            }
            return File(Encoding.GetEncoding("gbk").GetBytes(sb.ToString()), "application/octet-stream", "对比分析.csv");
        }
        
        private int Filetongji(string type,string d,int offset,int limit,string b, string r, string n, string c,string s,int g,out object rows)
        {

            
            cuser user = (cuser)Session["loginuser"];
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles/" + d));
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                FileInfo file = null;
                foreach (var fi in files)
                {
                    if (fi.FullName == type)
                    {
                        file = fi;
                    }
                }
                string content = System.IO.File.ReadAllText(file.FullName, Encoding.Default);
                var result = CsvToJson(content, user, limit, offset, 0, new Dictionary<int, string>() { { 2, b }, { 8, r }, { 9, n } });
                int total = (int)result["total"];
                rows = result["data"];
                bool hass = false;
                var data3=new Dictionary<string,Dictionary<string,object>>();
                if (!string.IsNullOrEmpty(s)) {
                    var spath=file.FullName.Substring(0,file.FullName.LastIndexOf("."));
                    var sfilename = Path.Combine(spath,s);

                    if (System.IO.File.Exists(sfilename)) {
                        hass = true;
                        var sres = CsvToJson(System.IO.File.ReadAllText(sfilename,Encoding.GetEncoding("gbk")),user,limit,offset,0,null);

                        var srow = (List<Dictionary<string, string>>)sres["data"];
                        data3=srow.ToDictionary(a => a["city"] + "_" + a["chang"], a => new Dictionary<string, object> { 
                            {"city",a["city"]},
                            {"chang",a["chang"]},

                            {"rrum2",O2.O2I(a["rrum2"])},
                            {"rrug2",O2.O2I(a["rrug2"])},
                            {"prru2",O2.O2I(a["prru2"])},

                            {"rrum3",O2.O2I(a["rrum3"])},
                            {"rrug3",O2.O2I(a["rrug3"])},
                            {"prru3",O2.O2I(a["prru3"])},

                            {"rrumu",O2.O2I(a["rrumu"])},
                            {"rrugu",O2.O2I(a["rrugu"])},
                            {"prruu",O2.O2I(a["prruu"])},
                        });
                    }
                }

                #region 统计csv特殊处理
                if (d == "wangtongji")
                {
                    var sql = "select city,chang,sum(iif(rru_type<>'PRRU' and pinduan='800M',isnull(arrive,0)-isnull(free,0),0)) rrumd,sum(iif(rru_type<>'PRRU' and (pinduan='1.8G' or pinduan='2.1G' or pinduan='1.8G+2.1G' or pinduan='2.6G'),isnull(arrive,0)-isnull(free,0),0)) rrugd,sum(iif(rru_type='PRRU',isnull(arrive,0)-isnull(free,0),0)) prrud from r_lterru {0} group by city,chang";
                    if (user.utype != 0)
                    {
                        sql = string.Format(sql, "where city='" + user.wname + "'");
                    }
                    else
                    {
                        sql = string.Format(sql, "");
                    }
                    var data2t = DB.QueryAsDics(sql);
                    if (data2t != null)
                    {
                        var data1 = ((List<Dictionary<string, string>>)rows).ToDictionary(a => a["city"] + "_" + a["chang"], a => new Dictionary<string, object> { 
                        {"city",a["city"]},
                        {"chang",a["chang"]},

                        {"rrumd",0},
                        {"rrugd",0},
                        {"prrud",0},

                        {"rrumj",0},
                        {"rrugj",0},
                        {"prruj",0},

                        {"rrum",O2.O2I(a["rrum"])},
                        {"rrug",O2.O2I(a["rrug"])},
                        {"prru",O2.O2I(a["prru"])},

                        {"rrum2",O2.O2I(hass?"0":a["rrum2"])},
                        {"rrug2",O2.O2I(hass?"0":a["rrug2"])},
                        {"prru2",O2.O2I(hass?"0":a["prru2"])},

                        {"rrum3",O2.O2I(hass?"0":a["rrum3"])},
                        {"rrug3",O2.O2I(hass?"0":a["rrug3"])},
                        {"prru3",O2.O2I(hass?"0":a["prru3"])},

                        {"rrum4",O2.O2I(a["rrum4"])},
                        {"rrug4",O2.O2I(a["rrug4"])},
                        {"prru4",O2.O2I(a["prru4"])},

                        {"rrum5",O2.O2I(a["rrum5"])},
                        {"rrug5",O2.O2I(a["rrug5"])},
                        {"prru5",O2.O2I(a["prru5"])},

                        {"rrumu",O2.O2I(a.ContainsKey("rrumu")&&!hass?a["rrumu"]:"0")},
                        {"rrugu",O2.O2I(a.ContainsKey("rrugu")&&!hass?a["rrugu"]:"0")},
                        {"prruu",O2.O2I(a.ContainsKey("prruu")&&!hass?a["prruu"]:"0")},
                        
                        });
                        var data2 = data2t.ToDictionary(a => a["city"] + "_" + a["chang"], a => new Dictionary<string, object> { 
                        {"city",a["city"]},
                        {"chang",a["chang"]},

                        {"rrumd",a["rrumd"]},
                        {"rrugd",a["rrugd"]},
                        {"prrud",a["prrud"]}
                        });

                        if (hass) {
                            foreach (var dd in data3)
                            {
                                if (!data1.ContainsKey(dd.Key))
                                {
                                    data1[dd.Key] = new Dictionary<string, object>();
                                    data1[dd.Key]["city"] = dd.Value["city"];
                                    data1[dd.Key]["chang"] = dd.Value["chang"];
                                    data1[dd.Key]["rrum"] = 0;
                                    data1[dd.Key]["rrug"] = 0;
                                    data1[dd.Key]["prru"] = 0;

                                    data1[dd.Key]["rrum4"] = 0;
                                    data1[dd.Key]["rrug4"] = 0;
                                    data1[dd.Key]["prru4"] = 0;

                                    data1[dd.Key]["rrum5"] = 0;
                                    data1[dd.Key]["rrug5"] = 0;
                                    data1[dd.Key]["prru5"] = 0;

                                    data1[dd.Key]["rrumd"] = 0;
                                    data1[dd.Key]["rrugd"] = 0;
                                    data1[dd.Key]["prrud"] = 0;


                                    data1[dd.Key]["rrumj"] = 0;
                                    data1[dd.Key]["rrugj"] = 0;
                                    data1[dd.Key]["prruj"] = 0;
                                }
                                data1[dd.Key]["rrum2"] = dd.Value["rrum2"];
                                data1[dd.Key]["rrug2"] = dd.Value["rrug2"];
                                data1[dd.Key]["prru2"] = dd.Value["prru2"];
                                data1[dd.Key]["rrum3"] = dd.Value["rrum3"];
                                data1[dd.Key]["rrug3"] = dd.Value["rrug3"];
                                data1[dd.Key]["prru3"] = dd.Value["prru3"];

                                data1[dd.Key]["rrumu"] = dd.Value["rrumu"];
                                data1[dd.Key]["rrugu"] = dd.Value["rrugu"];
                                data1[dd.Key]["prruu"] = dd.Value["prruu"];
                            }
                        }

                        foreach (var dd in data2)
                        {
                            if (!data1.ContainsKey(dd.Key))
                            {
                                data1[dd.Key] = new Dictionary<string, object>();
                                data1[dd.Key]["city"] = dd.Value["city"];
                                data1[dd.Key]["chang"] = dd.Value["chang"];
                                data1[dd.Key]["rrum"] = 0;
                                data1[dd.Key]["rrug"] = 0;
                                data1[dd.Key]["prru"] = 0;

                                data1[dd.Key]["rrum2"] = 0;
                                data1[dd.Key]["rrug2"] = 0;
                                data1[dd.Key]["prru2"] = 0;

                                data1[dd.Key]["rrum3"] = 0;
                                data1[dd.Key]["rrug3"] = 0;
                                data1[dd.Key]["prru3"] = 0;

                                data1[dd.Key]["rrum4"] = 0;
                                data1[dd.Key]["rrug4"] = 0;
                                data1[dd.Key]["prru4"] = 0;

                                data1[dd.Key]["rrum5"] = 0;
                                data1[dd.Key]["rrug5"] = 0;
                                data1[dd.Key]["prru5"] = 0;

                                data1[dd.Key]["rrumu"] = 0;
                                data1[dd.Key]["rrugu"] = 0;
                                data1[dd.Key]["prruu"] = 0;
                            }
                            data1[dd.Key]["rrumd"] = dd.Value["rrumd"];
                            data1[dd.Key]["rrugd"] = dd.Value["rrugd"];
                            data1[dd.Key]["prrud"] = dd.Value["prrud"];

                            data1[dd.Key]["rrum5"] = (int)data1[dd.Key]["rrum"] - (int)dd.Value["rrumd"];
                            data1[dd.Key]["rrug5"] = (int)data1[dd.Key]["rrug"] - (int)dd.Value["rrugd"];
                            data1[dd.Key]["prru5"] = (int)data1[dd.Key]["prru"] - (int)dd.Value["prrud"];

                            data1[dd.Key]["rrumj"] = (int)data1[dd.Key]["rrum4"] - (int)dd.Value["rrumd"];
                            data1[dd.Key]["rrugj"] = (int)data1[dd.Key]["rrug4"] - (int)dd.Value["rrugd"];
                            data1[dd.Key]["prruj"] = (int)data1[dd.Key]["prru4"] - (int)dd.Value["prrud"];
                        }


                        if (g == 1) {
                            rows=data1.GroupBy(a => a.Value["city"]).Select(a => {
                                var res = new Dictionary<string, object>();
                                foreach (var i in a.First().Value.Keys) {
                                    if (i == "chang") {
                                        res[i] = "全部";
                                        
                                    }
                                    else if (i == "city")
                                    {
                                        res[i] = a.Key;
                                    }
                                    else {
                                        res[i] = a.Sum(j=>(int)j.Value[i]);
                                    }
                                }
                                return res;
                            }).OrderBy(a=>a["city"]).ToList();
                        } else if (g == 2) {
                            rows = data1.GroupBy(a => a.Value["chang"]).Select(a =>
                            {
                                var res = new Dictionary<string, object>();
                                foreach (var i in a.First().Value.Keys)
                                {
                                    if (i == "chang")
                                    {
                                        res[i] = a.Key;

                                    }
                                    else if (i == "city")
                                    {
                                        res[i] = "全部";
                                    }
                                    else
                                    {
                                        res[i] = a.Sum(j => (int)j.Value[i]);
                                    }
                                }
                                return res;
                            }).OrderBy(a => a["city"]).ToList();
                        } else {
                            rows = data1.OrderBy(a => a.Key).Select(a => a.Value).ToList();
                        }
                        
                        total = ((List<Dictionary<string, object>>)rows).Count;
                    }
                }
                #endregion


                return total;
            }
            rows = null;
            return 0;
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult ImportWang(string txttime)
        {
            if (Request.Files.Count <= 0)
            {
                return Content("<script>alert(\"没有检测到上传文件，上传失败\");location=\"/rru/wang\"</script>");
            }
            var file = Request.Files[0];
            if(file.FileName.IndexOf(".") < 0 || !new String[] {"xlsx", "csv" }.Contains(file.FileName.Substring(file.FileName.LastIndexOf(".")+1).ToLower()))
            {
                return Content("<script>alert(\"上传失败，请上传xlsx、csv格式文件\");location=\"/rru/wang\"</script>");
            }
            var uppath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wangtmp");
            if(!Directory.Exists(uppath))
            {
                Directory.CreateDirectory(uppath);
            }
            var now = DateTime.Now;
            var filename = now.ToString("yyyyMMdd_HHmmss_") + file.FileName;
            file.SaveAs(Path.Combine(uppath, filename));

            var targetname = now.ToString("yyyyMMdd_HHmmss") + ".csv";
            DateTime tartime;
            if (!string.IsNullOrEmpty(txttime) && DateTime.TryParse(txttime, out tartime))
            {
                targetname = tartime.ToString("yyyyMMdd") + now.ToString("_HHmmss") + ".csv";
            }
            ctasksHandle.addtask(2, filename, targetname, "");
            return Content("<script>alert(\"已上传到服务器，进入后台任务\");location=\"/task\"</script>");
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Xinzeng(string file1, string file2) {
            ctasksHandle.addtask(3, file1, file2, "");
            return Json(new { ok=true,msg="新增计算已加入后台任务，请在后台任务中查看运行，成功后在本页面查看新增计算结果"}, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult ImportLteRru()
        {
            if (Request.Files.Count == 0)
            {
                return Content("<script>alert(\"没有检测到上传文件，上传失败\");location=\"/rru/wang\"</script>");
            }
            var file = Request.Files[0];
            if (file.FileName.IndexOf(".") < 0 || !new String[] { "xlsx", "csv" }.Contains(file.FileName.Substring(file.FileName.LastIndexOf(".") + 1).ToLower()))
            {
                return Content("<script>alert(\"上传失败，请上传xlsx、csv格式文件\");location=\"/rru/lterru\"</script>");
            }
            var uppath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\lterrutmp");
            if (!Directory.Exists(uppath))
            {
                Directory.CreateDirectory(uppath);
            }
            var now = DateTime.Now;
            var filename = now.ToString("yyyyMMddHHmmss") + file.FileName;
            file.SaveAs(Path.Combine(uppath, filename));
            ctasksHandle.addtask(4, filename, now.ToString("yyyyMMddHHmmss") + ".csv", "");
            return Content("<script>alert(\"已上传到服务器，进入后台任务\");location=\"/task\"</script>");
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult LteRruFile(string city,string chang,string outin,string leixing,string pinduan)
        {
            cuser user = (cuser)Session["loginuser"];
            int offset = Request["offset"] == null ? 0 : int.Parse(Request["offset"]);
            int limit = Request["limit"] == null ? 10 : int.Parse(Request["limit"]);
            string sql = "select top " + limit + " * from(select count(0)over()c,row_number()over(order by id)no,id,format(borrowtime, 'yyyy-MM-dd') borrowtime,city,stage,outin,rru_type,pinduan,chang,borrow,arrive,remark,need,free,order_num from r_lterru where 1=1 {0})a where no>=" + offset;
            var tiaojian = "";
            if (user.utype != 0)
            {
                tiaojian += " and city = '" + user.wname+"'";
            }
            if (!string.IsNullOrEmpty(city)) {
                tiaojian += " and city= '" + city + "'";
            }
            if (!string.IsNullOrEmpty(chang))
            {
                tiaojian += " and chang= '" + chang + "'";
            }
            if (!string.IsNullOrEmpty(outin))
            {
                tiaojian += " and outin= '" + outin + "'";
            }
            if (!string.IsNullOrEmpty(leixing))
            {
                tiaojian += " and rru_type= '" + leixing + "'";
            }
            if (!string.IsNullOrEmpty(pinduan))
            {
                tiaojian += " and pinduan= '" + pinduan + "'";
            }
            sql = string.Format(sql, tiaojian);
            var data = DB.QueryAsDics(sql);
            var total = data != null ? O2.O2I(data[0]["c"]) : 0;
            var rows = data != null ?data.ToList() : null;
            //rows = rows != null ? rows.Skip(offset).Take(limit).ToList() : null;
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        [Breadcrumb(Auth = "1")]
        public ActionResult Export()
        {
            string file = Request["file"];
            int index = Request["index"] != null ? int.Parse(Request["index"]) : -1;
            cuser user = (cuser)Session["loginuser"];
            if (user.utype == 0)
            {
                return File(file, "application/octet-stream", file.Split('\\')[file.Split('\\').Length-1]);
            }
            string content = System.IO.File.ReadAllText(file, Encoding.Default);
            string r = ExportConvert(content, user, index);
            return File(Encoding.Default.GetBytes(r), "application/octet-stream", file.Split('\\')[file.Split('\\').Length - 1]);
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult ExportLteRru()
        {
            cuser user = (cuser)Session["loginuser"];
            int offset = Request["offset"] == null ? 0 : int.Parse(Request["offset"]);
            int limit = Request["limit"] == null ? 10 : int.Parse(Request["limit"]);
            string sql = "select * from r_lterru";
            if (user.utype != 0)
            {
                sql = sql + " where city = '" + user.wname+"'";
            }
            var data = DB.QueryAsDics(sql);
            StringBuilder sb = new StringBuilder();
            sb.Append("借货时间,地市,期次,室内/外,类型,频段,厂家,借货,到货,仍需到货,备注,减免数,排序用\r\n");

            if (data != null)
            {
                foreach (var d in data)
                {
                    sb.Append(((DateTime)d["borrowtime"]).ToString("yyyy/M/d") + "," + d["city"].ToString() + "," + d["stage"].ToString() + "," + d["outin"].ToString() + "," + d["rru_type"].ToString() + "," + d["pinduan"].ToString() + "," + d["chang"].ToString() + "," + d["borrow"].ToString() + "," + d["arrive"].ToString() + "," + d["need"].ToString() + "," + d["remark"].ToString() + "," + d["free"].ToString() + "," + d["order_num"].ToString() + "\r\n");
                }
            }
            return File(Encoding.Default.GetBytes(sb.ToString()), "application/octet-stream", "letrru.csv");
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult LteRruUpdate()
        {
            string sql = "update r_lterru set " + Request["field"] + "=" + Request["val"] + " where id = " + Request["id"];
            DB.Exec(sql);
            return Json(new {ok=true });
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Lterruinsert(string field,string val)
        {
            string sql = "insert into r_lterru ("+field+") values ("+val+")";
            var id=DB.Insert(sql);
            return Json(new { ok = true,id=id });
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult HiddenOrOpenFile()
        {
            string files = Request["files"];
            string type = Request["type"];
            string[] fs = files.Split(',');
            foreach(var f in fs)
            {
                FileInfo fi = new FileInfo(f);
                if(type == "open")
                {
                    if (f.Contains("[hidden]"))
                    {
                        var name = f.Replace("[hidden]", "");
                        fi.MoveTo(name);
                    }
                }
                else
                {
                    if(!f.Contains("[hidden]"))
                    {
                        fi.MoveTo(Path.Combine(fi.DirectoryName, fi.Name.Split(new String[] { fi.Extension }, StringSplitOptions.None)[0] + "[hidden]" + fi.Extension));
                    }
                }
            }
            return Json(new { result = true });
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Trunclterru() {
            DB.Exec("truncate table r_lterru");
            return Json(new { ok=true,msg="已清空"},JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Dlterru(int id)
        {
            int res=DB.Exec("delete r_lterru where id="+id);
            if (res <= 0) {
                return Json(new { ok = false, msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ok = true, msg = "已删除" }, JsonRequestBehavior.AllowGet);
        }

        public static string ExportConvert(string content,  cuser user, int index,int usercolno=0)
        {
            var list = SelectUserData(content, index, user,null,usercolno);
            StringBuilder sb = new StringBuilder();
            sb.Append(list[0]+"\r\n");
            for (int i = 1; i < list.Count; i++)
            {
                sb.Append(list[i]+"\r\n");
            }
            return sb.ToString();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Dreport(string name)
        {
            var nspath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,@"rrufiles\report");
            var wpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"rrufiles\wang");
            var newns = ""; var neww = "";
            if (Directory.Exists(nspath))
            {
                newns = Directory.GetFiles(nspath, "*.csv").OrderByDescending(a => a).Select(a => a.Substring(a.LastIndexOf("\\") + 1)).FirstOrDefault() + "";
            }
            if (Directory.Exists(wpath))
            {
                neww = Directory.GetFiles(wpath, "*.csv").OrderByDescending(a => a).Select(a => a.Substring(a.LastIndexOf("\\") + 1)).FirstOrDefault() + "";
            }

            var dbns = name.CompareTo(newns);
            var dbw = name.CompareTo(neww);

            var errs=new List<string>();
            if (dbns != 0) {
                errs.Add("不是最新文件");
            }
            if (dbw < 0) {
                errs.Add("被网管详表依赖");
            }
            if (dbw == 0) {
                errs.Add("是网管详表实时更新数据，请先删除网管详表相应数据，本条数据会自动删除");
            }
            if (errs.Count > 0) {
                return Json(new { ok = false, msg = name+"\r\n"+string.Join("\r\n",errs)+"\r\n"+"删除失败" }, JsonRequestBehavior.AllowGet);
            }
            System.IO.File.Delete(Path.Combine(nspath, name));
            return Json(new { ok = true, msg = "已删除" }, JsonRequestBehavior.AllowGet);
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Dwang(string name)
        {

            var nspath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"rrufiles\report");
            var wpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"rrufiles\wang");
            var newns = ""; var neww = "";
            if (Directory.Exists(nspath))
            {
                newns = Directory.GetFiles(nspath, "*.csv").OrderByDescending(a => a).Select(a => a.Substring(a.LastIndexOf("\\") + 1)).FirstOrDefault() + "";
            }
            if (Directory.Exists(wpath))
            {
                neww = Directory.GetFiles(wpath, "*.csv").OrderByDescending(a => a).Select(a => a.Substring(a.LastIndexOf("\\") + 1)).FirstOrDefault() + "";
            }

            var dbns = name.CompareTo(newns);
            var dbw = name.CompareTo(neww);

            var errs = new List<string>();
            if (dbw != 0)
            {
                errs.Add("不是最新文件");
            }
            if (dbns < 0)
            {
                errs.Add("被新发货表依赖");
            }
            if (errs.Count > 0)
            {
                return Json(new { ok = false, msg = name + "\r\n" + string.Join("\r\n", errs) + "\r\n" + "删除失败" }, JsonRequestBehavior.AllowGet);
            }
            var msg = "已删除";
            System.IO.File.Delete(Path.Combine(wpath, name));
            if (dbw == 0) {
                System.IO.File.Delete(Path.Combine(nspath, newns));
                msg += ",同时还原新发货表实时更新数据";
            }
            var tjpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"rrufiles\wangtj");
            var tjname = Path.Combine(tjpath,name);
            if (System.IO.File.Exists(tjname)) {
                System.IO.File.Delete(tjname);
                msg += ",同时还原网管详表(累计)数据";
            }
            var fxpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"rrufiles\wangtongji");
            var fxname = Path.Combine(fxpath, name);
            if (System.IO.File.Exists(fxname)) {
                System.IO.File.Delete(fxname);
                msg += ",同时还原对比分析数据";

                var dir = fxname.Substring(0, fxname.LastIndexOf("."));
                if (Directory.Exists(dir)) {
                    Directory.Delete(dir,true);
                }
            }

            return Json(new { ok = true, msg = msg }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Truncrru(string pwd) {
            var user=(cuser)Session["loginuser"];
            if (user.pwd != pwd) {
                return Json(new { ok = false, msg = "管理员密码错误，已拒绝重置" }, JsonRequestBehavior.AllowGet);
            }
            var err = "";
            //清空数据库
            DB db = new DB();
            return Json(new { ok = false, msg = "功能已停用" }, JsonRequestBehavior.AllowGet);
            try
            {
                db.Execobj("truncate table ctasks");
                db.Execobj("truncate table r_lterru");
                db.End(true);
            }
            catch (Exception ex) {
                err = ex.Message;
                db.End(false);
            }
            if (!string.IsNullOrEmpty(err)) {
                return Json(new { ok = false, msg = "重置失败，"+err }, JsonRequestBehavior.AllowGet);
            }
            //删除csv
            var rrupath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"rrufiles");
            if (Directory.Exists(rrupath)) {
                Directory.Delete(rrupath,true);
            }
            //删除err
            var errpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "taskerr");
            if (Directory.Exists(errpath))
            {
                Directory.Delete(errpath, true);
            }
            //创建目录
            Directory.CreateDirectory(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"rrufiles/report"));
            Directory.CreateDirectory(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles/wang"));
            Directory.CreateDirectory(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles/wangtj"));
            Directory.CreateDirectory(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles/wangtongji"));
            Directory.CreateDirectory(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles/lterru"));


            return Json(new { ok = true, msg = "已重置" }, JsonRequestBehavior.AllowGet);
        }
        //private string[] SpliteLine(string s)
        //{
        //    Regex regex = new Regex("\".*?\"");
        //    var a = regex.Matches(s).Cast<Match>().Select(m => m.Value).ToList();
        //    var b = regex.Replace(s, "%_%");
        //    var c = b.Split(',');
        //    for (int i = 0, j = 0; i < c.Length && j < a.Count; i++)
        //    {
        //        if (c[i] == "%_%")
        //        {
        //            c[i] = a[j++];
        //        }
        //    }
        //    return c;
        //}
        public static string[] SpliteLine(string s)
        {
            if (!s.Contains("\"")) {
                return s.Split(new char[]{','});
            }
            List<string> e = new List<string>();
            StringBuilder local = new StringBuilder();
            bool d = false;
            foreach (var c in s) {
                if (c == ',') {
                    d = true;

                    var locals = local.ToString().Replace("\"\"", "");
                    if (locals.StartsWith("\"") && !locals.EndsWith("\"")) {
                        d = false;
                        local.Append(c);
                        continue;
                    }

                    e.Add(local.ToString());
                    local.Clear();
                } else {
                    d = false;
                    local.Append(c);
                }
            }
            if (!d)
            {
                e.Add(local.ToString());
            }
            return e.ToArray(); ;
        }

        public static List<string> SelectUserData(string content, int index, cuser user, Dictionary<int, string> brn = null,int usercolno=0)
        {
            if (brn == null) {
                brn = new Dictionary<int, string>();
            }
            string[] lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>();
            list.Add(lines[0]);
            for(int i = 1; i < lines.Length; i++)
            {
                //if(user.utype != 0 && index != -1)
                //{
                //    var vals = SpliteLine(lines[i]);
                //    if (vals[index] == user.wname)
                //    {
                //        list.Add(lines[i]);
                //    }
                //}
                //else
                //{
                //    list.Add(lines[i]);
                //}
                var vals = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                if (user.utype != 0 && index != -1 && vals[usercolno] != user.wname)
                {
                    continue;
                }
                var tjb = true;
                foreach (var tj in brn) {
                    if (string.IsNullOrEmpty(tj.Value)) {
                        continue;
                    }
                    if (!vals[tj.Key].ToLower().Contains(tj.Value.ToLower())) {
                        tjb = false;
                        break;
                    }
                }
                if (!tjb) {
                    continue;
                }
                list.Add(lines[i]);
            }
            return list;
        }

        //private Dictionary<string, object> CsvToJson(string content, cuser user, int limit, int offset, int index,Dictionary<int, string> brn=null)
        //{
        //    var lines = SelectUserData(content, index, user,brn);
        //    var keys = Regex.Split(lines[0], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        //    //var keys = SpliteLine(lines[0]);
        //    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
        //    int start = offset+1;
        //    int end = start + limit;
        //    end = end > lines.Count ? lines.Count : end;
        //    int length = lines.Count - 1;
        //    Dictionary<string, object> result = new Dictionary<string, object>();
        //    for(int i= start; i<end;i++)
        //    {
        //        //string[] vals = SpliteLine(lines[i]); ;
        //        string[] vals = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        //        Dictionary<string, string> a = new Dictionary<string, string>();
        //        a.Add("no", i.ToString());
        //        for(int j=0;j<vals.Length;j++)
        //        {
        //            a.Add(keys[j], vals[j]);
        //        }
        //        list.Add(a);
        //    }
        //    result.Add("total", length);
        //    result.Add("data", list);
        //    return result;
        //}

        public static Dictionary<string, object> CsvToJson(string content, cuser user, int limit, int offset, int index, Dictionary<int, string> brn = null,int citycolno=0)
        {
            if (brn != null) {
                brn = brn.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, a => a.Value);
                if (brn.Count == 0) {
                    brn = null;
                }
            }
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, object> result = new Dictionary<string, object>();
            var length = 100;

            var lines = content.Split(new string[]{"\r\n"},StringSplitOptions.RemoveEmptyEntries);
            var keys = Regex.Split(lines[0], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            
            var liste = lines.Skip(1).Select(a =>
            {
                //var cols = Regex.Split(a, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var cols = SpliteLine(a);
                return cols;
            }).Where(a =>
            {
                var res = true;

                if (user.utype != 0 && user.wname != a[citycolno])
                {
                    res = false;
                }
                else if (brn != null)
                {
                    foreach (var b in brn)
                    {
                        //if (b.Value == "") continue;
                        if (b.Value.IndexOf("{==}") == 0) {
                            if (a[b.Key].ToLower()!=b.Value.ToLower().Replace("{==}",""))
                            {
                                res = false;
                                break;
                            }
                        }
                        else
                        {
                            if (a[b.Key].ToLower().IndexOf(b.Value.ToLower()) < 0)
                            {
                                res = false;
                                break;
                            }
                        }
                    }
                }

                return res;
            });

            length = liste.Count();
            list = liste.Select(a =>
            {
                Dictionary<string, string> dici = new Dictionary<string, string>();
                for (int i = 0; i < keys.Length; i++) { 
                    dici[keys[i]]=a[i];
                }
                return dici;
            }).Skip(offset).Take(limit).ToList(); ;

            result.Add("total", length);
            result.Add("data", list);
            return result;
        }

        [Breadcrumb(Auth = "0")]
        public ActionResult Nsendimport(string[] flimport,string txttime) {
            if (Request.Files.Count <= 0) {
                return Content("<script>alert(\"没有检测到上传文件，上传失败\");location=\"/rru\"</script>");
            }
            var nsendfile=Request.Files[0];
            if (nsendfile.FileName.IndexOf(".") < 0 || !new string[] { "xlsx", "csv" }.Contains(nsendfile.FileName.Substring(nsendfile.FileName.LastIndexOf(".")+1).ToLower())) {
                return Content("<script>alert(\"上传失败，请上传xlsx、csv格式文件\");location=\"/rru\"</script>");
            }
            var uppath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\nsendtmp");
            if (!Directory.Exists(uppath)) {
                Directory.CreateDirectory(uppath);
            }
            var now=DateTime.Now;
            var filename = now.ToString("yyyyMMdd_HHmmss_")+nsendfile.FileName;
            nsendfile.SaveAs(Path.Combine(uppath,filename));
            var targetname = now.ToString("yyyyMMdd_HHmmss") + ".csv";
            DateTime tartime;
            if (!string.IsNullOrEmpty(txttime) && DateTime.TryParse(txttime, out tartime)) {
                targetname = tartime.ToString("yyyyMMdd") +now.ToString("_HHmmss") + ".csv";
            }
            ctasksHandle.addtask(1,filename,targetname,"");
            return Content("<script>alert(\"已上传到服务器，进入后台任务\");location=\"/task\"</script>");
        }
        [Breadcrumb(Auth = "1")]
        public ActionResult Wangtongji() {
            var wangtongjipath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wangtongji");
            var file = Directory.GetFiles(wangtongjipath, "*.csv").OrderByDescending(a => a).FirstOrDefault();
            ViewBag.file = file;

            var filespath = file.Substring(0, file.LastIndexOf("."));
            var files = new string[] { };
            if (Directory.Exists(filespath)) {
                files = Directory.GetFiles(filespath,"*.csv").Select(a=>a.Substring(a.LastIndexOf("\\")+1)).OrderByDescending(a=>a).ToArray();
            }
            ViewBag.files = files;

            var lastw = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang");
            ViewBag.dbwang = Directory.GetFiles(wangtongjipath, "*.csv").Select(a=>a.Substring(a.LastIndexOf("\\")+1)).OrderByDescending(a => a).Skip(1).FirstOrDefault();

            var wangpath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang");
            var wangs = Directory.GetFiles(wangpath, "*.csv").OrderByDescending(a => a).Select(a => a.Substring(a.LastIndexOf("\\") + 1)).ToArray();
            ViewBag.wangs = wangs;

            return View();
        }
        [Breadcrumb(Auth = "0")]
        public ActionResult Jstongji(string filename) {
            var wangtongjipath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rrufiles\\wang");
            var wangs = Directory.GetFiles(wangtongjipath, "*.csv").Select(a=>a.Substring(a.LastIndexOf("\\")+1)).OrderByDescending(a => a);

            if (wangs.Count() < 3) {
                return Json(new { ok = true, msg = "网管详表数量<3，无法计算" }, JsonRequestBehavior.AllowGet);
            }
            if (!wangs.Contains(filename)) {
                return Json(new { ok = true, msg = "要对比的网管详表不存在，无法计算" }, JsonRequestBehavior.AllowGet);
            }
            var file2 = wangs.Skip(1).FirstOrDefault();
            if (filename.CompareTo(file2) >= 0) {
                return Json(new { ok = true, msg = "只能计算"+file2+"之前（不包括）的数据" }, JsonRequestBehavior.AllowGet);
            }

            ctasksHandle.addtask(5,wangs.FirstOrDefault(),filename,"");

            return Json(new { ok = true, msg = "对比计算已加入后台任务，请在后台任务中查看运行，成功后在本页面查看对比计算结果" }, JsonRequestBehavior.AllowGet);
        }
    }
}
