using System;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class PolygonGenerator
	{
		public static Polygon RandomCircleSweep(double scale, int vertexCount)
		{
			double num = scale / 4.0;
			PolygonPoint[] array = new PolygonPoint[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				do
				{
					if (i % 250 == 0)
					{
						num += scale / 2.0 * (0.5 - PolygonGenerator.RNG.NextDouble());
					}
					else if (i % 50 == 0)
					{
						num += scale / 5.0 * (0.5 - PolygonGenerator.RNG.NextDouble());
					}
					else
					{
						num += 25.0 * scale / (double)vertexCount * (0.5 - PolygonGenerator.RNG.NextDouble());
					}
					num = ((num > scale / 2.0) ? (scale / 2.0) : num);
					num = ((num < scale / 10.0) ? (scale / 10.0) : num);
				}
				while (num < scale / 10.0 || num > scale / 2.0);
				PolygonPoint polygonPoint = new PolygonPoint(num * Math.Cos(PolygonGenerator.PI_2 * (double)i / (double)vertexCount), num * Math.Sin(PolygonGenerator.PI_2 * (double)i / (double)vertexCount), i);
				array[i] = polygonPoint;
			}
			return new Polygon(array);
		}

		public static Polygon RandomCircleSweep2(double scale, int vertexCount)
		{
			double num = scale / 4.0;
			PolygonPoint[] array = new PolygonPoint[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				do
				{
					num += scale / 5.0 * (0.5 - PolygonGenerator.RNG.NextDouble());
					num = ((num > scale / 2.0) ? (scale / 2.0) : num);
					num = ((num < scale / 10.0) ? (scale / 10.0) : num);
				}
				while (num < scale / 10.0 || num > scale / 2.0);
				PolygonPoint polygonPoint = new PolygonPoint(num * Math.Cos(PolygonGenerator.PI_2 * (double)i / (double)vertexCount), num * Math.Sin(PolygonGenerator.PI_2 * (double)i / (double)vertexCount), i);
				array[i] = polygonPoint;
			}
			return new Polygon(array);
		}

		private static readonly Random RNG = new Random();

		private static double PI_2 = 6.283185307179586;
	}
}
