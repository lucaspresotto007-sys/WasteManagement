using Microsoft.EntityFrameworkCore;
using WasteManagementAPI.Models;

namespace WasteManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<WasteBin> WasteBins { get; set; }
        public DbSet<DisposalRecord> DisposalRecords { get; set; }
        public DbSet<CollectionRoute> CollectionRoutes { get; set; }
        public DbSet<EducationalTip> EducationalTips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<DisposalRecord>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DisposalRecord>()
                .HasOne(d => d.WasteBin)
                .WithMany()
                .HasForeignKey(d => d.WasteBinId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Bins
            modelBuilder.Entity<WasteBin>().HasData(
                new WasteBin { Id = 1, Location = "Setor Norte - Quadra 10", MaterialType = "Plastic", MaxCapacityKg = 100, CurrentFillLevelKg = 15, IsFullAlertTriggered = false },
                new WasteBin { Id = 2, Location = "Setor Norte - Quadra 10", MaterialType = "Paper", MaxCapacityKg = 80, CurrentFillLevelKg = 73, IsFullAlertTriggered = true }, // > 90% (73/80 = 91.25%)
                new WasteBin { Id = 3, Location = "Setor Sul - Centro Comercial", MaterialType = "Glass", MaxCapacityKg = 150, CurrentFillLevelKg = 30, IsFullAlertTriggered = false },
                new WasteBin { Id = 4, Location = "Avenida Principal - Km 2", MaterialType = "Metal", MaxCapacityKg = 120, CurrentFillLevelKg = 10, IsFullAlertTriggered = false }
            );

            // Seed Educational Tips
            modelBuilder.Entity<EducationalTip>().HasData(
                new EducationalTip 
                { 
                    Id = 1, 
                    Title = "Como descartar plástico corretamente", 
                    Content = "Lave as embalagens plásticas para retirar os resíduos de alimentos antes de descartá-las no coletor vermelho. Isso evita mau cheiro e atração de vetores, facilitando a reciclagem.", 
                    MaterialCategory = "Plastic" 
                },
                new EducationalTip 
                { 
                    Id = 2, 
                    Title = "Papel molhado não é reciclável!", 
                    Content = "Papéis sujos, engordurados (como caixas de pizza) ou molhados não devem ser colocados na lixeira azul de recicláveis, pois contaminam o lote. Descarte-os no lixo comum/orgânico.", 
                    MaterialCategory = "Paper" 
                },
                new EducationalTip 
                { 
                    Id = 3, 
                    Title = "Atenção com vidros quebrados", 
                    Content = "Sempre embale vidros quebrados em caixas de papelão ou garrafas pet cortadas e identifique o conteúdo para evitar acidentes com os profissionais da coleta seletiva.", 
                    MaterialCategory = "Glass" 
                }
            );
        }
    }
}
