namespace SuperMarket.Application.Common;

public static class Paging
{
    private const int DefaultPageSize = 12;

    private const int MaxPageSize = 48;

    public static PagingResult Normalize(
        int pageNumber,
        int pageSize)
    {
        pageNumber = pageNumber <= 0
            ? 1
            : pageNumber;

        pageSize = pageSize <= 0
            ? DefaultPageSize
            : pageSize > MaxPageSize
                ? MaxPageSize
                : pageSize;

        return new PagingResult(
            pageNumber,
            pageSize,
            (pageNumber - 1) * pageSize,
            pageSize);
    }
}

public sealed record PagingResult(
    int PageNumber,
    int PageSize,
    int Skip,
    int Take);