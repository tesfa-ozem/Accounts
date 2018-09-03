using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounts.Models
{
    public partial class Agents
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TerminalId { get; set; }
       
        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }
        public string IdNumber { get; set; }
        public DateTime? DateRegistered { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
