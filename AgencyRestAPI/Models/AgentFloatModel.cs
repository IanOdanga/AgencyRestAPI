using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class AgentFloatRequest
    {
        public string agent_code { get; set; }
    }
    public class AgentFloatResponse
    {
        public string response { get; set; }
        public string status_code { get; set; }
    }
}