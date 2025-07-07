using System.Net.Http.Json;
using EAWorkerService.Models;

namespace EAWorkerService;

public class EmployeeAttendanceClient
{
    private readonly HttpClient _httpClient;

    public EmployeeAttendanceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Your attendance job logic here
    public async Task RunAttendanceJobAsync()
    {
        // Initialize the AttendanceDetails object with data
        var attendanceDetails = new AttendanceDetails
        {
            Name = "Moiz Ahmed",
            EmailId = "moeezinbox@gmail.com",
            MobileNumber = "9172499704",
            IsDeleted = false,
            CreatedBy = "System",
            CreatedOn = DateTime.Now
        };

        // Ensure HttpClient is initialized before making requests
        if (_httpClient == null)
        {
            throw new InvalidOperationException("HttpClient is not initialized.");
        }

        // Post attendance details to the API
        await _httpClient.PostAsJsonAsync("attendance", attendanceDetails);
    }
}
