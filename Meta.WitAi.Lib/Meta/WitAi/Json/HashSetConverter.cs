using System;
using System.Collections.Generic;

namespace Meta.WitAi.Json
{
	public class HashSetConverter<T> : JsonConverter
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
			return typeof(HashSet<T>) == objectType;
		}

		public override object ReadJson(WitResponseNode serializer, Type objectType, object existingValue)
		{
			WitResponseArray asArray = serializer.AsArray;
			HashSet<T> hashSet = new HashSet<T>();
			foreach (object obj in asArray)
			{
				WitResponseNode witResponseNode = (WitResponseNode)obj;
				hashSet.Add(witResponseNode.Cast<T>(default(T)));
			}
			return hashSet;
		}

		public override WitResponseNode WriteJson(object existingValue)
		{
			WitResponseArray witResponseArray = new WitResponseArray();
			HashSet<T> hashSet = existingValue as HashSet<T>;
			if (hashSet == null)
			{
				return witResponseArray;
			}
			foreach (T t in hashSet)
			{
				witResponseArray.Add(new WitResponseData(t.ToString()));
			}
			return witResponseArray;
		}
	}
}
