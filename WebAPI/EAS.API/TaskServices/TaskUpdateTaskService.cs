using System;
using EAS.API.Entities;
using EAS.API.Services;
using Microsoft.Extensions.Options;

namespace EAS.API.TaskServices;

public class TaskUpdateTaskService : BackgroundService
{
    private readonly ILogger<TaskUpdateTaskService> _logger;
    IOptions<SlackBotSettings> _options;
    IOptions<HolidayConfig> _holidayOptions;

    public TaskUpdateTaskService(ILogger<TaskUpdateTaskService> logger, IOptions<SlackBotSettings> options, IOptions<HolidayConfig> holidayOptions)
    {
        _logger = logger;
        _options = options;
        _holidayOptions = holidayOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("09:30 AM Task Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;

            var nextRunTime = DateTime.Today.AddHours(9).AddMinutes(30); // 9:30 AM today
            if (now >= nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1); // Schedule for next day if past 9:30 AM
            }

            var delay = nextRunTime - now;

            _logger.LogInformation($"Waiting until: {nextRunTime}");

            await Task.Delay(delay, stoppingToken);

            await RunTaskAsync(stoppingToken);
        }
    }

    private async Task RunTaskAsync(CancellationToken token)
    {
        try
        {
            var taskTime = DateTime.Now;
            
            var isSunday = new WeekendService().IsSunday(taskTime);
            var isOddSaturday = new WeekendService().IsOddSaturday(taskTime);
            var isHoliday = new HolidayService(_holidayOptions).IsHoliday(taskTime);
            
            if (!(isSunday || isOddSaturday || isHoliday))
            {
                _logger.LogInformation("Running 9:30 AM task at: {Time}", taskTime);

                var service = new SlackService(_options);
                await service.SendMessageToChannelAsync("*As-salamu alaykum,*\nHi, Good Morning!\nPlease update?");

                await Task.Delay(1000, token); // Simulated work

                _logger.LogInformation($"{taskTime} message sent to Channel.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during 9:30 AM task.");
        }
    }    
}
