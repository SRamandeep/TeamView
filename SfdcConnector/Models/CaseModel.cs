using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SfdcConnector.Models
{
    public class CaseModel
    {
        public virtual String Status { get; set; }
        public virtual String Subject { get; set; }
        public virtual String Origins { get; set; }
        public virtual string DeviceId { get; set; }
    }
}