using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{ 
	public interface ISearchService
    {
        long SearchTotalCount { get; }

        List<string> GetSearchResultIds(List<string> docTypeAlias, string searchKeyWord, string Culture = "ar-eg");
       
        IEnumerable<string> GetSearchDocTypes( string culture);
    }
}
