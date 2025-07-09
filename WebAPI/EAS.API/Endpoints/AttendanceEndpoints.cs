using EAS.API.Data;
using EAS.API.Dtos;
using EAS.API.Mapping;
using Microsoft.EntityFrameworkCore;

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
        group.MapPost("/", async (DSRReminderContext context, CreateAttendanceDto dto) =>
        {
            var attendance = dto.ToEntity();

            context.Attendances.Add(attendance);
            await context.SaveChangesAsync();

            return Results.Created($"/api/attendance/{attendance.Id}", attendance);
        });

        // Get attendance records by filters
        group.MapGet("/filters", async (DSRReminderContext context, [AsParameters] AttendanceFiltersDto dto) =>
        {
            var query = context.Attendances
                        .Where(a =>
                            (!dto.FromDate.HasValue || !dto.ToDate.HasValue || a.CreatedOn.Date >= dto.FromDate.Value.Date && a.CreatedOn.Date <= dto.ToDate.Value.Date) &&
                            (string.IsNullOrWhiteSpace(dto.Name) || a.Name.Contains(dto.Name))
                        );

            var results = await query.ToListAsync();
            return Results.Ok(results);
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

        return group;
    }
}
