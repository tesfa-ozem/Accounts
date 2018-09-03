using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Repo
{
    public class ExcelHandler
    {
        public string Extention { get; set; }
        public String FileHandler()
        {

            return Extention;
        }
        public Dictionary<String, String> ReadExcelFileSAX(string fileName)
        {
            var currentcellvalue = new Dictionary<string, string>();
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                //create the object for workbook part  
                WorkbookPart wbPart = spreadsheetDocument.WorkbookPart;

                //statement to get the count of the worksheet  
                int worksheetcount = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Count();

                //statement to get the sheet object  
                Sheet mysheet = (Sheet)spreadsheetDocument.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(0);

                //statement to get the worksheet object by using the sheet id  
                Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;

                //Note: worksheet has 8 children and the first child[1] = sheetviewdimension,....child[4]=sheetdata  
                int wkschildno = 4;
                SheetData Rows = (SheetData)Worksheet.ChildElements.GetItem(wkschildno);
                Row currentrow = (Row)Rows.ChildElements.GetItem(0);

                //getting the cell as per the specified index of getitem method  
                Cell currentcell = (Cell)currentrow.ChildElements.GetItem(0);



                if (currentcell.DataType != null)
                {
                    if (currentcell.DataType == CellValues.SharedString)
                    {
                        int id = 3;

                        if (Int32.TryParse(currentcell.InnerText, out id))
                        {
                            SharedStringItem item = GetSharedStringItemById(wbPart, id);

                            if (item.Text != null)
                            {
                                //code to take the string value  
                                currentcellvalue.Add(item.Text.Text, "");
                            }
                            else if (item.InnerText != null)
                            {
                                currentcellvalue.Add(item.InnerText, "");
                            }
                            else if (item.InnerXml != null)
                            {
                                currentcellvalue.Add(item.InnerXml, "");
                            }
                        }
                    }
                }

            }
            return currentcellvalue;
        }
        public static SharedStringItem GetSharedStringItemById(WorkbookPart workbookPart, int id)
        {
            return workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
        }
        public static Dictionary<String, String>
        GetDefinedNames(String fileName)
        {
            // Given a workbook name, return a dictionary of defined names.
            // The pairs include the range name and a string representing the range.
            var returnValue = new Dictionary<String, String>();

            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                // Retrieve a reference to the workbook part.
                var wbPart = document.WorkbookPart;

                // Retrieve a reference to the defined names collection.
                DefinedNames definedNames = wbPart.Workbook.DefinedNames;

                // If there are defined names, add them to the dictionary.
                if (definedNames != null)
                {
                    foreach (DefinedName dn in definedNames)
                        returnValue.Add(dn.Name.Value, dn.Text);
                }
            }
            return returnValue;
        }
        public void CreateSpreadsheetWorkbook(string filepath)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }
        public DataTable ImportExcel(string filepath)
        {
            DataTable dt = new DataTable();
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filepath, false))
            {
                //Read the first Sheet from Excel file.
                Sheet sheet = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(0);
                //Get the Worksheet instance.
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                //Fetch all the rows present in the Worksheet.
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                //Loop through the Worksheet rows.
                foreach (Row row in rows)
                {
                    //Use the first row to add columns to DataTable.
                    if (row.RowIndex.Value == 1)
                    {
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            dt.Columns.Add(GetValue(doc, cell));
                        }
                    }
                    else
                    {
                        //Add rows to DataTable.
                        dt.Rows.Add();
                        int i = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = GetValue(doc, cell);
                            i++;
                        }
                    }
                }
            }
            Table table = new Table(dt);
            return dt;
        }
        private string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = string.Empty;
            if (cell.CellValue != null)
            {
                value = cell.CellValue.InnerText;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                }

            }
            return value;
        }
        private string CheckTime(string date)
        {

            DateTime conv = DateTime.ParseExact(date, "ddMMyyyy",
                                  CultureInfo.InvariantCulture);

            return conv.ToString("yyyyMMdd");
        }
    }
}
