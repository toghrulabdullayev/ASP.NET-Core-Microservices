using Grpc.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Discount.Infrastructure.Settings
{
	public static class DbExtension
	{
		// extend app's IHost to apply database migration on startup in Program.cs with app.MigrateDatabase()
		public static IHost MigrateDatabase(this IHost host)
		{
			using var scope = host.Services.CreateScope();
			var services = scope.ServiceProvider;
			var logger = services.GetRequiredService<ILogger>();
			var databaseSettings = services.GetRequiredService<IOptions<DatabaseSettings>>().Value;

			try
			{
				logger.Info("Discount Db migration started.");
				ApplyMigration(databaseSettings.ConnectionString);
				logger.Info("Discount Db migration completed.");
			}
			catch (Exception ex)
			{
				logger.Error(ex, "An error occurred while migrating the Discount database.");
				throw;
			}

			return host;
		}

		private static void ApplyMigration(string connectionString)
		{
			var retry = 5;
			while (retry > 0)
			{
				try
				{
					using var connection = new NpgsqlConnection(connectionString);
					connection.Open();

					using var cmd = new NpgsqlCommand { Connection = connection };
					cmd.CommandText = "DROP TABLE IF EXISTS Coupon";
					cmd.ExecuteNonQuery();

					cmd.CommandText =
						@"
            CREATE TABLE Coupon(
              Id SERIAL PRIMARY KEY,
              ProductName VARCHAR(500) NOT NULL,
              Description TEXT,
              Amount INT
            )";
					cmd.ExecuteNonQuery();

					cmd.CommandText =
						@"
            INSERT INTO Coupon(ProductName, Description, Amount)
            VALUES('Adidas Quick Force Indoor Badminton Shoes', 'Shoe Discount', 500)";
					cmd.ExecuteNonQuery();

					cmd.CommandText =
						@"
            INSERT INTO Coupon(ProductName, Description, Amount)
            VALUES('Yonex VCORE Pro 100 A Tennis Racquet (270gm, Strung)', 'Racquet Discount', 700)";
					cmd.ExecuteNonQuery();
					//success -> exit retry loop
					break;
				}
				catch
				{
					retry--;
					if (retry == 0)
					{
						throw;
					}
				}
			}
		}
	}
}
