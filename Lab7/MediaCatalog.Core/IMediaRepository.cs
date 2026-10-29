namespace MediaCatalog.Core;

public interface IMediaRepository
{
    void AddRange(IEnumerable<MediaItem> items);
    void Clear();
    IReadOnlyList<MediaItem> GetAll();
    IReadOnlyList<MediaItem> SearchByKeyword(string keyword);
    IReadOnlyList<MediaItem> FilterByType(MediaType type);
    int Count { get; }
}
