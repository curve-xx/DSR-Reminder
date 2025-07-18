using System;
using EAS.API.Entities;
using Microsoft.Extensions.Options;

namespace EAS.API.Services;

public class HolidayService
{
    private readonly List<DateTime> _holidays;

    public HolidayService(IOptions<HolidayConfig> config)
    {
        _holidays = config.Value.Holidays
                    .Select(d => DateTime.Parse(d))
                    .ToList();
    }

    public bool IsHoliday(DateTime date)
    {
        return _holidays.Contains(date);
    }
}
