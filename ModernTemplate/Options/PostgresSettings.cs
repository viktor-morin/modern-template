using System.ComponentModel.DataAnnotations;

namespace ModernTemplate.Options;

public sealed class PostgresSettings
{
    [Required]
    public string? ConnectionString { get; init; }
}