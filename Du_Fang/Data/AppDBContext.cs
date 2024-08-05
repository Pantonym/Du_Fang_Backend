using Microsoft.EntityFrameworkCore;

namespace Du_Fang
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<User_Security> UserSecurities { get; set; }
        public DbSet<Authentication_Log> AuthenticationLogs { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the one-to-one relationship between User and User_Security
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserSecurity)
                .WithOne(us => us.User)
                .HasForeignKey<User_Security>(us => us.UserId);

            // Define the one-to-many relationship between User and Authentication_Log
            modelBuilder.Entity<User>()
                .HasMany(u => u.AuthenticationLogs)
                .WithOne(al => al.User)
                .HasForeignKey(al => al.UserId);

            // Define the one-to-one relationship between User and Account
            modelBuilder.Entity<User>()
                .HasOne(u => u.Account)
                .WithOne(a => a.User)
                .HasForeignKey<Account>(a => a.UserId);

            // Define the one-to-many relationship between Status and Account
            modelBuilder.Entity<Status>()
                .HasMany(s => s.Accounts)
                .WithOne(a => a.Status)
                .HasForeignKey(a => a.AccountStatusId);

            // Define the one-to-many relationship between Account and Transaction (FromAccount)
            modelBuilder.Entity<Account>()
                .HasMany(a => a.TransactionsFrom)
                .WithOne(t => t.FromAccount)
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Define the one-to-many relationship between Account and Transaction (ToAccount)
            modelBuilder.Entity<Account>()
                .HasMany(a => a.TransactionsTo)
                .WithOne(t => t.ToAccount)
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}