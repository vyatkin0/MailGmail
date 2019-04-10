using Newtonsoft.Json.Linq;
using System;

namespace MailGmail.Models
{
    public class SendMailModel
    {
        public string clientMsgId;
        public string from;
        public string to;
        public int bodyTemplate;
        public JObject subjectModel;
        public JObject bodyModel;
        public DateTime? expiration;
    }

    public class EmailStatus
    {
        public long? messageId;
        public string clientMsgId;
        public object status;
    }
}
