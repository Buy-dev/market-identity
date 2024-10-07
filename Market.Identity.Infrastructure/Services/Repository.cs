using System.Linq.Expressions;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Services;
using Market.Identity.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Infrastructure.Services;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly IIdentityDbContext _dbContext;

    public Repository(IIdentityDbContext dbContext)
    {
        var isEntityPresentInDb = IsEntityPresentInDb(dbContext);
        if (!isEntityPresentInDb)
            throw new NotImplementedException($"Сущность {typeof(TEntity).Name} не найдена в базе данных");
        
        _dbContext = dbContext;
    }

    public Task<TEntity?> GetByAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken)
        => _dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);

    public IEnumerable<TEntity> GetAllByAsync(
        Expression<Func<TEntity, bool>>? predicate)
        => _dbContext.Set<TEntity>()
                .AsNoTracking()
                .WhereIf(predicate, predicate is not null)
                .AsEnumerable();

    public Task<TResult?> GetByAndMapAsync<TResult>(
        Expression<Func<TEntity, bool>>? predicate, 
        IMapWith<TEntity, TResult> mapper, 
        CancellationToken cancellationToken)
        => _dbContext.Set<TEntity>()
                .AsNoTracking()
                .WhereIf(predicate, predicate is not null)
                .Select(e => mapper.Map(e))
                .FirstOrDefaultAsync(cancellationToken);

    public IEnumerable<TResult> GetAllByAndMapAsync<TResult>(
        Expression<Func<TEntity, bool>>? predicate, 
        IMapWith<TEntity, TResult> mapper)
        => _dbContext.Set<TEntity>()
                .AsNoTracking()
                .WhereIf(predicate, predicate is not null)
                .Select(e => mapper.Map(e))
                .AsEnumerable();

    public async Task<TEntity> AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken)
    {
        await _dbContext.Set<TEntity>()
            .AddAsync(entity, cancellationToken);
        
        var result = await _dbContext.SaveAsync(cancellationToken);
        return result 
            ? entity 
            : throw new Exception("Ошибка при добавлении сущности");
    }

    public async Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken)
    {
        _dbContext.AttachEntityIfNeeded(entity);
        _dbContext.Set<TEntity>()
            .Update(entity);
        
        var result = await _dbContext.SaveAsync(cancellationToken);
        return result
            ? entity
            : throw new Exception("Ошибка при изменении сущности");
    }

    public async Task<TEntity> DeleteAsync(
        TEntity entity, 
        CancellationToken cancellationToken)
    {
        _dbContext.AttachEntityIfNeeded(entity);
        _dbContext.Set<TEntity>()
            .Remove(entity);
        
        var result = await _dbContext.SaveAsync(cancellationToken);
        return result
            ? entity
            : throw new Exception("Ошибка при удалении сущности");
    }

    public bool IsEntityPresentInDb(IIdentityDbContext dbContext)
    {
        try
        {
            var set = dbContext.Set<TEntity>();
            _ = set.EntityType;
            return true;
        }
        catch
        {
            return false;
        }
    }
}