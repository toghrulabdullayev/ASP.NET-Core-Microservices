using Catalog.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Application.Responses
{
	public record ProductResponse
	{
		public string Id { get; init; } = null!;
		public string Name { get; init; } = null!;
		public string Summary { get; init; } = null!;
		public string Description { get; init; } = null!;
		public string ImageFile { get; init; } = null!;
		public ProductBrand Brand { get; init; } = null!;
		public ProductType Type { get; init; } = null!;

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal Price { get; init; }
		public DateTimeOffset CreatedDate { get; init; }
	}
}
