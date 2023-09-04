using Examine;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Examine;
using static Umbraco.Cms.Core.Constants.Conventions;

namespace UmbracoStarterProject.CustomSearch.EnglishSearch
{
    public class EnglishIndexValueSetBuilder : IValueSetBuilder<IContent>
    {
        public IEnumerable<ValueSet> GetValueSets(params IContent[] contents)
        {
            foreach (IContent content in contents)
            {
                var indexValues = new Dictionary<string, object>
                {

                    // this is a special field used to display the content name in the Examine dashboard
                    [UmbracoExamineFieldNames.NodeNameFieldName] = content.GetValue("title", culture: "en-US")!,
                    ["name"] = content.GetValue("title", culture: "en-US")!,
                    // add the fields you want in the index
                    ["nodeName"] = content.GetValue("title", culture: "en-US")!,
                    ["id"] = content.Id,
                };

                foreach (var property in content.Properties)
                {
                    indexValues.Add(property.Alias, property.GetValue(culture: "en-US", published: true));
                }

                yield return new ValueSet(content.Id.ToString(), "content", content.ContentType.Alias, indexValues);
            }


        }
    }
}
