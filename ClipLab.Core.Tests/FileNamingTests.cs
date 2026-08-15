using ClipLab.Core;

namespace ClipLab.Core.Tests;

public class FileNamingTests
{
    [Theory]
    [InlineData("Normal Video Title", "Normal Video Title")]
    [InlineData("Video: The Sequel?", "Video_ The Sequel_")]
    [InlineData("a/b\\c", "a_b_c")]
    [InlineData("", "video")]
    [InlineData("???", "___")]
    public void SanitizeFileName_RemovesInvalidChars(string input, string expected)
    {
        Assert.Equal(expected, FileNaming.SanitizeFileName(input));
    }

    [Fact]
    public void SanitizeFileName_ResultHasNoInvalidChars()
    {
        string sanitized = FileNaming.SanitizeFileName("weird: / \\ * ? \" < > | name");
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            Assert.DoesNotContain(c, sanitized);
        }
    }

    [Fact]
    public void BuildSavePath_CombinesDirectoryAndFileName()
    {
        string path = FileNaming.BuildSavePath(@"C:\Downloads", "video.mp4");
        Assert.Equal(Path.Combine(@"C:\Downloads", "video.mp4"), path);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void BuildSavePath_ThrowsOnEmptyDirectory(string? directory)
    {
        Assert.Throws<ArgumentException>(() => FileNaming.BuildSavePath(directory!, "video.mp4"));
    }
}
