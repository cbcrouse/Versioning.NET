using Domain.Entities;
using System;
using Xunit;

namespace Business.Tests.Entities
{
    public class GitCommitFilterTests
    {
        [Fact]
        public void Instantiation_Throws_ArgumentNullException_When_FromHash_IsNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() => new GitCommitFilter(null!, "notEmpty"));
            Assert.Throws<ArgumentException>(() => new GitCommitFilter(string.Empty, "notEmpty"));
        }

        [Fact]
        public void Instantiation_Throws_ArgumentNullException_When_UntilHash_IsNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() => new GitCommitFilter("notEmpty", null!));
            Assert.Throws<ArgumentException>(() => new GitCommitFilter("notEmpty", string.Empty));
        }
    }
}
