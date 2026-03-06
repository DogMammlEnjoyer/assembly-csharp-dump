using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class TimeSpanHandler : ITypeHandler
	{
		public TimeSpanHandler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			this.longHandler = (this.longHandler ?? this.context.TypeHandlers.Get<long>());
			return new TimeSpan((long)this.longHandler.Read(format, reader));
		}

		public void Write(object obj, FormatWriter writer)
		{
			writer.Write(((TimeSpan)obj).Ticks);
		}

		private readonly SerializationContext context;

		private ITypeHandler longHandler;
	}
}
