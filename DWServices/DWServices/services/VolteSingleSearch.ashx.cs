using DWServices.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace DWServices.services
{
    /// <summary>
    /// VolteSingleSearch 的摘要说明
    /// </summary>
    public class VolteSingleSearch : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //DWServices.Common.User user = (DWServices.Common.User)context.Session["user"];
            //if (user == null)
            //{
            //    context.Response.Write("{\"ok\":false}");
            //    return;
            //}
            string type = context.Request["type"];

            if(type == "alldata")
            {
                var start = context.Request["start"];
                var end = context.Request["end"];
                var num = context.Request["num"];
                if(start != "" || end != "" || num != "")
                {
                    this.GetData(context, start, end, num);
                }
                else
                {
                    this.GetAllData(context);
                }
                
            }
            if(type == "brands")
            {
                this.GetBrandsAndTypes(context);
            }
            if(type == "terminaldata")
            {
                var start = context.Request["start"];
                var end = context.Request["end"];
                var brand = context.Request["brand"];
                var model = context.Request["model"];
                if(start != "" || end != "" ||  brand != "-1" ||  model != "-1")
                {
                    this.GetTerminalData(context, start, end, brand, model);
                }
                else
                {
                    this.GetTerminalAllData(context);
                }
            }
            if(type == "volteheader")
            {
                this.GetVolteHeader(context);
            }
            if(type == "chartdata")
            {
                this.GetChartData(context);
            }
            if(type== "coretable")
            {
                this.GetCoreTableData(context);
            }
            if(type=="wifitable")
            {
                this.GetWifiTableData(context);
            }
            if (type== "download")
            {
                this.GetDownLoad(context);
            }
            if(type== "downloadwifi")
            {
                this.GetDownLoadWifi(context);
            }
            if(type== "getterminalchart")
            {
                this.GetTerminalChart(context);
            }
            if(type == "corereason")
            {
                this.GetCoreReasonData(context);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void GetAllData(HttpContext context)
        {
            // 查询返回
            int limit = Int32.Parse(context.Request["limit"]);
            int offset = Int32.Parse(context.Request["offset"]);
            string sql = @"SELECT B.*, concat(concat(substr(MSISDN, 0, 3), '*****'), substr(MSISDN, 9, 12)) msisdn2
FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM VOLTE_DAY) A WHERE ROWNUM <= "+(limit+offset)+@") B
WHERE RN >= " + offset;
            DataTable dt = OraConnect.ReadData(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                context.Response.Write("{\"total\": 0, \"rows\": 0}");
                return;
            }
            var data = dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
            var rows = data.ToList();
            sql = "select count(1) from volte_day";
            dt = OraConnect.ReadData(sql);
            var total = dt.Rows[0][0];
            
            context.Response.Write(JsonConvert.SerializeObject(new {total=total, rows= rows }));
        }

        public void GetTerminalChart(HttpContext context)
        {
            string sql = @"select count(distinct(MSISDN)) from VOLTE_DAY";
            DataTable dt = OraConnect.ReadData(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                context.Response.Write("{\"ok\": false}");
                return;
            }
            int sum = Int32.Parse(dt.Rows[0][0].ToString());
            sql = @"select a.BRAND, sum(a.c) sum from (
    select BRAND, count(1) c from  VOLTE_DAY
 where BRAND is not null and BRAND<> '--'
group by MSISDN,BRAND
    ) a group by a.BRAND";
            dt = OraConnect.ReadData(sql);
            var data = dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
            var rows = data.ToList();

            context.Response.Write(JsonConvert.SerializeObject(new {ok=true, sum = sum, data = rows }));
        }

        public void GetCoreReasonData(HttpContext context)
        {
            string mmeName = context.Request["mmename"];
            int downmos = 0;
            int upmos = 0;
            string sql = "select count(1) from VOLTE_DAY where CLASSNAME = '核心网' and DOWNMOS< 4 and MMENAME = '" + mmeName + "'";
            DataTable dt = OraConnect.ReadData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                downmos = Int32.Parse(dt.Rows[0][0].ToString());
            }
            sql = "select count(1) from VOLTE_DAY where CLASSNAME = '核心网' and upmos< 4 and MMENAME = '" + mmeName + "'";
            dt = OraConnect.ReadData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                upmos = Int32.Parse(dt.Rows[0][0].ToString());
            }
            sql = "select SCENENAME, count(1) from VOLTE_DAY where CLASSNAME = '核心网' and MMENAME = '"+mmeName+"' and FIRSTREASON <> '200 OK' group by SCENENAME";
            Dictionary<string, double> coreData2 = new Dictionary<string, double>();
            coreData2.Add("upmos", upmos);
            coreData2.Add("downmos", downmos);
            coreData2.Add("认证鉴权阶段发起释放", 0);
            coreData2.Add("永久性失败", 0);
            coreData2.Add("没有标准原因", 0);
            coreData2.Add("请求失败", 0);
            coreData2.Add("VSNCP协商失败", 0);
            coreData2.Add("网络异常", 0);
            coreData2.Add("LCP协商阶段发起释放", 0);
            coreData2.Add("MIP协商失败", 0);
            coreData2.Add("暂时性失败", 0);
            Dictionary<string, double> core2 = ChartTableToDic(sql);
            foreach (var a in core2.Keys)
            {
                coreData2[a] = core2[a];
            }
            context.Response.Write(JsonConvert.SerializeObject(new { ok = true, data=coreData2}));
        }

        public void GetDownLoad(HttpContext context)
        {
            string start = context.Request["start"];
            string end = context.Request["end"];
            string num = context.Request["num"];
            string sql = "select * from volte_day where ";
            if (start != "")
            {
                sql += " starttime > to_date('" + start + "','yyyy-mm-dd') ";
            }
            else
            {
                sql += " 1 = 1 ";
            }
            if (end != "")
            {
                sql += " and endtime < to_date('" + end + "','yyyy-mm-dd') ";
            }
            else
            {
                sql += " and 1 = 1 ";
            }
            if (num != "")
            {
                sql += " and msisdn = '" + num + "'";
            }
            else
            {
                sql += " and 1 = 1";
            }
            DataTable dt = OraConnect.ReadData(sql);
            StringBuilder sb = new StringBuilder();
            sb.Append("开始时间,结束时间,业务类型,业务状态,MSISDN,IMSI,IMEI,终端品牌,终端型号,VoLTE终端OS版本,\"Warning Text\",\"Warning Text\",第一拆线网元类型,第一拆线网元IP,第一拆线网元,第一拆线原因,接入网类型,省,市,初始4G小区,跟踪区,\"型号PCSCF IP\",PCSCF名称,\"ICSCF IP\",ICSCF名称,\"MME IP\",\"MME 名称\",\"SGW IP\",\"SGW 名称\",\"eNodeB IP\",eNodeB名称,媒体类型,初始ECGI,结束ECGI,主被叫标识,主叫号码,被叫号码,振铃时延(ms),应答时延(ms),通话时长(s),初始eNodeB,结束eNodeB,接口类型,对端呼叫类型,编解码类型,上行编解码速率(kbps),下行编解码速率(kbps),上行视频分辨率,下行视频分辨率,上行视频帧率(fps),下行视频帧率(fps),上行MOS均值,上行抖动均值(RTCP)(ms),上行RTP期望包数(RTCP),上行RTP丢包数(RTCP),下行MOS均值,下行抖动均值(RTCP)(ms),下行RTP期望包数(RTCP),下行RTP丢包数(RTCP),上行MOS差周期数,上行MOS统计周期数,下行MOS差周期数,下行MOS统计周期数,环路时延均值(ms),上行IPMOS均值,上行抖动均值(ms),上行RTP期望包数,上行RTP丢包数,下行IPMOS均值,下行抖动均值(ms),下行RTP期望包数,下行RTP丢包数,上行IPMOS差周期数,上行IPMOS统计周期数,下行IPMOS差周期数,下行RTP丢包数,单通标识,上行单通时长(RTP)(ms),下行单通时长(RTP)(ms),上行单通时长(RTCP)(ms),下行单通时长(RTCP)(ms),上行吞字时长(ms),上行断续时长(ms),下行吞字时长(ms),下行断续时长(ms),切换请求次数,切换成功次数,class-name,scene-name\n");
            for(int i=0;i<dt.Rows.Count;i++)
            {
                for(int j=0;j<89;j++)
                {
                    if(j==88) sb.Append("\"" + dt.Rows[i][j].ToString() + "\"");
                    else sb.Append("\""+dt.Rows[i][j].ToString()+"\",");
                }
                sb.Append("\n");
            }
            context.Response.ContentType = "multipart/form-data";
            context.Response.AddHeader("Content-Disposition", "attachment;fileName=" + "volte-data.csv");
            System.IO.StringWriter strWriter = new System.IO.StringWriter(sb);
            context.Response.Write(strWriter.ToString());
        }

        public void GetData(HttpContext context, string start, string end, string num)
        {
            int limit = Int32.Parse(context.Request["limit"]);
            int offset = Int32.Parse(context.Request["offset"]);
            string sql = "select * from volte_day where ";
            if(start != "")
            {
                sql += " starttime > to_date('" + start + "','yyyy-mm-dd') ";
            }
            else
            {
                sql += " 1 = 1 ";
            }
            if(end != "")
            {
                sql += " and endtime < to_date('" + end + "','yyyy-mm-dd') ";
            }
            else
            {
                sql += " and 1 = 1 ";
            }
            if(num != "")
            {
                sql += " and msisdn = '" + num + "'";
            }
            else
            {
                sql += " and 1 = 1";
            }
            DataTable dt = OraConnect.ReadData(sql);
            var total = dt.Rows.Count;
            sql = @"SELECT B.*, concat(concat(substr(MSISDN, 0, 3), '*****'), substr(MSISDN, 9, 12)) msisdn2
FROM (SELECT A.*, ROWNUM RN FROM (" + sql + @") A WHERE ROWNUM <= "+(limit+offset)+@" ) B WHERE RN >= "+offset;
            dt = OraConnect.ReadData(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                context.Response.Write("{\"total\": 0, \"rows\": 0}");
                return;
            }
            var data = dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
            var rows = data.ToList();

            context.Response.Write(JsonConvert.SerializeObject(new { total = total, rows = rows }));
        }


        public void GetBrandsAndTypes(HttpContext context)
        {
            string sql = "select brand,model from volte_day group by brand, model";
            DataTable dt = OraConnect.ReadData(sql);
            if(dt == null || dt.Rows.Count<=0)
            {
                context.Response.Write("{\"ok\": false\"brands\":[], \"models\":{}}");
            }
            List<string> brands = new List<string>();
            Dictionary<string, List<string>> models = new Dictionary<string, List<string>>();
            for(int i=0;i<dt.Rows.Count;i++)
            {
                string brand = dt.Rows[i][0].ToString();
                if(!brands.Contains(brand))
                {
                    brands.Add(brand);
                }
                if(!models.Keys.Contains(brand))
                {
                    models.Add(brand, new List<string>());
                }
                else
                {
                    models[brand].Add(dt.Rows[i][1].ToString());
                }
            }
            string s = JsonConvert.SerializeObject(new { ok=true, brands=brands, models=models});
            context.Response.Write(s);
        }

        public void GetTerminalAllData(HttpContext context)
        {
            string sql = @"select a.brand,
       a.badcount,
       b.sumcount,
       round((b.sumcount - a.badcount) / b.sumcount, 4) * 100 rate,
       b.ringdelay,
       b.upmos,
       b.DOWNMOS,
       b.UPSINGLETIMERTP,
       b.DOWNSINGLETIMERTP
from(select brand, count(1) badcount
      from VOLTE_DAY
      where (FIRSTREASON <> '200 OK'
         or UPMOS < 4
         or DOWNMOS < 4)
        and BRAND is not null and BRAND <> '--'
      group by brand) a
       left join(select brand,
                         count(1)                         sumcount,
                         round(avg(RINGDELAY), 4)         ringdelay,
                         round(avg(UPMOS), 4)             upmos,
                         round(avg(DOWNMOS), 4)           DOWNMOS,
                         round(avg(UPSINGLETIMERTP), 4)   UPSINGLETIMERTP,
                         round(avg(DOWNSINGLETIMERTP), 4) DOWNSINGLETIMERTP
                  from volte_day
                  group by brand) b on a.brand = b.brand";

            DataTable dt = OraConnect.ReadData(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                context.Response.Write("{\"total\": 0, \"rows\": 0}");
                return;
            }
            var data = dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
            var total = data.Length;
            var rows = data.ToList();
            string rowsJson = JsonConvert.SerializeObject(rows);
            string s = "{\"total\":" + total + ",\"rows\":" + rowsJson + "}";

            context.Response.Write(rowsJson);
        }

        public void GetTerminalData(HttpContext context, string start, string end, string brand, string model)
        {
            var tj = "";
            if(start != "")
            {
                tj += " and starttime > to_date('" + start + "','yyyy-mm-dd')  ";
            }
            if(end != "")
            {
                tj += " and endtime < to_date('" + end + "','yyyy-mm-dd') ";
            }
            if(brand != "-1")
            {
                tj += " and brand = '" + brand + "' ";
            }
            if(model != "-1")
            {
                tj += " and model = '" + model + "' ";
            }
            string sql = @"select a.brand,
       a.badcount,
       b.sumcount,
       round((b.sumcount - a.badcount) / b.sumcount, 4) * 100 rate,
       b.ringdelay,
       b.upmos,
       b.DOWNMOS,
       b.UPSINGLETIMERTP,
       b.DOWNSINGLETIMERTP
from(select brand, count(1) badcount
      from VOLTE_DAY
      where (FIRSTREASON <> '200 OK'
         or UPMOS < 4
         or DOWNMOS < 4) " + tj +
      @"group by brand) a
       left join(select brand,
                         count(1)                         sumcount,
                         round(avg(RINGDELAY), 4)         ringdelay,
                         round(avg(UPMOS), 4)             upmos,
                         round(avg(DOWNMOS), 4)           DOWNMOS,
                         round(avg(UPSINGLETIMERTP), 4)   UPSINGLETIMERTP,
                         round(avg(DOWNSINGLETIMERTP), 4) DOWNSINGLETIMERTP
                  from volte_day where "+tj.Substring(4)+
                  @"group by brand) b on a.brand = b.brand";

            DataTable dt = OraConnect.ReadData(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                context.Response.Write("{\"total\": 0, \"rows\": 0}");
                return;
            }
            var data = dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
            var total = data.Length;
            var rows = data.ToList();
            string rowsJson = JsonConvert.SerializeObject(rows);
            string s = "{\"total\":" + total + ",\"rows\":" + rowsJson + "}";

            context.Response.Write(rowsJson);
        }

        public void GetVolteHeader(HttpContext context)
        {
            double allcount = 0;
            int badcount = 0;
            int coreCount = 0;
            int wifiCount = 0;
            int terCount = 0;
            string sql = "select count(1) from VOLTE_DAY";
            var dt = OraConnect.ReadData(sql);
            if(dt != null && dt.Rows.Count > 0)
            {
                allcount = double.Parse(dt.Rows[0][0].ToString());
            }
            sql = "select count(1) from VOLTE_DAY where FIRSTREASON <> '200 OK' or UPMOS < 4 or DOWNMOS < 4";
            dt = OraConnect.ReadData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                badcount = Int32.Parse(dt.Rows[0][0].ToString());
            }
            List<int> res = GetCountGroupByClassName();
            wifiCount = res[0];
            coreCount = res[1];
            terCount = res[2];
            if (allcount == 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(new { ok=false }));
            }
            context.Response.Write(JsonConvert.SerializeObject(new { ok=true, data=new { allcount=allcount, badcount=badcount, rate=badcount/allcount, corerate= coreCount /allcount, terrate= terCount /allcount, wifirate= wifiCount /allcount}}));
        }

        public void GetChartData(HttpContext context)
        {
            string sql = @"select b.city,
       decode(ROUND((b.allcount - a.badcount) / b.allcount, 4) * 100, null, 1,
              ROUND((b.allcount - a.badcount) / b.allcount, 4) * 100) rate
from (select CITY, count(1) allcount from VOLTE_DAY where CITY is not null
                                                      and CLASSNAME = '无线' group by CITY) b
       left join (select CITY, count(1) badcount
                  from VOLTE_DAY
                  where (FIRSTREASON <> '200 OK' or UPMOS < 4 or DOWNMOS < 4)
                    and CLASSNAME = '无线'
                    and CITY is not null
                  group by CITY) a on a.city = b.city";
            Dictionary<string, double> wifiData = ChartTableToDic(sql);
            sql = @"select '全省', decode(ROUND((b.allcount - a.badcount) / b.allcount, 4) * 100, null, 1,
                    ROUND((b.allcount - a.badcount) / b.allcount, 4) * 100) rate
