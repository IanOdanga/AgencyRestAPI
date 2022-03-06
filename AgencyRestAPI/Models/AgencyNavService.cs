using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AgencyRestAPI.mukiAgencyWR;

namespace AgencyRestAPI.Models
{
    public class AgencyNavService
    {
        public static AgentProxy MUKIService
        {

            get
            {
                var ws = new AgentProxy();

                return ws;
            }
        }
    }
}