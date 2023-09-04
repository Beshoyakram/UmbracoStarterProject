using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoStarterProject.Models;

namespace UmbracoStarterProject.Services
{
    public interface IGenericService
    {
         List<GenericDetailsPage> GetAll(FilterModel model);
        IEnumerable<IPublishedContent> GetchildernOfContent(string culture, int docId);

	}
}
