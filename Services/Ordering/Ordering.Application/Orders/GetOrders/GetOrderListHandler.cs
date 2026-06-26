using Ordering.Application.Abstractions;
using Ordering.Application.DTOs;
using Ordering.Application.Mapper;
using Ordering.Core.Repositories;

namespace Ordering.Application.Orders.GetOrders
{
	public class GetOrderListHandler : IQueryHandler<GetOrderListQuery, List<OrderDto>>
	{
		private readonly IOrderRepository _orderRepository;

		public GetOrderListHandler(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}

		public async Task<List<OrderDto>> Handle(
			GetOrderListQuery query,
			CancellationToken cancellationToken
		)
		{
			var orders = await _orderRepository.GetOrdersByUserName(query.UserName);
			return orders.Select(o => o.ToDto()).ToList();
		}
	}
}
