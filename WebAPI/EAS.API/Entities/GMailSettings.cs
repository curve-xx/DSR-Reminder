using System;

namespace EAS.API.Entities;

public class GMailSettings
{
    public required string GMailAPIUrl { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}
