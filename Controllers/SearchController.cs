using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using UmbracoStarterProject.Models;
using UmbracoStarterProject.Services;

namespace UmbracoStarterProject.Controllers
{
	public class SearchController : UmbracoApiController
	{
		private readonly ISearchService _searchService;
		private readonly UmbracoHelper _umbracoHelper;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly SearchOptions _searchOptions;
		private readonly ILogger<SearchController> _logger;



		public SearchController(ISearchService searchService, UmbracoHelper umbracoHelper,
			IVariationContextAccessor variationContextAccessor, IOptions<SearchOptions> searchOptions, ILogger<SearchController> logger)
		{
			_searchService = searchService;
			_umbracoHelper = umbracoHelper;
			_variationContextAccessor = variationContextAccessor;
			_searchOptions = searchOptions.Value;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Index(string text, int order = -1, string culture = "ar-EG", int pageNumber = 1)
		{
			try
			{

				_variationContextAccessor.VariationContext = new VariationContext(culture);
				int.TryParse(_searchOptions.PageSize, out int pageSize);

				var docTypes = _searchService.GetSearchDocTypes(culture)?.ToList();

				if (docTypes == null || docTypes.Count == 0)
				{
					return NotFound();
				}
				var excludedNodes = _umbracoHelper.Content(_searchOptions.ExcludedNodes);
				var resultIds = _searchService.GetSearchResultIds(docTypes, text, culture);
				var contents = _umbracoHelper.Content(resultIds).
					Where(c => c.IsPublished(culture) && !excludedNodes.Any(en => en.IsAncestorOrSelf(c)));

				//For getting parent page for pages without template.
				List<IPublishedContent> ContentAfterFilter = new List<IPublishedContent>();
				foreach (var item in contents)
				{
					if (item != null)
					{
						bool flag = false;
						var NextItem = item;
						while (!flag)
						{
							if (NextItem.TemplateId != null)
							{
								if(!ContentAfterFilter.Contains(NextItem))
									ContentAfterFilter.Add(NextItem);
								flag = true;
								break;
							}
							else
							{
								if (NextItem.Parent != null)
									NextItem = NextItem.Parent;
								else
								{
									//if there is no page with template in this tree
									flag = true;
									break;
								}
							}
						}
					}
				}


				var response = new ResponseObject
				{
					Success = true,
					TotalCount = ContentAfterFilter.Count()
				};

				//newest first
				if (order == 0)
				{
					ContentAfterFilter = (List<IPublishedContent>)ContentAfterFilter.OrderByDescending(c => c.CreateDate);
				}
				//oldest first
				else if (order == 1)
				{
					ContentAfterFilter = (List<IPublishedContent>)ContentAfterFilter.OrderBy(c => c.CreateDate);
				}
				//Pagination
				ContentAfterFilter = ContentAfterFilter.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

				response.Data = ContentAfterFilter.Select(c => new
				{
					Id = c.Id,
					Title = c.HasProperty("title") ? c.Value<string>("title") : string.Empty,
					Brief = c.HasProperty("brief") ? c.Value<string>("brief") :
					c.HasProperty("description") ? c.Value<string>("description") : string.Empty,
					Url = c.Url()
				});
				return Ok(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return Ok(new ResponseObject { Success = false });
			}
		}
	}
}
