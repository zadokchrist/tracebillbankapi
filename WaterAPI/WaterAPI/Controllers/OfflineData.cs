using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class OfflineDataController : Controller
    {

        /*{"CustRef":"NC/01/102","VendorCode":2,"Amount":"11000","Date":"2019-06-30","DigitalSignature":"3c7c50c4af1ece3f0887b3faedc1ee89cdd7218f588538f64c660429e9cd5a72","TransactionReference":"46899639","CustName":"Diane"}*/
        DataStore dh = new DataStore();
        Customer customer = new Customer();
        PaymentsResponse reponse = new PaymentsResponse();
        SendMail mail = new SendMail();

        // GET: Post

        public ActionResult Index(VerificationData data)
        {
            VerifyCust cust = new VerifyCust();
            String result = "";
            string res = "";
            String ipaddress = GetUserIP().Trim();
            Boolean isvalid = LordMayer.IsValidIP(ipaddress);
            DataTable dt=null;
            if (!isvalid)
            {
                Response.StatusCode = 400; // Replace .AddHeader
                var error = new Error();  // Create class Error() w/ prop
                error.ErrorID = 123;
                error.Level = 2;
                error.Message = "Bad Request invalid IP" + ipaddress;
            }
            else
            {
                if (Request.RequestType == "GET")
                {
                    Response.StatusCode = 400; // Replace .AddHeader
                    var error = new Error();  // Create class Error() w/ prop
                    error.ErrorID = 123;
                    error.Level = 2;
                    error.Message = "Bad Request Method Not Allowed";

                    return Json(error, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        if ((Request.RequestType == "POST"))
                        {
                            using (StreamReader reader = new StreamReader(Request.InputStream))
                            {
                                result = reader.ReadToEnd();
                                // read the stream here using reader.ReadLine() and do your stuff.
                            }
                        }
                        if (data == null)
                        {
                            cust.title = "Wrong Request";
                            cust.data = customer;
                        }
                        else if (data.CustRef == null || data.CustRef.Equals(""))
                        {
                            cust.title = "CustRef Missing";
                            cust.data = customer;
                        }
                        else if (data.VendorCode == null || data.VendorCode.Equals(""))
                        {
                            cust.title = "VendorCode Missing";
                            cust.data = customer;
                        }
                        else
                        {

                            var authHeader = Request.Headers["Authorization"];
                            if (authHeader != null)
                            {
                                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                                if (authHeaderVal.Scheme.Equals("basic",
                                        StringComparison.OrdinalIgnoreCase) &&
                                    authHeaderVal.Parameter != null)
                                {
                                    string auth = authHeaderVal.Parameter;
                                    var credentialBytes = Convert.FromBase64String(auth);
                                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                                    var api = credentials[0];
                                    var password = credentials[1];
                                    string vendor = data.VendorCode;
                                    //
                                    try
                                    {
                                        Boolean isvalidconnection = LordMayer.isValidApi(vendor, api, password);
                                        if (isvalidconnection)
                                        {
                                            //continue with the data
                                             dt = dh.LWC_OfflineData(data.CustRef);
                                            cust.title = "Offline data pull successfully";
                                            
                                           
                                        }
                                        else
                                        {
                                            //prepare a response
                                            cust.title = "invalid vendor details ";// +VendorCode+"Api "+api+" password "+password;
                                            cust.data = customer;
                                        }
                                    }
                                    catch (Exception er)
                                    {
                                        cust.title = "Please wait as our Engineers fix the erro or contact System Administrator";
                                        cust.data = customer;
                                    }

                                }
                                else
                                {
                                    cust.title = "Basic Authorisation mis Match (Best example) (Authorisation:Basic ....)";
                                    cust.data = customer;
                                }
                            }

                            else
                            {

                                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                                response.Content = new StringContent("Authorisation Header Missing");
                                var message = "Authorisation Header Missing";
                                HttpError err = new HttpError(message);
                                //return Unauthorized(val);
                                //return response.ToString();
                                cust.title = "Basic Authorisation Response Header Missing";
                                cust.data = customer;
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        cust.title = "Please wait as our Engineers fix the erro or contact System Administrator";
                        cust.data = customer;
                    }
                }
            }
            dh.saveRequest(data.VendorCode, result, cust.title,ipaddress);
            return Json(dt);

        }
        private string GetUserIP()
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }
            else
            {
                return Request.ServerVariables["REMOTE_ADDR"];
            }
        }
    }
}
 