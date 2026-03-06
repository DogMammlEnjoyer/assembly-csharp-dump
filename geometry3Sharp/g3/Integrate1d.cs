using System;

namespace g3
{
	public static class Integrate1d
	{
		public static double RombergIntegral(int order, double a, double b, Func<double, object, double> function, object userData)
		{
			if (order <= 0)
			{
				throw new Exception("Integrate1d.RombergIntegral: Integration order must be positive\n");
			}
			double[,] array = new double[2, order];
			double num = b - a;
			array[0, 0] = 0.5 * num * (function(a, userData) + function(b, userData));
			int i = 2;
			int num2 = 1;
			while (i <= order)
			{
				double num3 = 0.0;
				for (int j = 1; j <= num2; j++)
				{
					num3 += function(a + num * ((double)j - 0.5), userData);
				}
				array[1, 0] = 0.5 * (array[0, 0] + num * num3);
				int k = 1;
				int num4 = 4;
				while (k < i)
				{
					array[1, k] = ((double)num4 * array[1, k - 1] - array[0, k - 1]) / (double)(num4 - 1);
					k++;
					num4 *= 4;
				}
				for (int j = 0; j < i; j++)
				{
					array[0, j] = array[1, j];
				}
				i++;
				num2 *= 2;
				num *= 0.5;
			}
			return array[0, order - 1];
		}

		public static double GaussianQuadrature(double a, double b, Func<double, object, double> function, object userData)
		{
			double num = 0.5 * (b - a);
			double num2 = 0.5 * (b + a);
			double num3 = 0.0;
			for (int i = 0; i < 5; i++)
			{
				num3 += Integrate1d.coeff[i] * function(num * Integrate1d.root[i] + num2, userData);
			}
			return num3 * num;
		}

		public static double TrapezoidRule(int numSamples, double a, double b, Func<double, object, double> function, object userData)
		{
			if (numSamples < 2)
			{
				throw new Exception("Integrate1d.TrapezoidRule: Must have more than two samples\n");
			}
			double num = (b - a) / (double)(numSamples - 1);
			double num2 = 0.5 * (function(a, userData) + function(b, userData));
			for (int i = 1; i <= numSamples - 2; i++)
			{
				num2 += function(a + (double)i * num, userData);
			}
			return num2 * num;
		}

		private static readonly double[] root = new double[]
		{
			-0.9061798459,
			-0.5384693101,
			0.0,
			0.5384693101,
			0.9061798459
		};

		private static readonly double[] coeff = new double[]
		{
			0.236926885,
			0.4786286705,
			0.5688888889,
			0.4786286705,
			0.236926885
		};
	}
}
