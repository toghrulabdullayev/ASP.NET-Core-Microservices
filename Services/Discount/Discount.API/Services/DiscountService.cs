using Discount.Application.Commands;
using Discount.Application.Mappers;
using Discount.Application.Queries;
using Discount.Grpc.Protos;
using Grpc.Core;
using MediatR;
using static Discount.Grpc.Protos.DiscountProtoService; // static class import syntax

namespace Discount.API.Services
{
	public class DiscountService : DiscountProtoServiceBase
	{
		private readonly IMediator _mediator;

		public DiscountService(IMediator mediator)
		{
			_mediator = mediator;
		}

		public override async Task<CouponModel> GetDiscount(
			GetDiscountRequest request,
			ServerCallContext context
		)
		{
			var query = new GetDiscountQuery(request.ProductName);
			var dto = await _mediator.Send(query);
			return dto.ToModel();
		}

		public override async Task<CouponModel> CreateDiscount(
			CreateDiscountRequest request,
			ServerCallContext context
		)
		{
			var cmd = request.Coupon.ToCreateCommand();
			var dto = await _mediator.Send(cmd);
			return dto.ToModel();
		}

		public override async Task<CouponModel> UpdateDiscount(
			UpdateDiscountRequest request,
			ServerCallContext context
		)
		{
			var cmd = request.Coupon.ToUpdateCommand();
			var dto = await _mediator.Send(cmd);
			return dto.ToModel();
		}

		public override async Task<DeleteDiscountResponse> DeleteDiscount(
			DeleteDiscountRequest request,
			ServerCallContext context
		)
		{
			var cmd = new DeleteDiscountCommand(request.ProductName);
			var success = await _mediator.Send(cmd);
			return new DeleteDiscountResponse { Success = success };
		}
	}
}
