using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Result
{
    public class ChallengeResult 
    {
        public string Type { get; set; }

        public string Message { get; set; }

        public string Title { get; set; }
    }
}
