using Demo.Logging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;

// Let´s create a Host... since the .Net default IoC/DI is very deeply integrated into
// multiple other libraries (Package: Microsoft.Extensions.Host).
//There are multiple pre-configured hosts to chose from... have a read over here:
// https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder
var host = Host.CreateApplicationBuilder(args);

host.Services.AddSerilog(o =>
{   
    // From where should we write the serilog configuration?
    // In this case we read the configurations from appsettings.json (or our other configuration providers)
    o.ReadFrom.Configuration(host.Configuration)
        // We can also set configurations by code...
        // Encrichers: lets you add additional context to your entries, keep in mind that if the formatter does not support 
        // the enricher (might be missing in its message-template... then it won´t show up, in most cases you can write your own messagetemplate for the formatters
        // if needed... see custom outputTemplate sample below for console sink)
        .Enrich.FromLogContext()
        // Let´s add the environment name using an enricher
        .Enrich.WithEnvironmentName()
        //Sinks (WriteTo):... to where should we write this log?
        .WriteTo.Console()
        // Sample of custom outputTemplate/messageTemplate
        // .WriteTo.Console(outputTemplate:"[{Timestamp:HH:mm:ss} {EnvironmentName} {Level}] {Message}{NewLine}{Exception}")
        // Uncomment the below to log to file
        .WriteTo.File(path: "logs/log.json", rollOnFileSizeLimit: true, fileSizeLimitBytes: 1000*1000, formatter: new JsonFormatter())
        // each sink might have additional options...in this case we format the message into JSON and set a max
        // byte size before it should create a new log-file, we also set a max allowed number of files to be logged.
        // we also set the log to be created in the relative path called "logs".
        // Lets for the sake of this demo set the MinimumLevel to debug using code.
        .MinimumLevel.Debug();

    //Sinks, Enrichers etc can be found as multiple packages..
    //Keep in mind that the order of how configurations are read do matter (in this case code overrides appsettings.json)
});

// We can also register Serilog like this...
// var logger = new LoggerConfiguration()
//     .ReadFrom.Configuration(host.Configuration)
//     // We can also set configurations by code...
//     //Encrichers: lets you add additional context to your entries
//     .Enrich.FromLogContext()
//     // Lets add the environment name using an encricher
//     .Enrich.WithEnvironmentName()
//     //Sinks (WriteTo):... to where should we write this log?
//     .WriteTo.Console()
//     // Lets for the sake of this demo set the MinimumLevel to debug using code.
//     .MinimumLevel.Debug()
//     .CreateLogger();
//
// host.Services.AddSerilog(logger);
//And there are multiple other ways of doing this...

// The actual "code" needs to run as a hosted service since the execution of code "stops" after we run the app.
// More about hosted services here: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio
host.Services.AddHostedService<MyConsoleWorker>();

var app = host.Build();

//As mentioned... all code after the below line will be "blocked" (or not executed until we close the applicaiton releasing the "main-thread")
app.Run();

// Now try to comment out the AddSerilog... this will remove serilog and add the default logging provider, which is way more limited
// but yet good to be aware of.
// Also have a look at the appsettings.json...