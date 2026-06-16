using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
	public class CreateShoppingCartHandler
		: IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
	{
		private readonly IBasketRepository _basketRepository;
		private readonly DiscountGrpcService _discountGrpcService;

		public CreateShoppingCartHandler(
			IBasketRepository basketRepository,
			DiscountGrpcService discountGrpcService
		)
		{
			_basketRepository = basketRepository;
			_discountGrpcService = discountGrpcService;
		}

		public async Task<ShoppingCartResponse> Handle(
			CreateShoppingCartCommand request,
			CancellationToken cancellationToken
		)
		{
			// apply discounts using GRPC call
			foreach (var item in request.Items)
			{
				var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
				item.Price -= coupon.Amount;
			}

			// convert command to domain entity
			var shoppingCartEntity = request.ToEntity();

			// save to redis
			var updatedCart = await _basketRepository.UpdateBasket(shoppingCartEntity);

			// convert back to response
			return updatedCart!.ToResponse();
		}
	}
}
