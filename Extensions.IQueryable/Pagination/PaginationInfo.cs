using System;

namespace Extensions.IQueryable.Pagination
{
    public class PaginationInfo
    {
        public int PageSize { get; }
        public int CurrentPage { get; }

        public PaginationInfo(int pageSize, int currentPage)
        {
            if(pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (currentPage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentPage));
            }

            PageSize = pageSize;
            CurrentPage = currentPage;
        }
    }
}
