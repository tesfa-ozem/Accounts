using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Accounts.Repo
{
    public static class HttpContentExtension
    {
        public static void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        public static async Task<DataTable> ParseMultipartAsync(this HttpContent postedContent)
        {

            ExcelHandler excelHandler = new ExcelHandler();

            var provider = await postedContent.ReadAsMultipartAsync();
            string worksheetName = "Sheet1";
            string cellName = "B2";
            var files = new Dictionary<string, HttpPostedFile>(StringComparer.InvariantCultureIgnoreCase);
            var fields = new Dictionary<string, HttpPostedField>(StringComparer.InvariantCultureIgnoreCase);
            var ExcelData = new Dictionary<string, string>();
            DataTable table = new DataTable();
            foreach (var content in provider.Contents)
            {
                var fieldName = content.Headers.ContentDisposition.Name.Trim('"');
                if (!string.IsNullOrEmpty(content.Headers.ContentDisposition.FileName))
                {
                    var file = await content.ReadAsByteArrayAsync();
                    //MatchRes = FingerprintMatcher.MatchPrintInnovatrics(file);
                    var fileName = content.Headers.ContentDisposition.FileName.Trim('"');
                    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Xlxs_Files");
                    CreateFolder(folderPath);

                    using (FileStream fileStream = new FileStream(Path.Combine(folderPath, fileName), FileMode.OpenOrCreate))
                    {
                        fileStream.Write(file, 0, file.Length);
                        fileStream.Close();

                    }

                    files.Add(fieldName, new HttpPostedFile(fieldName, fileName, file));
                    if (fileName.EndsWith(".xlsx") || (fileName.EndsWith(".xls")))
                    {

                        excelHandler.Extention = "true";
                        //ExcelData = ExcelHandler.GetDefinedNames(Path.Combine(folderPath, fileName));
                        table = excelHandler.ImportExcel(Path.Combine(folderPath, fileName));
                        //excelHandler.CreateSpreadsheetWorkbook(Path.Combine(folderPath, fileName));
                    }

                    else
                    {
                        var data = await content.ReadAsStringAsync();
                        fields.Add(fieldName, new HttpPostedField(fieldName, data));
                    }
                }

                return table;
            }
            return table;
        }
    }
}
