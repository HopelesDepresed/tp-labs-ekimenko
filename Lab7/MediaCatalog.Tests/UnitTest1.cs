using MediaCatalog.Core;
using MediaCatalog.Infrastructure;
using Xunit;

namespace MediaCatalog.Tests;

// ==========================================
// 1. Тесты классификатора расширений
// ==========================================
public class FileExtensionClassifierTests
{
    private readonly FileExtensionClassifier _classifier = new();

    // Параметризованный тест (Theory)
    [Theory]
    [InlineData(".mp3", MediaType.Audio)]
    [InlineData(".WAV", MediaType.Audio)] // Проверка регистра
    [InlineData(".mp4", MediaType.Video)]
    [InlineData(".jpg", MediaType.Image)]
    public void Classify_KnownExtensions_ReturnsCorrectType(string ext, MediaType expected)
    {
        // Act
        var result = _classifier.Classify(ext);
        // Assert
        Assert.Equal(expected, result);
    }

    // Тест граничного случая (неизвестное расширение)
    [Fact]
    public void Classify_UnknownExtension_ReturnsUnknown()
    {
        Assert.Equal(MediaType.Unknown, _classifier.Classify(".txt"));
        Assert.Equal(MediaType.Unknown, _classifier.Classify(".exe"));
    }

    // Тест граничного случая (пустая строка)
    [Fact]
    public void Classify_EmptyString_ReturnsUnknown()
    {
        Assert.Equal(MediaType.Unknown, _classifier.Classify(""));
    }
}

// ==========================================
//