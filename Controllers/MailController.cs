using MailGmail.Infrastructure;
using MailGmail.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MailGmail.Controllers
{
    // Mail controller.

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private ILogger _logger;
        private AppDbContext _ctx;

        public MailController(ILogger<MailController> logger, AppDbContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [HttpPost]
        public ActionResult Send([Required] [FromBody] SendMailModel model)
        {
            EmailTemplate template = _ctx.emailTemplates.Where(t => t.Id == model.bodyTemplate).FirstOrDefault();

            if (null == template)
            {
                throw new ApplicationException($"Email template with id {model.bodyTemplate} was not found");
            }

            MailAgent agent = new MailAgent(_logger);

            DateTime utcNow = DateTime.UtcNow;

            EmailMessage m = new EmailMessage
            {
                clientMsgId = model.clientMsgId,
                from = model.from,
                to = model.to,
                template = template,
                subjectModel = model.subjectModel?.ToString(),
                bodyModel = model.bodyModel?.ToString(),
                createdAt = utcNow,
                expiration = model.expiration
                //lastAttemptAt = utcNow
            };

            //m.resultMessage = agent.Send(m);

            _ctx.emailMessages.Add(m);

            _ctx.SaveChanges();

            return Ok(new EmailStatus { messageId = m.Id, clientMsgId = model.clientMsgId, status = m.resultMessage });
        }

        [HttpPost]
        public ActionResult FindMessages([Required] [FromBody] EmailStatus model)
        {
            IQueryable<EmailMessage> query = _ctx.emailMessages.Include(m => m.template).OrderBy(m => m.createdAt);

            if (model.messageId.HasValue)
            {
                query = query.Where(m => m.Id == model.messageId);
            }

            if (!string.IsNullOrEmpty(model.clientMsgId))
            {
                query = query.Where(m => m.clientMsgId == model.clientMsgId);
            }

            if (null != model.status)
            {
                string status = model.status as string;
                if (!string.IsNullOrEmpty(status))
                    query = query.Where(m => m.resultMessage == model.status as string);
            }

            return Ok(query.ToArray());
        }
    }
}
