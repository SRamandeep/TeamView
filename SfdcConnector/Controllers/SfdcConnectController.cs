using Microsoft.Azure.Documents.Client.TransientFaultHandling;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SfdcApi.Sfdc;
using SfdcConnector.Data;
using SfdcConnector.Data.Models;
using SfdcConnector.Data.Repositories;
using SfdcConnector.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SfdcConnector.Controllers
{
    public class SfdcConnectController : ApiController
    {
        protected RepositoryCaseContract Repository;

        public SfdcConnectController(RepositoryCaseContract Repository)
        {
            this.Repository = Repository;
            
        }
        public async Task<IHttpActionResult> Post()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
            HttpClient authClient = new HttpClient();
            string oauthToken, serviceUrl;
            int syncCount = 0;

            //set system config variables
            int sfdcSyncFrequency = Convert.ToInt32(ConfigurationManager.AppSettings["SalesforceSyncFrequency"]);
            int sfdcBatchSize = Convert.ToInt32(ConfigurationManager.AppSettings["SalesforceSyncBatchSize"]);
            //defined remote access app - develop --> remote access --> new

            //set OAuth key and secret variables
            string sfdcConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            string sfdcConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];

            //set to Force.com user account that has API access enabled
            string sfdcUserName = ConfigurationManager.AppSettings["ForceUserName"];
            string sfdcPassword = ConfigurationManager.AppSettings["ForcePassword"];
            string sfdcToken = ConfigurationManager.AppSettings["ForceToken"];

            //create login password value
            string loginPassword = sfdcPassword + sfdcToken;

            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type","password"},
                {"client_id",sfdcConsumerKey},
                {"client_secret",sfdcConsumerSecret},
                {"username",sfdcUserName},
                {"password",loginPassword}
            });

            try
            {
                //print response values

                RootObject sfdcJson = new RootObject();
                sfdcJson.reqst = new Reqst();

                sfdcJson.reqst.caseList = new List<CaseList>();

                var unsynchedCases = Repository.All().Where(x=>x.SyncStatus==0).OrderBy(x=>x.Timestamp).Take(sfdcBatchSize);

                syncCount = unsynchedCases.Count();

                if (syncCount == 0)
                {
                    return Ok(new { sync_frequency = sfdcSyncFrequency, sync_count = syncCount });
                }

                foreach (var unsyncedCase in unsynchedCases)
                {
                    sfdcJson.reqst.caseList.Add(new CaseList
                    {
                        origin = unsyncedCase.Origins,
                        status = unsyncedCase.Status,
                        subject = unsyncedCase.Subject,
                        MongoID__c= unsyncedCase.MongoId.ToString()
                    });
                }

                #region Salesforce Token Obtain
                
                HttpResponseMessage message = await authClient.PostAsync(ConfigurationManager.AppSettings["SalesforceAuthUri"], content);

                string responseString = await message.Content.ReadAsStringAsync();

                JObject obj = JObject.Parse(responseString);
                oauthToken = (string)obj["access_token"];
                serviceUrl = (string)obj["instance_url"];
                #endregion

                #region SalesforceApi call
                HttpContent postContent = new StringContent(JsonConvert.SerializeObject(sfdcJson), Encoding.UTF8, "application/json");

                authClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", oauthToken));

                HttpResponseMessage responseMessage = await authClient.PostAsync(ConfigurationManager.AppSettings["SalesforceBulkCaseUri"], postContent);


                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {

                    //var filter = Builders<Case>.Filter.In(x => x.Id, unsynchedCases.Select(x => x.Id));
                    //var update = Builders<Case>.Update.Set(x => x.SyncStatus, 1);
                    //Repository.UpdateMany(filter, update, null, new System.Threading.CancellationToken());

                    var responseJson = responseMessage.Content.ReadAsStringAsync().Result;

                    var result = JsonConvert.DeserializeObject<SfdcResponse>(responseJson);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (var item in result.CaseRespList)
                    {
                        dict.Add(item.MongoID__c, item.CaseNumber);
                    }

                    foreach (var c in dict)
                    {
                        var filter = Builders<Case>.Filter.Eq("MongoId", c.Key);
                        //var update = Builders<Case>.Update.Set(x => x.CaseNumber, c.Value);
                        //update.AddToSet("SyncStatus",1);k

                        UpdateDefinitionBuilder<Case> updateBuilder = new UpdateDefinitionBuilder<Case>();

                        //updateBuilder.Set(x=>x.CaseNumber, c.Value);
                        //updateBuilder.Set(x=>x.SyncStatus, 1);

                        Repository.UpdateMany(filter, updateBuilder.Combine(Builders<Case>.Update.Set(x => x.CaseNumber, c.Value), Builders<Case>.Update.Set(x => x.SyncStatus, 1)), null, new System.Threading.CancellationToken());
                    }

                    #endregion
                    return Ok(new { sync_frequency = sfdcSyncFrequency, sync_count = syncCount });
                }

                return Ok(new { sync_frequency = sfdcSyncFrequency, sync_count = syncCount });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
        }
    }


    public class Attributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class CaseRespList
    {
        public Attributes attributes { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
        public string Origin { get; set; }
        public string CaseNumber { get; set; }
        public string MongoID__c { get; set; }
        public string Id { get; set; }
    }

    public class SfdcResponse
    {
        public string StatusMessage { get; set; }
        public string StatusCode { get; set; }
        public List<CaseRespList> CaseRespList { get; set; }
    }

}
