using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class DynamicArrayHandler : ITypeHandler
	{
		public DynamicArrayHandler(SerializationContext context, Type type)
		{
			this.context = context;
			this.elementType = type.GetElementType();
			this.elementTypeHandler = context.TypeHandlers.Get(this.elementType);
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsArrayFamily)
			{
				int num = reader.ReadArrayLength(format);
				Array array = Array.CreateInstance(this.elementType, num);
				for (int i = 0; i < num; i++)
				{
					object value = this.elementTypeHandler.Read(reader.ReadFormat(), reader);
					array.SetValue(value, i);
				}
				return array;
			}
			if (!format.IsNil)
			{
				throw new FormatException(this, format, reader);
			}
			if (this.context.ArrayOptions.NullAsEmptyOnUnpack)
			{
				return Array.CreateInstance(this.elementType, 0);
			}
			return null;
		}

		public void Write(object obj, FormatWriter writer)
		{
			if (obj == null)
			{
				writer.WriteNil();
				return;
			}
			Array array = (Array)obj;
			writer.WriteArrayHeader(array.Length);
			foreach (object obj2 in array)
			{
				this.elementTypeHandler.Write(obj2, writer);
			}
		}

		private readonly SerializationContext context;

		private readonly Type elementType;

		private readonly ITypeHandler elementTypeHandler;
	}
}
