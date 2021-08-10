using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for VerifyCust
/// </summary>
public class PaymentsResponse
{
    public PaymentsResponse()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string ResponseCode { get; set; }//mandatory
    public string Description { get; set; }//mandatory

}