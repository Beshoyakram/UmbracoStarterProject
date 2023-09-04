using Examine;
using Examine.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoStarterProject.Models;
using static Umbraco.Cms.Core.Constants;
using UmbracoStarterProject.Models;
using System.Diagnostics.Tracing;
using System.Globalization;
using Lucene.Net.Util;

namespace UmbracoStarterProject.Services
{
	public class SearchService : ISearchService
	{
		private readonly IConfiguration _config;
		private readonly UmbracoHelper _umbracoHelper;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IExamineManager _examineManager;
		private readonly SearchOptions _searchOptions;
		private readonly AppCaches appCaches;
		private readonly ILogger<SearchService> logger;
		private string _culture = string.Empty;

		public long SearchTotalCount { get; private set; }

		public SearchService(UmbracoHelper umbracoHelper, IVariationContextAccessor variationContextAccessor,
			IExamineManager examineManager,
			IOptions<SearchOptions> searchOptions,
			AppCaches appCaches,
			ILogger<SearchService> logger,
			IConfiguration config)
		{
			_umbracoHelper = umbracoHelper;
			_variationContextAccessor = variationContextAccessor;
			_examineManager = examineManager;
			_searchOptions = searchOptions.Value;
			this.appCaches = appCaches;
			this.logger = logger;
			_config = config;
		}

		public List<string> GetSearchResultIds(List<string> docTypeAlias, string searchKeyWord, string culture = "ar-EG")
		{
			_culture = culture;
			int pageSize = int.Parse(_searchOptions.PageSize);
			_variationContextAccessor.VariationContext = new VariationContext(culture);
			//var searchResultModel = new SearchResultModel();
			IIndex index = null;
			string cultureCode = culture.Split('-')[0];
			if (cultureCode == "ar")
				_examineManager.TryGetIndex("ArabicIndex", out index);
			else if (cultureCode == "en")
				_examineManager.TryGetIndex("EnglishIndex", out index);
			else if (cultureCode == "fr")
				_examineManager.TryGetIndex("FrenchIndex", out index);
			else
				_examineManager.TryGetIndex("ExternalIndex", out index);

			if (!string.IsNullOrEmpty(searchKeyWord) && searchKeyWord.Trim() != "" && searchKeyWord.Length >= 3 && docTypeAlias.Any())
			{
				bool useFuzzyness = true;
			Start:

				float.TryParse(_searchOptions.Fuzzyness, out float fuzzyValue);
				//var term = searchKeyWord.Fuzzy(fuzzyValue);

				var searcher = index.Searcher;
				var criteria = searcher.CreateQuery(IndexTypes.Content, BooleanOperation.Or);
				var examineQuery = criteria.GroupedOr(new[] { "__NodeTypeAlias" }, docTypeAlias.ToArray());

				string[] terms = searchKeyWord.Trim().Split(' ');
				List<IExamineValue> searchWordsValues = new List<IExamineValue>();

				if (useFuzzyness)
				{
					foreach (string word in terms)
					{
						if (!word.IsNullOrWhiteSpace())
						{
							searchWordsValues.Add(word.Fuzzy(fuzzyValue));

							string trimmedWord = word.Replace("آ", "ا").Replace("أ", "ا").Replace("إ", "ا").Trim("ال");
							if (trimmedWord.Length >= 4 && (trimmedWord.EndsWith("ة") || trimmedWord.EndsWith("ه")))
							{
								trimmedWord = trimmedWord.Trim("ة").Trim("ه");
							}

							searchWordsValues.Add(trimmedWord.Fuzzy(fuzzyValue));
						}
					}
				}

				var searchFieldsSplitted = _searchOptions.Fields.Split(',');
				var searchFields = new List<string>();
				searchFields.AddRange(searchFieldsSplitted);
				searchFields.AddRange(searchFieldsSplitted.Select(f => f + "_" + culture.ToLower()));

				if (useFuzzyness)
				{
					examineQuery.And().GroupedOr(searchFields, searchWordsValues.ToArray());
				}
				else
				{
					examineQuery.And().GroupedOr(searchFields, terms);
				}

				var searchResult = examineQuery.Execute();
				if (searchResult.TotalItemCount <= 0)
				{
					if (useFuzzyness)
					{
						useFuzzyness = false;
						goto Start;
					}
				}

				var results = searchResult.OrderByDescending(x => x.Score);
				SearchTotalCount = searchResult.TotalItemCount;
				var _return = results.Select(x => x.Id).ToList();
				return _return;
			}
			SearchTotalCount = 0;
			return new List<string>();
		}


		#region Get Documents Types
		public IEnumerable<string> GetSearchDocTypes(string culture)
		{
			_culture = culture;

			string cacheKey = $"searchDocTypes";
			return appCaches.RuntimeCache.GetCacheItem(cacheKey, () => _GetSearchDocTypes(culture));
		}
		private IEnumerable<string> _GetSearchDocTypes(string culture)
		{
			_culture = culture;

			var doctypes = _umbracoHelper.ContentAtXPath($"//{HomePage.ModelTypeAlias}").
				FirstOrDefault()?.
				Children(c => !_searchOptions.ExcludedTypes.Contains(c.ContentType.Alias) &&
							  !_searchOptions.ExcludedNodes.Contains(c.Id))?.
				DescendantsOrSelf<IPublishedContent>().
				Where(c => !_searchOptions.ExcludedTypes.Contains(c.ContentType.Alias) &&
							  !_searchOptions.ExcludedNodes.Contains(c.Id)).
				Select(c => c.ContentType.Alias).
				Distinct().
				ToList();

			return doctypes ?? new List<string>();
		}
		#endregion




	}
}
