using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	public readonly struct Pagination<T>
	{
		[JsonConstructor]
		internal Pagination(T data, long resultCount, long resultOffset, long resultLimit, long resultTotal)
		{
			this.Data = data;
			this.ResultCount = resultCount;
			this.ResultOffset = resultOffset;
			this.ResultLimit = resultLimit;
			this.ResultTotal = resultTotal;
		}

		public readonly T Data;

		public readonly long ResultCount;

		public readonly long ResultOffset;

		public readonly long ResultLimit;

		public readonly long ResultTotal;
	}
}
