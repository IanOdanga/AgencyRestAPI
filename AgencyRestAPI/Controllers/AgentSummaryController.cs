using AgencyRestAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using static AgencyRestAPI.Models.AgentSummaryResponse;

namespace AgencyRestAPI.Controllers
{
    public class AgentSummaryController : ApiController
    {
        mukiAgencyWR.AgentProxy MUKIService = new mukiAgencyWR.AgentProxy();


        [ResponseType(typeof(void))]
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            WrongHeaderResponse wrongheadersResponse = null;
            string callerIP = HttpContext.Current.Request.UserHostAddress;
            string Resultresponse = "";
            var headers = Request.Headers;
            var jsonresponse = Request.CreateResponse();
            AgentSummaryRequest agentsummRequest = null;
            AgentSummaryResponse agentsummResponse = null;
            SuccessResponse successResponse = null;
            if (headers.Contains("Accept"))
            {
                try
                {
                    //string response = string.Empty;
                    var jsonContent = await Request.Content.ReadAsStringAsync();
                    agentsummRequest = JsonConvert.DeserializeObject<AgentSummaryRequest>(jsonContent);

                    if (agentsummRequest != null)
                    {

                        string acc = string.Empty;
                        double agent_float = 0;

                        String response = string.Empty;
                        String agent = string.Empty;
                        string[] separators = new string[] { ":::" };

                        try
                        {
                            agent = Convert.ToString(MUKIService.GetAgentTransaction(agentsummRequest.agent_code));
                            string[] agentArray = agent.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

                            if (agentArray != null)
                            {
                                agentsummResponse = new AgentSummaryResponse
                                {
                                    response = Convert.ToString(agentArray)
                                };
                                jsonresponse.StatusCode = HttpStatusCode.OK;
                                Resultresponse = JsonConvert.SerializeObject(agentsummResponse);
                            }
                            else
                            {
                                agentsummResponse = new AgentSummaryResponse
                                {
                                    status_code = "401",
                                    response = "Agent does not exist"
                                };
                                jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                Resultresponse = JsonConvert.SerializeObject(agentsummResponse);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.WriteLogOnFile(ex.Message);
                        }
                    }
                    else
                    {
                        agentsummResponse = new AgentSummaryResponse
                        {
                            status_code = "400",
                            response = "Bad Request"
                        };
                        jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                        Resultresponse = JsonConvert.SerializeObject(agentsummResponse);
                    }
                }
                catch (Exception ex)
                {
                    successResponse = new SuccessResponse
                    {
                        Description = ex.Message,
                        status_code = "502",
                        Success = false
                    };
                    jsonresponse.StatusCode = HttpStatusCode.BadGateway;
                    Resultresponse = JsonConvert.SerializeObject(successResponse);
                }
            }
            else
            {
                wrongheadersResponse = new WrongHeaderResponse
                {
                    Description = "Request Headers do not exist",
                    statusCode = "400",
                    logged_in = false,
                    Success = false
                };
                jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                Resultresponse = JsonConvert.SerializeObject(wrongheadersResponse);
            }
            jsonresponse.Content = new StringContent(Resultresponse, Encoding.UTF8, "application/json");
            return jsonresponse;
        }
    }
}
