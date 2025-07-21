using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;

namespace EAS.API.Endpoints;

public static class GmailEndpoints
{
    public static RouteGroupBuilder MapGmailServiceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/gmail").WithParameterValidation();

        group.MapGet("/emails", async (HttpContext httpContext) =>
        {
            var authResult = await httpContext.AuthenticateAsync();
            if (!authResult.Succeeded || authResult.Principal == null || authResult.Principal.Identity == null || !authResult.Principal.Identity.IsAuthenticated)
                return Results.Unauthorized();

            var accessToken = await httpContext.GetTokenAsync("access_token");

            var credential = GoogleCredential.FromAccessToken(accessToken);
            var gmailService = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "DSR Reminder"
            });

            // Example: Get list of messages
            var messages = await gmailService.Users.Messages.List("me").ExecuteAsync();
            return Results.Ok(messages.Messages);

        }).RequireAuthorization();

        // group.MapGet("/emails", async (HttpContext httpContext, string from, DateTime fromdate, DateTime todate) =>
        // {
        //     var authResult = await httpContext.AuthenticateAsync();
        //     if (!authResult.Succeeded || authResult.Principal == null || authResult.Principal.Identity == null || !authResult.Principal.Identity.IsAuthenticated)
        //         return Results.Unauthorized();

        //     var accessToken = await httpContext.GetTokenAsync("access_token");

        //     var credential = GoogleCredential.FromAccessToken(accessToken);
        //     var gmailService = new GmailService(new BaseClientService.Initializer
        //     {
        //         HttpClientInitializer = credential,
        //         ApplicationName = "DSR Reminder"
        //     });

        //     var query = $"after:{fromdate:yyyy/MM/dd} before:{todate:yyyy/MM/dd} from:{from}";

        //     var request = gmailService.Users.Messages.List("me");
        //     request.Q = query;

        //     var response = await request.ExecuteAsync();
        //     var messages = new List<object>();

        //     if (response.Messages == null || !response.Messages.Any())
        //         return Results.NoContent();

        //     foreach (var msg in response.Messages)
        //     {
        //         var emailInfo = await gmailService.Users.Messages.Get("me", msg.Id).ExecuteAsync();
        //         var subject = emailInfo.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")?.Value;
        //         var date = emailInfo.Payload.Headers.FirstOrDefault(h => h.Name == "Date")?.Value;

        //         messages.Add(new
        //         {
        //             Subject = subject,
        //             Date = date,
        //             Snippet = emailInfo.Snippet
        //         });
        //     }

        //     return Results.Ok(messages);

        // }).RequireAuthorization();


        // group.MapPost("/dsr-reminder", async (IConfiguration config, [FromQuery] string from, [FromQuery] DateTime fromdate, [FromQuery] DateTime todate) =>
        // {
        //     var service = new GMailService(config);
        //     var emails = await service.GetEmailsFrom(from, fromdate, todate);
        //     return Results.Ok("DSR reminder messages sent to Channel.");
        // });

        return group;
    }

}
