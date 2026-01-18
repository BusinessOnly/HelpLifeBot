using Microsoft.EntityFrameworkCore;

namespace HelpLifeBot.Domain
{
    public interface IUserRepository : IBaseRepository<long, User>
    {
    }

    public class UserRepository : BaseRepository<long, User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public override Task<User?> FindAsync(long id, CancellationToken cancellationToken)
            => DbContext.Users
                .Where(a => a.UserId == id)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
