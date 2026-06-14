using Basket.Application.Commands;
using Basket.Application.DTOs;
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

		public CreateShoppingCartHandler(IBasketRepository basketRepository)
		{
			_basketRepository = basketRepository;
		}

		public async Task<ShoppingCartResponse> Handle(
			CreateShoppingCartCommand request,
			CancellationToken cancellationToken
		)
		{
			// convert command to domain entity
			var shoppingCartEntity = request.ToEntity();

			// save to redis
			var updatedCart = await _basketRepository.UpdateBasket(shoppingCartEntity);

			// convert back to response
			return updatedCart!.ToResponse();
		}
	}
}
