using System;

namespace UnityEngine.Rendering.Universal
{
	internal struct InclusiveRange
	{
		public InclusiveRange(short startEnd)
		{
			this.start = startEnd;
			this.end = startEnd;
		}

		public InclusiveRange(short start, short end)
		{
			this.start = start;
			this.end = end;
		}

		public void Expand(short index)
		{
			this.start = Math.Min(this.start, index);
			this.end = Math.Max(this.end, index);
		}

		public void Clamp(short min, short max)
		{
			this.start = Math.Max(min, this.start);
			this.end = Math.Min(max, this.end);
		}

		public bool isEmpty
		{
			get
			{
				return this.end < this.start;
			}
		}

		public bool Contains(short index)
		{
			return index >= this.start && index <= this.end;
		}

		public static InclusiveRange Merge(InclusiveRange a, InclusiveRange b)
		{
			return new InclusiveRange(Math.Min(a.start, b.start), Math.Max(a.end, b.end));
		}

		public static InclusiveRange empty
		{
			get
			{
				return new InclusiveRange(short.MaxValue, short.MinValue);
			}
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", this.start, this.end);
		}

		public short start;

		public short end;
	}
}
