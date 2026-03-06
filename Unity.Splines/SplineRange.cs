using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[Serializable]
	public struct SplineRange : IEnumerable<int>, IEnumerable
	{
		public int Start
		{
			get
			{
				return this.m_Start;
			}
			set
			{
				this.m_Start = value;
			}
		}

		public int End
		{
			get
			{
				return this[this.Count - 1];
			}
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
			set
			{
				this.m_Count = math.max(value, 0);
			}
		}

		public SliceDirection Direction
		{
			get
			{
				return this.m_Direction;
			}
			set
			{
				this.m_Direction = value;
			}
		}

		public SplineRange(int start, int count)
		{
			this = new SplineRange(start, count, (count < 0) ? SliceDirection.Backward : SliceDirection.Forward);
		}

		public SplineRange(int start, int count, SliceDirection direction)
		{
			this.m_Start = start;
			this.m_Count = math.abs(count);
			this.m_Direction = direction;
		}

		public int this[int index]
		{
			get
			{
				if (this.Direction != SliceDirection.Backward)
				{
					return this.m_Start + index;
				}
				return this.m_Start - index;
			}
		}

		public IEnumerator<int> GetEnumerator()
		{
			return new SplineRange.SplineRangeEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override string ToString()
		{
			return string.Format("{{{0}..{1}}}", this.Start, this.End);
		}

		[SerializeField]
		private int m_Start;

		[SerializeField]
		private int m_Count;

		[SerializeField]
		private SliceDirection m_Direction;

		public struct SplineRangeEnumerator : IEnumerator<int>, IEnumerator, IDisposable
		{
			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_Count;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public int Current
			{
				get
				{
					if (!this.m_Reverse)
					{
						return this.m_Start + this.m_Index;
					}
					return this.m_End - this.m_Index;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public SplineRangeEnumerator(SplineRange range)
			{
				this.m_Index = -1;
				this.m_Reverse = (range.Direction == SliceDirection.Backward);
				int start = range.Start;
				int y = this.m_Reverse ? (range.Start - range.Count) : (range.Start + range.Count);
				this.m_Start = math.min(start, y);
				this.m_End = math.max(start, y);
				this.m_Count = range.Count;
			}

			public void Dispose()
			{
			}

			private int m_Index;

			private int m_Start;

			private int m_End;

			private int m_Count;

			private bool m_Reverse;
		}
	}
}
