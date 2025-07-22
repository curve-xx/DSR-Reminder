using EAS.API;
using EAS.API.Data;
using EAS.API.Endpoints;
using EAS.API.Entities;
using EAS.API.Services;
using EAS.API.TaskServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DSRReminderDb");
builder.Services.AddSqlite<DSRReminderContext>(connectionString)
                .AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.Configure<SlackBotSettings>(builder.Configuration.GetSection("SlackBotSettings"));

// Bind Google settings from config
builder.Services.Configure<GMailSettings>(builder.Configuration.GetSection("GMailSettings"));
var googleAuth = builder.Configuration.GetSection("GMailSettings").Get<GMailSettings>();

if (googleAuth == null || string.IsNullOrEmpty(googleAuth.ClientId) || string.IsNullOrEmpty(googleAuth.ClientSecret))
{
    throw new InvalidOperationException("GMailSettings section is missing or invalid.");
}

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = googleAuth.ClientId;
    options.ClientSecret = googleAuth.ClientSecret;
    options.Scope.Add(googleAuth.Scopes);
    options.SaveTokens = true;
    options.CallbackPath = "/signin-google"; // <--- Important
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<HolidayConfig>(builder.Configuration.GetSection("HolidayConfig"));

// Register services
builder.Services.AddHostedService<TaskUpdateTaskService>();
builder.Services.AddSingleton<GmailOAuthService>();

// Swagger configuration: register services and middleware together for clarity
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.MapAttendanceEndpoints();
app.MapSlackServiceEndpoints();
app.MapGmailServiceEndpoints();

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
