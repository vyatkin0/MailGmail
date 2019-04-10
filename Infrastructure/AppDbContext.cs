using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace MailGmail.Infrastructure
{
    /// <summary>
    /// Application database contexxt
    /// </summary>
    public class AppDbContext : DbContext
    {
        // Migration flag
        static bool _migrated;

        // Email templates database table
        public DbSet<EmailTemplate> emailTemplates { get; set; }

        // Email messages database table
        public DbSet<EmailMessage> emailMessages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            if (!_migrated)
            {
                _migrated = true;
                Database.Migrate();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class EmailTemplate
    {
        [Key]
        public int Id { get; set; } // Email template id
        public string subjectTemplate { get; set; } // subject pug template
        public string templateId { get; set; } // Body template identifier (it is file name)
    }

    public class EmailMessage
    {
        [Key]
        public long Id { get; set; } // Email message identifier
        public string clientMsgId { get; set; } // Email message identifier created by client application
        public string from { get; set; }
        public string to { get; set; }

        public EmailTemplate template { get; set; }
        public string subjectModel { get; set; }
        public string bodyModel { get; set; }
        public string createdBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? lastAttemptAt { get; set; }
        public DateTime? expiration { get; set; }
        public string resultMessage { get; set; }
    }
}
