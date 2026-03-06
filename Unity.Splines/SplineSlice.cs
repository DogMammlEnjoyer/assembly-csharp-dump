using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	public struct SplineSlice<T> : ISpline, IReadOnlyList<BezierKnot>, IEnumerable<BezierKnot>, IEnumerable, IReadOnlyCollection<BezierKnot> where T : ISpline
	{
		public int Count
		{
			get
			{
				if (this.Spline.Closed)
				{
					return math.clamp(this.Range.Count, 0, this.Spline.Count + 1);
				}
				if (this.Range.Direction == SliceDirection.Backward)
				{
					return math.clamp(this.Range.Count, 0, this.Range.Start + 1);
				}
				return math.clamp(this.Range.Count, 0, this.Spline.Count - this.Range.Start);
			}
		}

		public bool Closed
		{
			get
			{
				return false;
			}
		}

		private static BezierKnot FlipTangents(BezierKnot knot)
		{
			return new BezierKnot(knot.Position, knot.TangentOut, knot.TangentIn, knot.Rotation);
		}

		public BezierKnot this[int index]
		{
			get
			{
				int num = this.Range[index];
				num = (num + this.Spline.Count) % this.Spline.Count;
				if (this.Range.Direction != SliceDirection.Backward)
				{
					return this.Spline[num].Transform(this.Transform);
				}
				return SplineSlice<T>.FlipTangents(this.Spline[num]).Transform(this.Transform);
			}
		}

		public IEnumerator<BezierKnot> GetEnumerator()
		{
			int i = 0;
			int c = this.Range.Count;
			while (i < c)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public SplineSlice(T spline, SplineRange range)
		{
			this = new SplineSlice<T>(spline, range, float4x4.identity);
		}

		public SplineSlice(T spline, SplineRange range, float4x4 transform)
		{
			this.Spline = spline;
			this.Range = range;
			this.Transform = transform;
		}

		public float GetLength()
		{
			float num = 0f;
			int i = 0;
			int count = this.Count;
			while (i < count)
			{
				num += this.GetCurveLength(i);
				i++;
			}
			return num;
		}

		public BezierCurve GetCurve(int index)
		{
			int num = math.min(math.max(index + 1, 0), this.Range.Count - 1);
			BezierKnot bezierKnot = this[index];
			BezierKnot bezierKnot2 = this[num];
			if (index == num)
			{
				return new BezierCurve(bezierKnot.Position, bezierKnot2.Position);
			}
			return new BezierCurve(bezierKnot, bezierKnot2);
		}

		public float GetCurveLength(int index)
		{
			return CurveUtility.CalculateLength(this.GetCurve(index), 30);
		}

		public float3 GetCurveUpVector(int index, float t)
		{
			return this.CalculateUpVector(index, t);
		}

		public float GetCurveInterpolation(int curveIndex, float curveDistance)
		{
			return CurveUtility.GetDistanceToInterpolation(this.GetCurve(curveIndex), curveDistance);
		}

		public T Spline;

		public SplineRange Range;

		public float4x4 Transform;
	}
}
