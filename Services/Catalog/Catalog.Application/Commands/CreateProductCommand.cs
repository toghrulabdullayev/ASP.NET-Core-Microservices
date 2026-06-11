using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Commands
{
	public record CreateProductCommand : IRequest<ProductResponse>
	{
		public string Name { get; init; } = null!;
		public string Summary { get; init; } = null!;
		public string Description { get; init; } = null!;
		public string ImageFile { get; init; } = null!;
		public string BrandId { get; init; } = null!;
		public string TypeId { get; init; } = null!;
		public decimal Price { get; init; }
	}
}
