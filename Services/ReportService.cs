using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Ocsp;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Umbraco.Cms.Core.Collections.TopoGraph;

namespace UmbracoStarterProject.Services
{
	public class ReportService : IReportService
	{
		private readonly IVariationContextAccessor _VariationContextAccessor;
		public UmbracoHelper _UmbracoHelper;
		public IContentService _ContentService;


		public Dictionary<string, Dictionary<string, int>> report = new Dictionary<string, Dictionary<string, int>>();


		public ReportService(IVariationContextAccessor variationContextAccessor
			, UmbracoHelper umbracoHelper, IContentService contentService)
		{
			_VariationContextAccessor = variationContextAccessor;
			_UmbracoHelper = umbracoHelper;
			_ContentService = contentService;
		}

		public Dictionary<string, int> GetSocialStatisticsFormPage(int PageId, string Culture)
		{
			var content = _ContentService.GetById(PageId);
			var page = _UmbracoHelper.Content(PageId);

			Dictionary<string, int> dic = new Dictionary<string, int>();

			
			if (content.HasProperty("Facebook"))
			{
				int.TryParse(content.GetValue("Facebook", Culture)?.ToString(), out int propCount);
				dic.Add("Facebook", propCount);
			}
			if (content.HasProperty("WhatsApp"))
			{
				int.TryParse(content.GetValue("WhatsApp", Culture)?.ToString(), out int propCount);
				dic.Add("WhatsApp", propCount);
			}
			if (content.HasProperty("Twitter"))
			{
				int.TryParse(content.GetValue("Twitter", Culture)?.ToString(), out int propCount);
				dic.Add("Twitter", propCount);
			}
			if (content.HasProperty("CopyLink"))
			{
				int.TryParse(content.GetValue("CopyLink", Culture)?.ToString(), out int propCount);
				dic.Add("CopyLink", propCount);
			}
			
			return dic;
		}


		public void SearchIntoOurTree(IPublishedContent Content,string Culture)
		{
			var dic = GetSocialStatisticsFormPage(Content.Id, Culture);
			if (dic.Count > 0)
				report.Add(Content.Name, dic);


			if (Content.Children.Count() != 0)
			{
				foreach (var child in Content.Children)
				{
					SearchIntoOurTree(child, Culture);
				}
			}
		}


		public Dictionary<string, Dictionary<string, int>> GetReport()
		{
			return report;
		}


	}
}
