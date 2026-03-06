using System;
using System.Collections.Generic;
using System.Globalization;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Linq.JsonPath
{
	internal class ArrayIndexFilter : PathFilter
	{
		public int? Index { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken t in current)
			{
				if (this.Index != null)
				{
					JToken tokenIndex = PathFilter.GetTokenIndex(t, errorWhenNoMatch, this.Index.GetValueOrDefault());
					if (tokenIndex != null)
					{
						yield return tokenIndex;
					}
				}
				else if (t is JArray || t is JConstructor)
				{
					foreach (JToken jtoken in ((IEnumerable<JToken>)t))
					{
						yield return jtoken;
					}
					IEnumerator<JToken> enumerator2 = null;
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, t.GetType().Name));
				}
				t = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}
	}
}
