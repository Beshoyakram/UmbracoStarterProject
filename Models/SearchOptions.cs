namespace UmbracoStarterProject.Models
{
	public class SearchOptions
	{
		public string Fields { get; set; }
		public string DateFields { get; set; }
		public string ExcludedTypes { get; set; }
		public string HiddenTypes { get; set; }
		public List<int> ExcludedNodes { get; set; }
		public string PageSize { get; set; }
		public string Fuzzyness { get; set; }
	}
}
