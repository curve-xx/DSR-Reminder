using Attendance.Frontend.Clients;
using Attendance.Frontend.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var easAPIUrl = builder.Configuration["EASApiUrl"] ?? throw new Exception("EAS API URL is missing.");

builder.Services.AddHttpClient<AttendanceClient>(client =>
    client.BaseAddress = new Uri(easAPIUrl));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForErrors: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
