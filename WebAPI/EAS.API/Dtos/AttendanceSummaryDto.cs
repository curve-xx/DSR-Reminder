namespace EAS.API.Dtos;

public record class AttendanceSummaryDto(
    int Id,
    string Name,
    string EmailId,
    string MobileNumber,
    string IPAddress,
    DateTime CreatedOn
);
