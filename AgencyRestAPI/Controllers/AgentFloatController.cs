using AgencyRestAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace AgencyRestAPI.Controllers
{
    public class AgentFloatController : ApiController
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
            AgentFloatRequest agentFloatRequest = null;
            AgentFloatResponse agentFloatResponse = null;
            SuccessResponse successResponse = null;
            if (headers.Contains("Accept"))
            {
                try
                {
                    //string response = string.Empty;
                    var jsonContent = await Request.Content.ReadAsStringAsync();
                    agentFloatRequest = JsonConvert.DeserializeObject<AgentFloatRequest>(jsonContent);

                    if (agentFloatRequest != null)
                    {

                        string acc = string.Empty;
                        double agent_float = 0;

                        String response = string.Empty;
                        String agent = string.Empty;
                        string[] separators = new string[] { ":::" };

                        try
                        {
                            agent = Convert.ToString(MUKIService.GetAgentFloat(agentFloatRequest.agent_code));
                            string[] agentArray = agent.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

                            if (agentArray != null)
                            {
                                agentFloatResponse = new AgentFloatResponse
                                {
                                    response = Convert.ToString(agentArray)
                                };
                                jsonresponse.StatusCode = HttpStatusCode.OK;
                                Resultresponse = JsonConvert.SerializeObject(agentFloatResponse);
                            }
                            else
                            {
                                agentFloatResponse = new AgentFloatResponse
                                {
                                    status_code = "401",
                                    response = "Agent does not exist"
                                };
                                jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                Resultresponse = JsonConvert.SerializeObject(agentFloatResponse);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.WriteLogOnFile(ex.Message);
                        }
                    }
                    else
                    {
                        agentFloatResponse = new AgentFloatResponse
                        {
                            status_code = "400",
                            response = "Bad Request"
                        };
                        jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                        Resultresponse = JsonConvert.SerializeObject(agentFloatResponse);
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
