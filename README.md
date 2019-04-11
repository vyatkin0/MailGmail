# MailGmail
MailGmail application.

Example of a web application that sends emails that created from templates using the Gmail API (https://developers.google.com/gmail/api).

Solution is based on ASP.NET CORE, Entity Framework Core, NodeJS and Angular.

It uses pug (https://pugjs.org) to process messages templates.

Messages are stored to EF storage and processed in ASP.NET Core background tasks with hosted services.


These are generic installation instructions.

Download credentials for Gmail API from https://console.developers.google.com/apis/credentials";

Edit database connection string in file appsettings.json;

Download and install .NET CORE SDK 2.2;

In the solution's folder execute command "npm install pug"

In the solution's folder execute following commands: "dotnet restore", "dotnet ef migrations add Initial", "dotnet ef database update", "dotnet build";

Populate table emailTemplates in database MailGmailDb (file templates.sql contains TSQL script to populate table);

In the solution's folder execute "dotnet run" command.


Your service is running now on a local web-server with url http://localhost:5000
