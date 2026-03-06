using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class UriHandler : ITypeHandler
	{
		public UriHandler(SerializationContext context)
		{
			this.context = context;
		}

		private ITypeHandler GetStringHandler()
		{
			return this.stringHandler = (this.stringHandler ?? this.context.TypeHandlers.Get<string>());
		}

		public object Read(Format format, FormatReader reader)
		{
			if (format.IsNil)
			{
				return null;
			}
			return new Uri((string)this.GetStringHandler().Read(format, reader));
		}

		public void Write(object obj, FormatWriter writer)
		{
			if (obj == null)
			{
				writer.WriteNil();
				return;
			}
			string obj2 = ((Uri)obj).ToString();
			this.GetStringHandler().Write(obj2, writer);
		}

		private readonly SerializationContext context;

		private ITypeHandler stringHandler;
	}
}
