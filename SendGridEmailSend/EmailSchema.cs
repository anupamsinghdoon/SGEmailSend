using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SendGridEmailSend
{


    public partial class EmailSchema
    {
        [JsonProperty("personalizations")]
        public List<Personalization> Personalizations { get; set; }

        [JsonProperty("from")]
        public From From { get; set; }

        [JsonProperty("reply_to")]
        public From ReplyTo { get; set; }

        [JsonProperty("template_id")]
        public string TemplateId { get; set; }
       
        [JsonProperty("ip_pool_name")]
        public string ip_pool_name { get; set; }

        static string file = ConfigurationManager.AppSettings["Schema"].ToString();//Directory.GetCurrentDirectory() + "Schema.json";
        static string data = File.ReadAllText(file);
        static object json = JsonConvert.DeserializeObject<object>(data);
        
        public static EmailSchema LoadEmailData(SentLetter item)
        {
            try
            {
                //******************************************* Start Code for Json Data ***********************************************////////
                //List<string> bcc = item.BCC.Split(';').ToList();
                //List<BCC> bccList = new List<BCC>();
                //foreach (string s in bcc)
                //{ bccList.Add(new BCC { Email = s }); }

                var emailData = EmailSchema.FromJson(json.ToString());
                emailData.Personalizations.RemoveAt(0);
                emailData.Personalizations.Add(new Personalization
                {
                    To = new List<To> { new To { Email = item.E_Notice_Email, Name = item.Debtor_First_Name } },
                    bcc = item.BCC == "" ? null : new List<BCC> { new BCC { Email = item.BCC } },//bccList
                    DynamicTemplateData = (JObject)JsonConvert.DeserializeObject(item.dynamic_template_data_json),
                    custom_args = (JObject)JsonConvert.DeserializeObject(item.custom_args_json),
                    Subject = ""

                });
                emailData.TemplateId = item.Primary_SendGrid_Template_Id;
                emailData.From.Email = item.From_Email;
                emailData.From.Name = item.From_Name;
                emailData.ReplyTo.Email = item.Reply_To;
                emailData.ReplyTo.Name = item.Reply_To_Name;

                emailData.ip_pool_name = item.IP_Pool_Name != "" ? item.IP_Pool_Name : null;
                //******************************************* End Code for Json Data ***********************************************////////

                return emailData;
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"----------------------Error Captured: {DateTime.Now} ------------------------------");
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.Message}");
                Logger.LogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.StackTrace}");
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"----------------------Error Captured: {DateTime.Now} ------------------------------");
                Logger.ErrorLogIt(System.Reflection.MethodBase.GetCurrentMethod(), $"{ex.Message}");
                throw ex;
            }
        }
        
    }

    public partial class From
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public partial class To
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public partial class BCC
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Personalization
    {
        [JsonProperty("to")]
        public List<To> To { get; set; }
        [JsonProperty("bcc")]
        public List<BCC> bcc { get; set; }
        [JsonProperty("dynamic_template_data")]
        public object DynamicTemplateData { get; set; }

        [JsonProperty("Subject")]
        public string Subject { get; set; }
        [JsonProperty("custom_args")]
        public object custom_args { get; set; }

    }

    
    public partial class EmailSchema
    {
        public static EmailSchema FromJson(string json) => JsonConvert.DeserializeObject<EmailSchema>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this EmailSchema self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {

            NullValueHandling = NullValueHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
