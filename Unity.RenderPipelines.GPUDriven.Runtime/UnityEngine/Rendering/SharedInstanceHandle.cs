using System;

namespace UnityEngine.Rendering
{
	internal struct SharedInstanceHandle : IEquatable<SharedInstanceHandle>, IComparable<SharedInstanceHandle>
	{
		public int index { readonly get; set; }

		public bool valid
		{
			get
			{
				return this.index != -1;
			}
		}

		public bool Equals(SharedInstanceHandle other)
		{
			return this.index == other.index;
		}

		public int CompareTo(SharedInstanceHandle other)
		{
			return this.index.CompareTo(other.index);
		}

		public override int GetHashCode()
		{
			return this.index;
		}

		public static readonly SharedInstanceHandle Invalid = new SharedInstanceHandle
		{
			index = -1
		};
	}
}
