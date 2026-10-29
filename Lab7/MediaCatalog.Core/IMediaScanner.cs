namespace MediaCatalog.Core;

public interface IMediaScanner
{
    IReadOnlyList<MediaItem> ScanDirectory(string path, bool recursive = true);
}

