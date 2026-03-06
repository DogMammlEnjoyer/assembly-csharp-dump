using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	public class SplinePath<T> : ISpline, IReadOnlyList<BezierKnot>, IEnumerable<BezierKnot>, IEnumerable, IReadOnlyCollection<BezierKnot>, IHasEmptyCurves where T : ISpline
	{
		public IReadOnlyList<T> Slices
		{
			get
			{
				return this.m_Splines;
			}
			set
			{
				this.m_Splines = value.ToArray<T>();
				this.BuildSplitData();
			}
		}

		public SplinePath(IEnumerable<T> slices)
		{
			this.m_Splines = slices.ToArray<T>();
			this.BuildSplitData();
		}

		private void BuildSplitData()
		{
			this.m_Splits = new int[this.m_Splines.Length];
			int i = 0;
			int num = this.m_Splits.Length;
			int num2 = 0;
			while (i < num)
			{
				this.m_Splits[i] = (num2 += this.m_Splines[i].Count + (this.m_Splines[i].Closed ? 1 : 0)) - 1;
				i++;
			}
		}

		public IEnumerator<BezierKnot> GetEnumerator()
		{
			foreach (T t in this.m_Splines)
			{
				foreach (BezierKnot bezierKnot in t)
				{
					yield return bezierKnot;
				}
				IEnumerator<BezierKnot> enumerator = null;
			}
			T[] array = null;
			yield break;
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int Count
		{
			get
			{
				int num = 0;
				foreach (T t in this.m_Splines)
				{
					num += t.Count + (t.Closed ? 1 : 0);
				}
				return num;
			}
		}

		public BezierKnot this[int index]
		{
			get
			{
				return this[this.GetBranchKnotIndex(index)];
			}
		}

		public BezierKnot this[SplineKnotIndex index]
		{
			get
			{
				T t = this.m_Splines[index.Spline];
				int index2 = t.Closed ? (index.Knot % t.Count) : index.Knot;
				return t[index2];
			}
		}

		internal SplineKnotIndex GetBranchKnotIndex(int knot)
		{
			knot = (this.Closed ? (knot % this.Count) : math.clamp(knot, 0, this.Count));
			int i = 0;
			int num = 0;
			while (i < this.m_Splines.Length)
			{
				T t = this.m_Splines[i];
				int num2 = t.Count + (t.Closed ? 1 : 0);
				if (knot < num + num2)
				{
					return new SplineKnotIndex(i, math.max(0, knot - num));
				}
				num += num2;
				i++;
			}
			int spline = this.m_Splines.Length - 1;
			T[] splines = this.m_Splines;
			return new SplineKnotIndex(spline, splines[splines.Length - 1].Count - 1);
		}

		public bool Closed
		{
			get
			{
				return false;
			}
		}

		public float GetLength()
		{
			float num = 0f;
			int i = 0;
			int num2 = this.Closed ? this.Count : (this.Count - 1);
			while (i < num2)
			{
				num += this.GetCurveLength(i);
				i++;
			}
			return num;
		}

		public IReadOnlyList<int> EmptyCurves
		{
			get
			{
				return this.m_Splits;
			}
		}

		private bool IsDegenerate(int index)
		{
			return Array.BinarySearch<int>(this.m_Splits, index) >= 0;
		}

		public BezierCurve GetCurve(int knot)
		{
			SplineKnotIndex branchKnotIndex = this.GetBranchKnotIndex(knot);
			if (this.IsDegenerate(knot))
			{
				BezierKnot bezierKnot = new BezierKnot(this[branchKnotIndex].Position);
				return new BezierCurve(bezierKnot, bezierKnot);
			}
			BezierKnot a = this[branchKnotIndex];
			BezierKnot b = this.Next(knot);
			return new BezierCurve(a, b);
		}

		public float GetCurveLength(int index)
		{
			if (this.IsDegenerate(index))
			{
				return 0f;
			}
			SplineKnotIndex branchKnotIndex = this.GetBranchKnotIndex(index);
			T t = this.m_Splines[branchKnotIndex.Spline];
			if (branchKnotIndex.Spline >= this.m_Splines.Length - 1 && branchKnotIndex.Knot >= t.Count - 1)
			{
				return CurveUtility.CalculateLength(this.GetCurve(index), 30);
			}
			return t.GetCurveLength(branchKnotIndex.Knot);
		}

		public float3 GetCurveUpVector(int index, float t)
		{
			if (this.IsDegenerate(index))
			{
				return 0f;
			}
			SplineKnotIndex branchKnotIndex = this.GetBranchKnotIndex(index);
			T t2 = this.m_Splines[branchKnotIndex.Spline];
			if (branchKnotIndex.Spline >= this.m_Splines.Length - 1 && branchKnotIndex.Knot >= t2.Count - 1)
			{
				BezierKnot bezierKnot = this[branchKnotIndex];
				BezierKnot bezierKnot2 = this.Next(index);
				BezierCurve curve = new BezierCurve(bezierKnot, bezierKnot2);
				float3 startUp = math.rotate(bezierKnot.Rotation, math.up());
				float3 endUp = math.rotate(bezierKnot2.Rotation, math.up());
				return CurveUtility.EvaluateUpVector(curve, t, startUp, endUp, true);
			}
			return t2.GetCurveUpVector(branchKnotIndex.Knot, t);
		}

		public float GetCurveInterpolation(int curveIndex, float curveDistance)
		{
			SplineKnotIndex branchKnotIndex = this.GetBranchKnotIndex(curveIndex);
			T t = this.m_Splines[branchKnotIndex.Spline];
			return t.GetCurveInterpolation(branchKnotIndex.Knot, curveDistance);
		}

		private T[] m_Splines;

		private int[] m_Splits;
	}
}
