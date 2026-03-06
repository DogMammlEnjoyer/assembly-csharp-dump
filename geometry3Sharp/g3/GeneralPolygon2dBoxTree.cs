using System;

namespace g3
{
	public class GeneralPolygon2dBoxTree
	{
		public GeneralPolygon2dBoxTree(GeneralPolygon2d poly)
		{
			this.Polygon = poly;
			this.OuterTree = new Polygon2dBoxTree(poly.Outer);
			int count = poly.Holes.Count;
			if (count > 0)
			{
				this.HoleTrees = new Polygon2dBoxTree[count];
				for (int i = 0; i < count; i++)
				{
					this.HoleTrees[i] = new Polygon2dBoxTree(poly.Holes[i]);
				}
			}
		}

		public double DistanceSquared(Vector2d pt, out int iHoleIndex, out int iNearSeg, out double fNearSegT)
		{
			iHoleIndex = -1;
			double num = this.OuterTree.SquaredDistance(pt, out iNearSeg, out fNearSegT, double.MaxValue);
			int num2 = (this.HoleTrees == null) ? 0 : this.HoleTrees.Length;
			for (int i = 0; i < num2; i++)
			{
				int num4;
				double num5;
				double num3 = this.HoleTrees[i].SquaredDistance(pt, out num4, out num5, num);
				if (num3 < num)
				{
					num = num3;
					iHoleIndex = i;
					iNearSeg = num4;
					fNearSegT = num5;
				}
			}
			return num;
		}

		public double DistanceSquared(Vector2d pt)
		{
			int num;
			int num2;
			double num3;
			return this.DistanceSquared(pt, out num, out num2, out num3);
		}

		public double Distance(Vector2d pt)
		{
			int num;
			int num2;
			double num3;
			return Math.Sqrt(this.DistanceSquared(pt, out num, out num2, out num3));
		}

		public Vector2d NearestPoint(Vector2d pt)
		{
			int iHoleIndex;
			int iSegment;
			double fSegT;
			this.DistanceSquared(pt, out iHoleIndex, out iSegment, out fSegT);
			return this.Polygon.PointAt(iSegment, fSegT, iHoleIndex);
		}

		public GeneralPolygon2d Polygon;

		private Polygon2dBoxTree OuterTree;

		private Polygon2dBoxTree[] HoleTrees;
	}
}
