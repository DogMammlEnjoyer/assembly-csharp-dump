using System;

namespace SouthPointe.Serialization.MessagePack
{
	public class SerializationContext
	{
		public static SerializationContext Default
		{
			get
			{
				return SerializationContext.defaultContext = (SerializationContext.defaultContext ?? new SerializationContext());
			}
		}

		public SerializationContext()
		{
			this.DateTimeOptions = new DateTimeOptions();
			this.EnumOptions = new EnumOptions();
			this.ArrayOptions = new ArrayOptions();
			this.MapOptions = new MapOptions();
			this.JsonOptions = new JsonOptions();
			this.TypeHandlers = new TypeHandlers(this);
		}

		private static SerializationContext defaultContext;

		public readonly DateTimeOptions DateTimeOptions;

		public readonly EnumOptions EnumOptions;

		public readonly ArrayOptions ArrayOptions;

		public readonly MapOptions MapOptions;

		public readonly JsonOptions JsonOptions;

		public readonly TypeHandlers TypeHandlers;
	}
}
