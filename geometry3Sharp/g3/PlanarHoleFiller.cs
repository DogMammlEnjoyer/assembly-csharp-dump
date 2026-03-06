using System;
using System.Collections.Generic;

namespace g3
{
	public class PlanarHoleFiller
	{
		public PlanarHoleFiller(DMesh3 mesh)
		{
			this.Mesh = mesh;
			this.Bounds = AxisAlignedBox2d.Empty;
		}

		public PlanarHoleFiller(MeshPlaneCut cut)
		{
			this.Mesh = cut.Mesh;
			this.AddFillLoops(cut.CutLoops);
			this.SetPlane(cut.PlaneOrigin, cut.PlaneNormal);
		}

		public void SetPlane(Vector3d origin, Vector3d normal)
		{
			this.PlaneOrigin = origin;
			this.PlaneNormal = normal;
			Vector3d.ComputeOrthogonalComplement(1, this.PlaneNormal, ref this.PlaneX, ref this.PlaneY);
		}

		public void SetPlane(Vector3d origin, Vector3d normal, Vector3d planeX, Vector3d planeY)
		{
			this.PlaneOrigin = origin;
			this.PlaneNormal = normal;
			this.PlaneX = planeX;
			this.PlaneY = planeY;
		}

		public void AddFillLoop(EdgeLoop loop)
		{
			this.Loops.Add(new PlanarHoleFiller.FillLoop
			{
				edgeLoop = loop
			});
		}

		public void AddFillLoops(IEnumerable<EdgeLoop> loops)
		{
			foreach (EdgeLoop loop in loops)
			{
				this.AddFillLoop(loop);
			}
		}

