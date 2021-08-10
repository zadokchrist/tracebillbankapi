using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Customer
/// </summary>
public class OnlinePayment
{
    public OnlinePayment()
    {


    }
    public string custref { get; set; }
    public string custname { get; set; }
    public string transid { get; set; }
    public string vendor { get; set; }
    public string zone { get; set; }
    public string type { get; set; }
    public double amount { get; set; }
    public string signature { get; set; }
    public string propref { get; set; }
    public string IP { get; set; }
    public string zoneCode { get; set; }
    public string reconciled { get; set; }
    public string description { get; set; }
    public DateTime date { get; set; }
    public string paymentmethod { get; set; }
    public string testData { get; set; }
    public string category { get; set; }

}