using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal class ConfinerOven
	{
		public ConfinerOven(in List<List<Vector2>> inputPath, in float aspectRatio, float maxFrustumHeight, float skeletonPadding)
		{
			this.Initialize(inputPath, aspectRatio, maxFrustumHeight, Mathf.Max(0f, skeletonPadding) + 1f);
		}

		public ConfinerOven.BakedSolution GetBakedSolution(float frustumHeight)
		{
			if (this.m_Cache.userSetMaxFrustumHeight > 0f)
			{
				frustumHeight = Mathf.Min(this.m_Cache.userSetMaxFrustumHeight, frustumHeight);
			}
			if (this.State == ConfinerOven.BakingState.BAKED && frustumHeight >= this.m_Cache.theoreticalMaxFrustumHeight)
			{
				return new ConfinerOven.BakedSolution(this.m_AspectStretcher.Aspect, frustumHeight, false, this.m_PolygonRect, this.m_OriginalPolygon, this.m_Cache.theoreticalMaxCandidate);
			}
			ClipperOffset clipperOffset = new ClipperOffset(2.0, 0.0, false, false);
			clipperOffset.AddPaths(this.m_OriginalPolygon, JoinType.Miter, EndType.Polygon);
			List<List<Point64>> list = clipperOffset.Execute((double)(-1f * this.m_FloatToInt.FloatToInt(frustumHeight)));
			if (list.Count == 0)
			{
				list = this.m_Cache.theoreticalMaxCandidate;
			}
			List<List<Point64>> list2 = new List<List<Point64>>();
			if (this.State == ConfinerOven.BakingState.BAKING || this.m_Skeleton.Count == 0)
			{
				list2 = list;
			}
			else
			{
				Clipper64 clipper = new Clipper64();
				clipper.AddSubject(list);
				clipper.AddClip(this.m_Skeleton);
				clipper.Execute(ClipType.Union, FillRule.EvenOdd, list2);
			}
			return new ConfinerOven.BakedSolution(this.m_AspectStretcher.Aspect, frustumHeight, this.m_MinFrustumHeightWithBones < frustumHeight, this.m_PolygonRect, this.m_OriginalPolygon, list2);
		}

		public ConfinerOven.BakingState State { get; private set; }

		private void Initialize(in List<List<Vector2>> inputPath, in float aspectRatio, float maxFrustumHeight, float skeletonPadding)
		{
			this.m_Skeleton.Clear();
			this.m_Cache.userSetMaxFrustumHeight = maxFrustumHeight;
			this.m_MinFrustumHeightWithBones = float.MaxValue;
			this.m_SkeletonPadding = skeletonPadding;
			this.m_PolygonRect = ConfinerOven.<Initialize>g__GetPolygonBoundingBox|24_0(inputPath);
			this.m_AspectStretcher = new ConfinerOven.AspectStretcher(aspectRatio, this.m_PolygonRect.center.x);
			this.m_FloatToInt = new ConfinerOven.FloatToIntScaler(this.m_PolygonRect);
			this.m_Cache.theoreticalMaxFrustumHeight = Mathf.Max(this.m_PolygonRect.width / aspectRatio, this.m_PolygonRect.height) / 2f;
			this.m_OriginalPolygon = new List<List<Point64>>(inputPath.Count);
			for (int i = 0; i < inputPath.Count; i++)
			{
				List<Vector2> list = inputPath[i];
				int count = list.Count;
				List<Point64> list2 = new List<Point64>(count);
				for (int j = 0; j < count; j++)
				{
					Vector2 vector = this.m_AspectStretcher.Stretch(list[j]);
					list2.Add(new Point64((double)this.m_FloatToInt.FloatToInt(vector.x), (double)this.m_FloatToInt.FloatToInt(vector.y)));
				}
				this.m_OriginalPolygon.Add(list2);
			}
			this.m_MidPoint = ConfinerOven.<Initialize>g__MidPointOfIntRect|24_1(Clipper.GetBounds(this.m_OriginalPolygon));
			this.m_Cache.theoreticalMaxCandidate = new List<List<Point64>>
			{
				new List<Point64>
				{
					this.m_MidPoint
				}
			};
			if (this.m_Cache.userSetMaxFrustumHeight < 0f)
			{
				this.State = ConfinerOven.BakingState.BAKED;
				return;
			}
			this.m_Cache.offsetter = new ClipperOffset(2.0, 0.0, false, false);
			this.m_Cache.offsetter.AddPaths(this.m_OriginalPolygon, JoinType.Miter, EndType.Polygon);
			this.m_Cache.maxFrustumHeight = this.m_Cache.userSetMaxFrustumHeight;
			if (this.m_Cache.maxFrustumHeight == 0f || this.m_Cache.maxFrustumHeight > this.m_Cache.theoreticalMaxFrustumHeight)
			{
				this.m_Cache.maxFrustumHeight = this.m_Cache.theoreticalMaxFrustumHeight;
				this.m_Cache.userSetMaxCandidate = this.m_Cache.theoreticalMaxCandidate;
			}
			else
			{
				this.m_Cache.userSetMaxCandidate = new List<List<Point64>>(this.m_Cache.offsetter.Execute((double)(-1f * this.m_FloatToInt.FloatToInt(this.m_Cache.userSetMaxFrustumHeight))));
				if (this.m_Cache.userSetMaxCandidate.Count == 0)
				{
					this.m_Cache.userSetMaxCandidate = this.m_Cache.theoreticalMaxCandidate;
				}
			}
			this.m_Cache.stepSize = this.m_Cache.maxFrustumHeight;
			List<List<Point64>> polygons = new List<List<Point64>>(this.m_Cache.offsetter.Execute(0.0));
			this.m_Cache.solutions = new List<ConfinerOven.PolygonSolution>();
			this.m_Cache.solutions.Add(new ConfinerOven.PolygonSolution
			{
				polygons = polygons,
				frustumHeight = 0f
			});
			this.m_Cache.rightCandidate = default(ConfinerOven.PolygonSolution);
			this.m_Cache.leftCandidate = new ConfinerOven.PolygonSolution
			{
				polygons = polygons,
				frustumHeight = 0f
			};
			this.m_Cache.currentFrustumHeight = 0f;
			this.m_Cache.bakeTime = 0f;
			this.State = ConfinerOven.BakingState.BAKING;
			this.bakeProgress = 0f;
		}

		public void BakeConfiner(float maxComputationTimePerFrameInSeconds)
		{
			if (this.State != ConfinerOven.BakingState.BAKING)
			{
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num = this.m_FloatToInt.IntToFloat(50L);
			while (this.m_Cache.solutions.Count < 1000)
			{
				this.m_Cache.stepSize = Mathf.Min(this.m_Cache.stepSize, this.m_Cache.maxFrustumHeight - this.m_Cache.leftCandidate.frustumHeight);
				this.m_Cache.currentFrustumHeight = this.m_Cache.leftCandidate.frustumHeight + this.m_Cache.stepSize;
				List<List<Point64>> list = (Math.Abs(this.m_Cache.currentFrustumHeight - this.m_Cache.maxFrustumHeight) < 0.0001f) ? this.m_Cache.userSetMaxCandidate : this.m_Cache.offsetter.Execute((double)(-1f * this.m_FloatToInt.FloatToInt(this.m_Cache.currentFrustumHeight)));
				if (list.Count == 0)
				{
					list = this.m_Cache.userSetMaxCandidate;
				}
				if (this.m_Cache.leftCandidate.StateChanged(list))
				{
					this.m_Cache.rightCandidate = new ConfinerOven.PolygonSolution
					{
						polygons = new List<List<Point64>>(list),
						frustumHeight = this.m_Cache.currentFrustumHeight
					};
					this.m_Cache.stepSize = Mathf.Max(this.m_Cache.stepSize / 2f, num);
				}
				else
				{
					this.m_Cache.leftCandidate = new ConfinerOven.PolygonSolution
					{
						polygons = new List<List<Point64>>(list),
						frustumHeight = this.m_Cache.currentFrustumHeight
					};
					if (!this.m_Cache.rightCandidate.IsNull)
					{
						this.m_Cache.stepSize = Mathf.Max(this.m_Cache.stepSize / 2f, num);
					}
				}
				if (!this.m_Cache.rightCandidate.IsNull && this.m_Cache.stepSize <= num)
				{
					this.m_Cache.solutions.Add(this.m_Cache.leftCandidate);
					this.m_Cache.solutions.Add(this.m_Cache.rightCandidate);
					this.m_Cache.leftCandidate = this.m_Cache.rightCandidate;
					this.m_Cache.rightCandidate = default(ConfinerOven.PolygonSolution);
					this.m_Cache.stepSize = this.m_Cache.maxFrustumHeight;
				}
				else if (this.m_Cache.rightCandidate.IsNull || this.m_Cache.leftCandidate.frustumHeight >= this.m_Cache.maxFrustumHeight)
				{
					this.m_Cache.solutions.Add(this.m_Cache.leftCandidate);
					break;
				}
				float num2 = Time.realtimeSinceStartup - realtimeSinceStartup;
				if (num2 > maxComputationTimePerFrameInSeconds)
				{
					this.m_Cache.bakeTime = this.m_Cache.bakeTime + num2;
					if (this.m_Cache.bakeTime > 5f)
					{
						this.State = ConfinerOven.BakingState.TIMEOUT;
					}
					this.bakeProgress = this.m_Cache.leftCandidate.frustumHeight / this.m_Cache.maxFrustumHeight;
					return;
				}
			}
			this.<BakeConfiner>g__ComputeSkeleton|25_0(this.m_Cache.solutions);
			for (int i = this.m_Cache.solutions.Count - 1; i >= 0; i--)
			{
				if (this.m_Cache.solutions[i].polygons.Count == 0)
				{
					this.m_Cache.solutions.RemoveAt(i);
				}
			}
			this.bakeProgress = 1f;
			this.State = ConfinerOven.BakingState.BAKED;
		}

		[CompilerGenerated]
		internal static Rect <Initialize>g__GetPolygonBoundingBox|24_0(in List<List<Vector2>> polygons)
		{
			float num = float.PositiveInfinity;
			float num2 = float.NegativeInfinity;
			float num3 = float.PositiveInfinity;
			float num4 = float.NegativeInfinity;
			for (int i = 0; i < polygons.Count; i++)
			{
				for (int j = 0; j < polygons[i].Count; j++)
				{
					Vector2 vector = polygons[i][j];
					num = Mathf.Min(num, vector.x);
					num2 = Mathf.Max(num2, vector.x);
					num3 = Mathf.Min(num3, vector.y);
					num4 = Mathf.Max(num4, vector.y);
				}
			}
			return new Rect(num, num3, Mathf.Max(0f, num2 - num), Mathf.Max(0f, num4 - num3));
		}

		[CompilerGenerated]
		internal static Point64 <Initialize>g__MidPointOfIntRect|24_1(Rect64 bounds)
		{
			return new Point64((bounds.left + bounds.right) / 2L, (bounds.top + bounds.bottom) / 2L);
		}

		[CompilerGenerated]
		private void <BakeConfiner>g__ComputeSkeleton|25_0(in List<ConfinerOven.PolygonSolution> solutions)
		{
			Clipper64 clipper = new Clipper64();
			ClipperOffset clipperOffset = new ClipperOffset(2.0, 0.0, false, false);
			for (int i = 1; i < solutions.Count - 1; i += 2)
			{
				ConfinerOven.PolygonSolution polygonSolution = solutions[i];
				ConfinerOven.PolygonSolution polygonSolution2 = solutions[i + 1];
				double num = (double)(this.m_FloatToInt.FloatToInt(this.m_SkeletonPadding) * (polygonSolution2.frustumHeight - polygonSolution.frustumHeight));
				clipperOffset.Clear();
				clipperOffset.AddPaths(polygonSolution.polygons, JoinType.Miter, EndType.Polygon);
				List<List<Point64>> paths = new List<List<Point64>>(clipperOffset.Execute(num));
				clipperOffset.Clear();
				clipperOffset.AddPaths(polygonSolution2.polygons, JoinType.Miter, EndType.Polygon);
				List<List<Point64>> paths2 = new List<List<Point64>>(clipperOffset.Execute(num * 2.0));
				List<List<Point64>> list = new List<List<Point64>>();
				clipper.Clear();
				clipper.AddSubject(paths);
				clipper.AddClip(paths2);
				clipper.Execute(ClipType.Difference, FillRule.EvenOdd, list);
				if (list.Count > 0 && list[0].Count > 0)
				{
					this.m_Skeleton.AddRange(list);
					if (this.m_MinFrustumHeightWithBones == 3.4028235E+38f)
					{
						this.m_MinFrustumHeightWithBones = polygonSolution2.frustumHeight;
					}
				}
			}
		}

		private float m_MinFrustumHeightWithBones;

		private float m_SkeletonPadding;

		private List<List<Point64>> m_OriginalPolygon;

		private Point64 m_MidPoint;

		internal List<List<Point64>> m_Skeleton = new List<List<Point64>>();

		private ConfinerOven.FloatToIntScaler m_FloatToInt;

		private const int k_MiterLimit = 2;

		private const float k_MaxComputationTimeForFullSkeletonBakeInSeconds = 5f;

		private Rect m_PolygonRect;

		private ConfinerOven.AspectStretcher m_AspectStretcher = new ConfinerOven.AspectStretcher(1f, 0f);

		public float bakeProgress;

		private ConfinerOven.BakingStateCache m_Cache;

		private class FloatToIntScaler
		{
			public float FloatToInt(float f)
			{
				return f * (float)this.m_FloatToInt;
			}

			public float IntToFloat(long i)
			{
				return (float)i * this.m_IntToFloat;
			}

			public float ClipperEpsilon
			{
				get
				{
					return 0.01f * (float)this.m_FloatToInt;
				}
			}

			public FloatToIntScaler(Rect polygonBounds)
			{
				float num = Mathf.Max(polygonBounds.width, polygonBounds.height);
				float t = Mathf.Max(0f, num - 100f) / 9900f;
				this.m_FloatToInt = (long)Mathf.Lerp(100000f, 100f, t);
				this.m_IntToFloat = 1f / (float)this.m_FloatToInt;
			}

			private readonly long m_FloatToInt;

			private readonly float m_IntToFloat;
		}

		public class BakedSolution
		{
			public BakedSolution(float aspectRatio, float frustumHeight, bool hasBones, Rect polygonBounds, List<List<Point64>> originalPolygon, List<List<Point64>> solution)
			{
				this.m_FloatToInt = new ConfinerOven.FloatToIntScaler(polygonBounds);
				this.m_AspectStretcher = new ConfinerOven.AspectStretcher(aspectRatio, polygonBounds.center.x);
				this.m_FrustumSizeIntSpace = this.m_FloatToInt.FloatToInt(frustumHeight);
				this.m_HasBones = hasBones;
				this.m_OriginalPolygon = originalPolygon;
				this.m_Solution = solution;
				float num = this.m_FloatToInt.FloatToInt(polygonBounds.width / aspectRatio);
				float num2 = this.m_FloatToInt.FloatToInt(polygonBounds.height);
				this.m_SqrPolygonDiagonal = (double)(num * num + num2 * num2);
			}

			public bool IsValid()
			{
				return this.m_Solution != null;
			}

			public Vector2 ConfinePoint(in Vector2 pointToConfine)
			{
				if (this.m_Solution.Count <= 0)
				{
					return pointToConfine;
				}
				Vector2 vector = this.m_AspectStretcher.Stretch(pointToConfine);
				Point64 point = new Point64((double)this.m_FloatToInt.FloatToInt(vector.x), (double)this.m_FloatToInt.FloatToInt(vector.y));
				for (int i = 0; i < this.m_Solution.Count; i++)
				{
					if (Clipper.PointInPolygon(point, this.m_Solution[i]) != PointInPolygonResult.IsOutside)
					{
						return pointToConfine;
					}
				}
				bool flag = this.m_HasBones && this.<ConfinePoint>g__IsInsideOriginal|9_1(point);
				Point64 point2 = point;
				double num = double.MaxValue;
				for (int j = 0; j < this.m_Solution.Count; j++)
				{
					int count = this.m_Solution[j].Count;
					for (int k = 0; k < count; k++)
					{
						Point64 point3 = this.m_Solution[j][k];
						Point64 point4 = this.m_Solution[j][(k + 1) % count];
						Point64 point5 = ConfinerOven.BakedSolution.<ConfinePoint>g__IntPointLerp|9_0(point3, point4, this.<ConfinePoint>g__ClosestPointOnSegment|9_2(point, point3, point4));
						double num2 = (double)Mathf.Abs((float)(point.X - point5.X));
						double num3 = (double)Mathf.Abs((float)(point.Y - point5.Y));
						double num4 = num2 * num2 + num3 * num3;
						if (num2 > (double)this.m_FrustumSizeIntSpace || num3 > (double)this.m_FrustumSizeIntSpace)
						{
							num4 += this.m_SqrPolygonDiagonal;
						}
						if (num4 < num && (!flag || !this.<ConfinePoint>g__DoesIntersectOriginal|9_3(point, point5)))
						{
							num = num4;
							point2 = point5;
						}
					}
				}
				Vector2 p = new Vector2(this.m_FloatToInt.IntToFloat(point2.X), this.m_FloatToInt.IntToFloat(point2.Y));
				return this.m_AspectStretcher.Unstretch(p);
			}

			private static int FindIntersection(in Point64 p1, in Point64 p2, in Point64 p3, in Point64 p4, double epsilon)
			{
				double num = (double)(p2.X - p1.X);
				double num2 = (double)(p2.Y - p1.Y);
				double num3 = (double)(p4.X - p3.X);
				double num4 = (double)(p4.Y - p3.Y);
				double num5 = num2 * num3 - num * num4;
				double num6 = ((double)(p1.X - p3.X) * num4 + (double)(p3.Y - p1.Y) * num3) / num5;
				if (double.IsInfinity(num6) || double.IsNaN(num6))
				{
					if (ConfinerOven.BakedSolution.<FindIntersection>g__IntPointDiffSqrMagnitude|10_0(p1, p3) < epsilon || ConfinerOven.BakedSolution.<FindIntersection>g__IntPointDiffSqrMagnitude|10_0(p1, p4) < epsilon || ConfinerOven.BakedSolution.<FindIntersection>g__IntPointDiffSqrMagnitude|10_0(p2, p3) < epsilon || ConfinerOven.BakedSolution.<FindIntersection>g__IntPointDiffSqrMagnitude|10_0(p2, p4) < epsilon)
					{
						return 2;
					}
					return 0;
				}
				else
				{
					double num7 = ((double)(p3.X - p1.X) * num2 + (double)(p1.Y - p3.Y) * num) / -num5;
					if (num6 < 0.0 || num6 > 1.0 || num7 < 0.0 || num7 >= 1.0)
					{
						return 1;
					}
					return 2;
				}
			}

			[CompilerGenerated]
			internal static Point64 <ConfinePoint>g__IntPointLerp|9_0(Point64 a, Point64 b, float lerp)
			{
				return new Point64
				{
					X = (long)Mathf.RoundToInt((float)a.X + (float)(b.X - a.X) * lerp),
					Y = (long)Mathf.RoundToInt((float)a.Y + (float)(b.Y - a.Y) * lerp)
				};
			}

			[CompilerGenerated]
			private bool <ConfinePoint>g__IsInsideOriginal|9_1(Point64 point)
			{
				for (int i = 0; i < this.m_OriginalPolygon.Count; i++)
				{
					if (Clipper.PointInPolygon(point, this.m_OriginalPolygon[i]) != PointInPolygonResult.IsOutside)
					{
						return true;
					}
				}
				return false;
			}

			[CompilerGenerated]
			private float <ConfinePoint>g__ClosestPointOnSegment|9_2(Point64 point, Point64 s0, Point64 s1)
			{
				double num = (double)(s1.X - s0.X);
				double num2 = (double)(s1.Y - s0.Y);
				double num3 = num * num + num2 * num2;
				if (num3 < (double)this.m_FloatToInt.ClipperEpsilon)
				{
					return 0f;
				}
				float num4 = (float)((double)(point.X - s0.X));
				double num5 = (double)(point.Y - s0.Y);
				return Mathf.Clamp01((float)(((double)num4 * num + num5 * num2) / num3));
			}

			[CompilerGenerated]
			private bool <ConfinePoint>g__DoesIntersectOriginal|9_3(Point64 l1, Point64 l2)
			{
				double epsilon = (double)this.m_FloatToInt.ClipperEpsilon;
				for (int i = 0; i < this.m_OriginalPolygon.Count; i++)
				{
					List<Point64> list = this.m_OriginalPolygon[i];
					int count = list.Count;
					for (int j = 0; j < count; j++)
					{
						Point64 point = list[j];
						Point64 point2 = list[(j + 1) % count];
						if (ConfinerOven.BakedSolution.FindIntersection(l1, l2, point, point2, epsilon) == 2)
						{
							return true;
						}
					}
				}
				return false;
			}

			[CompilerGenerated]
			internal static double <FindIntersection>g__IntPointDiffSqrMagnitude|10_0(Point64 point1, Point64 point2)
			{
				double num = (double)(point1.X - point2.X);
				double num2 = (double)(point1.Y - point2.Y);
				return num * num + num2 * num2;
			}

			private float m_FrustumSizeIntSpace;

			private readonly ConfinerOven.AspectStretcher m_AspectStretcher;

			private readonly bool m_HasBones;

			private readonly double m_SqrPolygonDiagonal;

			private List<List<Point64>> m_OriginalPolygon;

			internal List<List<Point64>> m_Solution;

			private ConfinerOven.FloatToIntScaler m_FloatToInt;
		}

		private readonly struct AspectStretcher
		{
			public float Aspect { get; }

			public AspectStretcher(float aspect, float centerX)
			{
				this.Aspect = aspect;
				this.m_InverseAspect = 1f / this.Aspect;
				this.m_CenterX = centerX;
			}

			public Vector2 Stretch(Vector2 p)
			{
				return new Vector2((p.x - this.m_CenterX) * this.m_InverseAspect + this.m_CenterX, p.y);
			}

			public Vector2 Unstretch(Vector2 p)
			{
				return new Vector2((p.x - this.m_CenterX) * this.Aspect + this.m_CenterX, p.y);
			}

			private readonly float m_InverseAspect;

			private readonly float m_CenterX;
		}

		private struct PolygonSolution
		{
			public bool StateChanged(in List<List<Point64>> paths)
			{
				if (paths.Count != this.polygons.Count)
				{
					return true;
				}
				for (int i = 0; i < paths.Count; i++)
				{
					if (paths[i].Count != this.polygons[i].Count)
					{
						return true;
					}
				}
				return false;
			}

			public bool IsNull
			{
				get
				{
					return this.polygons == null;
				}
			}

			public List<List<Point64>> polygons;

			public float frustumHeight;
		}

		public enum BakingState
		{
			BAKING,
			BAKED,
			TIMEOUT
		}

		private struct BakingStateCache
		{
			public ClipperOffset offsetter;

			public List<ConfinerOven.PolygonSolution> solutions;

			public ConfinerOven.PolygonSolution rightCandidate;

			public ConfinerOven.PolygonSolution leftCandidate;

			public List<List<Point64>> userSetMaxCandidate;

			public List<List<Point64>> theoreticalMaxCandidate;

			public float stepSize;

			public float maxFrustumHeight;

			public float userSetMaxFrustumHeight;

			public float theoreticalMaxFrustumHeight;

			public float currentFrustumHeight;

			public float bakeTime;
		}
	}
}
