using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data
{
	public class OrderContextSeed
	{
		public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
		{
			if (!orderContext.Orders.Any())
			{
				orderContext.Orders.AddRange(GetOrders());
				await orderContext.SaveChangesAsync();
				logger.LogInformation($"Ordering Database: {typeof(OrderContext).Name} seeded");
			}
		}

		private static IEnumerable<Order> GetOrders()
		{
			return new List<Order>
			{
				new()
				{
					UserName = "Tog",
					FirstName = "Tog",
					LastName = "Tog",
					EmailAddress = "TogTog@ecommerce.net",
					AddressLine = "Tog Street",
					State = "Baku",
					Country = "Azerbaijan",
					ZipCode = "834009",

					CardName = "Visa",
					CardNumber = "4111111111111111",
					CreatedBy = "Tog",
					Expiration = "12/25",
					CVV = "123",
					PaymentMethod = 1,
					LastModifiedBy = "Tog",
					LastModifiedDate = DateTime.UtcNow,
				},
			};
		}
	}
}
