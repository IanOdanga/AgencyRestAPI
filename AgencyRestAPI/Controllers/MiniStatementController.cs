using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Description;
using System.Data;
using System.Data.SqlClient;
using AgencyRestAPI.Models;
using Newtonsoft.Json;

namespace AgencyRestAPI.Controllers
{
    public class MiniStatementController : ApiController
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
            MiniStatementRequest statementsRequest = null;
            MiniStatementResponse statementsResponse = null;
            SuccessResponse successResponse = null;
            SqlConnection connToSMobileDb = null;
            if (headers.Contains("Accept"))
            {
                string response = string.Empty;
                var jsonContent = await Request.Content.ReadAsStringAsync();
                statementsRequest = JsonConvert.DeserializeObject<MiniStatementRequest>(jsonContent);

                if (statementsRequest != null)
                {
                    string acc = string.Empty;
                    double agent_float = 0;

                    String agent = string.Empty;
                    string[] separators = new string[] { ":::" };

                    try
                    {
                        agent = Convert.ToString(MUKIService.GetMember(statementsRequest.id_Number));
                        string[] agentArray = agent.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

                        if (agentArray != null)
                        {
                            using (connToSMobileDb = new SqlConnection("Data Source=.;Initial Catalog=S-Mobile;User ID=mobile;Password=Mw@mba-J1w3%;MultipleActiveResultSets=True"))
                            {
                                string SQL = "SELECT * FROM [AGENCY BANKING].[dbo].[Agency Transactions] WHERE [Agent Code] = @agent_code AND [ID No] = @id_Number";
                                SqlCommand cmd = new SqlCommand(SQL, connToSMobileDb);
                                if (connToSMobileDb.State != ConnectionState.Open)
                                {
                                    connToSMobileDb.Open();
                                }
                                cmd.Parameters.AddWithValue("@agent_code", statementsRequest.agent_code);
                                cmd.Parameters.AddWithValue("@id_Number", statementsRequest.id_Number);

                                using (SqlDataReader sqlReader = cmd.ExecuteReader())
                                {
                                    if (sqlReader.HasRows)
                                    {

                                        try
                                        {
                                            using (var db = new AGENCY_BANKINGEntities())
                                            {
                                                var a = db.Agency_Transactions.FirstOrDefault(b => b.ID_No == statementsRequest.id_Number && b.Agent_Code == statementsRequest.agent_code && b.Transaction_Date == DateTime.Today && b.Status == 1);
                                                if (a != null)
                                                {
                                                    String agent1 = string.Empty;
                                                    string refNo = a.Document_No;
                                                    decimal Amount = Convert.ToInt32(a.Amount);
                                                    string accountNo = a.Account_No;
                                                    string agentName = a.Agent_Name;
                                                    string Telephone = a.Receiver_Telephone_No;
                                                    string accName = a.Account_Name;
                                                    DateTime transDate = Convert.ToDateTime(a.Transaction_Date);
                                                    string res = string.Empty;

                                                    res = MUKIService.InsertAgencyTransaction(refNo, accountNo, "Mini Statement", Amount, "", "", statementsRequest.agent_code, "", accName, Telephone, statementsRequest.id_Number, 2, "", transDate, "", "", "", "");

                                                    agent1 = MUKIService.GetMiniStatement(statementsRequest.id_Number);
                                                    statementsResponse = new MiniStatementResponse
                                                    {
                                                        response = agent1
                                                    };
                                                    jsonresponse.StatusCode = HttpStatusCode.OK;
                                                    Resultresponse = JsonConvert.SerializeObject(statementsRequest);
                                                }
                                                else
                                                {
                                                    statementsResponse = new MiniStatementResponse
                                                    {
                                                        status_code = "400",
                                                        response = "Transaction does not exist"
                                                    };
                                                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                                                    Resultresponse = JsonConvert.SerializeObject(statementsResponse);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Utilities.WriteLogOnFile(ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        statementsResponse = new MiniStatementResponse
                                        {
                                            status_code = "401",
                                            response = "Agent does not exist"
                                        };
                                        jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                        Resultresponse = JsonConvert.SerializeObject(statementsResponse);
                                    }
                                }
                            }
                        }
                        else
                        {
                            statementsResponse = new MiniStatementResponse
                            {
                                status_code = "401",
                                response = "Member does not exist"
                            };
                            jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                            Resultresponse = JsonConvert.SerializeObject(statementsResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogOnFile(ex.Message);
                    }
                }
                else
                {
                    statementsResponse = new MiniStatementResponse
                    {
                        status_code = "400",
                        response = "Bad Request"
                    };
                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                    Resultresponse = JsonConvert.SerializeObject(statementsResponse);
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
