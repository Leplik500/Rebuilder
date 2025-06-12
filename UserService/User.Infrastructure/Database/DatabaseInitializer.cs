using Microsoft.EntityFrameworkCore;

namespace User.Infrastructure.Database
{
    public class DatabaseInitializer
    {
        private readonly DbContext context;

        public DatabaseInitializer(DbContext context)
        {
            this.context = context;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            //TODO if you are not using migrations, then uncomment this line
            //await _context!.Database.EnsureCreatedAsync(cancellationToken);
            var pending = await context.Database.GetPendingMigrationsAsync(
                cancellationToken: cancellationToken
            );
            if (pending.Any())
            {
                await context!.Database.MigrateAsync(
                    cancellationToken: cancellationToken
                );
            }
        }

        public void Seed()
        {
            //TODO if you are not using migrations, then uncomment this line
            // _context!.Database.EnsureCreated();
            var pending = context.Database.GetPendingMigrations();
            if (pending.Any())
            {
                context!.Database.Migrate();
            }
        }
    }
}
