using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EAS.API.Entities;
using Microsoft.Extensions.Options;

namespace EAS.API.Services;

public class SlackService
{
    private readonly HttpClient _httpClient;
    private readonly SlackBotSettings _settings;

    public SlackService(IOptions<SlackBotSettings> options)
    {
        _settings = options.Value;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.BotToken);
    }

    public async Task SendMessageToChannelAsync(string message)
    {
        var payload = new
        {
            channel = _settings.ChannelId,
            text = message
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_settings.SlackAPIUrl, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode || !responseBody.Contains("\"ok\":true"))
        {
            Console.Write($"Slack API error: {responseBody}");
        }
    }
}
