using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class AgentInfoRequest
    {
        public string agent_code { get; set; }
    }
    public class AgentInfoResponse
    {
        public string agent_code { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
    }
}