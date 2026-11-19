using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Homeworks.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IQueryable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IQueryable<User>>(_dbContext.Users.AsNoTracking());
    }

    public async Task<User?> GetUserByGuidAsync(Guid guid, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == guid, cancellationToken);
    }

    public async Task DeleteUserByGuidAsync(Guid guid, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { guid }, cancellationToken);
        if (user is not null)
            _dbContext.Users.Remove(user);
    }

    public Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<Guid> InsertUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        return user.Id;
    }
}
