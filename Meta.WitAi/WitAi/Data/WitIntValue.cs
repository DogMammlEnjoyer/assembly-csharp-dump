using System;
using Meta.WitAi.Json;

namespace Meta.WitAi.Data
{
	public class WitIntValue : WitValue
	{
		public override object GetValue(WitResponseNode response)
		{
			return this.GetIntValue(response);
		}

		public override bool Equals(WitResponseNode response, object value)
		{
			int num = 0;
			if (value is int)
			{
				int num2 = (int)value;
				num = num2;
			}
			else if (value != null && !int.TryParse(((value != null) ? value.ToString() : null) ?? "", out num))
			{
				return false;
			}
			return this.GetIntValue(response) == num;
		}

		public int GetIntValue(WitResponseNode response)
		{
			return base.Reference.GetIntValue(response);
		}
	}
}
