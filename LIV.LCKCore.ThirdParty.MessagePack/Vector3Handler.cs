using System;
using UnityEngine;

namespace SouthPointe.Serialization.MessagePack
{
	public class Vector3Handler : ITypeHandler
	{
		public Vector3Handler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsArrayFamily)
			{
				this.floatHandler = (this.floatHandler ?? this.context.TypeHandlers.Get<float>());
				return new Vector3
				{
					x = (float)this.floatHandler.Read(reader.ReadFormat(), reader),
					y = (float)this.floatHandler.Read(reader.ReadFormat(), reader),
					z = (float)this.floatHandler.Read(reader.ReadFormat(), reader)
				};
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			Vector3 vector = (Vector3)obj;
			writer.WriteArrayHeader(3);
			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
		}

		private readonly SerializationContext context;

		private ITypeHandler floatHandler;
	}
}
