using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Entities.File> Files { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetGoal> BudgetGoals { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relação: Um usuário (User) pode ter vários perfis (Profile)
            modelBuilder.Entity<Profile>()
                .HasOne(p => p.User)
                .WithMany(u => u.Profiles)
                .HasForeignKey(p => p.UserId); 

            // Relação: Wallets, Labels, Budgets e Files são associados a um único Profile
            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.Profile)
                .WithMany(p => p.Wallets)
                .HasForeignKey(w => w.ProfileId);

            modelBuilder.Entity<Label>()
                .HasOne(l => l.Profile)
                .WithMany(p => p.Labels)
                .HasForeignKey(l => l.ProfileId);

            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Profile)
                .WithMany(p => p.Budgets)
                .HasForeignKey(b => b.ProfileId);

            modelBuilder.Entity<Entities.File>()
                .HasOne(f => f.Profile)
                .WithMany(p => p.Files)
                .HasForeignKey(f => f.ProfileId);

            // Relação: Transactions são associadas a uma única Wallet
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Wallet)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.WalletId);

            // Relação: Transactions são associadas a um único File
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.File)
                .WithMany(f => f.Transactions)
                .HasForeignKey(t => t.FileId);

            // Configuração do relacionamento muitos-para-muitos entre Transaction e Label
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Label)
                .WithMany(f => f.Transactions)
                .HasForeignKey(t => t.LabelId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Notification)
                .WithOne(n => n.Transaction)
                .HasForeignKey<Notification>(n => n.TransactionId);

            // Relação: BudgetGoals são associadas a um único Budget
            modelBuilder.Entity<BudgetGoal>()
                .HasOne(bg => bg.Budget)
                .WithMany(b => b.BudgetGoals)
                .HasForeignKey(bg => bg.BudgetId);

            // Relação: BudgetGoals são associadas a um único Label
            modelBuilder.Entity<BudgetGoal>()
                .HasOne(bg => bg.Label)
                .WithMany(l => l.BudgetGoals)
                .HasForeignKey(bg => bg.LabelId);

            // Index único UserId + Título
            modelBuilder.Entity<Profile>()
                    .HasIndex(p => new { p.UserId, p.Title })
                    .IsUnique();

            // Index único UserId + Título
            modelBuilder.Entity<Wallet>()
                    .HasIndex(w => new { w.Month, w.Year, w.ProfileId })
                    .IsUnique();

            modelBuilder.Entity<User>()
                    .HasIndex(w => new { w.Email })
                    .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }
}