using System.Linq.Expressions;

namespace NencerApi.Helpers
{
    public static class Filter
    {
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Dictionary<string, object> filters)
        {
            if (filters == null || filters.Count == 0)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");

            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperty(filter.Key);
                if (property == null)
                    continue;

                var left = Expression.Property(parameter, property);
                var right = Expression.Constant(filter.Value);

                var predicate = Expression.Equal(left, right);
                var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);

                query = query.Where(lambda);
            }

            return query;
        }

        public static IQueryable<T> ApplyDateRangeFilter<T>(this IQueryable<T> query, Expression<Func<T, DateTime>> dateProperty, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                query = query.Where(e => dateProperty.Compile()(e) >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => dateProperty.Compile()(e) <= endDate.Value);
            }

            return query;
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
