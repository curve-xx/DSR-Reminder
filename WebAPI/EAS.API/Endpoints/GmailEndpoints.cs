using EAS.API.Data;
using EAS.API.Entities;
using EAS.API.Services;
using Microsoft.EntityFrameworkCore;
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

        app.MapGet("/oauth2callback", async (DSRReminderContext context, HttpContext http, GmailOAuthService gmailService, IOptions<GMailSettings> gmailOptions, IOptions<SlackBotSettings> slackOptions) =>
        {
            var code = http.Request.Query["code"].ToString();
            var userId = gmailOptions.Value.UserEmail; // match the earlier one

            if (string.IsNullOrEmpty(code))
                return Results.BadRequest("No code returned from Google");

            var credential = await gmailService.ExchangeCodeForTokenAsync(userId, code);

            // Check if the user has any attendance records for today
            var todate = DateTime.UtcNow.Date;
            var fromdate = todate.AddDays(-1);

            var query = context.Attendances
                    .Where(a =>
                        a.IsPresent && a.CreatedOn.Date >= fromdate.Date && a.CreatedOn.Date <= todate.Date
                    );

            var attendances = await query.Select(a => new
            {
                a.EmailId,
                a.Name
            }).Distinct().ToListAsync();

            foreach (var attendance in attendances)
            {
                var messages = await gmailService.GetTodayEmailsFromAsync(credential, attendance.EmailId);
                foreach (var message in messages)
                {
                    if (!message.Subject.Contains(fromdate.ToString("ddMMyyyy")) && Convert.ToDateTime(message.Date).Date == fromdate.Date)
                    {
                        var service = new SlackService(slackOptions);
                        await service.SendMessageToChannelAsync($"*@{attendance.Name.Split().First()}* please send the *DSR* of *{fromdate.ToString("ddMMyyyy")}*.");
                    }
                }
            }

            return Results.Ok("Messages sent to Channel.");
        });
    }
}
