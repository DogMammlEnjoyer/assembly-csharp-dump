using System;
using Meta.WitAi.Json;

namespace Meta.WitAi.Data
{
	public class WitStringValue : WitValue
	{
		public override object GetValue(WitResponseNode response)
		{
			return this.GetStringValue(response);
		}

		public override bool Equals(WitResponseNode response, object value)
		{
			string text = value as string;
			if (text != null)
			{
				return this.GetStringValue(response) == text;
			}
			return (((value != null) ? value.ToString() : null) ?? "") == this.GetStringValue(response);
		}

		public string GetStringValue(WitResponseNode response)
		{
			return base.Reference.GetStringValue(response);
		}
	}
}
