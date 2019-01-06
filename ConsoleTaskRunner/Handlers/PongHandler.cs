using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleTaskRunner.Handlers
{
    public class PongRequest : IRequest<string>
    {
        public string Description { get; }
        public string From { get; }
        public int Number { get; }

        public PongRequest(string description, string from, int number)
        {
            Description = description;
            From = @from;
            Number = number;
        }
    }
    
    public class PongHandler : IRequestHandler<PongRequest,string>
    {
        public Task<string> Handle(PongRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Retrieve pong from: {request.From}. It says {request.Description}");
            return Task.FromResult(string.Empty);
        }
    }
}