using System;
using UnityEngine;

namespace SouthPointe.Serialization.MessagePack
{
	public class Vector2IntHandler : ITypeHandler
	{
		public Vector2IntHandler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsArrayFamily)
			{
				this.intHandler = (this.intHandler ?? this.context.TypeHandlers.Get<int>());
				return new Vector2Int
				{
					x = (int)this.intHandler.Read(reader.ReadFormat(), reader),
					y = (int)this.intHandler.Read(reader.ReadFormat(), reader)
				};
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			Vector2Int vector2Int = (Vector2Int)obj;
			writer.WriteArrayHeader(2);
			writer.Write(vector2Int.x);
			writer.Write(vector2Int.y);
		}

		private readonly SerializationContext context;

		private ITypeHandler intHandler;
	}
}
