using System;
using EAS.Frontend.Models;

namespace EAS.Frontend.Clients;

public class AttendanceClient(HttpClient httpClient)
{
   public async Task<AttendanceSummary[]> GetAttendanceSummariesAsync()
   {
      var summaries = await httpClient.GetFromJsonAsync<AttendanceSummary[]>($"attendance/search?fromdate={DateTime.Now.ToString("yyyy-MM-dd")}&todate={DateTime.Now.ToString("yyyy-MM-dd")}") ?? [];
      return summaries;
   }

   public async Task<AttendanceSummary[]> GetAttendanceSearchAsync()
   {
      var summaries = await httpClient.GetFromJsonAsync<AttendanceSummary[]>($"attendance/search?fromdate={DateTime.Now.ToString("yyyy-MM-dd")}&todate={DateTime.Now.ToString("yyyy-MM-dd")}") ?? [];
      return summaries;
   }

   public async Task UpdateAttendanceAsync(AttendanceSummary updateAttendance)
   {
      await httpClient.PostAsJsonAsync($"attendance/edit", updateAttendance);
   }
}
