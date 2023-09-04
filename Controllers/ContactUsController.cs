using MailKit;
using Microsoft.AspNetCore.Mvc;
using NPoco.Expressions;
using SixLabors.ImageSharp.ColorSpaces;
using Smidge.Models;
using System.Web;

using TD.SLA.Web.Helpers.Recaptcha;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Extensions;
using UmbracoStarterProject.Models;
using UmbracoStarterProject.Services;
using UmbracoStarterProject.Validation;
using static Umbraco.Cms.Core.Constants.Conventions;
using static Umbraco.Cms.Core.PropertyEditors.ImageCropperConfiguration;
using Umbraco.Cms.Web;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Umbraco.Cms.Core.Web;

namespace UmbracoStarterProject.Controllers
{
	public class ContactUsController : UmbracoApiController
	{
		private readonly UmbracoHelper _umbracoHelper;
		private readonly IConfiguration _config;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IContentService _contentService;
		private readonly IEmailService _emailService;
		private readonly IGoogleRecaptchaV3Service _googleService;

		private IMediaService _mediaService;
		private readonly MediaFileManager _mediaFileManager;
		private readonly MediaUrlGeneratorCollection _mediaUrlGenerators;
		private readonly IShortStringHelper _shortStringHelper;
		private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
		private readonly IUmbracoContextAccessor _umbracoContextAccessor;

		public ContactUsController(UmbracoHelper umbracoHelper,
			IConfiguration config,
			IVariationContextAccessor variationContextAccessor,
			IContentService contentService,
			IEmailService mailService,
			IGoogleRecaptchaV3Service googleService,
			IMediaService mediaService,
			MediaFileManager mediaFileManager,
			MediaUrlGeneratorCollection mediaUrlGenerators,
			IShortStringHelper shortStringHelper,
			IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
			IUmbracoContextAccessor umbracoContextAccessor
			)
		{
			_umbracoHelper = umbracoHelper;
			_config = config;
			_variationContextAccessor = variationContextAccessor;
			_contentService = contentService;
			_emailService = mailService;
			_googleService = googleService;

			_mediaService = mediaService;
			_shortStringHelper = shortStringHelper;
			_contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
			_umbracoContextAccessor = umbracoContextAccessor;
			_mediaUrlGenerators = mediaUrlGenerators;
			_mediaFileManager = mediaFileManager;
		}

		[HttpPost]
		public async Task<IActionResult> SubmitForm([FromForm] SubmissionModel submissionViewModel)
		{

			var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();

			ContactUsFormValidation contactUsFormValidation = new();

			_variationContextAccessor.VariationContext = new VariationContext("ar-EG");

			var IsValidModel = contactUsFormValidation.ValidForm(submissionViewModel);
			if (IsValidModel.IsValid == false)
				return StatusCode(StatusCodes.Status500InternalServerError, IsValidModel.ErrorMessage);


			GRequestModel rm = new GRequestModel(_config, submissionViewModel.ReCaptchaToken, HttpContext.Connection.RemoteIpAddress.ToString(), false);
			_googleService.InitializeRequest(rm);

			if (!await _googleService.Execute())
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Invalid Recaptcha");
			}

			if (submissionViewModel.contactUsId <= 0)
				return StatusCode(StatusCodes.Status500InternalServerError, "Contact us page id not correct");

			var contactUsContent = _umbracoHelper.Content(submissionViewModel.contactUsId);

