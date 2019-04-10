using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MailGmail.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "emailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    subjectTemplate = table.Column<string>(nullable: true),
                    templateId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emailMessages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    clientMsgId = table.Column<string>(nullable: true),
                    from = table.Column<string>(nullable: true),
                    to = table.Column<string>(nullable: true),
                    templateId = table.Column<int>(nullable: true),
                    subjectModel = table.Column<string>(nullable: true),
                    bodyModel = table.Column<string>(nullable: true),
                    createdBy = table.Column<string>(nullable: true),
                    createdAt = table.Column<DateTime>(nullable: false),
                    lastAttemptAt = table.Column<DateTime>(nullable: true),
                    expiration = table.Column<DateTime>(nullable: true),
                    resultMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emailMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emailMessages_emailTemplates_templateId",
                        column: x => x.templateId,
                        principalTable: "emailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_emailMessages_templateId",
                table: "emailMessages",
                column: "templateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emailMessages");

            migrationBuilder.DropTable(
                name: "emailTemplates");
        }
    }
}
