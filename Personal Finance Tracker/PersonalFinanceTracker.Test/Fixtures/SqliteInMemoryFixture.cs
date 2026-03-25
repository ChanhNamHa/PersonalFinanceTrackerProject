using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class SqliteInMemoryFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    public DbContextOptions<ApplicationDbContext> Options { get; }

    public SqliteInMemoryFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = new ApplicationDbContext(Options);
        ctx.Database.EnsureCreated(); // hoặc Migrate() nếu bạn muốn chạy migrations
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}