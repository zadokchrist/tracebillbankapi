using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PaymentController : Controller
    {

        DataStore dh = new DataStore();
        Customer customer = new Customer();
        PaymentsResponse reponse = new PaymentsResponse();
        SendMail mail = new SendMail();

        // GET: Post

        public ActionResult Index(PaymentData data)
        {

            PaymentsResponse cust = new PaymentsResponse();

            string res = "";
            String ipaddress = GetUserIP().Trim();
            Boolean ipvaild = LordMayer.IsValidIP(ipaddress);
           // ipvaild = true;
            if (Request.RequestType == "GET")
            {
                Response.StatusCode = 400; // Replace .AddHeader
                var error = new Error();  // Create class Error() w/ prop
                error.ErrorID = 123;
                error.Level = 2;
                error.Message = "Bad Request Method Not Allowed";
                dh.saveRequest(data.VendorCode, "ivalid ip", "invalid ip", ipaddress);
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            if (!ipvaild)
            {
                Response.StatusCode = 401; // Replace .AddHeader
                var error = new Error();  // Create class Error() w/ prop
                error.ErrorID = 124;
                error.Level = 2;
                error.Message = "Bad Request From Unauthorised Location "+ ipaddress;
                //return UnauthorizedAccessException("");
                dh.saveRequest(data.VendorCode, "ivalid ip", "invalid ip", ipaddress);
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            else
            {

                try
                {

                    if ((Request.RequestType == "POST") && ipvaild)
                    {
                        using (StreamReader reader = new StreamReader(Request.InputStream))
                        {
                            res = reader.ReadToEnd();
                            // read the stream here using reader.ReadLine() and do your stuff.
                        }
                    }


                    if (data == null)
                    {
                        cust.Description = "Wrong Request Please check Input Values some mandatory Fields are Missing";
                        cust.ResponseCode = "01";

                        data.VendorCode = "";
                    }
                    else if (data.Amount.Equals("") || Double.Parse(data.Amount) == 0)
                    {
                        cust.Description = "Amount Missing";
                        cust.ResponseCode = "02";
                    }
                    else if (data.CustName ==null || data.CustName.Equals(""))
                    {
                        cust.Description = "Bill Payer Name Missing";
                        cust.ResponseCode = "16";
                    }
                    else if (data.VendorCode == null || data.VendorCode.Equals(""))
                    {
                        cust.Description = "VendorCode Missing";
                        cust.ResponseCode = "12";
                        data.CustRef = "";

                    }
                    else if (data.DigitalSignature == null || data.DigitalSignature.Equals(""))
                    {
                        cust.Description = "DigitalSignature Missing";
                        cust.ResponseCode = "13";
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
                    else if (data.Date == null || data.Date.Equals("")
                       )
                    {
                        cust.Description = "Transaction Date Missing";
                        cust.ResponseCode = "09";
                    }
                    else if (
                        data.TransactionReference == null ||
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
                                        if (dtvendor.Rows.Count > 0)
                                        {

                                            String vendorName = dtvendor.Rows[0]["VendorName"].ToString();


                                            String SharedSecret = dtvendor.Rows[0]["SharedSecret"].ToString();
                                            String dataToSign = data.CustRef + data.CustName + data.Amount;
                                            String digsignature = XPayTokenGenerator.GetHash(dataToSign, SharedSecret);

                                            //continue with the data
                                            DataTable dt = dh.LWC_IsValidPaymentRerence(data.CustRef);
                                            if (dt.Rows.Count > 0)
                                            {
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    String propertyRef = row["PropertyRef"].ToString();
                                                    String zonename = row["ZoneName"].ToString();
                                                    String zoneCode = row["ZoneCode"].ToString();
                                                    String InvoiceType = row["Type"].ToString().Trim();
                                                    double invoiceAmount = Convert.ToDouble(row["AccountBalance"].ToString().Trim());
                                                    OnlinePayment money = new OnlinePayment();
                                                    money.amount = Double.Parse(data.Amount);
                                                    money.custref = data.CustRef;
                                                    money.signature = data.DigitalSignature;
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

                                                    if (money.zoneCode.Equals(""))
                                                    {
                                                        cust.ResponseCode = "14";
                                                        cust.Description = "Customer has No Zone";

                                                    }
                                                    else if (!digsignature.Equals(data.DigitalSignature))
                                                    {
                                                        cust.ResponseCode = "13";
                                                        cust.Description = "Digital Signature Mismatch";
                                                    }
                                                    else
                                                    {
                                                        //check type of invoice
                                                        if (!InvoiceType.Equals("Customer") && Double.Parse(data.Amount) != invoiceAmount)
                                                        {
                                                            cust.ResponseCode = "15";
                                                            cust.Description = "Full Invoice Amount Must Paid";

                                                        }
                                                        else
                                                        {

                                                            if (dh.LWC_OnlinePayment(money))
                                                            {
                                                                cust.ResponseCode = "00";
                                                                cust.Description = "Payment Successfully Received by Lagos Water";
                                                            }
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

                 
                }
                catch (Exception er)
                {
                    mail.SendEmail("icytreyrichards@yahoo.com", "Bank Code" + data.VendorCode + " Request " + cust.Description + "-" + er.ToString(), "error from lwc payments api");
                    cust.Description = "Please wait as our Engineers fix the erro or contact System Administrator";
                    cust.ResponseCode = "06";
                }
            }
            try
            {
                dh.saveRequest(data.VendorCode, JsonConvert.SerializeObject(data), cust.ResponseCode + "-" + cust.Description, ipaddress);
            }
            catch (Exception er)
            {

            }
            return Json(cust);

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