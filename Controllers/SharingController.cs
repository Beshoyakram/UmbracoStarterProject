using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoStarterProject.Models;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Core.Services;

namespace UmbracoStarterProject.Controllers
{

	public class SharingController : UmbracoApiController
	{
		private readonly IVariationContextAccessor _VariationContextAccessor;
		public UmbracoHelper _UmbracoHelper;
		public IContentService _ContentService;


		public SharingController(IVariationContextAccessor variationContextAccessor
			, UmbracoHelper umbracoHelper,IContentService contentService)
		{
			_VariationContextAccessor = variationContextAccessor;
			_UmbracoHelper = umbracoHelper;
			_ContentService = contentService;
		}

		

		[HttpGet]
		public IActionResult Visit(int PageId, string LinkName, string Culture = "ar-EG")
		{
			_VariationContextAccessor.VariationContext = new VariationContext(Culture);
			try
			{
				var page = _ContentService.GetById(PageId);
				if (page != null)
				{
					if (page.HasProperty(LinkName))
					{
						
						int.TryParse(page.GetValue(LinkName, Culture)?.ToString(), out int propCount);
						propCount++;

						page.SetValue(LinkName, propCount, Culture);
						_ContentService.SaveAndPublish(page);
					}
					else
					{
						throw new Exception("There is no Link name like you entered in this page.");
					}
				}
				else
				{
					throw new Exception($"There is no page for this id : {PageId}");
				}

				return Ok();

			}catch (Exception ex) 
			{ throw new Exception(ex.Message); }

		}
	}
}
