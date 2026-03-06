using System;
using System.Collections.Generic;

namespace Valve.Newtonsoft.Json.Linq.JsonPath
{
	internal class ScanFilter : PathFilter
	{
		public string Name { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken root in current)
			{
				if (this.Name == null)
				{
					yield return root;
				}
				JToken value = root;
				JToken jtoken = root;
				for (;;)
				{
					if (jtoken != null && jtoken.HasValues)
					{
						value = jtoken.First;
					}
					else
					{
						while (value != null && value != root && value == value.Parent.Last)
						{
							value = value.Parent;
						}
						if (value == null || value == root)
						{
							break;
						}
						value = value.Next;
					}
					JProperty jproperty = value as JProperty;
					if (jproperty != null)
					{
						if (jproperty.Name == this.Name)
						{
							yield return jproperty.Value;
						}
					}
					else if (this.Name == null)
					{
						yield return value;
					}
					jtoken = (value as JContainer);
				}
				value = null;
				root = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}
	}
}
