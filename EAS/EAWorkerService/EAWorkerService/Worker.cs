namespace EAWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly EmployeeAttendanceClient _client;

    public Worker(ILogger<Worker> logger, EmployeeAttendanceClient client)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var nextMidnight = now.AddMinutes(60);
        var delay = nextMidnight - now;

        _logger.LogInformation("Service started. Waiting until ({targetTime}) to execute task.",
            nextMidnight);

        try
        {
            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Executing task at {time}", DateTime.Now);
                await _client.RunAttendanceJobAsync();
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Execution cancelled before midnight.");
        }

        _logger.LogInformation("Task completed. Exiting service.");
    }
}
