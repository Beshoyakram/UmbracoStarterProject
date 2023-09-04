using Examine;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Examine;
using UmbracoStarterProject.CustomSearch.ArabicSearch;
using UmbracoStarterProject.CustomSearch.EnglishSearch;
using UmbracoStarterProject.CustomSearch.FrenchSearch;

namespace UmbracoStarterProject.CustomSearch
{
    public class SearchComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Services.AddExamineLuceneIndex<ArabicIndex, ConfigurationEnabledDirectoryFactory>("ArabicIndex");
			builder.Services.AddSingleton<ArabicIndexValueSetBuilder>();
			builder.Services.AddSingleton<IIndexPopulator, ArabicIndexPopulator>();
			builder.Services.ConfigureOptions<ConfigureArabicIndexOptions>();
            builder.AddNotificationHandler<ContentCacheRefresherNotification, ArabicIndexingNotificationHandler>();

            builder.Services.AddExamineLuceneIndex<EnglishIndex, ConfigurationEnabledDirectoryFactory>("EnglishIndex");
			builder.Services.AddSingleton<EnglishIndexValueSetBuilder>();
			builder.Services.AddSingleton<IIndexPopulator, EnglishIndexPopulator>();
			builder.Services.ConfigureOptions<ConfigureEnglishIndexOptions>();
            builder.AddNotificationHandler<ContentCacheRefresherNotification, EnglishIndexingNotificationHandler>();

            builder.Services.AddExamineLuceneIndex<FrenchIndex, ConfigurationEnabledDirectoryFactory>("FrenchIndex");
            builder.Services.AddSingleton<FrenchIndexValueSetBuilder>();
            builder.Services.AddSingleton<IIndexPopulator, FrenchIndexPopulator>();
            builder.Services.ConfigureOptions<ConfigureFrenchIndexOptions>();
            builder.AddNotificationHandler<ContentCacheRefresherNotification, FrenchIndexingNotificationHandler>();

        }
    }
}
