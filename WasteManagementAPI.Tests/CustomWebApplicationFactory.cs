using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;
using WasteManagementAPI.Data;
using WasteManagementAPI.Models;

namespace WasteManagementAPI.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _dbName = System.Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Find and remove the existing SQL Server DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using InMemory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                // Build service provider and seed data for integration tests
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                SeedTestingData(db);
            });
        }

        private void SeedTestingData(ApplicationDbContext db)
        {
            // Add test users
            db.Users.AddRange(new List<User>
            {
                new User
                {
                    Id = 100,
                    Name = "Test Resident",
                    Email = "resident@test.com",
                    PasswordHash = HashPassword("password123"),
                    Role = "Resident",
                    RecyclingPoints = 0
                },
                new User
                {
                    Id = 101,
                    Name = "Test Collector",
                    Email = "collector@test.com",
                    PasswordHash = HashPassword("password123"),
                    Role = "Collector",
                    RecyclingPoints = 0
                },
                new User
                {
                    Id = 102,
                    Name = "Test Admin",
                    Email = "admin@test.com",
                    PasswordHash = HashPassword("password123"),
                    Role = "Admin",
                    RecyclingPoints = 0
                }
            });

            // Add test waste bins
            db.WasteBins.AddRange(new List<WasteBin>
            {
                new WasteBin
                {
                    Id = 10,
                    Location = "Lixeira Teste 1",
                    MaterialType = "Plastic",
                    MaxCapacityKg = 100,
                    CurrentFillLevelKg = 10,
                    IsFullAlertTriggered = false
                },
                new WasteBin
                {
                    Id = 11,
                    Location = "Lixeira Teste 2",
                    MaterialType = "Paper",
                    MaxCapacityKg = 50,
                    CurrentFillLevelKg = 48,
                    IsFullAlertTriggered = true
                }
            });

            // Add test educational tips
            db.EducationalTips.AddRange(new List<EducationalTip>
            {
                new EducationalTip
                {
                    Id = 10,
                    Title = "Dica Teste",
                    Content = "Conteúdo informativo de teste para reciclagem.",
                    MaterialCategory = "General"
                }
            });

            // Add test collection routes
            db.CollectionRoutes.AddRange(new List<CollectionRoute>
            {
                new CollectionRoute
                {
                    Id = 10,
                    RouteName = "Rota Teste Centro",
                    DriverName = "José Motorista",
                    ScheduledDate = DateTime.UtcNow.AddDays(1),
                    Status = "Pending"
                }
            });

            db.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
