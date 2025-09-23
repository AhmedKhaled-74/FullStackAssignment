using FullStackAssignment.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Infrastructure.Internal;

public class TestAppDbContext : AppDbContext
{
    private static int _counter = 1;

    public TestAppDbContext(DbContextOptions<TestAppDbContext> options)
        : base(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(options.Extensions
                .OfType<InMemoryOptionsExtension>()
                .First().StoreName) // reuse the in-memory DB name
            .Options)
    { }

    public override Task<int> GetNextProductCodeAsync()
    {
        return Task.FromResult(_counter++);
    }

    // (optional) helper so each test starts fresh
    public static void ResetCounter() => _counter = 1;
}