from (select 1 c, count(1) allcount from VOLTE_DAY where CITY is not null
                                                     and CLASSNAME = '无线') b
       left join (select 1 c, count(1) badcount
                  from VOLTE_DAY
                  where (FIRSTREASON <> '200 OK' or UPMOS < 4 or DOWNMOS < 4)
                    and CLASSNAME = '无线') a on a.c = b.c";
            var t = ChartTableToDic(sql);
            wifiData.Add("全省", t["全省"]);
            sql = @"select b.MMENAME,
       decode(ROUND((b.allcount - a.badcount) / b.allcount, 4) * 100, null, 1,
              ROUND((b.allcount - a.badcount) / b.allcount, 4) * 100) rate
from (select MMENAME, count(1) allcount
      from VOLTE_DAY
      where MMENAME is not null  and MMENAME <> '--'
        and CLASSNAME = '核心网'
      group by MMENAME) b
       left join (select MMENAME, count(1) badcount
                  from VOLTE_DAY
                  where (FIRSTREASON <> '200 OK' or UPMOS < 4 or DOWNMOS < 4)
                    and CLASSNAME = '核心网'
                    and MMENAME is not null and MMENAME <> '--'
                  group by MMENAME) a on a.MMENAME = b.MMENAME";
            Dictionary<string, double> coreData = ChartTableToDic(sql);
            sql = @"select b.BRAND,
       decode(ROUND((b.allcount - a.badcount) / b.allcount, 4) * 100, null, 1,
              ROUND((b.allcount - a.badcount) / b.allcount, 4) * 100) rate
