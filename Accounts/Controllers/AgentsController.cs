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

namespace Accounts.Controllers
{
    public class AgentsController : Controller
    {
       private readonly UHCContext _context;
        Logic logic = new Logic();
        
        public IActionResult Index()
        {
            List<AgentsViewModel> agents = logic.GetAllAgents();

            return View("Index",agents);
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
                    var worksheet = package.Workbook.Worksheets[1];

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
            if(login.UserName=="Admin" & login.Password == "kitui@254")
            {
                return RedirectToAction(actionName: nameof(Index));
            }

            return View("Login");
        }
    }
}
