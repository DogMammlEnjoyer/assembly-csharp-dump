using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.InputSystem.Utilities
{
	[StructLayout(LayoutKind.Explicit)]
	public struct PrimitiveValue : IEquatable<PrimitiveValue>, IConvertible
	{
		internal unsafe byte* valuePtr
		{
			get
			{
				return (byte*)UnsafeUtility.AddressOf<PrimitiveValue>(ref this) + 4;
			}
		}

		public TypeCode type
		{
			get
			{
				return this.m_Type;
			}
		}

		public bool isEmpty
		{
			get
			{
				return this.type == TypeCode.Empty;
			}
		}

		public PrimitiveValue(bool value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.Boolean;
			this.m_BoolValue = value;
		}

		public PrimitiveValue(char value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.Char;
			this.m_CharValue = value;
		}

		public PrimitiveValue(byte value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.Byte;
			this.m_ByteValue = value;
		}

		public PrimitiveValue(sbyte value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.SByte;
			this.m_SByteValue = value;
		}

		public PrimitiveValue(short value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.Int16;
			this.m_ShortValue = value;
		}

		public PrimitiveValue(ushort value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.UInt16;
			this.m_UShortValue = value;
		}

		public PrimitiveValue(int value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.Int32;
			this.m_IntValue = value;
		}

		public PrimitiveValue(uint value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.UInt32;
			this.m_UIntValue = value;
		}

		public PrimitiveValue(long value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.Int64;
			this.m_LongValue = value;
		}

		public PrimitiveValue(ulong value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.UInt64;
			this.m_ULongValue = value;
		}

		public PrimitiveValue(float value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.Single;
			this.m_FloatValue = value;
		}

		public PrimitiveValue(double value)
		{
			this = default(PrimitiveValue);
			this.m_Type = TypeCode.Double;
			this.m_DoubleValue = value;
		}

		public PrimitiveValue ConvertTo(TypeCode type)
		{
			switch (type)
			{
			case TypeCode.Empty:
				return default(PrimitiveValue);
			case TypeCode.Boolean:
				return this.ToBoolean(null);
			case TypeCode.Char:
				return this.ToChar(null);
			case TypeCode.SByte:
				return this.ToSByte(null);
			case TypeCode.Byte:
				return this.ToByte(null);
			case TypeCode.Int16:
				return this.ToInt16(null);
			case TypeCode.UInt16:
				return this.ToInt16(null);
			case TypeCode.Int32:
				return this.ToInt32(null);
			case TypeCode.UInt32:
				return this.ToInt32(null);
			case TypeCode.Int64:
				return this.ToInt64(null);
			case TypeCode.UInt64:
				return this.ToUInt64(null);
			case TypeCode.Single:
				return this.ToSingle(null);
			case TypeCode.Double:
				return this.ToDouble(null);
			}
			throw new ArgumentException(string.Format("Don't know how to convert PrimitiveValue to '{0}'", type), "type");
		}

		public unsafe bool Equals(PrimitiveValue other)
		{
			if (this.m_Type != other.m_Type)
			{
				return false;
			}
			void* ptr = UnsafeUtility.AddressOf<double>(ref this.m_DoubleValue);
			void* ptr2 = UnsafeUtility.AddressOf<double>(ref other.m_DoubleValue);
			return UnsafeUtility.MemCmp(ptr, ptr2, 8L) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is PrimitiveValue)
			{
				PrimitiveValue other = (PrimitiveValue)obj;
				return this.Equals(other);
			}
			return (obj is bool || obj is char || obj is byte || obj is sbyte || obj is short || obj is ushort || obj is int || obj is uint || obj is long || obj is ulong || obj is float || obj is double) && this.Equals(PrimitiveValue.FromObject(obj));
		}

		public static bool operator ==(PrimitiveValue left, PrimitiveValue right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PrimitiveValue left, PrimitiveValue right)
		{
			return !left.Equals(right);
		}

		public unsafe override int GetHashCode()
		{
			fixed (double* ptr = &this.m_DoubleValue)
			{
				double* ptr2 = ptr;
				return this.m_Type.GetHashCode() * 397 ^ ptr2->GetHashCode();
			}
		}

		public override string ToString()
		{
			switch (this.type)
			{
			case TypeCode.Boolean:
				if (!this.m_BoolValue)
				{
					return "false";
				}
				return "true";
			case TypeCode.Char:
				return "'" + this.m_CharValue.ToString() + "'";
			case TypeCode.SByte:
				return this.m_SByteValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Byte:
				return this.m_ByteValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Int16:
				return this.m_ShortValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.UInt16:
				return this.m_UShortValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Int32:
				return this.m_IntValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.UInt32:
				return this.m_UIntValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Int64:
				return this.m_LongValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.UInt64:
				return this.m_ULongValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Single:
				return this.m_FloatValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Double:
				return this.m_DoubleValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			default:
				return string.Empty;
			}
		}

		public static PrimitiveValue FromString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return default(PrimitiveValue);
			}
			if (value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				return new PrimitiveValue(true);
			}
			if (value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
			{
				return new PrimitiveValue(false);
			}
			double value2;
			if ((value.Contains('.') || value.Contains("e") || value.Contains("E") || value.Contains("infinity", StringComparison.InvariantCultureIgnoreCase)) && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out value2))
			{
				return new PrimitiveValue(value2);
			}
			long value3;
			if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value3))
			{
				return new PrimitiveValue(value3);
			}
			if (value.IndexOf("0x", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				string text = value.TrimStart();
				if (text.StartsWith("0x"))
				{
					text = text.Substring(2);
				}
				long value4;
				if (long.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value4))
				{
					return new PrimitiveValue(value4);
				}
			}
			throw new NotImplementedException();
		}

		public TypeCode GetTypeCode()
		{
			return this.type;
		}

		public bool ToBoolean(IFormatProvider provider = null)
		{
			switch (this.type)
			{
			case TypeCode.Boolean:
				return this.m_BoolValue;
			case TypeCode.Char:
				return this.m_CharValue > '\0';
			case TypeCode.SByte:
				return this.m_SByteValue != 0;
			case TypeCode.Byte:
				return this.m_ByteValue > 0;
			case TypeCode.Int16:
				return this.m_ShortValue != 0;
			case TypeCode.UInt16:
				return this.m_UShortValue > 0;
			case TypeCode.Int32:
				return this.m_IntValue != 0;
			case TypeCode.UInt32:
				return this.m_UIntValue > 0U;
			case TypeCode.Int64:
				return this.m_LongValue != 0L;
			case TypeCode.UInt64:
				return this.m_ULongValue > 0UL;
			case TypeCode.Single:
				return !Mathf.Approximately(this.m_FloatValue, 0f);
			case TypeCode.Double:
				return !NumberHelpers.Approximately(this.m_DoubleValue, 0.0);
			default:
				return false;
			}
		}

		public byte ToByte(IFormatProvider provider = null)
		{
			return (byte)this.ToInt64(provider);
		}

		public char ToChar(IFormatProvider provider = null)
		{
			TypeCode type = this.type;
			if (type == TypeCode.Char)
			{
				return this.m_CharValue;
			}
			if (type - TypeCode.Int16 > 5)
			{
				return '\0';
			}
			return (char)this.ToInt64(provider);
		}

		public DateTime ToDateTime(IFormatProvider provider = null)
		{
			throw new NotSupportedException("Converting PrimitiveValue to DateTime");
		}

		public decimal ToDecimal(IFormatProvider provider = null)
		{
			return new decimal(this.ToDouble(provider));
		}

		public double ToDouble(IFormatProvider provider = null)
		{
			switch (this.type)
			{
			case TypeCode.Boolean:
				if (this.m_BoolValue)
				{
					return 1.0;
				}
				return 0.0;
			case TypeCode.Char:
				return (double)this.m_CharValue;
			case TypeCode.SByte:
				return (double)this.m_SByteValue;
			case TypeCode.Byte:
				return (double)this.m_ByteValue;
			case TypeCode.Int16:
				return (double)this.m_ShortValue;
			case TypeCode.UInt16:
				return (double)this.m_UShortValue;
			case TypeCode.Int32:
				return (double)this.m_IntValue;
			case TypeCode.UInt32:
				return this.m_UIntValue;
			case TypeCode.Int64:
				return (double)this.m_LongValue;
			case TypeCode.UInt64:
				return this.m_ULongValue;
			case TypeCode.Single:
				return (double)this.m_FloatValue;
			case TypeCode.Double:
				return this.m_DoubleValue;
			default:
				return 0.0;
			}
		}

		public short ToInt16(IFormatProvider provider = null)
		{
			return (short)this.ToInt64(provider);
		}

		public int ToInt32(IFormatProvider provider = null)
		{
			return (int)this.ToInt64(provider);
		}

		public long ToInt64(IFormatProvider provider = null)
		{
			switch (this.type)
			{
			case TypeCode.Boolean:
				if (this.m_BoolValue)
				{
					return 1L;
				}
				return 0L;
			case TypeCode.Char:
				return (long)((ulong)this.m_CharValue);
			case TypeCode.SByte:
				return (long)this.m_SByteValue;
			case TypeCode.Byte:
				return (long)((ulong)this.m_ByteValue);
			case TypeCode.Int16:
				return (long)this.m_ShortValue;
			case TypeCode.UInt16:
				return (long)((ulong)this.m_UShortValue);
			case TypeCode.Int32:
				return (long)this.m_IntValue;
			case TypeCode.UInt32:
				return (long)((ulong)this.m_UIntValue);
			case TypeCode.Int64:
				return this.m_LongValue;
			case TypeCode.UInt64:
				return (long)this.m_ULongValue;
			case TypeCode.Single:
				return (long)this.m_FloatValue;
			case TypeCode.Double:
				return (long)this.m_DoubleValue;
			default:
				return 0L;
			}
		}

		public sbyte ToSByte(IFormatProvider provider = null)
		{
			return (sbyte)this.ToInt64(provider);
		}

		public float ToSingle(IFormatProvider provider = null)
		{
			return (float)this.ToDouble(provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return this.ToString();
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		public ushort ToUInt16(IFormatProvider provider = null)
		{
			return (ushort)this.ToUInt64(null);
		}

		public uint ToUInt32(IFormatProvider provider = null)
		{
			return (uint)this.ToUInt64(null);
		}

		public ulong ToUInt64(IFormatProvider provider = null)
		{
			switch (this.type)
			{
			case TypeCode.Boolean:
				if (this.m_BoolValue)
				{
					return 1UL;
				}
				return 0UL;
			case TypeCode.Char:
				return (ulong)this.m_CharValue;
			case TypeCode.SByte:
				return (ulong)((long)this.m_SByteValue);
			case TypeCode.Byte:
				return (ulong)this.m_ByteValue;
			case TypeCode.Int16:
				return (ulong)((long)this.m_ShortValue);
			case TypeCode.UInt16:
				return (ulong)this.m_UShortValue;
			case TypeCode.Int32:
				return (ulong)((long)this.m_IntValue);
			case TypeCode.UInt32:
				return (ulong)this.m_UIntValue;
			case TypeCode.Int64:
				return (ulong)this.m_LongValue;
			case TypeCode.UInt64:
				return this.m_ULongValue;
			case TypeCode.Single:
				return (ulong)this.m_FloatValue;
			case TypeCode.Double:
				return (ulong)this.m_DoubleValue;
			default:
				return 0UL;
			}
		}

		public object ToObject()
		{
			switch (this.m_Type)
			{
			case TypeCode.Boolean:
				return this.m_BoolValue;
			case TypeCode.Char:
				return this.m_CharValue;
			case TypeCode.SByte:
				return this.m_SByteValue;
			case TypeCode.Byte:
				return this.m_ByteValue;
			case TypeCode.Int16:
				return this.m_ShortValue;
			case TypeCode.UInt16:
				return this.m_UShortValue;
			case TypeCode.Int32:
				return this.m_IntValue;
			case TypeCode.UInt32:
				return this.m_UIntValue;
			case TypeCode.Int64:
				return this.m_LongValue;
			case TypeCode.UInt64:
				return this.m_ULongValue;
			case TypeCode.Single:
				return this.m_FloatValue;
			case TypeCode.Double:
				return this.m_DoubleValue;
			default:
				return null;
			}
		}

		public static PrimitiveValue From<TValue>(TValue value) where TValue : struct
		{
			Type type = typeof(TValue);
			if (type.IsEnum)
			{
				type = type.GetEnumUnderlyingType();
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return new PrimitiveValue(Convert.ToBoolean(value));
			case TypeCode.Char:
				return new PrimitiveValue(Convert.ToChar(value));
			case TypeCode.SByte:
				return new PrimitiveValue(Convert.ToSByte(value));
			case TypeCode.Byte:
				return new PrimitiveValue(Convert.ToByte(value));
			case TypeCode.Int16:
				return new PrimitiveValue(Convert.ToInt16(value));
			case TypeCode.UInt16:
				return new PrimitiveValue(Convert.ToUInt16(value));
			case TypeCode.Int32:
				return new PrimitiveValue(Convert.ToInt32(value));
			case TypeCode.UInt32:
				return new PrimitiveValue(Convert.ToUInt32(value));
			case TypeCode.Int64:
				return new PrimitiveValue(Convert.ToInt64(value));
			case TypeCode.UInt64:
				return new PrimitiveValue(Convert.ToUInt64(value));
			case TypeCode.Single:
				return new PrimitiveValue(Convert.ToSingle(value));
			case TypeCode.Double:
				return new PrimitiveValue(Convert.ToDouble(value));
			default:
				throw new ArgumentException(string.Format("Cannot convert value '{0}' of type '{1}' to PrimitiveValue", value, typeof(TValue).Name), "value");
			}
		}

		public static PrimitiveValue FromObject(object value)
		{
			if (value == null)
			{
				return default(PrimitiveValue);
			}
			string text = value as string;
			if (text != null)
			{
				return PrimitiveValue.FromString(text);
			}
			if (value is bool)
			{
				bool value2 = (bool)value;
				return new PrimitiveValue(value2);
			}
			if (value is char)
			{
				char value3 = (char)value;
				return new PrimitiveValue(value3);
			}
			if (value is byte)
			{
				byte value4 = (byte)value;
				return new PrimitiveValue(value4);
			}
			if (value is sbyte)
			{
				sbyte value5 = (sbyte)value;
				return new PrimitiveValue(value5);
			}
			if (value is short)
			{
				short value6 = (short)value;
				return new PrimitiveValue(value6);
			}
			if (value is ushort)
			{
				ushort value7 = (ushort)value;
				return new PrimitiveValue(value7);
			}
			if (value is int)
			{
				int value8 = (int)value;
				return new PrimitiveValue(value8);
			}
			if (value is uint)
			{
				uint value9 = (uint)value;
				return new PrimitiveValue(value9);
			}
			if (value is long)
			{
				long value10 = (long)value;
				return new PrimitiveValue(value10);
			}
			if (value is ulong)
			{
				ulong value11 = (ulong)value;
				return new PrimitiveValue(value11);
			}
			if (value is float)
			{
				float value12 = (float)value;
				return new PrimitiveValue(value12);
			}
			if (value is double)
			{
				double value13 = (double)value;
				return new PrimitiveValue(value13);
			}
			if (value is Enum)
			{
				switch (Type.GetTypeCode(value.GetType().GetEnumUnderlyingType()))
				{
				case TypeCode.SByte:
					return new PrimitiveValue((sbyte)value);
				case TypeCode.Byte:
					return new PrimitiveValue((byte)value);
				case TypeCode.Int16:
					return new PrimitiveValue((short)value);
				case TypeCode.UInt16:
					return new PrimitiveValue((ushort)value);
				case TypeCode.Int32:
					return new PrimitiveValue((int)value);
				case TypeCode.UInt32:
					return new PrimitiveValue((uint)value);
				case TypeCode.Int64:
					return new PrimitiveValue((long)value);
				case TypeCode.UInt64:
					return new PrimitiveValue((ulong)value);
				}
			}
			throw new ArgumentException(string.Format("Cannot convert '{0}' to primitive value", value), "value");
		}

		public static implicit operator PrimitiveValue(bool value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(char value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(byte value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(sbyte value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(short value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(ushort value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(int value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(uint value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(long value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(ulong value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(float value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(double value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromBoolean(bool value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromChar(char value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromByte(byte value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromSByte(sbyte value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromInt16(short value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromUInt16(ushort value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromInt32(int value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromUInt32(uint value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromInt64(long value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromUInt64(ulong value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromSingle(float value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromDouble(double value)
		{
			return new PrimitiveValue(value);
		}

		[FieldOffset(0)]
		private TypeCode m_Type;

		[FieldOffset(4)]
		private bool m_BoolValue;

		[FieldOffset(4)]
		private char m_CharValue;

		[FieldOffset(4)]
		private byte m_ByteValue;

		[FieldOffset(4)]
		private sbyte m_SByteValue;

		[FieldOffset(4)]
		private short m_ShortValue;

		[FieldOffset(4)]
		private ushort m_UShortValue;

		[FieldOffset(4)]
		private int m_IntValue;

		[FieldOffset(4)]
		private uint m_UIntValue;

		[FieldOffset(4)]
		private long m_LongValue;

		[FieldOffset(4)]
		private ulong m_ULongValue;

		[FieldOffset(4)]
		private float m_FloatValue;

		[FieldOffset(4)]
		private double m_DoubleValue;
	}
}
