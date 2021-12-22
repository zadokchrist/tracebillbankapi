using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WaterAPI.Models;
using System.Web;
using System.Net.Http.Headers;
using System.Text;
using System.Data;
using Newtonsoft.Json;

namespace WaterAPI.Controllers
{
    //[RoutePrefix("api/books")]

  
    public class CustomerController : ApiController
    {

        DataStore dh = new DataStore();
        Customer customer = new Customer();
        PaymentsResponse reponse = new PaymentsResponse();
        SendMail mail = new SendMail();

        [Route("api/verifyCustomer")]
        [HttpPost]
 
        public async Task<VerifyCust> verifyCustomer([FromBody] VerificationData data)
        {
            VerifyCust cust = new VerifyCust();
            if (data == null)
            {
                cust.title = "Wrong Request";
                cust.data =customer;
            }
            else
            {


               // string result = await Request.Content.ReadAsStringAsync();        
                var request = HttpContext.Current.Request;
                var authHeader = request.Headers["Authorization"];
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
                                DataTable dt = dh.LWC_IsValidCustomers(data.CustRef);
                                if (dt.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        customer.AccountBalance = row["AccountBalance"].ToString();
                                        customer.CustRef = row["CustRef"].ToString();
                                        customer.CustName = row["cust_fullname"].ToString();
                                        customer.PropRef = row["PropertyRef"].ToString();
                                        customer.ZoneName = row["ZoneName"].ToString();

                                    }
                                    cust.title = "success";
                                    cust.data = customer;
                                }
                                else
                                {
                                    cust.title = "invalid customer";
                                    cust.data = customer;
                                }
                            }
                            else
                            {
                                //prepare a response
                                cust.title = "invalid vendor";
                                cust.data = customer;
                            }
                        }
                        catch(Exception er)
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
            return cust;
        }

        //post the data now this is payments data
        [Route("api/postPayment")]
        [HttpPost]

        public async Task<PaymentsResponse> postPayment([FromBody] PaymentData data)
        {
            PaymentsResponse cust = new PaymentsResponse();
            string result = await Request.Content.ReadAsStringAsync();
            String res="";
            if (data == null)
            {
                cust.Description = "Wrong Request Please check Input Values some mandatory Fields are Missing";
                cust.ResponseCode = "01";
                res = result;
                data.VendorCode = "";
            }
            else if (data.Amount.Equals("") || Double.Parse(data.Amount)==0)
            {
                cust.Description = "Amount Missing";
                cust.ResponseCode = "02";
            }
            else if (data.VendorCode == null || data.VendorCode.Equals(""))
            {
                cust.Description = "VendorCode Missing";
                cust.ResponseCode = "12";
                data.CustRef = "";
            }
            else if (
                 data.CustRef == null
                 ||

                data.CustRef.Equals("")
                )
            {
                cust.Description = "Customer Reference Number Missing";
                cust.ResponseCode = "03";
            }
            else if (data.Date == null ||data.Date.Equals("")
               )
            {
                cust.Description = "Transaction Date Missing";
                cust.ResponseCode = "09";
            }
            else if (
                data.TransactionReference == null||
                data.TransactionReference.Equals("")
                )
            {
                cust.Description = "Transaction Reference Missing";
                cust.ResponseCode = "10";
            }
            else
            {
                res = JsonConvert.SerializeObject(data);
                // string result = await Request.Content.ReadAsStringAsync();        
                var request = HttpContext.Current.Request;
                var authHeader = request.Headers["Authorization"];
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
                            DataTable dtvendor = dh.DT_validApi(vendor, api, password);
                            //check if this payment already exists
                            DataTable dtexits = dh.getVendorTransId(vendor, data.TransactionReference);
                            if (dtexits.Rows.Count > 0)
                            {
                                cust.Description = "Payment Already Exists for Vendor";
                                cust.ResponseCode = "11";
                            }
                            else
                            {
                                if (dtvendor.Rows.Count>0)
                                {
                                   
                                    String vendorName = dtvendor.Rows[0]["VendorName"].ToString();
                                           
                                    
                                    //continue with the data
                                    DataTable dt = dh.LWC_IsValidCustomersEP(data.CustRef);
                                    if (dt.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            String propertyRef = row["PropertyRef"].ToString();
                                            String zonename = row["ZoneName"].ToString();
                                            String zoneCode = row["PropertyRef"].ToString();
                                            OnlinePayment money = new OnlinePayment();
                                            money.amount = Double.Parse(data.Amount);
                                            money.custref = data.CustRef;
                                           // money.signature = null;
                                            money.custname = data.CustName;
                                            money.transid = data.TransactionReference;
                                            money.vendor = vendor;
                                            money.type = "1";
                                            money.propref = propertyRef;
                                            money.zone = zonename;
                                            money.zoneCode = zoneCode;
                                            money.reconciled = "0";
                                            money.testData = "0";
                                            money.description = vendorName;
                                            //some smaller validations
                                            if (vendor.Equals("2"))
                                            {
                                                money.paymentmethod = "4";
                                            }
                                            else
                                            {
                                                money.paymentmethod = "1";
                                            }
                                            try
                                            {
                                                money.date = DateTime.Parse(data.Date);
                                            }
                                            catch (Exception er)
                                            {
                                                money.date = DateTime.Now;
                                            }

                                            if (!money.zoneCode.Equals(""))
                                            {

                                                if (dh.LWC_OnlinePayment(money))
                                                {
                                                    cust.ResponseCode = "00";
                                                    cust.Description = "Payment Successfully Received by Lagos Water";
                                                }
                                            }
                                         }//foreach customer (this customer)
                                        
                                        }
                                        else
                                        {
                                            cust.Description = "Invalid Customer";
                                            cust.ResponseCode = "04";
                                        }
                                }
                                else
                                {
                                    cust.Description = "Invalid Vendor";
                                    cust.ResponseCode = "05";
                                }
                            }//only if the vendor id is not a duplicate
                        }
                        catch (Exception er)
                        {
                            mail.SendEmail("icytreyrichards@yahoo.com", "Bank Code" + data.VendorCode + " Request " + cust.Description + "-" + er.ToString(), "error from lwc payments api");
                            cust.Description = "Please wait as our Engineers fix the erro or contact System Administrator";
                            cust.ResponseCode = "06";
                        }

                    }
                    else
                    {
     
                        cust.Description = "Basic Authorisation mis Match (Best example) (Authorisation:Basic ....)";
                        cust.ResponseCode = "07";
                    }
                }

                else
                {                
                    cust.Description = "Basic Authorisation Response Header Missing";
                    cust.ResponseCode = "08";

                }
            }
            //log them

            try
            {
                dh.saveRequest(data.VendorCode, res, cust.ResponseCode + "-" + cust.Description,"");
            }
            catch(Exception er)
            {

            }
            return cust;
        }

        [Route("api/GetTranStatus")]
        [HttpPost]
        public async Task<TranStatusResponse> GetTranStatus([FromBody] TranStatusRequest data) 
        {
            TranStatusResponse response = new TranStatusResponse();

            return response;
        }
    }
}
