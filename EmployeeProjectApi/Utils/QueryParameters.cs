namespace EmployeeProjectApi.Utils;

public record QueryParameters
(
    int Page = 1,
    int PageSize = 10,
    string? Department = null,
    string SortBy = "Id",
    string SortOrder = "asc"
)
{
    public const int MaxPageSize = 50;

    public int Skip => (Page - 1) * PageSize;

    public int Take => PageSize > MaxPageSize ? MaxPageSize : PageSize;
}
