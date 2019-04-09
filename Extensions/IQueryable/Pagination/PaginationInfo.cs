namespace Extensions.IQueryable.Pagination
{
    public class PaginationInfo
    {
        public int PageSize { get; }
        public int CurrentPage { get; }

        public PaginationInfo(int pageSize, int currentPage)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
        }
    }
}
