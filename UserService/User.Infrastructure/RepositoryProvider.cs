using Pepegov.UnitOfWork;
using Pepegov.UnitOfWork.EntityFramework;
using Pepegov.UnitOfWork.EntityFramework.Repository;
using Pepegov.UnitOfWork.Repository;

namespace User.Infrastructure;

public class RepositoryProvider : IRepositoryProvider
{
    private readonly IUnitOfWorkEntityFrameworkInstance unitOfWorkInstance;

    public RepositoryProvider(IUnitOfWorkManager unitOfWorkManager)
    {
        this.unitOfWorkInstance =
            unitOfWorkManager.GetInstance<IUnitOfWorkEntityFrameworkInstance>();
        this.unitOfWorkInstance.SetAutoDetectChanges(false);
    }

    public IRepositoryEntityFramework<TEntity> GetRepository<TEntity>()
        where TEntity : class
    {
        return this.unitOfWorkInstance.GetRepository<TEntity>();
    }
}
