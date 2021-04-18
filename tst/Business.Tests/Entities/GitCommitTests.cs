using Domain.Entities;
using System;
using System.Linq;
using Xunit;

namespace Business.Tests.Entities
{
    public class GitCommitTests
    {
        [Fact]
        public void Instantiation_Throws_ArgumentException_When_Id_IsNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() => new GitCommit(null!, "notEmpty", Enumerable.Empty<GitCommitFileInfo>()));
            Assert.Throws<ArgumentException>(() => new GitCommit(string.Empty, "notEmpty", Enumerable.Empty<GitCommitFileInfo>()));
        }

        [Fact]
        public void Instantiation_Throws_ArgumentException_When_Subject_IsNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() => new GitCommit("notEmpty", null!, Enumerable.Empty<GitCommitFileInfo>()));
            Assert.Throws<ArgumentException>(() => new GitCommit("notEmpty", string.Empty, Enumerable.Empty<GitCommitFileInfo>()));
        }
    }
}
