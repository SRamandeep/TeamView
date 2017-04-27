using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SfdcConnector.Models
{
    public class CaseDocDbModel
    {
        public String id { get; set; }
        public String Status { get; set; }
        public String Subject { get; set; }
        public String Origins { get; set; }
        public string DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public int SyncStatus { get; set; }
    }
}