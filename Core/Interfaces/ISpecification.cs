using System.Linq.Expressions;

namespace Core.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; } //for then include
    bool IsDistinct { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
    IQueryable<T> ApplyCriteria(IQueryable<T> inputQuery);
}

public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>> Selector { get; }
    Expression<Func<TResult, bool>>? SelectorCriteria { get; }
}
