using System.Web;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common;
using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
	public class GalleryService : IGalleryService
	{
		private readonly UmbracoHelper _umbracoHelper;

		public GalleryService(UmbracoHelper umbracoHelper)
		{
			_umbracoHelper = umbracoHelper;
		}

		public List<MediaItem> GetItemsInGallery(int pageId)
		{
			var ResponseList = new List<MediaItem>();

			var CurrentPage = _umbracoHelper.Content(pageId);
			if (CurrentPage != null)
			{

				var mediaList = CurrentPage.HasValue("gallerySection") != false ? CurrentPage.Value<IEnumerable<BlockListItem>>("gallerySection") : null;
				if (mediaList != null)
				{
					foreach (var item in mediaList)
					{
						string thumbImage = "";
						string mediaUrl = "";
						string mediaType = "";
						string mediaDescription = "";
						string youtubeLink = "";
						string embedLink = "";



						var itemType = item.Content.ContentType.Alias;
						;
						//((Umbraco.Cms.Web.Common.PublishedModels.ImageInGallery)item.Content).MediaImage.Alias;
						if (itemType == "imageInGallery")
						{
							mediaType = "image";
							var ImageInGallery = ((Umbraco.Cms.Web.Common.PublishedModels.ImageInGallery)item.Content);
							thumbImage = mediaUrl = ImageInGallery.MediaImage != null ?
								ImageInGallery.MediaImage.GetCropUrl("GalleryImage") : "";
							mediaDescription = ImageInGallery?.ImageDescription;
						}
						else if (itemType == "youTubeVideo")
						{

							mediaType = "youtube";
							var YouTubeVideo = ((Umbraco.Cms.Web.Common.PublishedModels.YouTubeVideo)item.Content);

							youtubeLink = YouTubeVideo?.YouTubeVideoLink.ToString();
							Uri myUri = new Uri(youtubeLink);
							string param1 = myUri != null ? HttpUtility.ParseQueryString(myUri.Query).Get("v") : "";
							var newLink = "https://www.youtube.com" + "/watch?v=" + param1;
							embedLink = !string.IsNullOrEmpty(newLink) ? newLink.ReplaceFirst("/watch?v=", "/embed/") : "";
							thumbImage = mediaUrl = embedLink;
						}

						else if (itemType == "videoUploader")
						{
							mediaType = "internalVideo";
							var VideoUploader = ((Umbraco.Cms.Web.Common.PublishedModels.VideoUploader)item.Content);

							mediaUrl = VideoUploader?.MediaVideoUploader != null ?
								VideoUploader?.MediaVideoUploader.LocalCrops.Src : "";
							thumbImage = VideoUploader?.VideoThumbnail != null ?
							VideoUploader?.VideoThumbnail.GetCropUrl("GalleryVideoThumbnail") : "";
							mediaDescription = VideoUploader?.VideoDescription;
						}
						ResponseList.Add(new MediaItem()
						{
							MediaType = mediaType,
							MediaDescription = mediaDescription,
							MediaUrl = mediaUrl,
							ThumbImage = thumbImage,

						});
					}
				}
			}
			return ResponseList;
		}
	}
}
