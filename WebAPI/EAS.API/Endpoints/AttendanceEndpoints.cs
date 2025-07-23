using EAS.API.Data;
using EAS.API.Dtos;
using EAS.API.Entities;
using EAS.API.Mapping;
using EAS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EAS.API.Endpoints;

public static class AttendanceEndpoints
{
    public static RouteGroupBuilder MapAttendanceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/attendance").WithParameterValidation();

        // Get all attendance records
        group.MapGet("/", async (DSRReminderContext context) =>
        {
            return await context.Attendances.ToListAsync();
        });

        // Get attendance by ID
        group.MapGet("/{id:int}", async (DSRReminderContext context, int id) =>
        {
            var attendance = await context.Attendances.FindAsync(id);
            return attendance is not null ? Results.Ok(attendance) : Results.NotFound();
        });

        // Post a new attendance record
        group.MapPost("/", async (DSRReminderContext context, IConfiguration config, CreateAttendanceDto dto) =>
        {
            string publicIP = config["PublicIP"]?.ToString() ?? string.Empty;

            var attendance = dto.ToEntity();

            attendance.IsPresent = (attendance.IPAddress == publicIP);

            context.Attendances.Add(attendance);
            await context.SaveChangesAsync();

            return Results.Created($"/api/attendance/{attendance.Id}", attendance);
        });

        // Update attendance by ID
        group.MapPut("/{id:int}", async (DSRReminderContext context, int id, UpdateAttendanceDto dto) =>
        {
            var attendance = await context.Attendances.FindAsync(id);
            if (attendance is null) return Results.NotFound();

            attendance.IsPresent = attendance.IsPresent ? false : true; // Toggle IsPresent
            attendance.UpdatedBy = "Administrator";
            attendance.UpdatedOn = DateTime.Now;

            await context.SaveChangesAsync();
            return Results.Ok(attendance);
        });

        // Get attendance records by filters
        group.MapGet("/filters", async (DSRReminderContext context, [AsParameters] AttendanceFiltersDto dto) =>
        {
            var query = context.Attendances
                        .Where(a =>
                            (!dto.FromDate.HasValue || !dto.ToDate.HasValue || a.CreatedOn.Date >= dto.FromDate.Value.Date && a.CreatedOn.Date <= dto.ToDate.Value.Date) &&
                            (string.IsNullOrWhiteSpace(dto.Name) || a.Name.ToLower().Contains(dto.Name.ToLower()))
                        );

            var results = await query.ToListAsync();
            return Results.Ok(results);
        });

        // Search attendance by name or date
        group.MapGet("/search", async (DSRReminderContext context, string? name = null, DateTime? fromdate = null, DateTime? todate = null) =>
        {
            var query = context.Attendances
                       .Where(a =>
                           (!fromdate.HasValue || !todate.HasValue || a.CreatedOn.Date >= fromdate.Value.Date && a.CreatedOn.Date <= todate.Value.Date) &&
                           (string.IsNullOrWhiteSpace(name) || a.Name.ToLower().Contains(name.ToLower()))
                       );

            var results = await query.ToListAsync();
            return Results.Ok(results);
        });

        // Edit attendance by ID
        group.MapPost("/edit", async (DSRReminderContext context, UpdateAttendanceDto dto) =>
        {
            var attendance = await context.Attendances.FindAsync(dto.Id);
            if (attendance is null) return Results.NotFound();

            attendance.IsPresent = attendance.IsPresent ? false : true; // Toggle IsPresent
            attendance.UpdatedBy = "Administrator";
            attendance.UpdatedOn = DateTime.Now;

            await context.SaveChangesAsync();
            return Results.Ok(attendance);
        });

        // Search DSR Reminder by name or date
        group.MapGet("/dsr-reminder-search", async (DSRReminderContext context, string? name = null, DateTime? fromdate = null, DateTime? todate = null) =>
        {
            var query = context.Attendances
                       .Where(a =>
                           (!fromdate.HasValue || !todate.HasValue || a.CreatedOn.Date >= fromdate.Value.Date &&
                           ((todate.Value.Date < DateTime.Now.Date && a.CreatedOn.Date <= todate.Value.Date) ||
                           (todate.Value.Date >= DateTime.Now.Date && a.CreatedOn.Date < DateTime.Now.Date))) &&
                           (string.IsNullOrWhiteSpace(name) || a.Name.ToLower().Contains(name.ToLower()))
                       );

            var results = await query.ToListAsync();
            return Results.Ok(results);
        });

        // Send DSR reminder
        group.MapGet("/send-dsr-reminder", (int id, IOptions<GMailSettings> options, GmailOAuthService gmailService) =>
        {
            if (id <= 0)
                return Results.BadRequest("Invalid attendance id.");
            DSRReminder.id = id;

            var userId = options.Value.UserEmail; // or derive from session
            var authUrl = gmailService.GetAuthorizationUrl(userId, options.Value.DSRReminderRedirectUri);
            return Results.Redirect(authUrl);
        });

        return group;
    }
}