from (select BRAND, count(1) allcount
      from VOLTE_DAY
      where BRAND is not null and BRAND <> '--'
        and CLASSNAME = '用户或终端'
      group by BRAND) b
       left join (select BRAND, count(1) badcount
                  from VOLTE_DAY
                  where (FIRSTREASON <> '200 OK' or UPMOS < 4 or DOWNMOS < 4)
                    and CLASSNAME = '用户或终端'
                    and BRAND is not null and BRAND <> '--'
                  group by BRAND) a on a.BRAND = b.BRAND";
            Dictionary<string, double> terData = ChartTableToDic(sql);
            sql = @"select CONCAT(CONCAT(a.BRAND, '-'), a.MODEL)                          BRAND,
       decode(ROUND((a.allcount - b.badcount) / a.allcount, 4) * 100, null, 100,
              ROUND((a.allcount - b.badcount) / a.allcount, 4) * 100) rate
from (select BRAND, MODEL, count(1) allcount
      from VOLTE_DAY
      where BRAND is not null
        and BRAND <> '--'
        and CLASSNAME = '用户或终端'
      group by BRAND, MODEL) a
       left join (select BRAND, MODEL, count(1) badcount
                  from VOLTE_DAY
                  where (FIRSTREASON <> '200 OK' or UPMOS < 4 or DOWNMOS < 4)
                    and CLASSNAME = '用户或终端'
                    and BRAND is not null
                    and BRAND <> '--'
                  group by BRAND, MODEL) b on a.BRAND = b.BRAND and a.MODEL = b.MODEL";
            Dictionary<string, double> terData2 = ChartTableToDic(sql);
            int downmos = 0;
            int upmos = 0;
            sql = "select count(1) from VOLTE_DAY where CLASSNAME = '核心网' and MMENAME is not null and MMENAME <> '--' and DOWNMOS< 4";
            DataTable dt = OraConnect.ReadData(sql);
            if(dt != null && dt.Rows.Count > 0)
            {
                downmos = Int32.Parse(dt.Rows[0][0].ToString());
            }
            sql = "select count(1) from VOLTE_DAY where CLASSNAME = '核心网' and MMENAME is not null and MMENAME <> '--' and upmos< 4";
            dt = OraConnect.ReadData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                upmos = Int32.Parse(dt.Rows[0][0].ToString());
            }
            sql = "select SCENENAME, count(1) from VOLTE_DAY where CLASSNAME = '核心网' and MMENAME is not null and MMENAME <> '--' and FIRSTREASON<> '200 OK' group by SCENENAME";
            Dictionary<string, double> coreData2 = new Dictionary<string, double>();
            coreData2.Add("upmos", upmos);
            coreData2.Add("downmos", downmos);
            coreData2.Add("认证鉴权阶段发起释放", 0);
            coreData2.Add("永久性失败", 0);
            coreData2.Add("没有标准原因", 0);
            coreData2.Add("请求失败", 0);
            coreData2.Add("VSNCP协商失败", 0);
            coreData2.Add("网络异常", 0);
            coreData2.Add("LCP协商阶段发起释放", 0);
            coreData2.Add("MIP协商失败", 0);
            coreData2.Add("暂时性失败", 0);
            Dictionary<string, double> core2 = ChartTableToDic(sql);
            foreach(var a in core2.Keys)
            {
                coreData2[a] = core2[a];
            }
            context.Response.Write(JsonConvert.SerializeObject(new { ok=true, data= new { core=coreData, core2=coreData2, wifi=wifiData, terminal=terData, terminal2=terData2 }}));
        }

        public Dictionary<string,double> ChartTableToDic(string sql)
        {
            var dt = OraConnect.ReadData(sql);
            if(dt == null)
            {
                return new Dictionary<string, double>();
            }
            Dictionary<string, double> dic = new Dictionary<string, double>();
            for (int i=0;i<dt.Rows.Count;i++)
            {
                dic.Add(dt.Rows[i][0].ToString(), double.Parse(dt.Rows[i][1].ToString()));
            }
            return dic;
        }

        public List<int> GetCountGroupByClassName()
        {
            List<int> res = new List<int>();
            string sql = "select CLASSNAME, count(1) from VOLTE_DAY where CLASSNAME in ('核心网','用户或终端', '无线') group by CLASSNAME";
            var dt = OraConnect.ReadData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString() == "无线") res.Add(Int32.Parse(dt.Rows[i][1].ToString()));
                    if (dt.Rows[i][0].ToString() == "核心网") res.Add(Int32.Parse(dt.Rows[i][1].ToString()));
                    if (dt.Rows[i][0].ToString() == "用户或终端") res.Add(Int32.Parse(dt.Rows[i][1].ToString()));
                }
            }
            return res;
        }

        public void GetWifiTableData(HttpContext context)
        {
            string city = context.Request["city"];
            string sql = "";
            if(city != null && city != "全省")
            {
                sql = @"select a.start4g,decode(b.upmos, null,0, b.UPMOS) upmos,decode(c.DOWNMOS,null,0,c.DOWNMOS) downmos,decode(d.FIRSTREASON,null,0,d.FIRSTREASON) firstreason from
(select START4G from VOLTE_DAY
where CITY = '"+city+ @"' group by START4G) a left join (
select START4G, count(UPMOS) upmos  from VOLTE_DAY
where CITY = '" + city + @"' and UPMOS < 4 group by START4G) b
on a.start4g = b.start4g left join (select START4G, count(DOWNMOS) DOWNMOS  from VOLTE_DAY
where CITY = '" + city + @"' and DOWNMOS < 4 group by START4G) c on a.START4G = c.START4G left join
    (select START4G, count(FIRSTREASON) FIRSTREASON  from VOLTE_DAY
where CITY = '" + city + @"' and FIRSTREASON <> '200 OK' group by START4G) d on a.START4G = d.START4G order by a.START4G";
            }
            else
            {
                sql = @"select a.city,a.start4g,decode(b.upmos, null,0, b.UPMOS) upmos,decode(c.DOWNMOS,null,0,c.DOWNMOS) downmos,decode(d.FIRSTREASON,null,0,d.FIRSTREASON) firstreason from
(select city,START4G from VOLTE_DAY
group by CITY,START4G) a left join (
select START4G, count(UPMOS) upmos  from VOLTE_DAY
where UPMOS < 4 group by CITY,START4G) b
on a.start4g = b.start4g left join (select START4G, count(DOWNMOS) DOWNMOS  from VOLTE_DAY
where DOWNMOS < 4 group by CITY,START4G) c on a.START4G = c.START4G left join
    (select START4G, count(FIRSTREASON) FIRSTREASON  from VOLTE_DAY
where FIRSTREASON <> '200 OK' group by CITY,START4G) d on a.START4G = d.START4G order by a.START4G";
            }
            DataTable dt = OraConnect.ReadData(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                context.Response.Write("{\"total\": 0, \"rows\": 0}");
                return;
            }
            var data = dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
            var total = data.Length;
            var rows = data.ToList();
            string rowsJson = JsonConvert.SerializeObject(rows);
            string s = "{\"total\":" + total + ",\"rows\":" + rowsJson + "}";

            context.Response.Write(rowsJson);
        }


        public void GetCoreTableData(HttpContext context)
        {
            string city = context.Request["city"];
            string sql = @"select a.SCENENAME                                   reason,
       decode(b.downmos, null, 0, b.downmos)         downmos,
       decode(c.upmos, null, 0, c.upmos)             upmos,
       decode(d.firstreason, null, 0, d.FIRSTREASON) firstreason
from (select SCENENAME from VOLTE_DAY where CLASSNAME = '核心网' group by SCENENAME) a
       left join (select SCENENAME, count(DOWNMOS) downmos
                  from VOLTE_DAY
                  where DOWNMOS < 4
                    and CLASSNAME = '核心网'
                  group by SCENENAME) b on a.SCENENAME = b.SCENENAME
       left join (select SCENENAME, count(UPMOS) upmos
                  from VOLTE_DAY
                  where UPMOS < 4
                    and CLASSNAME = '核心网'
                  group by SCENENAME) c on a.SCENENAME = c.SCENENAME
       left join (select SCENENAME, count(FIRSTREASON) firstreason
                  from VOLTE_DAY
                  where FIRSTREASON <> '200 OK'
                    and CLASSNAME = '核心网'
                  group by SCENENAME) d on a.SCENENAME = d.SCENENAME";
            DataTable dt = OraConnect.ReadData(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                context.Response.Write("{\"total\": 0, \"rows\": 0}");
                return;
            }
            var data = dt.AsEnumerable().Select(a =>
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn v in dt.Columns)
                {
                    dic[v.ColumnName] = a[v.ColumnName];
                }
                return dic;
            }).ToArray();
            var total = data.Length;
            var rows = data.ToList();
            string rowsJson = JsonConvert.SerializeObject(rows);
            string s = "{\"total\":" + total + ",\"rows\":" + rowsJson + "}";

            context.Response.Write(rowsJson);
        }

        public void GetDownLoadWifi(HttpContext context)
        {
            string city = context.Request["wifiCity"];
            string sql = "";
            if (city != null && city != "全省")
            {
                sql = @"select a.start4g,decode(b.upmos, null,0, b.UPMOS) upmos,decode(c.DOWNMOS,null,0,c.DOWNMOS) downmos,decode(d.FIRSTREASON,null,0,d.FIRSTREASON) firstreason from
(select START4G from VOLTE_DAY
where CITY = '" + city + @"' group by START4G) a left join (
select START4G, count(UPMOS) upmos  from VOLTE_DAY
where CITY = '" + city + @"' and UPMOS < 4 group by START4G) b
on a.start4g = b.start4g left join (select START4G, count(DOWNMOS) DOWNMOS  from VOLTE_DAY
where CITY = '" + city + @"' and DOWNMOS < 4 group by START4G) c on a.START4G = c.START4G left join
    (select START4G, count(FIRSTREASON) FIRSTREASON  from VOLTE_DAY
where CITY = '" + city + @"' and FIRSTREASON <> '200 OK' group by START4G) d on a.START4G = d.START4G order by a.START4G";
            }
            else
            {
                sql = @"select a.city,a.start4g,decode(b.upmos, null,0, b.UPMOS) upmos,decode(c.DOWNMOS,null,0,c.DOWNMOS) downmos,decode(d.FIRSTREASON,null,0,d.FIRSTREASON) firstreason from
(select city,START4G from VOLTE_DAY
group by CITY,START4G) a left join (
select START4G, count(UPMOS) upmos  from VOLTE_DAY
where UPMOS < 4 group by CITY,START4G) b
on a.start4g = b.start4g left join (select START4G, count(DOWNMOS) DOWNMOS  from VOLTE_DAY
where DOWNMOS < 4 group by CITY,START4G) c on a.START4G = c.START4G left join
    (select START4G, count(FIRSTREASON) FIRSTREASON  from VOLTE_DAY
where FIRSTREASON <> '200 OK' group by CITY,START4G) d on a.START4G = d.START4G order by a.START4G";
            }
            DataTable dt = OraConnect.ReadData(sql);
            StringBuilder sb = new StringBuilder();
            sb.Append("城市,小区,上行MOS质差数,下行MOS质差数,\"非200 ok 质差数\"\n");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (j == 4) sb.Append("\"" + dt.Rows[i][j].ToString() + "\"");
                    else sb.Append("\"" + dt.Rows[i][j].ToString() + "\",");
                }
                sb.Append("\n");
            }
            context.Response.ContentType = "multipart/form-data";
            context.Response.AddHeader("Content-Disposition", "attachment;fileName=" + "volte-wifi-data.csv");
            System.IO.StringWriter strWriter = new System.IO.StringWriter(sb);
            context.Response.Write(strWriter.ToString());
        }
    }
}