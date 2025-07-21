using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;

namespace EAS.API.Services;

public class GMailService
{
    private readonly IConfiguration _config;
    private readonly GmailService _gmailService;
    private readonly string? _userEmail;

    private readonly string? _redirectUri;

    public GMailService(IConfiguration config)
    {
        _config = config;
        _userEmail = _config["GMailSettings:UserEmail"];
        _redirectUri= _config["GMailSettings:RedirectUri"];

        var clientSecrets = new ClientSecrets
        {
            ClientId = _config["GMailSettings:ClientId"],
            ClientSecret = _config["GMailSettings:ClientSecret"]
        };

        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            clientSecrets,
            new[] { GmailService.Scope.GmailReadonly },
            _userEmail,
            CancellationToken.None,
            new FileDataStore("GmailTokenStore", true)
        ).Result;

        _gmailService = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _config["GMailSettings:ApplicationName"]
        });
    }

    // Service to interact with Gmail API for retrieving emails.
    public async Task<List<object>> GetEmailsFrom(string from, DateTime fromDate, DateTime toDate)
    {
        //var service = CreateGmailService(context);

        // var today = DateTime.UtcNow.Date;
        // var tomorrow = today.AddDays(1);

        var query = $"after:{fromDate:yyyy/MM/dd} before:{toDate:yyyy/MM/dd} from:{from}";

        var request = _gmailService.Users.Messages.List("me");
        request.Q = query;

        var response = await request.ExecuteAsync();
        var messages = new List<object>();

        if (response.Messages == null || !response.Messages.Any())
            return [];

        foreach (var msg in response.Messages)
        {
            var emailInfo = await _gmailService.Users.Messages.Get("me", msg.Id).ExecuteAsync();
            var subject = emailInfo.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")?.Value;
            var date = emailInfo.Payload.Headers.FirstOrDefault(h => h.Name == "Date")?.Value;

            messages.Add(new
            {
                Subject = subject,
                Date = date,
                Snippet = emailInfo.Snippet
            });
        }

        return messages;
    }

    // Methods to interact with the Gmail API using OAuth2 authentication.
    private GmailService CreateGmailService(HttpContext context)
    {
        var token = context.GetTokenAsync("access_token").Result;
        var credential = GoogleCredential.FromAccessToken(token);
        return new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "GmailApiReader"
        });
    }
}
