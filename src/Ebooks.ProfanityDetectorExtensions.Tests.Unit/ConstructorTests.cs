using Ebooks.ProfanityDetector;
using Xunit;

namespace Ebooks.ProfanityDetectorExtensions.Tests.Unit
{
    public class ConstructorTests
    {
        [Fact]
        public void Constructor_NoExceptionsThrown()
        {
            // This example is used in the default README.md so 
            // we should strive to make sure it always passes
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var filter = new ProfanityFilter().UseDefaults();
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        [Fact]
        public void Constructor_ProfanitiesExist()
        {
            var filter = new ProfanityFilter().UseDefaults();
            Assert.NotEmpty(filter.Terms.Prohibited);
        }
    }
}
