using System;
using Microsoft.EntityFrameworkCore;

namespace EAS.API.Data;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<DSRReminderContext>();
            await context.Database.MigrateAsync();
        }
        catch (Exception)
        {
            // var logger = services.GetRequiredService<ILogger<DataExtensions>>();
            // logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
