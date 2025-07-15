using System;
using EAS.API.Dtos;
using EAS.API.Entities;

namespace EAS.API.Mapping;

public static class AttendanceMapping
{
    public static AttendanceSummaryDto ToSummaryDto(this Attendance attendance)
    {
        return new AttendanceSummaryDto(
            attendance.Id,
            attendance.Name,
            attendance.EmailId,
            attendance.MobileNumber,
            attendance.IPAddress,
            attendance.CreatedOn
        );
    }

    public static Attendance ToEntity(this CreateAttendanceDto dto)
    {
        return new Attendance
        {
            Name = dto.Name,
            EmailId = dto.EmailId,
            MobileNumber = dto.MobileNumber,
            IPAddress = dto.IPAddress,
            IsDeleted = dto.IsDeleted,
            CreatedBy = dto.CreatedBy,
            CreatedOn = dto.CreatedOn
        };
    }
}
