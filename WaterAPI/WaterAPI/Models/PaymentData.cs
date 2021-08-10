using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for VerifyCust
/// </summary>
public class PaymentData
{
    public PaymentData()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string CustRef { get; set; }//mandatory
    public string CustName { get; set; }//mandatory Bill Payer Name
    public String Amount { get; set; }//mandatory
    public string Date { get; set; }//transaction date
    public string TransactionReference { get; set; }//transaction date
    public string VendorCode { get; set; }//mandatory
    public string DigitalSignature { get; set; }//mandatory
}