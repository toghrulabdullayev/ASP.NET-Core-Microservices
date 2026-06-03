//? idea: Separate abstraction and implementation so both can evolve independently
using System;

namespace DesignPatterns
{
	class Bridge
	{
		static void Main()
		{
			IRenderer vector = new VectorRenderer();
			IRenderer raster = new RasterRenderer();

			Shape circle1 = new Circle(vector, 5);
			Shape circle2 = new Circle(raster, 10);

			circle1.Draw();
			circle2.Draw();
		}
	}

	public class Circle : Shape
	{
		private readonly double _radius;

		public Circle(IRenderer renderer, double radius)
			: base(renderer)
		{
			_radius = radius;
		}

		public override void Draw()
		{
			renderer.RenderCircle(_radius);
		}
	}

	public abstract class Shape
	{
		protected IRenderer renderer;

		protected Shape(IRenderer renderer)
		{
			this.renderer = renderer;
		}

		public abstract void Draw();
	}

	public interface IRenderer
	{
		void RenderCircle(double radius);
	}

	public class VectorRenderer : IRenderer
	{
		public void RenderCircle(double radius)
		{
			Console.WriteLine($"Vector circle with radius {radius}");
		}
	}

	public class RasterRenderer : IRenderer
	{
		public void RenderCircle(double radius)
		{
			Console.WriteLine($"Raster circle with radius {radius}");
		}
	}
}
