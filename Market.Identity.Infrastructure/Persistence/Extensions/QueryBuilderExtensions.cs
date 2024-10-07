using System.Linq.Expressions;

namespace Market.Identity.Infrastructure.Persistence.Extensions;

public static class QueryBuilderExtensions
{
    public static IQueryable<TEntity> WhereIf<TEntity>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>> predicate,
        bool condition)
    {
        return condition ? query.Where(predicate) : query;
    }
}