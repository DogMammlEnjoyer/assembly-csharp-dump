using System;

namespace SouthPointe.Serialization.MessagePack
{
	public struct Format
	{
		public Format(byte value)
		{
			this.Value = value;
		}

		public bool IsPositiveFixInt
		{
			get
			{
				return this.Between(0, 127);
			}
		}

		public bool IsFixMap
		{
			get
			{
				return this.Between(128, 143);
			}
		}

		public bool IsFixArray
		{
			get
			{
				return this.Between(144, 159);
			}
		}

		public bool IsFixStr
		{
			get
			{
				return this.Between(160, 191);
			}
		}

		public bool IsNil
		{
			get
			{
				return this.Value == 192;
			}
		}

		public bool IsNeverUsed
		{
			get
			{
				return this.Value == 193;
			}
		}

		public bool IsFalse
		{
			get
			{
				return this.Value == 194;
			}
		}

		public bool IsTrue
		{
			get
			{
				return this.Value == 195;
			}
		}

		public bool IsBin8
		{
			get
			{
				return this.Value == 196;
			}
		}

		public bool IsBin16
		{
			get
			{
				return this.Value == 197;
			}
		}

		public bool IsBin32
		{
			get
			{
				return this.Value == 198;
			}
		}

		public bool IsExt8
		{
			get
			{
				return this.Value == 199;
			}
		}

		public bool IsExt16
		{
			get
			{
				return this.Value == 200;
			}
		}

		public bool IsExt32
		{
			get
			{
				return this.Value == 201;
			}
		}

		public bool IsFloat32
		{
			get
			{
				return this.Value == 202;
			}
		}

		public bool IsFloat64
		{
			get
			{
				return this.Value == 203;
			}
		}

		public bool IsUInt8
		{
			get
			{
				return this.Value == 204;
			}
		}

		public bool IsUInt16
		{
			get
			{
				return this.Value == 205;
			}
		}

		public bool IsUInt32
		{
			get
			{
				return this.Value == 206;
			}
		}

		public bool IsUInt64
		{
			get
			{
				return this.Value == 207;
			}
		}

		public bool IsInt8
		{
			get
			{
				return this.Value == 208;
			}
		}

		public bool IsInt16
		{
			get
			{
				return this.Value == 209;
			}
		}

		public bool IsInt32
		{
			get
			{
				return this.Value == 210;
			}
		}

		public bool IsInt64
		{
			get
			{
				return this.Value == 211;
			}
		}

		public bool IsFixExt1
		{
			get
			{
				return this.Value == 212;
			}
		}

		public bool IsFixExt2
		{
			get
			{
				return this.Value == 213;
			}
		}

		public bool IsFixExt4
		{
			get
			{
				return this.Value == 214;
			}
		}

		public bool IsFixExt8
		{
			get
			{
				return this.Value == 215;
			}
		}

		public bool IsFixExt16
		{
			get
			{
				return this.Value == 216;
			}
		}

		public bool IsStr8
		{
			get
			{
				return this.Value == 217;
			}
		}

		public bool IsStr16
		{
			get
			{
				return this.Value == 218;
			}
		}

		public bool IsStr32
		{
			get
			{
				return this.Value == 219;
			}
		}

		public bool IsArray16
		{
			get
			{
				return this.Value == 220;
			}
		}

		public bool IsArray32
		{
			get
			{
				return this.Value == 221;
			}
		}

		public bool IsMap16
		{
			get
			{
				return this.Value == 222;
			}
		}

		public bool IsMap32
		{
			get
			{
				return this.Value == 223;
			}
		}

		public bool IsNegativeFixInt
		{
			get
			{
				return this.Between(224, byte.MaxValue);
			}
		}

		public bool IsEmptyArray
		{
			get
			{
				return this.Value == 144;
			}
		}

		public bool IsIntFamily
		{
			get
			{
				return this.IsPositiveFixInt || this.IsNegativeFixInt || this.IsInt8 || this.IsUInt8 || this.IsInt16 || this.IsUInt16 || this.IsInt32 || this.IsUInt32 || this.IsInt64 || this.IsUInt64;
			}
		}

		public bool IsBoolFamily
		{
			get
			{
				return this.IsFalse || this.IsTrue;
			}
		}

		public bool IsFloatFamily
		{
			get
			{
				return this.IsFloat32 || this.IsFloat64;
			}
		}

		public bool IsStringFamily
		{
			get
			{
				return this.IsFixStr || this.IsStr8 || this.IsStr16 || this.IsStr32;
			}
		}

		public bool IsBinaryFamily
		{
			get
			{
				return this.IsBin8 || this.IsBin16 || this.IsBin32;
			}
		}

		public bool IsArrayFamily
		{
			get
			{
				return this.IsFixArray || this.IsArray16 || this.IsArray32;
			}
		}

		public bool IsMapFamily
		{
			get
			{
				return this.IsFixMap || this.IsMap16 || this.IsMap32;
			}
		}

		public bool IsExtFamily
		{
			get
			{
				return this.IsFixExt1 || this.IsFixExt2 || this.IsFixExt4 || this.IsFixExt8 || this.IsFixExt16 || this.IsExt8 || this.IsExt16 || this.IsExt32;
			}
		}

		private bool Between(byte min, byte max)
		{
			return this.Value >= min && this.Value <= max;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Format)
			{
				return this.Value == ((Format)obj).Value;
			}
			return obj is byte && this.Value == (byte)obj;
		}

		public static byte operator &(Format f1, byte value)
		{
			return f1.Value & value;
		}

		public static bool operator ==(Format f1, Format f2)
		{
			return f1.Value == f2.Value;
		}

		public static bool operator !=(Format f1, Format f2)
		{
			return f1.Value != f2.Value;
		}

		public override string ToString()
		{
			return "0x" + this.Value.ToString("X2");
		}

		public readonly byte Value;

		public const byte PositiveFixIntMin = 0;

		public const byte PositiveFixIntMax = 127;

		public const byte FixMapMin = 128;

		public const byte FixMapMax = 143;

		public const byte FixArrayMin = 144;

		public const byte FixArrayMax = 159;

		public const byte FixStrMin = 160;

		public const byte FixStrMax = 191;

		public const byte Nil = 192;

		public const byte NeverUsed = 193;

		public const byte False = 194;

		public const byte True = 195;

		public const byte Bin8 = 196;

		public const byte Bin16 = 197;

		public const byte Bin32 = 198;

		public const byte Ext8 = 199;

		public const byte Ext16 = 200;

		public const byte Ext32 = 201;

		public const byte Float32 = 202;

		public const byte Float64 = 203;

		public const byte UInt8 = 204;

		public const byte UInt16 = 205;

		public const byte UInt32 = 206;

		public const byte UInt64 = 207;

		public const byte Int8 = 208;

		public const byte Int16 = 209;

		public const byte Int32 = 210;

		public const byte Int64 = 211;

		public const byte FixExt1 = 212;

		public const byte FixExt2 = 213;

		public const byte FixExt4 = 214;

		public const byte FixExt8 = 215;

		public const byte FixExt16 = 216;

		public const byte Str8 = 217;

		public const byte Str16 = 218;

		public const byte Str32 = 219;

		public const byte Array16 = 220;

		public const byte Array32 = 221;

		public const byte Map16 = 222;

		public const byte Map32 = 223;

		public const byte NegativeFixIntMin = 224;

		public const byte NegativeFixIntMax = 255;
	}
}
