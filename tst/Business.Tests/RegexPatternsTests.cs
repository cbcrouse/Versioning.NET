using Domain.Constants;
using System.Text.RegularExpressions;
using Xunit;

namespace Business.Tests;

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
    [InlineData("test(Default Test): Added ability to test", true, "test", "Default Test", "Added ability to test")]
    [InlineData("test(Pre-Colon Space Test) : test", true, "test", "Pre-Colon Space Test", "test")]
    [InlineData("test(Special.(Char-Te$t)): test", true, "test", "Special.(Char-Te$t)", "test")]
    [InlineData("test(): No scope test", true, "test", "", "No scope test")]
    [InlineData("test(Whitespace Subject Test): ", false)]
    [InlineData("(No Type Test): test", false)]
    public void MatchesCommitSubject(string commitId, bool expectedResult, string expectedType = "", string expectedScope = "", string expectedSubject = "")
    {
        // Arrange
        var sut = new Regex(RegexPatterns.GitLogCommitSubject);

        // Act
        var match = sut.Match(commitId);

        // Act & Assert
        Assert.Equal(expectedResult, match.Success);
        Assert.Equal(expectedType, match.Groups["Type"].Value);
        Assert.Equal(expectedScope, match.Groups["Scope"].Value);
        Assert.Equal(expectedSubject, match.Groups["Subject"].Value);
    }
}