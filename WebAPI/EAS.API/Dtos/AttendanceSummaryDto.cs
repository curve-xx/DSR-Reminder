namespace EAS.API.Dtos;

public record class AttendanceSummaryDto(
    int Id,
    string Name,
    string EmailId,
    string MobileNumber,
    DateTime CreatedOn
);
