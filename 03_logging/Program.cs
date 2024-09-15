using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmbeddedDevices.Dotnet.Playground;

public interface IProgram
{
    void HelloWorld();
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
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(config.GetSection("Logging"));
            builder.AddConsole();
        });

        // build service provider based on service collection
        var provider = services.BuildServiceProvider();

        // get service by its interface
        var program = provider.GetRequiredService<IProgram>();

        // call method interface
        program.HelloWorld();
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
}
