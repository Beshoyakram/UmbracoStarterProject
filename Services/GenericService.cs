using Azure;
using System.Drawing.Printing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Common;
using UmbracoStarterProject.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UmbracoStarterProject.Services
{
    public class GenericService :IGenericService
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public GenericService(UmbracoHelper umbracoHelper, IVariationContextAccessor variationContextAccessor)
        {
            _umbracoHelper = umbracoHelper;
            _variationContextAccessor = variationContextAccessor;
        }

        public List<GenericDetailsPage> GetAll(FilterModel model) {

            _variationContextAccessor.VariationContext = new VariationContext(model.Lang);
            var currentPage = _umbracoHelper.Content(model.ContentId);
            var childs = new List<GenericDetailsPage>();
            if (currentPage != null)
            {
                 childs = currentPage?.Children.OfType<GenericDetailsPage>()?
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.CreateDate).ToList() ?? null;
                if (!string.IsNullOrEmpty(model.SearchText))
                {
                    childs = childs.Where(x =>
                                x.Title.ToString().ToLower().Contains(model.SearchText.ToLower()) ||
                                x.Brief.ToString().ToLower().Contains(model.SearchText.ToLower()) ||
                                x.Description.ToString().ToLower().Contains(model.SearchText.ToLower())
                            ).ToList();
                }
                if (!string.IsNullOrEmpty(model.DateFrom))
                {
                    childs = childs.Where(x => x.Date.Date >= DateTime.Parse(model.DateFrom).Date).ToList();
                }
                if (!string.IsNullOrEmpty(model.DateTo))
                {
                    childs = childs.Where(x => x.Date.Date <= DateTime.Parse(model.DateTo).Date).ToList();
                }
                if (model.Catogray != null && model.Catogray > 0)
                {
                    childs = childs
                        .Where(x => x.Category!=null  && x.Category?.Id == model.Catogray).ToList();
                }
       
            }
            return childs;
        }

		public IEnumerable<IPublishedContent> GetchildernOfContent(string culture, int docId)
		{
			_variationContextAccessor.VariationContext = new VariationContext(culture);

			var result = new List<IPublishedContent>();
			IPublishedContent contents = null;

			contents = _umbracoHelper.Content(docId);
			if (contents != null)
			{
				result = contents.Children.ToList();

			}
			return result;
		}

	}
}
