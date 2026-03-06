using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class DynamicEnumHandler : ITypeHandler
	{
		public DynamicEnumHandler(SerializationContext context, Type type)
		{
			this.context = context;
			this.type = type;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsIntFamily)
			{
				this.intHandler = (this.intHandler ?? this.context.TypeHandlers.Get<int>());
				return Enum.ToObject(this.type, this.intHandler.Read(format, reader));
			}
			if (format.IsStringFamily)
			{
				this.stringHandler = (this.stringHandler ?? this.context.TypeHandlers.Get<string>());
				return Enum.Parse(this.type, (string)this.stringHandler.Read(format, reader), true);
			}
			if (format.IsNil)
			{
				return Enum.ToObject(this.type, 0);
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			EnumPackingFormat packingFormat = this.context.EnumOptions.PackingFormat;
			if (packingFormat == EnumPackingFormat.Integer)
			{
				this.intHandler = (this.intHandler ?? this.context.TypeHandlers.Get<int>());
				this.intHandler.Write(obj, writer);
				return;
			}
			if (packingFormat != EnumPackingFormat.String)
			{
				return;
			}
			this.stringHandler = (this.stringHandler ?? this.context.TypeHandlers.Get<string>());
			this.stringHandler.Write(obj.ToString(), writer);
		}

		private readonly SerializationContext context;

		private readonly Type type;

		private ITypeHandler intHandler;

		private ITypeHandler stringHandler;
	}
}
