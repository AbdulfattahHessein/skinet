using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class GenericRepository<T>(StoreContext context) : IGenericRepository<T>
    where T : BaseEntity
{
    readonly DbSet<T> dbSet = context.Set<T>();

    public void Add(T entity) => context.Add(entity);

    public Task<int> CountAsync(ISpecification<T> spec)
    {
        throw new NotImplementedException();
    }

    public void Delete(T entity) => context.Remove(entity);

    public bool Exists(int id) => dbSet.Any(p => p.Id == id);

    public async Task<T?> GetByIdAsync(int id) => await dbSet.FindAsync(id);

    public async Task<T?> GetEntityWithSpec(ISpecification<T> spec)
    {
        return await dbSet.ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> spec)
    {
        return await dbSet.ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<T>> ListAllAsync() => await dbSet.ToListAsync();

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        return await dbSet.ApplySpecification(spec).ToListAsync();
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec)
    {
        return await dbSet.ApplySpecification(spec).ToListAsync();
    }

    public async Task<bool> SaveAllAsync() => await context.SaveChangesAsync() > 0;

    public void Update(T entity)
    {
        context.Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
    }

    // private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    // {
    //     return dbSet.ApplySpecification(spec);
    //     // return SpecificationEvaluator<T>.Apply(dbSet.AsQueryable(), spec);
    // }
}
