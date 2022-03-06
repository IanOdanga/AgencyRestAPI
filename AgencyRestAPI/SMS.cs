using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyRestAPI
{
    public class SMS
    {
        SPRINTERWEBREF.Service1SoapClient sprintSms = new SPRINTERWEBREF.Service1SoapClient();
        public string Telephone = null;
        public string Text = null;
        public bool Results;

        public bool Send(string tranID, string saccoID, SMS sms)
        {
            bool res = sprintSms.SendSMSFromNav(sms.Telephone, tranID, sms.Text, saccoID, "CloudPESA2018!@2030");
            return res;
        }
    }
}