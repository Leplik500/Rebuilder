using Microsoft.EntityFrameworkCore;

namespace PublicWorkout.Infrastructure.Database
{
    public class DatabaseInitializer
    {
        private readonly DbContext _context;

        public DatabaseInitializer(DbContext context)
        {
            this._context = context;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            // TODO if you are not using migrations, then uncomment this line
            // await _context!.Database.EnsureCreatedAsync(cancellationToken);
            var pending = await this._context.Database.GetPendingMigrationsAsync(
                cancellationToken: cancellationToken
            );
            if (pending.Any())
            {
                await this._context!.Database.MigrateAsync(
                    cancellationToken: cancellationToken
                );
            }
        }

        public void Seed()
        {
            // TODO if you are not using migrations, then uncomment this line
            // _context!.Database.EnsureCreated();
            var pending = this._context.Database.GetPendingMigrations();
            if (pending.Any())
            {
                this._context!.Database.Migrate();
            }
        }
    }
}
