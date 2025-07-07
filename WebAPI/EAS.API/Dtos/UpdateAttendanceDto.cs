using System.ComponentModel.DataAnnotations;

namespace EAS.API.Dtos;

public record class UpdateAttendanceDto(
    int Id,
    [Required][StringLength(50)] string Name,
    [Required][EmailAddress] string EmailId,
    [Required][Phone] string MobileNumber,
    [Required] bool IsPresent,
    [Required] DateTime CreatedOn
);
