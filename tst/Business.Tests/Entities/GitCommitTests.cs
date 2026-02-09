using Domain.Entities;
using System;
using System.Linq;
using Xunit;

namespace Business.Tests.Entities;

public class GitCommitTests
{
    [Fact]
    public void Instantiation_Throws_ArgumentException_When_Id_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => new GitCommit(null!, "notEmpty", []));
        Assert.Throws<ArgumentException>(() => new GitCommit(string.Empty, "notEmpty", []));
    }

    [Fact]
    public void Instantiation_Throws_ArgumentException_When_Subject_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => new GitCommit("notEmpty", null!, []));
        Assert.Throws<ArgumentException>(() => new GitCommit("notEmpty", string.Empty, []));
    }
}