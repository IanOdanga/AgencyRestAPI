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
    public class DepositsController : ApiController
    {
        mukiAgencyWR.AgentProxy MUKIService = new mukiAgencyWR.AgentProxy();
        SMS sms = new SMS();

        [ResponseType(typeof(void))]
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            WrongHeaderResponse wrongheadersResponse = null;
            string callerIP = HttpContext.Current.Request.UserHostAddress;
            string Resultresponse = "";
            var headers = Request.Headers;
            var jsonresponse = Request.CreateResponse();
            DepositsRequest depositsRequest = null;
            DepositsResponse depositsResponse = null;
            SuccessResponse successResponse = null;
            SqlConnection connToSMobileDb = null;
            if (headers.Contains("Accept"))
            {
                string response = string.Empty;
                var jsonContent = await Request.Content.ReadAsStringAsync();
                depositsRequest = JsonConvert.DeserializeObject<DepositsRequest>(jsonContent);

                if (depositsRequest != null)
                {
                    string session_id = depositsRequest.session_id;
                    if (session_id != null)
                    {
                        string acc = string.Empty;
                        double agent_float = 0;

                        String agent = string.Empty;
                        string[] separators = new string[] { ":::" };

                        try
                        {
                            agent = Convert.ToString(MUKIService.GetMember(depositsRequest.id_Number));
                            string[] agentArray = agent.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

                            if (agentArray != null)
                            {
                                using (connToSMobileDb = new SqlConnection("Data Source=.;Initial Catalog=S-Mobile;User ID=mobile;Password=Mw@mba-J1w3%;MultipleActiveResultSets=True"))
                                {
                                    string SQL1 = "INSERT INTO [AGENCY BANKING].[dbo].[Agency Transactions] ([Document No], [Agent Code], [Agent Name], [ID No], [Amount], [Transaction Type], [Account No], [Funds Received], [Transaction Date], [Transaction Time]) VALUES(@session_id, @agent_code, @agent_name, @id_Number, @Amount, @Transtype, @AccountNo, 0, @DateTime, @Time);";
                                    SqlCommand cmd1 = new SqlCommand(SQL1, connToSMobileDb);
                                    if (connToSMobileDb.State != ConnectionState.Open)
                                    {
                                        connToSMobileDb.Open();
                                    }
                                    cmd1.Parameters.AddWithValue("@session_id", depositsRequest.session_id);
                                    cmd1.Parameters.AddWithValue("@agent_code", depositsRequest.agent_code);
                                    cmd1.Parameters.AddWithValue("@agent_name", depositsRequest.agent_name);
                                    cmd1.Parameters.AddWithValue("@id_Number", depositsRequest.id_Number);
                                    cmd1.Parameters.AddWithValue("@Amount", depositsRequest.Amount);
                                    cmd1.Parameters.AddWithValue("@Transtype", "Deposit");
                                    cmd1.Parameters.AddWithValue("@AccountNo", agentArray);
                                    cmd1.Parameters.AddWithValue("@DateTime", DateTime.Today);
                                    cmd1.Parameters.AddWithValue("@Time", DateTime.Now);
                                    int rst = cmd1.ExecuteNonQuery();

                                    if (rst > 0)
                                    {
                                        using (connToSMobileDb = new SqlConnection("Data Source=.;Initial Catalog=S-Mobile;User ID=mobile;Password=Mw@mba-J1w3%;MultipleActiveResultSets=True"))
                                        {
                                            string SQL = "SELECT * FROM [AGENCY BANKING].[dbo].[Agency Transactions] WHERE [Agent Code] = @agent_code AND [ID No] = @id_Number";
                                            SqlCommand cmd = new SqlCommand(SQL, connToSMobileDb);
                                            if (connToSMobileDb.State != ConnectionState.Open)
                                            {
                                                connToSMobileDb.Open();
                                            }
                                            cmd.Parameters.AddWithValue("@agent_code", depositsRequest.agent_code);
                                            cmd.Parameters.AddWithValue("@id_Number", depositsRequest.id_Number);

                                            using (SqlDataReader sqlReader = cmd.ExecuteReader())
                                            {
                                                if (sqlReader.HasRows)
                                                {

                                                    try
                                                    {
                                                        agent = Convert.ToString(MUKIService.GetAgentTransaction(depositsRequest.agent_code));

                                                        if (agentArray != null)
                                                        {
                                                            using (var db = new AGENCY_BANKINGEntities())
                                                            {
                                                                var a = db.Agency_Transactions.FirstOrDefault(b => b.ID_No == depositsRequest.id_Number && b.Agent_Code == depositsRequest.agent_code && b.Transaction_Date == DateTime.Today && b.Status == 1);
                                                                if (a != null)
                                                                {
                                                                    string refNo = a.Document_No;
                                                                    decimal Amount = Convert.ToInt32(a.Amount);
                                                                    string accountNo = a.Account_No;
                                                                    string agentName = a.Agent_Name;
                                                                    string Telephone = a.Receiver_Telephone_No;
                                                                    string accName = a.Account_Name;
                                                                    DateTime transDate = Convert.ToDateTime(a.Transaction_Date);
                                                                    string res = string.Empty;
                                                                    bool resp = false;

                                                                    res = MUKIService.InsertAgencyTransaction(refNo, accountNo, "Cash Deposit", Amount, "", "", depositsRequest.agent_code, "", accName, Telephone, depositsRequest.id_Number, 2, "", transDate, "", "", "", "");
                                                                    resp = MUKIService.PostAgentTransaction(refNo);

                                                                    if (resp == true)
                                                                    {
                                                                        sms.Text = String.Format("{0} Transaction completed.\n Reference: {1}\n Account: {2}\nAmount: {3} .", "Cash Deposit", refNo, accountNo, Amount);
                                                                        //MUKIService.InsertMessages(refNo, Telephone, sms.Text);

                                                                        if (sms != null)
                                                                        {
                                                                            //sms.Send(refNo, "00014", sms);
                                                                            depositsResponse = new DepositsResponse
                                                                            {
                                                                                response = "Transaction Completed and Posted Successfully"
                                                                            };
                                                                            jsonresponse.StatusCode = HttpStatusCode.OK;
                                                                            Resultresponse = JsonConvert.SerializeObject(depositsResponse);
                                                                        }
                                                                        else
                                                                        {
                                                                            depositsResponse = new DepositsResponse
                                                                            {
                                                                                status_code = "502",
                                                                                response = "Failed to Send SMS"
                                                                            };
                                                                            jsonresponse.StatusCode = HttpStatusCode.BadGateway;
                                                                            Resultresponse = JsonConvert.SerializeObject(depositsResponse);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        depositsResponse = new DepositsResponse
                                                                        {
                                                                            status_code = "502",
                                                                            response = "Failed to Post the Transaction"
                                                                        };
                                                                        jsonresponse.StatusCode = HttpStatusCode.BadGateway;
                                                                        Resultresponse = JsonConvert.SerializeObject(depositsResponse);
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    depositsResponse = new DepositsResponse
                                                                    {
                                                                        status_code = "400",
                                                                        response = "Transaction does not exist"
                                                                    };
                                                                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                                                                    Resultresponse = JsonConvert.SerializeObject(depositsResponse);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            depositsResponse = new DepositsResponse
                                                            {
                                                                status_code = "401",
                                                                response = "Agent does not exist"
                                                            };
                                                            jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                                            Resultresponse = JsonConvert.SerializeObject(depositsResponse);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Utilities.WriteLogOnFile(ex.Message);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        depositsResponse = new DepositsResponse
                                        {
                                            status_code = "401",
                                            response = "Failed to Insert Transaction"
                                        };
                                        jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                        Resultresponse = JsonConvert.SerializeObject(depositsResponse);
                                    }
                                }
                            }
                            else
                            {
                                depositsResponse = new DepositsResponse
                                {
                                    status_code = "401",
                                    response = "Member does not exist"
                                };
                                jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                Resultresponse = JsonConvert.SerializeObject(depositsResponse);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.WriteLogOnFile(ex.Message);
                        }
                    }
                    else
                    {
                        depositsResponse = new DepositsResponse
                        {
                            status_code = "400",
                            response = "Request parameters missing"
                        };
                        jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                        Resultresponse = JsonConvert.SerializeObject(depositsResponse);
                    }
                }
                else
                {
                    depositsResponse = new DepositsResponse
                    {
                        status_code = "400",
                        response = "Bad Request"
                    };
                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                    Resultresponse = JsonConvert.SerializeObject(depositsResponse);
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
