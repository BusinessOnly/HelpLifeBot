using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HelpLifeBot.Domain
{
    public interface IBaseRepository<TKey, T>
    {
        Task<T?> FindAsync(TKey id, CancellationToken cancellationToken);
        Task SaveAsync(T entity);
    }

    public abstract class BaseRepository<TKey, T> : IBaseRepository<TKey, T>
    {
        internal readonly AppDbContext DbContext = null!;

        public BaseRepository(AppDbContext context)
        {
            DbContext = context;
        }

        public abstract Task<T?> FindAsync(TKey id, CancellationToken cancellationToken);

        public virtual async Task SaveAsync(T entity)
        {
            Debug.Assert(entity != null);

            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                await DbContext.AddAsync(entity);
            }

            await DbContext.SaveChangesAsync();
        }
    }
}
