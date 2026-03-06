using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class DynamicNullableHandler : ITypeHandler
	{
		public DynamicNullableHandler(SerializationContext context, Type type)
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			this.underlyingTypeHandler = context.TypeHandlers.Get(underlyingType);
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsNil)
			{
				return null;
			}
			return this.underlyingTypeHandler.Read(format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			if (obj == null)
			{
				writer.WriteNil();
				return;
			}
			this.underlyingTypeHandler.Write(obj, writer);
		}

		private readonly ITypeHandler underlyingTypeHandler;
	}
}
