namespace Basket.Application.Responses
{
	public record ShoppingCartResponse
	{
		public string UserName { get; init; }
		public List<ShoppingCartItemResponse> Items { get; init; }

		public ShoppingCartResponse()
			: this(string.Empty, new List<ShoppingCartItemResponse>()) { }

		public ShoppingCartResponse(string userName)
			: this(userName, new List<ShoppingCartItemResponse>()) { }

		public ShoppingCartResponse(string userName, List<ShoppingCartItemResponse> items)
		{
			UserName = userName ?? string.Empty;
			Items = items ?? new List<ShoppingCartItemResponse>();
		}

		// a computed property
		public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
		/* equivalent of:
			public decimal TotalPrice
			{
				get
				{
					return Items.Sum(item => item.Price * item.Quantity);
				}
			}
		*/
	}
}
