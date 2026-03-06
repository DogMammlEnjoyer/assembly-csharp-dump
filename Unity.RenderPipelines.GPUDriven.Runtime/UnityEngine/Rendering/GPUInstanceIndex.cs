using System;

namespace UnityEngine.Rendering
{
	internal struct GPUInstanceIndex : IEquatable<GPUInstanceIndex>, IComparable<GPUInstanceIndex>
	{
		public int index { readonly get; set; }

		public bool valid
		{
			get
			{
				return this.index != -1;
			}
		}

		public bool Equals(GPUInstanceIndex other)
		{
			return this.index == other.index;
		}

		public int CompareTo(GPUInstanceIndex other)
		{
			return this.index.CompareTo(other.index);
		}

		public override int GetHashCode()
		{
			return this.index;
		}

		public static readonly GPUInstanceIndex Invalid = new GPUInstanceIndex
		{
			index = -1
		};
	}
}
