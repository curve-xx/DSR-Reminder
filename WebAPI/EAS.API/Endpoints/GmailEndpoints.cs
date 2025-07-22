using EAS.API.Entities;
using EAS.API.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EAS.API.Endpoints;

public static class GmailEndpoints
{
    public static void MapGmailServiceEndpoints(this WebApplication app)
    {
        app.MapGet("/auth/start", (IOptions<GMailSettings> options, GmailOAuthService gmailService) =>
        {
            var userId = options.Value.UserEmail; // or derive from session
            var authUrl = gmailService.GetAuthorizationUrl(userId);
            return Results.Redirect(authUrl);
        });

        app.MapGet("/oauth2callback", async (HttpContext http, IOptions<GMailSettings> options, GmailOAuthService gmailService) =>
        {
            var code = http.Request.Query["code"].ToString();
            var userId = options.Value.UserEmail; // match the earlier one

            if (string.IsNullOrEmpty(code))
                return Results.BadRequest("No code returned from Google");

            var credential = await gmailService.ExchangeCodeForTokenAsync(userId, code);           

            // You can now use gmailService.Users.Messages.List("me") etc.
            string fromEmail = "*************";
            var messages = await gmailService.GetTodayEmailsFromAsync(credential, fromEmail);

            return Results.Ok(messages);
        });
    }
}
