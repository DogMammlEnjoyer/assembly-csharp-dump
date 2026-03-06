using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using andywiecko.BurstTriangulator;
using Unity.Collections;
using Unity.Mathematics;

namespace g3
{
	public class GeneralPolygon2d : IDuplicatable<GeneralPolygon2d>
	{
		public GeneralPolygon2d()
		{
		}

		public GeneralPolygon2d(Polygon2d outer)
		{
			this.Outer = outer;
		}

		public GeneralPolygon2d(GeneralPolygon2d copy)
		{
			this.outer = new Polygon2d(copy.outer);
			this.bOuterIsCW = copy.bOuterIsCW;
			this.holes = new List<Polygon2d>();
			foreach (Polygon2d copy2 in copy.holes)
			{
				this.holes.Add(new Polygon2d(copy2));
			}
		}

		public GeneralPolygon2d(IEnumerable<DCurve3> curves, out Frame3f frame, out IEnumerable<Vector3d> AllVerticesItr)
		{
			OrthogonalPlaneFit3 orthogonalPlaneFit = new OrthogonalPlaneFit3(curves.ElementAt(0).Vertices);
			frame = new Frame3f(orthogonalPlaneFit.Origin, orthogonalPlaneFit.Normal);
			AllVerticesItr = null;
			int num = 0;
			foreach (DCurve3 dcurve in curves)
			{
				List<Vector3d> list = dcurve.Vertices.ToList<Vector3d>();
				List<Vector2d> list2 = new List<Vector2d>();
				foreach (Vector3d v in list)
				{
					Vector2f v2 = frame.ToPlaneUV((Vector3f)v, 3);
					if (num != 0 && !this.Outer.Contains(v2))
					{
						break;
					}
					list2.Add(v2);
				}
				Polygon2d polygon2d = new Polygon2d(list2);
				if (num == 0)
				{
					polygon2d = new Polygon2d(list2);
					polygon2d.Reverse();
					this.Outer = polygon2d;
					list.Reverse();
					AllVerticesItr = list;
				}
				else
				{
					try
					{
						try
						{
							this.AddHole(polygon2d, true, true);
							AllVerticesItr = AllVerticesItr.Concat(list);
						}
						catch (Exception)
						{
							polygon2d.Reverse();
							this.AddHole(polygon2d, true, true);
							list.Reverse();
							AllVerticesItr = AllVerticesItr.Concat(list);
						}
					}
					catch (Exception)
					{
					}
				}
				num++;
			}
		}

		public virtual GeneralPolygon2d Duplicate()
		{
			return new GeneralPolygon2d(this);
		}

		public Polygon2d Outer
		{
			get
			{
				return this.outer;
			}
			set
			{
				this.outer = value;
				this.bOuterIsCW = this.outer.IsClockwise;
			}
		}

		public void AddHole(Polygon2d hole, bool bCheckContainment = true, bool bCheckOrientation = true)
		{
			if (this.outer == null)
			{
				throw new Exception("GeneralPolygon2d.AddHole: outer polygon not set!");
			}
			if (bCheckContainment)
			{
				if (!this.outer.Contains(hole))
				{
					throw new Exception("GeneralPolygon2d.AddHole: outer does not contain hole!");
				}
				foreach (Polygon2d o in this.holes)
				{
					if (hole.Intersects(o))
					{
						throw new Exception("GeneralPolygon2D.AddHole: new hole intersects existing hole!");
					}
				}
			}
			if (bCheckOrientation && ((this.bOuterIsCW && hole.IsClockwise) || (!this.bOuterIsCW && !hole.IsClockwise)))
			{
				throw new Exception("GeneralPolygon2D.AddHole: new hole has same orientation as outer polygon!");
			}
			this.holes.Add(hole);
		}

		public void ClearHoles()
		{
			this.holes.Clear();
		}

		private bool HasHoles
		{
			get
			{
				return this.holes.Count > 0;
			}
		}

		public ReadOnlyCollection<Polygon2d> Holes
		{
			get
			{
				return this.holes.AsReadOnly();
			}
		}

		public double Area
		{
			get
			{
				double num = this.bOuterIsCW ? -1.0 : 1.0;
				double num2 = num * this.Outer.SignedArea;
				foreach (Polygon2d polygon2d in this.holes)
				{
					num2 += num * polygon2d.SignedArea;
				}
				return num2;
			}
		}

		public double HoleArea
		{
			get
			{
				double num = 0.0;
				foreach (Polygon2d polygon2d in this.Holes)
				{
					num += Math.Abs(polygon2d.SignedArea);
				}
				return num;
			}
		}

		public double Perimeter
		{
			get
			{
				double num = this.outer.Perimeter;
				foreach (Polygon2d polygon2d in this.holes)
				{
					num += polygon2d.Perimeter;
				}
				return num;
			}
		}

