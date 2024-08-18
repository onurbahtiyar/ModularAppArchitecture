
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete.EntityFramework.Context;
using Entities.Concrete.EntityFramework.Entities;

namespace DataAccess.Concrete.EntityFramework;
public class EfUserRoleDal : EfEntityRepositoryBase<UserRole, ContextDb>, IUserRoleDal
{
    public EfUserRoleDal(ContextDb context) : base(context)
    {
    }
}
