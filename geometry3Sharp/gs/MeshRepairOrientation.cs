using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using g3;

namespace gs
{
	public class MeshRepairOrientation
	{
		protected DMeshAABBTree3 Spatial
		{
			get
			{
				if (this.spatial == null)
				{
					this.spatial = new DMeshAABBTree3(this.Mesh, true);
				}
				return this.spatial;
			}
		}

		public MeshRepairOrientation(DMesh3 mesh3, DMeshAABBTree3 spatial = null)
		{
			this.Mesh = mesh3;
			this.spatial = spatial;
		}

		public void OrientComponents()
		{
			this.Components = new List<MeshRepairOrientation.Component>();
			HashSet<int> hashSet = new HashSet<int>(this.Mesh.TriangleIndices());
			List<int> list = new List<int>();
			while (hashSet.Count > 0)
			{
				MeshRepairOrientation.Component component = new MeshRepairOrientation.Component();
				component.triangles = new List<int>();
				list.Clear();
				int item = hashSet.First<int>();
				hashSet.Remove(item);
				component.triangles.Add(item);
				list.Add(item);
				while (list.Count > 0)
				{
					int tID = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
					Index3i triangle = this.Mesh.GetTriangle(tID);
					Index3i triNeighbourTris = this.Mesh.GetTriNeighbourTris(tID);
					for (int i = 0; i < 3; i++)
					{
						int num = triNeighbourTris[i];
						if (hashSet.Contains(num))
						{
							int b = triangle[i];
							int a = triangle[(i + 1) % 3];
							Index3i triangle2 = this.Mesh.GetTriangle(num);
							if (IndexUtil.find_tri_ordered_edge(a, b, ref triangle2) == -1)
							{
								this.Mesh.ReverseTriOrientation(num);
							}
							list.Add(num);
							hashSet.Remove(num);
							component.triangles.Add(num);
						}
					}
				}
				this.Components.Add(component);
			}
		}

		public void ComputeStatistics()
		{
			DMeshAABBTree3 dmeshAABBTree = this.Spatial;
			foreach (MeshRepairOrientation.Component c in this.Components)
			{
				this.compute_statistics(c);
			}
		}

		private void compute_statistics(MeshRepairOrientation.Component c)
		{
			int count = c.triangles.Count;
			c.inFacing = (c.outFacing = 0.0);
			double dist = 2.0 * this.Mesh.CachedBounds.DiagonalLength;
			HashSet<int> @object = new HashSet<int>(c.triangles);
			this.spatial.TriangleFilterF = new Func<int, bool>(@object.Contains);
			SpinLock count_lock = default(SpinLock);
			gParallel.BlockStartEnd(0, count - 1, delegate(int a, int b)
			{
				for (int i = a; i <= b; i++)
				{
					int num = c.triangles[i];
					Vector3d vector3d;
					double num2;
					Vector3d v;
					this.Mesh.GetTriInfo(num, out vector3d, out num2, out v);
					if (num2 >= 9.999999974752427E-07)
					{
						Vector3d origin = v + dist * vector3d;
						Vector3d origin2 = v - dist * vector3d;
						int num3 = this.spatial.FindNearestHitTriangle(new Ray3d(origin, -vector3d, false), double.MaxValue);
						int num4 = this.spatial.FindNearestHitTriangle(new Ray3d(origin2, vector3d, false), double.MaxValue);
						if ((num3 == num || num4 == num) && (num3 != num || num4 != num))
						{
							bool flag = false;
							count_lock.Enter(ref flag);
							if (num4 == num)
							{
								c.inFacing += num2;
							}
							else if (num3 == num)
							{
								c.outFacing += num2;
							}
							count_lock.Exit();
						}
					}
				}
			}, -1, false);
			this.spatial.TriangleFilterF = null;
		}

		public void SolveGlobalOrientation()
		{
			this.ComputeStatistics();
			MeshEditor meshEditor = new MeshEditor(this.Mesh);
			foreach (MeshRepairOrientation.Component component in this.Components)
			{
				if (component.inFacing > component.outFacing)
				{
					meshEditor.ReverseTriangles(component.triangles, true);
				}
			}
		}

		public DMesh3 Mesh;

		private DMeshAABBTree3 spatial;

		private List<MeshRepairOrientation.Component> Components = new List<MeshRepairOrientation.Component>();

		private class Component
		{
			public List<int> triangles;

			public double outFacing;

			public double inFacing;
		}
	}
}
