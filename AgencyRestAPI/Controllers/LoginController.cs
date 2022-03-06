using AgencyRestAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Text;
using RestSharp;
using System.Data.Sql;

namespace AgencyRestAPI.Controllers
{
    public class LoginController : ApiController
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
            LoginRequest loginR = null;
            SuccessResponse agentLoginFailed = null;
            SqlConnection connToSMobileDb = null;
            if (headers.Contains("Accept"))
            {
                try
                {
                    string response = string.Empty;
                    var jsonContent = await Request.Content.ReadAsStringAsync();
                    //string json -
                    loginR = JsonConvert.DeserializeObject<LoginRequest>(jsonContent);

                    if (loginR != null)
                        {
                            using (connToSMobileDb = new SqlConnection("Data Source=.;Initial Catalog=S-Mobile;User ID=mobile;Password=Mw@mba-J1w3%;MultipleActiveResultSets=True"))
                            {
                                string SQL = "SELECT * FROM [AGENCY BANKING].[dbo].[Agents] WHERE [Agent Code] = @agent_code AND [Password]=@password AND [Active]=1";
                                SqlCommand cmd = new SqlCommand(SQL, connToSMobileDb);
                                if (connToSMobileDb.State != ConnectionState.Open)
                                {
                                    connToSMobileDb.Open();
                                }
                                cmd.Parameters.AddWithValue("@agent_code", loginR.agent_code);
                                //cmd.Parameters.AddWithValue("@active", 1);
                                cmd.Parameters.AddWithValue("@password", loginR.password);
                                using (SqlDataReader sqlReader = cmd.ExecuteReader())
                                {
                                    if (sqlReader.HasRows)
                                    {

                                        string SQL1 = string.Format(@"SELECT * FROM [AGENCY BANKING].[dbo].[Agents] WHERE [Agent Code]=@agent_code AND [Password]=@password");
                                        SqlCommand cmd1 = new SqlCommand(SQL1, connToSMobileDb);
                                        cmd1.Parameters.AddWithValue("@agent_code", loginR.agent_code);
                                        cmd1.Parameters.AddWithValue("@password", loginR.password);
                                        if (connToSMobileDb.State != ConnectionState.Open)
                                        {
                                            connToSMobileDb.Open();
                                        }
                                        using (SqlDataReader sqlReader1 = cmd1.ExecuteReader())
                                        {
                                            if (sqlReader1.HasRows)
                                            {
                                                agentLoginFailed = new SuccessResponse
                                                {
                                                    Description = "Login Successful",
                                                    status_code = "200",
                                                    logged_in = true,
                                                    Success = true
                                                };
                                                jsonresponse.StatusCode = HttpStatusCode.OK;
                                                Resultresponse = JsonConvert.SerializeObject(agentLoginFailed);
                                            }
                                            else
                                            {
                                                agentLoginFailed = new SuccessResponse
                                                {
                                                    Description = "Wrong PIN.",
                                                    status_code = "300",
                                                    logged_in = false,
                                                    Success = false
                                                };
                                                jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                                Resultresponse = JsonConvert.SerializeObject(agentLoginFailed);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        agentLoginFailed = new SuccessResponse
                                        {
                                            Description = "Your account is not active. Check your sacco.",
                                            status_code = "301",
                                            logged_in = false,
                                            Success = false
                                        };
                                        jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                        Resultresponse = JsonConvert.SerializeObject(agentLoginFailed);
                                    }
                                }
                            }

                    }
                    else
                    {
                        agentLoginFailed = new SuccessResponse
                        {
                            Description = "Request Header does not exist",
                            status_code = "400",
                            logged_in = false,
                            Success = false
                        };

                        jsonresponse.StatusCode = HttpStatusCode.BadGateway;
                        Resultresponse = JsonConvert.SerializeObject(agentLoginFailed);

                    }


                }
                catch (Exception e)
                {
                    agentLoginFailed = new SuccessResponse
                    {
                        Description = e.Message,
                        status_code = "500",
                        logged_in = false,
                        Success = false
                    };


                    jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                    Resultresponse = JsonConvert.SerializeObject(agentLoginFailed);
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
