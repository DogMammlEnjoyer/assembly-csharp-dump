using System;
using System.Collections;
using System.Collections.Generic;

namespace SouthPointe.Serialization.MessagePack
{
	public class DynamicListHandler : ITypeHandler
	{
		public DynamicListHandler(SerializationContext context, Type type)
		{
			this.context = context;
			this.innerType = type.GetGenericArguments()[0];
			this.innerTypeHandler = context.TypeHandlers.Get(this.innerType);
		}

		public object Read(Format format, FormatReader reader)
		{
			Type type = typeof(List<>).MakeGenericType(new Type[]
			{
				this.innerType
			});
			if (format.IsArrayFamily)
			{
				IList list = (IList)Activator.CreateInstance(type);
				int num = reader.ReadArrayLength(format);
				for (int i = 0; i < num; i++)
				{
					list.Add(this.innerTypeHandler.Read(reader.ReadFormat(), reader));
				}
				return list;
			}
			if (!format.IsNil)
			{
				throw new FormatException(this, format, reader);
			}
			if (this.context.ArrayOptions.NullAsEmptyOnUnpack)
			{
				return Activator.CreateInstance(type);
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
			IList list = (IList)obj;
			writer.WriteArrayHeader(list.Count);
			foreach (object obj2 in list)
			{
				this.innerTypeHandler.Write(obj2, writer);
			}
		}

		private readonly SerializationContext context;

		private readonly Type innerType;

		private readonly ITypeHandler innerTypeHandler;
	}
}
