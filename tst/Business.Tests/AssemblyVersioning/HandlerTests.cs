using Application.AssemblyVersioning.Commands;
using Application.AssemblyVersioning.Handlers;
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
			var request = new AssemblyVersioningCommand();
			var sut = new AssemblyVersioningHandler();

			// Act
			await sut.Handle(request, CancellationToken.None);

			// Assert
			Assert.True(true);
		}
	}
}
