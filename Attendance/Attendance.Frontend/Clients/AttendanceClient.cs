using System;

namespace Attendance.Frontend.Clients;

public class AttendanceClient(HttpClient httpClient)
{
    // Fetch attendance summaries for today by default
   public async Task<AttendanceSummary[]> GetAttendanceSummariesAsync()
   {
      var summaries = await httpClient.GetFromJsonAsync<AttendanceSummary[]>($"attendance/search?fromdate={DateTime.Now.ToString("yyyy-MM-dd")}&todate={DateTime.Now.ToString("yyyy-MM-dd")}") ?? [];
      return summaries;
   }
}
