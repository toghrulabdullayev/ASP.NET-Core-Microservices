//? idea: Provide a simple interface over a complex subsystem
using System;

namespace DesignPatterns
{
	class Facade
	{
		static void Main()
		{
			var theater = new HomeTheaterFacade(new Amplifier(), new Projector(), new Lights());
			theater.WatchMovie(); // instead of calling everything one by one
		}
	}

	public class HomeTheaterFacade
	{
		private readonly Amplifier _amp;
		private readonly Projector _projector;
		private readonly Lights _lights;

		public HomeTheaterFacade(Amplifier amp, Projector projector, Lights lights)
		{
			_amp = amp;
			_projector = projector;
			_lights = lights;
		}

		public void WatchMovie()
		{
			_lights.Dim();
			_projector.On();
			_amp.On();

			Console.WriteLine("Movie started 🎬");
		}
	}

	public class Amplifier
	{
		public void On() => Console.WriteLine("Amplifier ON");
	}

	public class Projector
	{
		public void On() => Console.WriteLine("Projector ON");
	}

	public class Lights
	{
		public void Dim() => Console.WriteLine("Lights dimmed");
	}
}
