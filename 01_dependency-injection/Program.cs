using Microsoft.Extensions.DependencyInjection;

namespace EmbeddedDevices.Dotnet.Playground;

public interface IProgram
{
        void HelloWorld();
}

public class Program : IProgram
{
        static void Main(string[] args)
        {
                // create service collection
                var collection = new ServiceCollection();
                // add our Program class to service collection as Singleton
                collection.AddSingleton<IProgram, Program>();

                // build service provider based on service collection
                var provider = collection.BuildServiceProvider();

                // get service by its interface
                var program = provider.GetRequiredService<IProgram>();

                // call method interface
                program.HelloWorld();
        }

        public Program()
        {
                // no expected dependencies in constructor class
        }

        public void HelloWorld()
        {
                Console.WriteLine("Hello, World!");
        }
}
