using Examine.Lucene;
using Examine;
using Lucene.Net.Analysis.En;
using Lucene.Net.Index;
using Lucene.Net.Util;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using UmbracoStarterProject.Models;
using Lucene.Net.Analysis.Ar;

namespace UmbracoStarterProject.CustomSearch.EnglishSearch
{
    public class ConfigureEnglishIndexOptions : IConfigureNamedOptions<LuceneDirectoryIndexOptions>
    {
        private readonly IOptions<IndexCreatorSettings> _settings;
        private readonly SearchOptions _searchOptions;

        public ConfigureEnglishIndexOptions(
            IOptions<IndexCreatorSettings> settings,
            IOptions<SearchOptions> searchOptions)
        {
            _settings = settings;
            _searchOptions = searchOptions.Value;
        }
        public void Configure(string name, LuceneDirectoryIndexOptions options)
        {

            if (name.Equals("EnglishIndex"))
            {
                options.Analyzer = new EnglishAnalyzer(LuceneVersion.LUCENE_48);
                options.FieldDefinitions = new FieldDefinitionCollection(new FieldDefinition("id", FieldDefinitionTypes.Integer));

                options.UnlockIndex = true;

                if (_settings.Value.LuceneDirectoryFactory == LuceneDirectoryFactory.SyncedTempFileSystemDirectoryFactory)
                {
                    // if this directory factory is enabled then a snapshot deletion policy is required
                    options.IndexDeletionPolicy = new SnapshotDeletionPolicy(new KeepOnlyLastCommitDeletionPolicy());
                }
            }
        }

        // Part of the interface, but does not need to be implemented for this.
        public void Configure(LuceneDirectoryIndexOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
