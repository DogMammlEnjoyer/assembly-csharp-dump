using System;

namespace UnityEngine.Rendering
{
	internal struct InstanceHandle : IEquatable<InstanceHandle>, IComparable<InstanceHandle>
	{
		public int index { readonly get; private set; }

		public int instanceIndex
		{
			get
			{
				return this.index >> 1;
			}
		}

		public InstanceType type
		{
			get
			{
				return (InstanceType)((long)this.index & 1L);
			}
		}

		public bool valid
		{
			get
			{
				return this.index != -1;
			}
		}

		public static InstanceHandle Create(int instanceIndex, InstanceType instanceType)
		{
			return new InstanceHandle
			{
				index = (instanceIndex << 1 | (int)instanceType)
			};
		}

		public static InstanceHandle FromInt(int value)
		{
			return new InstanceHandle
			{
				index = value
			};
		}

		public bool Equals(InstanceHandle other)
		{
			return this.index == other.index;
		}

		public int CompareTo(InstanceHandle other)
		{
			return this.index.CompareTo(other.index);
		}

		public override int GetHashCode()
		{
			return this.index;
		}

		public static readonly InstanceHandle Invalid = new InstanceHandle
		{
			index = -1
		};
	}
}
