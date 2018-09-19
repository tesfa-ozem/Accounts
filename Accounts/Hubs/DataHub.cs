using Accounts.Repo;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Accounts.Hubs
{
     public interface IChatClient
    {Task ReceivePrinciples(string message);
    }

    public class DataHub:Hub
    { 
        
    }
   

   

}