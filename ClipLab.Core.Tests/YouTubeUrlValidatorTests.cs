using ClipLab.Core;

namespace ClipLab.Core.Tests;

public class YouTubeUrlValidatorTests
{
    [Theory]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("http://www.youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://youtube.com/watch?v=dQw4w9WgXcQ")] // no www. - the original regex rejected this
    [InlineData("youtube.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://youtu.be/dQw4w9WgXcQ")]
    [InlineData("youtu.be/dQw4w9WgXcQ")]
    [InlineData("https://m.youtube.com/watch?v=dQw4w9WgXcQ")]
    public void IsValid_AcceptsRealYouTubeLinks(string url)
    {
        Assert.True(YouTubeUrlValidator.IsValid(url));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("not a link")]
    [InlineData("https://vimeo.com/12345")]
    [InlineData("https://notyoutube.com/watch?v=abc")]
    [InlineData("https://youtube.com.evil.com/watch?v=abc")]
    public void IsValid_RejectsInvalidLinks(string? url)
    {
        Assert.False(YouTubeUrlValidator.IsValid(url));
    }
}
