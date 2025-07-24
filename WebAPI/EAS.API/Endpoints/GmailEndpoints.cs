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
        // This endpoint starts the OAuth flow for Gmail API
        app.MapGet("/auth/start", (IOptions<GMailSettings> options, GmailOAuthService gmailService) =>
        {
            var userId = options.Value.UserEmail; // or derive from session
            var authUrl = gmailService.GetAuthorizationUrl(userId, options.Value.RedirectUri);
            return Results.Redirect(authUrl);
        });

        // This endpoint handles the OAuth callback from Gmail API
        app.MapGet("/oauth2callback", async (DSRReminderContext context, HttpContext http, GmailOAuthService gmailService, IOptions<GMailSettings> gmailOptions, IOptions<SlackBotSettings> slackOptions) =>
        {
            var code = http.Request.Query["code"].ToString();
            var userId = gmailOptions.Value.UserEmail; // match the earlier one

            if (string.IsNullOrEmpty(code))
                return Results.BadRequest("No code returned from Google");

            var credential = await gmailService.ExchangeCodeForTokenAsync(userId, code, gmailOptions.Value.RedirectUri);

            // Check if the user has any attendance records for today
            var todate = DateTime.Now.Date;
            var fromdate = todate.AddDays(-1);

            var query = context.Attendances
                    .Where(a =>
                        a.IsPresent && !a.IsDSRSent && a.CreatedOn.Date >= fromdate.Date && a.CreatedOn.Date <= todate.Date
                    );

            var attendances = await query.Select(a => new
            {
                a.Id,
                a.EmailId,
                a.Name,
                a.CreatedOn
            }).Distinct().ToListAsync();

            foreach (var attendance in attendances)
            {
                var messages = await gmailService.GetTodayEmailsFromAsync(credential, attendance.EmailId, fromdate, todate);
                if (messages is null || !messages.Any())
                {
                    var service = new SlackService(slackOptions);
                    await service.SendMessageToChannelAsync($"*@{attendance.Name}* please send the *DSR* of *{fromdate.ToString("ddMMyyyy")}*.");
                    continue;
                }
                else
                {
                    foreach (var message in messages)
                    {
                        if (message.Subject.Contains(fromdate.ToString("ddMMyyyy")) && Convert.ToDateTime(message.Date).Date == fromdate.Date)
                        {
                            var existingAttendances = await context.Attendances.Where(a => a.EmailId == attendance.EmailId && a.CreatedOn.Date == attendance.CreatedOn.Date).ToListAsync();
                            if (existingAttendances is null || !existingAttendances.Any()) continue;

                            foreach (var existing in existingAttendances)
                            {
                                existing.IsDSRSent = true;
                                existing.UpdatedBy = "Administrator";
                                existing.UpdatedOn = DateTime.Now;
                            }

                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            var service = new SlackService(slackOptions);
                            await service.SendMessageToChannelAsync($"*@{attendance.Name}* please send the *DSR* of *{fromdate.ToString("ddMMyyyy")}*.");
                        }
                    }
                }
            }

            return Results.Ok("DSR activity done.");
        });

        // This endpoint handles the OAuth callback for DSR reminder for a specific user from Gmail API
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

            var attendance = await context.Attendances.Where(a => a.Id == id && a.IsPresent && !a.IsDSRSent).FirstOrDefaultAsync();
            if (attendance is null) return Results.NotFound();


            var messages = await gmailService.GetTodayEmailsFromAsync(credential, attendance.EmailId, attendance.CreatedOn.Date, DateTime.Now.Date);
            if (messages is null || !messages.Any())
            {
                var service = new SlackService(slackOptions);
                await service.SendMessageToChannelAsync($"*@{attendance.Name}* please send the *DSR* of *{attendance.CreatedOn.ToString("ddMMyyyy")}*.");
            }
            else
            {
                foreach (var message in messages)
                {
                    if (message.Subject.Contains(attendance.CreatedOn.ToString("ddMMyyyy")) && Convert.ToDateTime(message.Date).Date >= attendance.CreatedOn.Date)
                    {
                        var existingAttendances = await context.Attendances.Where(a => a.EmailId == attendance.EmailId && a.CreatedOn.Date == attendance.CreatedOn.Date).ToListAsync();
                        if (existingAttendances is null || !existingAttendances.Any()) continue;

                        foreach (var item in existingAttendances)
                        {
                            item.IsDSRSent = true;
                            item.UpdatedBy = "Administrator";
                            item.UpdatedOn = DateTime.Now;
                        }

                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        var service = new SlackService(slackOptions);
                        await service.SendMessageToChannelAsync($"*@{attendance.Name}* please send the *DSR* of *{attendance.CreatedOn.ToString("ddMMyyyy")}*.");
                    }
                }
            }

            return Results.Ok("DSR activity done.");
        });
    }
}