using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class AutoHoleFill
	{
		public AutoHoleFill(DMesh3 mesh, EdgeLoop fillLoop)
		{
			this.Mesh = mesh;
			this.FillLoop = fillLoop;
		}

		public bool Apply()
		{
			AutoHoleFill.UseFillType useFillType = this.classify_hole();
			bool flag = false;
			bool flag2;
			if (useFillType == AutoHoleFill.UseFillType.PlanarFill && !flag)
			{
				flag2 = this.fill_planar();
			}
			else if (useFillType == AutoHoleFill.UseFillType.MinimalFill)
			{
				flag2 = this.fill_minimal();
			}
			else if (useFillType == AutoHoleFill.UseFillType.PlanarSpansFill)
			{
				flag2 = this.fill_planar_spans();
			}
			else
			{
				flag2 = this.fill_smooth();
			}
			if (!flag2 && useFillType != AutoHoleFill.UseFillType.SmoothFill)
			{
				flag2 = this.fill_smooth();
			}
			return flag2;
		}

		private AutoHoleFill.UseFillType classify_hole()
		{
			return AutoHoleFill.UseFillType.MinimalFill;
		}

		private bool fill_smooth()
		{
			return new SmoothedHoleFill(this.Mesh, this.FillLoop)
			{
				TargetEdgeLength = this.TargetEdgeLength,
				SmoothAlpha = 1.0,
				ConstrainToHoleInterior = true
			}.Apply();
		}

		private bool fill_planar()
		{
			Vector3d vector3d = Vector3d.Zero;
			Vector3d vector3d2 = Vector3d.Zero;
			int edgeCount = this.FillLoop.EdgeCount;
			for (int i = 0; i < edgeCount; i++)
			{
				int eID = this.FillLoop.Edges[i];
				vector3d += this.Mesh.GetTriNormal(this.Mesh.GetEdgeT(eID).a);
				vector3d2 += this.Mesh.GetEdgePoint(eID, 0.5);
			}
			vector3d.Normalize(2.220446049250313E-16);
			vector3d2 /= (double)edgeCount;
			PlanarHoleFiller planarHoleFiller = new PlanarHoleFiller(this.Mesh);
			planarHoleFiller.FillTargetEdgeLen = this.TargetEdgeLength;
			planarHoleFiller.AddFillLoop(this.FillLoop);
			planarHoleFiller.SetPlane(vector3d2, vector3d);
			return planarHoleFiller.Fill();
		}

		private bool fill_minimal()
		{
			return new MinimalHoleFill(this.Mesh, this.FillLoop).Apply();
		}

		private bool fill_planar_spans()
		{
			foreach (KeyValuePair<Vector3d, List<EdgeSpan>> keyValuePair in this.find_coplanar_span_sets(this.Mesh, this.FillLoop))
			{
				Vector3d key = keyValuePair.Key;
				List<EdgeSpan> value = keyValuePair.Value;
				Vector3d vertex = value[0].GetVertex(0);
				if (value.Count > 1)
				{
					using (List<List<EdgeSpan>>.Enumerator enumerator2 = this.sort_planar_spans(value, key).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							List<EdgeSpan> list = enumerator2.Current;
							if (list.Count == 1)
							{
								PlanarSpansFiller planarSpansFiller = new PlanarSpansFiller(this.Mesh, list);
								planarSpansFiller.FillTargetEdgeLen = this.TargetEdgeLength;
								planarSpansFiller.SetPlane(vertex, key);
								planarSpansFiller.Fill();
							}
						}
						continue;
					}
				}
				PlanarSpansFiller planarSpansFiller2 = new PlanarSpansFiller(this.Mesh, value);
				planarSpansFiller2.FillTargetEdgeLen = this.TargetEdgeLength;
				planarSpansFiller2.SetPlane(vertex, key);
				planarSpansFiller2.Fill();
			}
			return true;
		}

		private List<List<EdgeSpan>> sort_planar_spans(List<EdgeSpan> allspans, Vector3d normal)
		{
			List<List<EdgeSpan>> list = new List<List<EdgeSpan>>();
			Frame3f polyFrame = new Frame3f(Vector3d.Zero, normal);
			int count = allspans.Count;
			List<PolyLine2d> list2 = new List<PolyLine2d>();
			foreach (EdgeSpan span in allspans)
			{
				list2.Add(this.to_polyline(span, polyFrame));
			}
			bool[] array = new bool[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = false;
			}
			bool[] array2 = new bool[count];
			for (int j = 0; j < count; j++)
			{
				if (!array2[j])
				{
					bool flag = array[j];
					AxisAlignedBox2d bounds = list2[j].Bounds;
					array2[j] = true;
					List<int> list3 = new List<int>
					{
						j
					};
					for (int k = j + 1; k < count; k++)
					{
						if (!array2[k])
						{
							AxisAlignedBox2d bounds2 = list2[k].Bounds;
							if (bounds.Intersects(bounds2))
							{
								array2[k] = true;
								flag = (flag || array[k]);
								bounds.Contain(bounds2);
								list3.Add(k);
							}
						}
					}
					if (!flag)
					{
						List<EdgeSpan> list4 = new List<EdgeSpan>();
						foreach (int index in list3)
						{
							list4.Add(allspans[index]);
						}
						list.Add(list4);
					}
				}
			}
			return list;
		}

		private PolyLine2d to_polyline(EdgeSpan span, Frame3f polyFrame)
		{
			int vertexCount = span.VertexCount;
			PolyLine2d polyLine2d = new PolyLine2d();
			for (int i = 0; i < vertexCount; i++)
			{
				polyLine2d.AppendVertex(polyFrame.ToPlaneUV((Vector3f)span.GetVertex(i), 2));
			}
			return polyLine2d;
		}

		private Polygon2d to_polygon(EdgeSpan span, Frame3f polyFrame)
		{
			int vertexCount = span.VertexCount;
			Polygon2d polygon2d = new Polygon2d();
			for (int i = 0; i < vertexCount; i++)
			{
				polygon2d.AppendVertex(polyFrame.ToPlaneUV((Vector3f)span.GetVertex(i), 2));
			}
			return polygon2d;
		}

		private bool self_intersects(PolyLine2d poly)
		{
			Segment2d segment2d = new Segment2d(poly.Start, poly.End);
			int num = poly.VertexCount - 2;
			for (int i = 1; i < num; i++)
			{
				if (poly.Segment(i).Intersects(ref segment2d, 5E-324, 0.0))
				{
					return true;
				}
			}
			return false;
		}

		private Dictionary<Vector3d, List<EdgeSpan>> find_coplanar_span_sets(DMesh3 mesh, EdgeLoop loop)
		{
			double num = 0.999;
			Dictionary<Vector3d, List<EdgeSpan>> dictionary = new Dictionary<Vector3d, List<EdgeSpan>>();
			int num2 = loop.Vertices.Length;
			int num3 = loop.Edges.Length;
			Vector3d[] array = new Vector3d[num3];
			for (int i = 0; i < num3; i++)
			{
				array[i] = mesh.GetTriNormal(mesh.GetEdgeT(loop.Edges[i]).a);
			}
			bool[] array2 = new bool[num2];
			int num4 = 0;
			for (int j = 0; j < num2; j++)
			{
				int num5 = (j == 0) ? (num2 - 1) : (j - 1);
				if (array[j].Dot(ref array[num5]) > num)
				{
					array2[j] = true;
					num4++;
				}
			}
			if (num4 < 2)
			{
				return null;
			}
			int num6 = 0;
			while (array2[num6])
			{
				num6++;
			}
			int num7 = num6;
			int num8 = num6 + 1;
			while (num8 != num6)
			{
				if (!array2[num8])
				{
					num7 = num8;
					num8 = (num8 + 1) % num2;
				}
				else
				{
					List<int> list = new List<int>
					{
						loop.Edges[num7]
					};
					int num9 = num8;
					while (array2[num8])
					{
						list.Add(loop.Edges[num8]);
						num8 = (num8 + 1) % num2;
					}
					if (list.Count > 1)
					{
						Vector3d key = array[num9];
						EdgeSpan edgeSpan = EdgeSpan.FromEdges(mesh, list);
						edgeSpan.CheckValidity(FailMode.Throw);
						foreach (KeyValuePair<Vector3d, List<EdgeSpan>> keyValuePair in dictionary)
						{
							if (keyValuePair.Key.Dot(ref key) > num)
							{
								key = keyValuePair.Key;
								break;
							}
						}
						List<EdgeSpan> list2;
						if (!dictionary.TryGetValue(key, out list2))
						{
							dictionary[key] = new List<EdgeSpan>
							{
								edgeSpan
							};
						}
						else
						{
							list2.Add(edgeSpan);
						}
					}
				}
			}
			return dictionary;
		}

		public DMesh3 Mesh;

		public double TargetEdgeLength = 2.5;

		public EdgeLoop FillLoop;

		public int[] FillTriangles;

		private enum UseFillType
		{
			PlanarFill,
			MinimalFill,
			PlanarSpansFill,
			SmoothFill
		}
	}
}
