using System;

namespace WoadEngine.Persistence;

public sealed class SaveFile<T>
{
    public string GameId { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public T Data { get; set; } = default!;
}