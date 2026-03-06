using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	public abstract class DateTimeConverterBase : JsonConverter
	{
		[NullableContext(1)]
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTime) || objectType == typeof(DateTime?) || (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?));
		}
	}
}
