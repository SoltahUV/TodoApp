using DataAccess.Data.UserRepo;
using DataAccess.Models;

namespace XUnit_Tests.Services;

internal sealed class FakeUserRepository : IUserRepository
{
    public List<UserModel> Users { get; } = [];
    public int CreateCalls { get; private set; }

    public Task CreateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        CreateCalls++;
        Users.Add(user);
        return Task.CompletedTask;
    }

    public Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Users.SingleOrDefault(x => x.Id == userId));

    public Task<UserModel?> GetByNameAsync(string userName, CancellationToken cancellationToken = default) =>
        Task.FromResult(Users.SingleOrDefault(x => x.Name == userName));

    public Task<UserModel?> UpdateAsync(UserModel user, CancellationToken cancellationToken = default) =>
        Task.FromResult<UserModel?>(user);

    public Task DeleteAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        Users.Remove(user);
        return Task.CompletedTask;
    }
}
