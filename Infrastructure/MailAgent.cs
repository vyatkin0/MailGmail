using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading;

namespace MailGmail.Infrastructure
{
    /// <summary>
    /// Class of an object that sends email messages using Gmail API
    /// </summary>
    public class MailAgent
    {
        ILogger _logger;

        public MailAgent(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sends a message using Gmail API
        /// </summary>
        /// <param name="msg">Message to send</param>
        /// <returns>JSON serialized string of a result object</returns>
        public string Send(EmailMessage msg)
        {
            string emailTemplate = GetMsgTemplate(msg.template.templateId);

            if (null == emailTemplate)
            {
                _logger.LogError($"Unable to load Email template with id {msg.template.templateId}");
                return null;
            }

            string htmlBody = null;
            string subject = null;

            ServiceCollection services = new ServiceCollection();
            services.AddNodeServices();

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                NodeServicesOptions options = new NodeServicesOptions(serviceProvider) { /* Assign/override any other options here */ };
                using (INodeServices nodeServices = NodeServicesFactory.CreateNodeServices(options))
                {
                    htmlBody = nodeServices.InvokeAsync<string>("pugcompile", emailTemplate, null==msg.bodyModel ? null: JsonConvert.DeserializeObject(msg.bodyModel)).Result;
                    subject = nodeServices.InvokeAsync<string>("pugcompile", msg.template.subjectTemplate, null == msg.subjectModel ? null : JsonConvert.DeserializeObject(msg.subjectModel)).Result;
                }
            }

            services.Clear();

            if (null == subject)
            {
                _logger.LogError($"Unable to load email subject");
                return null;
            }

            if (null == emailTemplate)
            {
                _logger.LogError($"Unable to load Email template with id {msg.template.templateId}");
                return null;
            }

            UserCredential credential;
            string[] scopes = { GmailService.Scope.GmailSend };

            using (FileStream stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                _logger.LogInformation("Google credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            using (GmailService service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MailGmail",
            }))
            {

                Google.Apis.Gmail.v1.Data.Message message = CreateGmailMessage(msg.from, msg.to, subject, htmlBody);
                Google.Apis.Gmail.v1.Data.Message messageResult = service.Users.Messages.Send(message, "me").Execute();

                return JsonConvert.SerializeObject(messageResult);
            }
        }

        /// <summary>
        /// Prepares MailMessage object for Gmail API
        /// </summary>
        /// <param name="to">To field of a message</param>
        /// <param name="from">From field of a message</param>
        /// <param name="subject">Subject field of a message</param>
        /// <param name="bodyHtml">HTML formatted body of a message</param>
        /// <returns>Result message</returns>
        private static Google.Apis.Gmail.v1.Data.Message CreateGmailMessage(string from, string to, string subject, string bodyHtml)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);
            string[] splittedTo = to.Split(new Char[] {';', ','});
            foreach (string t in splittedTo)
            {
                mailMessage.To.Add(t.Trim());
            }

            mailMessage.ReplyToList.Add(from);
            mailMessage.Subject = subject;
            mailMessage.Body = bodyHtml;
            mailMessage.IsBodyHtml = true;

            MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);

            Google.Apis.Gmail.v1.Data.Message gmailMessage = new Google.Apis.Gmail.v1.Data.Message
            {
                Raw = Base64UrlEncoder.Encode(mimeMessage.ToString())
            };

            return gmailMessage;
        }

        /// <summary>
        /// Read a message template from a file
        /// </summary>
        /// <param name="objectKey">Template's identifier</param>
        private string GetMsgTemplate(string objectKey)
        {
            return File.ReadAllText($"email-templates/{objectKey}");
        }
    }
}
