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
            var authUrl = gmailService.GetAuthorizationUrl(userId, options.Value.RedirectUri);
            return Results.Redirect(authUrl);
        });

        app.MapGet("/oauth2callback", async (DSRReminderContext context, HttpContext http, GmailOAuthService gmailService, IOptions<GMailSettings> gmailOptions, IOptions<SlackBotSettings> slackOptions) =>
        {
            var code = http.Request.Query["code"].ToString();
            var userId = gmailOptions.Value.UserEmail; // match the earlier one

            if (string.IsNullOrEmpty(code))
                return Results.BadRequest("No code returned from Google");

            var credential = await gmailService.ExchangeCodeForTokenAsync(userId, code, gmailOptions.Value.RedirectUri);

            // Check if the user has any attendance records for today
            var todate = DateTime.UtcNow.Date;
            var fromdate = todate.AddDays(-1);

            var query = context.Attendances
                    .Where(a =>
                        a.IsPresent && a.CreatedOn.Date >= fromdate.Date && a.CreatedOn.Date <= todate.Date
                    );

            var attendances = await query.Select(a => new
            {
                a.Id,
                a.EmailId,
                a.Name
            }).Distinct().ToListAsync();

            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            foreach (var attendance in attendances)
            {
                var messages = await gmailService.GetTodayEmailsFromAsync(credential, attendance.EmailId, yesterday, today);
                foreach (var message in messages)
                {
                    if (!message.Subject.Contains(fromdate.ToString("ddMMyyyy")) && Convert.ToDateTime(message.Date).Date == fromdate.Date)
                    {
                        var service = new SlackService(slackOptions);
                        await service.SendMessageToChannelAsync($"*@{attendance.Name}* please send the *DSR* of *{fromdate.ToString("ddMMyyyy")}*.");
                    }
                    else
                    {
                        var existing = await context.Attendances.FindAsync(attendance.Id);
                        if (existing is null) continue;

                        existing.IsDSRSent = true;
                        existing.UpdatedBy = "Administrator";
                        existing.UpdatedOn = DateTime.Now;

                        await context.SaveChangesAsync();
                    }
                }
            }

            return Results.Ok("DSR activity done.");
        });

        app.MapGet("/dsrreminderoauth2callback", async (DSRReminderContext context, HttpContext http, GmailOAuthService gmailService, IOptions<GMailSettings> gmailOptions, IOptions<SlackBotSettings> slackOptions) =>
        {
            var code = http.Request.Query["code"].ToString();
            var id = DSRReminder.id; // Assuming this is set somewhere in the context or passed as a query parameter
            if (id == 0)
                return Results.BadRequest("No attendance id provided");

            var userId = gmailOptions.Value.UserEmail; // match the earlier one

            if (string.IsNullOrEmpty(code))
                return Results.BadRequest("No code returned from Google");

            var credential = await gmailService.ExchangeCodeForTokenAsync(userId, code, gmailOptions.Value.DSRReminderRedirectUri);

            var attendance = await context.Attendances.Where(a => a.Id == Convert.ToInt32(id) && a.IsPresent).FirstOrDefaultAsync();
            if (attendance is null) return Results.NotFound();


            var messages = await gmailService.GetTodayEmailsFromAsync(credential, attendance.EmailId, attendance.CreatedOn.Date, attendance.CreatedOn.Date.AddDays(1));
            foreach (var message in messages)
            {
                if (!message.Subject.Contains(attendance.CreatedOn.ToString("ddMMyyyy")) && Convert.ToDateTime(message.Date).Date == attendance.CreatedOn.Date)
                {
                    var service = new SlackService(slackOptions);
                    await service.SendMessageToChannelAsync($"*@{attendance.Name}* please send the *DSR* of *{attendance.CreatedOn.ToString("ddMMyyyy")}*.");
                }
                else
                {
                    var existing = await context.Attendances.FindAsync(attendance.Id);
                    if (existing is null) continue;

                    existing.IsDSRSent = true;
                    existing.UpdatedBy = "Administrator";
                    existing.UpdatedOn = DateTime.Now;

                    await context.SaveChangesAsync();
                }
            }

            return Results.Ok("DSR activity done.");
        });
    }
}
