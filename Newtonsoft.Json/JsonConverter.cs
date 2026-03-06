using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class JsonConverter
	{
		public abstract void WriteJson(JsonWriter writer, [Nullable(2)] object value, JsonSerializer serializer);

		[return: Nullable(2)]
		public abstract object ReadJson(JsonReader reader, Type objectType, [Nullable(2)] object existingValue, JsonSerializer serializer);

		public abstract bool CanConvert(Type objectType);

		public virtual bool CanRead
		{
			get
			{
				return true;
			}
		}

		public virtual bool CanWrite
		{
			get
			{
				return true;
			}
		}
	}
}
