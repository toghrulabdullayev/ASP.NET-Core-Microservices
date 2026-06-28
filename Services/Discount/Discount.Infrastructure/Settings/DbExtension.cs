using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
			var loggerFactory = services.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger("DiscountDbMigration");
			var databaseSettings = services.GetRequiredService<IOptions<DatabaseSettings>>().Value;

			try
			{
				logger.LogInformation("Discount Db migration started.");
				ApplyMigration(databaseSettings.ConnectionString);
				logger.LogInformation("Discount Db migration completed.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while migrating the Discount database.");
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
					//? using var connection ... does something similar to defer connection.Close() in Go
					using var connection = new NpgsqlConnection(connectionString);
					connection.Open();

					//? ADO.NET style db migration, dapper isn't used here (NpgsqlCommand)
					using var cmd = new NpgsqlCommand { Connection = connection };
					cmd.CommandText = "DROP TABLE IF EXISTS Coupon";
					cmd.ExecuteNonQuery();

					//? @" " is a verbatim string literal in C#, allows multiline strings and ignores escape sequences
					//? """ """ (raw string literal, C# 11, can be more than 3 pairs) unlike verbatim,
					//? allows for interpolation and escape sequences
					/*
						var name = "Toghrul";

						var s = $$"""
						{name}      // literal text
						{{name}}    // interpolation
						""";

						Output:
						{name}
						Toghrul
					*/
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
