namespace UmbracoStarterProject.Models
{
    public class FilterModel
    {
        public bool IsInit { get; set; }
        public string ContentId { get; set; }
        public string Lang { get; set; }
        public string SearchText { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int? Catogray { get; set; }
        public int PageNumber { get; set; }

    }
}
