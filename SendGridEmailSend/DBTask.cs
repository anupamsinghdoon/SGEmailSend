using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace SendGridEmailSend
{
    class DBTask
    {
        private static DataTable mDataTable = null;
        private static SqlConnection mConnection = null;
        private static SqlCommand mCommand = null;
        private static SqlDataAdapter mDataAdapter = null;
        static string GetEmailsProc = ConfigurationManager.AppSettings["GetEmailsProc"].ToString();
        public static DataTable GetEmailData(string ConnectionString)
        {
            try
            {
                mDataTable = new DataTable();
                using (mConnection = new SqlConnection(ConnectionString))
                {
                    mCommand = new SqlCommand();
                    mCommand.Connection = mConnection;
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandText = GetEmailsProc;
                    mDataAdapter = new SqlDataAdapter(mCommand);
                    mDataAdapter.Fill(mDataTable);
                    Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"Get_Send_Email Process Rows: {mDataTable.Rows.Count}");
                    return mDataTable;
                }
            }
            catch (Exception ex)
            {
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), "Error: " + ex.Message.ToString());
                return mDataTable;
            }
        }

        
       

        public static void Send_Email_History(string ConnectionString, string JSon_Request, string JSon_Response, string Status_Code, SentLetter item, string SubjectLine)
        {
            try
            {
               
                var ACTMessageId =Convert.ToString(((JObject)JsonConvert.DeserializeObject(item.custom_args_json))["ACTMessageId"]);

                using (mConnection = new SqlConnection(ConnectionString))
                {
                    mCommand = new SqlCommand();
                    mCommand.Connection = mConnection;
                    mConnection.Open();
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandText = "Insert_Send_Email_History";
                    mCommand.Parameters.AddWithValue("@Entity", item.Entity);
                    mCommand.Parameters.AddWithValue("@FileName", item.FileName);
                    mCommand.Parameters.AddWithValue("@Namespace_Code", item.Namespace_Code);
                    mCommand.Parameters.AddWithValue("@Account_Num", item.Account_Number);
                    mCommand.Parameters.AddWithValue("@Client_Name", item.Client_Name);
                    mCommand.Parameters.AddWithValue("@Client_ID", item.Client_ID);
                    mCommand.Parameters.AddWithValue("@Letter_Number", item.Letter_Number);
                    mCommand.Parameters.AddWithValue("@Letter_Date", item.Letter_Date);
                    mCommand.Parameters.AddWithValue("@Email_Id", item.E_Notice_Email);
                    mCommand.Parameters.AddWithValue("@Template_Id", item.Primary_SendGrid_Template_Id);
                    mCommand.Parameters.AddWithValue("@Letter_Name", item.SendGrid_Template_Name);
                    mCommand.Parameters.AddWithValue("@JSon_Request", JSon_Request);
                    mCommand.Parameters.AddWithValue("@Status_Code", Status_Code);
                    mCommand.Parameters.AddWithValue("@JSon_Response", JSon_Response);
                    mCommand.Parameters.AddWithValue("@Group_id", item.Group_ID);
                    mCommand.Parameters.AddWithValue("@IP_Pool_Name", item.IP_Pool_Name);
                    mCommand.Parameters.AddWithValue("@From_Email", item.From_Email);
                    mCommand.Parameters.AddWithValue("@Reply_To_Email", item.Reply_To);
                    mCommand.Parameters.AddWithValue("@SendGrid_API_Key", item.SendGrid_API_Key);
                    mCommand.Parameters.AddWithValue("@ACTMessageId", ACTMessageId);
                    mCommand.ExecuteNonQuery();
                    Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"Insert_Send_Email_History | Email: {item.E_Notice_Email} | Template_Id:{item.Primary_SendGrid_Template_Id} | Letter_Name:{item.SendGrid_Template_Name} | Letter_Date:{ item.Letter_Date} | Namespace_Code:{item.Namespace_Code} | Account_Num:{item.Account_Number} | Client_Name:{item.Client_Name} | Client_ID:{item.Client_ID} | Letter_Number:{item.Letter_Number} ");
                }
            }
            catch (Exception ex)
            {
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), "Error: " + ex.Message.ToString());
            }
        }

        public static void Update_Send_Email(string ConnectionString, SentLetter item)
        {
            try
            {
                using (mConnection = new SqlConnection(ConnectionString))
                {
                    mCommand = new SqlCommand();
                    mCommand.Connection = mConnection;
                    mConnection.Open();
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.CommandText = "Update_Send_Email";
                    mCommand.Parameters.AddWithValue("@Entity", item.Entity);
                    mCommand.Parameters.AddWithValue("@Namespace_Code", item.Namespace_Code);
                    mCommand.Parameters.AddWithValue("@Account_Num", item.Account_Number);
                    mCommand.Parameters.AddWithValue("@Client_Name", item.Client_Name);
                    mCommand.Parameters.AddWithValue("@Letter_Number", item.Letter_Number);

                    mCommand.ExecuteNonQuery();
                    Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"Update_Send_Email | Entity: {item.Entity} | Namespace_Code: {item.Namespace_Code} | Account_Num:{item.Account_Number} | Client_Name:{item.Client_Name} | Letter_Number:{item.Letter_Number}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), "Error: " + ex.Message.ToString());
            }
        }

    }
}
