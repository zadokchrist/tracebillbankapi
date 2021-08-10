using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


public class SendMail
{
    string host_ip = "smtp.gmail.com";
    //smtpout.europe.secureserver.net
    string smtpuser = "info.lagoswater@gmail.com";
    string smtppass = "T1meL@pse";
    public SendMail()
	{
	}
    public void Alert(string Message,int Who)
    {

    }
    
    public void SendMailWithAttachment(string emailAddress, string Subject, string Message, string FailPath,string title)
    {

        try
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@lagoswater.org", title);
            message.To.Add(emailAddress);
            message.Subject = Subject;
            message.Body = Message;
            Attachment attachment = new Attachment(FailPath);
            message.Attachments.Add(attachment);
            message.BodyEncoding = System.Text.Encoding.ASCII;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient(host_ip);
            smtp.Credentials = new System.Net.NetworkCredential(smtpuser,smtppass);
            if (!String.IsNullOrEmpty(emailAddress))
            {
                smtp.Send(message);

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public Boolean SendEmail(string emailAddress, string Message, string Subject)
    {
        Boolean output =false;
        try
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@lagoswater.org", "LAGOS WATER CORPORATION");
            message.To.Add(emailAddress);
            message.Subject = Subject;
            message.Body = Message;
            message.BodyEncoding = System.Text.Encoding.ASCII;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;     
            SmtpClient smtp = new SmtpClient(host_ip);
            smtp.Credentials = new System.Net.NetworkCredential(smtpuser, smtppass);
            smtp.Port = 587;
            //smtp.Port = 995;
            //smtp.Port = 465;
            smtp.EnableSsl = true;
            smtp.Send(message);
            output = true;
        }
        catch (Exception ex)
        {
            output = false;
        }
        return output;
    }



    public string SendEmail(string emailAddress, string Message, string Subject,string sendername)
    {
        string output = "";
        try
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@nwsc.co.ug", sendername);
            message.To.Add(emailAddress);
            message.Subject = Subject;
            message.Body = Message;
            message.BodyEncoding = System.Text.Encoding.ASCII;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;
            if (host_ip.Equals(""))
            {
                host_ip = "10.0.0.10";
            }
            SmtpClient smtp = new SmtpClient(host_ip);
            smtp.Credentials = new System.Net.NetworkCredential(smtpuser, smtppass);
            smtp.Send(message);
            output = "SENT";
        }
        catch (Exception ex)
        {
            output = ex.Message;
        }
        return output;
    }
}
