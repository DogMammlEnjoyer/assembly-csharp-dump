using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(4)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkObjectGuid : INetworkStruct, IEquatable<NetworkObjectGuid>, IComparable<NetworkObjectGuid>
	{
		public static NetworkObjectGuid Empty
		{
			get
			{
				return default(NetworkObjectGuid);
			}
		}

		public NetworkObjectGuid(string guid)
		{
			this = new Guid(guid);
		}

		public NetworkObjectGuid(long data0, long data1)
		{
			this._data0 = data0;
			this._data1 = data1;
		}

		public NetworkObjectGuid(byte[] guid)
		{
			this._data0 = BitConverter.ToInt64(guid, 0);
			this._data1 = BitConverter.ToInt64(guid, 8);
		}

		public unsafe NetworkObjectGuid(byte* guid)
		{
			this._data0 = *(long*)guid;
			this._data1 = *(long*)(guid + 8);
		}

		public bool IsValid
		{
			get
			{
				return this._data0 != 0L || this._data1 != 0L;
			}
		}

		public unsafe static implicit operator NetworkObjectGuid(Guid guid)
		{
			NetworkObjectGuid result = default(NetworkObjectGuid);
			NetworkObjectGuidUtils.CopyAndMangleGuid((byte*)(&guid), (byte*)(&result));
			return result;
		}

		public unsafe static implicit operator Guid(NetworkObjectGuid guid)
		{
			Guid result = default(Guid);
			NetworkObjectGuidUtils.CopyAndMangleGuid((byte*)(&guid), (byte*)(&result));
			return result;
		}

		public static bool TryParse(string str, out NetworkObjectGuid guid)
		{
			Guid guid2;
			bool flag = Guid.TryParse(str, out guid2);
			bool result;
			if (flag)
			{
				guid = guid2;
				result = true;
			}
			else
			{
				guid = default(NetworkObjectGuid);
				result = false;
			}
			return result;
		}

		public static NetworkObjectGuid Parse(string str)
		{
			return Guid.Parse(str);
		}

		public static bool operator ==(NetworkObjectGuid a, NetworkObjectGuid b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(NetworkObjectGuid a, NetworkObjectGuid b)
		{
			return !a.Equals(b);
		}

		public bool Equals(NetworkObjectGuid other)
		{
			return this._data0 == other._data0 && this._data1 == other._data1;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkObjectGuid)
			{
				NetworkObjectGuid other = (NetworkObjectGuid)obj;
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
			return this.GetHashCode();
		}

		public override string ToString()
		{
			return this.ToString();
		}

		public string ToUnityGuidString()
		{
			return this.ToString("N");
		}

		public string ToString(string format)
		{
			return this.ToString(format);
		}

		public int CompareTo(NetworkObjectGuid other)
		{
			long num = this._data0 - other._data0;
			bool flag = num == 0L;
			if (flag)
			{
				num = this._data1 - other._data1;
				bool flag2 = num == 0L;
				if (flag2)
				{
					return 0;
				}
			}
			bool flag3 = num < 0L;
			int result;
			if (flag3)
			{
				result = -1;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		public unsafe static explicit operator NetworkPrefabRef(NetworkObjectGuid t)
		{
			return new NetworkPrefabRef((byte*)(&t));
		}

		public const int SIZE = 16;

		public const int ALIGNMENT = 4;

		[FixedBuffer(typeof(long), 2)]
		[FieldOffset(0)]
		public NetworkObjectGuid.<RawGuidValue>e__FixedBuffer RawGuidValue;

		[NonSerialized]
		[FieldOffset(0)]
		private long _data0;

		[NonSerialized]
		[FieldOffset(8)]
		private long _data1;

		public sealed class EqualityComparer : IEqualityComparer<NetworkObjectGuid>
		{
			public bool Equals(NetworkObjectGuid x, NetworkObjectGuid y)
			{
				return x.Equals(y);
			}

			public int GetHashCode(NetworkObjectGuid obj)
			{
				return obj.GetHashCode();
			}
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <RawGuidValue>e__FixedBuffer
		{
			public long FixedElementField;
		}
	}
}
