using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Repo
{
    public class HttpPostedField
    {
        private string fieldName;
        private string data;

        public HttpPostedField(string fieldName, string data)
        {
            this.fieldName = fieldName;
            this.data = data;
        }
    }
}
