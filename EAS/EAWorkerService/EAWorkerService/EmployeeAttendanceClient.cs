using System.Net.Http.Json;
using EAWorkerService.Models;
using Microsoft.Extensions.Options;

namespace EAWorkerService;

public class EmployeeAttendanceClient
{
    private readonly HttpClient _httpClient;
    private readonly MySettings _settings;

    public EmployeeAttendanceClient(HttpClient httpClient, IOptions<MySettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    // Your attendance job logic here
    public async Task RunAttendanceJobAsync()
    {
        // Initialize the AttendanceDetails object with data
        var attendanceDetails = new AttendanceDetails
        {
            Name = _settings.Name,
            EmailId = _settings.EmailId,
            MobileNumber = _settings.MobileNumber,
            IPAddress = IPAddressClient.GetPublicIPAsync().Result,
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
