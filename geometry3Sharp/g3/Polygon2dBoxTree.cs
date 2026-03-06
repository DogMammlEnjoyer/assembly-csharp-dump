using System;
using System.Collections.Generic;

namespace g3
{
	public class Polygon2dBoxTree
	{
		public Polygon2dBoxTree(Polygon2d poly)
		{
			this.Polygon = poly;
			this.build_sequential(poly);
		}

		public double DistanceSquared(Vector2d pt)
		{
			int num;
			double num2;
			return this.SquaredDistance(pt, out num, out num2, double.MaxValue);
		}

		public double Distance(Vector2d pt)
		{
			int num;
			double num2;
			return Math.Sqrt(this.SquaredDistance(pt, out num, out num2, double.MaxValue));
		}

		public Vector2d NearestPoint(Vector2d pt)
		{
			int iSegment;
			double fSegT;
			this.SquaredDistance(pt, out iSegment, out fSegT, double.MaxValue);
			return this.Polygon.PointAt(iSegment, fSegT);
		}

		public double SquaredDistance(Vector2d pt, out int iNearSeg, out double fNearSegT, double max_dist = 1.7976931348623157E+308)
		{
			int iLayerStart = this.boxes.Length - 1;
			int iLayer = this.layers - 1;
			double result = max_dist;
			iNearSeg = -1;
			fNearSegT = 0.0;
			this.find_min_distance(ref pt, ref result, ref iNearSeg, ref fNearSegT, 0, iLayerStart, iLayer);
			if (iNearSeg == -1)
			{
				return double.MaxValue;
			}
			return result;
		}

		private void find_min_distance(ref Vector2d pt, ref double min_dist, ref int min_dist_seg, ref double min_dist_segt, int bi, int iLayerStart, int iLayer)
		{
			if (iLayer == 0)
			{
				int num = 2 * bi;
				double num3;
				double num2 = this.Polygon.Segment(num).DistanceSquared(pt, out num3);
				if (num2 <= min_dist)
				{
					min_dist = num2;
					min_dist_seg = num;
					min_dist_segt = num3;
				}
				if (num + 1 < this.Polygon.VertexCount)
				{
					num2 = this.Polygon.Segment(num + 1).DistanceSquared(pt, out num3);
					if (num2 <= min_dist)
					{
						min_dist = num2;
						min_dist_seg = num + 1;
						min_dist_segt = num3;
					}
				}
				return;
			}
			int num4 = iLayer - 1;
			int num5 = this.layer_counts[num4];
			int num6 = iLayerStart - num5;
			int num7 = num6 + 2 * bi;
			if (this.boxes[num7].DistanceSquared(pt) <= min_dist)
			{
				this.find_min_distance(ref pt, ref min_dist, ref min_dist_seg, ref min_dist_segt, 2 * bi, num6, num4);
			}
			if (2 * bi + 1 >= num5)
			{
				return;
			}
			int num8 = num7 + 1;
			if (this.boxes[num8].DistanceSquared(pt) <= min_dist)
			{
				this.find_min_distance(ref pt, ref min_dist, ref min_dist_seg, ref min_dist_segt, 2 * bi + 1, num6, num4);
			}
		}

		private void build_sequential(Polygon2d poly)
		{
			int vertexCount = poly.VertexCount;
			int i = vertexCount;
			int num = 0;
			this.layers = 0;
			this.layer_counts = new List<int>();
			int num2 = 0;
			while (i > 1)
			{
				int num3 = i / 2 + ((i % 2 == 0) ? 0 : 1);
				num += num3;
				i = num3;
				this.layer_counts.Add(num3);
				num2 += num3;
				this.layers++;
			}
			this.boxes = new Box2d[num];
			num2 = 0;
			for (int j = 0; j < vertexCount; j += 2)
			{
				Vector2d vector2d = poly[(j + 1) % vertexCount];
				Segment2d seg = new Segment2d(poly[j], vector2d);
				Box2d box2d = new Box2d(seg);
				if (j < vertexCount - 1)
				{
					Segment2d seg2 = new Segment2d(vector2d, poly[(j + 2) % vertexCount]);
					Box2d box2d2 = new Box2d(seg2);
					box2d = Box2d.Merge(ref box2d, ref box2d2);
				}
				this.boxes[num2++] = box2d;
			}
			i = num2;
			int num4 = 0;
			bool flag = false;
			while (!flag)
			{
				int num5 = num2;
				for (int k = 0; k < i; k += 2)
				{
					Box2d box2d3 = Box2d.Merge(ref this.boxes[num4 + k], ref this.boxes[num4 + k + 1]);
					this.boxes[num2++] = box2d3;
				}
				i = i / 2 + ((i % 2 == 0) ? 0 : 1);
				num4 = num5;
				if (i == 1)
				{
					flag = true;
				}
			}
		}

		public Polygon2d Polygon;

		private Box2d[] boxes;

		private int layers;

		private List<int> layer_counts;
	}
}
