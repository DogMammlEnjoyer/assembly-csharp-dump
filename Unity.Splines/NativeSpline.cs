using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	public struct NativeSpline : ISpline, IReadOnlyList<BezierKnot>, IEnumerable<BezierKnot>, IEnumerable, IReadOnlyCollection<BezierKnot>, IDisposable
	{
		public NativeArray<BezierKnot> Knots
		{
			get
			{
				return this.m_Knots;
			}
		}

		public NativeArray<BezierCurve> Curves
		{
			get
			{
				return this.m_Curves;
			}
		}

		public bool Closed
		{
			get
			{
				return this.m_Closed;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Knots.Length;
			}
		}

		public float GetLength()
		{
			return this.m_Length;
		}

		public BezierKnot this[int index]
		{
			get
			{
				return this.m_Knots[index];
			}
		}

		public IEnumerator<BezierKnot> GetEnumerator()
		{
			return this.m_Knots.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public NativeSpline(ISpline spline, Allocator allocator = Allocator.Temp)
		{
			this = new NativeSpline(spline, float4x4.identity, false, allocator);
		}

		public NativeSpline(ISpline spline, bool cacheUpVectors, Allocator allocator = Allocator.Temp)
		{
			this = new NativeSpline(spline, float4x4.identity, cacheUpVectors, allocator);
		}

		public NativeSpline(ISpline spline, float4x4 transform, Allocator allocator = Allocator.Temp)
		{
			IHasEmptyCurves hasEmptyCurves = spline as IHasEmptyCurves;
			this = new NativeSpline(spline, (hasEmptyCurves != null) ? hasEmptyCurves.EmptyCurves : null, spline.Closed, transform, false, allocator);
		}

		public NativeSpline(ISpline spline, float4x4 transform, bool cacheUpVectors, Allocator allocator = Allocator.Temp)
		{
			IHasEmptyCurves hasEmptyCurves = spline as IHasEmptyCurves;
			this = new NativeSpline(spline, (hasEmptyCurves != null) ? hasEmptyCurves.EmptyCurves : null, spline.Closed, transform, cacheUpVectors, allocator);
		}

		public NativeSpline(IReadOnlyList<BezierKnot> knots, bool closed, float4x4 transform, Allocator allocator = Allocator.Temp)
		{
			this = new NativeSpline(knots, null, closed, transform, false, allocator);
		}

		public NativeSpline(IReadOnlyList<BezierKnot> knots, bool closed, float4x4 transform, bool cacheUpVectors, Allocator allocator = Allocator.Temp)
		{
			this = new NativeSpline(knots, null, closed, transform, cacheUpVectors, allocator);
		}

		public NativeSpline(IReadOnlyList<BezierKnot> knots, IReadOnlyList<int> splits, bool closed, float4x4 transform, Allocator allocator = Allocator.Temp)
		{
			this = new NativeSpline(knots, splits, closed, transform, false, allocator);
		}

		public NativeSpline(IReadOnlyList<BezierKnot> knots, IReadOnlyList<int> splits, bool closed, float4x4 transform, bool cacheUpVectors, Allocator allocator = Allocator.Temp)
		{
			int count = knots.Count;
			this.m_Knots = new NativeArray<BezierKnot>(count, allocator, NativeArrayOptions.ClearMemory);
			this.m_Curves = new NativeArray<BezierCurve>(count, allocator, NativeArrayOptions.ClearMemory);
			this.m_SegmentLengthsLookupTable = new NativeArray<DistanceToInterpolation>(count * 30, allocator, NativeArrayOptions.ClearMemory);
			this.m_Closed = closed;
			this.m_Length = 0f;
			this.m_UpVectorsLookupTable = new NativeArray<float3>(cacheUpVectors ? (count * 30) : 0, allocator, NativeArrayOptions.ClearMemory);
			NativeArray<DistanceToInterpolation> lookupTable = new NativeArray<DistanceToInterpolation>(30, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<float3> upVectors = cacheUpVectors ? new NativeArray<float3>(30, Allocator.Temp, NativeArrayOptions.ClearMemory) : default(NativeArray<float3>);
			if (count > 0)
			{
				BezierKnot bezierKnot = knots[0].Transform(transform);
				for (int i = 0; i < count; i++)
				{
					BezierKnot bezierKnot2 = knots[(i + 1) % count].Transform(transform);
					this.m_Knots[i] = bezierKnot;
					if (splits != null && splits.Contains(i))
					{
						this.m_Curves[i] = new BezierCurve(new BezierKnot(bezierKnot.Position), new BezierKnot(bezierKnot.Position));
						float3 value = cacheUpVectors ? math.rotate(bezierKnot.Rotation, math.up()) : float3.zero;
						for (int j = 0; j < 30; j++)
						{
							lookupTable[j] = default(DistanceToInterpolation);
							if (cacheUpVectors)
							{
								upVectors[j] = value;
							}
						}
					}
					else
					{
						this.m_Curves[i] = new BezierCurve(bezierKnot, bezierKnot2);
						CurveUtility.CalculateCurveLengths(this.m_Curves[i], lookupTable);
						if (cacheUpVectors)
						{
							float3 startUp = math.rotate(bezierKnot.Rotation, math.up());
							float3 endUp = math.rotate(bezierKnot2.Rotation, math.up());
							CurveUtility.EvaluateUpVectors(this.m_Curves[i], startUp, endUp, upVectors);
						}
					}
					if (this.m_Closed || i < count - 1)
					{
						this.m_Length += lookupTable[29].Distance;
					}
					for (int k = 0; k < 30; k++)
					{
						this.m_SegmentLengthsLookupTable[i * 30 + k] = lookupTable[k];
						if (cacheUpVectors)
						{
							this.m_UpVectorsLookupTable[i * 30 + k] = upVectors[k];
						}
					}
					bezierKnot = bezierKnot2;
				}
			}
		}

		public BezierCurve GetCurve(int index)
		{
			return this.m_Curves[index];
		}

		public float GetCurveLength(int curveIndex)
		{
			return this.m_SegmentLengthsLookupTable[curveIndex * 30 + 30 - 1].Distance;
		}

		public float3 GetCurveUpVector(int index, float t)
		{
			if (this.m_UpVectorsLookupTable.Length == 0)
			{
				return this.CalculateUpVector(index, t);
			}
			int num = index * 30;
			float num2 = 0.03448276f;
			float num3 = 0f;
			for (int i = 0; i < 30; i++)
			{
				if (t <= num3 + num2)
				{
					return math.lerp(this.m_UpVectorsLookupTable[num + i], this.m_UpVectorsLookupTable[num + i + 1], (t - num3) / num2);
				}
				num3 += num2;
			}
			return this.m_UpVectorsLookupTable[num + 30 - 1];
		}

		public void Dispose()
		{
			this.m_Knots.Dispose();
			this.m_Curves.Dispose();
			this.m_SegmentLengthsLookupTable.Dispose();
			this.m_UpVectorsLookupTable.Dispose();
		}

		public float GetCurveInterpolation(int curveIndex, float curveDistance)
		{
			if (curveIndex < 0 || curveIndex >= this.m_SegmentLengthsLookupTable.Length || curveDistance <= 0f)
			{
				return 0f;
			}
			float curveLength = this.GetCurveLength(curveIndex);
			if (curveDistance >= curveLength)
			{
				return 1f;
			}
			int start = curveIndex * 30;
			return CurveUtility.GetDistanceToInterpolation<NativeSpline.Slice<DistanceToInterpolation>>(new NativeSpline.Slice<DistanceToInterpolation>(this.m_SegmentLengthsLookupTable, start, 30), curveDistance);
		}

		[ReadOnly]
		private NativeArray<BezierKnot> m_Knots;

		[ReadOnly]
		private NativeArray<BezierCurve> m_Curves;

		[ReadOnly]
		private NativeArray<DistanceToInterpolation> m_SegmentLengthsLookupTable;

		[ReadOnly]
		private NativeArray<float3> m_UpVectorsLookupTable;

		private bool m_Closed;

		private float m_Length;

		private const int k_SegmentResolution = 30;

		private struct Slice<T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : struct
		{
			public Slice(NativeArray<T> array, int start, int count)
			{
				this.m_Slice = new NativeSlice<T>(array, start, count);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return this.m_Slice.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public int Count
			{
				get
				{
					return this.m_Slice.Length;
				}
			}

			public T this[int index]
			{
				get
				{
					return this.m_Slice[index];
				}
			}

			private NativeSlice<T> m_Slice;
		}
	}
}
