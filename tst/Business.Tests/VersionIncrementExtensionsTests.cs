using Application.Extensions;
using Domain.Enumerations;
using System;
using Xunit;

namespace Business.Tests
{
    public class VersionIncrementExtensionsTests
    {
        [Theory]
        [InlineData(VersionIncrement.Major, VersionIncrement.Minor)]
        [InlineData(VersionIncrement.Minor, VersionIncrement.Patch)]
        [InlineData(VersionIncrement.Patch, VersionIncrement.Patch)]
        [InlineData(VersionIncrement.None, VersionIncrement.None)]
        public void CanLowerIncrement(VersionIncrement initial, VersionIncrement expected)
        {
            // Act
            var actual = initial.Lower();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Lower_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var badIncrement = (VersionIncrement) 10;

            // Act
            VersionIncrement Act()
            {
                return badIncrement.Lower();
            }

            // Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Act());
            Assert.IsType<ArgumentOutOfRangeException>(ex);
        }
    }
}
