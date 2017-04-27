using Microsoft.Azure.Documents.Client.TransientFaultHandling;
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
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SfdcConnector.Controllers
{
    public class SfdcController : ApiController
    {
        protected RepositoryCaseContract Repository;
        public static IReliableReadWriteDocumentClient Client { get; set; }


        public SfdcController(RepositoryCaseContract Repository)
        {
            this.Repository = Repository;
        }

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



            HttpResponseMessage message = await authClient.PostAsync(ConfigurationManager.AppSettings["SalesforceAuthUri"], content);

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

            HttpContent postContent = new StringContent(JsonConvert.SerializeObject(sfdcJson), Encoding.UTF8, "application/json");

            authClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", oauthToken));

            HttpResponseMessage responseMessage = await authClient.PostAsync(ConfigurationManager.AppSettings["SalesforceBulkCaseUri"], postContent);

            string postResponseString = await responseMessage.Content.ReadAsStringAsync();

            JObject responseObject = JObject.Parse(postResponseString);

            return Ok(responseObject);
        }

        [Route("Sfdc/Create")]
        public async Task<IHttpActionResult> Post([FromUri]CaseModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Invalid input values. Please send correct input.");
            }
            String transactionId = "";
            try
            {
                Case entity = new Case();
                entity.Id = Guid.NewGuid().ToString();
                entity.Origins = model.Origins;
                entity.Status = model.Status;
                entity.Subject = model.Subject;
                entity.SyncStatus = 0;
                entity.Timestamp = DateTime.UtcNow;
                entity.DeviceId = model.DeviceId;
                entity.MongoId = Guid.NewGuid().ToString();
                //entity = await _repository.AddOrUpdateAsync(entity);
                entity = Repository.Add(entity);
                SmsContact con = new SmsContact();
                con.adminName = "Ramandeep Singh";
                con.contactId = "14251231";
                con.deviceName = "IoT";
                con.email = "Ramandeep.singh@onactuate.com";
                con.message = entity.ToString();
                con.phone = "8800220368";
                SendSMS(con).GetAwaiter().GetResult();

                transactionId = entity.Id;
                

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
            return Ok(new { transactionId = transactionId });
        }
        // [Route("api/sms/send")]
        //[HttpGet]
        //public async Task<IHttpActionResult> SendSms([FromUri]SmsContact contact)
        //{

        //    try
        //    {
        //        HttpClient smsSender = new HttpClient();

        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest("model state not valid");
        //        }
        //        var data = new
        //        {
        //            url = ConfigurationManager.AppSettings["SmsGatewayRequestUrl"],
        //            username = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayUser"]),
        //            password = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayPassword"]),
        //            type = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestType"]),
        //            dlr = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestDelivery"]),
        //            source = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestSource"]),
        //            destination = HttpUtility.UrlEncode(String.Format("91{0}", contact.phone)),
        //            message = HttpUtility.UrlEncode(contact.message)
        //        };

        //        string encodedPostUrl = string.Format("{0}?username={1}&password={2}&type={3}&dlr={4}&destination={5}&source={6}&message={7}", data.url, data.username, data.password, data.type, data.dlr, data.destination, data.source, data.message);

        //        HttpResponseMessage responseMessage = await smsSender.PostAsync(encodedPostUrl, new StringContent(""));

        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            string messageResponseString = await responseMessage.Content.ReadAsStringAsync();

        //            if (messageResponseString.Split('|')[0] == "1701")
        //            {
        //                return Ok(messageResponseString);
        //            }
        //            else
        //            {
        //                return BadRequest("Someting wrong happened. Response returned : " + messageResponseString);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        BadRequest(ex.Message);
        //    }

        //    return Ok();
        //}
        

        public async Task< bool> SendSMS(SmsContact contact)
        {

            try
            {
                HttpClient smsSender = new HttpClient();

                if (!ModelState.IsValid)
                {
                    throw new Exception("Model is not valid");
                }
                var data = new
                {
                    url = ConfigurationManager.AppSettings["SmsGatewayRequestUrl"],
                    username = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayUser"]),
                    password = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayPassword"]),
                    type = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestType"]),
                    dlr = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestDelivery"]),
                    source = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestSource"]),
                    destination = HttpUtility.UrlEncode(String.Format("91{0}", contact.phone)),
                    message = HttpUtility.UrlEncode(contact.message)
                };

                string encodedPostUrl = string.Format("{0}?username={1}&password={2}&type={3}&dlr={4}&destination={5}&source={6}&message={7}", data.url, data.username, data.password, data.type, data.dlr, data.destination, data.source, data.message);

                HttpResponseMessage responseMessage = await smsSender.PostAsync(encodedPostUrl, new StringContent("")); ;

                if (responseMessage.IsSuccessStatusCode)
                {
                    string messageResponseString = await responseMessage.Content.ReadAsStringAsync();

                    if (messageResponseString.Split('|')[0] == "1701")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }

            return true;
        }


    }

    public class SmsContact
    {
        [Required]
        public string message { get; set; }

        [Required]
        public string phone { get; set; }
        [Required]
        public string email { get; set; }

        [Required]
        public string adminName { get; set; }
        [Required]
        public string deviceName { get; set; }
        [Required]
        public string contactId { get; set; }
    }
}
