using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
    public interface ILookupService
    {
        List<LookupItemModel> GetLookupList(string docTypeAlias, string culture);
    }
}
