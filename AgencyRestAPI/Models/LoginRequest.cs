using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class LoginRequest
    {
        public string agent_code { get; set; }
        public string password { get; set; }
    }
}