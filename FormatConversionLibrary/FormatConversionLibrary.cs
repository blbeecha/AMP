/*
 * AUTHOR: Brendan Beecham
 * FILE NAME: FormatConversionLibrary.cs
 * SOLUTION: AMP (Accounts Management Portal)
 * PROGRAM SCOPE: .NET 5.0 Library
 * PROGRAM PURPOSE:
 *      The purpose of this library is to assist with exporting accounts information
 *      from the 'savedData.xml' to a Microsoft Office Excel 2016 file.
 * CALLED BY:
 *      Program.cs
 */

using System;
using System.Data;
using System.IO;

namespace FormatConversionLibrary
{
    public class FormatExcel
    {
        public FormatExcel(){}  //constructor

        public System.Data.DataTable convertXmlToEl()
        {
            //Converts XML savedData at this point to DataTable format
            string XmlFile = "savedData.xml";
            System.Data.DataTable Dt = new System.Data.DataTable();
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(XmlFile);
                Dt.Load(ds.CreateDataReader());
                Dt = AdjustDataTableColumns(Dt);    //set the order of columns to be static for spreadsheet

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Dt;
        }

        private static DataTable AdjustDataTableColumns(DataTable sourceDataTable)
        {
            //Ensures that each column is in the correct order for Excel export
            sourceDataTable.Columns["Name"].SetOrdinal(0);
            sourceDataTable.Columns["Organization"].SetOrdinal(1);
            sourceDataTable.Columns["Balance"].SetOrdinal(2);

            return sourceDataTable;
        }

        public Microsoft.Office.Interop.Excel.Workbook CreateWorkbook(string fileName, Microsoft.Office.Interop.Excel.Application excel)
        {

            //Create workbook object
                
            string currentDirectory = Directory.GetCurrentDirectory();
            string str = Path.Combine(currentDirectory, fileName);

            Microsoft.Office.Interop.Excel.Workbook workbook;
            if(File.Exists(str))
            {
                workbook = excel.Application.Workbooks.Open(Filename: str);
            }
            else
            {
                workbook = excel.Application.Workbooks.Add(Type.Missing);
            }

            excel.Visible = false;
            excel.DisplayAlerts = false;

            return workbook;

        }

        public Microsoft.Office.Interop.Excel.Workbook PopulateWorkbook(Microsoft.Office.Interop.Excel.Workbook workbook, Microsoft.Office.Interop.Excel.Worksheet worksheet, Microsoft.Office.Interop.Excel.Application excel, System.Data.DataTable dt)
        {
            

            // Column Headings
            int iColumn = 0;

            foreach (DataColumn c in dt.Columns)
            {
                iColumn++;
                excel.Cells[1, iColumn] = c.ColumnName;
            }

            // Row Data
            int iRow = worksheet.UsedRange.Rows.Count - 1;

            foreach (DataRow dr in dt.Rows)
            {
                iRow++;

                // Row's Cell Data
                iColumn = 0;
                foreach (DataColumn c in dt.Columns)
                {
                    iColumn++;
                    excel.Cells[iRow + 1, iColumn] = dr[c.ColumnName];
                }
            }

            ((Microsoft.Office.Interop.Excel.Worksheet)worksheet).Activate();

            return workbook;
        }
    }
}
