using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CTCCGoods.Controllers
{
    public enum e_etctype { 
        [EnumDescription("文本")]
        text=0,
        [EnumDescription("数字")]
        numner=1,
        [EnumDescription("时间")]
        time=2,
        [EnumDescription("下拉")]
        dropdown=3
    }
    public class e_templet {
        public int? id { get; set; }
        public string name { get; set; }
        public string des { get; set; }
        public string filename { get; set; }
        public Dictionary<int, e_templet_col> cols { get; set; }
    }
    public class e_templet_col
    {
        public int? etid { get; set; }
        public int? ordernum { get; set; }
        public string name { get; set; }
        public string classname { get; set; }
        public string comments { get; set; }
        public e_etctype? etctype { get; set; }
        public int? eeid { get; set; }
        public string eename { get; set; }
        public string eeval { get; set; }
        public bool? notnull { get; set; }
        public string expr { get; set; }
        public string ecc { get; set; }
    }
    public class etempletfactory {
        private e_templet _templet;
        public e_templet templet { get { return _templet; } }
        public etempletfactory(int id) {
            load(id);
        }
        public etempletfactory(e_templet temp){
            if (temp == null || temp.cols == null || temp.cols.Count <= 0)
            {
                throw new Exception("模板信息和模板列不能为空");
            }
            DateTime ctime = DateTime.Now;
            if (string.IsNullOrWhiteSpace(temp.name))
            {
                throw new Exception("模板名不能为空");
            }
            foreach (var col in temp.cols)
            {
                if (!col.Value.ordernum.HasValue || string.IsNullOrWhiteSpace(col.Value.name) || !col.Value.etctype.HasValue || !col.Value.notnull.HasValue)
                {
                    throw new Exception("模板列不完整");
                }
            }
            if (temp.cols.Count != temp.cols.Select(a => a.Value.ordernum.Value).Distinct().Count())
            {
                throw new Exception("模板列序号重复");
            }
            DB db = new DB();
            try
            {
                var sqlinserttemp = string.Format("insert into e_templet (name,des) values('{0}','{1}')", temp.name,temp.des);
                var etid = db.Insertobj(sqlinserttemp);
                foreach (var col in temp.cols)
                {
                    var colsql = string.Format("insert into e_templet_col (etid,ordernum,name,classname,etctype,eeid,notnull,expr,ecc,comments) values({0},{1},'{2}','{3}',{4},{5},{6},'{7}','{8}', '{9}')",
                        etid, col.Value.ordernum,col.Value.name,col.Value.classname,(int?)col.Value.etctype,col.Value.eeid,col.Value.notnull.HasValue&&col.Value.notnull.Value?1:0,col.Value.expr,col.Value.ecc, col.Value.comments);
                    db.Execobj(colsql);
                }
                db.End(true);
                load(etid);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
        }
        private void load(int id) {
            var etempdic = DB.QueryOne(@"select * from e_templet where id="+id);
            if (etempdic == null)
            {
                throw new Exception("模板不存在");
            }
            _templet = new e_templet();
            _templet.id = O2.O2I(etempdic["id"]);
            _templet.name = etempdic["name"].ToString();
            _templet.des = etempdic["des"].ToString();
            _templet.filename = etempdic["filename"].ToString();
            _templet.cols = new Dictionary<int, e_templet_col>();
            var colsdics = DB.QueryAsDics("select a.etid,a.ordernum,a.name,a.classname,a.comments,a.etctype,a.eeid,b.name eename,b.val eeval,a.notnull,a.expr,a.ecc from e_templet_col a left join e_enum b on a.eeid=b.id where etid=" + id + " order by a.ordernum");
            if (colsdics != null) {
                foreach (var dic in colsdics) {
                    var col = new e_templet_col();
                    col.etid = O2.O2I(dic["etid"]);
                    col.ordernum = O2.O2I(dic["ordernum"]);
                    col.name = dic["name"].ToString();
                    col.classname = dic["classname"].ToString();
                    col.comments = dic["comments"].ToString();
                    col.etctype = (e_etctype)O2.O2I(dic["etctype"]);
                    col.eeid = O2.O2I(dic["eeid"]);
                    col.eename = dic["eename"].ToString();
                    col.eeval = dic["eeval"].ToString();
                    col.notnull = O2.O2B(dic["notnull"]);
                    col.expr = dic["expr"].ToString();
                    col.ecc=dic["ecc"].ToString();
                    _templet.cols[col.ordernum.Value] = col;
                }
            }
        }
        public bool update()
        {
            var res = false;

            if (_templet == null || _templet.cols == null || _templet.cols.Count <= 0)
            {
                throw new Exception("模板信息和模板列不能为空");
            }
            DateTime ctime = DateTime.Now;
            if (string.IsNullOrWhiteSpace(_templet.name))
            {
                throw new Exception("模板名不能为空");
            }
            foreach (var col in _templet.cols)
            {
                if (!col.Value.ordernum.HasValue || string.IsNullOrWhiteSpace(col.Value.name) || !col.Value.etctype.HasValue || !col.Value.notnull.HasValue)
                {
                    throw new Exception("模板列不完整");
                }
            }
            if (_templet.cols.Count != _templet.cols.Select(a => a.Value.ordernum.Value).Distinct().Count())
            {
                throw new Exception("模板列序号重复");
            }

            DB db = new DB();
            try
            {
                db.Execobj("update e_templet set name='"+_templet.name+"',des='"+_templet.des+"',filename='"+_templet.filename+"' where id="+_templet.id);
                db.Execobj("delete e_templet_col where etid=" + _templet.id);
                foreach (var col in _templet.cols)
                {
                    var colsql = string.Format("insert into e_templet_col (etid,ordernum,name,classname,etctype,eeid,notnull,expr,ecc,comments) values({0},{1},'{2}','{3}',{4},{5},{6},'{7}','{8}','{9}')",
                        _templet.id, col.Value.ordernum, col.Value.name, col.Value.classname,(int?)col.Value.etctype, col.Value.eeid, col.Value.notnull.HasValue && col.Value.notnull.Value ? 1 : 0, col.Value.expr, col.Value.ecc, col.Value.comments);
                    db.Execobj(colsql);
                }

                db.End(true);
            }
            catch (Exception ex)
            {
                db.End(false);
                throw ex;
            }
            load(_templet.id.Value);
            res = true;
            return res;
        }
        public static e_templet CreateEmptyEtemplet() {
            var temp = new e_templet() {cols=new Dictionary<int,e_templet_col>() };
            return temp;
        }
        public static void Analysis(int whid, int epid, string fileName, int userid)
        {
            try
            {
                //DataTable rules = DB.Query("select * from e_templet_col etc join e_plan ep on etc.etid = ep.etid where ep.id = " + epid);
                var id = DB.QueryOne("select Top 1 etc.etid from e_templet_col etc join e_plan ep on etc.etid = ep.etid where ep.id = " + epid);
                var cityName = DB.QueryOne("select name from cwarehouse where id = " + whid)["name"].ToString();
                var rules = new etempletfactory(O2.O2I(id["etid"].ToString()));
                bool hasClass = true;
                bool hasComments = true;
                //for (int i = 1; i < rules.templet.cols.Count; i++)
                //{
                //    if (rules.templet.cols[i].classname != null && rules.templet.cols[i].classname != "")
                //    {
                //        hasClass = true;
                //        break;
                //    }
                //}
                //for (int i = 1; i < rules.templet.cols.Count; i++)
                //{
                //    if (rules.templet.cols[i].comments != null && rules.templet.cols[i].classname != "")
                //    {
                //        hasComments = true;
                //        break;
                //    }
                //}
                IWorkbook workbook = null;
                FileStream fileStream = new FileStream(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName), FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本  
                {
                    workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook  
                }
                else if (fileName.IndexOf(".xls") > 0) // 2003版本  
                {
                    workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook  
                }
                bool isRight = true;
                ISheet sheet = workbook.GetSheetAt(0);
                IRow row;
                StringBuilder sb = new StringBuilder();
                int start = 3;
                if (sheet.LastRowNum + 1 == start)
                {
                    sb.Append("空白模板，请填写数据！\r\n");
                    isRight = false;
                }
                if (sheet.GetRow(start - 1) != null && rules.templet.cols.Count != sheet.GetRow(start - 1).Cells.Count)
                {
                    sb.Append("未使用标准模板，请重新上传\r\n");
                    isRight = false;
                }
                else
                {
                    for (int i = start; i < sheet.LastRowNum + 1; i++)  //对工作表每一行  
                    {
                        row = sheet.GetRow(i);   //row读入第i行数据  
                        int last = hasClass ? sheet.GetRow(1).Cells.Count : sheet.GetRow(0).Cells.Count;
                        if (row != null)
                        {
                            for (int j = 0; j < last; j++)  //对工作表每一列  
                            {
                                string name = hasClass ? sheet.GetRow(1).GetCell(j).ToString() : sheet.GetRow(0).GetCell(j).ToString();
                                ICell cell = row.GetCell(j); //获取i行j列
                                int typeNum = (int)rules.templet.cols[j + 1].etctype;
                                CellType type = typeNum == 1 || typeNum == 2 ? CellType.Numeric : CellType.String;
                                bool notNull = rules.templet.cols[j + 1].notnull.Value;
                                string classname = rules.templet.cols[j + 1].classname;

                                if (cell == null)
                                {
                                    cell = row.CreateCell(j);
                                }
                                var eccstr = rules.templet.cols[j + 1].ecc.ToString().Trim();
                                var ecclist = eccstr.Split(new string[] { "&amp;&amp;","&&" }, StringSplitOptions.RemoveEmptyEntries);
                                if (notNull && cell.ToString() == "")
                                {
                                    isRight = false;
                                    sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据不能为空\r\n");
                                }
                                else if (cell.ToString() == "")
                                {
                                    var eccRs = ecclist.Where(a=>a.StartsWith("R")).ToArray();
                                    foreach(var ecc in eccRs){
                                        Regex reg = new Regex(@"(\d+)");
                                        var match = reg.Match(ecc);
                                        string result1 = match.Groups[0].ToString();
                                        reg = new Regex(@"\{(\S+?)\}");
                                        match = reg.Match(ecc);
                                        string result2 = match.Groups[0].ToString();
                                        string thisval = row.GetCell(O2.O2I(result1) - 1).ToString();
                                        if (thisval == result2.Substring(1, result2.Length - 2) && cell.ToString() == "")
                                        {
                                            isRight = false;
                                            sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据当" + result1 + "列的值等于[" + result2 + "]时为必填\r\n");
                                        }
                                    }
                                }
                                else {
                                    if (cell.CellType == type)
                                    {
                                        int test;
                                        double testd;
                                        if (typeNum == 1 && !(int.TryParse(cell.ToString(), out test) || double.TryParse(cell.ToString(), out testd)))
                                        {
                                            isRight = false;
                                            sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据类型不匹配\r\n");
                                        }
                                    }
                                    else
                                    {
                                        isRight = false;
                                        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据类型不匹配\r\n");
                                    }

                                    if (typeNum == 3)
                                    {
                                        string eeval = rules.templet.cols[j + 1].eeval;
                                        if (!eeval.Contains(cell.ToString()))
                                        {
                                            isRight = false;
                                            sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据未从指定值选择\r\n");
                                        }
                                    }
                                    var eccother = ecclist.Where(a => !a.StartsWith("R")).ToArray();
                                    foreach(var ecc in eccother){
                                        if (ecc == "city")
                                        {
                                            if (cell.ToString() != cityName)
                                            {
                                                isRight = false;
                                                sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据应填写[" + cityName + "]\r\n");
                                            }
                                        }
                                        if (ecc.StartsWith("^"))
                                        {
                                            Regex reg = new Regex(@"(\d+)");
                                            var match = reg.Match(ecc);
                                            string result = match.Groups[0].ToString();
                                            if (!cell.ToString().StartsWith(row.GetCell(O2.O2I(result) - 1).ToString()))
                                            {
                                                isRight = false;
                                                sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据应以第" + result + "列数据开头\r\n");
                                            }
                                        }
                                        int testI = 0;
                                        if (ecc.StartsWith("D"))
                                        {
                                            if (!int.TryParse(cell.ToString(), out testI))
                                            {
                                                isRight = false;
                                                sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据应填入整数\r\n");
                                            }
                                        }
                                        double testF = 0;
                                        if (ecc.StartsWith("F"))
                                        {
                                            Regex reg = new Regex(@"(\d+)");
                                            var match = reg.Match(ecc);
                                            string result = match.Groups[0].ToString();
                                            int num = cell.ToString().Length - cell.ToString().IndexOf('.') - 1;
                                            if (num > O2.O2I(result) || !double.TryParse(cell.ToString(), out testF))
                                            {
                                                isRight = false;
                                                sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据应填入小数且小数点后是" + result + "位\r\n");
                                            }
                                        }
                                    }
                                }

                                
                                //if (!notNull && cell != null && cell.ToString() != "")
                                //{
                                //    if (cell.CellType != type)
                                //    {
                                //        isRight = false;
                                //        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据类型不匹配\r\n");
                                //    }
                                //}
                                //else if (notNull)
                                //{
                                //    if (cell.ToString() == "")
                                //    {
                                //        isRight = false;
                                //        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据不能为空\r\n");
                                //        continue;
                                //    }
                                //    if (cell.CellType == type)
                                //    {
                                //        int test;
                                //        double testd;
                                //        if (typeNum == 1 && !(int.TryParse(cell.ToString(), out test) || double.TryParse(cell.ToString(), out testd)))
                                //        {
                                //            isRight = false;
                                //            sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据类型不匹配\r\n");
                                //        }
                                //    }
                                //    else
                                //    {
                                //        isRight = false;
                                //        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据类型不匹配\r\n");
                                //    }

                                //    if (typeNum == 3)
                                //    {
                                //        string eeval = rules.templet.cols[j + 1].eeval;
                                //        if (!eeval.Contains(cell.ToString()))
                                //        {
                                //            isRight = false;
                                //            sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据未从指定值选择\r\n");
                                //        }
                                //    }
                                //}
                                //var ecc = rules.templet.cols[j + 1].ecc.ToString();
                                //if (ecc == null || ecc == "")
                                //{
                                //    continue;
                                //}
                                //if (ecc == "city")
                                //{
                                //    if (cell.ToString() != cityName)
                                //    {
                                //        isRight = false;
                                //        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据应填写[" + cityName + "]\r\n");
                                //    }
                                //}
                                //if (ecc.StartsWith("^"))
                                //{
                                //    Regex reg = new Regex(@"(\d+)");
                                //    var match = reg.Match(ecc);
                                //    string result = match.Groups[0].ToString();
                                //    if (!cell.ToString().StartsWith(row.GetCell(O2.O2I(result) - 1).ToString()))
                                //    {
                                //        isRight = false;
                                //        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据应以第" + result + "列数据开头\r\n");
                                //    }
                                //}
                                //int testI = 0;
                                //if (ecc.StartsWith("D"))
                                //{
                                //    if (!int.TryParse(cell.ToString(), out testI))
                                //    {
                                //        isRight = false;
                                //        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据应填入整数\r\n");
                                //    }
                                //}
                                //double testF = 0;
                                //if (ecc.StartsWith("F"))
                                //{
                                //    Regex reg = new Regex(@"(\d+)");
                                //    var match = reg.Match(ecc);
                                //    string result = match.Groups[0].ToString();
                                //    int num = cell.ToString().Length - cell.ToString().IndexOf('.') - 1;
                                //    if (num > O2.O2I(result) || !double.TryParse(cell.ToString(), out testF))
                                //    {
                                //        isRight = false;
                                //        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据应填入小数且小数点后是" + result + "位\r\n");
                                //    }
                                //}

                                //if (ecc.StartsWith("R"))
                                //{
                                //    Regex reg = new Regex(@"(\d+)");
                                //    var match = reg.Match(ecc);
                                //    string result1 = match.Groups[0].ToString();
                                //    reg = new Regex(@"\{(\S+?)\}");
                                //    match = reg.Match(ecc);
                                //    string result2 = match.Groups[0].ToString();
                                //    string thisval = row.GetCell(O2.O2I(result1) - 1).ToString();
                                //    if (thisval == result2.Substring(1, result2.Length - 2) && cell.ToString() == "")
                                //    {
                                //        isRight = false;
                                //        sb.Append("第" + (i + 1) + "行[" + classname + "--" + name + "--]字段数据当" + result1 + "列的值等于[" + result2 + "]时为必填\r\n");
                                //    }
                                //}
                            }
                        }
                    }
                }
                string errname = "planup\\error\\" + epid + "\\" + whid;
                string errpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, errname);
                var status = DB.QueryOne("select status from e_plan_upload where whid = " + whid + " and epid = " + epid);
                if (status == null)
                {
                    var sql = "insert into e_plan_upload (epid, status, uploadtime, filename, upuid, whid) values (" + epid + ",0,getdate(),'" + fileName + "'," + userid + ", " + whid + ")";
                    DB.Insert(sql);
                }
                if (!isRight)
                {
                    if (!Directory.Exists(errpath))
                    {
                        Directory.CreateDirectory(errpath);
                    }
                    File.WriteAllText(errpath + "\\error.txt", sb.ToString(), Encoding.UTF8);
                    
                    DB.Exec("update e_plan_upload set status = -1 where whid = " + whid + " and epid = " + epid);
                }
                else
                {
                    DB.Exec("update e_plan_upload set status = 1 where whid = " + whid + " and epid = " + epid);
                    if (Directory.Exists(errpath))
                    {
                        if (File.Exists(errpath + "\\error.txt"))
                        {
                            File.Delete(errpath + "\\error.txt");
                        }
                    }
                    Compute(fileName, whid, epid, rules, hasClass, hasComments);
                }
        }
            catch (Exception e)
            {

            }
}

        public static void Compute(string fileName,int whid, int epid, etempletfactory rules, bool hasClass, bool hasComments)
        {
            try
            {
                var mdfile = System.AppDomain.CurrentDomain.BaseDirectory + rules.templet.filename;
                IWorkbook workbook = null;
                IWorkbook workbook2 = null;
                FileStream fileStream = new FileStream(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName), FileMode.Open, FileAccess.ReadWrite);
                //FileStream fileStream = File.OpenWrite(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName));
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本  
                {
                    workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook  
                    workbook2 = new XSSFWorkbook(mdfile);
                }
                else if (fileName.IndexOf(".xls") > 0) // 2003版本  
                {
                    workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook  
                    workbook2 = new XSSFWorkbook(mdfile);
                }
                
                ISheet sheet = workbook.GetSheetAt(0);
                var sheet2 = workbook2.GetSheetAt(0);
                sheet.ForceFormulaRecalculation = true;
                sheet2.ForceFormulaRecalculation = true;
                IRow row,row2;
                int start = 3;
                var cstext = workbook2.CreateCellStyle();
                var f3 = workbook2.CreateFont();
                f3.FontHeightInPoints = 10;
                f3.FontName = "宋体";
                cstext.SetFont(f3);

                var csdate = workbook2.CreateCellStyle();
                csdate.SetFont(f3);
                var df = workbook2.CreateDataFormat();
                csdate.DataFormat = df.GetFormat("yyyy/MM/dd");

                for (int i = start; i < sheet.LastRowNum + 1; i++)  //对工作表每一行  
                {
                    row = sheet.GetRow(i);   //row读入第i行数据  
                    row2 = sheet2.CreateRow(i);
                    int last = hasClass ? sheet.GetRow(1).Cells.Count : sheet.GetRow(0).Cells.Count;
                    if (row != null)
                    {
                        for (int j = 0; j < last; j++)
                        {
                            string expr = rules.templet.cols[j + 1].expr;
                            if (expr == null || expr == "")
                            {
                                var cell2 = row2.CreateCell(j);
                                cell2.CellStyle = cstext;
                                ICell cell = row.GetCell(j);
                                if (cell == null)
                                {
                                    cell = row.CreateCell(j);
                                }
                                switch (rules.templet.cols[j + 1].etctype.Value) { 
                                    case e_etctype.dropdown:
                                    case e_etctype.text:
                                        cell2.SetCellValue(cell.StringCellValue);
                                        break;
                                    case e_etctype.numner:
                                        var ctstr = cell.ToString();
                                        if (ctstr == "") {
                                            cell2.SetCellValue("");
                                        } else {
                                            cell2.SetCellValue(cell.NumericCellValue);
                                        }
                                        break;
                                    case e_etctype.time:
                                        var ncv = cell.NumericCellValue;
                                        if (ncv <= 0) {
                                            cell2.SetCellValue("");
                                        } else {
                                            cell2.SetCellValue(ncv);
                                        }
                                        cell2.CellStyle = csdate;
                                        break;
                                }
                                continue;
                            }
                            else
                            {
                                var cell2 = row2.CreateCell(j);
                                cell2.CellStyle = cstext;
                                string formula = getLetters(expr.Substring(1), i + 1);
                                ICell cell = row.GetCell(j);
                                if (cell == null)
                                {
                                    cell = row.CreateCell(j);
                                }
                                cell.SetCellType(CellType.Formula);
                                cell.CellFormula = formula;
                                //cell.SetCellValue(expr.Substring(1));

                                cell2.SetCellType(CellType.Formula);
                                cell2.CellFormula = formula;
                            }
                        }
                    }
                }
                

                string name = fileName.Split('/')[fileName.Split('/').Length - 1];
                string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\target\\" + epid + "\\" + whid + "\\");
                if (!Directory.Exists(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\target\\" + epid + "\\" + whid)))
                {
                    DB.Exec("update e_plan set uploadnum = uploadnum + 1 where id = " + epid);
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
                FileStream fs2 = File.Create(path + name);
                workbook2.Write(fs2);
                fs2.Close();
                //workbook.Close();
                //workbook2.Close();
                string target = "/planup/target/" + epid + "/" + whid +"/"+ name;
                
                
                DB.Exec("update e_plan_upload set status = 2, filename = '"+ target +"' where whid = " + whid + " and epid = " + epid);
                
                //var nums = DB.QueryAsDics("select uploadnum, plannum FROM e_plan where id = "+ epid);
                //if(nums[0]["uploadnum"].ToString() == nums[0]["plannum"].ToString())
                //{
                //    DB.Exec("update e_plan set status = 1, completetime = getdate() where id = " + epid);
                //}
                File.Delete(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName));
                string errpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\error\\" + epid + "\\" + whid);
                if(Directory.Exists(errpath))
                {
                    Directory.Delete(errpath, true);
                }
                Combine(path + name,whid, epid, hasClass,hasComments, rules);
            }
            catch (Exception e)
            {
                DB.Exec("update e_plan_upload set status = -2 where whid = " + whid + " and epid = " + epid);
            }
        }

        public static void Combine(string fileName,int whid, int epid, bool hasClass, bool hasComments, etempletfactory rules)
        {
            try
            {
                int etid = rules.templet.id.Value;
                IWorkbook workbook = null;
                IWorkbook workbook2 = null;

                string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\combine\\" + epid + "\\");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName2 = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, path + rules.templet.name + ".xlsx");
                if (!File.Exists(fileName2))
                {
                    string source = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "plantemp\\" + etid + "\\" + rules.templet.name + ".xlsx");
                    File.Copy(source, fileName2);
                }
                else
                {
                    File.Delete(fileName2);
                    string source = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "plantemp\\" + etid + "\\" + rules.templet.name + ".xlsx");
                    File.Copy(source, fileName2);
                }
                ISheet sheet2;
                IRow row2;
                FileStream fs = new FileStream(fileName2, FileMode.Open, FileAccess.ReadWrite);
                workbook2 = new XSSFWorkbook(fs);

                int start = 3;
                sheet2 = workbook2.GetSheetAt(0);
                string rootPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "planup\\target\\" + epid);
                string[] paths = Directory.GetDirectories(rootPath);
                var cstext = workbook2.CreateCellStyle();
                var f3 = workbook2.CreateFont();
                f3.FontHeightInPoints = 10;
                f3.FontName = "宋体";
                cstext.SetFont(f3);

                var csdate = workbook2.CreateCellStyle();
                csdate.SetFont(f3);
                var df = workbook2.CreateDataFormat();
                csdate.DataFormat = df.GetFormat("yyyy/MM/dd");

                for (int a=0;a<paths.Length;a++) {
                    string[] files = Directory.GetFiles(paths[a]);
                    for (int b=0;b<files.Length;b++) {
                        FileStream fileStream = new FileStream(files[b], FileMode.Open, FileAccess.Read);
                        if (fileName.IndexOf(".xlsx") > 0) // 2007版本  
                        {
                            workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook  
                        }
                        else if (fileName.IndexOf(".xls") > 0) // 2003版本  
                        {
                            workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook  
                        }
                        ISheet sheet = workbook.GetSheetAt(0);
                        IRow row;
                        XSSFFormulaEvaluator evalor = new XSSFFormulaEvaluator(workbook);
                        for (int i = start; i < sheet.LastRowNum + 1; i++)  //对工作表每一行  
                        {
                            row = sheet.GetRow(i);   //row读入第i行数据 
                            row2 = sheet2.CreateRow(sheet2.LastRowNum + 1);
                            int last = hasClass ? sheet.GetRow(1).Cells.Count : sheet.GetRow(0).Cells.Count;
                            if (row != null)
                            {
                                for (int j = 0; j < last; j++)
                                {
                                    ICell cell = row.GetCell(j);
                                    var t = rules.templet.cols[j + 1].etctype;
                                    if (cell == null)
                                    {
                                        continue;
                                    }
                                    ICell cell2 = row2.CreateCell(j);
                                    cell2.CellStyle = cstext;
                                    if (cell.CellType == CellType.Formula) {
                                    //cell2.SetCellFormula(evalor.EvaluateFormulaCell(cell).ToString());
                                    
                                    var fval = evalor.Evaluate(cell);
                                    switch (fval.CellType) {
                                        case CellType.Numeric:
                                            cell2.SetCellValue(fval.NumberValue);
                                            break;
                                    }
                                    }else
                                    {
                                    //cell2.SetCellValue(cell.ToString());
                                    switch (cell.CellType)
                                    {
                                        case CellType.Numeric:
                                            if (t == e_etctype.time)
                                            {
                                                cell2.SetCellValue(cell.NumericCellValue);
                                                cell2.CellStyle = csdate;
                                            }
                                            else
                                            {
                                                cell2.SetCellValue(cell.NumericCellValue);
                                            }
                                            
                                            
                                            break;
                                        case CellType.String:
                                            cell2.SetCellValue(cell.StringCellValue);
                                            break;
                                        
                                    }
                                }
                                }
                            }
                        }
                    }
                }
                DB.Exec("update e_plan_upload set status = 3 where whid = " + whid + " and epid = " + epid);
                FileStream fs2 = File.Create(fileName2);
                workbook2.Write(fs2);
                fs2.Close();
            }
            catch (Exception e)
            {
                DB.Exec("update e_plan_upload set status = -3 where whid = " + whid + " and epid = " + epid);
            }
        }

        private static string getLetters(string expr, int row)
        {
            Regex reg = new Regex(@"(\d+)");
            var matches = reg.Matches(expr);
            string result = "";
            foreach (var m in matches)
            {
                result += m + ",";
            }
            result = result.TrimEnd(',');
            string[] arr = result.Split(',');
            for(int i=0;i<arr.Length;i++)
            {
                arr[i] = getExcelColumnLabel(O2.O2I(arr[i])-1) + row;
            }
            Regex reg2 = new Regex(@"(\[\d+\])");
            for (int i=0;i<arr.Length;i++)
            {
                expr = reg2.Replace(expr, arr[i], 1);
            }
            return expr;
        }

        private static string getExcelColumnLabel(int num)
        {
            string temp = "";

            double i = Math.Floor(Math.Log(25.0 * (num) / 26.0 + 1) / Math.Log(26)) + 1;
            if (i > 1)
            {
                double sub = num - 26 * (Math.Pow(26, i - 1) - 1) / 25;
                for (double j = i; j > 0; j--)
                {
                    temp = temp + (char)(sub / Math.Pow(26, j - 1) + 65);
                    sub = sub % Math.Pow(26, j - 1);
                }
            }
            else
            {
                temp = temp + (char)(num + 65);
            }
            return temp;
        }
    }
}