using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public static class SpecificationEvaluator
{
    public static IQueryable<T> ApplySpecification<T>(
        this IQueryable<T> inputQuery,
        ISpecification<T> spec
    )
        where T : BaseEntity
    {
        var query = inputQuery;
        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        if (spec.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }

        if (spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        // if (spec.IsDistinct)
        // {
        //     query = query.Distinct();
        // }
        // if (spec.IsPagingEnabled)
        // {
        //     query = query.Skip(spec.Skip).Take(spec.Take);
        // }

        // query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        // query = spec.IncludeStrings.Aggregate(
        //     query,
        //     (current, include) => current.Include(include)
        // );
        return query;
    }

    public static IQueryable<TResult> ApplySpecification<T, TResult>(
        this IQueryable<T> inputQuery,
        ISpecification<T, TResult> spec
    )
        where T : BaseEntity
    {
        var query = inputQuery.ApplySpecification<T>(spec);

        var selectQuery = query.Select(spec.Selector);

        if (spec.IsDistinct)
        {
            selectQuery = selectQuery.Distinct();
        }

        return selectQuery;
    }
}
