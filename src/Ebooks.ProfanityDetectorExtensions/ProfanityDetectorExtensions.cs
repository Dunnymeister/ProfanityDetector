using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Ebooks.ProfanityDetector
{
    public static class ProfanityDetectorExtensions
    {
        public static ProfanityFilter UseDefaults(this ProfanityFilter filter)
        {
            // Read out the default filter object
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Ebooks.ProfanityDetector.Extensions.Resources.en_US.Terms.json";
            string result;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            
            var terms = JsonConvert.DeserializeObject<Terms>(result);

            // Append it to the terms already in the ProfanityFilter
            filter.Terms.Prohibited.UnionWith(terms.Prohibited);
            filter.Terms.Permitted.UnionWith(terms.Permitted);

            // Return the instance back to allow for chaining
            return filter;
        }
    }
}
