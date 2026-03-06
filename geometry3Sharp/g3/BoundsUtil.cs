using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public static class BoundsUtil
	{
		public static AxisAlignedBox3d Bounds(IEnumerable<DMesh3> meshes)
		{
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			foreach (DMesh3 dmesh in meshes)
			{
				empty.Contain(dmesh.CachedBounds);
			}
			return empty;
		}

		public static AxisAlignedBox3d Bounds(IPointSet source)
		{
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			foreach (int i in source.VertexIndices())
			{
				empty.Contain(source.GetVertex(i));
			}
			return empty;
		}

		public static AxisAlignedBox3d Bounds(ref Triangle3d tri)
		{
			return BoundsUtil.Bounds(ref tri.V0, ref tri.V1, ref tri.V2);
		}

		public static AxisAlignedBox3d Bounds(ref Vector3d v0, ref Vector3d v1, ref Vector3d v2)
		{
			AxisAlignedBox3d result;
			MathUtil.MinMax(v0.x, v1.x, v2.x, out result.Min.x, out result.Max.x);
			MathUtil.MinMax(v0.y, v1.y, v2.y, out result.Min.y, out result.Max.y);
			MathUtil.MinMax(v0.z, v1.z, v2.z, out result.Min.z, out result.Max.z);
			return result;
		}

		public static AxisAlignedBox2d Bounds(ref Vector2d v0, ref Vector2d v1, ref Vector2d v2)
		{
			AxisAlignedBox2d result;
			MathUtil.MinMax(v0.x, v1.x, v2.x, out result.Min.x, out result.Max.x);
			MathUtil.MinMax(v0.y, v1.y, v2.y, out result.Min.y, out result.Max.y);
			return result;
		}

		public static AxisAlignedBox3d Bounds(ref AxisAlignedBox3d boxIn, Func<Vector3d, Vector3d> TransformF)
		{
			if (TransformF == null)
			{
				return boxIn;
			}
			AxisAlignedBox3d result = new AxisAlignedBox3d(TransformF(boxIn.Corner(0)));
			for (int i = 1; i < 8; i++)
			{
				result.Contain(TransformF(boxIn.Corner(i)));
			}
			return result;
		}

		public static AxisAlignedBox3d Bounds(IEnumerable<Vector3d> positions)
		{
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			foreach (Vector3d v in positions)
			{
				empty.Contain(v);
			}
			return empty;
		}

		public static AxisAlignedBox3f Bounds(IEnumerable<Vector3f> positions)
		{
			AxisAlignedBox3f empty = AxisAlignedBox3f.Empty;
			foreach (Vector3f v in positions)
			{
				empty.Contain(v);
			}
			return empty;
		}

		public static AxisAlignedBox2d Bounds(IEnumerable<Vector2d> positions)
		{
			AxisAlignedBox2d empty = AxisAlignedBox2d.Empty;
			foreach (Vector2d v in positions)
			{
				empty.Contain(v);
			}
			return empty;
		}

		public static AxisAlignedBox2f Bounds(IEnumerable<Vector2f> positions)
		{
			AxisAlignedBox2f empty = AxisAlignedBox2f.Empty;
			foreach (Vector2f v in positions)
			{
				empty.Contain(v);
			}
			return empty;
		}

		public static AxisAlignedBox3d Bounds<T>(IEnumerable<T> values, Func<T, Vector3d> PositionF)
		{
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			foreach (T arg in values)
			{
				empty.Contain(PositionF(arg));
			}
			return empty;
		}

		public static AxisAlignedBox3f Bounds<T>(IEnumerable<T> values, Func<T, Vector3f> PositionF)
		{
			AxisAlignedBox3f empty = AxisAlignedBox3f.Empty;
			foreach (T arg in values)
			{
				empty.Contain(PositionF(arg));
			}
			return empty;
		}

		public static AxisAlignedBox3d Bounds(IEnumerable<Vector3d> values, TransformSequence xform)
		{
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			foreach (Vector3d p in values)
			{
				empty.Contain(xform.TransformP(p));
			}
			return empty;
		}

		public static AxisAlignedBox3d BoundsInFrame(IEnumerable<Vector3d> values, Frame3f f)
		{
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			foreach (Vector3d v in values)
			{
				empty.Contain(f.ToFrameP(v));
			}
			return empty;
		}

		public static void TrianglesContained(DMesh3 mesh, Func<Vector3d, int, bool> ContainF, Action<int> AddF, int nMode = 0)
		{
			BitArray bitArray = null;
			if (nMode != 0)
			{
				bitArray = new BitArray(mesh.MaxVertexID);
				foreach (int num in mesh.VertexIndices())
				{
					if (ContainF(mesh.GetVertex(num), num))
					{
						bitArray[num] = true;
					}
				}
			}
			foreach (int num2 in mesh.TriangleIndices())
			{
				Index3i triangle = mesh.GetTriangle(num2);
				bool flag = false;
				if (nMode == 0)
				{
					if (ContainF(mesh.GetTriCentroid(num2), num2))
					{
						flag = true;
					}
				}
				else
				{
					flag = ((bitArray[triangle.a] ? 1 : 0) + (bitArray[triangle.b] ? 1 : 0) + (bitArray[triangle.c] ? 1 : 0) >= nMode);
				}
				if (flag)
				{
					AddF(num2);
				}
			}
		}
	}
}
