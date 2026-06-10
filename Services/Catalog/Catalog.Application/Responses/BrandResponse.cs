namespace Catalog.Application.Responses
{
	public record BrandResponse
	{
		public string Id { get; init; } = null!;
		public string Name { get; init; } = null!;
	}
}
