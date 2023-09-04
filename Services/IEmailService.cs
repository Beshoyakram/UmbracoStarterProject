using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
	public interface IEmailService
	{
		Task SendEmailAsync(MailRequest mailRequest);
	}
}
