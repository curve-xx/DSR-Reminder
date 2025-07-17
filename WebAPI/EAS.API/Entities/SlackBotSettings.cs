using System;

namespace EAS.API.Entities;

public class SlackBotSettings
{
    public required string SlackAPIUrl { get; set; }
    public required string BotToken { get; set; }
    public required string ChannelId { get; set; }
}
