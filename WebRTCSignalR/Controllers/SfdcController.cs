using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebRTCSignalR.Sfdc;

namespace WebRTCSignalR.Controllers
{
    public class SfdcController : ApiController
    {
        public async Task<IHttpActionResult> Get()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
            HttpClient authClient = new HttpClient();
            string oauthToken, serviceUrl;

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

            HttpResponseMessage message = await authClient.PostAsync("https://na35.salesforce.com/services/oauth2/token", content);

            string responseString = await message.Content.ReadAsStringAsync();

            JObject obj = JObject.Parse(responseString);
            oauthToken = (string)obj["access_token"];
            serviceUrl = (string)obj["instance_url"];

            //print response values
            
            RootObject sfdcJson = new RootObject();
            sfdcJson.reqst = new Reqst();

            sfdcJson.reqst.caseList = new List<CaseList>() {
                new CaseList() {
                    origin ="origin 1",
                    status ="success",
                    subject = "new subject 1"
                },
                new CaseList() {
                    origin ="origin 2",
                    status ="success",
                    subject = "new subject 2"
                },
                new CaseList() {
                    origin ="origin 3",
                    status ="fail",
                    subject = "new subject 3"
                },
                new CaseList() {
                    origin ="origin 4",
                    status ="success",
                    subject = "new subject 4"
                },
            };

            HttpContent postContent = new StringContent(JsonConvert.SerializeObject(sfdcJson),Encoding.UTF8,"application/json");

            authClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", oauthToken));

            HttpResponseMessage responseMessage = await authClient.PostAsync("https://na35.salesforce.com/services/apexrest/CaseService/", postContent);

            string postResponseString = await responseMessage.Content.ReadAsStringAsync();

            JObject responseObject = JObject.Parse(postResponseString);

            return Ok(responseObject);
        }
    }
}
