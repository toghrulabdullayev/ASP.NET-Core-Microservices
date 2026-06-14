using Basket.Application.Commands;
using Basket.Application.DTOs;
using Basket.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controller
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class BasketController : ControllerBase
	{
		private readonly IMediator _mediator;

		public BasketController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{userName}")] // api/v1/basket/{userName}
		public async Task<ActionResult<ShoppingCartDto>> GetBasket(string userName)
		{
			var query = new GetBasketByUserNameQuery(userName);
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpPost] // api/v1/basket
		public async Task<ActionResult<ShoppingCartDto>> CreateOrUpdateBasket(
			[FromBody] CreateShoppingCartCommand command
		)
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		[HttpDelete("{userName}")] // api/v1/basket/{userName}
		public async Task<IActionResult> DeleteBasket(string userName)
		{
			var command = new DeleteBasketByUserNameCommand(userName);
			await _mediator.Send(command);
			return NoContent();
		}
	}
}
