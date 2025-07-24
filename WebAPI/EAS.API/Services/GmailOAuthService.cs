using EAS.API.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace EAS.API.Services;

public class GmailOAuthService
{
    private readonly IConfiguration _config;

    public GmailOAuthService(IConfiguration config)
    {
        _config = config;
    }

    public GoogleAuthorizationCodeFlow CreateAuthorizationFlow()
    {
        var clientSecrets = new ClientSecrets
        {
            ClientId = _config["GMailSettings:ClientId"],
            ClientSecret = _config["GMailSettings:ClientSecret"]
        };

        return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets,
            Scopes = new[] { GmailService.Scope.GmailReadonly },
            DataStore = new FileDataStore("GmailTokenStore", true)
        });
    }

    public string GetAuthorizationUrl(string userId, string redirectUri)
    {
        var flow = CreateAuthorizationFlow();
        var request = flow.CreateAuthorizationCodeRequest(redirectUri);
        return request.Build().ToString();
    }

    public async Task<UserCredential> ExchangeCodeForTokenAsync(string userId, string code, string redirectUri)
    {
        var flow = CreateAuthorizationFlow();
        var token = await flow.ExchangeCodeForTokenAsync(userId, code, redirectUri, CancellationToken.None);
        return new UserCredential(flow, userId, token);
    }

    public async Task<List<Message>> GetTodayEmailsFromAsync(UserCredential credential, string fromEmail, DateTime fromDate, DateTime toDate)
    {
        // Example: create Gmail service and get emails
        var gmailService = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _config["GMailSettings:ApplicationName"]
        });

        // var today = DateTime.Now.Date;
        // var yesterday = today.AddDays(-1);

        var query = $"after:{fromDate:yyyy/MM/dd} before:{toDate:yyyy/MM/dd} from:{fromEmail}";

        var request = gmailService.Users.Messages.List("me");
        request.Q = query;

        var response = await request.ExecuteAsync();
        var messages = new List<Message>();

        if (response.Messages == null || !response.Messages.Any())
            return messages;

        foreach (var msg in response.Messages)
        {
            var emailInfo = await gmailService.Users.Messages.Get("me", msg.Id).ExecuteAsync();
            var subject = emailInfo.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")?.Value;
            var date = emailInfo.Payload.Headers.FirstOrDefault(h => h.Name == "Date")?.Value;

            messages.Add(new Message
            {
                Id = msg.Id,
                Subject = subject ?? string.Empty,
                Date = date ?? string.Empty,
                Snippet = emailInfo.Snippet
            });
        }

        return messages;
    }
}
