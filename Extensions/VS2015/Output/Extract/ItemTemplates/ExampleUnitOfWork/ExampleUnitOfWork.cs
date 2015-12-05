using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using TrackableEntities.Patterns.EF6;

namespace $rootnamespace$
{
    // Implements I$safeitemname$ in the Persistence project
    public class $safeitemname$ : UnitOfWork, I$safeitemname$
    {
        // TODO: Add read-only fields for each entity repository interface
        //private readonly IEntityRepository _entityRepository;

		// TODO: Rename IDatabaseContext to match context interface
		// TODO: Add parameters for each repository interface
        public $safeitemname$(IDatabaseContext context
            /* , IEntityRepository entityRepository */) :
            base(context as DbContext)
        {
		    // TODO: Initizlialize each entity repository field
            //_entityRepository = entityRepository;
        }

        // TODO: Add read-only property for each entity repository interface
        //public IEntityRepository EntityRepository
        //{
        //    get { return _entityRepository; }
        //}

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                throw new UpdateConcurrencyException(concurrencyException.Message,
                    concurrencyException);
            }
            catch (DbUpdateException updateException)
            {
                throw new UpdateException(updateException.Message,
                    updateException);
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(CancellationToken.None);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                throw new UpdateConcurrencyException(concurrencyException.Message,
                    concurrencyException);
            }
            catch (DbUpdateException updateException)
            {
                throw new UpdateException(updateException.Message, 
                    updateException);
            }
        }
    }
}
