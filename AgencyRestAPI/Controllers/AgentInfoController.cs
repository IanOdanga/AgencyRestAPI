using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Web.Http.Description;
using AgencyRestAPI.Models;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace AgencyRestAPI.Controllers
{
    public class AgentInfoController : ApiController
    {
        [ResponseType(typeof(void))]
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            WrongHeaderResponse wrongheadersResponse = null;
            string callerIP = HttpContext.Current.Request.UserHostAddress;
            string Resultresponse = "";
            var headers = Request.Headers;
            var jsonresponse = Request.CreateResponse();
            AgentInfoRequest agentInfoRequest = null;
            AgentInfoResponse agentInfoResponse = null;
            SuccessResponse successResponse = null;
            SqlConnection connToSMobileDb = null;
            if (headers.Contains("Accept"))
            {
                try
                {
                    string response = string.Empty;
                    var jsonContent = await Request.Content.ReadAsStringAsync();
                    agentInfoRequest = JsonConvert.DeserializeObject<AgentInfoRequest>(jsonContent);

                    if (agentInfoRequest != null)
                    {

                        using (connToSMobileDb = new SqlConnection("Data Source=LUKE;Initial Catalog=AGENCY BANKING;User ID=Luke;Password=DNqfrXMIya62kGX;MultipleActiveResultSets=True"))
                        {
                            string SQL = "SELECT * FROM [AGENCY BANKING].[dbo].[Agents] WHERE [Agent Code] = @agent_code AND [Active]=1";
                            using (SqlCommand cmd = new SqlCommand(SQL, connToSMobileDb))
                            {
                                if (connToSMobileDb.State != ConnectionState.Open)
                                {
                                    connToSMobileDb.Open();
                                }
                                cmd.Parameters.AddWithValue("@agent_code", agentInfoRequest.agent_code);
                                using (SqlDataReader sqlReader = cmd.ExecuteReader())
                                {

                                    if (sqlReader.HasRows)
                                    {
                                        using (var db = new AGENCY_BANKINGEntities())
                                        {
                                            var a = db.Agents.FirstOrDefault(b => b.Agent_Code == agentInfoRequest.agent_code);
                                            if (a != null)
                                            {
                                                agentInfoResponse = new AgentInfoResponse
                                                {
                                                    agent_code = agentInfoRequest.agent_code,
                                                    Name = a.Name,
                                                    Telephone = a.Telephone
                                                };
                                                jsonresponse.StatusCode = HttpStatusCode.OK;
                                                Resultresponse = JsonConvert.SerializeObject(agentInfoResponse);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        successResponse = new SuccessResponse
                                        {
                                            status_code = "401",
                                            Description = "Agent Code does not exist",
                                            Success = false
                                        };
                                        jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                        Resultresponse = JsonConvert.SerializeObject(successResponse);
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        successResponse = new SuccessResponse
                        {
                            status_code = "400",
                            Description = "Bad Request",
                            Success = false
                        };
                        jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                        Resultresponse = JsonConvert.SerializeObject(successResponse);
                    }
                }
                catch (Exception e)
                {
                    successResponse = new SuccessResponse
                    {
                        Description = e.Message,
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
