using System;
using UnityEngine;

namespace Meta.WitAi.Json
{
	public class ColorConverter : JsonConverter
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
			return typeof(Color) == objectType;
		}

		public override object ReadJson(WitResponseNode serializer, Type objectType, object existingValue)
		{
			Color color;
			if (ColorUtility.TryParseHtmlString(serializer.Value, out color))
			{
				return color;
			}
			return existingValue;
		}

		public override WitResponseNode WriteJson(object existingValue)
		{
			return new WitResponseData(ColorUtility.ToHtmlStringRGBA((Color)existingValue));
		}
	}
}
