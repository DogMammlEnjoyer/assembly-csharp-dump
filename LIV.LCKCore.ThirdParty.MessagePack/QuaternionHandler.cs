using System;
using UnityEngine;

namespace SouthPointe.Serialization.MessagePack
{
	public class QuaternionHandler : ITypeHandler
	{
		public QuaternionHandler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsArrayFamily)
			{
				this.floatHandler = (this.floatHandler ?? this.context.TypeHandlers.Get<float>());
				return new Quaternion
				{
					x = (float)this.floatHandler.Read(reader.ReadFormat(), reader),
					y = (float)this.floatHandler.Read(reader.ReadFormat(), reader),
					z = (float)this.floatHandler.Read(reader.ReadFormat(), reader),
					w = (float)this.floatHandler.Read(reader.ReadFormat(), reader)
				};
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			Quaternion quaternion = (Quaternion)obj;
			writer.WriteArrayHeader(4);
			writer.Write(quaternion.x);
			writer.Write(quaternion.y);
			writer.Write(quaternion.z);
			writer.Write(quaternion.w);
		}

		private readonly SerializationContext context;

		private ITypeHandler floatHandler;
	}
}
