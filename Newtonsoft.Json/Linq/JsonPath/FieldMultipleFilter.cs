using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class FieldMultipleFilter : PathFilter
	{
		public FieldMultipleFilter(List<string> names)
		{
			this.Names = names;
		}

		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, [Nullable(2)] JsonSelectSettings settings)
		{
			foreach (JToken jtoken in current)
			{
				JObject o = jtoken as JObject;
				if (o != null)
				{
					foreach (string name in this.Names)
					{
						JToken jtoken2 = o[name];
						if (jtoken2 != null)
						{
							yield return jtoken2;
						}
						if (settings != null && settings.ErrorWhenNoMatch)
						{
							throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, name));
						}
						name = null;
					}
					List<string>.Enumerator enumerator2 = default(List<string>.Enumerator);
				}
				else if (settings != null && settings.ErrorWhenNoMatch)
				{
					throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", from n in this.Names
					select "'" + n + "'"), jtoken.GetType().Name));
				}
				o = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}

		internal List<string> Names;
	}
}
