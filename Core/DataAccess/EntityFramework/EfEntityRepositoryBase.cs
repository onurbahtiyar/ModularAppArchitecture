using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.DataAccess.EntityFramework;

public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
    where TEntity : class, IEntity, new()
    where TContext : DbContext
{
    private readonly TContext context;

    public EfEntityRepositoryBase(TContext _context)
    {
        context = _context;
    }

    public void Add(TEntity entitiy)
    {
        var addedEntity = context.Entry(entitiy);
        addedEntity.State = EntityState.Added;
        context.SaveChanges();
    }

    public void Delete(TEntity entitiy)
    {
        var deletedEntity = context.Entry(entitiy);
        deletedEntity.State = EntityState.Deleted;
        context.SaveChanges();
    }

    public TEntity Get(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = context.Set<TEntity>();

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return query.FirstOrDefault(filter);
    }

    public List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = context.Set<TEntity>();

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return filter == null ? query.ToList() : query.Where(filter).ToList();
    }

    public void Update(TEntity entitiy)
    {
        var updatedEntity = context.Entry(entitiy);
        updatedEntity.State = EntityState.Modified;
        context.SaveChanges();
    }

    public async Task AddAsync(TEntity entity)
    {
        var addedEntity = context.Entry(entity);
        addedEntity.State = EntityState.Added;
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        var updatedEntity = context.Entry(entity);
        updatedEntity.State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        var deletedEntity = context.Entry(entity);
        deletedEntity.State = EntityState.Deleted;
        await context.SaveChangesAsync();
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null)
    {
        return await context.Set<TEntity>().SingleOrDefaultAsync(filter);
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null)
    {
        return filter == null
            ? await context.Set<TEntity>().ToListAsync()
            : await context.Set<TEntity>().Where(filter).ToListAsync();
    }

    public TEntity GetNoTracking(Expression<Func<TEntity, bool>> filter = null)
    {
        return filter == null
            ? context.Set<TEntity>().AsNoTracking().FirstOrDefault()
            : context.Set<TEntity>().AsNoTracking().FirstOrDefault(filter);
    }

    public List<TEntity> GetListNoTracking(Expression<Func<TEntity, bool>> filter = null)
    {
        return filter == null
        ? context.Set<TEntity>().AsNoTracking().ToList()
        : context.Set<TEntity>().AsNoTracking().Where(filter).ToList();
    }

    public async Task<List<TEntity>> GetListNoTrackingAsync(Expression<Func<TEntity, bool>> filter = null)
    {
        return filter == null
            ? await context.Set<TEntity>().AsNoTracking().ToListAsync()
            : await context.Set<TEntity>().AsNoTracking().Where(filter).ToListAsync();
    }
}