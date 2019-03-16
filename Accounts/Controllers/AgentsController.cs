using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices.ComTypes;
using Accounts.Models;
using Accounts.Repo;
using System.Data;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Accounts.ViewModels;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Accounts.Controllers
{
    public class AgentsController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        public AgentsController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        Logic logic = new Logic();
        public IActionResult Index()
        {
            List<AgentsViewModel> agents = logic.GetAllAgents();

            return View("Index", agents);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            Logic logic = new Logic();
            List<AgentsViewModel> agents = logic.GetAllAgents();
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("Index");
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream).ConfigureAwait(false);

                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets["Sheet1"];

                    logic.readExcelPackageToString(package, worksheet);
                    // Tip: To access the first worksheet, try index 1, not 0

                }
            }

            return RedirectToAction(actionName: nameof(Index));


        }
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        public IActionResult RegisterSingleUser([Bind("FirstName,LastName,SUBCOUNTY,WARD,VILLAGE,UserName,PhoneNumber,IdNumber")] AgentsViewModel appUsers)
        {


            if (ModelState.IsValid)
            {
                logic.AddNewMember(appUsers);
            }
            return RedirectToAction(actionName: nameof(Index));
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult DeleteAgent([FromRoute]string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            logic.DeletUser(id);

            return RedirectToAction(actionName: nameof(Index));
        }
        public IActionResult Login()
        {


            return View("Login");
        }
        public IActionResult ValidateLogin(Login login)
        {
            if (login.UserName == "Admin" & login.Password == "kitui@254")
            {
                return RedirectToAction(actionName: nameof(Index));
            }

            return View("Login");
        }
        [HttpPost]
        public async Task<IActionResult> UploadBulkFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction(actionName: nameof(BulkPayment));
            }
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream).ConfigureAwait(false);

                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets["Sheet1"];

                    logic.readExcelPackageToStringBulk(package, worksheet);
                    // Tip: To access the first worksheet, try index 1, not 0

                }
            }

            return RedirectToAction(actionName: nameof(BulkPayment));
        }

        public IActionResult BulkPayment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExportExcel(List<AgentsViewModel> agent)
        {
            List<AgentsViewModel> agents = logic.GetAllAgents();
            List<AgentsViewModel> Checked = new List<AgentsViewModel>();
            foreach (AgentsViewModel checkedAgent in agents)
            {
                Checked.Add(checkedAgent.Selected == true ? checkedAgent : null);
                ExportExcel_ClickAsync(agents);
            }
            return RedirectToAction(actionName: nameof(Index));
        }

        protected async Task<IActionResult> ExportExcel_ClickAsync(List<AgentsViewModel> Agents)
        {
            try
            {
                var currentRow = 1;
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                string sFileName = @"demo.xlsx";
                string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Demo");
                    IRow row = excelSheet.CreateRow(0);

                    row.CreateCell(0).SetCellValue("Name");
                    row.CreateCell(1).SetCellValue("Id Number");
                    row.CreateCell(2).SetCellValue("Phone Number");
                    row.CreateCell(3).SetCellValue("PassWord");


                    

                    foreach (var user in Agents)
                    {
                        
                    row = excelSheet.CreateRow(currentRow++);
                    row.CreateCell(0).SetCellValue(user.FirstName);
                    row.CreateCell(1).SetCellValue(user.IdNumber);
                    row.CreateCell(2).SetCellValue(user.PhoneNumber);
                    row.CreateCell(3).SetCellValue(Security.DecryptString(user.Password,Security.pPhrase));
                    }

                    workbook.Write(fs);

                }
                using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }

}
