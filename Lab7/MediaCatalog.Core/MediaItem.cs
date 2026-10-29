namespace MediaCatalog.Core;

public enum MediaType
{
    Audio,
    Video,
    Image,
    Unknown
}

public class MediaItem
{
    public string FilePath { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public MediaType Type { get; init; }
    public long SizeBytes { get; init; }
    public DateTime LastModified { get; init; }

    public override string ToString() =>
        $"[{Type}] {FileName} ({SizeBytes / 1024} КБ)";
}
