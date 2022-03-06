using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class ChangePasswordRequest
    {
        public string agent_code { get; set; }
        public string old_password { get; set; }
        public string new_password { get; set; }
        public string session_id { get; set; }
    }

    public class ChangePasswordResponse
    {
        public string status_code { get; set; }
        public string Description { get; set; }
        public bool Success { get; set; }
    }
}