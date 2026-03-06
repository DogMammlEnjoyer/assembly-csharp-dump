using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class QueryExpression
	{
		public QueryExpression(QueryOperator @operator)
		{
			this.Operator = @operator;
		}

		public bool IsMatch(JToken root, JToken t)
		{
			return this.IsMatch(root, t, null);
		}

		public abstract bool IsMatch(JToken root, JToken t, [Nullable(2)] JsonSelectSettings settings);

		internal QueryOperator Operator;
	}
}
