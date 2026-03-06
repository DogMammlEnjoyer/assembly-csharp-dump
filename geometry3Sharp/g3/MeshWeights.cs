using System;

namespace g3
{
	public static class MeshWeights
	{
		public static Vector3d OneRingCentroid(DMesh3 mesh, int vID)
		{
			Vector3d vector3d = Vector3d.Zero;
			int num = 0;
			foreach (int vID2 in mesh.VtxVerticesItr(vID))
			{
				vector3d += mesh.GetVertex(vID2);
				num++;
			}
			if (num == 0)
			{
				return mesh.GetVertex(vID);
			}
			double num2 = 1.0 / (double)num;
			vector3d.x *= num2;
			vector3d.y *= num2;
			vector3d.z *= num2;
			return vector3d;
		}

		public static Vector3d CotanCentroid(DMesh3 mesh, int v_i)
		{
			Vector3d vector3d = Vector3d.Zero;
			double num = 0.0;
			Vector3d vertex = mesh.GetVertex(v_i);
			int vID = -1;
			int vID2 = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			bool flag = false;
			foreach (int eID in mesh.VtxEdgesItr(v_i))
			{
				num2 = -1;
				mesh.GetVtxNbrhood(eID, v_i, ref vID, ref vID2, ref num2, ref num3, ref num4);
				Vector3d vertex2 = mesh.GetVertex(vID);
				Vector3d vertex3 = mesh.GetVertex(vID2);
				double num5 = MathUtil.VectorCot((vertex - vertex3).Normalized, (vertex2 - vertex3).Normalized);
				if (num5 == 0.0)
				{
					flag = true;
					break;
				}
				double num6 = num5;
				if (num2 != -1)
				{
					Vector3d vertex4 = mesh.GetVertex(num2);
					double num7 = MathUtil.VectorCot((vertex - vertex4).Normalized, (vertex2 - vertex4).Normalized);
					if (num7 == 0.0)
					{
						flag = true;
						break;
					}
					num6 += num7;
				}
				vector3d += num6 * vertex2;
				num += num6;
			}
			if (flag || Math.Abs(num) < 1E-08)
			{
				return vertex;
			}
			return vector3d / num;
		}

		public static double VoronoiArea(DMesh3 mesh, int v_i)
		{
			double num = 0.0;
			Vector3d vertex = mesh.GetVertex(v_i);
			foreach (int tID in mesh.VtxTrianglesItr(v_i))
			{
				Index3i triangle = mesh.GetTriangle(tID);
				int num2 = (triangle[0] == v_i) ? 0 : ((triangle[1] == v_i) ? 1 : 2);
				Vector3d vertex2 = mesh.GetVertex(triangle[(num2 + 1) % 3]);
				Vector3d vertex3 = mesh.GetVertex(triangle[(num2 + 2) % 3]);
				if (MathUtil.IsObtuse(vertex, vertex2, vertex3))
				{
					Vector3d v = vertex2 - vertex;
					Vector3d v2 = vertex3 - vertex;
					v.Normalize(2.220446049250313E-16);
					v2.Normalize(2.220446049250313E-16);
					double num3 = 0.5 * v.Cross(v2).Length;
					if (Vector3d.AngleR(v, v2) > 1.5707963267948966)
					{
						num += num3 * 0.5;
					}
					else
					{
						num += num3 * 0.25;
					}
				}
				else
				{
					Vector3d v3 = vertex - vertex2;
					double num4 = v3.Normalize(2.220446049250313E-16);
					Vector3d v4 = vertex - vertex3;
					double num5 = v4.Normalize(2.220446049250313E-16);
					Vector3d normalized = (vertex2 - vertex3).Normalized;
					double num6 = MathUtil.VectorCot(v4, normalized);
					double num7 = MathUtil.VectorCot(v3, -normalized);
					num += num4 * num4 * num6 * 0.125;
					num += num5 * num5 * num7 * 0.125;
				}
			}
			return num;
		}

		public static Vector3d MeanValueCentroid(DMesh3 mesh, int v_i)
		{
			Vector3d vector3d = Vector3d.Zero;
			double num = 0.0;
			Vector3d vertex = mesh.GetVertex(v_i);
			int vID = -1;
			int vID2 = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			foreach (int eID in mesh.VtxEdgesItr(v_i))
			{
				num2 = -1;
				mesh.GetVtxNbrhood(eID, v_i, ref vID, ref vID2, ref num2, ref num3, ref num4);
				Vector3d vertex2 = mesh.GetVertex(vID);
				Vector3d a = vertex2 - vertex;
				double num5 = a.Normalize(2.220446049250313E-16);
				if (num5 >= 1E-08)
				{
					Vector3d normalized = (mesh.GetVertex(vID2) - vertex).Normalized;
					double num6 = MeshWeights.VectorTanHalfAngle(a, normalized);
					if (num2 != -1)
					{
						Vector3d normalized2 = (mesh.GetVertex(num2) - vertex).Normalized;
						num6 += MeshWeights.VectorTanHalfAngle(a, normalized2);
					}
					num6 /= num5;
					vector3d += num6 * vertex2;
					num += num6;
				}
			}
			if (num < 1E-08)
			{
				return vertex;
			}
			return vector3d / num;
		}

		public static double VectorTanHalfAngle(Vector3d a, Vector3d b)
		{
			double num = a.Dot(b);
			return Math.Sqrt(MathUtil.Clamp((1.0 - num) / (1.0 + num), 0.0, double.MaxValue));
		}
	}
}
