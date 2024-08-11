namespace ModernTemplate;

public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Type { get; init; }
    public string Content { get; init; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
    public string? Error { get; set; }
}
