using Umbraco.Cms.Web.Common.PublishedModels;

namespace UmbracoStarterProject.Services
{
    public interface INewsLetterSubscriptionService
    {
        public NewsLetterSubscriptionList GetSubscriptionList(string culture);

    }
}
