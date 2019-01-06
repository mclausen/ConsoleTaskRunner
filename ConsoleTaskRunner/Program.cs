using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ConsoleTaskRunner.Extensions;
using ConsoleTaskRunner.Modules;
using MediatR;

namespace ConsoleTaskRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Constants.Title);
            
            var program = new Program();

            Task.Run(async () => await program.Run()).Wait();
        }

        private IContainer container;

        public Program()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<MediatRModule>();
            container = containerBuilder.Build();
        }

        public async Task Run()
        {
            var availableCommands = GetAvailableCommands();
            var commandType = GetCommandType(availableCommands);            
            var command = GetCommand(commandType);
            
            await ExecuteCommand(command);
        }

        private static List<Type> GetAvailableCommands()
        {
            var availableCommands = typeof(Program)
                .Assembly
                .GetTypes()
                .Where(x => x.IsAssignableToGenericType(typeof(IRequest<>)))
                .ToList();
            return availableCommands;
        }

        private static Type GetCommandType(List<Type> availableCommands)
        {
            PrintAvailableCommands(availableCommands);
            
            Console.WriteLine("Command: > ");
            var value = Console.ReadLine();

            if (int.TryParse(value, out var key) == false)
            {
                Console.WriteLine($"{value} is not a valid input");
                throw new InvalidOperationException();
            }

            if (key > availableCommands.Count - 1 || key < 0)
            {
                Console.WriteLine(
                    $"Could not determine command from input {key}. Options are from 0 to {availableCommands.Count - 1}");
                throw new InvalidOperationException();
            }

            var commandType = availableCommands[key];
            return commandType;
        }

        private static void PrintAvailableCommands(List<Type> availableCommands)
        {
            Console.WriteLine("Available Commands");
            foreach (var availableCommandType in availableCommands)
            {
                Console.WriteLine($"{availableCommands.IndexOf(availableCommandType)}) {availableCommandType.Name}");
            }
        }

        private static object GetCommand(Type commandType)
        {
            var constructorInfo = commandType.GetConstructors().SingleOrDefault();
            object request = null;
            var parameterInfos = constructorInfo.GetParameters();
            if (parameterInfos.Length > 0)
            {
                var parameters = CollectConstructorParameters(parameterInfos);
                request = constructorInfo.Invoke(parameters);
            }
            else
            {
                request = Activator.CreateInstance(commandType);
            }

            return request;
        }

        private static object[] CollectConstructorParameters(ParameterInfo[] parameterInfos)
        {
            var parameters = new object[parameterInfos.Length];
            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var parameter = parameterInfos[i];
                Console.WriteLine($"{parameter.Name}: ");

                var parameterInput = Console.ReadLine();
                var actualInput = Convert.ChangeType(parameterInput, parameter.ParameterType);
                parameters[i] = actualInput;
            }

            return parameters;
        }

        private async Task ExecuteCommand(object command)
        {
            var requestInterface = command.GetType().GetInterfaces().First();
            var responseParameter = requestInterface.GenericTypeArguments.Last();
            
            var mediatR = container.Resolve<IMediator>();
            var method = typeof(IMediator).GetMethod("Send")
                .MakeGenericMethod(responseParameter);
            
            var action = (Task) method.Invoke(mediatR, new[] { command, CancellationToken.None });

            action.Wait();
        }
    }
}