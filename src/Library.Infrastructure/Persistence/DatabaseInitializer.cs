using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IHost app, CancellationToken ct = default)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            await EnsureDatabaseReadyAsync(db, ct);
            await EnsureSchemaReadyAsync(db, ct);
            await DbSeeder.SeedAsync(db);
        }

        public static async Task EnsureDatabaseReadyAsync(LibraryDbContext db, CancellationToken ct = default)
        {
            const int maxAttempts = 60;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    await db.Database.EnsureCreatedAsync(ct);
                    return;
                }
                catch (Exception ex) when (attempt < maxAttempts && IsTransientPostgresStartupFailure(ex))
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
                }
            }
        }

        private static async Task EnsureSchemaReadyAsync(LibraryDbContext db, CancellationToken ct)
        {
            const string sql =
                "CREATE UNIQUE INDEX IF NOT EXISTS \"IX_Borrowings_Active_BookCopyId\" " +
                "ON \"Borrowings\" (\"BookCopyId\") " +
                "WHERE \"ReturnedAt\" IS NULL;";

            await db.Database.ExecuteSqlRawAsync(sql, ct);
        }
        private static bool IsTransientPostgresStartupFailure(Exception ex)
        {
            for (var e = ex; e != null; e = e.InnerException!)
            {
                if (e is NpgsqlException or EndOfStreamException or IOException)
                    return true;
                if (e is SocketException)
                    return true;
            }
            return false;
        }
    }
}
