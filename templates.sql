USE [MailGmailDb]
GO

SET IDENTITY_INSERT [dbo].[emailTemplates] ON  

INSERT INTO [dbo].[emailTemplates]
           ([Id]
		   ,[subjectTemplate]
           ,[templateId])
     VALUES
           (1, ' Confirm email', 'confirm.template.pug'),
           (2, ' Reset password', 'reset.template.pug')

SET IDENTITY_INSERT [dbo].[emailTemplates] OFF

GO