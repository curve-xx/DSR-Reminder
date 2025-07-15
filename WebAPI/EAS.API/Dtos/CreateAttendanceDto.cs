using System.ComponentModel.DataAnnotations;

namespace EAS.API.Dtos;

public record class CreateAttendanceDto(
    [Required][StringLength(50)] string Name,
    [Required][EmailAddress] string EmailId,
    [Required][Phone] string MobileNumber,
    [Required] string IPAddress,
    [Required] bool IsDeleted,
    [Required] string CreatedBy,
    [Required] DateTime CreatedOn
);

