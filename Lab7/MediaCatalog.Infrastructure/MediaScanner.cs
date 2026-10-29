using MediaCatalog.Core;

namespace MediaCatalog.Infrastructure;

public class MediaScanner : IMediaScanner
{
    private readonly IFileExtensionClassifier _classifier;

    public MediaScanner(IFileExtensionClassifier classifier)
    {
        _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
    }

    public IReadOnlyList<MediaItem> ScanDirectory(string path, bool recursive = true)
    {
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Папка не найдена: {path}");

        var searchOption = recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        var items = new List<MediaItem>();

        foreach (var file in Directory.EnumerateFiles(path, "*", searchOption))
        {
            var ext = Path.GetExtension(file);
            var type = _classifier.Classify(ext);

            if (type == MediaType.Unknown)
                continue;

            var fileInfo = new FileInfo(file);
            items.Add(new MediaItem
            {
                FilePath = file,
                FileName = Path.GetFileNameWithoutExtension(file),
                Extension = ext,
                Type = type,
                SizeBytes = fileInfo.Length,
                LastModified = fileInfo.LastWriteTime
            });
        }

        return items.AsReadOnly();
    }
}