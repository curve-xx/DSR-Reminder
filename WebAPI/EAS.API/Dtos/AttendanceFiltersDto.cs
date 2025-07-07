namespace EAS.API.Dtos;

public record class AttendanceFiltersDto(
    string? Name,
    DateTime? FromDate,
    DateTime? ToDate
);
