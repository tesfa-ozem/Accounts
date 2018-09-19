using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Accounts.ViewModels;
using Accounts.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq.Dynamic.Core;
using Accounts.Models;
using Microsoft.AspNetCore.Http;
using Accounts.Repo;
using System.IO;
using OfficeOpenXml;

namespace Accounts.Controllers
{
    public class AgentsController : Controller
    {
    
        private UHCContext _context;
        public AgentsController(UHCContext context)
        {
            _context = context;
        }
        Logic logic = new Logic();
        public IActionResult Index()
        {
            

            return View();
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


        } public async Task<IActionResult> Download(string filename)
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
                logic.GetAllPeople();
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
                return RedirectToAction(actionName: nameof(Dashboard));
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
                    var worksheet = package.Workbook.Worksheets[1];

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

        public async Task<IActionResult> Registration()
        {
          
        
          // TODO Remove
          await Task.Yield();
        
          return View("Registration");
        }

        public IActionResult Dashboard()
        {
            

            return View("Dashboard");
        }

        public IActionResult Payments()
        {
        
        return View();
        }
        
        [HttpPost]
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data  
                var customerData = (from tempcustomer in _context.AppUsers
                                    select tempcustomer);

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.Password== searchValue);
                }

                //total number of rows count   
                recordsTotal = customerData.Count();
                //Paging   
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception e)
            {
                return NotFound(e.Message.ToString());
            }
        }

    }
}
