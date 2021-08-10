using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for LordMayer
/// </summary>
public class LordMayer
{
    static DataStore dh = new DataStore();
    public LordMayer()
    {
        //
        // TODO: Add constructor logic here       
    }
    public static string encryptData(string input)

    {

        // step 1, calculate MD5 hash from input
        //SHA1
        MD5 md5 = System.Security.Cryptography.MD5.Create();

        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

        byte[] hash = md5.ComputeHash(inputBytes);


        // step 2, convert byte array to hex string

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < hash.Length; i++)

        {

            sb.Append(hash[i].ToString("X2"));

        }

        return sb.ToString();

    }

    public static string getActivePeriod()
    {
        string res = "";
        DataTable dt = dh.getActiviePeriod();
        foreach (DataRow row in dt.Rows)
        {
            res = row["Period"].ToString();
        }
        return res;
    }
    public static DateTime getDate(string date, int type)
    {
        DateTime res = DateTime.Now;
        if (type == 0)
        {
            if (date.Equals(""))
            {
                res = DateTime.Parse("2016-01-01 00:00:00");
            }
            else
            {
                res = DateTime.Parse(date);
            }
        }
        else if (type == 1)
        {
            if (date.Equals(""))
            {
                res = DateTime.Now;
            }
            else
            {
                res = DateTime.Parse(date);
            }
        }
        return res;
    }
    public static Boolean isValidApi(string vendor, string api, string password)
    {
        Boolean result = false;
        DataTable dt = dh.DT_validApi(vendor, api, password);
        if (dt.Rows.Count > 0)
        {
            result = true;
        }
        return result;
    }

    public static string getCustPropRef(string custref)
    {
        String res = "";
        DataTable cust = dh.LWC_getCustomers(custref, "", "0", "0", "0", "0");
        foreach (DataRow row in cust.Rows)
        {
            res = row["PropertyRef"].ToString();
        }

        return res;
    }
    public static Boolean IsValidIP(string ip)
    {
        Boolean result = false;
        DataTable dt = dh.EW_getIPs(ip);
        if (dt.Rows.Count > 0)
        {
            result = true;
        }
        return result;
    }
}