using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class RemoveDuplicateTriangles
	{
		public RemoveDuplicateTriangles(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public virtual bool Apply()
		{
			this.Removed = 0;
			double tolSqr = this.VertexTolerance * this.VertexTolerance;
			PointSetHashtable pointSetHashtable = new PointSetHashtable(new RemoveDuplicateTriangles.TriCentroids
			{
				Mesh = this.Mesh
			});
			int maxAxisSubdivs = (this.Mesh.TriangleCount > 100000) ? 128 : 64;
			pointSetHashtable.Build(maxAxisSubdivs);
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			Vector3d zero4 = Vector3d.Zero;
			Vector3d zero5 = Vector3d.Zero;
			Vector3d zero6 = Vector3d.Zero;
			int maxTriangleID = this.Mesh.MaxTriangleID;
			int[] array = new int[1024];
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (this.Mesh.IsTriangle(i))
				{
					Vector3d triCentroid = this.Mesh.GetTriCentroid(i);
					int num;
					while (!pointSetHashtable.FindInBall(triCentroid, this.VertexTolerance, array, out num))
					{
						array = new int[array.Length];
					}
					if (num == 1 && array[0] != i)
					{
						throw new Exception("RemoveDuplicateTriangles.Apply: how could this happen?!");
					}
					if (num > 1)
					{
						this.Mesh.GetTriVertices(i, ref zero, ref zero2, ref zero3);
						Vector3d vector3d = MathUtil.Normal(zero, zero2, zero3);
						for (int j = 0; j < num; j++)
						{
							if (array[j] != i)
							{
								this.Mesh.GetTriVertices(array[j], ref zero4, ref zero5, ref zero6);
								if (this.is_same_triangle(ref zero, ref zero2, ref zero3, ref zero4, ref zero5, ref zero6, tolSqr))
								{
									if (this.CheckOrientation)
									{
										Vector3d v = MathUtil.Normal(zero4, zero5, zero6);
										if (vector3d.Dot(v) < 0.99)
										{
											goto IL_1A1;
										}
									}
									if (this.Mesh.RemoveTriangle(array[j], true, false) == MeshResult.Ok)
									{
										this.Removed++;
									}
								}
							}
							IL_1A1:;
						}
					}
				}
			}
			return true;
		}

		private bool is_same_triangle(ref Vector3d a, ref Vector3d b, ref Vector3d c, ref Vector3d x, ref Vector3d y, ref Vector3d z, double tolSqr)
		{
			if (a.DistanceSquared(x) < tolSqr)
			{
				if (b.DistanceSquared(y) < tolSqr && c.DistanceSquared(z) < tolSqr)
				{
					return true;
				}
				if (b.DistanceSquared(z) < tolSqr && c.DistanceSquared(y) < tolSqr)
				{
					return true;
				}
			}
			else if (a.DistanceSquared(y) < tolSqr)
			{
				if (b.DistanceSquared(x) < tolSqr && c.DistanceSquared(z) < tolSqr)
				{
					return true;
				}
				if (b.DistanceSquared(z) < tolSqr && c.DistanceSquared(x) < tolSqr)
				{
					return true;
				}
			}
			else if (a.DistanceSquared(z) < tolSqr)
			{
				if (b.DistanceSquared(x) < tolSqr && c.DistanceSquared(y) < tolSqr)
				{
					return true;
				}
				if (b.DistanceSquared(y) < tolSqr && c.DistanceSquared(x) < tolSqr)
				{
					return true;
				}
			}
			return false;
		}

		public DMesh3 Mesh;

		public double VertexTolerance = 9.999999974752427E-07;

		public bool CheckOrientation = true;

		public int Removed;

		private class TriCentroids : IPointSet
		{
			public int VertexCount
			{
				get
				{
					return this.Mesh.TriangleCount;
				}
			}

			public int MaxVertexID
			{
				get
				{
					return this.Mesh.MaxTriangleID;
				}
			}

			public bool HasVertexNormals
			{
				get
				{
					return false;
				}
			}

			public bool HasVertexColors
			{
				get
				{
					return false;
				}
			}

			public Vector3d GetVertex(int i)
			{
				return this.Mesh.GetTriCentroid(i);
			}

			public Vector3f GetVertexNormal(int i)
			{
				return Vector3f.AxisY;
			}

			public Vector3f GetVertexColor(int i)
			{
				return Vector3f.One;
			}

			public bool IsVertex(int tID)
			{
				return this.Mesh.IsTriangle(tID);
			}

			public IEnumerable<int> VertexIndices()
			{
				return this.Mesh.TriangleIndices();
			}

			public int Timestamp
			{
				get
				{
					return this.Mesh.Timestamp;
				}
			}

			public DMesh3 Mesh;
		}
	}
}
