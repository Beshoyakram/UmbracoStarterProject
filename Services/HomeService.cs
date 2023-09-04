using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common;
using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
    public class HomeService : IHomeService
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IConfiguration _config;

        public HomeService(UmbracoHelper umbracoHelper, IVariationContextAccessor variationContextAccessor, IConfiguration config)
        {
            _umbracoHelper = umbracoHelper;
            _variationContextAccessor = variationContextAccessor;
            _config = config;
        }

        public List<SliderBanner> GetsliderBanner(string lang)
        {
            _variationContextAccessor.VariationContext = new VariationContext(lang);
            var homePage = _umbracoHelper.ContentAtXPath($"//{HomePage.ModelTypeAlias}").Cast<HomePage>().FirstOrDefault();
            List<SliderBanner> sliderBanner = new();
            string pageTitle = "";
            var sliderbannersBlocks = homePage?.SliderBannerSelection?.Select(x => x.Content);
            if (sliderbannersBlocks != null)
            {
                foreach (var item in sliderbannersBlocks)
                {
                    if (item.GetType() == typeof(InternalLink))
                    {
                        IEnumerable<IPublishedContent> ListOfLinks = 
                            item.Value<IEnumerable<IPublishedContent>>("link") != null ?
                         item.Value<IEnumerable<IPublishedContent>>("link") : null;

                        if (ListOfLinks != null)
                        {
                            foreach (var link in ListOfLinks)
                            {
                                if (link.IsPublished(lang))
                                {
                                    var Name = link.Value<string>("title") != null ? link.Value<string>("title") : "";
                                    var Url = link.Url() != null ? link.Url() : "";
                                    sliderBanner.Add(new()
                                    {
                                        Id = link.Id,
                                        Title = Name,
                                        Url = Url,
                                        IsInternal = true
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        var announcementData = item?.SafeCast<ExternalLink>();
                        if (announcementData != null)
                        {
                            sliderBanner.Add(new()
                            {
                                Title = announcementData?.Title,
                                Url = announcementData?.Link,
                                IsInternal = false
                            });
                        }
                    }

                }
            }
           
            return sliderBanner;

        }
    }
}
