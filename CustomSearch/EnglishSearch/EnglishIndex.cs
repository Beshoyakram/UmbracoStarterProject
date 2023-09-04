using Examine.Lucene;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;

namespace UmbracoStarterProject.CustomSearch.EnglishSearch
{
    public class EnglishIndex : UmbracoExamineIndex
    {
        public EnglishIndex(
            ILoggerFactory loggerFactory,
            string name,
            IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions,
            Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
            IRuntimeState runtimeState)
            : base(loggerFactory,
            name,
            indexOptions,
            hostingEnvironment,
            runtimeState)
        {
            PublishedValuesOnly = true;
            EnableDefaultEventHandler = true;
            RunAsync = true;
        }
    }
}
