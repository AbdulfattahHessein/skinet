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

    public Expression<Func<T, bool>>? Criteria { get; protected set; }

    public Expression<Func<T, object>>? OrderBy { get; set; }

    public Expression<Func<T, object>>? OrderByDescending { get; set; }

    public bool IsDistinct { get; private set; }

    public int Take { get; private set; }

    public int Skip { get; private set; }

    public bool IsPagingEnabled { get; private set; }

    public List<Expression<Func<T, object>>> Includes { get; } = [];

    public List<string> IncludeStrings { get; } = [];

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString) => IncludeStrings.Add(includeString);

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    protected void ApplyDistinct() => IsDistinct = true;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    public IQueryable<T> ApplyCriteria(IQueryable<T> inputQuery)
    {
        if (Criteria != null)
            inputQuery = inputQuery.Where(Criteria);
        return inputQuery;
    }
}

public class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
{
    public Expression<Func<T, TResult>> Selector { get; private set; }

    public Expression<Func<TResult, bool>>? SelectorCriteria { get; private set; }

    public BaseSpecification(
        Expression<Func<T, TResult>> selector,
        Expression<Func<TResult, bool>>? SelectorCriteria = null
    )
    {
        Selector = selector;
        this.SelectorCriteria = SelectorCriteria;
    }

    public BaseSpecification(
        Expression<Func<T, bool>>? criteria,
        Expression<Func<T, TResult>> selector,
        Expression<Func<TResult, bool>>? SelectorCriteria = null
    )
        : base(criteria)
    {
        Selector = selector;
        this.SelectorCriteria = SelectorCriteria;
    }
}
