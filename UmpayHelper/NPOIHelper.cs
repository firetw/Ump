using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Umpay.Hjdl
{
    public class NPOIHelper
    {
        //static readonly NPOIHelper instance = new NPOIHelper();

        //public static NPOIHelper Instance { get { return instance; } }
        private object lockObject = new object();

        public NPOIHelper()
        {
            InitializeWorkbook();
        }


        HSSFWorkbook hssfworkbook;
        ISheet _currentSheet = null;
        int _currentMask = 0;
        void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "支付查询";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "支付查询";
            hssfworkbook.SummaryInformation = si;

            _currentSheet = hssfworkbook.CreateSheet("支付查询");
        }

        /// <summary>
        /// 获取单元格样式
        /// </summary>
        /// <param name="hssfworkbook">Excel操作类</param>
        /// <param name="font">单元格字体</param>
        /// <param name="fillForegroundColor">图案的颜色</param>
        /// <param name="fillPattern">图案样式</param>
        /// <param name="fillBackgroundColor">单元格背景</param>
        /// <param name="ha">垂直对齐方式</param>
        /// <param name="va">垂直对齐方式</param>
        /// <returns></returns>
        //public static ICellStyle GetCellStyle(HSSFWorkbook hssfworkbook, IFont font, HSSFColor fillForegroundColor, FillPatternType fillPattern, HSSFColor fillBackgroundColor, HorizontalAlignment ha, VerticalAlignment va)
        //{
        //    ICellStyle cellstyle = hssfworkbook.CreateCellStyle();
        //    cellstyle.FillPattern = fillPattern;
        //    cellstyle.Alignment = ha;
        //    cellstyle.VerticalAlignment = va;
        //    if (fillForegroundColor != null)
        //    {
        //        cellstyle.FillForegroundColor = fillForegroundColor.GetIndex();
        //    }
        //    if (fillBackgroundColor != null)
        //    {
        //        cellstyle.FillBackgroundColor = fillBackgroundColor.GetIndex();
        //    }
        //    if (font != null)
        //    {
        //        cellstyle.SetFont(font);
        //    }
        //    //有边框
        //    cellstyle.BorderBottom = CellBorderType.THIN;
        //    cellstyle.BorderLeft = CellBorderType.THIN;
        //    cellstyle.BorderRight = CellBorderType.THIN;
        //    cellstyle.BorderTop = CellBorderType.THIN;
        //    return cellstyle;
        //}

        private void GenerateHeader(ExportItem item)
        {
            IRow row = _currentSheet.CreateRow(0);


            ICellStyle cellStle = hssfworkbook.CreateCellStyle();
            HSSFFont font = (HSSFFont)hssfworkbook.CreateFont();
            //字體粗體
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            //字體尺寸
            font.FontHeightInPoints = 16;
            cellStle.SetFont(font);

            cellStle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Double;
            cellStle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
            cellStle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
            cellStle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
            cellStle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
            cellStle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;

            for (int i = 0; i < item.Columns.Count; i++)
            {
                int col = i;
                ICell cell = row.CreateCell(col);
                cell.CellStyle = cellStle;
                Column viewCol = item.Columns.ElementAt(i).Value;
                cell.SetCellValue(viewCol.LabelCn);

                _currentSheet.SetColumnWidth(col, viewCol.Width);
            }
            row.Height = 400;
            _currentMask = 1;
        }

        private void WriteRow(ExportItem item)
        {
            lock (lockObject)
            {
                if (item == null) return;

                for (int i = 0; i < item.Data.Rows.Count; i++)
                {
                    IRow row = _currentSheet.CreateRow(i + 1);
                    int col = 0;

                    foreach (var column in item.Columns)
                    {
                        for (int colIndex = 0; colIndex < item.Data.Columns.Count; colIndex++)
                        {
                            if (column.Value.LabelEn.ToUpper() == item.Data.Columns[colIndex].ColumnName.ToUpper())
                            {
                                string tmpValue = item.Data.Rows[i][colIndex] == DBNull.Value ? string.Empty : item.Data.Rows[i][colIndex].ToString();
                                ICell cell = row.CreateCell(col);
                                cell.SetCellType((CellType)column.Value.CellType);
                                cell.SetCellValue(tmpValue);
                                break;
                            }
                        }
                        col++;
                    }
                }
            }
        }

        public string Export(ExportItem item)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/export");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string shorName = string.Format("查询结果{0}.xls", DateTime.Now.ToString("MMdd"));
            string fileName = dir + "/" + shorName;
            GenerateHeader(item);
            WriteRow(item);

            WriteToFile(fileName);
            return shorName;


        }

        private void WriteToFile(string fileName)
        {

            if (File.Exists(fileName))
                File.Delete(fileName);
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                hssfworkbook.Write(file);
            }
        }




    }
}
