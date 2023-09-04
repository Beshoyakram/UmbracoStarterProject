using Microsoft.AspNetCore.Mvc;
using System.Web;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoStarterProject.Models;
using UmbracoStarterProject.Services;

namespace UmbracoStarterProject.Controllers
{
    public class GenericListingController : UmbracoApiController
    {
        private readonly IConfiguration _config;
        private readonly IGenericService _genericService;
        private readonly ILookupService _lookupService;
		
		public GenericListingController(IConfiguration config, IGenericService genericService, ILookupService lookupService)
		{
			_config = config;
			_genericService = genericService;
			_lookupService = lookupService;
			
		}
		[HttpPost]
        public ResponseObject GetAll(FilterModel model)
        {
            var response = new ResponseObject { Success = false };
            try
            {
                var pageSize = int.Parse(_config["GenericListPageSize"]);
                var Data = new GenericDetailsResponse();
                var categories = new List<LookupItemModel>();
                var GenericDetailsList = new List<GenericDetailsModel>();
                var childs = _genericService.GetAll(model);
                if (childs.Count() > 0)
                {
                    foreach (var item in childs)
                    {
                        GenericDetailsList.Add(new GenericDetailsModel()
                        {
                            Title = item.Title,
                            Brief = item.Brief,
                           Image = item.CoverImage != null ? item.CoverImage.GetCropUrl("defaultCrop") : null,
                            Url = item?.Url()?.ToString(),
                            CategoryName=item?.Category?.Value<string>("title"),
                            Date = (item.Date > DateTime.MinValue) ? item.Date.ToString("dddd dd MMMM yyyy").ToString() : string.Empty
                        });
                    }
                }
                if (model.IsInit)
                {
                    categories = _lookupService.GetLookupList(CategoryList.ModelTypeAlias, model.Lang);

                  
                }

                if (GenericDetailsList.Count() > 0)
                {
                    GenericDetailsList = GenericDetailsList.Skip(pageSize * (model.PageNumber - 1)).Take(pageSize).ToList();
                }


                Data.GenericDetailsModels = GenericDetailsList;
                Data.Categories = categories;
                response.Success = true;
                response.Data = Data;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

		

	}
}
