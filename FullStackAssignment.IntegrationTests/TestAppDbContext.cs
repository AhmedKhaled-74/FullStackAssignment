using FullStackAssignment.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

public class TestAppDbContext : AppDbContext
{
    private static int _counter = 1;

    public TestAppDbContext(DbContextOptions<TestAppDbContext> options)
        : base(options) { }

    public override Task<int> GetNextProductCodeAsync()
    {
        return Task.FromResult(_counter++);
    }
}
