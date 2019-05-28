using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DWWinService
{
    class Runner
    {
        static AnalyzerStatusServer ass = null;
        static List<Worker> workers = new List<Worker>();
        public static void run() {
            LOG.WriteLog("start");
            var cfgs=AnalyzerCfgReader.load();
            ass = new AnalyzerStatusServer();
            ass.SendStr += ass_SendStr;
            ass.Start();
            foreach (var c in cfgs) {
                var asd = new Worker(c, asd_OnStatusChange);
                workers.Add(asd);
            }
        }

        static string ass_SendStr()
        {
            var xml = XDocument.Parse("<Analyzers></Analyzers>");
            foreach (var worker in workers) {
                XElement ele = new XElement("analyzer");
                ele.SetElementValue("name",worker.cfg.name);
                ele.SetElementValue("sttsd", worker.StatusDay);
                ele.SetElementValue("sttsq", worker.StatusQuarter);
                ele.SetElementValue("errd", worker.ErrDay);
                ele.SetElementValue("errq", worker.ErrQuarter);
                xml.Root.Add(ele);
            }
            return xml.ToString(); ;
        }

        static void asd_OnStatusChange(Analyzer analyzer, AnalyzerStatus status, LoopType looptype)
        {
            AnalyzerStatus before;
            var loopstr = "";
            switch (looptype) { 
                case LoopType.day:
                    before = analyzer.status_day;
                    loopstr = "天分析器";
                    break;
                case LoopType.quarter:
                    before = analyzer.status_quarter;
                    loopstr = "15分钟分析器";
                    break;
                default:
                    before = status;
                    break;
            }
            LOG.WriteLog(analyzer.cfg.dec+"-"+loopstr+"："+before+"->"+status);
            var err = "";
            if (status == AnalyzerStatus.error) {
                if (looptype == LoopType.day) {
                    err = ","+analyzer.error_day;
                }
                else if (looptype == LoopType.quarter) {
                    err = ","+analyzer.error_quarter;
                }
            }
            ass.send(Encoding.GetEncoding("gb2312").GetBytes(analyzer.cfg.name + "," + looptype + "," + before + "," + status+err+"|"));
        }
    }
}
