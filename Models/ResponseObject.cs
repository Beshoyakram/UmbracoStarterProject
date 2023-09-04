namespace UmbracoStarterProject.Models
{
    public class ResponseObject
    {
		public int TotalCount;

		public Object Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