		public bool Fill()
		{
			this.compute_polygons();
			Vector2d shiftOrigin = this.Bounds.Center;
			double scale = 1.0 / this.Bounds.MaxDim;
			foreach (PlanarHoleFiller.FillLoop fillLoop in this.Loops)
			{
				fillLoop.poly.Translate(-shiftOrigin);
				fillLoop.poly.Scale(scale * Vector2d.One, Vector2d.Zero);
			}
			Dictionary<PlanarComplex.Element, int> dictionary = new Dictionary<PlanarComplex.Element, int>();
			PlanarComplex planarComplex = new PlanarComplex();
			for (int i = 0; i < this.Loops.Count; i++)
			{
				PlanarComplex.Element key = planarComplex.Add(this.Loops[i].poly);
				dictionary[key] = i;
			}
			PlanarComplex.SolidRegionInfo solidRegionInfo = planarComplex.FindSolidRegions(PlanarComplex.FindSolidsOptions.SortPolygons);
			List<Index2i> list = new List<Index2i>();
			List<Index2i> list2 = new List<Index2i>();
			Func<Vector3d, Vector3d> <>9__0;
			for (int j = 0; j < solidRegionInfo.Polygons.Count; j++)
			{
				GeneralPolygon2d generalPolygon2d = solidRegionInfo.Polygons[j];
				PlanarComplex.GeneralSolid generalSolid = solidRegionInfo.PolygonsSources[j];
				float num = 1.5f;
				int num2 = 0;
				if (this.FillTargetEdgeLen < 1.7976931348623157E+308 && this.FillTargetEdgeLen > 0.0)
				{
					int num3 = (int)((double)(num / (float)scale) / this.FillTargetEdgeLen) + 1;
					num2 = ((num3 <= 1) ? 0 : num3);
				}
				MeshGenerator meshGenerator;
				if (num2 == 0)
				{
					meshGenerator = new TrivialRectGenerator
					{
						IndicesMap = new Index2i(1, 2),
						Width = num,
						Height = num
					};
				}
				else
				{
					meshGenerator = new GriddedRectGenerator
					{
						IndicesMap = new Index2i(1, 2),
						Width = num,
						Height = num,
						EdgeVertices = num2
					};
				}
				DMesh3 dmesh = meshGenerator.Generate().MakeDMesh();
				dmesh.ReverseOrientation(true);
				List<Polygon2d> list3 = new List<Polygon2d>
				{
					generalPolygon2d.Outer
				};
				list3.AddRange(generalPolygon2d.Holes);
				int[][] array = new int[list3.Count][];
				for (int k = 0; k < list3.Count; k++)
				{
					MeshInsertUVPolyCurve meshInsertUVPolyCurve = new MeshInsertUVPolyCurve(dmesh, list3[k]);
					bool flag = meshInsertUVPolyCurve.Validate(9.999999974752427E-07 * scale) != ValidationStatus.Ok;
					bool flag2 = true;
					if (!flag && meshInsertUVPolyCurve.Apply())
					{
						meshInsertUVPolyCurve.Simplify();
						array[k] = meshInsertUVPolyCurve.CurveVertices;
						flag2 = (meshInsertUVPolyCurve.Loops.Count != 1 || meshInsertUVPolyCurve.Loops[0].VertexCount != list3[k].VertexCount);
					}
					if (flag2)
					{
						list.Add(new Index2i(j, k));
					}
				}
				List<int> list4 = new List<int>();
				foreach (int num4 in dmesh.TriangleIndices())
				{
					if (!generalPolygon2d.Contains(dmesh.GetTriCentroid(num4).xy))
					{
						list4.Add(num4);
					}
				}
				foreach (int tID in list4)
				{
					dmesh.RemoveTriangle(tID, true, false);
				}
				IDeformableMesh mesh = dmesh;
				Func<Vector3d, Vector3d> transformF;
				if ((transformF = <>9__0) == null)
				{
					transformF = (<>9__0 = delegate(Vector3d v)
					{
						Vector2d vector2d = v.xy;
						vector2d /= scale;
						vector2d += shiftOrigin;
						return this.to3D(vector2d);
					});
				}
				MeshTransforms.PerVertexTransform(mesh, transformF);
				IndexMap mergeMapV = new IndexMap(true, -1);
				if (this.MergeFillBoundary)
				{
					for (int l = 0; l < list3.Count; l++)
					{
						if (array[l] != null)
						{
							int[] array2 = array[l];
							int num5 = array2.Length;
							PlanarComplex.Element key2 = (l == 0) ? generalSolid.Outer : generalSolid.Holes[l - 1];
							int index = dictionary[key2];
							EdgeLoop edgeLoop = this.Loops[index].edgeLoop;
							List<int> list5 = this.build_merge_map(dmesh, array2, this.Mesh, edgeLoop.Vertices, 9.999999974752427E-07, mergeMapV);
							if (list5 != null && list5.Count > 0)
							{
								list.Add(new Index2i(j, l));
								this.OutputHasCracks = true;
							}
						}
					}
				}
				int[] array3;
				new MeshEditor(this.Mesh).AppendMesh(dmesh, mergeMapV, out array3, this.Mesh.AllocateTriangleGroup());
			}
			this.FailedInsertions = list.Count;
			this.FailedMerges = list2.Count;
			return list.Count <= 0 && list2.Count <= 0;
		}

