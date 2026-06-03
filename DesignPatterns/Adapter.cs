//? idea: Convert an incompatible interface into what your system expects.
using System;

namespace DesignPatterns
{
	class Adapter
	{
		static void Main()
		{
			// by assigning to IPaymentProcessor, you can plug any adapter here, e.g. new PayPalAdapter
			// otherwise, it is not strictly required
			IPaymentProcessor payment = new StripeAdapter(new StripeApi());
			payment.Pay(100m);
		}
	}

	public interface IPaymentProcessor
	{
		void Pay(decimal amount);
	}

	public class StripeApi
	{
		public void MakePayment(decimal value)
		{
			Console.WriteLine($"Stripe paid: {value}");
		}
	}

	public class StripeAdapter : IPaymentProcessor
	{
		private readonly StripeApi _stripe; // type StripeApi is required to make .MakePayment() available

		public StripeAdapter(StripeApi stripe)
		{
			_stripe = stripe;
		}

		public void Pay(decimal amount)
		{
			_stripe.MakePayment(amount);
		}
	}
}
