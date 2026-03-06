using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class GraphCells2d
	{
		public GraphCells2d(DGraph2 graph)
		{
			this.Graph = graph;
		}

		public void FindCells()
		{
			Index2i[][] array = new Index2i[this.Graph.MaxVertexID][];
			HashSet<Index2i> hashSet = new HashSet<Index2i>();
			foreach (int num in this.Graph.VertexIndices())
			{
				int[] array2 = this.Graph.SortedVtxEdges(num);
				array[num] = new Index2i[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array[num][i] = new Index2i(array2[i], array2[(i + 1) % array2.Length]);
					hashSet.Add(new Index2i(num, i));
				}
			}
			this.CellLoops = new List<int[]>();
			List<int> list = new List<int>();
			while (hashSet.Count > 0)
			{
				Index2i index2i = hashSet.First<Index2i>();
				hashSet.Remove(index2i);
				int a = index2i.a;
				int b = index2i.b;
				int a2 = array[a][b].a;
				int b2 = array[a][b].b;
				list.Clear();
				list.Add(a);
				int num2 = a;
				int num3 = b2;
				bool flag = false;
				while (!flag)
				{
					Index2i edgeV = this.Graph.GetEdgeV(num3);
					int num4 = (edgeV.a == num2) ? edgeV.b : edgeV.a;
					if (num4 == a)
					{
						flag = true;
					}
					else
					{
						Index2i[] array3 = array[num4];
						int num5 = -1;
						for (int j = 0; j < array3.Length; j++)
						{
							if (array3[j].a == num3)
							{
								num5 = j;
								break;
							}
						}
						if (num5 == -1)
						{
							throw new Exception("could not find next wedge?");
						}
						hashSet.Remove(new Index2i(num4, num5));
						list.Add(num4);
						num2 = num4;
						num3 = array3[num5].b;
					}
				}
				this.CellLoops.Add(list.ToArray());
			}
		}

		public List<Polygon2d> CellsToPolygons(Func<Polygon2d, bool> FilterF = null)
		{
			List<Polygon2d> list = new List<Polygon2d>();
			for (int i = 0; i < this.CellLoops.Count; i++)
			{
				int[] array = this.CellLoops[i];
				Polygon2d polygon2d = new Polygon2d();
				for (int j = 0; j < array.Length; j++)
				{
					polygon2d.AppendVertex(this.Graph.GetVertex(array[j]));
				}
				if (FilterF == null || FilterF(polygon2d))
				{
					list.Add(polygon2d);
				}
			}
			return list;
		}

		public List<Polygon2d> ContainedCells(GeneralPolygon2d container)
		{
			Func<Polygon2d, bool> filterF = delegate(Polygon2d poly)
			{
				bool isClockwise = poly.IsClockwise;
				for (int i = 0; i < poly.VertexCount; i++)
				{
					Segment2d segment2d = poly.Segment(i);
					Vector2d vector2d = segment2d.Center + 1.1920928955078125E-07 * segment2d.Direction.Perp;
					if (poly.Contains(vector2d) == isClockwise)
					{
						return container.Contains(vector2d);
					}
				}
				return false;
			};
			return this.CellsToPolygons(filterF);
		}

		public DGraph2 Graph;

		public List<int[]> CellLoops;
	}
}
