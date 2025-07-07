using EAS.API;
using EAS.API.Data;
using EAS.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DSRReminderDb");
builder.Services.AddSqlite<DSRReminderContext>(connectionString)
                .AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();

// Swagger configuration: register services and middleware together for clarity
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.MapAttendanceEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // Serve Swagger UI at application's root
    });
}

// Migrate the database
await app.MigrateDbAsync();

app.Run();
