using System;
using System.Linq.Expressions;
using Core.Interfaces;

namespace Core.Specification;

public class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification() { }

    public BaseSpecification(Expression<Func<T, bool>>? criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>>? Criteria { get; private set; }

    public List<Expression<Func<T, object>>> Includes => throw new NotImplementedException();

    public List<string> IncludeStrings => throw new NotImplementedException();

    public Expression<Func<T, object>>? OrderBy { get; set; }

    public Expression<Func<T, object>>? OrderByDescending { get; set; }

    public bool IsDistinct { get; private set; }

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    protected void ApplyDistinct() => IsDistinct = true;
}

public class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
{
    public BaseSpecification(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? criteria = null
    )
        : base(criteria)
    {
        Selector = selector;
    }

    public Expression<Func<T, TResult>> Selector { get; private set; }
}
