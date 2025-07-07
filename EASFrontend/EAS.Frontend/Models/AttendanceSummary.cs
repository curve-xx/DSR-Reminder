using System;

namespace EAS.Frontend.Models;

public class AttendanceSummary
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string EmailId { get; set; }
    public required string MobileNumber { get; set; }
    public bool IsPresent { get; set; }
    public DateTime CreatedOn { get; set; }
}
