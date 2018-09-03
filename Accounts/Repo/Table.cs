using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Repo
{
    public class Table
    {
        public System.Data.DataTable dataTable { get; set; }
        public DataColumn column { get; set; }
        public DataRow row { get; set; }
        public Table(System.Data.DataTable data)
        {
            dataTable = data;


        }
    }
}
