using ClipLab.Core;

namespace ClipLab.Core.Tests;

public class TrimRangeValidatorTests
{
    [Fact]
    public void TryValidate_AcceptsValidRange()
    {
        bool ok = TrimRangeValidator.TryValidate("5", "10", out var range, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal(5, range.Start);
        Assert.Equal(10, range.End);
    }

    [Fact]
    public void TryValidate_AcceptsDecimalSeconds()
    {
        bool ok = TrimRangeValidator.TryValidate("1.5", "3.25", out var range, out _);

        Assert.True(ok);
        Assert.Equal(1.5, range.Start);
        Assert.Equal(3.25, range.End);
    }

    [Theory]
    [InlineData("abc", "10")]
    [InlineData("5", "xyz")]
    [InlineData("", "10")]
    [InlineData("5", "")]
    public void TryValidate_RejectsNonNumeric(string start, string end)
    {
        bool ok = TrimRangeValidator.TryValidate(start, end, out _, out var error);

        Assert.False(ok);
        Assert.NotNull(error);
    }

    [Fact]
    public void TryValidate_RejectsNegativeStart()
    {
        bool ok = TrimRangeValidator.TryValidate("-5", "10", out _, out var error);

        Assert.False(ok);
        Assert.NotNull(error);
    }

    [Theory]
    [InlineData("10", "10")] // end == start
    [InlineData("10", "5")]  // end < start - this exact case used to silently pass to ffmpeg with -ss 10 -to 5
    public void TryValidate_RejectsEndNotAfterStart(string start, string end)
    {
        bool ok = TrimRangeValidator.TryValidate(start, end, out _, out var error);

        Assert.False(ok);
        Assert.NotNull(error);
    }
}
