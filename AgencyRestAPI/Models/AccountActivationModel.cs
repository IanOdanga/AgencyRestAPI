using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI.Models
{
    public class AccountActivationRequest
    {
        public string idNumber { get; set; }
    }

    public class AccountActivationResponse
    {
        public string status_code { get; set; }
        public string Description { get; set; }
        public bool Success { get; set; }
        public string ID_No { get; set; }
        public string Telephone_No { get; set; }
        public DateTime Date_Registered { get; set; }
        public string Corporate_No { get; set; }
    }
}