		private List<int> build_merge_map(DMesh3 fillMesh, int[] fillLoopV, DMesh3 targetMesh, int[] targetLoopV, double tol, IndexMap mergeMapV)
		{
			if (fillLoopV.Length == targetLoopV.Length && this.build_merge_map_simple(fillMesh, fillLoopV, targetMesh, targetLoopV, tol, mergeMapV))
			{
				return null;
			}
			int num = fillLoopV.Length;
			int num2 = targetLoopV.Length;
			bool[] array = new bool[num];
			new bool[num2];
			new int[num];
			new int[num2];
			List<int> list = new List<int>();
			SmallListSet smallListSet = new SmallListSet();
			smallListSet.Resize(num);
			double num3 = tol * tol;
			for (int i = 0; i < num; i++)
			{
				if (!fillMesh.IsVertex(fillLoopV[i]))
				{
					array[i] = true;
					list.Add(i);
				}
				else
				{
					smallListSet.AllocateAt(i);
					Vector3d vertex = fillMesh.GetVertex(fillLoopV[i]);
					for (int j = 0; j < num2; j++)
					{
						Vector3d vertex2 = targetMesh.GetVertex(targetLoopV[j]);
						if (vertex.DistanceSquared(ref vertex2) < num3)
						{
							smallListSet.Insert(i, j);
						}
					}
				}
			}
			for (int k = 0; k < num; k++)
			{
				if (!array[k] && smallListSet.Count(k) == 1)
				{
					int num4 = smallListSet.First(k);
					mergeMapV[fillLoopV[k]] = targetLoopV[num4];
					array[k] = true;
				}
			}
			for (int l = 0; l < num; l++)
			{
				if (!array[l])
				{
					list.Add(l);
				}
			}
			return list;
		}

		private bool build_merge_map_simple(DMesh3 fillMesh, int[] fillLoopV, DMesh3 targetMesh, int[] targetLoopV, double tol, IndexMap mergeMapV)
		{
			if (fillLoopV.Length != targetLoopV.Length)
			{
				return false;
			}
			int num = fillLoopV.Length;
			for (int i = 0; i < num; i++)
			{
				if (!fillMesh.IsVertex(fillLoopV[i]))
				{
					return false;
				}
				Vector3d vertex = fillMesh.GetVertex(fillLoopV[i]);
				Vector3d vertex2 = this.Mesh.GetVertex(targetLoopV[i]);
				if (vertex.Distance(vertex2) > tol)
				{
					return false;
				}
			}
			for (int j = 0; j < num; j++)
			{
				mergeMapV[fillLoopV[j]] = targetLoopV[j];
			}
			return true;
		}

		private void compute_polygons()
		{
			this.Bounds = AxisAlignedBox2d.Empty;
			for (int i = 0; i < this.Loops.Count; i++)
			{
				EdgeLoop edgeLoop = this.Loops[i].edgeLoop;
				Polygon2d polygon2d = new Polygon2d();
				foreach (int vID in edgeLoop.Vertices)
				{
					Vector2d v = this.to2D(this.Mesh.GetVertex(vID));
					polygon2d.AppendVertex(v);
				}
				this.Loops[i].poly = polygon2d;
				this.Bounds.Contain(polygon2d.Bounds);
			}
		}

		private bool inPolygon(Vector2d v2, List<GeneralPolygon2d> polys, bool all = false)
		{
			int num = 0;
			using (List<GeneralPolygon2d>.Enumerator enumerator = polys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Contains(v2))
					{
						if (!all)
						{
							return true;
						}
						num++;
					}
				}
			}
			return all && num == polys.Count;
		}

		private Vector2d to2D(Vector3d v)
		{
			Vector3d vector3d = v - this.PlaneOrigin;
			vector3d -= vector3d.Dot(this.PlaneNormal) * this.PlaneNormal;
			return new Vector2d(this.PlaneX.Dot(vector3d), this.PlaneY.Dot(vector3d));
		}

		private Vector3d to3D(Vector2d v)
		{
			return this.PlaneOrigin + this.PlaneX * v.x + this.PlaneY * v.y;
		}

		public DMesh3 Mesh;

		public Vector3d PlaneOrigin;

		public Vector3d PlaneNormal;

		public double FillTargetEdgeLen = double.MaxValue;

		public bool MergeFillBoundary = true;

		public bool OutputHasCracks;

		public int FailedInsertions;

		public int FailedMerges;

		private Vector3d PlaneX;

		private Vector3d PlaneY;

		private List<PlanarHoleFiller.FillLoop> Loops = new List<PlanarHoleFiller.FillLoop>();

		private AxisAlignedBox2d Bounds;

		private class FillLoop
		{
			public EdgeLoop edgeLoop;

			public Polygon2d poly;
		}
	}
}
