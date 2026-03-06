using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class DecimalHandler : ITypeHandler
	{
		public DecimalHandler(SerializationContext context)
		{
			this.context = context;
		}

		public object Read(Format format, FormatReader reader)
		{
			this.intArrayHandler = (this.intArrayHandler ?? this.context.TypeHandlers.Get<int[]>());
			return new decimal((int[])this.intArrayHandler.Read(format, reader));
		}

		public void Write(object obj, FormatWriter writer)
		{
			this.intArrayHandler = (this.intArrayHandler ?? this.context.TypeHandlers.Get<int[]>());
			int[] bits = decimal.GetBits((decimal)obj);
			this.intArrayHandler.Write(bits, writer);
		}

		private readonly SerializationContext context;

		private ITypeHandler intArrayHandler;
	}
}
