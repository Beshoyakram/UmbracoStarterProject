using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoStarterProject.Models;
using UmbracoStarterProject.Services;

namespace UmbracoStarterProject.Controllers
{
    public class HomeController :  UmbracoApiController
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet]
        public List<SliderBanner> GetsliderBanner(string lang)
        {
            try
            {
                var sliderBanners = _homeService.GetsliderBanner(lang);
                return sliderBanners;
            }
            catch
            {
                return null;
            }
        }

    }
}
