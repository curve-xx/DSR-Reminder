using System;

namespace EAS.API.Services;

public class WeekendService
{
    // Helper methods
    public bool IsSunday(DateTime date) => date.DayOfWeek == DayOfWeek.Sunday;

    public bool IsOddSaturday(DateTime date) =>
        date.DayOfWeek == DayOfWeek.Saturday && (GetWeekOfMonth(date) % 2 == 1);

    // Week of month logic
    private int GetWeekOfMonth(DateTime date)
    {
        var firstDay = new DateTime(date.Year, date.Month, 1);
        int offset = (int)firstDay.DayOfWeek;
        return ((date.Day + offset - 1) / 7) + 1;
    }
}
