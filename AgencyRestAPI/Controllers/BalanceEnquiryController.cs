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

namespace AgencyRestAPI.Controllers
{
    public class BalanceEnquiryController : ApiController
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
            BalanceEnquiryRequest balancerequest = null;
            BalanceEnquiryResponse balanceresponse = null;
            SuccessResponse successResponse = null;
            SqlConnection connToSMobileDb = null;
            if (headers.Contains("Accept"))
            {
                string response = string.Empty;
                var jsonContent = await Request.Content.ReadAsStringAsync();
                balancerequest = JsonConvert.DeserializeObject<BalanceEnquiryRequest>(jsonContent);

                if (balancerequest != null)
                {
                    string acc = string.Empty;
                    double agent_float = 0;

                    String agent = string.Empty;
                    string[] separators = new string[] { ":::" };

                    try
                    {
                        agent = Convert.ToString(MUKIService.GetMember(balancerequest.id_Number));
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
                                cmd.Parameters.AddWithValue("@agent_code", balancerequest.agent_code);
                                cmd.Parameters.AddWithValue("@id_Number", balancerequest.id_Number);

                                using (SqlDataReader sqlReader = cmd.ExecuteReader())
                                {
                                    if (sqlReader.HasRows)
                                    {

                                        try
                                        {
                                            using (var db = new AGENCY_BANKINGEntities())
                                            {
                                                var a = db.Agency_Transactions.FirstOrDefault(b => b.ID_No == balancerequest.id_Number && b.Agent_Code == balancerequest.agent_code && b.Transaction_Date == DateTime.Today && b.Status == 1);
                                                if (a != null)
                                                {
                                                    decimal agent1 = 0;
                                                    string refNo = a.Document_No;
                                                    decimal Amount = Convert.ToInt32(a.Amount);
                                                    string accountNo = a.Account_No;
                                                    string agentName = a.Agent_Name;
                                                    string Telephone = a.Receiver_Telephone_No;
                                                    string accName = a.Account_Name;
                                                    DateTime transDate = Convert.ToDateTime(a.Transaction_Date);
                                                    string res = string.Empty;

                                                    res = MUKIService.InsertAgencyTransaction(refNo, accountNo, "Balance Enquiry", Amount, "", "", balancerequest.agent_code, "", accName, Telephone, balancerequest.id_Number, 2, "", transDate, "", "", "", "");

                                                    agent1 = MUKIService.GetAccountBalance(balancerequest.id_Number);
                                                    balanceresponse = new BalanceEnquiryResponse
                                                    {
                                                        response = Convert.ToString(agent1)
                                                    };
                                                    jsonresponse.StatusCode = HttpStatusCode.OK;
                                                    Resultresponse = JsonConvert.SerializeObject(balancerequest);
                                                }
                                                else
                                                {
                                                    balanceresponse = new BalanceEnquiryResponse
                                                    {
                                                        status_code = "400",
                                                        response = "Transaction does not exist"
                                                    };
                                                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                                                    Resultresponse = JsonConvert.SerializeObject(balanceresponse);
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
                                        balanceresponse = new BalanceEnquiryResponse
                                        {
                                            status_code = "401",
                                            response = "Agent does not exist"
                                        };
                                        jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                        Resultresponse = JsonConvert.SerializeObject(balanceresponse);
                                    }
                                }
                            }
                        }
                        else
                        {
                            balanceresponse = new BalanceEnquiryResponse
                            {
                                status_code = "401",
                                response = "Member does not exist"
                            };
                            jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                            Resultresponse = JsonConvert.SerializeObject(balanceresponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogOnFile(ex.Message);
                    }
                }
                else
                {
                    balanceresponse = new BalanceEnquiryResponse
                    {
                        status_code = "400",
                        response = "Bad Request"
                    };
                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                    Resultresponse = JsonConvert.SerializeObject(balanceresponse);
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
