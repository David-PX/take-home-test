using Microsoft.EntityFrameworkCore;
using Fundo.Applications.WebApi.Domain.Entities;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<AppUser> Users => Set<AppUser>(); // para auth simple

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.FullName).IsRequired().HasMaxLength(200);
                b.Property(x => x.Email).HasMaxLength(200);

                b.HasMany(x => x.Loans)
                 .WithOne(x => x.Customer)
                 .HasForeignKey(x => x.CustomerId);
            });

            modelBuilder.Entity<Loan>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.OriginalAmount).HasColumnType("decimal(18,2)");
                b.Property(x => x.CurrentBalance).HasColumnType("decimal(18,2)");
                b.Property(x => x.Status).IsRequired();
                b.Property(x => x.CreatedAtUtc).IsRequired();
            });

            modelBuilder.Entity<AppUser>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Email).IsRequired().HasMaxLength(200);
                b.HasIndex(x => x.Email).IsUnique();
                b.Property(x => x.PasswordHash).IsRequired();
            });
        }
    }
}
