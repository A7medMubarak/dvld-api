using DVLD.Contracts.Common;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Mapping
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResult<T>> ToPagedListAsync<T>(
            this IQueryable<T> source,
            PaginationParams paging,
            CancellationToken ct = default)
        {
            var totalCount = await source.CountAsync(ct);

            var items = await source
                .Skip((paging.PageNumber - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync(ct);

            return new PagedResult<T>
            {
                Items = items,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
