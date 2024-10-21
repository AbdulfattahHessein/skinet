using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Core.Entities;

namespace Core.Specification;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(string? brand, string? type, string? sort)
        : base(p =>
            (string.IsNullOrEmpty(brand) || p.Brand == brand)
            && (string.IsNullOrEmpty(type) || p.Type == type)
        )
    {
        switch (sort)
        {
            case "priceAsc":
                AddOrderBy(p => p.Price);
                break;
            case "priceDesc":
                AddOrderByDescending(p => p.Price);
                break;
            default:
                AddOrderBy(p => p.Name);
                break;
        }
    }
}

public class ProductSpecification<TResult> : BaseSpecification<Product, TResult>
{
    public ProductSpecification(
        Expression<Func<Product, TResult>> selector,
        bool IsDistinct = false
    )
        : base(selector)
    {
        if (IsDistinct)
            ApplyDistinct();
    }
}
