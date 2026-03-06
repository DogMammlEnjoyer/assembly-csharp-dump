using System;
using System.Collections.Generic;

namespace g3
{
	public class GaussPointsFit3
	{
		public GaussPointsFit3(IEnumerable<Vector3d> points)
		{
			this.Box = new Box3d(Vector3d.Zero, Vector3d.One);
			int num = 0;
			foreach (Vector3d v in points)
			{
				this.Box.Center = this.Box.Center + v;
				num++;
			}
			double num2 = 1.0 / (double)num;
			this.Box.Center = this.Box.Center * num2;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			foreach (Vector3d v2 in points)
			{
				Vector3d vector3d = v2 - this.Box.Center;
				num3 += vector3d[0] * vector3d[0];
				num4 += vector3d[0] * vector3d[1];
				num5 += vector3d[0] * vector3d[2];
				num6 += vector3d[1] * vector3d[1];
				num7 += vector3d[1] * vector3d[2];
				num8 += vector3d[2] * vector3d[2];
			}
			this.do_solve(num3, num4, num5, num6, num7, num8, num2);
		}

		public GaussPointsFit3(IEnumerable<Vector3d> points, IEnumerable<double> weights)
		{
			this.Box = new Box3d(Vector3d.Zero, Vector3d.One);
			int num = 0;
			double num2 = 0.0;
			IEnumerator<double> enumerator = weights.GetEnumerator();
			foreach (Vector3d v in points)
			{
				enumerator.MoveNext();
				double num3 = enumerator.Current;
				this.Box.Center = this.Box.Center + num3 * v;
				num2 += num3;
				num++;
			}
			double num4 = 1.0 / num2;
			this.Box.Center = this.Box.Center * num4;
			double num5 = 0.0;
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			double num10 = 0.0;
			enumerator = weights.GetEnumerator();
			foreach (Vector3d v2 in points)
			{
				enumerator.MoveNext();
				double num11 = enumerator.Current;
				num11 *= num11;
				Vector3d vector3d = v2 - this.Box.Center;
				num5 += num11 * vector3d[0] * vector3d[0];
				num6 += num11 * vector3d[0] * vector3d[1];
				num7 += num11 * vector3d[0] * vector3d[2];
				num8 += num11 * vector3d[1] * vector3d[1];
				num9 += num11 * vector3d[1] * vector3d[2];
				num10 += num11 * vector3d[2] * vector3d[2];
			}
			this.do_solve(num5, num6, num7, num8, num9, num10, num4 * num4);
		}

		private void do_solve(double sumXX, double sumXY, double sumXZ, double sumYY, double sumYZ, double sumZZ, double invSumMultiplier)
		{
			sumXX *= invSumMultiplier;
			sumXY *= invSumMultiplier;
			sumXZ *= invSumMultiplier;
			sumYY *= invSumMultiplier;
			sumYZ *= invSumMultiplier;
			sumZZ *= invSumMultiplier;
			double[] input = new double[]
			{
				sumXX,
				sumXY,
				sumXZ,
				sumXY,
				sumYY,
				sumYZ,
				sumXZ,
				sumYZ,
				sumZZ
			};
			SymmetricEigenSolver symmetricEigenSolver = new SymmetricEigenSolver(3, 4096);
			int num = symmetricEigenSolver.Solve(input, SymmetricEigenSolver.SortType.Increasing);
			this.ResultValid = (num > 0 && num < int.MaxValue);
			if (this.ResultValid)
			{
				this.Box.Extent = new Vector3d(symmetricEigenSolver.GetEigenvalues());
				double[] eigenvectors = symmetricEigenSolver.GetEigenvectors();
				this.Box.AxisX = new Vector3d(eigenvectors[0], eigenvectors[1], eigenvectors[2]);
				this.Box.AxisY = new Vector3d(eigenvectors[3], eigenvectors[4], eigenvectors[5]);
				this.Box.AxisZ = new Vector3d(eigenvectors[6], eigenvectors[7], eigenvectors[8]);
			}
		}

		public Box3d Box;

		public bool ResultValid;
	}
}
