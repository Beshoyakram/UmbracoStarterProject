using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbracoStarterProject.Services
{
	public interface IReportService
	{

		public Dictionary<string, int> GetSocialStatisticsFormPage(int PageId, string Culture);

		public void SearchIntoOurTree(IPublishedContent Content, string Culture);

		public Dictionary<string, Dictionary<string, int>> GetReport();
	}
}
