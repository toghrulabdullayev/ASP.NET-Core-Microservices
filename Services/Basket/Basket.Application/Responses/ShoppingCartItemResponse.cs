namespace Basket.Application.Responses
{
	public record ShoppingCartItemResponse
	{
		public int Quantity { get; init; }
		public string ImageFile { get; init; } = null!;
		public decimal Price { get; init; }
		public string ProductId { get; init; } = null!;
		public string ProductName { get; init; } = null!;
	}
}
