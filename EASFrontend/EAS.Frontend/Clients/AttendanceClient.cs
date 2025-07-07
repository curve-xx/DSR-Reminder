using System;
using EAS.Frontend.Models;

namespace EAS.Frontend.Clients;

public class AttendanceClient(HttpClient httpClient)
{
   public async Task<AttendanceSummary[]> GetAttendanceSummariesAsync()
      => await httpClient.GetFromJsonAsync<AttendanceSummary[]>("attendance") ?? [];

   public async Task UpdateAttendanceAsync(AttendanceSummary updateAttendance)
      => await httpClient.PutAsJsonAsync($"attendance/{updateAttendance.Id}", updateAttendance);
}
