using System;

namespace EAS.API.Entities;

public class Message
{
    public required string Id { get; set; }
    public required string Subject { get; set; }
    public required string Snippet { get; set; }
    public required string Date { get; set; }

}
