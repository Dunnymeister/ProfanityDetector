using Ebooks.ProfanityDetector;
using System;
using Xunit;

namespace Ebooks.ProfanityDetectorExtensions.Tests.Unit
{
    public class ConstructorTests
    {
        [Fact]
        public void Constructor_NoExceptionsThrown()
        {
            var filter = new ProfanityFilter().UseDefaults();
        }

        [Fact]
        public void Constructor_ProfanitiesExist()
        {
            var filter = new ProfanityFilter().UseDefaults();
            Assert.NotEmpty(filter.Terms.Prohibited);
        }
    }
}
