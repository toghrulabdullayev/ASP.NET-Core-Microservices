using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.DTOs
{
	public record ProductDto(
		string Id,
		string Name,
		string Summary,
		string Description,
		string ImageFile,
		BrandDto Brand,
		TypeDto Type,
		decimal Price,
		DateTimeOffset CreatedDate
	);

	public record BrandDto(string Id, string Name);

	public record TypeDto(string Id, string Name);

	// init can only be set during object initialization, making the object immutable after creation.
	// This is useful for DTOs to ensure that their state cannot be changed after they have been created.
	public record CreateProductDto
	{
		public string Name { get; init; } = null!;
		public string Summary { get; init; } = null!;
		public string Description { get; init; } = null!;
		public string ImageFile { get; init; } = null!;
		public string BrandId { get; init; } = null!;
		public string TypeId { get; init; } = null!;

		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
		public decimal Price { get; init; }
	}

	public record UpdateProductDto
	{
		// equivalent of [Required] decorator for record types
		public string Name { get; init; } = null!;
		public string Summary { get; init; } = null!;
		public string Description { get; init; } = null!;
		public string ImageFile { get; init; } = null!;
		public string BrandId { get; init; } = null!;
		public string TypeId { get; init; } = null!;

		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
		public decimal Price { get; init; }
	}
}
