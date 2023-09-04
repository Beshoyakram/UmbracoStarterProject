namespace UmbracoStarterProject.Models
{
	public class MediaItem
	{
		public string ThumbImage { get; set; }
		public string MediaUrl { get; set; }
		public string MediaType { get; set; }
		public string MediaDescription { get; set; }
	}


	public class MediaList
	{
		public string ListCount { get; set; }
		public List<MediaItem> MediaItems { get; set; }
	}
	
}
