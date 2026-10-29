using MediaCatalog.Core;

namespace MediaCatalog.Infrastructure;

public class FileExtensionClassifier : IFileExtensionClassifier
{
    private static readonly HashSet<string> AudioExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a"
    };

    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".mpg"
    };

    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".tiff"
    };

    public MediaType Classify(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return MediaType.Unknown;

        var ext = extension.StartsWith('.') ? extension : "." + extension;

        if (AudioExtensions.Contains(ext)) return MediaType.Audio;
        if (VideoExtensions.Contains(ext)) return MediaType.Video;
        if (ImageExtensions.Contains(ext)) return MediaType.Image;

        return MediaType.Unknown;
    }
}