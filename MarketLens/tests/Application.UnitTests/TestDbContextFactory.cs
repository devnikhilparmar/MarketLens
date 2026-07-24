namespace CWM.CleanArchitecture.Application.UnitTests;

using CWM.CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
