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

        // build service provider based on service collection
        var provider = services.BuildServiceProvider();

        // get service by its interface
        var program = provider.GetRequiredService<IProgram>();

        // call method interface
        program.HelloWorld();
    }

    public Program(IConfiguration config)
    {
        // get confguration interface through dependency injection
        _config = config;
    }

    public void HelloWorld()
    {
        // get config item from configuration interface
        var text = _config.GetValue<string>("Application:Text");
        // print value
        Console.WriteLine($"Application:Text = {text}");
    }
}
