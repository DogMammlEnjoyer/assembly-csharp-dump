using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public static class DGraph2Util
	{
		public static DGraph2Util.Curves ExtractCurves(DGraph2 graph)
		{
			DGraph2Util.Curves curves = new DGraph2Util.Curves();
			curves.Loops = new List<Polygon2d>();
			curves.Paths = new List<PolyLine2d>();
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
					PolyLine2d polyLine2d = new PolyLine2d();
					polyLine2d.AppendVertex(graph.GetVertex(num2));
					do
					{
						hashSet.Add(num3);
						Index2i index2i = DGraph2Util.NextEdgeAndVtx(num3, num2, graph);
						num3 = index2i.a;
						num2 = index2i.b;
						polyLine2d.AppendVertex(graph.GetVertex(num2));
					}
					while (!hashSet2.Contains(num2) && !hashSet3.Contains(num2));
					curves.Paths.Add(polyLine2d);
				}
			}
			hashSet2.Clear();
			foreach (int num4 in hashSet3)
			{
				foreach (int num5 in graph.VtxEdgesItr(num4))
				{
					if (!hashSet.Contains(num5))
					{
						int num6 = num4;
						int num7 = num5;
						PolyLine2d polyLine2d2 = new PolyLine2d();
						polyLine2d2.AppendVertex(graph.GetVertex(num6));
						bool flag = false;
						do
						{
							hashSet.Add(num7);
							Index2i index2i2 = DGraph2Util.NextEdgeAndVtx(num7, num6, graph);
							num7 = index2i2.a;
							num6 = index2i2.b;
							if (num6 == num4)
							{
								goto Block_21;
							}
							polyLine2d2.AppendVertex(graph.GetVertex(num6));
						}
						while (num7 != 2147483647 && !hashSet3.Contains(num6));
						IL_1ED:
						if (flag)
						{
							curves.Loops.Add(new Polygon2d(polyLine2d2.Vertices));
							continue;
						}
						curves.Paths.Add(polyLine2d2);
						continue;
						Block_21:
						flag = true;
						goto IL_1ED;
					}
				}
			}
			foreach (int num8 in graph.EdgeIndices())
			{
				if (!hashSet.Contains(num8))
				{
					int num9 = num8;
					int num10 = graph.GetEdgeV(num9).a;
					Polygon2d polygon2d = new Polygon2d();
					polygon2d.AppendVertex(graph.GetVertex(num10));
					for (;;)
					{
						hashSet.Add(num9);
						Index2i index2i3 = DGraph2Util.NextEdgeAndVtx(num9, num10, graph);
						num9 = index2i3.a;
						num10 = index2i3.b;
						polygon2d.AppendVertex(graph.GetVertex(num10));
						if (num9 == 2147483647 || hashSet3.Contains(num10))
						{
							break;
						}
						if (hashSet.Contains(num9))
						{
							goto Block_31;
						}
					}
					throw new Exception("how did this happen??");
					Block_31:
					polygon2d.RemoveVertex(polygon2d.VertexCount - 1);
					curves.Loops.Add(polygon2d);
				}
			}
			return curves;
		}

		public static void ChainOpenPaths(DGraph2Util.Curves c, double epsilon = 2.220446049250313E-16)
		{
			List<PolyLine2d> list = new List<PolyLine2d>(c.Paths);
			c.Paths = new List<PolyLine2d>();
			List<PolyLine2d> list2 = new List<PolyLine2d>();
			List<PolyLine2d> list3 = new List<PolyLine2d>();
			bool flag = true;
			while (flag && list.Count > 0)
			{
				flag = false;
				foreach (PolyLine2d polyLine2d in list)
				{
					List<PolyLine2d> list4 = DGraph2Util.find_connected_start(polyLine2d, list, epsilon);
					List<PolyLine2d> list5 = DGraph2Util.find_connected_end(polyLine2d, list, epsilon);
					if (list4.Count == 0 || list5.Count == 0)
					{
						list2.Add(polyLine2d);
						flag = true;
					}
					else
					{
						list3.Add(polyLine2d);
					}
				}
				list.Clear();
				list.AddRange(list3);
				list3.Clear();
			}
			flag = true;
			while (flag && list.Count > 0)
			{
				flag = false;
				for (;;)
				{
					IL_B8:
					foreach (PolyLine2d polyLine2d2 in list)
					{
						List<PolyLine2d> list6 = DGraph2Util.find_connected_start(polyLine2d2, list, epsilon);
						List<PolyLine2d> list7 = DGraph2Util.find_connected_end(polyLine2d2, list, 2.0 * epsilon);
						if (list6.Count == 1 && list7.Count == 1 && list6[0] == list7[0])
						{
							c.Loops.Add(DGraph2Util.to_loop(polyLine2d2, list6[0], epsilon));
							list.Remove(polyLine2d2);
							list.Remove(list6[0]);
							list3.Remove(list6[0]);
							flag = true;
							goto IL_B8;
						}
						if (list6.Count == 1 && list7.Count < 2)
						{
							list3.Add(DGraph2Util.merge_paths(list6[0], polyLine2d2, 2.0 * epsilon));
							list.Remove(polyLine2d2);
							list.Remove(list6[0]);
							list3.Remove(list6[0]);
							flag = true;
							goto IL_B8;
						}
						if (list7.Count == 1 && list6.Count < 2)
						{
							list3.Add(DGraph2Util.merge_paths(polyLine2d2, list7[0], 2.0 * epsilon));
							list.Remove(polyLine2d2);
							list.Remove(list7[0]);
							list3.Remove(list7[0]);
							flag = true;
							goto IL_B8;
						}
						list3.Add(polyLine2d2);
					}
					break;
				}
				list.Clear();
				list.AddRange(list3);
				list3.Clear();
			}
			c.Paths.AddRange(list);
			c.Paths.AddRange(list2);
		}

		private static List<PolyLine2d> find_connected_start(PolyLine2d pTest, List<PolyLine2d> potential, double eps = 2.220446049250313E-16)
		{
			List<PolyLine2d> list = new List<PolyLine2d>();
			foreach (PolyLine2d polyLine2d in potential)
			{
				if (pTest != polyLine2d && (pTest.Start.Distance(polyLine2d.Start) < eps || pTest.Start.Distance(polyLine2d.End) < eps))
				{
					list.Add(polyLine2d);
				}
			}
			return list;
		}

		private static List<PolyLine2d> find_connected_end(PolyLine2d pTest, List<PolyLine2d> potential, double eps = 2.220446049250313E-16)
		{
			List<PolyLine2d> list = new List<PolyLine2d>();
			foreach (PolyLine2d polyLine2d in potential)
			{
				if (pTest != polyLine2d && (pTest.End.Distance(polyLine2d.Start) < eps || pTest.End.Distance(polyLine2d.End) < eps))
				{
					list.Add(polyLine2d);
				}
			}
			return list;
		}

		private static Polygon2d to_loop(PolyLine2d p1, PolyLine2d p2, double eps = 2.220446049250313E-16)
		{
			Polygon2d polygon2d = new Polygon2d(p1.Vertices);
			if (p1.End.Distance(p2.Start) > eps)
			{
				p2.Reverse();
			}
			polygon2d.AppendVertices(p2);
			return polygon2d;
		}

		private static PolyLine2d merge_paths(PolyLine2d p1, PolyLine2d p2, double eps = 2.220446049250313E-16)
		{
			PolyLine2d polyLine2d;
			if (p1.End.Distance(p2.Start) < eps)
			{
				polyLine2d = new PolyLine2d(p1);
				polyLine2d.AppendVertices(p2);
			}
			else if (p1.End.Distance(p2.End) < eps)
			{
				polyLine2d = new PolyLine2d(p1);
				p2.Reverse();
				polyLine2d.AppendVertices(p2);
			}
			else if (p1.Start.Distance(p2.Start) < eps)
			{
				p2.Reverse();
				polyLine2d = new PolyLine2d(p2);
				polyLine2d.AppendVertices(p1);
			}
			else
			{
				if (p1.Start.Distance(p2.End) >= eps)
				{
					throw new Exception("shit");
				}
				polyLine2d = new PolyLine2d(p2);
				polyLine2d.AppendVertices(p1);
			}
			return polyLine2d;
		}

		public static int DisconnectJunctions(DGraph2 graph)
		{
			List<int> list = new List<int>();
			foreach (int num in graph.VertexIndices())
			{
				if (graph.IsJunctionVertex(num))
				{
					list.Add(num);
				}
			}
			foreach (int num2 in list)
			{
				Vector2d vertex = graph.GetVertex(num2);
				int[] array = graph.VtxVerticesItr(num2).ToArray<int>();
				Index2i max = Index2i.Max;
				double num3 = 0.0;
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = i + 1; j < array.Length; j++)
					{
						double num4 = Vector2d.AngleD((graph.GetVertex(array[i]) - vertex).Normalized, (graph.GetVertex(array[j]) - vertex).Normalized);
						num4 = Math.Abs(num4);
						if (num4 > num3)
						{
							num3 = num4;
							max = new Index2i(array[i], array[j]);
						}
					}
				}
				for (int k = 0; k < array.Length; k++)
				{
					if (array[k] != max.a && array[k] != max.b)
					{
						int eID = graph.FindEdge(num2, array[k]);
						graph.RemoveEdge(eID, true);
						if (graph.IsVertex(array[k]))
						{
							Vector2d v = Vector2d.Lerp(graph.GetVertex(array[k]), vertex, 0.99);
							int v2 = graph.AppendVertex(v);
							graph.AppendEdge(array[k], v2, -1);
						}
					}
				}
			}
			return list.Count;
		}

		public static void DisconnectJunction(DGraph2 graph, int vid, double shrinkFactor = 1.0)
		{
			Vector2d vertex = graph.GetVertex(vid);
			int[] array = graph.VtxVerticesItr(vid).ToArray<int>();
			for (int i = 0; i < array.Length; i++)
			{
				int eID = graph.FindEdge(vid, array[i]);
				graph.RemoveEdge(eID, true);
				if (graph.IsVertex(array[i]))
				{
					Vector2d v = Vector2d.Lerp(graph.GetVertex(array[i]), vertex, shrinkFactor);
					int v2 = graph.AppendVertex(v);
					graph.AppendEdge(array[i], v2, -1);
				}
			}
		}

		public static Vector2d VertexLaplacian(DGraph2 graph, int vid, out bool isValid)
		{
			Vector2d vertex = graph.GetVertex(vid);
			Vector2d vector2d = Vector2d.Zero;
			int num = 0;
			foreach (int vID in graph.VtxVerticesItr(vid))
			{
				vector2d += graph.GetVertex(vID);
				num++;
			}
			if (num == 2)
			{
				vector2d /= (double)num;
				isValid = true;
				return vector2d - vertex;
			}
			isValid = false;
			return vertex;
		}

		public static bool FindRayIntersection(Vector2d o, Vector2d d, out int hit_eid, out double hit_ray_t, DGraph2 graph)
		{
			Line2d line = new Line2d(o, d);
			Vector2d zero = Vector2d.Zero;
			Vector2d zero2 = Vector2d.Zero;
			int num = -1;
			double num2 = double.MaxValue;
			IntrLine2Segment2 intrLine2Segment = new IntrLine2Segment2(line, new Segment2d(zero, zero2));
			foreach (int num3 in graph.VertexIndices())
			{
				graph.GetEdgeV(num3, ref zero, ref zero2);
				intrLine2Segment.Segment = new Segment2d(zero, zero2);
				if (intrLine2Segment.Find() && intrLine2Segment.IsSimpleIntersection && intrLine2Segment.Parameter > 0.0 && intrLine2Segment.Parameter < num2)
				{
					num = num3;
					num2 = intrLine2Segment.Parameter;
				}
			}
			hit_eid = num;
			hit_ray_t = num2;
			return hit_ray_t < double.MaxValue;
		}

		public static Index2i NextEdgeAndVtx(int eid, int prev_vid, DGraph2 graph)
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

		public static List<int> WalkToNextNonRegularVtx(DGraph2 graph, int fromVtx, int eid)
		{
			List<int> list = new List<int>();
			list.Add(fromVtx);
			int prev_vid = fromVtx;
			int eid2 = eid;
			bool flag = true;
			while (flag)
			{
				Index2i index2i = DGraph2Util.NextEdgeAndVtx(eid2, prev_vid, graph);
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

		public static double PathLength(DGraph2 graph, IList<int> pathVertices)
		{
			double num = 0.0;
			int count = pathVertices.Count;
			Vector2d vector2d = graph.GetVertex(pathVertices[0]);
			Vector2d vector2d2 = Vector2d.Zero;
			for (int i = 1; i < count; i++)
			{
				vector2d2 = graph.GetVertex(pathVertices[i]);
				num += vector2d.Distance(vector2d2);
				vector2d = vector2d2;
			}
			return num;
		}

		public class Curves
		{
			public List<Polygon2d> Loops;

			public List<PolyLine2d> Paths;
		}
	}
}
