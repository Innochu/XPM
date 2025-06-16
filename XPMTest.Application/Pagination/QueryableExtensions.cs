namespace XPMTest.Application.Pagination
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page index and page size must be greater than 0.");
            }
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
    }
}
