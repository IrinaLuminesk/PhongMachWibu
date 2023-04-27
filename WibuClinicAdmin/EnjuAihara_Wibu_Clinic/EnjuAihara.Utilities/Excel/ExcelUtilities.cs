
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Web;

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

        
        public static List<T> ImportExcel<T>(HttpPostedFileBase file, int Endcolumn, int Startrow, int Startcolumn)
        {
            try
            {
                using (var package = new ExcelPackage(file.InputStream))
                {
                    DataTable table = new DataTable();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    PropertyInfo[] properties = typeof(T).GetProperties();
                    foreach (var j in properties)
                    {
                        table.Columns.Add(j.Name, j.PropertyType);
                    }

                    for (int row = Startrow; worksheet.Cells[row, Startcolumn].Value != null; row++)
                    {
                        var dtrow = table.NewRow();
                        int start = 0;
                        for (int i = Startcolumn; i <= Endcolumn; i++)
                        {
                            dtrow[start] = worksheet.Cells[row, i].Value;
                            start++;
                        }
                        table.Rows.Add(dtrow);
                    }
                    List<T> list = ConvertDataTableToList<T>(table);
                    if(list == null || list.Count == 0)
                        return new List<T>();
                    return list;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message.ToString();
                return new List<T>();
            }
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


        private static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