		public AxisAlignedBox2d Bounds
		{
			get
			{
				AxisAlignedBox2d bounds = this.outer.GetBounds();
				foreach (Polygon2d polygon2d in this.holes)
				{
					bounds.Contain(polygon2d.GetBounds());
				}
				return bounds;
			}
		}

		public int VertexCount
		{
			get
			{
				int num = this.outer.VertexCount;
				foreach (Polygon2d polygon2d in this.holes)
				{
					num += polygon2d.VertexCount;
				}
				return num;
			}
		}

		public void Translate(Vector2d translate)
		{
			this.outer.Translate(translate);
			foreach (Polygon2d polygon2d in this.holes)
			{
				polygon2d.Translate(translate);
			}
		}

		public void Rotate(Matrix2d rotation, Vector2d origin)
		{
			this.outer.Rotate(rotation, origin);
			foreach (Polygon2d polygon2d in this.holes)
			{
				polygon2d.Rotate(rotation, origin);
			}
		}

		public void Scale(Vector2d scale, Vector2d origin)
		{
			this.outer.Scale(scale, origin);
			foreach (Polygon2d polygon2d in this.holes)
			{
				polygon2d.Scale(scale, origin);
			}
		}

		public void Transform(Func<Vector2d, Vector2d> transformF)
		{
			this.outer.Transform(transformF);
			foreach (Polygon2d polygon2d in this.holes)
			{
				polygon2d.Transform(transformF);
			}
		}

		public void Reverse()
		{
			this.Outer.Reverse();
			this.bOuterIsCW = this.Outer.IsClockwise;
			foreach (Polygon2d polygon2d in this.Holes)
			{
				polygon2d.Reverse();
			}
		}

