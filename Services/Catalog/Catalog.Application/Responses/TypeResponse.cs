namespace Catalog.Application.Responses
{
	public record TypeResponse
	{
		public string Id { get; init; } = null!;
		public string Name { get; init; } = null!;
	}
}
