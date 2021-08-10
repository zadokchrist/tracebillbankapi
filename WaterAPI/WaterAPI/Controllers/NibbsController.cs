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
using System.Xml;

namespace WebApplication1.Controllers
{
    public class NibbsApiController : Controller
    {
        DataStore dh = new DataStore();
        SendMail mail = new SendMail();
        StringBuilder sb = new StringBuilder();
        public ActionResult Index()
        {
            VerifyCust cust = new VerifyCust();
            String result = "";
            string res = "<?xml version='1.0' encoding='UTF-8' standalone='yes'?><Customer>Test Data</Customer>", xml = "";
            String ipaddress = GetUserIP().Trim();
            Boolean ipvaild = LordMayer.IsValidIP(ipaddress);

            if ((Request.RequestType == "POST"))
            {
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    xml = reader.ReadToEnd();
                    // read the stream here using reader.ReadLine() and do your stuff.
                }

                string requestCode = "";



                if (xml.Contains("ValidationRequest"))
                {
                    requestCode = "ValidationRequest";
                    res = handleValidation(xml);
                }
                else if (xml.Contains("NotificationRequest"))
                {
                    requestCode = "NotificationRequest";
                    res = handlePayments(xml);
                }

                else
                {
                    //Response.Write(xml + " data posted");
                    res = xml;
                }
                dh.saveRequest("7", xml, requestCode + ipaddress,ipaddress);

            }

            return Content(res, "application/xml");
        }
        protected string handleValidation(String xml)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='UTF-8' standalone='yes'?>");
            sb.Append("<ValidationResponse>");
            // sb.Append("<BillerID>286</BillerID>");
            // sb.Append("<NextStep>0</NextStep>");

            string xpath = "/ValidationRequest";
            var nodes = xmlDoc.SelectNodes(xpath);

