using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	internal class RamerDouglasPeucker<T> where T : IList<float3>
	{
		public RamerDouglasPeucker(T points)
		{
			this.m_Points = points;
		}

		public void Reduce(List<float3> results, float epsilon)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			this.m_Epsilon = math.max(float.Epsilon, epsilon);
			this.m_KeepCount = this.m_Points.Count;
			this.m_Keep = new bool[this.m_KeepCount];
			for (int i = 0; i < this.m_KeepCount; i++)
			{
				this.Keep(i);
			}
			this.Reduce(new RamerDouglasPeucker<T>.Range(0, this.m_KeepCount));
			results.Clear();
			if (results.Capacity < this.m_KeepCount)
			{
				results.Capacity = this.m_KeepCount;
			}
			for (int j = 0; j < this.m_Keep.Length; j++)
			{
				if (this.m_Keep[j])
				{
					results.Add(this.m_Points[j]);
				}
			}
		}

		private void Keep(int index)
		{
			this.m_Keep[index] = true;
		}

		private void Discard(RamerDouglasPeucker<T>.Range range)
		{
			this.m_KeepCount -= range.Count;
			for (int i = range.Start; i <= range.End; i++)
			{
				this.m_Keep[i] = false;
			}
		}

		private void Reduce(RamerDouglasPeucker<T>.Range range)
		{
			if (range.Count < 3)
			{
				return;
			}
			ValueTuple<int, float> valueTuple = this.FindFarthest(range);
			if (valueTuple.Item2 < this.m_Epsilon)
			{
				this.Discard(new RamerDouglasPeucker<T>.Range(range.Start + 1, range.Count - 2));
				return;
			}
			this.Reduce(new RamerDouglasPeucker<T>.Range(range.Start, valueTuple.Item1 - range.Start + 1));
			this.Reduce(new RamerDouglasPeucker<T>.Range(valueTuple.Item1, range.End - valueTuple.Item1 + 1));
		}

		[return: TupleElementNames(new string[]
		{
			"index",
			"distance"
		})]
		private ValueTuple<int, float> FindFarthest(RamerDouglasPeucker<T>.Range range)
		{
			float num = 0f;
			int item = -1;
			for (int i = range.Start + 1; i < range.End; i++)
			{
				float num2 = SplineMath.DistancePointLine(this.m_Points[i], this.m_Points[range.Start], this.m_Points[range.End]);
				if (num2 > num)
				{
					num = num2;
					item = i;
				}
			}
			return new ValueTuple<int, float>(item, num);
		}

		private T m_Points;

		private bool[] m_Keep;

		private float m_Epsilon;

		private int m_KeepCount;

		private struct Range
		{
			public int End
			{
				get
				{
					return this.Start + this.Count - 1;
				}
			}

			public Range(int start, int count)
			{
				this.Start = start;
				this.Count = count;
			}

			public override string ToString()
			{
				return string.Format("[{0}, {1}]", this.Start, this.End);
			}

			public int Start;

			public int Count;
		}
	}
}
