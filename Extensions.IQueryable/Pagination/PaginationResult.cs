using System;
using System.Collections.Generic;

namespace Extensions.IQueryable.Pagination
{
    public class PaginationResult<T>
    {
        public IEnumerable<T> Data { get; }
        public int TotalRecords { get; }
        public int PageSize { get; }
        public int CurrentPage { get; }

        public PaginationResult(IEnumerable<T> data, int totalRecords, int pageSize, int currentPage)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (currentPage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentPage));
            }

            if (totalRecords <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalRecords));
            }

            Data = data;
            TotalRecords = totalRecords;
            PageSize = pageSize;
            CurrentPage = currentPage;
        }
    }
}
