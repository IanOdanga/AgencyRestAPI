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
    public class ChangePasswordController : ApiController
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
            ChangePasswordRequest changePasswordRequest = null;
            ChangePasswordResponse changePasswordResponse = null;
            SuccessResponse successResponse = null;
            SqlConnection connToSMobileDb = null;
            if (headers.Contains("Accept"))
            {
                try
                {
                    string response = string.Empty;
                    var jsonContent = await Request.Content.ReadAsStringAsync();
                    changePasswordRequest = JsonConvert.DeserializeObject<ChangePasswordRequest>(jsonContent);

                    if (changePasswordRequest != null)
                    {
                        string session_id = changePasswordRequest.session_id;
                        if (session_id != null)
                        {
                            using (connToSMobileDb = new SqlConnection("Data Source=.;Initial Catalog=S-Mobile;User ID=mobile;Password=Mw@mba-J1w3%;MultipleActiveResultSets=True"))
                            {
                                string SQL = "UPDATE [AGENCY BANKING].[dbo].[Agents] SET [Password] = @new_password WHERE [Agent Code] = @agent_code AND [Password] = @old_password AND [Active]=1";
                                using (SqlCommand cmd = new SqlCommand(SQL, connToSMobileDb))
                                {
                                    if (connToSMobileDb.State != ConnectionState.Open)
                                    {
                                        connToSMobileDb.Open();
                                    }
                                    cmd.Parameters.AddWithValue("@new_password", changePasswordRequest.new_password);
                                    cmd.Parameters.AddWithValue("@agent_code", changePasswordRequest.agent_code);
                                    cmd.Parameters.AddWithValue("@old_password", changePasswordRequest.old_password);
                                    int rst = cmd.ExecuteNonQuery();

                                    if (rst > 0)
                                    {
                                        changePasswordResponse = new ChangePasswordResponse
                                        {
                                            status_code = "200",
                                            Description = "Password Changed Successfully",
                                            Success = true
                                        };
                                        jsonresponse.StatusCode = HttpStatusCode.OK;
                                        Resultresponse = JsonConvert.SerializeObject(changePasswordResponse);
                                    }
                                    else
                                    {
                                        changePasswordResponse = new ChangePasswordResponse
                                        {
                                            status_code = "401",
                                            Description = "Wrong Agent Code or Password provided",
                                            Success = false
                                        };
                                        jsonresponse.StatusCode = HttpStatusCode.Unauthorized;
                                        Resultresponse = JsonConvert.SerializeObject(changePasswordResponse);
                                    }
                                }
                            }
                        }
                        else
                        {
                            changePasswordResponse = new ChangePasswordResponse
                            {
                                status_code = "400",
                                Description = "Request Parameters missing",
                                Success = false
                            };
                            jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                            Resultresponse = JsonConvert.SerializeObject(changePasswordResponse);
                        }

                    }
                    else
                    {
                        changePasswordResponse = new ChangePasswordResponse
                        {
                            status_code = "400",
                            Description = "Bad Request",
                            Success = false
                        };
                        jsonresponse.StatusCode = HttpStatusCode.BadRequest;
                        Resultresponse = JsonConvert.SerializeObject(changePasswordResponse);
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
