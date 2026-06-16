using Discount.Grpc.Protos;
using static Discount.Grpc.Protos.DiscountProtoService;

namespace Basket.Application.GrpcService
{
	public class DiscountGrpcService
	{
		private readonly DiscountProtoServiceClient _discountProtoServiceClient;

		public DiscountGrpcService(DiscountProtoServiceClient discountProtoServiceClient)
		{
			_discountProtoServiceClient = discountProtoServiceClient;
		}

		public async Task<CouponModel> GetDiscount(string productName)
		{
			var request = new GetDiscountRequest { ProductName = productName };
			return await _discountProtoServiceClient.GetDiscountAsync(request);
		}
	}
}
