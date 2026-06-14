using Basket.Application.Commands;
using Basket.Application.Responses;
using Basket.Core.Entities;

namespace Basket.Application.Mappers
{
	public static class BasketMapper
	{
		public static ShoppingCartResponse ToResponse(this ShoppingCart shoppingCart)
		{
			return new ShoppingCartResponse
			{
				UserName = shoppingCart.UserName!,
				Items = shoppingCart
					.Items!.Select(item => new ShoppingCartItemResponse
					{
						Quantity = item.Quantity,
						ImageFile = item.ImageFile!,
						Price = item.Price,
						ProductId = item.ProductId!,
						ProductName = item.ProductName!,
					})
					.ToList(),
			};
		}

		// //? extension method using delegate-based mapper
		// public static ShoppingCartResponse ToResponse(this ShoppingCart cart) => MapCart(cart);

		// //? delegate-based mapper
		// public static readonly Func<ShoppingCart, ShoppingCartResponse> MapCart =
		// 	cart => new ShoppingCartResponse
		// 	{
		// 		UserName = cart.UserName!,
		// 		Items = cart.Items!.Select(item => new ShoppingCartItemResponse
		// 			{
		// 				Quantity = item.Quantity,
		// 				ImageFile = item.ImageFile!,
		// 				Price = item.Price,
		// 				ProductId = item.ProductId!,
		// 				ProductName = item.ProductName!,
		// 			})
		// 			.ToList(),
		// 	};

		public static ShoppingCart ToEntity(this CreateShoppingCartCommand command)
		{
			return new ShoppingCart
			{
				UserName = command.UserName,
				Items = command
					.Items.Select(item => new ShoppingCartItem
					{
						ProductId = item.ProductId,
						ProductName = item.ProductName,
						Price = item.Price,
						Quantity = item.Quantity,
						ImageFile = item.ImageFile,
					})
					.ToList(),
			};
		}
	}
}
