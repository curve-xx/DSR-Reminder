using System;

namespace EAS.Frontend.Models;

public class AttendanceFilters
{
    public string? Name { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
