using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demo.Logging.Console;

public class MyConsoleWorker : BackgroundService
{
    private readonly ILogger<MyConsoleWorker> _logger;
    
    public MyConsoleWorker(ILogger<MyConsoleWorker> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Regarding loging-levels.. have a read here:
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-8.0#log-level
        _logger.LogTrace("Hello World!");
        _logger.LogDebug("Hello world");
        _logger.LogInformation("Hello World!");
        _logger.LogWarning("Hello World!");
        _logger.LogError("Hello World!");
        _logger.LogCritical("Hello World!");

        try
        {
            throw new NotImplementedException("Ouch! This should never happen!");
        }
        catch (Exception e)
        {
            // All logging-levels can take an exception as the first parameter..
            _logger.LogError(e, "An exception occurred");
        }


        var person = new Person
        {
            FirstName = "Arnold",
            Age = 54,
            Family =
            [
                new Person
                {
                    FirstName = "Camilla",
                    Age = 52
                },
                new Person
                {
                    FirstName = "James",
                    Age = 20
                }
            ]
        };
        // Lets log an object the "old way"...
        // We serialize it to some sort of text...
        var personJson = JsonSerializer.Serialize(person);
        // And we log it...
        _logger.LogDebug($"This is not how we log objects in a structured way.. but here we go {personJson}");
        // We could also log it like...which is more performant than the above due to allocation and should be prefered...
        _logger.LogDebug("This is not how we log objects in a structured way.. but here we go {personJson}", personJson);
        // However, in both the above cases... we log a "string"... not an actual structured object.
        // Now lets log this as a structured object...
        _logger.LogDebug("This is not how we log objects in a structured way.. but here we go {@personJson}", person);
        // Notice the @ and how we are passing the entire object. This will get "serialized" by the formatter in a way that makes sense and
        // keep the object "structured" so we can actually search by properties in any more sophisticated tools such as Kibana, Splunk, Seq etc...
        // Also pay attention to how these differ in the logged JSON-file (/logs). (Make sure to uncomment in Program.cs to enable it).
        // Doing structured logging have multiple benefits, one of the more obvious is that its easier to read.. another as mentioned is that we can
        // now query more precicely using our tools...and another would be that we could also create graphs and metrics based on more specific properties...
        // For instance...a graph showing how many times the user with a give username has signed in...or generated an error which we could also create an alert rule for
        // triggering a notification if the user has generated X ammount of errors during the past Y minutes, which might indicated that either that user struggels alot... or is trying to
        // temper with the software in ways which are not your "average user". 
        
        return Task.CompletedTask;
    }
}

public class Person
{
    public string FirstName { get; set; }

    public int Age { get; set; }

    public List<Person> Family { get; set; } = [];
}