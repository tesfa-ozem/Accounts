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
using Accounts.ViewModels;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base.Enums;
using Accounts.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Accounts.Repo
{
    public class Logic
    {
        private static string _con= "Data Source=MAIN-SERVER;Initial Catalog=UHC;Integrated Security=True";
        public static string _con2 = "Data Source=MAIN-SERVER;Initial Catalog=TableDependencyDB;Integrated Security=True;User Id=Tesfa;Password=123@Team;TrustServerCertificate=True";

        UHCContext Context = new UHCContext();
        DateTime DateTime = new DateTime();
        private static Random random = new Random();
        int pin = random.Next(1000, 9999);
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomPassword()
        {

            int pin = random.Next(1000, 9999);
            return new string(Security.EncryptString(pin.ToString(), Security.pPhrase));
        }

        public string SendSMSAsync(AppUsers agents)
        {
            try
            {
                UHCContext context = new UHCContext();


                Sms sms = new Sms();

                Random random = new Random();
                int pin = random.Next(1000, 9999);
                string msg = string.Format("Dear {0} your KChic login credentials are : username : {1}, password : {2}", agents.FirstName, agents.UserName, pin);
                sms.SendSms(agents.PhoneNumber, msg);
                agents.Password = Security.EncryptString(pin.ToString(), Security.pPhrase);



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
            try
            {


                var rowCount = worksheet.Dimension?.Rows;
                var colCount = worksheet.Dimension?.Columns;

                if (!rowCount.HasValue || !colCount.HasValue)
                {
                    return string.Empty;
                }
                List<AppUsers> AgentsList = new List<AppUsers>();
                var sb = new StringBuilder();
                for (int row = 3; row <= rowCount.Value; row++)
                {
                    string fullNames = worksheet.Cells[row, 2].Value.ToString();


                    AppUsers agent = (new AppUsers
                    {
                        IdNumber = worksheet.Cells[row, 3].Value.ToString(),
                        PhoneNumber = worksheet.Cells[row, 4].Value.ToString(),
                        SUBCOUNTY = worksheet.Cells[row, 5].Value.ToString(),
                        WARD = worksheet.Cells[row, 6].Value.ToString(),
                        VILLAGE = worksheet.Cells[row, 6].Value.ToString(),
                        Password = RandomPassword(),
                        UserName = worksheet.Cells[row, 3].Value.ToString(),
                        DateRegistered = DateTime.Now.Date
                    });
                    if (fullNames != null)
                    {
                        string[] names = fullNames.Split(' ');
                        agent.FirstName = names[0];
                        if (names.Length > 1)
                            agent.LastName = names[1];
                    }
                    AgentsList.Add(agent);

                }
                Context.AppUsers.AddRangeAsync(AgentsList);

                Context.SaveChanges();
                return sb.ToString();
            }
            catch (Exception e)
            {

                return e.ToString();

            }
        }

        public List<AgentsViewModel> GetAllAgents()
        {

            try
            {

                var AgentData = Context.AppUsers.OrderByDescending(u => u.Id).ToList();
                List<AgentsViewModel> agentsViews = new List<AgentsViewModel>();
                foreach (AppUsers agent in AgentData)
                {
                    AgentsViewModel AgentsViewModel = new AgentsViewModel()
                    {
                        FirstName = agent.FirstName,
                        LastName = agent.LastName,
                        IdNumber = agent.IdNumber,
                        PhoneNumber = agent.PhoneNumber,
                        SUBCOUNTY = agent.SUBCOUNTY,
                        TerminalId = agent.TerminalId,
                        UserName = agent.UserName,

                    };
                    agentsViews.Add(AgentsViewModel);
                }

                return agentsViews;
            }
            catch (Exception)
            {
                return new List<AgentsViewModel>();

            }
        }

        public AppUserResult AddNewMember(AgentsViewModel agentsViewModel)
        {
            try
            {
                UHCContext context = new UHCContext();

                AppUsers agent = (new AppUsers
                {
                    IdNumber = agentsViewModel.IdNumber,
                    FirstName = agentsViewModel.FirstName,
                    LastName = agentsViewModel.LastName,
                    PhoneNumber = agentsViewModel.PhoneNumber,
                    SUBCOUNTY = agentsViewModel.SUBCOUNTY,
                    WARD = agentsViewModel.WARD,
                    VILLAGE = agentsViewModel.VILLAGE,
                    Password = agentsViewModel.Password,
                    UserName = agentsViewModel.UserName,

                    DateRegistered = DateTime.Now.Date
                });

                SendSMSAsync(agent);
                context.Add(agent);
                context.SaveChanges();
                return new AppUserResult()
                {

                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public AppUserResult DeletUser(string Idnumber)
        {
            try
            {
                UHCContext context = new UHCContext();

                var key = context.AppUsers.Where(e => e.IdNumber == Idnumber).FirstOrDefault();
                var Agent = context.AppUsers.Find(key.Id);

                context.AppUsers.Remove(Agent);
                context.SaveChanges();
                return new AppUserResult()
                {

                };
            }
            catch (Exception)
            {
                return new AppUserResult()
                {

                };

            }
        }
        public string readExcelPackageToStringBulk(ExcelPackage package, ExcelWorksheet worksheet)
        {
            try
            {


                var rowCount = worksheet.Dimension?.Rows;
                var colCount = worksheet.Dimension?.Columns;

                if (!rowCount.HasValue || !colCount.HasValue)
                {
                    return string.Empty;
                }
                List<BulkPayment> AgentsList = new List<BulkPayment>();
                var sb = new StringBuilder();
                for (int row = 3; row <= rowCount.Value; row++)
                {
                    string fullNames = worksheet.Cells[row, 2].Value.ToString();


                    BulkPayment Person = (new BulkPayment
                    {
                        FirstName = worksheet.Cells[row, 3].Value.ToString(),
                        MiddleName = worksheet.Cells[row, 4].Value.ToString(),
                        LastName = worksheet.Cells[row, 5].Value.ToString(),
                        IdentificationNo = worksheet.Cells[row, 6].Value.ToString(),
                        PhoneNumber = worksheet.Cells[row, 6].Value.ToString(),
                    });

                    AgentsList.Add(Person);

                }
                Context.BulkPayments.AddRangeAsync(AgentsList);

                Context.SaveChanges();
                return sb.ToString();
            }
            catch (Exception e)
            {

                return e.ToString();

            }
        }

        public string GetAllPeople()
        {
            DataHub hub = new DataHub();
            UHCContext DbContext = new UHCContext();
        
            //List<Principals> principals = new List<Principals>();
            using (var tableDependency = new SqlTableDependency<AppUsers>(_con))
            {
                tableDependency.OnChanged += TableDependency_Changed;
                
            }
            void TableDependency_Changed(object sender, RecordChangedEventArgs<AppUsers> e)
            {
                if (e.ChangeType != ChangeType.None)
                {
                    var changedEntity = e.Entity;
                    hub.Clients.All.SendAsync("Notify",changedEntity.Id);
                
                }
            }

            var PrinciplesCount = DbContext.AppUsers.OrderByDescending(u => u.Id).ToList();

            return PrinciplesCount.ToString();
        }

        public  List<Payments> GetPayments()
        {
            
            var AllPayments = Context.Payments.OrderByDescending(e => e.Id).ToList();
            
            return AllPayments;
        }
        
        
    }
}
