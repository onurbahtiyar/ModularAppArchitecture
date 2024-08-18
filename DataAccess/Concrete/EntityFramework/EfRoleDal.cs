
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete.EntityFramework.Context;
using Entities.Concrete.EntityFramework.Entities;

namespace DataAccess.Concrete.EntityFramework;
public class EfRoleDal: EfEntityRepositoryBase<Role, ContextDb>, IRoleDal
{
    public EfRoleDal(ContextDb context) : base(context)
    {
    }
}
