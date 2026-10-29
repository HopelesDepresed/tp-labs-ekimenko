using MediaCatalog.Core;

namespace MediaCatalog.Infrastructure;

public class InMemoryMediaRepository : IMediaRepository
{
    private readonly List<MediaItem> _items = new();
    private readonly object _lock = new();

    public void AddRange(IEnumerable<MediaItem> items)
    {
        lock (_lock)
        {
            _items.AddRange(items);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _items.Clear();
        }
    }

    public IReadOnlyList<MediaItem> GetAll()
    {
        lock (_lock)
        {
            return _items.ToList().AsReadOnly();
        }
    }

    public IReadOnlyList<MediaItem> SearchByKeyword(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Ключевое слово не может быть пустым", nameof(keyword));

        var all = GetAll();
        var lower = keyword.ToLowerInvariant();

        return all
            .Where(item =>
                item.FileName.ToLowerInvariant().Contains(lower) ||
                item.FilePath.ToLowerInvariant().Contains(lower))
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<MediaItem> FilterByType(MediaType type)
    {
        return GetAll()
            .Where(item => item.Type == type)
            .ToList()
            .AsReadOnly();
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _items.Count;
            }
        }
    }
}