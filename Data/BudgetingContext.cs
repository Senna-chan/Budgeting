using Budgeting.Models;
using Microsoft.EntityFrameworkCore;

namespace Budgeting.Data
{
    public class BudgetingContext : DbContext
    {
        public BudgetingContext(DbContextOptions<BudgetingContext> options)
            : base(options)
        {
        }


        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is CreateUpdateTime && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((CreateUpdateTime)entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((CreateUpdateTime)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }

        public DbSet<BudgetEntry> BudgetEntry { get; set; } = default!;

        public DbSet<BudgetList>? BudgetList { get; set; }
    }
}
