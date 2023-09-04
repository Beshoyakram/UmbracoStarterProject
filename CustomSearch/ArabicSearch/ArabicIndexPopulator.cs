using Examine;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace UmbracoStarterProject.CustomSearch.ArabicSearch
{
    public class ArabicIndexPopulator : IndexPopulator
    {
        private readonly IContentService _contentService;
        private readonly ArabicIndexValueSetBuilder _arabicIndexValueSetBuilder;

        public ArabicIndexPopulator(IContentService contentService,
            ArabicIndexValueSetBuilder arabicIndexValueSetBuilder)
        {
            _contentService = contentService;
            _arabicIndexValueSetBuilder = arabicIndexValueSetBuilder;
            RegisterIndex("ArabicIndex");
        }
        protected override void PopulateIndexes(IReadOnlyList<IIndex> indexes)
        {
            foreach (var index in indexes)
            {
                var roots = _contentService.GetRootContent().Where(c => c.ContentType.Alias == HomePage.ModelTypeAlias);

                index.IndexItems(_arabicIndexValueSetBuilder.GetValueSets(roots.ToArray()));

                foreach (var root in roots)
                {
                    var valueSets = _arabicIndexValueSetBuilder.GetValueSets(_contentService.GetPagedDescendants(root.Id, 0, int.MaxValue, out _).ToArray());
                    index.IndexItems(valueSets);
                }
            }

        }
    }
}
