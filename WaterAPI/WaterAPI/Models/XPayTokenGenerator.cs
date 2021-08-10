using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for XPayTokenGenerator
/// </summary>
public class XPayTokenGenerator
{
    public XPayTokenGenerator()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static string GetTimestamp()
    {
        long timeStamp = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000;
        return timeStamp.ToString();
    }

    public static string GetHash(string data, string sharedSecret)
    {
        var hashString = new HMACSHA256(Encoding.ASCII.GetBytes(sharedSecret));
        var hashbytes = hashString.ComputeHash(Encoding.ASCII.GetBytes(data));
        string digest = String.Empty;

        foreach (byte b in hashbytes)
        {
            digest += b.ToString("x2");
        }

        return digest;
    }

}