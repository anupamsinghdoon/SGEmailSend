using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace SendGridEmailSend
{
    public class SentLetter
    {
        public string Entity { get; set; }
        public string Namespace_Code { get; set; }       
        public string StateCd { get; set; }        
        public string Letter_Number { get; set; }
        public string Letter_Date { get; set; }
        public string Account_Number { get; set; }
        public string FACS_Account_Num { get; set; }
        public string Client_Name { get; set; }
        public string Client_ID { get; set; }
        public string Debtor_First_Name { get; set; }        
        public string E_Notice_Email { get; set; }       
        public string Primary_SendGrid_Template_Id { get; set; }
        public string SendGrid_Template_Name { get; set; }
        public string SendGrid_API_Key { get; set; }
        public string From_Email { get; set; }
        public string From_Name { get; set; }
        public string Reply_To { get; set; }
        public string Reply_To_Name { get; set; }
        public string Group_ID { get; set; }
        public string IP_Pool_Name { get; set; }        
        public string BCC { get; set; }
        public string dynamic_template_data_json { get; set; }
        public string custom_args_json { get; set; }
        public string FileName { get; set; }
        public static List<SentLetter> LoadSentLetterList(string ConnectionString)
        {
            List<SentLetter> listLetter = new List<SentLetter>();
            try
            {
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"** start LoadSentLetterList function... ****");

                DataTable dt = new DataTable();
                dt = DBTask.GetEmailData(ConnectionString);
               
                #region "Converting data table into List of Class SentLetter"
                foreach (DataRow row in dt.Rows)
                {

                    listLetter.Add(new SentLetter
                    {
                        Entity = Convert.ToString(row["Entity"]),
                        Namespace_Code = Convert.ToString(row["Namespace_Code"]),
                        FileName = Convert.ToString(row["Source_File"]),
                        StateCd = Convert.ToString(row["State"]),

                        Letter_Number = Convert.ToString(row["Letter_Number"]),
                        Letter_Date = Convert.ToString(row["Letter_Date"]),
                        Account_Number = Convert.ToString(row["FACS_Account_Num"]),
                        FACS_Account_Num = Convert.ToString(row["FACS_Account_Num"]),

                        Client_ID = Convert.ToString(row["Client_ID"]),
                        Client_Name = Convert.ToString(row["Client_Name"]),

                        E_Notice_Email = Convert.ToString(row["email_Id"]),
                        Primary_SendGrid_Template_Id = Convert.ToString(row["Primary_SendGrid_Template_Id"]).Trim(),

                        SendGrid_Template_Name = Convert.ToString(row["SendGrid_Template_Name"]),
                        SendGrid_API_Key = Convert.ToString(row["SendGrid_API_Key"]),
                        From_Email = Convert.ToString(row["From_Email"]),
                        From_Name = Convert.ToString(row["From_Name"]),
                        Reply_To = Convert.ToString(row["Reply_To"]),
                        Reply_To_Name = Convert.ToString(row["Reply_To_Name"]),
                        IP_Pool_Name = Convert.ToString(row["IP_Pool_Name"]),
                        BCC = Convert.ToString(row["BCC"]),
                        Group_ID = "",
                        dynamic_template_data_json = Convert.ToString(row["dynamic_template_data_json"]),
                        custom_args_json = Convert.ToString(row["custom_args_json"])
                    });

                }
                #endregion
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"** End LoadSentLetterList function... ****");
                
            }
            catch(Exception ex)
            {
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"----------------------Error Captured: {DateTime.Now} ------------------------------");
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.Message}");
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.StackTrace}");
            }
            return listLetter;

        }
       
    }
}
