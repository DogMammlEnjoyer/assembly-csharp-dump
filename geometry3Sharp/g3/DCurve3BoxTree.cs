using System;
using System.Collections.Generic;

namespace g3
{
	public class DCurve3BoxTree
	{
		public DCurve3BoxTree(DCurve3 curve)
		{
			this.Curve = curve;
			this.build_sequential(curve);
		}

		public double DistanceSquared(Vector3d pt)
		{
			int num;
			double num2;
			return this.SquaredDistance(pt, out num, out num2, double.MaxValue);
		}

		public double Distance(Vector3d pt)
		{
			int num;
			double num2;
			return Math.Sqrt(this.SquaredDistance(pt, out num, out num2, double.MaxValue));
		}

		public Vector3d NearestPoint(Vector3d pt)
		{
			int iSegment;
			double fSegT;
			this.SquaredDistance(pt, out iSegment, out fSegT, double.MaxValue);
			return this.Curve.PointAt(iSegment, fSegT);
		}

		public double SquaredDistance(Vector3d pt, out int iNearSeg, out double fNearSegT, double max_dist = 1.7976931348623157E+308)
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

		private void find_min_distance(ref Vector3d pt, ref double min_dist, ref int min_dist_seg, ref double min_dist_segt, int bi, int iLayerStart, int iLayer)
		{
			if (iLayer == 0)
			{
				int num = 2 * bi;
				double num3;
				double num2 = this.Curve.GetSegment(num).DistanceSquared(pt, out num3);
				if (num2 <= min_dist)
				{
					min_dist = num2;
					min_dist_seg = num;
					min_dist_segt = num3;
				}
				if (num + 1 < this.Curve.SegmentCount)
				{
					num2 = this.Curve.GetSegment(num + 1).DistanceSquared(pt, out num3);
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

		public double SquaredDistance(Ray3d ray, out int iNearSeg, out double fNearSegT, out double fRayT, double max_dist = 1.7976931348623157E+308)
		{
			int iLayerStart = this.boxes.Length - 1;
			int iLayer = this.layers - 1;
			double result = max_dist;
			iNearSeg = -1;
			fNearSegT = 0.0;
			fRayT = double.MaxValue;
			this.find_min_distance(ref ray, ref result, ref iNearSeg, ref fNearSegT, ref fRayT, 0, iLayerStart, iLayer);
			if (iNearSeg == -1)
			{
				return double.MaxValue;
			}
			return result;
		}

		private void find_min_distance(ref Ray3d ray, ref double min_dist, ref int min_dist_seg, ref double min_dist_segt, ref double min_dist_rayt, int bi, int iLayerStart, int iLayer)
		{
			if (iLayer == 0)
			{
				int num = 2 * bi;
				Segment3d segment = this.Curve.GetSegment(num);
				double num3;
				double num4;
				double num2 = Math.Sqrt(DistRay3Segment3.SquaredDistance(ref ray, ref segment, out num3, out num4));
				if (num2 <= min_dist)
				{
					min_dist = num2;
					min_dist_seg = num;
					min_dist_segt = num4;
					min_dist_rayt = num3;
				}
				if (num + 1 < this.Curve.SegmentCount)
				{
					Segment3d segment2 = this.Curve.GetSegment(num + 1);
					num2 = Math.Sqrt(DistRay3Segment3.SquaredDistance(ref ray, ref segment2, out num3, out num4));
					if (num2 <= min_dist)
					{
						min_dist = num2;
						min_dist_seg = num + 1;
						min_dist_segt = num4;
						min_dist_rayt = num3;
					}
				}
				return;
			}
			int num5 = iLayer - 1;
			int num6 = this.layer_counts[num5];
			int num7 = iLayerStart - num6;
			int num8 = num7 + 2 * bi;
			if (IntrRay3Box3.Intersects(ref ray, ref this.boxes[num8], min_dist))
			{
				this.find_min_distance(ref ray, ref min_dist, ref min_dist_seg, ref min_dist_segt, ref min_dist_rayt, 2 * bi, num7, num5);
			}
			if (2 * bi + 1 >= num6)
			{
				return;
			}
			int num9 = num8 + 1;
			if (IntrRay3Box3.Intersects(ref ray, ref this.boxes[num9], min_dist))
			{
				this.find_min_distance(ref ray, ref min_dist, ref min_dist_seg, ref min_dist_segt, ref min_dist_rayt, 2 * bi + 1, num7, num5);
			}
		}

		public bool FindClosestRayIntersction(Ray3d ray, double radius, out int hitSegment, out double fRayT)
		{
			int iLayerStart = this.boxes.Length - 1;
			int iLayer = this.layers - 1;
			hitSegment = -1;
			fRayT = double.MaxValue;
			this.find_closest_ray_intersction(ref ray, radius, ref hitSegment, ref fRayT, 0, iLayerStart, iLayer);
			return hitSegment != -1;
		}

		private void find_closest_ray_intersction(ref Ray3d ray, double radius, ref int nearestSegment, ref double nearest_ray_t, int bi, int iLayerStart, int iLayer)
		{
			if (iLayer == 0)
			{
				int num = 2 * bi;
				Segment3d segment = this.Curve.GetSegment(num);
				double num2;
				double num3;
				if (DistRay3Segment3.SquaredDistance(ref ray, ref segment, out num2, out num3) <= radius * radius && num2 < nearest_ray_t)
				{
					nearestSegment = num;
					nearest_ray_t = num2;
				}
				if (num + 1 < this.Curve.SegmentCount)
				{
					Segment3d segment2 = this.Curve.GetSegment(num + 1);
					if (DistRay3Segment3.SquaredDistance(ref ray, ref segment2, out num2, out num3) <= radius * radius && num2 < nearest_ray_t)
					{
						nearestSegment = num + 1;
						nearest_ray_t = num2;
					}
				}
				return;
			}
			int num4 = iLayer - 1;
			int num5 = this.layer_counts[num4];
			int num6 = iLayerStart - num5;
			int num7 = num6 + 2 * bi;
			if (IntrRay3Box3.Intersects(ref ray, ref this.boxes[num7], radius))
			{
				this.find_closest_ray_intersction(ref ray, radius, ref nearestSegment, ref nearest_ray_t, 2 * bi, num6, num4);
			}
			if (2 * bi + 1 >= num5)
			{
				return;
			}
			int num8 = num7 + 1;
			if (IntrRay3Box3.Intersects(ref ray, ref this.boxes[num8], radius))
			{
				this.find_closest_ray_intersction(ref ray, radius, ref nearestSegment, ref nearest_ray_t, 2 * bi + 1, num6, num4);
			}
		}

		private void build_sequential(DCurve3 curve)
		{
			int vertexCount = curve.VertexCount;
			int i = curve.Closed ? vertexCount : (vertexCount - 1);
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
			if (this.layers == 0)
			{
				this.layers = 1;
				num = 1;
				this.layer_counts = new List<int>
				{
					1
				};
			}
			this.boxes = new Box3d[num];
			num2 = 0;
			int num4 = curve.Closed ? vertexCount : (vertexCount - 1);
			for (int j = 0; j < num4; j += 2)
			{
				Vector3d vector3d = curve[(j + 1) % vertexCount];
				Segment3d seg = new Segment3d(curve[j], vector3d);
				Box3d box3d = new Box3d(seg);
				if (j < vertexCount - 1)
				{
					Segment3d seg2 = new Segment3d(vector3d, curve[(j + 2) % vertexCount]);
					Box3d box3d2 = new Box3d(seg2);
					box3d = Box3d.Merge(ref box3d, ref box3d2);
				}
				this.boxes[num2++] = box3d;
			}
			i = num2;
			if (i == 1)
			{
				return;
			}
			int num5 = 0;
			bool flag = false;
			while (!flag)
			{
				int num6 = num2;
				for (int k = 0; k < i; k += 2)
				{
					Box3d box3d3 = Box3d.Merge(ref this.boxes[num5 + k], ref this.boxes[num5 + k + 1]);
					this.boxes[num2++] = box3d3;
				}
				i = i / 2 + ((i % 2 == 0) ? 0 : 1);
				num5 = num6;
				if (i == 1)
				{
					flag = true;
				}
			}
		}

		public DCurve3 Curve;

		private Box3d[] boxes;

		private int layers;

		private List<int> layer_counts;
	}
}
