using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Serialization;

namespace Fusion
{
	[InlineHelp]
	[NetworkStructWeaved(1)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkPrefabId : INetworkStruct, IEquatable<NetworkPrefabId>, IComparable, IComparable<NetworkPrefabId>
	{
		public bool IsNone
		{
			get
			{
				return this.RawValue == 0U;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.RawValue > 0U;
			}
		}

		public int AsIndex
		{
			get
			{
				return (int)(this.RawValue - 1U);
			}
		}

		public static NetworkPrefabId FromIndex(int index)
		{
			bool flag = index < 0 || index >= int.MaxValue;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			NetworkPrefabId result;
			result.RawValue = (uint)(index + 1);
			return result;
		}

		public static NetworkPrefabId FromRaw(uint value)
		{
			NetworkPrefabId result;
			result.RawValue = value;
			return result;
		}

		public bool Equals(NetworkPrefabId other)
		{
			return this.RawValue == other.RawValue;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkPrefabId)
			{
				NetworkPrefabId other = (NetworkPrefabId)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return (int)this.RawValue;
		}

		public override string ToString()
		{
			return this.ToString(true, true);
		}

		int IComparable.CompareTo(object obj)
		{
			NetworkPrefabId other;
			bool flag;
			if (obj is NetworkPrefabId)
			{
				other = (NetworkPrefabId)obj;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			int result;
			if (flag2)
			{
				result = this.CompareTo(other);
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public string ToString(bool brackets, bool prefix)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (brackets)
			{
				stringBuilder.Append('[');
			}
			bool isValid = this.IsValid;
			if (isValid)
			{
				if (prefix)
				{
					stringBuilder.Append("Index:");
				}
				stringBuilder.Append(this.AsIndex);
			}
			else
			{
				stringBuilder.Append("Invalid");
			}
			if (brackets)
			{
				stringBuilder.Append(']');
			}
			return stringBuilder.ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(NetworkPrefabId a, NetworkPrefabId b)
		{
			return a.RawValue == b.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(NetworkPrefabId a, NetworkPrefabId b)
		{
			return a.RawValue != b.RawValue;
		}

		public int CompareTo(NetworkPrefabId other)
		{
			return this.RawValue.CompareTo(other.RawValue);
		}

		public const int SIZE = 4;

		public const int ALIGNMENT = 4;

		public const int MAX_INDEX = 2147483646;

		[FormerlySerializedAs("Value")]
		[FieldOffset(0)]
		public uint RawValue;

		public sealed class EqualityComparer : IEqualityComparer<NetworkPrefabId>
		{
			public bool Equals(NetworkPrefabId x, NetworkPrefabId y)
			{
				return x.RawValue == y.RawValue;
			}

			public int GetHashCode(NetworkPrefabId obj)
			{
				return (int)obj.RawValue;
			}
		}
	}
}
