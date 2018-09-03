using Accounts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.Table;
using System.IO;
using System.Data;
using Microsoft.AspNetCore.Hosting;

namespace Accounts.Repo
{
    public class Logic
    {
        
        public async Task<string> SendSMSAsync(Agents agents)
        {
            try
            {
                UHCContext context = new UHCContext();
                var phone = context.Agents
                                .Where(b => b.PhoneNumber == agents.PhoneNumber)
                                .FirstOrDefault();
              

                Sms sms = new Sms();

                Random random = new Random();
                int pin = random.Next(1000, 9999);
                string msg = string.Format("Dear {0} your KChic login credentials are : username : {1}, password : {2}", agents.FirstName, agents.UserName, pin);
                sms.SendSms(agents.PhoneNumber, msg);
                agents.Password = Security.EncryptString(pin.ToString(), Security.pPhrase);

                context.Add(agents);
                await context.SaveChangesAsync();

                return "";
            }
            catch (Exception)
            {

                
            }
            return "";
        }

        public string readExcelPackage(FileInfo fileInfo, string worksheetName)
        {
            using (var package = new ExcelPackage(fileInfo))
            {
                return readExcelPackageToString(package, package.Workbook.Worksheets[worksheetName]);
            }
        }

        public string readExcelPackageToString(ExcelPackage package, ExcelWorksheet worksheet)
        {
            var rowCount = worksheet.Dimension?.Rows;
            var colCount = worksheet.Dimension?.Columns;

            if (!rowCount.HasValue || !colCount.HasValue)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (int row = 1; row <= rowCount.Value; row++)
            {
                for (int col = 1; col <= colCount.Value; col++)
                {
                    sb.AppendFormat("{0}\t", worksheet.Cells[row, col].Value);
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

    }
}
