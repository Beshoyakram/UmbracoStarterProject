using Microsoft.AspNetCore.Mvc;
using System.Web;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoStarterProject.Models;
using UmbracoStarterProject.Services;

namespace UmbracoStarterProject.Controllers
{
	public class GalleryController : UmbracoApiController
	{
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IGalleryService _galleryService;


		public GalleryController(IVariationContextAccessor variationContextAccessor, IGalleryService galleryService)
		{
			_variationContextAccessor = variationContextAccessor;
			_galleryService = galleryService;
		}


		[HttpGet]
		public MediaList GetItemsInGallery(int pageId, string lang)
		{
			try
			{
				_variationContextAccessor.VariationContext = new VariationContext(lang);
				var ItemList = new MediaList();
				var MediaList = new List<MediaItem>();

				MediaList = _galleryService.GetItemsInGallery(pageId);
				ItemList.MediaItems = MediaList;
				ItemList.ListCount = MediaList.Count().ToString();
				return ItemList;
			}
			catch (Exception ex)
			{

				return null;
			}

		}

	}
}
