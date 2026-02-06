using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct SceneRef : INetworkStruct, IEquatable<SceneRef>
	{
		public static SceneRef None
		{
			get
			{
				return default(SceneRef);
			}
		}

		public bool IsValid
		{
			get
			{
				return this.RawValue > 0U;
			}
		}

		public bool IsIndex
		{
			get
			{
				return (this.RawValue & 2147483648U) == 0U;
			}
		}

		public int AsIndex
		{
			get
			{
				bool flag = !this.IsIndex;
				if (flag)
				{
					throw new InvalidOperationException(string.Format("SceneRef {0:X8} is not an index", this.RawValue));
				}
				return (int)(this.RawValue - 1U);
			}
		}

		public uint AsPathHash
		{
			get
			{
				bool isIndex = this.IsIndex;
				if (isIndex)
				{
					throw new InvalidOperationException(string.Format("SceneRef {0:X8} is not a path hash", this.RawValue));
				}
				return this.RawValue & 2147483647U;
			}
		}

		public bool IsPath(string path)
		{
			bool isIndex = this.IsIndex;
			return !isIndex && this == SceneRef.FromPath(path);
		}

		public static SceneRef FromIndex(int index)
		{
			bool flag = index < 0 || index == int.MaxValue;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			SceneRef result;
			result.RawValue = (uint)(index + 1);
			return result;
		}

		public static SceneRef FromPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			uint num = (uint)HashCodeUtilities.GetHashCodeDeterministic(path, 0);
			num &= 2147483647U;
			SceneRef result;
			result.RawValue = (2147483648U | num);
			return result;
		}

		public static SceneRef FromRaw(uint rawValue)
		{
			SceneRef result;
			result.RawValue = rawValue;
			return result;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is SceneRef)
			{
				SceneRef other = (SceneRef)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool Equals(SceneRef other)
		{
			return this.RawValue == other.RawValue;
		}

		public override int GetHashCode()
		{
			return this.RawValue.GetHashCode();
		}

		public override string ToString()
		{
			return this.ToString(true, true);
		}

		public string ToString(bool brackets, bool prefix)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (brackets)
			{
				stringBuilder.Append('[');
			}
			if (prefix)
			{
				stringBuilder.Append("Scene:");
			}
			bool isValid = this.IsValid;
			if (isValid)
			{
				bool isIndex = this.IsIndex;
				if (isIndex)
				{
					stringBuilder.Append("#").Append(this.AsIndex);
				}
				else
				{
					stringBuilder.AppendFormat("0x{0:X8}", this.AsPathHash);
				}
			}
			else
			{
				stringBuilder.Append("None");
			}
			if (brackets)
			{
				stringBuilder.Append(']');
			}
			return stringBuilder.ToString();
		}

		public static SceneRef Parse(string str)
		{
			ReadOnlySpan<char> span = str.AsSpan();
			bool flag = span.StartsWith("[");
			if (flag)
			{
				bool flag2 = !span.EndsWith("]");
				if (flag2)
				{
					throw new FormatException("Invalid SceneRef format: " + str);
				}
				span = span.Slice(1, span.Length - 2);
			}
			bool flag3 = span.StartsWith("Scene:");
			if (flag3)
			{
				span = span.Slice(6);
			}
			bool flag4 = span.StartsWith("#");
			SceneRef result;
			if (flag4)
			{
				result = SceneRef.FromIndex(int.Parse(span.Slice(1), NumberStyles.Integer, null));
			}
			else
			{
				bool flag5 = span.StartsWith("0x");
				if (flag5)
				{
					result = SceneRef.FromRaw(uint.Parse(span.Slice(2), NumberStyles.HexNumber, null) | 2147483648U);
				}
				else
				{
					bool flag6 = span.SequenceEqual("None".AsSpan());
					if (!flag6)
					{
						throw new FormatException("Invalid SceneRef format: " + str);
					}
					result = SceneRef.None;
				}
			}
			return result;
		}

		public static bool operator ==(SceneRef a, SceneRef b)
		{
			return a.RawValue == b.RawValue;
		}

		public static bool operator !=(SceneRef a, SceneRef b)
		{
			return a.RawValue != b.RawValue;
		}

		public const int SIZE = 4;

		public const uint FLAG_ADDRESSABLE = 2147483648U;

		[FieldOffset(0)]
		public uint RawValue;
	}
}
