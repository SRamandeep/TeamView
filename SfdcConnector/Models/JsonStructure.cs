using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SfdcApi.Sfdc
{
    public class CaseList
    {
        public string status { get; set; }
        public string subject { get; set; }
        public string origin { get; set; }
        public string MongoID__c { get; internal set; }
    }

    public class Reqst
    {
        public List<CaseList> caseList { get; set; }
    }

    public class RootObject
    {
        public Reqst reqst { get; set; }
    }
}