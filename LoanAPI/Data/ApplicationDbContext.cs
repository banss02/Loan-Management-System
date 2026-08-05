using Microsoft.EntityFrameworkCore;
using LoanAPI.Models;

namespace LoanAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<LoanSchedule> LoanSchedules => Set<LoanSchedule>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Document> Documents => Set<Document>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().Property(c => c.Salary).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Loan>().Property(l => l.LoanAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Loan>().Property(l => l.InterestRate).HasColumnType("decimal(5,2)");
            modelBuilder.Entity<LoanSchedule>().Property(s => s.EMIAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<LoanSchedule>().Property(s => s.PrincipalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<LoanSchedule>().Property(s => s.InterestAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Customer>().HasIndex(c => c.Email).IsUnique();
            modelBuilder.Entity<Customer>().HasIndex(c => c.Phone).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithMany()
                .HasForeignKey(u => u.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Customer)
                .WithMany()
                .HasForeignKey(l => l.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoanSchedule>()
                .HasOne(s => s.Loan)
                .WithMany()
                .HasForeignKey(s => s.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Loan)
                .WithMany()
                .HasForeignKey(p => p.LoanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Schedule)
                .WithMany()
                .HasForeignKey(p => p.ScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Customer)
                .WithMany()
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
