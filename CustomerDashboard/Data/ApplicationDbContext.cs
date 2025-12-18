using CustomerDashboard.Model;
using Microsoft.EntityFrameworkCore;

namespace CustomerDashboard.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            //// Definisi Index untuk FirstName
            //modelBuilder.Entity<Customers>()
            //    .HasIndex(c => c.FirstName.ToLower()) // 1. Buat index versi lowercase
            //    .HasDatabaseName("idx_customers_firstname_lower") // 2. Beri nama (opsional)
            //    .HasOperators("text_pattern_ops"); // 3. PENTING: Agar support 'search%' yang cepat

            //// Jika mau LastName juga:
            //modelBuilder.Entity<Customers>()
            //    .HasIndex(c => c.LastName.ToLower())
            //    .HasDatabaseName("idx_customers_lastname_lower")
            //    .HasOperators("text_pattern_ops");
        }
        public DbSet<Customers> Customers {  get; set; }
    }
}
