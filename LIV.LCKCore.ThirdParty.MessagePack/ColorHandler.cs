using System;
using System.Collections.Generic;
using UnityEngine;

namespace SouthPointe.Serialization.MessagePack
{
	public class ColorHandler : ITypeHandler
	{
		public ColorHandler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsArrayFamily)
			{
				this.floatHandler = (this.floatHandler ?? this.context.TypeHandlers.Get<float>());
				int num = reader.ReadArrayLength(format);
				float[] array = new float[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = (float)this.floatHandler.Read(reader.ReadFormat(), reader);
				}
				return new Color(array[0], array[1], array[2], array[3]);
			}
			if (format.IsStringFamily)
			{
				this.stringHandler = (this.stringHandler ?? this.context.TypeHandlers.Get<string>());
				Color color;
				ColorUtility.TryParseHtmlString((string)this.stringHandler.Read(format, reader), out color);
				return color;
			}
			if (format.IsMapFamily)
			{
				this.mapHandler = (this.mapHandler ?? this.context.TypeHandlers.Get<Dictionary<string, float>>());
				Dictionary<string, float> dictionary = (Dictionary<string, float>)this.mapHandler.Read(format, reader);
				return new Color(dictionary["r"], dictionary["g"], dictionary["b"], dictionary["a"]);
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			Color color = (Color)obj;
			writer.WriteArrayHeader(4);
			writer.Write(color.r);
			writer.Write(color.g);
			writer.Write(color.b);
			writer.Write(color.a);
		}

		private readonly SerializationContext context;

		private ITypeHandler floatHandler;

		private ITypeHandler stringHandler;

		private ITypeHandler mapHandler;
	}
}
