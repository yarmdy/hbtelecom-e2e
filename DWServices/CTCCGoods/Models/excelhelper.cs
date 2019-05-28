using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.OpenXml4Net.OPC;
using System.Text.RegularExpressions;

namespace CTCCGoods.Controllers
{
    public class excelhelper
    {
        public static bool CreateTemplet(e_templet temp,string path){
            var wb = new XSSFWorkbook();
            var sh = (XSSFSheet)wb.CreateSheet(temp.name);
            var tt = new IRow[] { sh.CreateRow(0), sh.CreateRow(1), sh.CreateRow(2) };
            
            var cstext = wb.CreateCellStyle();
            cstext.DataFormat = NPOI.HSSF.UserModel.HSSFDataFormat.GetBuiltinFormat("@");
            var f3 = wb.CreateFont();
            f3.FontHeightInPoints = 10;
            f3.FontName = "宋体";
            cstext.SetFont(f3);

            var tt1style = wb.CreateCellStyle();
            tt1style.Alignment = HorizontalAlignment.Center;
            tt1style.VerticalAlignment = VerticalAlignment.Center;
            var f1 = wb.CreateFont();
            f1.Boldweight = (short)FontBoldWeight.Bold;
            f1.FontHeightInPoints = 10;
            f1.FontName = "宋体";
            tt1style.SetFont(f1);
            tt1style.FillForegroundColor = 42;
            tt1style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            tt1style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            tt1style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            tt1style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            tt1style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            tt1style.LeftBorderColor = 53;
            tt1style.RightBorderColor = 53;
            tt1style.BottomBorderColor = 53;
            tt1style.TopBorderColor = 53;
            tt1style.WrapText = true;

            var tt2style = wb.CreateCellStyle();
            tt2style.Alignment = HorizontalAlignment.Center;
            tt2style.VerticalAlignment = VerticalAlignment.Center;
            var f2 = wb.CreateFont();
            f2.Boldweight = (short)FontBoldWeight.Bold;
            f2.FontHeightInPoints = 10;
            f2.Color = 10;
            f2.FontName = "宋体";
            tt2style.SetFont(f2);
            tt2style.FillForegroundColor = 42;
            tt2style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            tt2style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            tt2style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            tt2style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            tt2style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            tt2style.LeftBorderColor = 53;
            tt2style.RightBorderColor = 53;
            tt2style.BottomBorderColor = 53;
            tt2style.TopBorderColor = 53;
            tt2style.WrapText = true;

            var tt3style = wb.CreateCellStyle();
            tt3style.Alignment = HorizontalAlignment.Center;
            tt3style.VerticalAlignment = VerticalAlignment.Center;
            var f4 = wb.CreateFont();
            f4.Boldweight = (short)FontBoldWeight.Normal;
            f4.FontHeightInPoints = 10;
            f4.FontName = "宋体";
            f4.Color = 48;
            tt3style.SetFont(f4);
            tt3style.FillForegroundColor = 42;
            tt3style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            tt3style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            tt3style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            tt3style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            tt3style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            tt3style.LeftBorderColor = 53;
            tt3style.RightBorderColor = 53;
            tt3style.BottomBorderColor = 53;
            tt3style.TopBorderColor = 53;
            tt3style.WrapText = true;

            var cellreadonly = wb.CreateCellStyle();
            cellreadonly.FillForegroundColor = 55;
            cellreadonly.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            cellreadonly.SetFont(f3);

            var cellnormal = wb.CreateCellStyle();
            cellnormal.SetFont(f3);

            tt[0].Height = 790;
            tt[1].Height = 825;

            string lastt = null;
            int lasti = 0;
            var colsc=temp.cols.Count;
            foreach (var col in temp.cols) {
                if (!string.IsNullOrWhiteSpace(col.Value.expr))
                {
                    sh.SetDefaultColumnStyle(col.Key - 1, cellreadonly);
                }else if (col.Value.etctype.Value == e_etctype.dropdown || col.Value.etctype.Value == e_etctype.text)
                {
                    sh.SetDefaultColumnStyle(col.Key - 1, cstext);
                }
                else {
                    sh.SetDefaultColumnStyle(col.Key - 1, cellnormal);
                }
                var ccname = tt[0].CreateCell(col.Key - 1);
                ccname.SetCellValue(col.Value.classname);
                ccname.CellStyle = tt1style;
                var cname = tt[1].CreateCell(col.Key - 1);
                cname.CellStyle = col.Value.notnull.Value ? tt2style : tt1style; ;
                cname.SetCellValue(col.Value.name);
                if (col.Value.etctype.Value == e_etctype.dropdown) {
                    var rang = new NPOI.SS.Util.CellRangeAddressList(3, 65535, col.Key - 1, col.Key - 1);
                    XSSFDataValidationHelper val = new XSSFDataValidationHelper(sh);
                    var valr = val.CreateValidation(val.CreateExplicitListConstraint(col.Value.eeval.Split(new char[]{'，',','})), rang);
                    valr.CreateErrorBox("错误", "输入错误，请从下拉列表中选择");
                    valr.ShowErrorBox = true;
                    sh.AddValidationData(valr);
                }
                
                if (lastt != null && lastt!=col.Value.classname ) {
                    sh.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, lasti, col.Key-2));

                    lasti = col.Key - 1;
                }
                else if (lastt != null && col.Key == colsc && lasti < col.Key - 1) {
                    sh.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, lasti, col.Key - 1));
                }
                lastt = col.Value.classname;
            }
            IRow nrow = sh.CreateRow(2);
            nrow.Height = 825;
            for (int i = 0; i < temp.cols.Count; i++)
            {
                ICell cell = nrow.CreateCell(i);
                cell.CellStyle = tt3style;
                cell.SetCellType(CellType.String);
                if (string.IsNullOrEmpty(temp.cols[i + 1].comments))
                {
                    if (!string.IsNullOrEmpty(temp.cols[i + 1].expr))
                    {
                        cell.SetCellValue("勿填");
                    }
                    else if (!string.IsNullOrEmpty(temp.cols[i + 1].ecc)) {
                        string eccstr = temp.cols[i + 1].ecc;
                        var ecclist = eccstr.Split(new string[]{"&&"},StringSplitOptions.RemoveEmptyEntries);
                        var eccts = "";
                        foreach (var ecc in ecclist) {
                            if (ecc.StartsWith("F"))
                            {
                                Regex reg = new Regex(@"(\d+)");
                                var match = reg.Match(ecc);
                                string result = match.Groups[0].ToString();
                                eccts += "小数点" + result + "位;";
                            }
                            else if (ecc.StartsWith("D"))
                            {
                                eccts += "整数;";
                            }
                            else if (ecc.StartsWith("city"))
                            {
                                eccts += "本地市;";
                            }
                            else if (ecc.StartsWith("^"))
                            {
                                Regex reg = new Regex(@"(\d+)");
                                var match = reg.Match(ecc);
                                string result = match.Groups[0].ToString();
                                eccts += "以" + temp.cols[O2.O2I(result)].name + "列开头;";
                            }
                            else if (ecc.StartsWith("R"))
                            {
                                Regex reg = new Regex(@"(\d+)");
                                var match = reg.Match(ecc);
                                string result1 = match.Groups[0].ToString();
                                reg = new Regex(@"\{(\S+?)\}");
                                match = reg.Match(ecc);
                                string result2 = match.Groups[0].ToString();
                                eccts += temp.cols[O2.O2I(result1)].name + "列为" + result2 + "必填;";
                            }
                        }
                        cell.SetCellValue(eccts.Substring(0,eccts.Length-1));
                    } else if (temp.cols[i + 1].etctype == e_etctype.dropdown) {
                        cell.SetCellValue("下拉");
                    } else if (temp.cols[i + 1].etctype == e_etctype.numner) {
                        cell.SetCellValue("数字");
                    } else if (temp.cols[i + 1].etctype == e_etctype.text) {
                        cell.SetCellValue("文本");
                    }
                    else if (temp.cols[i + 1].etctype == e_etctype.time)
                    {
                        cell.SetCellValue("日期");
                    }
                    
                } 
                else
                {
                    cell.SetCellValue(temp.cols[i + 1].comments);
                }
                
            }
            if (!System.IO.Directory.Exists(path.Substring(0, path.LastIndexOf('\\')))) {
                System.IO.Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
            }
            wb.Write(new System.IO.FileStream(path,System.IO.FileMode.Create));
            return false;
        }
    }
}