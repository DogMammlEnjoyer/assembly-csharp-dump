using System;
using System.Collections.Generic;

namespace g3
{
	public static class FastPointWinding
	{
		public static void ComputeCoeffs(IPointSet pointSet, IEnumerable<int> points, double[] pointAreas, ref Vector3d p, ref double r, ref Vector3d order1, ref Matrix3d order2)
		{
			if (!pointSet.HasVertexNormals)
			{
				throw new Exception("FastPointWinding.ComputeCoeffs: point set does not have normals!");
			}
			p = Vector3d.Zero;
			order1 = Vector3d.Zero;
			order2 = Matrix3d.Zero;
			r = 0.0;
			double num = 0.0;
			foreach (int num2 in points)
			{
				num += pointAreas[num2];
				p += pointAreas[num2] * pointSet.GetVertex(num2);
			}
			p /= num;
			foreach (int num3 in points)
			{
				Vector3d vertex = pointSet.GetVertex(num3);
				Vector3d v = pointSet.GetVertexNormal(num3);
				double f = pointAreas[num3];
				order1 += f * v;
				Vector3d vector3d = vertex - p;
				order2 += f * new Matrix3d(ref vector3d, ref v);
				r = Math.Max(r, vertex.Distance(p));
			}
		}

		public static double EvaluateOrder1Approx(ref Vector3d center, ref Vector3d order1Coeff, ref Vector3d q)
		{
			Vector3d v = center - q;
			double length = v.Length;
			return 0.07957747154594767 * order1Coeff.Dot(v / (length * length * length));
		}

		public static double EvaluateOrder2Approx(ref Vector3d center, ref Vector3d order1Coeff, ref Matrix3d order2Coeff, ref Vector3d q)
		{
			Vector3d vector3d = center - q;
			double length = vector3d.Length;
			double num = length * length * length;
			double num2 = 1.0 / (12.566370614359172 * num);
			double num3 = num2 * order1Coeff.Dot(ref vector3d);
			double num4 = -3.0 / (12.566370614359172 * num * length * length);
			Matrix3d matrix3d = new Matrix3d(num2 + num4 * vector3d.x * vector3d.x, num4 * vector3d.x * vector3d.y, num4 * vector3d.x * vector3d.z, num4 * vector3d.y * vector3d.x, num2 + num4 * vector3d.y * vector3d.y, num4 * vector3d.y * vector3d.z, num4 * vector3d.z * vector3d.x, num4 * vector3d.z * vector3d.y, num2 + num4 * vector3d.z * vector3d.z);
			double num5 = order2Coeff.InnerProduct(ref matrix3d);
			return num3 + num5;
		}

		public static double ExactEval(ref Vector3d x, ref Vector3d xn, double xA, ref Vector3d q)
		{
			Vector3d v = x - q;
			double length = v.Length;
			return xA / 12.566370614359172 * xn.Dot(v / (length * length * length));
		}

		public static double Order1Approx(ref Vector3d x, ref Vector3d p, ref Vector3d xn, double xA, ref Vector3d q)
		{
			Vector3d v = p - q;
			double length = v.Length;
			return xA / 12.566370614359172 * xn.Dot(v / (length * length * length));
		}

		public static double Order2Approx(ref Vector3d x, ref Vector3d p, ref Vector3d xn, double xA, ref Vector3d q)
		{
			Vector3d v = p - q;
			Vector3d vector3d = x - p;
			double length = v.Length;
			double num = length * length * length;
			double num2 = xA / 12.566370614359172 * xn.Dot(v / num);
			Matrix3d matrix3d = new Matrix3d(ref v, ref v);
			matrix3d *= 3.0 / (12.566370614359172 * num * length * length);
			double num3 = 1.0 / (12.566370614359172 * num);
			Matrix3d matrix3d2 = new Matrix3d(num3, num3, num3) - matrix3d;
			double num4 = xA * new Matrix3d(ref vector3d, ref xn).InnerProduct(ref matrix3d2);
			return num2 + num4;
		}
	}
}
