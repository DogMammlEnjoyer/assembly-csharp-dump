using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class GuidHandler : ITypeHandler
	{
		public GuidHandler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsBin8)
			{
				this.binaryHandler = (this.binaryHandler ?? this.context.TypeHandlers.Get<byte[]>());
				return new Guid((byte[])this.binaryHandler.Read(format, reader));
			}
			if (format.IsStr8)
			{
				this.stringHandler = (this.stringHandler ?? this.context.TypeHandlers.Get<string>());
				return new Guid((string)this.stringHandler.Read(format, reader));
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			this.binaryHandler = (this.binaryHandler ?? this.context.TypeHandlers.Get<byte[]>());
			this.binaryHandler.Write(((Guid)obj).ToByteArray(), writer);
		}

		private readonly SerializationContext context;

		private ITypeHandler binaryHandler;

		private ITypeHandler stringHandler;
	}
}
