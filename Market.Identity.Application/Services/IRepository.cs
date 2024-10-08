using System.Linq.Expressions;
using Market.Identity.Application.Infrastructure.Mappers;

namespace Market.Identity.Application.Services;

public interface IRepository<TEntity>
{
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken);
    
    Task<TEntity?> GetByAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken);
    
    IEnumerable<TEntity> GetAllByAsync(
        Expression<Func<TEntity, bool>>? predicate);
    
    Task<TResult?> GetByAndMapAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate, 
        IMapWith<TEntity, TResult> mapper, 
        CancellationToken cancellationToken);
    
    IEnumerable<TResult> GetAllByAndMapAsync<TResult>(
        Expression<Func<TEntity, bool>>? predicate, 
        IMapWith<TEntity, TResult> mapper);
    
    Task<TEntity> AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken);
    
    Task<TEntity> UpdateAsync(TEntity entity, 
        CancellationToken cancellationToken);
    
    Task<TEntity> DeleteAsync(TEntity entity, 
        CancellationToken cancellationToken);

    bool IsEntityPresentInDb(IIdentityDbContext dbContext);
}