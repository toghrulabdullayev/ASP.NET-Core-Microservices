using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
	public class BrandRepository : IBrandRepository
	{
		private readonly IMongoCollection<ProductBrand> _brands;

		public BrandRepository(IOptions<DatabaseSettings> options)
		{
			// todo: use MongoClient via DI registered as a singleton instead of creating mongo client anew each time
			var settings = options.Value;
			var client = new MongoClient(settings.ConnectionString);
			var db = client.GetDatabase(settings.DatabaseName);
			_brands = db.GetCollection<ProductBrand>(settings.BrandCollectionName);
		}

		public async Task<IEnumerable<ProductBrand>> GetAllBrands()
		{
			// _ => true means "always match"
			return await _brands.Find(_ => true).ToListAsync(); // for C# mongodb driver
			// db.brands.find({}) for mongo shell, identical to the above statement
		}

		public async Task<ProductBrand> GetBrandAsync(string id)
		{
			return await _brands.Find(x => x.Id == id).FirstOrDefaultAsync();
		}
	}
}
