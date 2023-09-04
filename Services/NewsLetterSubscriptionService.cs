using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace UmbracoStarterProject.Services
{
    public class NewsLetterSubscriptionService : INewsLetterSubscriptionService
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public NewsLetterSubscriptionService(UmbracoHelper umbracoHelper, IVariationContextAccessor variationContextAccessor)
        {
            _umbracoHelper = umbracoHelper;
            _variationContextAccessor = variationContextAccessor;
        }

        public NewsLetterSubscriptionList? GetSubscriptionList(string culture)
        {
            _variationContextAccessor.VariationContext = new VariationContext(culture);

            return _umbracoHelper.ContentAtXPath($"//{NewsLetterSubscriptionList.ModelTypeAlias}")?
                    .FirstOrDefault()?.SafeCast<NewsLetterSubscriptionList>();

        }

    }
}
