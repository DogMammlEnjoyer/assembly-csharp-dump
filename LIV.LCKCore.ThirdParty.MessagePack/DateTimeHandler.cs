using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class DateTimeHandler : IExtTypeHandler, ITypeHandler
	{
		public sbyte ExtType
		{
			get
			{
				return -1;
			}
		}

		public DateTimeHandler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsExtFamily)
			{
				uint length = reader.ReadExtLength(format);
				if (this.ExtType == reader.ReadExtType(reader.ReadFormat()))
				{
					return this.ReadExt(length, reader);
				}
			}
			if (format.IsStringFamily)
			{
				this.stringHandler = (this.stringHandler ?? this.context.TypeHandlers.Get<string>());
				return DateTime.Parse((string)this.stringHandler.Read(format, reader));
			}
			if (format.IsFloatFamily || format.IsIntFamily)
			{
				this.doubleHandler = (this.doubleHandler ?? this.context.TypeHandlers.Get<double>());
				double value = (double)this.doubleHandler.Read(format, reader);
				return DateTimeHandler.epoch.AddSeconds(value).ToLocalTime();
			}
			throw new FormatException(this, format, reader);
		}

		public object ReadExt(uint length, FormatReader reader)
		{
			if (length == 4U)
			{
				return DateTimeHandler.epoch.AddSeconds(reader.ReadUInt32()).ToLocalTime();
			}
			if (length == 8U)
			{
				byte[] array = reader.ReadBytesOfLength(8);
				uint num = (uint)((int)array[0] << 22 | (int)array[1] << 14 | (int)array[2] << 6 | (int)((uint)array[3] >> 2));
				ulong num2 = (ulong)((long)(array[3] & 3) << 32 | (long)((long)((ulong)array[4]) << 24) | (long)((long)((ulong)array[5]) << 16) | (long)((long)((ulong)array[6]) << 8) | (long)((ulong)array[7]));
				return DateTimeHandler.epoch.AddTicks((long)((ulong)(num / 100U))).AddSeconds(num2).ToLocalTime();
			}
			if (length == 12U)
			{
				return DateTimeHandler.epoch.AddTicks((long)((ulong)(reader.ReadUInt32() / 100U))).AddSeconds((double)reader.ReadInt64()).ToLocalTime();
			}
			throw new FormatException();
		}

		public void Write(object obj, FormatWriter writer)
		{
			DateTime dateTime = (DateTime)obj;
			switch (this.context.DateTimeOptions.PackingFormat)
			{
			case DateTimePackingFormat.Extension:
			{
				TimeSpan timeSpan = dateTime.ToUniversalTime() - DateTimeHandler.epoch;
				writer.WriteExtHeader(12U, this.ExtType);
				writer.WriteUInt32((uint)(dateTime.Ticks % 10000000L) * 100U);
				writer.WriteUInt64((ulong)timeSpan.TotalSeconds);
				return;
			}
			case DateTimePackingFormat.String:
				writer.Write(dateTime.ToString("o"));
				return;
			case DateTimePackingFormat.Epoch:
				writer.Write((dateTime.ToUniversalTime() - DateTimeHandler.epoch).TotalSeconds);
				return;
			default:
				throw new FormatException();
			}
		}

		private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private readonly SerializationContext context;

		private ITypeHandler stringHandler;

		private ITypeHandler doubleHandler;
	}
}
