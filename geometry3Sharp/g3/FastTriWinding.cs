using System;
using System.Collections.Generic;

namespace g3
{
	public static class FastTriWinding
	{
		public static void ComputeCoeffs(DMesh3 mesh, IEnumerable<int> triangles, ref Vector3d p, ref double r, ref Vector3d order1, ref Matrix3d order2, MeshTriInfoCache triCache = null)
		{
			p = Vector3d.Zero;
			order1 = Vector3d.Zero;
			order2 = Matrix3d.Zero;
			r = 0.0;
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			double num = 0.0;
			foreach (int num2 in triangles)
			{
				if (triCache != null)
				{
					double num3 = triCache.Areas[num2];
					num += num3;
					p += num3 * triCache.Centroids[num2];
				}
				else
				{
					mesh.GetTriVertices(num2, ref zero, ref zero2, ref zero3);
					double num4 = MathUtil.Area(ref zero, ref zero2, ref zero3);
					num += num4;
					p += num4 * ((zero + zero2 + zero3) / 3.0);
				}
			}
			p /= num;
			Vector3d v = Vector3d.Zero;
			Vector3d v2 = Vector3d.Zero;
			double f = 0.0;
			foreach (int num5 in triangles)
			{
				mesh.GetTriVertices(num5, ref zero, ref zero2, ref zero3);
				if (triCache == null)
				{
					v2 = 0.3333333333333333 * (zero + zero2 + zero3);
					v = MathUtil.FastNormalArea(ref zero, ref zero2, ref zero3, out f);
				}
				else
				{
					triCache.GetTriInfo(num5, ref v, ref f, ref v2);
				}
				order1 += f * v;
				Vector3d vector3d = v2 - p;
				order2 += f * new Matrix3d(ref vector3d, ref v);
				double d = MathUtil.Max(zero.DistanceSquared(ref p), zero2.DistanceSquared(ref p), zero3.DistanceSquared(ref p));
				r = Math.Max(r, Math.Sqrt(d));
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

		public static double Order1Approx(ref Triangle3d t, ref Vector3d p, ref Vector3d xn, ref double xA, ref Vector3d q)
		{
			Vector3d vector3d = xA * xn;
			Vector3d v = p - q;
			double length = v.Length;
			return 0.07957747154594767 * vector3d.Dot(v / (length * length * length));
		}

		public static double Order2Approx(ref Triangle3d t, ref Vector3d p, ref Vector3d xn, ref double xA, ref Vector3d q)
		{
			Vector3d v = p - q;
			double length = v.Length;
			double num = length * length * length;
			double num2 = xA / 12.566370614359172 * xn.Dot(v / num);
			Matrix3d matrix3d = new Matrix3d(ref v, ref v);
			matrix3d *= 3.0 / (12.566370614359172 * num * length * length);
			double num3 = 1.0 / (12.566370614359172 * num);
			Matrix3d matrix3d2 = new Matrix3d(num3, num3, num3) - matrix3d;
			Vector3d vector3d = new Vector3d((t.V0.x + t.V1.x + t.V2.x) / 3.0, (t.V0.y + t.V1.y + t.V2.y) / 3.0, (t.V0.z + t.V1.z + t.V2.z) / 3.0) - p;
			Matrix3d matrix3d3 = new Matrix3d(ref vector3d, ref xn);
			double num4 = xA * matrix3d3.InnerProduct(ref matrix3d2);
			return num2 + num4;
		}
	}
}
