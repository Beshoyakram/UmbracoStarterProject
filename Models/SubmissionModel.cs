namespace UmbracoStarterProject.Models
{
	public class SubmissionModel
	{
		public int contactUsId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string MsgTitle { get; set; }
		public string MsgContent { get; set; }
		public string ReCaptchaToken { get; set; }
		public IFormFile? uploadFile { get; set; } 

    }
}
