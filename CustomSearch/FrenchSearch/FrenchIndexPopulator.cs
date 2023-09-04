using Examine;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace UmbracoStarterProject.CustomSearch.FrenchSearch
{
    public class FrenchIndexPopulator : IndexPopulator
    {
        private readonly IContentService _contentService;
        private readonly FrenchIndexValueSetBuilder _frenchIndexValueSetBuilder;

        public FrenchIndexPopulator(IContentService contentService, FrenchIndexValueSetBuilder frenchIndexValueSetBuilder)
        {
            _contentService = contentService;
            _frenchIndexValueSetBuilder = frenchIndexValueSetBuilder;
            RegisterIndex("FrenchIndex");
        }
        protected override void PopulateIndexes(IReadOnlyList<IIndex> indexes)
        {
            foreach (var index in indexes)
            {
                var roots = _contentService.GetRootContent().Where(c => c.ContentType.Alias == HomePage.ModelTypeAlias);

                index.IndexItems(_frenchIndexValueSetBuilder.GetValueSets(roots.ToArray()));

                foreach (var root in roots)
                {
                    var valueSets = _frenchIndexValueSetBuilder.GetValueSets(_contentService.GetPagedDescendants(root.Id, 0, int.MaxValue, out _).ToArray());
                    index.IndexItems(valueSets);
                }
            }

        }
    }
}
