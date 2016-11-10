using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using SimpleInjector.Extensions.ExecutionContextScoping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var busConfiguration = new EndpointConfiguration("test");

            var container = new SimpleInjector.Container();
                container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            container.Register(() => new MyService { Id = "Created outside" }, SimpleInjector.Lifestyle.Scoped);

            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.EnableInstallers();
            busConfiguration.UseContainer<SimpleInjectorBuilder>(customizations: cust =>
            {
                // which has a higher version than referenced assembly
                // Assembly 'NServiceBus.SimpleInjector' uses 'SimpleInjector, Version=3.2.7.0, Culture=neutral, PublicKeyToken=984cb50dea722e99' which has a higher version than referenced assembly 'SimpleInjector' with identity 'SimpleInjector, Version=3.2.3.0, Culture=neutral, PublicKeyToken=984cb50dea722e99'

                cust.UseExistingContainer(container);
            });

            var bus = Endpoint.Create(busConfiguration).Result.Start().Result;
            {
                bus.SendLocal(new CreateOrder { });
            }

            Console.ReadLine();
        }
    }

    public class MyService
    {
        public string Id { get; set; }
    }

    class ProvideConfiguration : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
    {
        public MessageForwardingInCaseOfFaultConfig GetConfiguration()
        {
            return new MessageForwardingInCaseOfFaultConfig
            {
                ErrorQueue = "error"
            };
        }
    }

    public class Handler : IHandleMessages<CreateOrder>
    {
        public MyService MyService { get; set; }

        public Task Handle(CreateOrder message, IMessageHandlerContext context)
        {
            Console.WriteLine("OMFG WTF BBQ!!!");
            return Task.FromResult
                (0);
        }
    }


    public class CreateOrder :ICommand
    { }


}
