using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public static class DGraph3Util
	{
		public static DGraph3Util.Curves ExtractCurves(DGraph3 graph, bool bWantLoopIndices = false, Func<int, bool> CurveOrientationF = null)
		{
			DGraph3Util.Curves curves = default(DGraph3Util.Curves);
			curves.Loops = new List<DCurve3>();
			curves.Paths = new List<DCurve3>();
			if (bWantLoopIndices)
			{
				curves.LoopEdges = new List<List<int>>();
				curves.PathEdges = new List<List<int>>();
			}
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> hashSet2 = new HashSet<int>();
			HashSet<int> hashSet3 = new HashSet<int>();
			foreach (int num in graph.VertexIndices())
			{
				if (graph.IsBoundaryVertex(num))
				{
					hashSet2.Add(num);
				}
				if (graph.IsJunctionVertex(num))
				{
					hashSet3.Add(num);
				}
			}
			foreach (int num2 in hashSet2)
			{
				int num3 = graph.GetVtxEdges(num2)[0];
				if (!hashSet.Contains(num3))
				{
					bool flag = CurveOrientationF != null && CurveOrientationF(num3);
					DCurve3 dcurve = new DCurve3
					{
						Closed = false
					};
					List<int> list = bWantLoopIndices ? new List<int>() : null;
					dcurve.AppendVertex(graph.GetVertex(num2));
					if (list != null)
					{
						list.Add(num3);
					}
					for (;;)
					{
						hashSet.Add(num3);
						Index2i index2i = DGraph3Util.NextEdgeAndVtx(num3, num2, graph);
						num3 = index2i.a;
						num2 = index2i.b;
						dcurve.AppendVertex(graph.GetVertex(num2));
						if (hashSet2.Contains(num2) || hashSet3.Contains(num2))
						{
							break;
						}
						if (list != null)
						{
							list.Add(num3);
						}
					}
					if (flag)
					{
						dcurve.Reverse();
					}
					curves.Paths.Add(dcurve);
					if (list != null)
					{
						if (flag)
						{
							list.Reverse();
						}
						curves.PathEdges.Add(list);
					}
				}
			}
			curves.BoundaryV = hashSet2;
			foreach (int num4 in hashSet3)
			{
				foreach (int num5 in graph.VtxEdgesItr(num4))
				{
					if (!hashSet.Contains(num5))
					{
						int num6 = num4;
						int num7 = num5;
						bool flag2 = CurveOrientationF != null && CurveOrientationF(num7);
						DCurve3 dcurve2 = new DCurve3
						{
							Closed = false
						};
						List<int> list2 = bWantLoopIndices ? new List<int>() : null;
						dcurve2.AppendVertex(graph.GetVertex(num6));
						if (list2 != null)
						{
							list2.Add(num7);
						}
						for (;;)
						{
							hashSet.Add(num7);
							Index2i index2i2 = DGraph3Util.NextEdgeAndVtx(num7, num6, graph);
							num7 = index2i2.a;
							num6 = index2i2.b;
							dcurve2.AppendVertex(graph.GetVertex(num6));
							if (num7 == 2147483647 || hashSet3.Contains(num6))
							{
								break;
							}
							if (list2 != null)
							{
								list2.Add(num7);
							}
						}
						if (num6 == num4)
						{
							dcurve2.RemoveVertex(dcurve2.VertexCount - 1);
							dcurve2.Closed = true;
							if (flag2)
							{
								dcurve2.Reverse();
							}
							curves.Loops.Add(dcurve2);
							if (list2 != null)
							{
								if (flag2)
								{
									list2.Reverse();
								}
								curves.LoopEdges.Add(list2);
							}
							if (num7 != 2147483647)
							{
								hashSet.Add(num7);
							}
						}
						else
						{
							if (flag2)
							{
								dcurve2.Reverse();
							}
							curves.Paths.Add(dcurve2);
							if (list2 != null)
							{
								if (flag2)
								{
									list2.Reverse();
								}
								curves.PathEdges.Add(list2);
							}
						}
					}
				}
			}
			curves.JunctionV = hashSet3;
			foreach (int num8 in graph.EdgeIndices())
			{
				if (!hashSet.Contains(num8))
				{
					int num9 = num8;
					int num10 = graph.GetEdgeV(num9).a;
					bool flag3 = CurveOrientationF != null && CurveOrientationF(num9);
					DCurve3 dcurve3 = new DCurve3
					{
						Closed = true
					};
					List<int> list3 = bWantLoopIndices ? new List<int>() : null;
					dcurve3.AppendVertex(graph.GetVertex(num10));
					if (list3 != null)
					{
						list3.Add(num9);
					}
					for (;;)
					{
						hashSet.Add(num9);
						Index2i index2i3 = DGraph3Util.NextEdgeAndVtx(num9, num10, graph);
						num9 = index2i3.a;
						num10 = index2i3.b;
						dcurve3.AppendVertex(graph.GetVertex(num10));
						if (list3 != null)
						{
							list3.Add(num9);
						}
						if (num9 == 2147483647 || hashSet3.Contains(num10))
						{
							break;
						}
						if (hashSet.Contains(num9))
						{
							goto Block_55;
						}
					}
					throw new Exception("how did this happen??");
					Block_55:
					dcurve3.RemoveVertex(dcurve3.VertexCount - 1);
					if (flag3)
					{
						dcurve3.Reverse();
					}
					curves.Loops.Add(dcurve3);
					if (list3 != null)
					{
						list3.RemoveAt(list3.Count - 1);
						if (flag3)
						{
							list3.Reverse();
						}
						curves.LoopEdges.Add(list3);
					}
				}
			}
			return curves;
		}

		public static void DisconnectJunction(DGraph3 graph, int vid, double shrinkFactor = 1.0)
		{
			Vector3d vertex = graph.GetVertex(vid);
			int[] array = graph.VtxVerticesItr(vid).ToArray<int>();
			for (int i = 0; i < array.Length; i++)
			{
				int eID = graph.FindEdge(vid, array[i]);
				graph.RemoveEdge(eID, true);
				if (graph.IsVertex(array[i]))
				{
					Vector3d v = Vector3d.Lerp(graph.GetVertex(array[i]), vertex, shrinkFactor);
					int v2 = graph.AppendVertex(v);
					graph.AppendEdge(array[i], v2, -1);
				}
			}
		}

		public static Index2i NextEdgeAndVtx(int eid, int prev_vid, DGraph3 graph)
		{
			Index2i edgeV = graph.GetEdgeV(eid);
			if (edgeV.a == -1)
			{
				return Index2i.Max;
			}
			int num = (edgeV.a == prev_vid) ? edgeV.b : edgeV.a;
			if (graph.GetVtxEdgeCount(num) != 2)
			{
				return new Index2i(int.MaxValue, num);
			}
			foreach (int num2 in graph.VtxEdgesItr(num))
			{
				if (num2 != eid)
				{
					return new Index2i(num2, num);
				}
			}
			return Index2i.Max;
		}

		public static List<int> WalkToNextNonRegularVtx(DGraph3 graph, int fromVtx, int eid)
		{
			List<int> list = new List<int>();
			list.Add(fromVtx);
			int prev_vid = fromVtx;
			int eid2 = eid;
			bool flag = true;
			while (flag)
			{
				Index2i index2i = DGraph3Util.NextEdgeAndVtx(eid2, prev_vid, graph);
				int a = index2i.a;
				int b = index2i.b;
				if (a == 2147483647)
				{
					if (graph.IsRegularVertex(b))
					{
						throw new Exception("WalkToNextNonRegularVtx: have no next edge but vtx is regular - how?");
					}
					list.Add(b);
					flag = false;
				}
				else
				{
					list.Add(b);
					prev_vid = b;
					eid2 = a;
				}
			}
			return list;
		}

		public static void ErodeOpenSpurs(DGraph3 graph)
		{
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> hashSet2 = new HashSet<int>();
			HashSet<int> hashSet3 = new HashSet<int>();
			foreach (int num in graph.VertexIndices())
			{
				if (graph.IsBoundaryVertex(num))
				{
					hashSet2.Add(num);
				}
				if (graph.IsJunctionVertex(num))
				{
					hashSet3.Add(num);
				}
			}
			foreach (int num2 in hashSet2)
			{
				if (graph.IsVertex(num2))
				{
					int num3 = num2;
					int num4 = graph.GetVtxEdges(num3)[0];
					if (!hashSet.Contains(num4))
					{
						List<int> list = new List<int>();
						if (list != null)
						{
							list.Add(num4);
						}
						for (;;)
						{
							hashSet.Add(num4);
							Index2i index2i = DGraph3Util.NextEdgeAndVtx(num4, num3, graph);
							num4 = index2i.a;
							num3 = index2i.b;
							if (hashSet2.Contains(num3) || hashSet3.Contains(num3))
							{
								break;
							}
							if (list != null)
							{
								list.Add(num4);
							}
						}
						foreach (int eID in list)
						{
							graph.RemoveEdge(eID, true);
						}
					}
				}
			}
		}

		public struct Curves
		{
			public List<DCurve3> Loops;

			public List<DCurve3> Paths;

			public HashSet<int> BoundaryV;

			public HashSet<int> JunctionV;

			public List<List<int>> LoopEdges;

			public List<List<int>> PathEdges;
		}
	}
}
