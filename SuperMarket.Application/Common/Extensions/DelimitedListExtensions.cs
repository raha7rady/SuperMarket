namespace SuperMarket.Application.Common.Extensions;

public static class DelimitedListExtensions
{
    public static IReadOnlyList<string> SplitList(this string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
