using Microsoft.EntityFrameworkCore;
using Unison.LibraryManagement.Domain.Entities;

namespace Unison.LibraryManagement.Infrastructure.Persistence
{
    public class LibraryManagementDbContext : DbContext
    {
        public LibraryManagementDbContext(DbContextOptions<LibraryManagementDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<BookCopy> BookCopies { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;
        public DbSet<Fine> Fines { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RoleConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserRoleConfiguration());

            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "admin" },
                new Role { Id = 2, Name = "librarian" },
                new Role { Id = 3, Name = "member" }
            );

            // Basic indexes and relationships for library entities
            modelBuilder.Entity<Book>(b =>
            {
                b.HasIndex(x => x.ISBN);
                b.Property(x => x.Title).HasMaxLength(512);
            });

            modelBuilder.Entity<BookCopy>(c =>
            {
                c.HasOne(x => x.Book).WithMany(b => b.Copies).HasForeignKey(x => x.BookId);
                c.Property(x => x.CopyNumber).HasMaxLength(128);
            });

            modelBuilder.Entity<Loan>(l =>
            {
                l.HasOne(x => x.BookCopy).WithMany().HasForeignKey(x => x.BookCopyId);
            });

            modelBuilder.Entity<Fine>(f =>
            {
                f.HasOne<Loan>().WithMany().HasForeignKey(x => x.LoanId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
