using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data.UserRepo
{
    internal class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task CreateAsync(UserModel user, CancellationToken cancellationToken = default)
        {
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(UserModel user, CancellationToken cancellationToken = default)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<UserModel?> GetByNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Name == userName, cancellationToken);
        }

        public async Task<UserModel?> UpdateAsync(UserModel user, CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}
