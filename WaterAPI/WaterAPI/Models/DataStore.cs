using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

public class DataStore
{
    private DbCommand command;
    DataTable returnData;
    Database BILLINGDB, EpaymentDB, PROCDB;
    public DataStore()
    {
        try
        {
            BILLINGDB = DatabaseFactory.CreateDatabase("BILLING");
            EpaymentDB = DatabaseFactory.CreateDatabase("BILLING");
            PROCDB = DatabaseFactory.CreateDatabase("BILLING");
            // BILLINGDB = DatabaseFactory.CreateDatabase("Training");
            // EpaymentDB = DatabaseFactory.CreateDatabase("PaymentsTest");
        }
        catch (Exception er)
        {
            Console.WriteLine("Error");
        }
    }
    public DataTable LWC_getCustomers(string cref, string prop, string zone, string street, string tech, string classid)
    {
        try
        {
            command = BILLINGDB.GetStoredProcCommand("LWC_getCustomers", cref, prop, zone, street, tech, classid);
            returnData = BILLINGDB.ExecuteDataSet(command).Tables[0];
            return returnData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable LWC_IsValidCustomersEP(string cref)
    {
        try
        {
            command = BILLINGDB.GetStoredProcCommand("Sp_CheckCustomerDetails", cref);
            returnData = BILLINGDB.ExecuteDataSet(command).Tables[0];
            return returnData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable getVendorTransId(string vendor, string transid)
    {
        try
        {
            command = EpaymentDB.GetStoredProcCommand("sp_GetTransactionByVendor", vendor, transid);
            returnData = EpaymentDB.ExecuteDataSet(command).Tables[0];
        }
        catch (Exception ex)
        {

        }
        return returnData;
    }
    public DataTable LWC_IsValidCustomers(string cref)
    {
        try
        {
            command = BILLINGDB.GetStoredProcCommand("Sp_CheckCustomerDetails", cref);
            returnData = BILLINGDB.ExecuteDataSet(command).Tables[0];
            return returnData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable DT_validApi(string vendor, string api, string password)
    {
        try
        {
            command = EpaymentDB.GetStoredProcCommand("Ugrade_getVendor", vendor, api, password);
        }
        catch (Exception ex)
        {

        }
        returnData = EpaymentDB.ExecuteDataSet(command).Tables[0];
        return returnData;
    }
    public DataTable getActiviePeriod()
    {
        try
        {
            command = BILLINGDB.GetStoredProcCommand("getActivePeriod");
            returnData = BILLINGDB.ExecuteDataSet(command).Tables[0];
            return returnData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable getVendors(string reference)
    {
        try
        {
            command = EpaymentDB.GetStoredProcCommand("getVendors", reference);
            returnData = EpaymentDB.ExecuteDataSet(command).Tables[0];
        }
        catch (Exception ex)
        {

        }
        return returnData;
    }
    public Boolean LWC_OnlinePayment
    (OnlinePayment money)
    {
        Boolean result = false;
        try
        {
            //check if this already exists

            command = EpaymentDB.GetStoredProcCommand("Sp_SavePaymentTransaction",
             money.custref, money.custname, money.date, money.amount, money.type,
             money.transid, money.vendor, money.paymentmethod, money.zone,  money.testData);
            if (EpaymentDB.ExecuteNonQuery(command) > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

        }
        catch (Exception er)
        {
            //throw er;
            result = false;
        }


        return result;
    }
    public Boolean saveRequest(string vendor, string resource, string response,string ip)
    {
        Boolean result = false;

        try
        {
            //check if this already exists
            command = EpaymentDB.GetStoredProcCommand("SaveRequests", vendor, resource, response,ip);
            if (EpaymentDB.ExecuteNonQuery(command) > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
        }
        catch (Exception er)
        {
            throw er;
        }


        return result;
    }
    public DataTable EW_getIPs(string ip)
    {
        try
        {
            command = EpaymentDB.GetStoredProcCommand("EW_getIPs", ip);
            returnData = EpaymentDB.ExecuteDataSet(command).Tables[0];
        }
        catch (Exception ex)
        {

        }
        return returnData;
    }
    
         public DataTable LWC_OfflineData(string cref)
    {
        try
        {
            command = BILLINGDB.GetStoredProcCommand("LWC_OfflineData", cref);
            returnData = BILLINGDB.ExecuteDataSet(command).Tables[0];
            return returnData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable LWC_IsValidPaymentRerence(string cref)
    {
        try
        {
            command = BILLINGDB.GetStoredProcCommand("LWC_IsValidPaymentRerence", cref);
            returnData = BILLINGDB.ExecuteDataSet(command).Tables[0];
            return returnData;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}

