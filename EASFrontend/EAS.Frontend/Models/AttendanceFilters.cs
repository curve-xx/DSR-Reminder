using System;

namespace EAS.Frontend.Models;

public class AttendanceFilters
{
    public string? SearchName { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
