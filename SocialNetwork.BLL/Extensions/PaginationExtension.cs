public static class PaginationExtension
{
    public static IQueryable<T> Pagination<T>(this IQueryable<T> source, int currentPage, int pageSize)
    {
        return source.Skip((currentPage - 1) * pageSize).Take(pageSize);
    }

}