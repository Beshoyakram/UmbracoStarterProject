using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
    public interface IHomeService
    {
        public List<SliderBanner> GetsliderBanner(string lang);
    }
}
