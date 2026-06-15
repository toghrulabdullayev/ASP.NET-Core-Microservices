using Discount.Application.DTOs;
using MediatR;

namespace Discount.Application.Commands
{
	public record CreateDiscountCommand(string ProductName, string Description, decimal Amount)
		: IRequest<CouponDto>;
}
