using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class DepositsRequest
    {
        public string agent_code { get; set; }
        public string agent_name { get; set; }
        public string id_Number { get; set; }
        public decimal Amount { get; set; }
        public string session_id { get; set; }
    }
    public class DepositsResponse
    {
        public string response { get; set; }
        public string status_code { get; set; }
    }
}