using System;

namespace EAS.API.Entities;

public class GMailSettings
{
    public required string Scopes { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string RedirectUri { get; set; }
    public required string ApplicationName { get; set; }
    public required string UserEmail { get; set; }
}
