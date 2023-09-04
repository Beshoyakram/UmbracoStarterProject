using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;
using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
    public class LookupService : ILookupService
    {
        private readonly UmbracoHelper _umbracoHelper;

        public LookupService(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        public List<LookupItemModel> GetLookupList(string docTypeAlias, string culture)
        {
            var result = new List<LookupItemModel>();
            var lookupListNode = _umbracoHelper.ContentAtXPath($"//{docTypeAlias}")?
                .FirstOrDefault();
        
            if (lookupListNode != null)
            {
                var lookupListNodes = lookupListNode.Children();

                if (lookupListNodes != null && lookupListNodes.Any())
                {
                    foreach (var lookup in lookupListNodes)
                    {
                        result.Add(new LookupItemModel()
                        {
                            Id = lookup.Id,
                            LookupName = lookup.Value<string>("Title")
                        });
                    }
                }

            }
            return result;
         }

       
    }
}
