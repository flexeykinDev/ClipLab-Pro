namespace ClipLab.Core;

public static class FileNaming
{
    public static string SanitizeFileName(string title)
    {
        if (string.IsNullOrEmpty(title))
            return "video";

        char[] invalidChars = Path.GetInvalidFileNameChars();
        Span<char> buffer = title.Length <= 256 ? stackalloc char[title.Length] : new char[title.Length];
        for (int i = 0; i < title.Length; i++)
        {
            buffer[i] = Array.IndexOf(invalidChars, title[i]) >= 0 ? '_' : title[i];
        }

        string sanitized = new string(buffer).Trim();
        return sanitized.Length == 0 ? "video" : sanitized;
    }

    public static string BuildSavePath(string directory, string fileName)
    {
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("Directory must not be empty.", nameof(directory));

        return Path.Combine(directory, fileName);
    }
}
