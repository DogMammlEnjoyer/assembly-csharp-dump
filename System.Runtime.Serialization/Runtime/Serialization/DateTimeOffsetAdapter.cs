using System;
using System.Globalization;
using System.Xml;

namespace System.Runtime.Serialization
{
	[DataContract(Name = "DateTimeOffset", Namespace = "http://schemas.datacontract.org/2004/07/System")]
	internal struct DateTimeOffsetAdapter
	{
		public DateTimeOffsetAdapter(DateTime dateTime, short offsetMinutes)
		{
			this.utcDateTime = dateTime;
			this.offsetMinutes = offsetMinutes;
		}

		[DataMember(Name = "DateTime", IsRequired = true)]
		public DateTime UtcDateTime
		{
			get
			{
				return this.utcDateTime;
			}
			set
			{
				this.utcDateTime = value;
			}
		}

		[DataMember(Name = "OffsetMinutes", IsRequired = true)]
		public short OffsetMinutes
		{
			get
			{
				return this.offsetMinutes;
			}
			set
			{
				this.offsetMinutes = value;
			}
		}

		public static DateTimeOffset GetDateTimeOffset(DateTimeOffsetAdapter value)
		{
			DateTimeOffset result;
			try
			{
				if (value.UtcDateTime.Kind == DateTimeKind.Unspecified)
				{
					result = new DateTimeOffset(value.UtcDateTime, new TimeSpan(0, (int)value.OffsetMinutes, 0));
				}
				else
				{
					DateTimeOffset dateTimeOffset = new DateTimeOffset(value.UtcDateTime);
					result = dateTimeOffset.ToOffset(new TimeSpan(0, (int)value.OffsetMinutes, 0));
				}
			}
			catch (ArgumentException exception)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value.ToString(CultureInfo.InvariantCulture), "DateTimeOffset", exception));
			}
			return result;
		}

		public static DateTimeOffsetAdapter GetDateTimeOffsetAdapter(DateTimeOffset value)
		{
			return new DateTimeOffsetAdapter(value.UtcDateTime, (short)value.Offset.TotalMinutes);
		}

		public string ToString(IFormatProvider provider)
		{
			return "DateTime: " + this.UtcDateTime.ToString() + ", Offset: " + this.OffsetMinutes.ToString();
		}

		private DateTime utcDateTime;

		private short offsetMinutes;
	}
}
