using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendGridEmailSend
{
    public class TemplateInfo
    {
        public string Primary_SendGrid_Template_Id { get; set; }
        public string SendGrid_Template_Name { get; set; }
        public string SettlementAllowed { get; set; }
        public int PercentageSettlement { get; set; }
        public string Client_Name { get; set; }
        public string Client_Class { get; set; }        
        public string Letter_Number { get; set; }
        public string SendGrid_API_Key { get; set; }
        public string From_Email { get; set; }
        public string From_Name { get; set; }
        public string Reply_To { get; set; }
        public string Reply_To_Name { get; set; }
        public string Group_ID { get; set; }
        public string IP_Pool_Name { get; set; }
        public string Settlement_Amount { get; set; }
        public string LastDigitOfFacsAcct { get; set; }
        public string Secondary_SendGrid_Template_Id { get; set; }
        public string Secondary_SendGrid_Template_Name { get; set; }
        public string Payment_URL { get; set; }
        public string BCC { get; set; }

        public TemplateInfo(string ConnectionString, SentLetter item)
        {
            this.Client_Name = item.Client_Name.Trim();
            this.Letter_Number = item.Letter_Number;
            this.Client_Class = item.Client_Class_Code;
            DBTask.GetLettersTemplateId(ConnectionString, this);
            this.Settlement_Amount = "$" + Convert.ToString(Math.Round(Convert.ToDecimal(item.Total_Balance_Ind) * this.PercentageSettlement / 100, 2));            
        }
        public void DecideTemplate(SentLetter item)
        {
            string LastDigitAccountNumber = item.Account_Number.Substring(item.Account_Number.Length - 1, 1);
            bool IsB = this.LastDigitOfFacsAcct.Contains(LastDigitAccountNumber);
            if (IsB && (!String.IsNullOrEmpty(Secondary_SendGrid_Template_Id)))
            {
                this.Primary_SendGrid_Template_Id = this.Secondary_SendGrid_Template_Id;
                this.SendGrid_Template_Name = this.Secondary_SendGrid_Template_Name;
                item.MM = item.MM_Secondary_Template;
                item.State_Backer = item.State_Backer_Secondary_Template;
            }
        }

    }
}
