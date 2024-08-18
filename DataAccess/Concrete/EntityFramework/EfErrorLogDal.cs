using Core.DataAccess.EntityFramework;
using Entities.Concrete.EntityFramework.Context;
using Entities.Concrete.EntityFramework.Entities;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfErrorLogDal : EfEntityRepositoryBase<ErrorLog, ContextDb>, IErrorLogDal
    {
        public EfErrorLogDal(ContextDb _context) : base(_context)
        {
        }
    }
}