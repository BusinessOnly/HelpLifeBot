using Microsoft.EntityFrameworkCore;

namespace HelpLifeBot.Domain
{
    public interface IUserActionRepository : IBaseRepository<long, UserAction>
    {
    }

    public class UserActionRepository : BaseRepository<long, UserAction>, IUserActionRepository
    {
        public UserActionRepository(AppDbContext context) : base(context)
        {
        }

        public override Task<UserAction?> FindAsync(long id, CancellationToken cancellationToken)
            => DbContext.UserActions
                .Where(a => a.UserActionId == id)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
