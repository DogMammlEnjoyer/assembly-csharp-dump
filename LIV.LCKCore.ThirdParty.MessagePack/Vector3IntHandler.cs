using System;
using UnityEngine;

namespace SouthPointe.Serialization.MessagePack
{
	public class Vector3IntHandler : ITypeHandler
	{
		public Vector3IntHandler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsArrayFamily)
			{
				this.intHandler = (this.intHandler ?? this.context.TypeHandlers.Get<int>());
				return new Vector3Int
				{
					x = (int)this.intHandler.Read(reader.ReadFormat(), reader),
					y = (int)this.intHandler.Read(reader.ReadFormat(), reader),
					z = (int)this.intHandler.Read(reader.ReadFormat(), reader)
				};
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			Vector3Int vector3Int = (Vector3Int)obj;
			writer.WriteArrayHeader(2);
			writer.Write(vector3Int.x);
			writer.Write(vector3Int.y);
			writer.Write(vector3Int.z);
		}

		private readonly SerializationContext context;

		private ITypeHandler intHandler;
	}
}
