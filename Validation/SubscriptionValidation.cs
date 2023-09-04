using System.Text.RegularExpressions;
using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Validation
{
    public class SubscriptionValidation
    {
        public (bool IsValid, string ErrorMessage) ValidSubscription(SubscriptionModel submitSubscriptionModel)
        {
            if (submitSubscriptionModel == null)
                return (false, "Please, enter required data");
            if (!string.IsNullOrEmpty(submitSubscriptionModel.EmailOfSubscription) &&
                !IsValidEmail(submitSubscriptionModel.EmailOfSubscription))
                return (false, "Please, enter correct Email ");
            if (!string.IsNullOrEmpty(submitSubscriptionModel.EmailOfSubscription))
            {
            
                return (true, "success");
            }
            else
                return (false, "Title and mail are require ");

        }

        public static bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (string.IsNullOrEmpty(email))
                return false;

            Regex regex = new Regex(emailPattern);
            return regex.IsMatch(email);
        }
    }
}
