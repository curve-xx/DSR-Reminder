using EAS.Frontend.Models;

namespace EAS.Frontend.Clients;

public class AttendanceClient(HttpClient httpClient)
{
   // Fetch attendance summaries for today by default
   public async Task<AttendanceSummary[]> GetAttendanceSummariesAsync()
   {
      var summaries = await httpClient.GetFromJsonAsync<AttendanceSummary[]>($"attendance/search?fromdate={DateTime.Now.ToString("yyyy-MM-dd")}&todate={DateTime.Now.ToString("yyyy-MM-dd")}") ?? [];
      return summaries;
   }

   // Fetch attendance summaries for a specific date range
   public async Task<AttendanceSummary[]> GetAttendanceSearchAsync(AttendanceFilters attendanceFilters)
   {
      string parameters = string.Empty;
      if (attendanceFilters.FromDate is null || attendanceFilters.ToDate is null)
      {
         if (!string.IsNullOrWhiteSpace(attendanceFilters.SearchName))
         {
            parameters = $"name={attendanceFilters.SearchName}";
         }
         else
         {
            parameters = $"fromdate={DateTime.Now.ToString("yyyy-MM-dd")}&todate={DateTime.Now.ToString("yyyy-MM-dd")}";
         }
      }
      else
      {
         if (!string.IsNullOrWhiteSpace(attendanceFilters.SearchName))
         {
            parameters = $"name={attendanceFilters.SearchName}&fromdate={attendanceFilters.FromDate?.ToString("yyyy-MM-dd")}&todate={attendanceFilters.ToDate?.ToString("yyyy-MM-dd")}";
         }
         else
         {
            parameters = $"fromdate={attendanceFilters.FromDate?.ToString("yyyy-MM-dd")}&todate={attendanceFilters.ToDate?.ToString("yyyy-MM-dd")}";
         }
      }
      var summaries = await httpClient.GetFromJsonAsync<AttendanceSummary[]>($"attendance/search?{parameters}") ?? [];
      return summaries;
   }

   // Update attendance for a specific user
   public async Task UpdateAttendanceAsync(AttendanceSummary updateAttendance)
   {
      await httpClient.PostAsJsonAsync($"attendance/edit", updateAttendance);
   }

   // Fetch attendance summaries for yesterday
   public async Task<AttendanceSummary[]> GetYesterdayAttendanceSummariesAsync()
   {
      var summaries = await httpClient.GetFromJsonAsync<AttendanceSummary[]>($"attendance/search?fromdate={DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}&todate={DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}") ?? [];
      return summaries;
   }

   // Fetch attendance summaries for DSR reminders
   public async Task<AttendanceSummary[]> GetAttendanceSearchForDSRReminderAsync(AttendanceFilters attendanceFilters)
   {
      string parameters = string.Empty;
      if (attendanceFilters.FromDate is null || attendanceFilters.ToDate is null)
      {
         if (!string.IsNullOrWhiteSpace(attendanceFilters.SearchName))
         {
            parameters = $"name={attendanceFilters.SearchName}";
         }
         else
         {
            parameters = $"fromdate={DateTime.Now.ToString("yyyy-MM-dd")}&todate={DateTime.Now.ToString("yyyy-MM-dd")}";
         }
      }
      else
      {
         if (!string.IsNullOrWhiteSpace(attendanceFilters.SearchName))
         {
            parameters = $"name={attendanceFilters.SearchName}&fromdate={attendanceFilters.FromDate?.ToString("yyyy-MM-dd")}&todate={attendanceFilters.ToDate?.ToString("yyyy-MM-dd")}";
         }
         else
         {
            parameters = $"fromdate={attendanceFilters.FromDate?.ToString("yyyy-MM-dd")}&todate={attendanceFilters.ToDate?.ToString("yyyy-MM-dd")}";
         }
      }
      var summaries = await httpClient.GetFromJsonAsync<AttendanceSummary[]>($"attendance/dsr-reminder-search?{parameters}") ?? [];
      return summaries;
   }

   // Send DSR reminder for a specific user
   public async Task SendDSRReminderAsync(AttendanceSummary updateAttendance)
   {
      var response = await httpClient.GetAsync($"attendance/send-dsr-reminder?email={updateAttendance.EmailId}");

      var content = await response.Content.ReadAsStringAsync();
      Console.WriteLine($"[Error] Response Body: {content}");
   }
}