		public bool Contains(Vector2d vTest)
		{
			if (!this.outer.Contains(vTest))
			{
				return false;
			}
			using (List<Polygon2d>.Enumerator enumerator = this.holes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Contains(vTest))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool Contains(Polygon2d poly)
		{
			if (!this.outer.Contains(poly))
			{
				return false;
			}
			using (List<Polygon2d>.Enumerator enumerator = this.holes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Contains(poly))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool Contains(Segment2d seg)
		{
			if (!this.outer.Contains(seg))
			{
				return false;
			}
			using (List<Polygon2d>.Enumerator enumerator = this.holes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Intersects(seg))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool Intersects(Polygon2d poly)
		{
			if (this.outer.Intersects(poly))
			{
				return true;
			}
			using (List<Polygon2d>.Enumerator enumerator = this.holes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Intersects(poly))
					{
						return true;
					}
				}
			}
			return false;
		}

		public Vector2d PointAt(int iSegment, double fSegT, int iHoleIndex = -1)
		{
			if (iHoleIndex == -1)
			{
				return this.outer.PointAt(iSegment, fSegT);
			}
			return this.holes[iHoleIndex].PointAt(iSegment, fSegT);
		}

		public Segment2d Segment(int iSegment, int iHoleIndex = -1)
		{
			if (iHoleIndex == -1)
			{
				return this.outer.Segment(iSegment);
			}
			return this.holes[iHoleIndex].Segment(iSegment);
		}

		public Vector2d GetNormal(int iSegment, double segT, int iHoleIndex = -1)
		{
			if (iHoleIndex == -1)
			{
				return this.outer.GetNormal(iSegment, segT);
			}
			return this.holes[iHoleIndex].GetNormal(iSegment, segT);
		}

		public double DistanceSquared(Vector2d p, out int iHoleIndex, out int iNearSeg, out double fNearSegT)
		{
			iNearSeg = (iHoleIndex = -1);
			fNearSegT = double.MaxValue;
			double num = this.outer.DistanceSquared(p, out iNearSeg, out fNearSegT);
			for (int i = 0; i < this.Holes.Count; i++)
			{
				int num3;
				double num4;
				double num2 = this.Holes[i].DistanceSquared(p, out num3, out num4);
				if (num2 < num)
				{
					num = num2;
					iHoleIndex = i;
					iNearSeg = num3;
					fNearSegT = num4;
				}
			}
			return num;
		}

		public IEnumerable<Segment2d> AllSegmentsItr()
		{
			foreach (Segment2d segment2d in this.outer.SegmentItr())
			{
				yield return segment2d;
			}
			IEnumerator<Segment2d> enumerator = null;
			foreach (Polygon2d polygon2d in this.holes)
			{
				foreach (Segment2d segment2d2 in polygon2d.SegmentItr())
				{
					yield return segment2d2;
				}
				enumerator = null;
			}
			List<Polygon2d>.Enumerator enumerator2 = default(List<Polygon2d>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerable<Vector2d> AllVerticesItr()
		{
			foreach (Vector2d vector2d in this.outer.Vertices)
			{
				yield return vector2d;
			}
			IEnumerator<Vector2d> enumerator = null;
			foreach (Polygon2d polygon2d in this.holes)
			{
				foreach (Vector2d vector2d2 in polygon2d.Vertices)
				{
					yield return vector2d2;
				}
				enumerator = null;
			}
			List<Polygon2d>.Enumerator enumerator2 = default(List<Polygon2d>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerable<Index2i> AllEdgesItr()
		{
			int i = this.Outer.VertexCount;
			int num;
			for (int j = 0; j < i; j = num + 1)
			{
				yield return new Index2i(j, (j != i - 1) ? (j + 1) : 0);
				num = j;
			}
			foreach (Polygon2d hole in this.Holes)
			{
				for (int j = 0; j < hole.VertexCount; j = num + 1)
				{
					yield return new Index2i(i + j, (j != hole.VertexCount - 1) ? (i + j + 1) : i);
					num = j;
				}
				i += hole.VertexCount;
				hole = null;
			}
			IEnumerator<Polygon2d> enumerator = null;
			yield break;
			yield break;
		}

		private NativeArray<int> ToEdges()
		{
			int vertexCount = this.VertexCount;
			NativeArray<int> result = new NativeArray<int>(vertexCount * 2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num = 0;
			foreach (Index2i index2i in this.AllEdgesItr())
			{
				result[num] = index2i.a;
				num++;
				result[num] = index2i.b;
				num++;
			}
			return result;
		}

		private NativeArray<float2> ToHoleSeeds()
		{
			List<Vector2d> list = new List<Vector2d>();
			foreach (Polygon2d polygon2d in this.Holes)
			{
				list.Add(polygon2d.PointInPolygon());
			}
			return new NativeArray<float2>((from vertex in list
			select (float2)vertex).ToArray<float2>(), Allocator.Persistent);
		}

		public Index3i[] GetMesh()
		{
			Triangulator triangulator = new Triangulator(Allocator.Persistent);
			triangulator.Input.Positions = new NativeArray<float2>((from vertex in this.AllVerticesItr()
			select (float2)vertex).ToArray<float2>(), Allocator.Persistent);
			triangulator.Input.ConstraintEdges = this.ToEdges();
			triangulator.Input.HoleSeeds = this.ToHoleSeeds();
			triangulator.Settings.RestoreBoundary = true;
			triangulator.Settings.ConstrainEdges = true;
			Triangulator triangulator2 = triangulator;
			Index3i[] result;
			try
			{
				triangulator2.Run();
				if (!triangulator2.Output.Status.IsCreated || triangulator2.Output.Status.Value != Triangulator.Status.OK)
				{
					throw new Exception("Could not create Delaunay Triangulation");
				}
				int[] array = triangulator2.Output.Triangles.AsArray().ToArray();
				int num = array.Length / 3;
				Index3i[] array2 = new Index3i[num];
				long num2 = 0L;
				for (int i = 0; i < num; i++)
				{
					Index3i[] array3 = array2;
					int num3 = i;
					int[] array4 = array;
					long num4 = num2;
					num2 = num4 + 1L;
					int ii = array4[(int)(checked((IntPtr)num4))];
					int[] array5 = array;
					long num5 = num2;
					num2 = num5 + 1L;
					int jj = array5[(int)(checked((IntPtr)num5))];
					int[] array6 = array;
					long num6 = num2;
					num2 = num6 + 1L;
					array3[num3] = new Index3i(ii, jj, array6[(int)(checked((IntPtr)num6))]);
				}
				triangulator2.Input.Positions.Dispose();
				triangulator2.Input.ConstraintEdges.Dispose();
				triangulator2.Input.HoleSeeds.Dispose();
				triangulator2.Dispose();
				result = array2;
			}
			catch
			{
				triangulator2.Input.Positions.Dispose();
				triangulator2.Input.ConstraintEdges.Dispose();
				triangulator2.Input.HoleSeeds.Dispose();
				triangulator2.Dispose();
				result = null;
			}
			return result;
		}

		public void Simplify(double clusterTol = 0.0001, double lineDeviationTol = 0.01, bool bSimplifyStraightLines = true)
		{
			this.Outer.Simplify(clusterTol, lineDeviationTol, bSimplifyStraightLines);
			foreach (Polygon2d polygon2d in this.holes)
			{
				polygon2d.Simplify(clusterTol, lineDeviationTol, bSimplifyStraightLines);
			}
		}

		public bool IsOutside(Segment2d seg)
		{
			bool flag = true;
			if (this.Outer.IsMember(seg, out flag))
			{
				return flag;
			}
			using (IEnumerator<Polygon2d> enumerator = this.Holes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsMember(seg, out flag))
					{
						if (flag)
						{
							return true;
						}
						return false;
					}
				}
			}
			return false;
		}

		private Polygon2d outer;

		private bool bOuterIsCW;

		private List<Polygon2d> holes = new List<Polygon2d>();
	}
}
