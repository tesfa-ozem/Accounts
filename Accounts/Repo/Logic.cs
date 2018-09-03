using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Repo
{
    public class Logic
    {
        public async Task<string> SendSMSAsync(Agents agents)
        {
            try
            {
                UHCContext context = new UHCContext();
                var phone = context.Agents
                                .Where(b => b.PhoneNumber == agents.PhoneNumber)
                                .FirstOrDefault();
              

                Sms sms = new Sms();

                Random random = new Random();
                int pin = random.Next(1000, 9999);
                string msg = string.Format("Dear {0} your KChic login credentials are : username : {1}, password : {2}", agents.FirstName, agents.UserName, pin);
                sms.SendSms(agents.PhoneNumber, msg);
                agents.Password = Security.EncryptString(pin.ToString(), Security.pPhrase);

                context.Add(agents);
                await context.SaveChangesAsync();

                return "";
            }
            catch (Exception)
            {

                
            }
            return "";
        }
    }
}
