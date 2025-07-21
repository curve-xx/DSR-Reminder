using EAS.API.Entities;
using EAS.API.Services;
using Microsoft.Extensions.Options;

namespace EAS.API.Endpoints;

public static class SlackEndpoints
{
    public static RouteGroupBuilder MapSlackServiceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/slack").WithParameterValidation();

        // Add more service endpoints here as needed
        group.MapPost("/task-update", async (IOptions<SlackBotSettings> options) =>
        {
            var service = new SlackService(options);
            await service.SendMessageToChannelAsync("*As-salamu alaykum,*\nHi, Good Morning!\nPlease update?");
            return Results.Ok("Message sent to Channel.");
        });

        return group;
    }
}
