using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class MiniStatementRequest
    {
        public string agent_code { get; set; }
        public string id_Number { get; set; }
        public string session_id { get; set; }
    }

    public class MiniStatementResponse
    {
        public string response { get; set; }
        public string status_code { get; set; }
    }
}