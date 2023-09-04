using Microsoft.AspNetCore.Mvc;
using TD.SLA.Web.Helpers.Recaptcha;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common;
using UmbracoStarterProject.Models;
using UmbracoStarterProject.Validation;
using Umbraco.Cms.Core.Services;
using UmbracoStarterProject.Services;
using Umbraco.Extensions;
using System.Web;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.Controllers;
using System.Text.RegularExpressions;

namespace UmbracoStarterProject.Controllers
{
	public class NewsLetterSubscriptionController : UmbracoApiController
	{
		private readonly UmbracoHelper _umbracoHelper;
		private readonly IConfiguration _config;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IContentService _contentService;
		private readonly IEmailService _emailService;
		private readonly IGoogleRecaptchaV3Service _googleService;




		public NewsLetterSubscriptionController(UmbracoHelper umbracoHelper, IConfiguration config, IVariationContextAccessor variationContextAccessor, IContentService contentService, IEmailService emailService, IGoogleRecaptchaV3Service googleService)
		{
			_umbracoHelper = umbracoHelper;
			_config = config;
			_variationContextAccessor = variationContextAccessor;
			_contentService = contentService;
			_emailService = emailService;
			_googleService = googleService;
		}

		[HttpPost]
		public async Task<IActionResult> SubmitSubscription(SubscriptionModel submitSubscriptionModel, string culture = "ar-EG")
		{

			SubscriptionValidation subscriptionValidation = new();

			_variationContextAccessor.VariationContext = new VariationContext(culture);

			var IsValidModel = subscriptionValidation.ValidSubscription(submitSubscriptionModel);
			if (IsValidModel.IsValid == false)
				return StatusCode(StatusCodes.Status500InternalServerError, IsValidModel.ErrorMessage);


			if (submitSubscriptionModel.NewsLetterSubscriptionId <= 0)
				return StatusCode(StatusCodes.Status500InternalServerError, "NewsLetterSubscriptionpage id not correct");

			var subscriptionContent = _umbracoHelper.Content(submitSubscriptionModel.NewsLetterSubscriptionId);

			if (subscriptionContent == null)
				return StatusCode(StatusCodes.Status500InternalServerError, "NewsLetterSubscription page not found");
			if (subscriptionContent.GetType() != typeof(NewsLetterSubscriptionList))
				return StatusCode(StatusCodes.Status500InternalServerError, "this page is not a NewsLetterSubscription page");
			try
			{
				string documentName = submitSubscriptionModel.EmailOfSubscription;

				var newSubscription = _contentService.Create(documentName, subscriptionContent.Id, SubscriptionItem.ModelTypeAlias);
				newSubscription.SetValue("emailOfSubscription", submitSubscriptionModel.EmailOfSubscription);
				_contentService.SaveAndPublish(newSubscription);

				#region SendMail          

				var email = submitSubscriptionModel.EmailOfSubscription;
				if (email == null)
					return Ok(new { savedSuccessfuly = true });

				string status = "تم الغاء الاشتراك في الهيئه بنجاح";
				var sendMail = SendEmailToInformAdmins(submitSubscriptionModel, newSubscription, email, status);
				if (sendMail.IsSent == true)
					return Ok(new { savedSuccessfuly = true });
				else
					return StatusCode(StatusCodes.Status500InternalServerError, sendMail.msg);

				#endregion SendMail

			}
			catch (Exception err)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
			}




		}

		[HttpPost]
		public async Task<IActionResult> RemoveSubscription(SubscriptionModel submitSubscriptionModel, string culture = "ar-EG")
		{

			SubscriptionValidation subscriptionValidation = new();

			_variationContextAccessor.VariationContext = new VariationContext(culture);

			var IsValidModel = subscriptionValidation.ValidSubscription(submitSubscriptionModel);
			if (IsValidModel.IsValid == false)
				return StatusCode(StatusCodes.Status500InternalServerError, IsValidModel.ErrorMessage);


			if (submitSubscriptionModel.NewsLetterSubscriptionId <= 0)
				return StatusCode(StatusCodes.Status500InternalServerError, "NewsLetterSubscriptionpage id not correct");

			var subscriptionContent = _umbracoHelper.Content(submitSubscriptionModel.NewsLetterSubscriptionId);

			if (subscriptionContent == null)
				return StatusCode(StatusCodes.Status500InternalServerError, "NewsLetterSubscription page not found");
			if (subscriptionContent.GetType() != typeof(NewsLetterSubscriptionList))
				return StatusCode(StatusCodes.Status500InternalServerError, "this page is not a NewsLetterSubscription page");
			try
			{
				var AllNodes =subscriptionContent.Children;
				var item = AllNodes.Where(x => x.Value<string>("EmailOfSubscription") == submitSubscriptionModel.EmailOfSubscription)
					.FirstOrDefault();

				if (item != null)
				{

					IContent content = _contentService.GetById(item.Id);
					bool res = false;
					if (content != null)
					{

						var newSubscription = _contentService.MoveToRecycleBin(content);
						res = newSubscription.Success;

						if (res)
						{
							#region SendMail

							var email = submitSubscriptionModel.EmailOfSubscription;
							if (email == null)
								return Ok(new { savedSuccessfuly = true });


							string status = "تم الغاء الاشتراك في الهيئه بنجاح";
							var sendMail = SendEmailToInformAdmins(submitSubscriptionModel, content, email, status);
							if (sendMail.IsSent == true)
								return Ok(new { savedSuccessfuly = true });
							else
								return StatusCode(StatusCodes.Status500InternalServerError, sendMail.msg);

							#endregion SendMail
						}

					}
					else
						return StatusCode(StatusCodes.Status500InternalServerError, "NewsLetterSubscription page not found");
				}
				return StatusCode(StatusCodes.Status500InternalServerError, "There is mistake and cant delete this item");

			}
			catch (Exception err)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
			}




		}
		private (bool IsSent, string msg) SendEmailToInformAdmins(SubscriptionModel requestModel,
			IContent submission, string emailName,string status )
		{
			string msg = "Sent Successfully";
			var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);
			if (uriBuilder.Uri.IsDefaultPort)
			{
				uriBuilder.Port = -1;
			}
			var baseUrl = uriBuilder.Uri.AbsoluteUri;
			string MailText = String.Empty;
			string FilePath = Directory.GetCurrentDirectory() + "\\EmailTemplates\\ContactUsEmailAR.html";

			using (StreamReader str = new StreamReader(FilePath))
			{

				MailText = str.ReadToEnd();
				MailText = HttpUtility.UrlDecode(MailText);
				var submissionAdminUrl = $"{baseUrl}/umbraco#/content/content/edit/{submission.Id}";
				var homePageUrl = $"{baseUrl}ar";

				MailText = MailText.Replace("##Email##", requestModel.EmailOfSubscription);

				MailText = MailText.Replace("##HomePageUrl##", homePageUrl);
			}

			try
			{

				if (emailName != null)
				{
					var emailsNames = new List<string>() { emailName };
					_emailService.SendEmailAsync(new MailRequest
					{
						ToEmails = emailsNames,
						Subject = status,
						Body = MailText
					});
				}
				return (true, msg);
			}
			catch (System.Exception ex)
			{
				msg = ex.Message.ToString();
				return (false, msg);
			}
		}
	}
}
