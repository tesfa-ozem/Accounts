using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace Accounts
{
    public class Sms
    {
        public string SendSms(string phoneNumber, string message)
        {
            try
            {
                var client = new RestClient("https://payments.nicoza.net/index.php");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Postman-Token", "4017ec33-76e8-4682-8d0d-54083356567b");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
                request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", string.Format("------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"function\"\r\n\r\nsendSMS\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"phoneNumber\"\r\n\r\n{0}\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"message\"\r\n\r\n{1}\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", phoneNumber, message), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            
        }
    }
}
