using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmbeddedDevices.Dotnet.Playground;

public interface IProgram
{
    void HelloWorld();
    Task<int> HelloWorldAsync(int delay, CancellationToken cancellationToken = default);
}

public class Program : IProgram
{
    private readonly IConfiguration _config;
    private readonly ILogger<Program> _logger;

    static async Task Main(string[] args)
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

        // create array of tasks
        var tasks = new Task<int>[3];

        // start first async call without cancellation token
        tasks[0] = Task.Run(async () => await program.HelloWorldAsync(1000));
        // start second async call without cancellation token
        tasks[1] = Task.Run(async () => await program.HelloWorldAsync(3000));

        // create cancellation token source
        var src = new CancellationTokenSource();
        // get token
        var token = src.Token;
        // start third async call
        tasks[2] = Task.Run(async () => await program.HelloWorldAsync(2000, token), token);

        // trigger cancel after 100ms
        src.CancelAfter(100);

        // wait until all tasks will finish
        try
        {
            // even task will be cancelled during waiting, it will wait for all, and throw exception at the end
            await Task.WhenAll(tasks);
            // results could be get directly from await Task.WhenAll result (as array),
            // but the result is not available in case of TaskCancelledException
        }
        catch (TaskCanceledException ex)
        {
            // print expected exception
            Console.WriteLine(ex.Message);
        }

        // print results
        foreach (var task in tasks)
        {
            int? res = task.IsCanceled ? null : task.Result;
            Console.WriteLine($"result = <{res}>, cancelled = {task.IsCanceled}");
        }

        // dispose cancellation token source
        src.Dispose();
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

    public async Task<int> HelloWorldAsync(int delay, CancellationToken cancellationToken = default)
    {
        // print log
        _logger.LogInformation("{HelloWorldAsync({Delay}) started", delay);
        // async delay
        await Task.Delay(delay, cancellationToken);
        // print message to console
        Console.WriteLine($"{delay} Hello, World!");
        // print log
        _logger.LogInformation("HelloWorldAsync({Delay}) finished", delay);
        // return result asynchronously
        return delay;
    }
}
