# MailGmail
MailGmail application.

Example of a web application that sends emails that created from templates using the Gmail API (https://developers.google.com/gmail/api).

Solution is based on ASP.NET CORE, Entity Framework Core, NodeJS and Angular.

It uses pug (https://pugjs.org) to process message templates.

Messages are stored to EF storage and processed in ASP.NET Core background tasks with hosted services.


These are generic installation instructions:

1. Download credentials for Gmail API from https://console.developers.google.com/apis/credentials;

2. Edit database connection string in file appsettings.json;

3. Download and install .NET CORE SDK 2.2;

4. In the solution's folder execute command "npm install pug"

5. In the solution's folder execute following commands: "dotnet restore", "dotnet ef migrations add Initial", "dotnet ef database update", "dotnet build";

6. Populate table emailTemplates in database MailGmailDb (file templates.sql contains TSQL script to populate the table);

7. In the solution's folder execute "dotnet run" command.


Your service is running now on a local web-server with url http://localhost:5000
