using System.ComponentModel.DataAnnotations;

namespace EAS.API.Dtos;

public record class UpdateAttendanceDto(
    int Id,
    string Name,
    string EmailId,
    string MobileNumber,
    string IPAddress,
    bool IsDSRSent,
    bool IsPresent,
    DateTime CreatedOn
);