			if (contactUsContent == null)
				return StatusCode(StatusCodes.Status500InternalServerError, "Contact us page not found");
			if (contactUsContent.GetType() != typeof(ContactUsPage) &&
				contactUsContent.GetType() != typeof(ContactUsWithTemplate))
				return StatusCode(StatusCodes.Status500InternalServerError, "this page is not a contact us page");
			try
			{
				string documentName = string.Empty;
				string fullName = submissionViewModel.Name;
				if (fullName.Length > 200)
				{
					var charsToRemove = fullName.Length - 200;
					documentName = fullName.Remove(fullName.Length - charsToRemove);
				}
				else
					documentName = fullName;
				var newMessage = _contentService.Create(documentName, contactUsContent.Id, SubmissionOfcontactUs.ModelTypeAlias);

				newMessage.SetValue("fullName", submissionViewModel.Name);
				newMessage.SetValue("phone", submissionViewModel.Phone);
				newMessage.SetValue("email", submissionViewModel.Email);
				newMessage.SetValue("messageTitle", submissionViewModel.MsgTitle);
				newMessage.SetValue("message", submissionViewModel.MsgContent);


				///////////////attached file /////////////////////
				IMedia media = null;

				if (submissionViewModel.uploadFile != null)
				{
					string fileName = submissionViewModel.uploadFile.FileName;
					string extension = Path.GetExtension(fileName);
					if (!HasFileAllowedExtension(extension))
						throw new Exception("Wrong file extention.");

					await using (var ms = new MemoryStream())
					{
						await submissionViewModel.uploadFile.CopyToAsync(ms);
						media = _mediaService.CreateMedia(fileName, -1, Image.ModelTypeAlias);
						media.SetValue(_mediaFileManager, _mediaUrlGenerators, _shortStringHelper, _contentTypeBaseServiceProvider, Constants.Conventions.Media.File, fileName, ms);
						//put file into media section
						_mediaService.Save(media);

					}
					var file = umbracoContext.Media.GetById(true, media.Id);
					var filePath = ((Image)file).MediaUrl();
					newMessage.SetValue("uploadFile", filePath);
				}

				////////////////////

				_contentService.SaveAndPublish(newMessage);

				#region SendMail

				var contact = contactUsContent.SafeCast<ContactUsPage>();
				List<string> emailsNames = new List<string>();
				if (contact != null)
				{

					if (contact.Mails == null)
						return Ok(new { savedSuccessfuly = true });

					var emails = contact.Mails.ToList();
					if (emails.Count <= 0)
						return Ok(new { savedSuccessfuly = true });

					foreach (var item in emails)
					{
						var emailTemp = item.SafeCast<EmailContent>();
						emailsNames.Add(emailTemp.EmailAddress);
					}
				}
				else
				{
					var contactUsWithTemplate = contactUsContent.SafeCast<ContactUsWithTemplate>();
					if (contactUsWithTemplate.Mails == null)
						return Ok(new { savedSuccessfuly = true });

					var emails = contactUsWithTemplate.Mails.ToList();
					if (emails.Count <= 0)
						return Ok(new { savedSuccessfuly = true });

					foreach (var item in emails)
					{
						var emailTemp = item.SafeCast<EmailContent>();
						emailsNames.Add(emailTemp.EmailAddress);
					}
				}



				var sendMail = SendEmailToInformAdmins(submissionViewModel, newMessage, emailsNames, media);
				if (sendMail.IsSent == true)
					return Ok(new { savedSuccessfuly = true });
				else
					return StatusCode(StatusCodes.Status500InternalServerError, sendMail.msg);

				#endregion
			}
			catch (Exception err)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
			}




		}


		private (bool IsSent, string msg) SendEmailToInformAdmins(SubmissionModel requestModel, IContent submission, List<string> emailsNames, IMedia media)
		{
			string msg = "Sent Successfully";
			var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);
			if (uriBuilder.Uri.IsDefaultPort)
			{
				uriBuilder.Port = -1;
			}
			var baseUrl = uriBuilder.Uri.AbsoluteUri;
			//string baseUrl = _config.GetSection("site").GetValue<string>("baseUrl");

			string MailText = String.Empty;
			string FilePath = Directory.GetCurrentDirectory() + "\\EmailTemplates\\ContactUsEmailAR.html";

			using (StreamReader str = new StreamReader(FilePath))
			{
				string Phone = GetPhone(requestModel.Phone);
				string Email = GetEmail(requestModel.Email);
				MailText = str.ReadToEnd();
				MailText = HttpUtility.UrlDecode(MailText);
				var submissionAdminUrl = $"{baseUrl}/umbraco#/content/content/edit/{submission.Id}";
				var homePageUrl = $"{baseUrl}ar";
				MailText = MailText.Replace("##FullName##", requestModel.Name);
				MailText = MailText.Replace("##Phone##", Phone);
				MailText = MailText.Replace("##Email##", Email);
				MailText = MailText.Replace("##MessageTitle##", requestModel.MsgTitle);
				MailText = MailText.Replace("##Message##", requestModel.MsgContent);
				MailText = MailText.Replace("##HomePageUrl##", homePageUrl);

				if (media != null)
				{
					var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();
					var file = umbracoContext.Media.GetById(true, media.Id);
					var filePath = ((Image)file).MediaUrl();
					string fileLoc = baseUrl + filePath;
					MailText = MailText.Replace("##ref##", fileLoc);
				}

			}

			try
			{

				if (emailsNames != null && emailsNames.Any())
				{
					_emailService.SendEmailAsync(new MailRequest
					{
						ToEmails = emailsNames,
						Subject = "يوجد مستخدم يريد التواصل مع الهيئة",
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
		public string GetPhone(string Phone)
		{
			if (!string.IsNullOrEmpty(Phone))
				return Phone;
			else
				return "لايوجد";
		}
		public string GetEmail(string Email)
		{
			if (!string.IsNullOrEmpty(Email))
				return Email;
			else
				return "لايوجد";
		}

		private bool HasFileAllowedExtension(string fileExtension)
		{
			string[] allowedExtensions = new string[] { ".PNG", ".JPG", ".JPEG", ".png", ".jpg", ".jpeg", ".pdf", ".PDF" };
			return allowedExtensions.Contains(fileExtension);
		}
	}
}