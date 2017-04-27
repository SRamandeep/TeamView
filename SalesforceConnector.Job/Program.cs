using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Threading;
using IoT.Common.Logging;

namespace SalesforceConnector.Job
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            HttpClient authClient = new HttpClient();

            LoggingManager.InitializeLogger("AppEventSourceManager", System.Diagnostics.Tracing.EventLevel.LogAlways);

            while (true)
            {
                try
                {
                    HttpResponseMessage message = authClient.PostAsync("http://oa-sfdcwear-dev.azurewebsites.net/api/sfdcconnect", new FormUrlEncodedContent(new Dictionary<string, string>())).GetAwaiter().GetResult();

                    string responseString = message.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    AppEventSourceManager.Log.Debug(responseString, "SalesforceSyncScheduler");

                    JObject obj = JObject.Parse(responseString);
                    int syncFrequency = (int)obj["sync_frequency"];
                    int syncCount = (int)obj["sync_count"];

                    Console.Write("Records Synced " + syncCount );
                    Console.Write("Sleeping for " + syncFrequency + " Seconds");
                    Console.Write("\n");
                    Thread.Sleep(syncFrequency);
                }
                catch (Exception ex)
                {
                    AppEventSourceManager.Log.Error(ex.Message, "SalesforceSyncScheduler");
                    //throw;
                }
            }           
        }
    }
}
