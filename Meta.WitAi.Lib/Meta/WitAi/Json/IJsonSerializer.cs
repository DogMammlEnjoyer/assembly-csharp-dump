using System;

namespace Meta.WitAi.Json
{
	public interface IJsonSerializer
	{
		bool SerializeObject(WitResponseClass jsonObject);
	}
}
