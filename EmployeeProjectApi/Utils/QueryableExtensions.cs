using EmployeeProjectApi.Models;
using EmployeeProjectApi.Utils;
using System.Linq.Dynamic.Core;          // NuGet: System.Linq.Dynamic.Core

namespace EmployeeProjectApi.Utils;

public static class QueryableExtensions
{
    public static IQueryable<Employee> Apply(this IQueryable<Employee> q,
                                             QueryParameters p)
    {
        if (!string.IsNullOrEmpty(p.Department))
            q = q.Where(e => e.Department == p.Department);

        q = p.SortOrder.ToLower() == "desc"
            ? q.OrderBy($"{p.SortBy} descending")
            : q.OrderBy($"{p.SortBy}");

        return q.Skip(p.Skip).Take(p.Take);
    }
}
