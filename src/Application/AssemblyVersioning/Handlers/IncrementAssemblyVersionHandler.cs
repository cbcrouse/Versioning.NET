using Application.AssemblyVersioning.Commands;
using Application.Interfaces;
using MediatR;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Application.AssemblyVersioning.Handlers
{
    /// <summary>
    /// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing assembly versions.
    /// </summary>
    public class IncrementAssemblyVersionHandler : IRequestHandler<IncrementAssemblyVersionCommand, Unit>
    {
        private readonly IAssemblyVersioningService _assemblyVersioningService;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="assemblyVersioningService">An abstraction for working with assembly versions.</param>
        public IncrementAssemblyVersionHandler(IAssemblyVersioningService assemblyVersioningService)
        {
            _assemblyVersioningService = assemblyVersioningService;
        }

        /// <summary>
        /// Handles the request to version assemblies.
        /// </summary>
        /// <param name="request">The <see cref="IMediator"/> request object responsible for incrementing assembly versions.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        public Task<Unit> Handle(IncrementAssemblyVersionCommand request, CancellationToken cancellationToken)
        {
            request.Directory ??= Assembly.GetExecutingAssembly().Location;
            _assemblyVersioningService.IncrementVersion(request.VersionIncrement, request.Directory);
            return Task.FromResult(Unit.Value);
        }
    }
}
