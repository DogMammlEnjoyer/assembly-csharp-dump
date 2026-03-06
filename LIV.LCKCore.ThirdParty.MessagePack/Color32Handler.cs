using System;
using System.Collections.Generic;
using UnityEngine;

namespace SouthPointe.Serialization.MessagePack
{
	public class Color32Handler : ITypeHandler
	{
		public Color32Handler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsBinaryFamily)
			{
				byte[] array = reader.ReadBin8();
				return new Color32(array[0], array[1], array[2], array[3]);
			}
			if (format.IsArrayFamily)
			{
				this.byteHandler = (this.byteHandler ?? this.context.TypeHandlers.Get<byte>());
				int num = reader.ReadArrayLength(format);
				byte[] array2 = new byte[num];
				for (int i = 0; i < num; i++)
				{
					array2[i] = (byte)this.byteHandler.Read(reader.ReadFormat(), reader);
				}
				return new Color32(array2[0], array2[1], array2[2], array2[3]);
			}
			if (format.IsStringFamily)
			{
				this.stringHandler = (this.stringHandler ?? this.context.TypeHandlers.Get<string>());
				Color c;
				ColorUtility.TryParseHtmlString((string)this.stringHandler.Read(format, reader), out c);
				return c;
			}
			if (format.IsMapFamily)
			{
				this.mapHandler = (this.mapHandler ?? this.context.TypeHandlers.Get<Dictionary<string, byte>>());
				Dictionary<string, byte> dictionary = (Dictionary<string, byte>)this.mapHandler.Read(format, reader);
				return new Color32(dictionary["r"], dictionary["g"], dictionary["b"], dictionary["a"]);
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			Color32 color = (Color32)obj;
			writer.WriteBinHeader(4);
			writer.WriteRawByte(color.r);
			writer.WriteRawByte(color.g);
			writer.WriteRawByte(color.b);
			writer.WriteRawByte(color.a);
		}

		private readonly SerializationContext context;

		private ITypeHandler byteHandler;

		private ITypeHandler stringHandler;

		private ITypeHandler mapHandler;
	}
}
