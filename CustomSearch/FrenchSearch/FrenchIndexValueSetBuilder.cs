using Examine;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Examine;

namespace UmbracoStarterProject.CustomSearch.FrenchSearch
{
    public class FrenchIndexValueSetBuilder : IValueSetBuilder<IContent>
    {
        public IEnumerable<ValueSet> GetValueSets(params IContent[] contents)
        {
            foreach (var content in contents)
            {

                var indexValues = new Dictionary<string, object>
                {

                    // this is a special field used to display the content name in the Examine dashboard
                    [UmbracoExamineFieldNames.NodeNameFieldName] = content.GetValue("title", culture: "fr-FR")!,
                    ["name"] = content.GetValue("title", culture: "fr-FR")!,
                    // add the fields you want in the index
                    ["nodeName"] = content.GetValue("title", culture: "fr-FR")!,
                    ["id"] = content.Id,
                };
                foreach (var property in content.Properties)
                {
                    indexValues.Add(property.Alias, property.GetValue(culture: "fr-FR", published: true));

                }
                yield return new ValueSet(content.Id.ToString(), "content", content.ContentType.Alias, indexValues);

            }
        }
    }
}
