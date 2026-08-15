using System.Text.RegularExpressions;

namespace ClipLab.Core;

public static class YouTubeUrlValidator
{
    private static readonly Regex Pattern =
        new(@"^(https?:\/\/)?(www\.)?(m\.)?(youtube\.com\/|youtu\.be\/).+$", RegexOptions.IgnoreCase);

    public static bool IsValid(string? url) =>
        !string.IsNullOrWhiteSpace(url) && Pattern.IsMatch(url);
}
