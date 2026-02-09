using Domain.Entities;
using System;
using Xunit;

namespace Business.Tests.Entities;

public class GitCommitFilterTests
{
    [Fact]
    public void Instantiation_Throws_ArgumentException_When_FromHash_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => new GitCommitFilter(null!, "notEmpty"));
        Assert.Throws<ArgumentNullException>(() => new GitCommitFilter(string.Empty, "notEmpty"));
    }

    [Fact]
    public void Instantiation_Throws_ArgumentException_When_UntilHash_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => new GitCommitFilter("notEmpty", null!));
        Assert.Throws<ArgumentNullException>(() => new GitCommitFilter("notEmpty", string.Empty));
    }
}