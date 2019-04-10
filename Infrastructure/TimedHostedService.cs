using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MailGmail.Infrastructure
{
    /// <summary>
    /// Class of a service that processes email messages in background
    /// </summary>

    internal class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer; // Reference to a timer

        private static object dontOverlapObject = new object(); // Synchronization object
        private readonly ILogger _logger; // Logger interface
        IServiceProvider _services; // DI services provider
        IConfiguration _configuration; // Configuration interface

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceProvider services, IConfiguration configuration)
        {
            _logger = logger;
            _services = services;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            int updateTimeout = Convert.ToInt32(_configuration["Settings:UPDATE_TIMEOUT"]);
            if (updateTimeout <= 0) updateTimeout = 60;

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(updateTimeout));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Method to be executed by the timer
        /// </summary>
        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            if (Monitor.TryEnter(dontOverlapObject))
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        AppDbContext ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        List<EmailMessage> messages = ctx.emailMessages.Where(m => m.resultMessage == null)
                            .Include(m => m.template)
                            .OrderBy(m => m.createdAt)
                            .ToList();

                        int intervalDays1 = Convert.ToInt32(_configuration["Settings:INTERVAL_DAYS1"]);
                        if (intervalDays1 <= 0) intervalDays1 = 1;
                        int intervalMinutes1 = Convert.ToInt32(_configuration["Settings:INTERVAL_MINS1"]);
                        if (intervalMinutes1 <= 0) intervalMinutes1 = 10;

                        int intervalDays2 = Convert.ToInt32(_configuration["Settings:INTERVAL_DAYS2"]);
                        if (intervalDays2 <= 0) intervalDays2 = 10;
                        int intervalHours2 = Convert.ToInt32(_configuration["Settings:INTERVAL_HOURS2"]);
                        if (intervalHours2 <= 0) intervalHours2 = 1;

                        int expiredAfter = Convert.ToInt32(_configuration["Settings:EXPIRATION_DAYS"]);
                        if (expiredAfter <= 0) expiredAfter = 30;
                        int intervalDays3 = Convert.ToInt32(_configuration["Settings:INTERVAL_DAYS3"]);
                        if (intervalDays3 <= 0) intervalDays3 = 1;

                        string expirationMessage = _configuration["Settings:EXPIRATION_MESSAGE"];
                        if (null == expirationMessage) expirationMessage = "EXPIRED";

                        MailAgent agent = new MailAgent(_logger);

                        foreach (EmailMessage m in messages)
                        {
                            DateTime utcNow = DateTime.UtcNow;

                            TimeSpan ts = utcNow - m.lastAttemptAt.GetValueOrDefault();
                            TimeSpan tsTotal = utcNow - m.createdAt;

                            if (!m.lastAttemptAt.HasValue
                                || ((m.expiration == null || m.expiration > utcNow)
                                    && (tsTotal.TotalDays < intervalDays1 && ts.TotalMinutes > intervalMinutes1
                                        || tsTotal.TotalDays < intervalDays2 && ts.TotalHours > intervalHours2
                                        || tsTotal.TotalDays < expiredAfter && ts.TotalDays > intervalDays3)))
                            {
                                m.resultMessage = agent.Send(m);

                                m.lastAttemptAt = utcNow;

                                ctx.SaveChanges();
                            }
                            else
                            {
                                if ((m.expiration != null && m.expiration <= utcNow) || tsTotal.TotalDays >= expiredAfter)
                                {
                                    m.resultMessage = expirationMessage;
                                    ctx.SaveChanges();
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(dontOverlapObject);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
