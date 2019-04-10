﻿// <auto-generated />
using System;
using MailGmail.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MailGmail.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190407061337_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MailGmail.Infrastructure.EmailMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("bodyModel");

                    b.Property<string>("clientMsgId");

                    b.Property<DateTime>("createdAt");

                    b.Property<string>("createdBy");

                    b.Property<DateTime?>("expiration");

                    b.Property<string>("from");

                    b.Property<DateTime?>("lastAttemptAt");

                    b.Property<string>("resultMessage");

                    b.Property<string>("subjectModel");

                    b.Property<int?>("templateId");

                    b.Property<string>("to");

                    b.HasKey("Id");

                    b.HasIndex("templateId");

                    b.ToTable("emailMessages");
                });

            modelBuilder.Entity("MailGmail.Infrastructure.EmailTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("subjectTemplate");

                    b.Property<string>("templateId");

                    b.HasKey("Id");

                    b.ToTable("emailTemplates");
                });

            modelBuilder.Entity("MailGmail.Infrastructure.EmailMessage", b =>
                {
                    b.HasOne("MailGmail.Infrastructure.EmailTemplate", "template")
                        .WithMany()
                        .HasForeignKey("templateId");
                });
#pragma warning restore 612, 618
        }
    }
}