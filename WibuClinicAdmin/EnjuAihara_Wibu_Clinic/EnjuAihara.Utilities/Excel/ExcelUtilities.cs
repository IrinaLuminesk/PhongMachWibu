
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace EnjuAihara.Utilities.Excel
{
    public static class ExcelUtilities
    {
        public static byte[] ExportExcel<T>(string ReportName, List<string> header, List<T> Item)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(ReportName);

            ws.Cells["A1"].Value = ReportName;

            ws.Cells["A2"].Value = "Thời gian tạo:";
            ws.Cells["B2"].Value = string.Format("{0:dd/MM/yyyy} vào lúc {0:HH:mm}", DateTime.Now);

            int column = 3;
            foreach (var i in header)
            {
                ws.Cells[6, column].Value = i;
                column++;
            }

            int RowStart = 7;
            DataTable data = ConvertListToDatatable<T>(Item);

            for (int i = 0; i < data.Rows.Count; i++)
            {
                column = 3;
                ws.Cells[RowStart, column].Value = i + 1;
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    ws.Cells[RowStart, column + 1].Value = data.Rows[i][j];
                    column++;
                }
                RowStart++;
            }



            ws.Cells[1, 1, 1, data.Columns.Count].AutoFitColumns();

            return pck.GetAsByteArray();
        }


        public static DataTable ConvertListToDatatable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
