using System;

namespace Meta.WitAi.Json
{
	public class DateTimeConverter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(DateTime) == objectType;
		}

		public override object ReadJson(WitResponseNode serializer, Type objectType, object existingValue)
		{
			DateTime dateTime;
			if (DateTime.TryParse(serializer.Value, out dateTime))
			{
				return dateTime;
			}
			return existingValue;
		}

		public override WitResponseNode WriteJson(object existingValue)
		{
			DateTime dateTime = (DateTime)existingValue;
			return new WitResponseData(dateTime.ToLongDateString() + " " + dateTime.ToLongTimeString());
		}
	}
}
