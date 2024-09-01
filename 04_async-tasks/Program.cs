using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmbeddedDevices.Dotnet.Playground;

public interface IProgram
{
        void HelloWorld();
        Task<int> HelloWorldAsync(int delay);
}

public class Program : IProgram
{
        private readonly IConfiguration _config;
        private readonly ILogger<Program> _logger;

        static void Main(string[] args)
        {
                // create configuration builder
                var builder = new ConfigurationBuilder();
                // load add configuration settings file to builder
                builder.AddJsonFile("settings.json");
                // build configuration
                var config = builder.Build();

                // create service collection
                var services = new ServiceCollection();
                // add configuration interface to service collection
                services.AddSingleton<IConfiguration>(config);
                // add our Program class to service collection as Singleton
                services.AddSingleton<IProgram, Program>();
                // add LoggerFactory to service collection
                services.AddLogging(builder => builder.AddConsole());

                // build service provider based on service collection
                var provider = services.BuildServiceProvider();
                
                // get service by its interface
                var program = provider.GetRequiredService<IProgram>();

                // call method interface
                program.HelloWorld();

                // start first async call
                var t1 = Task.Run(async () => await program.HelloWorldAsync(1000));
                // start second async call
                var t2 = Task.Run(async () => await program.HelloWorldAsync(2000));

                // wait until all tasks will finish
                Task.WhenAll(t1, t2).Wait();

                // print results
                Console.WriteLine(t1.Result);
                Console.WriteLine(t2.Result);
        }

        public Program(IConfiguration config, ILogger<Program> logger)
        {
                // get confguration interface through dependency injection
                _config = config;
                // get constructed logger through dependency injection
                _logger = logger;
        }

        public void HelloWorld()
        {
                // print log
                _logger.LogInformation("Hello world");
                // get config item from configuration interface
                var text = _config.GetValue<string>("Application:Text");
                // print value
                Console.WriteLine($"Application:Text = {text}");
        }

        public async Task<int> HelloWorldAsync(int delay)
        {
                // async delay
                await Task.Delay(delay);
                // print message to console
                Console.WriteLine($"{delay} Hello, World!");
                // return result asynchronously
                return delay;
        }
}
