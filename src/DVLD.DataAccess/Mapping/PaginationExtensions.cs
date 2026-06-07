using DVLD.Contracts.Common;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Mapping
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResult<T>> ToPagedListAsync<T>(
            this IOrderedQueryable<T> source,
            PaginationParams paging,
            CancellationToken ct = default)
        {
            var pageIndex = paging.PageNumber - 1;

            var result = await source
                .Skip(pageIndex * paging.PageSize)
                .Take(paging.PageSize)
                .Select(item => new
                {
                    Item = item,
                    TotalCount = source.Count()
                })
                .ToListAsync(ct);

            return new PagedResult<T>
            {
                Items = result.Select(r => r.Item).ToList(),
                TotalCount = result.FirstOrDefault()?.TotalCount ?? 0,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }
    }
}
