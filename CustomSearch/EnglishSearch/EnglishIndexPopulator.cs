using Examine;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace UmbracoStarterProject.CustomSearch.EnglishSearch
{
    public class EnglishIndexPopulator : IndexPopulator
    {
        private readonly IContentService _contentService;
        private readonly EnglishIndexValueSetBuilder _englishIndexValueSetBuilder;

        public EnglishIndexPopulator(IContentService contentService, EnglishIndexValueSetBuilder englishIndexValueSetBuilder)
        {
            _contentService = contentService;
            _englishIndexValueSetBuilder = englishIndexValueSetBuilder;
            RegisterIndex("EnglishIndex");
        }
        protected override void PopulateIndexes(IReadOnlyList<IIndex> indexes)
        {
            foreach (var index in indexes)
            {
                var roots = _contentService.GetRootContent().Where(c => c.ContentType.Alias == HomePage.ModelTypeAlias);

                index.IndexItems(_englishIndexValueSetBuilder.GetValueSets(roots.ToArray()));

                foreach (var root in roots)
                {
                    var valueSets = _englishIndexValueSetBuilder.GetValueSets(_contentService.GetPagedDescendants(root.Id, 0, int.MaxValue, out _).ToArray());
                    index.IndexItems(valueSets);
                }
            }


        }
    }
}
