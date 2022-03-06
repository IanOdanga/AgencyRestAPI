using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class WrongHeaderResponse
    {
        public string statusCode { get; set; }
        public string Description { get; set; }
        public bool logged_in { get; set; }
        public bool Success { get; set; }
    }
}