using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete.EntityFramework.Context;
using Entities.Concrete.EntityFramework.Entities;

public class EfUserActivityLogDal : EfEntityRepositoryBase<UserActivityLog, ContextDb>, IUserActivityLogDal
{
    public EfUserActivityLogDal(ContextDb _context) : base(_context)
    {
    }
}