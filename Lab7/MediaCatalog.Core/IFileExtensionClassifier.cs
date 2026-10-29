namespace MediaCatalog.Core;

public interface IFileExtensionClassifier
{
    MediaType Classify(string extension);
}
