using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class MergeCoincidentEdges
	{
		public MergeCoincidentEdges(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public virtual bool Apply()
		{
			this.merge_r2 = this.MergeDistance * this.MergeDistance;
			PointSetHashtable pointSetHashtable = new PointSetHashtable(new MeshBoundaryEdgeMidpoints(this.Mesh));
			int maxAxisSubdivs = 64;
			if (this.Mesh.TriangleCount > 100000)
			{
				maxAxisSubdivs = 128;
			}
			if (this.Mesh.TriangleCount > 1000000)
			{
				maxAxisSubdivs = 256;
			}
			pointSetHashtable.Build(maxAxisSubdivs);
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			Vector3d zero4 = Vector3d.Zero;
			int[] array = new int[1024];
			List<int>[] array2 = new List<int>[this.Mesh.MaxEdgeID];
			HashSet<int> hashSet = new HashSet<int>();
			foreach (int num in this.Mesh.BoundaryEdgeIndices())
			{
				Vector3d edgePoint = this.Mesh.GetEdgePoint(num, 0.5);
				int num2;
				while (!pointSetHashtable.FindInBall(edgePoint, this.MergeDistance, array, out num2))
				{
					array = new int[array.Length];
				}
				if (num2 == 1 && array[0] != num)
				{
					throw new Exception("MergeCoincidentEdges.Apply: how could this happen?!");
				}
				if (num2 > 1)
				{
					this.Mesh.GetEdgeV(num, ref zero, ref zero2);
					List<int> list = new List<int>(num2 - 1);
					for (int i = 0; i < num2; i++)
					{
						if (array[i] != num)
						{
							this.Mesh.GetEdgeV(array[i], ref zero3, ref zero4);
							if (this.is_same_edge(ref zero, ref zero2, ref zero3, ref zero4))
							{
								list.Add(array[i]);
							}
						}
					}
					if (list.Count > 0)
					{
						array2[num] = list;
						hashSet.Add(num);
					}
				}
			}
			DynamicPriorityQueue<MergeCoincidentEdges.DuplicateEdge> dynamicPriorityQueue = new DynamicPriorityQueue<MergeCoincidentEdges.DuplicateEdge>();
			using (HashSet<int>.Enumerator enumerator2 = hashSet.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					int num3 = enumerator2.Current;
					if (this.OnlyUniquePairs)
					{
						if (array2[num3].Count != 1)
						{
							continue;
						}
						foreach (int num4 in array2[num3])
						{
							if (array2[num4].Count == 1)
							{
								int num5 = array2[num4][0];
							}
						}
					}
					dynamicPriorityQueue.Enqueue(new MergeCoincidentEdges.DuplicateEdge
					{
						eid = num3
					}, (float)array2[num3].Count);
				}
				goto IL_3B2;
			}
			IL_270:
			MergeCoincidentEdges.DuplicateEdge duplicateEdge = dynamicPriorityQueue.Dequeue();
			if (this.Mesh.IsEdge(duplicateEdge.eid) && array2[duplicateEdge.eid] != null && hashSet.Contains(duplicateEdge.eid) && this.Mesh.IsBoundaryEdge(duplicateEdge.eid))
			{
				List<int> list2 = array2[duplicateEdge.eid];
				bool flag = false;
				int num6 = 0;
				int num7 = 0;
				while (num7 < list2.Count && !flag)
				{
					int num8 = list2[num7];
					if (this.Mesh.IsEdge(num8) && this.Mesh.IsBoundaryEdge(num8))
					{
						DMesh3.MergeEdgesInfo mergeEdgesInfo;
						if (this.Mesh.MergeEdges(duplicateEdge.eid, num8, out mergeEdgesInfo) != MeshResult.Ok)
						{
							list2.RemoveAt(num7);
							num7--;
							array2[num8].Remove(duplicateEdge.eid);
							num6++;
						}
						else
						{
							flag = true;
							array2[num8] = null;
							hashSet.Remove(num8);
						}
					}
					num7++;
				}
				if (flag)
				{
					array2[duplicateEdge.eid] = null;
					hashSet.Remove(duplicateEdge.eid);
				}
				else
				{
					array2[duplicateEdge.eid] = null;
					hashSet.Remove(duplicateEdge.eid);
				}
			}
			IL_3B2:
			if (dynamicPriorityQueue.Count <= 0)
			{
				return true;
			}
			goto IL_270;
		}

		private bool is_same_edge(ref Vector3d a, ref Vector3d b, ref Vector3d c, ref Vector3d d)
		{
			return (a.DistanceSquared(c) < this.merge_r2 && b.DistanceSquared(d) < this.merge_r2) || (a.DistanceSquared(d) < this.merge_r2 && b.DistanceSquared(c) < this.merge_r2);
		}

		public DMesh3 Mesh;

		public double MergeDistance = 9.999999974752427E-07;

		public bool OnlyUniquePairs;

		private double merge_r2;

		private class DuplicateEdge : DynamicPriorityQueueNode
		{
			public int eid;
		}
	}
}
