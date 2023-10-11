using SendGrid;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace SendGridEmailSend
{
    class Program
    {
        static string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        static string Send_Email_Flag = ConfigurationManager.AppSettings["Send_Email"].ToString();
        static string GetEmailsProc = ConfigurationManager.AppSettings["GetEmailsProc"].ToString();
        static void Main(string[] args)
        {
            Execute();
        }
        static void Execute()
        {
            try
            {
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"                        ");
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"********************* Start Send Email function {GetEmailsProc} ...{DateTime.Now} **************************");
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $" Connection String {ConnectionString} **************************");

                List<SentLetter> listLetter = SentLetter.LoadSentLetterList(ConnectionString); //Get the List of all the emails we need to send today.
                foreach (var letter in listLetter)
                {
                    Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"Row: {listLetter.IndexOf(letter)} ----------------------------------------------");
                    Send_Email(letter).Wait();// Send the email.
                }
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"********************* End Send Email function {GetEmailsProc} ****************************************");
            }
            catch (Exception ex)
            {                
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"----------------------Error Captured: {DateTime.Now} ------------------------------");
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.Message}");
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.StackTrace}");

            }
        }
        private static async Task Send_Email(SentLetter item)
        {
            try
            {
                if (item.E_Notice_Email != string.Empty)
                {
                    Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"Calling SendGridClient() with parameter SendGrid_API_Key:{item.SendGrid_API_Key.ToString()}");
                    var client = new SendGridClient(item.SendGrid_API_Key);
                    EmailSchema emailData = EmailSchema.LoadEmailData(item);
                    var jsonString = Serialize.ToJson(emailData);
                    Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"Json Request for Sendgrid: {jsonString.ToString()}");
                    string SGStatusCode = "", Result = string.Empty;
                    dynamic response = "";
                    if (Convert.ToString(item.Primary_SendGrid_Template_Id).Trim() != string.Empty)
                    {
                        if (Send_Email_Flag.ToUpper() == "TRUE")        // Production Mode send email.
                        {
                            response = await client.RequestAsync(SendGridClient.Method.POST, jsonString, urlPath: "mail/send");
                            SGStatusCode = Convert.ToString(response.StatusCode);
                            if (SGStatusCode.ToUpper().Trim() == "ACCEPTED")
                            {
                                DBTask.Update_Send_Email(ConnectionString, item);
                            }
                        }
                        else                                           // Test Mode email not being sent.
                        {
                            response = null;
                            SGStatusCode = "Test Mode, Email not being sent";
                        }

                    }
                    else
                    {
                        response = null;
                        SGStatusCode = "BadRequest";
                    }
                    Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"SendGrid Response:{Result.ToString()} | Status_Code: {SGStatusCode}");
                    if (Send_Email_Flag.ToUpper() == "TRUE")        // Production Mode send email.
                    {
                        DBTask.Send_Email_History(ConnectionString, jsonString, Result.ToString(), SGStatusCode, item, emailData.Personalizations[0].Subject);
                    }
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"----------------------Error Captured: {DateTime.Now} ------------------------------");
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.Message}");
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.StackTrace}");
            }
        }


    }
}
