using EAS.API.Data;
using EAS.API.Dtos;
using EAS.API.Entities;
using EAS.API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace EAS.API.Endpoints;

public static class AttendanceEndpoints
{
    public static RouteGroupBuilder MapAttendanceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/attendance").WithParameterValidation();

        // Get all attendance records
        group.MapGet("/", async (Data.DSRReminderContext context) =>
        {
            return await context.Attendances.ToListAsync();
        });

        // Get attendance by ID
        group.MapGet("/{id:int}", async (Data.DSRReminderContext context, int id) =>
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

        return group;
    }
}
