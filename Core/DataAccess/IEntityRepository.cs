using Core.Entities;
using System.Linq.Expressions;

namespace Core.DataAccess;

public interface IEntityRepository<T> where T : class, IEntity, new()
{
    void Add(T entitiy);

    void Update(T entitiy);

    void Delete(T entitiy);

    List<T> GetList(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);

    T Get(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task<List<T>> GetListAsync(Expression<Func<T, bool>> filter = null);

    Task<T> GetAsync(Expression<Func<T, bool>> filter = null);

    List<T> GetListNoTracking(Expression<Func<T, bool>> filter = null);

    Task<List<T>> GetListNoTrackingAsync(Expression<Func<T, bool>> filter = null);

    T GetNoTracking(Expression<Func<T, bool>> filter = null);
}