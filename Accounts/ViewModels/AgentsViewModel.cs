using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.ViewModels
{
    public class AgentsViewModel
    {
         public string FirstName { get; set; }
        
        public string LastName { get; set; }
        public string SUBCOUNTY { get; set; }
        public string WARD { get; set; }
        public string VILLAGE { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TerminalId { get; set; }
        public string PhoneNumber { get; set; }

        public string IdNumber { get; set; }
        public DateTime DateRegistered { get; set; }
    }

    public class PaymentsViewModel
    {
        public string PaymentMode { get; set; }
        public string PaymentType { get; set; }
        public string MemberNo { get; set; }
        public string PhoneNo { get; set; }
        public string AccountNo { get; set; }
        public string CardNo { get; set; }
        public decimal? Amount { get; set; }
        public int? Status { get; set; }
        public string DocumentNo { get; set; }
        public string Description { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
