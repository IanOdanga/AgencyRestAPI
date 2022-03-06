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
    public class AccountActivationController : ApiController
    {
        [ResponseType(typeof(void))]
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            string callerIP = HttpContext.Current.Request.UserHostAddress;
            WrongHeaderResponse wrongheadersResponse = null;
            string Resultresponse = "";
            var headers = Request.Headers;
            var jsonresponse = Request.CreateResponse();
            AccountActivationRequest activationRequest = null;
            AccountActivationResponse activationResponse = null;
            SuccessResponse successResponse = null;
            SqlConnection connToSMobileDb = null;
            if (headers.Contains("Accept"))
            {
                try
                {
                    string response = string.Empty;
                    var jsonContent = await Request.Content.ReadAsStringAsync();
                    activationRequest = JsonConvert.DeserializeObject<AccountActivationRequest>(jsonContent);

                    if (activationRequest != null)
                    {
                        using (connToSMobileDb = new SqlConnection("Data Source=LUKE;Initial Catalog=AGENCY BANKING;User ID=Luke;Password=DNqfrXMIya62kGX;MultipleActiveResultSets=True"))
                        {
                            string SQL = "SELECT * FROM [AGENCY BANKING].[dbo].[Agency Members] WHERE [ID No] = @idNumber";
                            SqlCommand cmd = new SqlCommand(SQL, connToSMobileDb);
                            if (connToSMobileDb.State != ConnectionState.Open)
                            {
                                connToSMobileDb.Open();
                            }
                            cmd.Parameters.AddWithValue("@idNumber", activationRequest.idNumber);
                            using (SqlDataReader sqlReader = cmd.ExecuteReader())
                            {
                                if (sqlReader.HasRows)
                                {

                                    string SQL1 = string.Format(@"SELECT * FROM [AGENCY BANKING].[dbo].[Agency Members] WHERE [ID No]= @idNumber");
                                    SqlCommand cmd1 = new SqlCommand(SQL1, connToSMobileDb);
                                    cmd1.Parameters.AddWithValue("@idNumber", activationRequest.idNumber);
                                    if (connToSMobileDb.State != ConnectionState.Open)
                                    {
                                        connToSMobileDb.Open();
                                    }
                                    using (SqlDataReader sqlReader1 = cmd1.ExecuteReader())
                                    {
                                        using (var db = new AGENCY_BANKINGEntities())
                                            if (sqlReader1.HasRows)
                                            {
                                                var a = db.Agency_Members.FirstOrDefault(b => b.ID_No == activationRequest.idNumber);
                                                if (a != null)
                                                {

                                                    activationResponse = new AccountActivationResponse
                                                        {
                                                            Success = true,
                                                            Description = "Activation Successful",
                                                            status_code = "200",
                                                            ID_No = a.ID_No,
                                                            Telephone_No = a.Telephone_No,
                                                            Date_Registered = Convert.ToDateTime(a.Date_Registered),
                                                            Corporate_No = a.Corporate_No
                                                        };
                                                        jsonresponse.StatusCode = HttpStatusCode.OK;
                                                        Resultresponse = JsonConvert.SerializeObject(activationResponse);

                                                }
                                                else
                                                {
                                                    activationResponse = new AccountActivationResponse
                                                    {
                                                        Success = false,
                                                        Description = "Member ID Number does not exist",
                                                        status_code = "400"
                                                    };
                                                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                                                    Resultresponse = JsonConvert.SerializeObject(activationResponse);
                                                }
                                            }
                                            else
                                            {
                                                activationResponse = new AccountActivationResponse
                                                {
                                                    Description = "ID Number Not Found",
                                                    status_code = "300",
                                                    Success = false
                                                };
                                                jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                                                Resultresponse = JsonConvert.SerializeObject(activationResponse);
                                            }
                                    }
                                }
                                else
                                {
                                    activationResponse = new AccountActivationResponse
                                    {
                                        Description = "ID Number Not Found",
                                        status_code = "301",
                                        Success = false
                                    };
                                    jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                    Resultresponse = JsonConvert.SerializeObject(activationResponse);
                                }
                            }
                        }

                    }
                    else
                    {
                        activationResponse = new AccountActivationResponse
                        {
                            Description = "Request Header does not exist",
                            status_code = "400",
                            Success = false
                        };

                        jsonresponse.StatusCode = HttpStatusCode.BadGateway;
                        Resultresponse = JsonConvert.SerializeObject(activationResponse);

                    }


                }
                catch (Exception e)
                {
                    successResponse = new SuccessResponse
                    {
                        Description = e.Message,
                        status_code = "500",
                        Success = false
                    };


                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                    Resultresponse = JsonConvert.SerializeObject(successResponse);
                }
            }
            else
            {
                wrongheadersResponse = new WrongHeaderResponse
                {
                    Description = "Request Headers do not exist",
                    statusCode = "403",
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
