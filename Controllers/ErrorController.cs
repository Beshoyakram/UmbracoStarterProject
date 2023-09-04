using Microsoft.AspNetCore.Mvc;
using UmbracoStarterProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;

namespace UmbracoStarterProject.Controllers
{
    public class ErrorController : Controller
    {
        private readonly UmbracoHelper _umbracoHelper;

        public ErrorController(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }
        [HttpGet]
        [Route("Error/Index")]
        public IActionResult Index()
        {
            var homePage = _umbracoHelper.ContentAtXPath($"//{HomePage.ModelTypeAlias}").Cast<HomePage>().FirstOrDefault();

            var model = new ErrorModel
            {
                ErrorMessage = "Internal Server Error",
                GoHomePage = "Go To Homepage",
                SiteName= homePage.SiteTitle,
                FormalWesiteforUmbracoStarterProject = "Something Went Wrong",
                FavIconUrl=homePage.FavIcon.Url()
            };
            return View("~/Views/Error/Index.cshtml", model);
        }

    
    }
}
