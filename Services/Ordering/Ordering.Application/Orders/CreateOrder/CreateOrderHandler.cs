using Ordering.Application.Abstractions;
using Ordering.Application.Mapper;
using Ordering.Core.Repositories;

namespace Ordering.Application.Orders.CreateOrder
{
	public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, int>
	{
		private readonly IOrderRepository _orderRepository;

		public CreateOrderHandler(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}

		public async Task<int> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
		{
			var orderEntity = command.ToEntity();
			var generatedOrder = await _orderRepository.AddAsync(orderEntity);
			return generatedOrder.Id;
		}
	}
}
