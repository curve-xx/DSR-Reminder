using System;

namespace EAS.API.Entities;

public class Attendance
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public required string EmailId { get; set; }
    public required string MobileNumber { get; set; }
    public bool IsPresent { get; set; } = true;

    public bool IsDeleted { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
