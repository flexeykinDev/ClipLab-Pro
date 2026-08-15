using System.Globalization;

namespace ClipLab.Core;

public readonly record struct TrimRange(double Start, double End);

public static class TrimRangeValidator
{
    public static bool TryValidate(string? startText, string? endText, out TrimRange range, out string? error)
    {
        range = default;

        if (!double.TryParse(startText, NumberStyles.Float, CultureInfo.InvariantCulture, out double start) ||
            !double.TryParse(endText, NumberStyles.Float, CultureInfo.InvariantCulture, out double end))
        {
            error = "Введіть числові значення для Start і End.";
            return false;
        }

        if (start < 0)
        {
            error = "Start не може бути від'ємним.";
            return false;
        }

        if (end <= start)
        {
            error = "End має бути більшим за Start.";
            return false;
        }

        range = new TrimRange(start, end);
        error = null;
        return true;
    }
}
