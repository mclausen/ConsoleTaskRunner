
# Console Task Runner Sample  
  
This repository is a sample of how a console task runner can be made using [MediatR](https://github.com/jbogard/MediatR/wiki)

Some typical usecases.
 - Event Emitters
 - Migration Tools

## How to?
First thing is that you need to duplicate this repository, so you can adjust the console runner to your project.

Secondly you can create the task handlers to your liking by (default MediatR usage)
1.Create a new class deriving from `IRequest<TReturnType>`
2 Create a new class deriving from `IRequestHandler<TRequestType, TReturnType>`

A sample handler from the the repository inside `Handlers` folder

       public class PingRequest : IRequest<string>{}  
         
       public class PingHandler : IRequestHandler<PingRequest, string>  
       {  
           public Task<string> Handle(PingRequest request, CancellationToken cancellationToken)  
           {  
               return Task.FromResult("Pong");  
           }  
       }

When done Hit F5 and Run the console task runner