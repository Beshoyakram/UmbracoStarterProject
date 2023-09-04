using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Validation
{
	public class ContactUsFormValidation
	{
		public (bool IsValid, string ErrorMessage) ValidForm(SubmissionModel submissionViewModel)
		{
			if (submissionViewModel == null)
				return (false, "Please, enter required data");
			if (!string.IsNullOrEmpty(submissionViewModel.Name) &&
				!string.IsNullOrEmpty(submissionViewModel.MsgContent) &&
				(!string.IsNullOrEmpty(submissionViewModel.Phone) || !string.IsNullOrEmpty(submissionViewModel.Email)))
			{
				return (true, "success");
			}
			else
				return (false, "Full name , Message title and Message are required , and phone or mail");

		}
	}
}
