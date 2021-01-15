using Application.AssemblyVersioning.Commands;
using Application.AssemblyVersioning.Handlers;
using Application.Interfaces;
using Domain.Enumerations;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Business.Tests.AssemblyVersioning
{
    public class HandlerTests
    {
        [Fact]
        public async Task CreateTodoItemHandler_CallsDependencies()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Minor
            };
            var service = new Mock<IAssemblyVersioningService>();
            var sut = new IncrementAssemblyVersionHandler(service.Object);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(VersionIncrement.Minor, "C:\\Temp"), Times.Once);
        }
    }
}
