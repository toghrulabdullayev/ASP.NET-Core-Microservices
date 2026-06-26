using Ordering.Application.DTOs;
using Ordering.Application.Orders.CreateOrder;
using Ordering.Application.Orders.UpdateOrder;
using Ordering.Core.Entities;

namespace Ordering.Application.Mapper
{
	public static class OrderMapper
	{
		public static OrderDto ToDto(this Order order) =>
			new(
				order.Id,
				order.UserName!,
				order.TotalPrice, // no ?? needed, because TotalPrice is not nullable in Order entity
				order.FirstName!,
				order.LastName!,
				order.EmailAddress!,
				order.AddressLine!,
				order.Country!,
				order.State!,
				order.ZipCode!,
				order.CardName!,
				order.CardNumber!,
				order.Expiration!,
				order.CVV!,
				order.PaymentMethod // no ?? needed, because PaymentMethod is not nullable in Order entity
			);

		//! 'Order' does not contain a constructor that takes 14 arguments (no constructor at all)
		// public static Order ToEntity(this CreateOrderCommand command) =>
		// 	new(
		// 		command.UserName!,
		// 		command.TotalPrice, // no ?? needed, because TotalPrice is not nullable in CreateOrderCommand
		// 		command.FirstName!,
		// 		command.LastName!,
		// 		command.EmailAddress!,
		// 		command.AddressLine!,
		// 		command.Country!,
		// 		command.State!,
		// 		command.ZipCode!,
		// 		command.CardName!,
		// 		command.CardNumber!,
		// 		command.Expiration!,
		// 		command.CVV!,
		// 		command.PaymentMethod // no ?? needed, because PaymentMethod is not nullable in CreateOrderCommand
		// 	);

		public static Order ToEntity(this CreateOrderCommand command)
		{
			return new Order
			{
				UserName = command.UserName!,
				TotalPrice = command.TotalPrice,
				FirstName = command.FirstName!,
				LastName = command.LastName!,
				EmailAddress = command.EmailAddress!,
				AddressLine = command.AddressLine!,
				Country = command.Country!,
				State = command.State!,
				ZipCode = command.ZipCode!,
				CardName = command.CardName!,
				CardNumber = command.CardNumber!,
				Expiration = command.Expiration!,
				CVV = command.CVV!,
				PaymentMethod = command.PaymentMethod,
			};
		}

		public static void ApplyUpdate(this Order orderToUpdate, UpdateOrderCommand request)
		{
			orderToUpdate.UserName = request.UserName!;
			orderToUpdate.TotalPrice = request.TotalPrice;
			orderToUpdate.FirstName = request.FirstName!;
			orderToUpdate.LastName = request.LastName!;
			orderToUpdate.EmailAddress = request.EmailAddress!;
			orderToUpdate.AddressLine = request.AddressLine!;
			orderToUpdate.Country = request.Country!;
			orderToUpdate.State = request.State!;
			orderToUpdate.ZipCode = request.ZipCode!;
			orderToUpdate.CardName = request.CardName!;
			orderToUpdate.CardNumber = request.CardNumber!;
			orderToUpdate.Expiration = request.Expiration!;
			orderToUpdate.CVV = request.CVV!;
			orderToUpdate.PaymentMethod = request.PaymentMethod;
		}
	}
}
