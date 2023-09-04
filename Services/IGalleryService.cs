using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
	public interface IGalleryService
	{
		public List<MediaItem> GetItemsInGallery(int pageId);
	}
}
