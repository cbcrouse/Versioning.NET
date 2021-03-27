using Common.Extensions;
using Common.Tests.TestClasses;
using Xunit;

namespace Common.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void HasEmptyConstructor_ReturnsTrue_WithEmptyConstructor()
        {
            // Arrange & Assert
            var hasEmptyConstructor = typeof(ClassWithEmptyConstructor).HasEmptyConstructor();

            // Assert
            Assert.True(hasEmptyConstructor);
        }

        [Fact]
        public void HasEmptyConstructor_ReturnsFalse_WithNoEmptyConstructor()
        {
            // Arrange & Assert
            var hasEmptyConstructor = typeof(ClassWithNoEmptyConstructor).HasEmptyConstructor();

            // Assert
            Assert.False(hasEmptyConstructor);
        }
    }
}
