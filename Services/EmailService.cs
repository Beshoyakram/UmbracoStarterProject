﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
	public class EmailService : IEmailService
	{
		private readonly MailSettings _mailSettings;
		public EmailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}

		public async Task SendEmailAsync(MailRequest mailRequest)
		{
			var email = new MimeMessage();

			email.Sender = MailboxAddress.Parse(_mailSettings.Mail);

			foreach (var ToEmail in mailRequest.ToEmails)
			{
				email.To.Add(MailboxAddress.Parse(ToEmail));
			}

			email.Subject = mailRequest.Subject;

			var builder = new BodyBuilder();
			builder.HtmlBody = mailRequest.Body;
			email.Body = builder.ToMessageBody();

			using var smtp = new SmtpClient();

			smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

			smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

			await smtp.SendAsync(email);

			smtp.Disconnect(true);
		}
	}
}
