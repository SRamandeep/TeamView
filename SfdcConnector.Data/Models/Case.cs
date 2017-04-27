using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfdcConnector.Data.Models
{
    public class Case
    {
        [MongoDB.Bson.Serialization.Attributes.BsonRequired()]
        [MongoDB.Bson.Serialization.Attributes.BsonId()]
        public String Id { get; set; }
        public String Status { get; set; }
        public String Subject { get; set; }
        public String Origins { get; set; }
        public string DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public int SyncStatus { get; set; }
        public string MongoId { get; set; }
        public string CaseNumber { get; set; }
    }
}
