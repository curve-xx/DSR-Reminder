using EAWorkerService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((context, services) =>
    {
        var easAPIUrl = context.Configuration["EASApiUrl"] ?? throw new Exception("EAS API URL is missing.");

        services.AddHttpClient<EmployeeAttendanceClient>(client =>
            client.BaseAddress = new Uri(easAPIUrl));

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();