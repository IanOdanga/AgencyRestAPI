using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class SuccessResponse
    {
        public string status_code { get; set; }
        public string Description { get; set; }
        public bool logged_in { get; set; }
        public bool Success { get; set; }
    }
}