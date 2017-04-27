using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaseDeviceSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient authClient = new HttpClient();

            while (true)
            {
                try
                {
                    string status = "";
                    string subject = "";
                    string origin = "";
                    string deviceId = "";

                    Random random = new Random();

                    switch (random.Next(1,4))
                    {
                        case 1: status = "Failure"; break;
                        case 2: status = "Malfunction"; break;
                        case 3: status = "Vitals"; break;
                        case 4: status = "Okay"; break;
                        default: status = "Warning"; break;
                    }

                    switch (random.Next(1, 4))
                    {
                        case 1: subject = "Turbine Speed"; break;
                        case 2: subject = "Heating"; break;
                        case 3: subject = "Power Off"; break;
                        case 4: subject = "Leak"; break;
                        default: subject = "Heating"; break;
                    }

                    switch (random.Next(1, 2))
                    {
                        case 1: origin = "Washing Machine"; break;
                        case 2: origin = "Air Conditioner"; break;
                        default: origin = "Heater"; break;
                    }

                    switch (random.Next(1, 4))
                    {
                        case 1: deviceId = "110090"; break;
                        case 2: deviceId = "110100"; break;
                        case 3: deviceId = "110098"; break;
                        case 4: deviceId = "110096"; break;
                        default: deviceId = "110094"; break;
                    }

                    string uri = string.Format("http://iotweb.azurewebsites.net/api/sfdc?Status={0}&Subject={1}&Origins={2}&DeviceId={3}",status,subject,origin,deviceId);

                    HttpResponseMessage message = authClient.PostAsync(uri, new FormUrlEncodedContent(new Dictionary<string, string>())).GetAwaiter().GetResult();

                    string responseString = message.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    JObject obj = JObject.Parse(responseString);
                    string transactionId = (string)obj["transactionId"];

                    Console.Write("Transaction Id " + transactionId);
                    Console.Write("\n");

                    int sleepPeriod = random.Next(5000, 20000);

                    Console.Write("Sleeping for "+ sleepPeriod + " Seconds");
                    Console.Write("\n");

                    Thread.Sleep(sleepPeriod);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
