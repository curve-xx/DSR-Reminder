using EAWorkerService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Manually add WindowsServiceLifetime
builder.Services.AddWindowsService();

// Register your background worker service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Run the application
host.Run();