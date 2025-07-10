using EAWorkerService;
using EAWorkerService.Models;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((context, services) =>
    {
        var easAPIUrl = "https://api.curvexx.in/api/";

        services.AddHttpClient<EmployeeAttendanceClient>(client => client.BaseAddress = new Uri(easAPIUrl));
        services.Configure<MySettings>(context.Configuration.GetSection("MySettings"));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();