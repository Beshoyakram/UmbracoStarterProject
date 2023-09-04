using Examine;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Examine;

namespace UmbracoStarterProject.CustomSearch.ArabicSearch
{
    public class ArabicIndexValueSetBuilder : IValueSetBuilder<IContent>
    {

        public IEnumerable<ValueSet> GetValueSets(params IContent[] contents)
        {
            foreach (IContent content in contents)
            {
                var indexValues = new Dictionary<string, object>
                {

                    // this is a special field used to display the content name in the Examine dashboard
                    [UmbracoExamineFieldNames.NodeNameFieldName] = content.GetValue("title", culture: "ar-EG")!,
                    ["name"] = content.GetValue("title", culture: "ar-EG")!,
                    // add the fields you want in the index
                    ["nodeName"] = content.GetValue("title", culture: "ar-EG")!,
                    ["id"] = content.Id,
                };

                foreach (var property in content.Properties)
                {
                    indexValues.Add(property.Alias, property.GetValue(culture: "ar-EG", published: true));
                }

                yield return new ValueSet(content.Id.ToString(), "content", content.ContentType.Alias, indexValues);
            }
        }


    }

}
