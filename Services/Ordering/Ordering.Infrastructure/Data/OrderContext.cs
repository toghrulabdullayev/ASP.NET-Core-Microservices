using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data
{
	public class OrderContext : DbContext
	{
		public OrderContext(DbContextOptions<OrderContext> options)
			: base(options) { }

		public DbSet<Order> Orders { get; set; }

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			// ChangeTracker: EF Core component that tracks entity changes and determines what SQL
			// (INSERT/UPDATE/DELETE) to generate on SaveChanges.
			foreach (var entry in ChangeTracker.Entries<EntityBase>())
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entry.Entity.CreatedDate = DateTime.UtcNow;
						entry.Entity.CreatedBy = "Toghrul"; // TODO: replace with user
						break;
					case EntityState.Modified:
						entry.Entity.LastModifiedDate = DateTime.UtcNow;
						entry.Entity.LastModifiedBy = "Toghrul"; // TODO: replace with user
						break;
				}
			}

			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
