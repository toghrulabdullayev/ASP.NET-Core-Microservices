namespace Basket.Application.DTOs
{
	public record ShoppingCartDto(
		string UserName,
		List<ShoppingCartItemDto> Items,
		decimal TotalPrice
	);

	public record ShoppingCartItemDto(
		string ProductId,
		string ProductName,
		string ImageFile,
		decimal Price,
		int Quantity
	);

	public record CreateShoppingCartItemDto
	{
		public string ProductId { get; set; } = string.Empty;
		public string ProductName { get; set; } = string.Empty;
		public string ImageFile { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}
}
