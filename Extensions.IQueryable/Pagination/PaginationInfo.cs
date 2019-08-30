using System;

namespace Extensions.IQueryable.Pagination
{
    public class PaginationInfo
    {
        private const int DefaultPageSize = 10;
        private const int DefaultCurrentPage = 1;

        public int PageSize { get; }
        public int CurrentPage { get; }

        public PaginationInfo()
        {
            PageSize = DefaultPageSize;
            CurrentPage = DefaultCurrentPage;
        }

        public PaginationInfo(int pageSize, int currentPage)
        {
            if(pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (currentPage <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(currentPage));
            }

            PageSize = pageSize;
            CurrentPage = currentPage;
        }
    }
}
