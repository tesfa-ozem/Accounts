using System;
using System.Collections.Generic;

namespace Accounts.Models
{
    public partial class FacilityKephlevel
    {
        public int Id { get; set; }
        public string Kephlevel { get; set; }
        public string KephlevelName { get; set; }
        public string Description { get; set; }
    }
}
