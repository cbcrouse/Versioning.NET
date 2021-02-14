using Domain.Constants;
using System.Text.RegularExpressions;
using Xunit;

namespace Business.Tests
{
    public class RegexPatternsTests
    {
        [Theory]
        [InlineData("9ed2a15 ", true)]
        [InlineData("9ed2a15", false)]
        public void MatchesCommitId(string commitId, bool expectedResult)
        {
            // Arrange
            var sut = new Regex(RegexPatterns.GitLogCommitId);

            // Act
            bool result = sut.Match(commitId).Success;

            // Act & Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("test(Test): Added ability to test", true)]
        [InlineData("test(Different Test): test", true)]
        [InlineData("test(Different Test) : test", true)]
        [InlineData("test(Different Test): ", false)]
        [InlineData("(Different Test): test", false)]
        public void MatchesCommitSubject(string commitId, bool expectedResult)
        {
            // Arrange
            var sut = new Regex(RegexPatterns.GitLogCommitSubject);

            // Act
            bool result = sut.IsMatch(commitId);

            // Act & Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
