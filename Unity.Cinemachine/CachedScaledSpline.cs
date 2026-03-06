using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	internal class CachedScaledSpline : ISpline, IReadOnlyList<BezierKnot>, IEnumerable<BezierKnot>, IEnumerable, IReadOnlyCollection<BezierKnot>, IDisposable
	{
		public CachedScaledSpline(Spline spline, Transform transform, Allocator allocator = Allocator.Persistent)
		{
			Vector3 vector = (transform != null) ? transform.lossyScale : Vector3.one;
			this.m_CachedSource = spline;
			this.m_NativeSpline = new NativeSpline(spline, Matrix4x4.Scale(vector), allocator);
			this.m_CachedScale = vector;
			this.m_IsAllocated = true;
		}

		public void Dispose()
		{
			if (this.m_IsAllocated)
			{
				this.m_NativeSpline.Dispose();
			}
			this.m_IsAllocated = false;
		}

		public bool IsCrudelyValid(Spline spline, Transform transform)
		{
			Vector3 b = (transform != null) ? transform.lossyScale : Vector3.one;
			return spline == this.m_CachedSource && (this.m_CachedScale - b).AlmostZero() && this.m_NativeSpline.Count == this.m_CachedSource.Count;
		}

		public bool KnotsAreValid(Spline spline, Transform transform)
		{
			if (this.m_NativeSpline.Count != spline.Count)
			{
				return false;
			}
			Matrix4x4 m = Matrix4x4.Scale((transform != null) ? transform.lossyScale : Vector3.one);
			IEnumerator<BezierKnot> enumerator = this.GetEnumerator();
			IEnumerator<BezierKnot> enumerator2 = spline.GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				BezierKnot bezierKnot = enumerator.Current;
				BezierKnot bezierKnot2 = enumerator2.Current;
				if (!bezierKnot.Equals(bezierKnot2.Transform(m)))
				{
					return false;
				}
			}
			return true;
		}

		public BezierKnot this[int index]
		{
			get
			{
				return this.m_NativeSpline[index];
			}
		}

		public bool Closed
		{
			get
			{
				return this.m_NativeSpline.Closed;
			}
		}

		public int Count
		{
			get
			{
				return this.m_NativeSpline.Count;
			}
		}

		public BezierCurve GetCurve(int index)
		{
			return this.m_NativeSpline.GetCurve(index);
		}

		public float GetCurveInterpolation(int curveIndex, float curveDistance)
		{
			return this.m_NativeSpline.GetCurveInterpolation(curveIndex, curveDistance);
		}

		public float GetCurveLength(int index)
		{
			return this.m_NativeSpline.GetCurveLength(index);
		}

		public float3 GetCurveUpVector(int index, float t)
		{
			return this.m_NativeSpline.GetCurveUpVector(index, t);
		}

		public IEnumerator<BezierKnot> GetEnumerator()
		{
			return this.m_NativeSpline.GetEnumerator();
		}

		public float GetLength()
		{
			return this.m_NativeSpline.GetLength();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.m_NativeSpline.GetEnumerator();
		}

		private NativeSpline m_NativeSpline;

		private readonly Spline m_CachedSource;

		private readonly Vector3 m_CachedScale;

		private bool m_IsAllocated;
	}
}
