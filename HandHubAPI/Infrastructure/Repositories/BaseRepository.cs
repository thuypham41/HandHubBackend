using HandHubAPI.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HandHubAPI.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly HandHubDbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(HandHubDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return entities;
    }

    public async Task Delete(int Id)
    {
        var entity = await GetByIdAsync(Id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is BaseEntity baseEntity && baseEntity.IsDeleted)
        {
            return null;
        }

        return entity;
    }

    public async Task<PaginatedResponse<T>> GetPaginatedAsync(int PageNumber, int PageSize)
    {
        var validPageNumber = Math.Max(1, PageNumber);
        var validPageSize = Math.Max(1, Math.Min(100, PageSize));

        var totalItems = await _dbSet.CountAsync();
        var items = await _dbSet
            .Skip((validPageNumber - 1) * validPageSize)
            .Take(validPageSize)
            .ToListAsync();

        return new PaginatedResponse<T>
        {
            Items = items,
            PageNumber = validPageNumber,
            PageSize = validPageSize,
            TotalItems = totalItems
        };
    }

    public async Task<bool> SoftDelete(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.IsDeleted = true;
            _dbSet.Update((T)(object)baseEntity);
            return true;
        }

        return false;
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}