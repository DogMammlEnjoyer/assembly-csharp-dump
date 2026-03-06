using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class JsonFormatterConverter : IFormatterConverter
	{
		public JsonFormatterConverter(JsonSerializerInternalReader reader, JsonISerializableContract contract, [Nullable(2)] JsonProperty member)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			ValidationUtils.ArgumentNotNull(contract, "contract");
			this._reader = reader;
			this._contract = contract;
			this._member = member;
		}

		private T GetTokenValue<[Nullable(2)] T>(object value)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			return (T)((object)System.Convert.ChangeType(((JValue)value).Value, typeof(T), CultureInfo.InvariantCulture));
		}

		public object Convert(object value, Type type)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			JToken jtoken = value as JToken;
			if (jtoken == null)
			{
				throw new ArgumentException("Value is not a JToken.", "value");
			}
			return this._reader.CreateISerializableItem(jtoken, type, this._contract, this._member);
		}

		public object Convert(object value, TypeCode typeCode)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			JValue jvalue = value as JValue;
			return System.Convert.ChangeType((jvalue != null) ? jvalue.Value : value, typeCode, CultureInfo.InvariantCulture);
		}

		public bool ToBoolean(object value)
		{
			return this.GetTokenValue<bool>(value);
		}

		public byte ToByte(object value)
		{
			return this.GetTokenValue<byte>(value);
		}

		public char ToChar(object value)
		{
			return this.GetTokenValue<char>(value);
		}

		public DateTime ToDateTime(object value)
		{
			return this.GetTokenValue<DateTime>(value);
		}

		public decimal ToDecimal(object value)
		{
			return this.GetTokenValue<decimal>(value);
		}

		public double ToDouble(object value)
		{
			return this.GetTokenValue<double>(value);
		}

		public short ToInt16(object value)
		{
			return this.GetTokenValue<short>(value);
		}

		public int ToInt32(object value)
		{
			return this.GetTokenValue<int>(value);
		}

		public long ToInt64(object value)
		{
			return this.GetTokenValue<long>(value);
		}

		public sbyte ToSByte(object value)
		{
			return this.GetTokenValue<sbyte>(value);
		}

		public float ToSingle(object value)
		{
			return this.GetTokenValue<float>(value);
		}

		public string ToString(object value)
		{
			return this.GetTokenValue<string>(value);
		}

		public ushort ToUInt16(object value)
		{
			return this.GetTokenValue<ushort>(value);
		}

		public uint ToUInt32(object value)
		{
			return this.GetTokenValue<uint>(value);
		}

		public ulong ToUInt64(object value)
		{
			return this.GetTokenValue<ulong>(value);
		}

		private readonly JsonSerializerInternalReader _reader;

		private readonly JsonISerializableContract _contract;

		[Nullable(2)]
		private readonly JsonProperty _member;
	}
}
