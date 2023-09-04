using System.Diagnostics.Metrics;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using J2N.Numerics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Polly.Caching;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoStarterProject.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Umbraco.Cms.Core.Constants.HttpContext;


namespace UmbracoStarterProject.Controllers
{

	public class CustomReportsController : UmbracoApiController
	{
		private readonly IVariationContextAccessor _VariationContextAccessor;
		public UmbracoHelper _UmbracoHelper;
		public IContentService _ContentService;
		public IReportService _reportService;


		public CustomReportsController(IVariationContextAccessor variationContextAccessor
			, UmbracoHelper umbracoHelper, IContentService contentService
			, IReportService iReportService)
		{
			_VariationContextAccessor = variationContextAccessor;
			_UmbracoHelper = umbracoHelper;
			_ContentService = contentService;
			_reportService = iReportService;
		}


		[HttpGet]
		public IActionResult SocialSharing(int PageId, string Culture = "ar-EG")
		{

			_VariationContextAccessor.VariationContext = new VariationContext(Culture);

			try
			{
				Dictionary<string, Dictionary<string, int>> report = new Dictionary<string, Dictionary<string, int>>();

				var page = _ContentService.GetById(PageId);
				var Content = _UmbracoHelper.Content(PageId);

				if (page != null && Content != null)
				{

					_reportService.SearchIntoOurTree(Content, Culture);
					report = _reportService.GetReport();


					#region Arrange The report
					Dictionary<string, string[]> PrintedData = new Dictionary<string,string[]>();

					int[] TotalRecord = new int[4];
					foreach (var item in report)
					{
						string[] arrangedArr = new string[item.Value.Count];

						int index = 0;
						foreach (var row in item.Value) 
						{
							arrangedArr[index] =row.Value.ToString();
							TotalRecord[index] += row.Value;
							index++;
						}

						PrintedData.Add(item.Key, arrangedArr);
					}
					PrintedData.Add("Total", TotalRecord.Select(i => i.ToString()).ToArray());
					#endregion

					#region Write data int CSV
					var cultureInfo = CultureInfo.GetCultureInfo(Culture);
					var csvConfig = new CsvConfiguration(cultureInfo)
					{
						// You can customize delimiter, quoting, etc. here if needed
						Delimiter = ",",
						Encoding = Encoding.UTF8
					};
					string csvFilePath = "exported_data.csv";

					// Write data to the CSV file using CsvHelper
					using (var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
					using (var csv = new CsvWriter(writer,csvConfig))
					{
						// Map the dictionary to a list of MyData objects
						//var records = report;

						var records = PrintedData.Select(data => new 
						{
							PageName = data.Key,
							Facebook = data.Value[0],
							WhatsApp = data.Value[1],
							Twitter = data.Value[2],
							CopyLink = data.Value[3]
						});

						// Write the records to the CSV file
						csv.WriteRecords(records);
					}


					#endregion




				}
				else
				{
					throw new Exception($"There is no page for this id : {PageId}");
				}
				return Ok();


			}
			catch (Exception ex)
			{ throw new Exception(ex.Message); }

		}








	}
}
