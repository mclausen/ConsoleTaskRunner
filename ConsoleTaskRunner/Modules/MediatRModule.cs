using Autofac;
using ConsoleTaskRunner.Handlers;
using MediatR;

namespace ConsoleTaskRunner.Modules
{
    public class MediatRModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();
            
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            
            builder.RegisterAssemblyTypes(typeof(PingHandler).Assembly).AsImplementedInterfaces();
        }
    }
}