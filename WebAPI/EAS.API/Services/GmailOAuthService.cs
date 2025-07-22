using EAS.API.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;

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

    public string GetAuthorizationUrl(string userId)
    {
        var redirectUri = _config["GMailSettings:RedirectUri"];
        var flow = CreateAuthorizationFlow();
        var request = flow.CreateAuthorizationCodeRequest(redirectUri);
        return request.Build().ToString();
    }

    public async Task<UserCredential> ExchangeCodeForTokenAsync(string userId, string code)
    {
        var redirectUri = _config["GMailSettings:RedirectUri"];
        var flow = CreateAuthorizationFlow();
        var token = await flow.ExchangeCodeForTokenAsync(userId, code, redirectUri, CancellationToken.None);
        return new UserCredential(flow, userId, token);
    }

    public async Task<List<Google.Apis.Gmail.v1.Data.Message>> GetTodayEmailsFromAsync(UserCredential credential, string fromEmail)
    {
        var gmailService = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "DSR Reminder"
        });

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var query = $"after:{today:yyyy/MM/dd} before:{tomorrow:yyyy/MM/dd} from:{fromEmail}";

        var request = gmailService.Users.Messages.List("me");
        request.Q = query;

        var response = await request.ExecuteAsync();
        return response.Messages?.ToList() ?? new List<Google.Apis.Gmail.v1.Data.Message>();
    }
}
