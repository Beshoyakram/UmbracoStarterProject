using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UmbracoStarterProject.Models;
using MailKit.Security;
using Umbraco.Cms.Web.Common.Controllers;

namespace UmbracoStarterProject.Controllers
{ 
    public class STMPController : UmbracoApiController
    {  

        [HttpPost]
        public async Task<IActionResult> STMPChecker(MailSettings settings)
        {
            try
            {
                using var smtp = new SmtpClient();

                smtp.Connect(settings.Host, settings.Port, SecureSocketOptions.StartTls);

                smtp.Authenticate(settings.Mail, settings.Password);

                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                throw new Exception("failure");
            }
            return Ok("Connected successfully");
        }

    }
}
