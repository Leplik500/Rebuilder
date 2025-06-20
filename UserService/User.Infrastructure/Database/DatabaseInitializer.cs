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
            var pending = await this.context.Database.GetPendingMigrationsAsync(
                cancellationToken: cancellationToken
            );
            if (pending.Any())
            {
                await this.context!.Database.MigrateAsync(
                    cancellationToken: cancellationToken
                );
            }
        }

        public void Seed()
        {
            //TODO if you are not using migrations, then uncomment this line
            // _context!.Database.EnsureCreated();
            var pending = this.context.Database.GetPendingMigrations();
            if (pending.Any())
            {
                this.context!.Database.Migrate();
            }
        }
    }
}
