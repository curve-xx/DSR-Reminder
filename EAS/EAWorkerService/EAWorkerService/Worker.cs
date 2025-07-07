namespace EAWorkerService;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var nextMidnight = now.AddMinutes(30);
        var delay = nextMidnight - now;

        logger.LogInformation("Service started. Waiting until ({targetTime}) to execute task.",
            nextMidnight);

        try
        {
            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Executing task at {time}", DateTime.Now);
                EmployeeAttendance.RunAttendanceJob();
            }
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Execution cancelled before midnight.");
        }

        logger.LogInformation("Task completed. Exiting service.");
    }    
}
