using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleTaskRunner.Handlers
{
    public class PingRequest : IRequest<string>{}
    
    public class PingHandler : IRequestHandler<PingRequest, string>
    {
        public Task<string> Handle(PingRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult("Pong");
        }
    }
}