            foreach (XmlNode xn in nodes)
            {

                string SourceBankName = xn["SourceBankName"].InnerText.Trim();
                string SourceBankCode = xn["SourceBankCode"].InnerText.Trim();
                string InstitutionCode = xn["InstitutionCode"].InnerText.Trim();
                string ChannelCode = xn["ChannelCode"].InnerText.Trim();
                string StepCount = xn["StepCount"].InnerText.Trim();
                string CustomerName = xn["CustomerName"].InnerText.Trim();
                string CustomerAccountNumber = xn["CustomerAccountNumber"].InnerText.Trim();

                string CustReference = xn["CustomerAccountNumber"].InnerText.Trim();
                sb.Append("<BillerID>286</BillerID>");
                sb.Append("<NextStep>0</NextStep>");
                string xpath2 = "/ValidationRequest/Param";
                var nodes2 = xmlDoc.SelectNodes(xpath2);

                foreach (XmlNode xn2 in nodes2)
                {
                    string Key = xn2["Key"].InnerText.Trim();
                    string Value = xn2["Value"].InnerText.Trim();
                    if(Key.Equals("CustRef"))
                    {
                        CustReference = Value;
                    }
                }


                /*sb.Append("<BillerName>Lagos State Water Corporation</BillerName>");
                sb.Append("<SourceBankCode>"+SourceBankCode+"</SourceBankCode>");
                sb.Append(" <SourceBankName>"+SourceBankName+"</SourceBankName>");
                sb.Append("<InstitutionCode>999032</InstitutionCode>");
                sb.Append("<ChannelCode>02</ChannelCode>");
                sb.Append("<Step>1</Step>");
                sb.Append("<StepCount>2</StepCount>");
                sb.end("<CustomerName>"+ CustomerName + "</CustomerName>");
                sb.Append("<CustomerAccountNumber>0294632029</CustomerAccountNumber>");
                sb.Append("<ProductID>430</ProductID>");
                sb.Append("<ProductName>Lagos State Water Corp</ProductName>");
                sb.Append("<Amount>0.00</Amount>");
                */
                DataTable dt = dh.LWC_IsValidCustomers(CustReference);
                if (dt.Rows.Count == 0)
                {
                    //sb.Append("<CustRef>"+CustReference+"</CustRef>");
                    sb.Append("<ResponseCode>03</ResponseCode>");
                    sb.Append("<Param><Key>CustRef</Key><Value>" + CustReference + "</Value></Param>");

                    sb.Append("<Param><Key>amount</Key><Value>43000.00</Value></Param>");
                    sb.Append("<Param><Key>Phone Number</Key><Value>08033333333</Value></Param>");
                    sb.Append("<Param><Key>Email</Key><Value>seuncharlesuche@yahoo.com</Value></Param>");
                    sb.Append("<Param><Key>Name</Key><Value></Value></Param>");

                    sb.Append("<Param><Key>Status</Key><Value>InValid</Value></Param>");
                }

                else
                {//valid customer
                    foreach (DataRow row in dt.Rows)
                    {
                        string custname = row["cust_fullname"].ToString();
                        string custref = row["CustRef"].ToString();
                        string propref = row["PropertyRef"].ToString();

                        // sb.Append("<CustRef>"+custref+"</CustRef>");
                        sb.Append("<ResponseCode>00</ResponseCode>");
                        String acc = row["AccountBalance"].ToString();
                        if (acc.Equals(""))
                        {
                            acc = "0";
                        }
                        String balance = String.Format("{0:.##}", Double.Parse(acc));
                        custref = "0294632029";
                        sb.Append("<Param><Key>CustRef</Key><Value>" + custref + "</Value></Param>");

                        sb.Append("<Param><Key>amount</Key><Value>" + balance + "</Value></Param>");
                        sb.Append("<Param><Key>Phone Number</Key><Value>08033333333</Value></Param>");
                        sb.Append("<Param><Key>Email</Key><Value>seuncharlesuche@yahoo.com</Value></Param>");
                        sb.Append("<Param><Key>Name</Key><Value>" + custname + "</Value></Param>");
                        sb.Append("<Param><Key>Status</Key><Value>Valid</Value></Param>");

                    }
                }
            }
            sb.Append("</ValidationResponse>");
            // sb.Append(xml);
            return sb.ToString();
        }
        protected string handlePayments(String xml)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='UTF-8' standalone='yes'?>");
            sb.Append("<NotificationResponse>");
            sb.Append("<SessionID>286</SessionID>");
            sb.Append("<BillerID>787</BillerID>");
            string xpath = "/NotificationRequest";
            var nodes = xmlDoc.SelectNodes(xpath);

            foreach (XmlNode xn in nodes)
            {
                //string bank = xn["SourceBankName"].InnerText.Trim();
                string CustRef = xn["CustomerAccountNumber"].InnerText.Trim();
                string TransactionReference = xn["PaymentReference"].InnerText.Trim();
                string Amount = xn["Amount"].InnerText.Trim();
                string CustomerName = xn["Amount"].InnerText.Trim();
                string TransactionApprovalDate = xn["TransactionApprovalDate"].InnerText.Trim();

                String dataToSign = CustRef + CustomerName + Amount;
                String SharedSecret = "7d3nuv";
                String digsignature = XPayTokenGenerator.GetHash(dataToSign, SharedSecret);
                String vendor = "10";
                DataTable dt = dh.LWC_IsValidCustomers(CustRef);
                foreach (DataRow row in dt.Rows)
                {
                    String acc = row["AccountBalance"].ToString();
                    string custname = row["cust_fullname"].ToString();
                    string custref = row["CustRef"].ToString();
                    string propref = row["PropertyRef"].ToString();

                    string zoneCode = row["ZoneCode"].ToString();
                    string zone = row["ZoneName"].ToString();


                    OnlinePayment money = new OnlinePayment();
                    money.amount = Double.Parse(Amount);
                    money.custref = CustRef;
                    money.signature = digsignature;
                    money.custname = CustomerName;
                    money.transid = TransactionReference;
                    money.vendor = vendor;
                    money.type = "1";
                    money.propref = propref;
                    money.zone = zone;
                    money.zoneCode = zoneCode;
                    money.reconciled = "0";
                    money.testData = "0";
                    money.description = "Sterling Bank Plc";
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
                        money.date = DateTime.Parse(TransactionApprovalDate);
                    }
                    catch (Exception er)
                    {
                        money.date = DateTime.Now;
                    }

                    //check duplicate entry here
                    DataTable dtexits = dh.getVendorTransId(vendor, TransactionReference);
                    if (dtexits.Rows.Count > 0)
                    {
                        sb.Append("<ResponseCode>94</ResponseCode>");
                        sb.Append("<Param><Key>amount</Key><Value></Value></Param>");
                        sb.Append("<Param><Key>Phone Number</Key><Value>08033333333</Value></Param>");
                        sb.Append("<Param><Key>Email</Key><Value>seuncharlesuche@yahoo.com</Value></Param>");
                        sb.Append("<Param><Key>Name</Key><Value></Value></Param>");

                        sb.Append("<Param><Key>Status</Key><Value>Duplicate Transaction</Value></Param>");

                    }
                    else
                    {

                        if (dh.LWC_OnlinePayment(money))
                        {
                            sb.Append("<ResponseCode>00</ResponseCode>");
                            sb.Append("<Param><Key>Status</Key><Value>Payment Successfully Received by Lagos Water</Value></Param>");

                        }
                    }

                    sb.Append("<Param><Key>amount</Key><Value>" + Amount + "</Value></Param>");
                    sb.Append("<Param><Key>Phone Number</Key><Value></Value></Param>");
                    sb.Append("<Param><Key>Email</Key><Value></Value></Param>");
                    sb.Append("<Param><Key>Name</Key><Value>" + custname + "</Value></Param>");

                }
            }
            sb.Append("</NotificationResponse>");
            // sb.Append(xml);
            return sb.ToString();
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
